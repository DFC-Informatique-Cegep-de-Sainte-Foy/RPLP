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
        private readonly ScriptGithubRPLP _scriptGithub;
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

            organisations.ForEach(o => model.Organisations.Add(new OrganisationViewModel { Name = o.Name }));

            return View(model);
        }

        public IActionResult GestionDonnees()
        {
            GestionDonneeViewModel model = getGestionDonneeModel();

            return View("GestionDonnees", model);
        }

        public GestionDonneeViewModel getGestionDonneeModel()
        {
            string email = User.FindFirst(u => u.Type == ClaimTypes.Email)?.Value;
            Type userType = this.verificator.GetUserTypeByEmail(email);
            GestionDonneeViewModel model = new GestionDonneeViewModel();

            if (userType == typeof(Administrator))
            {
                model.RoleType = "Administrator";
            }
            else if (userType == typeof(Teacher))
            {
                model.RoleType = "Teacher";
            }

            List<Administrator> adminsResult = _depotAdministrator.GetAdministrators();
            if (adminsResult.Count >= 1)
                adminsResult.ForEach(admin =>
                {
                    if (admin.Email != email)
                    {
                        model.Administrators.Add(new AdministratorViewModel { Id = admin.Id, Email = admin.Email, Token = admin.Token, FirstName = admin.FirstName, LastName = admin.LastName, Username = admin.Username });
                    }
                });

            List<Organisation> organisationsResult = new List<Organisation>();

            if (userType == typeof(Administrator))
            {
                organisationsResult = _depotOrganisation.GetOrganisations();
            }
            else if (userType == typeof(Teacher))
            {
                Teacher teacher = this._depotTeacher.GetTeacherByEmail(email);
                organisationsResult = _depotTeacher.GetTeacherOrganisations(teacher.Username);
            }

            if (organisationsResult.Count >= 1)
                organisationsResult.ForEach(organisation =>
                {
                    model.Organisations.Add(new OrganisationViewModel { Id = organisation.Id, Name = organisation.Name });
                });


            List<Teacher> teachersResult = _depotTeacher.GetTeachers();
            if (teachersResult.Count >= 1)
                teachersResult.ForEach(teacher =>
                {
                    model.Teachers.Add(new TeacherViewModel { Id = teacher.Id, Email = teacher.Email, FirstName = teacher.FirstName, LastName = teacher.LastName, Username = teacher.Username });
                });

            return model;
        }

        private List<OrganisationViewModel> GetOrganisations()
        {
            List<OrganisationViewModel> organisations = new List<OrganisationViewModel>();
            List<Organisation> databaseOrganisations = _depotOrganisation.GetOrganisations();

            if (databaseOrganisations.Count >= 1)
            {
                foreach (Organisation org in databaseOrganisations)
                {
                    organisations.Add(new OrganisationViewModel { Name = org.Name });
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
    }

}
