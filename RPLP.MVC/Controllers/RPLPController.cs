using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RPLP.DAL.SQL.Depots;
using RPLP.ENTITES;
using RPLP.MVC.Models;
using RPLP.SERVICES.Github;
using RPLP.SERVICES.InterfacesDepots;
using System.Collections.Generic;
using System.Security.Claims;

namespace RPLP.MVC.Controllers
{
    public class RPLPController : Controller
    {
        private readonly IDepotOrganisation _depotOrganisation = new DepotOrganisation();
        private readonly IDepotClassroom _depotClassroom = new DepotClassroom();
        private readonly IDepotTeacher _depotTeacher = new DepotTeacher();
        private readonly IDepotAdministrator _depotAdministrator = new DepotAdministrator();
        private readonly VerificatorForDepot verificator = new VerificatorForDepot();

        public RPLPController()
        {
            string token = "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";
            GithubApiAction _githubAction = new GithubApiAction(token);
            _scriptGithub = new ScriptGithubRPLP(new DepotClassroom(), new DepotRepository(), token);
        }

        public IActionResult Index()
        {
            PeerReviewViewModel model = new PeerReviewViewModel();
            List<Organisation> organisations = new List<Organisation>();

            string email = User.FindFirst(u => u.Type == ClaimTypes.Email)?.Value;
            Type userType = this.verificator.GetUserTypeByEmail(email);

            if (userType == typeof(Administrator))
            {
                Administrator administrator = this._depotAdministrator.GetAdministratorByEmail(email);
                organisations = this._depotAdministrator.GetAdminOrganisations(administrator.Username);
            }

            else if (userType == typeof(Teacher))
            {
                Teacher teacher = this._depotTeacher.GetTeacherByEmail(email);
                organisations = _depotTeacher.GetTeacherOrganisations(teacher.Username);
            }

            organisations.ForEach(o => model.Organisations.Add(new OrganisationViewModel(o.Name)));

            return View(model);
        }

        private List<OrganisationViewModel> GetOrganisations()
        {
            List<OrganisationViewModel> organisations = new List<OrganisationViewModel>();
            List<Organisation> databaseOrganisations = _depotOrganisation.GetOrganisations();

            if (databaseOrganisations.Count >= 1)
            {
                foreach (Organisation org in databaseOrganisations)
                {
                    organisations.Add(new OrganisationViewModel(org.Name));
                }
            }

            return organisations;
        }

        [HttpGet]
        public ActionResult<List<ClassroomViewModel>> GetClassroomsOfOrganisationByName(string orgName)
        {
            List<ClassroomViewModel> classes = new List<ClassroomViewModel>();
            string email = User.FindFirst(u => u.Type == ClaimTypes.Email)?.Value;
            Type userType = this.verificator.GetUserTypeByEmail(email);

            if (userType == typeof(Administrator))
            {
                List<Classroom> databaseClasses = _depotClassroom.GetClassroomsByOrganisationName(orgName);

                if (databaseClasses.Count >= 1)
                {
                    foreach (Classroom classroom in databaseClasses)
                    {
                        classes.Add(new ClassroomViewModel(classroom.Name));
                    }
                }
            }

            else if (userType == typeof(Teacher))
            {
                Teacher teacher = this._depotTeacher.GetTeacherByEmail(email);
                classes = GetClassroomsOfTeacherInOrganisation(teacher.Username, orgName);
            }

            return classes;
        }

        public List<ClassroomViewModel> GetClassroomsOfTeacherInOrganisation(string p_teacherUsername, string p_organisationName)
        {
            var classes = new List<ClassroomViewModel>();
            List<Classroom> databaseClasses = _depotClassroom.GetClassrooms()
                .Where(c => c.Teachers.FirstOrDefault(t => t.Username == p_teacherUsername) != null && c.OrganisationName == p_organisationName)
                .ToList();

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
            List<Assignment> databaseAssignments = _depotClassroom.GetAssignmentsByClassroomName(classroomName);

            if (databaseAssignments.Count >= 1)
            {
                foreach (Assignment assignment in databaseAssignments)
                {
                    assignments.Add(new AssignmentViewModel(assignment.Name));
                }
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
    }

}
