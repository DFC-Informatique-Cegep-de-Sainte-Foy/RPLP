﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RPLP.DAL.DTO.Json;
using RPLP.DAL.SQL.Depots;
using RPLP.ENTITES;
using RPLP.MVC.Models;
using RPLP.SERVICES.Github;
using RPLP.SERVICES.InterfacesDepots;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Net.Http;

namespace RPLP.MVC.Controllers
{
    public class RPLPController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IDepotOrganisation _depotOrganisation = new DepotOrganisation();
        private readonly IDepotClassroom _depotClassroom = new DepotClassroom();
        private readonly IDepotTeacher _depotTeacher = new DepotTeacher();
        private readonly IDepotAdministrator _depotAdministrator = new DepotAdministrator();
        private readonly ScriptGithubRPLP _scriptGithub;

        public RPLPController()
        {
            this._httpClient = new HttpClient();
            this._httpClient.BaseAddress = new Uri("http://rplp.api/api/");
            this._httpClient.DefaultRequestHeaders.Accept
                .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            string token = "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";
            GithubApiAction _githubAction = new GithubApiAction(token);
            _scriptGithub = new ScriptGithubRPLP(new DepotClassroom(), new DepotRepository(), new DepotOrganisation(), token);

            this._httpClient = new HttpClient();
            this._httpClient.BaseAddress = new Uri("http://rplp.api/api/");
            this._httpClient.DefaultRequestHeaders.Accept
                .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            this._httpClient.DefaultRequestHeaders.Accept
               .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/octet-stream"));
        }

        [Authorize]
        public IActionResult Index()
        {
            PeerReviewViewModel model = new PeerReviewViewModel();
            List<Organisation>? organisations = new List<Organisation>();


            string? email = User.FindFirst(u => u.Type == ClaimTypes.Email)?.Value;
            string? userType = this._httpClient
                .GetFromJsonAsync<string>($"Verificator/UserType/{email}")
                .Result;

            if (userType == typeof(Administrator).ToString())
            {
                organisations = this._httpClient
                    .GetFromJsonAsync<List<Organisation>>($"Administrator/Email/{email}/Organisations")
                    .Result;
            }

            else if (userType == typeof(Teacher).ToString())
            {
                organisations = this._httpClient
                    .GetFromJsonAsync<List<Organisation>>($"Teacher/Email/{email}/Organisations")
                    .Result;
            }

            organisations.ForEach(o => model.Organisations.Add(new OrganisationViewModel(o.Name)));

            return View(model);
        }

        private List<OrganisationViewModel> GetOrganisations()
        {
            List<OrganisationViewModel> organisations = new List<OrganisationViewModel>();
            List<Organisation>? databaseOrganisations = this._httpClient
                .GetFromJsonAsync<List<Organisation>>("Organisation")
                .Result;

            foreach (Organisation org in databaseOrganisations)
            {
                organisations.Add(new OrganisationViewModel(org.Name));
            }

            return organisations;
        }

        [HttpGet]
        public ActionResult<List<ClassroomViewModel>> GetClassroomsOfOrganisationByName(string organisationName)
        {
            List<ClassroomViewModel> classes = new List<ClassroomViewModel>();
            string email = User.FindFirst(u => u.Type == ClaimTypes.Email)?.Value;
            string? userType = this._httpClient
                .GetFromJsonAsync<string>($"Verificator/UserType/{email}")
                .Result;

            if (userType == typeof(Administrator).ToString())
            {
                List<Classroom>? databaseClasses = this._httpClient
                    .GetFromJsonAsync<List<Classroom>>($"Classroom/Organisation/{organisationName}")
                    .Result;
                foreach (Classroom classroom in databaseClasses)
                {
                    classes.Add(new ClassroomViewModel(classroom.Name));
                }
            }

            else if (userType == typeof(Teacher).ToString())
            {
                Teacher? teacher = this._httpClient
                    .GetFromJsonAsync<Teacher>($"Teacher/Email/{email}")
                    .Result;

                classes = GetClassroomsOfTeacherInOrganisation(teacher.Username, organisationName);
            }

            return classes;
        }

        public List<ClassroomViewModel> GetClassroomsOfTeacherInOrganisation(string p_teacherUsername, string p_organisationName)
        {
            var classes = new List<ClassroomViewModel>();
            string? teacherEmail = User.FindFirst(c => c.Type == ClaimTypes.Email)?.Value;

            List<Classroom>? databaseClasses = this._httpClient
                .GetFromJsonAsync<List<Classroom>>($"Teacher/Email/{teacherEmail}/Organisation/{p_organisationName}/Classrooms")
                .Result;

            foreach (Classroom classroom in databaseClasses)
            {
                classes.Add(new ClassroomViewModel(classroom.Name));
            }

            return classes;
        }

        [HttpGet]
        public ActionResult<List<AssignmentViewModel>> GetAssignmentsOfClassroomByName(string classroomName)
        {
            List<AssignmentViewModel> assignments = new List<AssignmentViewModel>();
            List<Assignment>? databaseAssignments = this._httpClient
                .GetFromJsonAsync<List<Assignment>>($"Assignment/Classroom/{classroomName}/Assignments")
                .Result;

            foreach (Assignment assignment in databaseAssignments)
            {
                assignments.Add(new AssignmentViewModel(assignment.Name));
            }

            return assignments;
        }

        [HttpGet]
        public ActionResult StartStudentAssignationScript(string organisationName, string classroomName, string assignmentName, int numberOfReviews)
        {
            try
            {
                _scriptGithub.ScriptAssignStudentToAssignmentReview(organisationName, classroomName, assignmentName, numberOfReviews);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet]
        public ActionResult StartTeachertAssignationScript(string organisationName, string classroomName, string assignmentName)
        {
            try
            {
                _scriptGithub.ScriptAssignTeacherToAssignmentReview(organisationName, classroomName, assignmentName);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> DownloadCommentsOfPullRequestByAssignment(string organisationName, string classroomName, string assignmentName)
        {
            Stream stream;
            FileStreamResult fileStreamResult;

            try
            {
                stream = await this._httpClient.GetStreamAsync($"Github/{organisationName}/{classroomName}/{assignmentName}/PullRequests/Comments/File");
                fileStreamResult = new FileStreamResult(stream, "application/octet-stream");
                fileStreamResult.FileDownloadName = $"Comments_{assignmentName}_{DateTime.Now}.json";
            }
            catch (Exception)
            {
                return NotFound("Un ou plusieurs dépôts n'ont pas pu être trouvés. Il est peut-être privé et inaccessible à l'utilisateur.");
            }

            return fileStreamResult;
        }
    }

}
