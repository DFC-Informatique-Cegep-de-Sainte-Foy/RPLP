using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RPLP.DAL.DTO.Json;
using RPLP.DAL.SQL.Depots;
using RPLP.ENTITES;
using RPLP.MVC.Models;
using RPLP.SERVICES.Github;
using RPLP.ENTITES.InterfacesDepots;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Net.Http;
using RPLP.JOURNALISATION;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System;
using System.Xml.Linq;
using RPLP.DAL.SQL.Migrations;
using RPLP.SERVICES.InterfacesDepots;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;

namespace RPLP.MVC.Controllers
{
    public class RPLPController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ScriptGithubRPLP _scriptGithub;
        private object classroomName;

        public RPLPController(IConfiguration configuration, IDepotClassroom depotClassroom,
            IDepotRepository depotRepository, IDepotOrganisation depotOrganisation, IDepotAllocation depotAllocation,
            IDepotStudent depotStudent)
        {
            if (configuration == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "LogsController - Index - la variable filePath est null ou vide", 0));
            }

            string token = configuration.GetValue<string>("Token");
            GithubApiAction _githubAction = new GithubApiAction(token);
            _scriptGithub = new ScriptGithubRPLP(depotClassroom, depotRepository, depotOrganisation, depotAllocation,
                depotStudent,
                token);

            this._httpClient = new HttpClient();
            this._httpClient.BaseAddress = new Uri("http://rplp.api/api/");
            this._httpClient.DefaultRequestHeaders.Accept
                .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            this._httpClient.DefaultRequestHeaders.Accept
                .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/octet-stream"));
        }

        #region Views

        public IActionResult Error(int? statusCode = null)
        {
            List<int> statues = new List<int>()
                { 400, 401, 402, 403, 404, 405, 406, 407, 408, 500, 501, 502, 503, 504 };

            if (statusCode.HasValue)
            {
                if (statues.Contains((int)statusCode))
                {
                    var viewName = statusCode.ToString();
                    return View(viewName);
                }
                else
                {
                    return View((object)400);
                }
            }
            else
            {
                return View((object)400);
            }
        }

        [Authorize]
        public IActionResult Index()
        {
            try
            {
                PeerReviewViewModel model = new PeerReviewViewModel();
                List<Organisation>? organisations = new List<Organisation>();

                string? email = User.FindFirst(u => u.Type == ClaimTypes.Email)?.Value;

                string? userType = this._httpClient
                    .GetFromJsonAsync<string>($"Verificator/UserType/{email}")
                    .Result;

                if (userType == "")
                {
                    return Error(403);
                }
                else
                {
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

                    organisations.ForEach(o => model.Organisations.Add(new OrganisationViewModel { Name = o.Name }));

                    return View(model);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Authorize]
        public IActionResult GestionDonnees()
        {
            GestionDonneeViewModel model = getGestionDonneeModel();

            if (model == null)
            {
                return Error(403);
            }
            else
            {
                return View("GestionDonnees", model);
            }
        }

        #endregion

        #region Methodes prive

        private GestionDonneeViewModel getGestionDonneeModel()
        {
            try
            {
                string email = User.FindFirst(u => u.Type == ClaimTypes.Email)?.Value;

                string? userType = this._httpClient
                    .GetFromJsonAsync<string>($"Verificator/UserType/{email}")
                    .Result;

                if (userType == "")
                {
                    return null;
                }

                {
                    GestionDonneeViewModel model = new GestionDonneeViewModel();

                    if (userType == typeof(Administrator).ToString())
                    {
                        model.RoleType = "Administrator";
                    }
                    else if (userType == typeof(Teacher).ToString())
                    {
                        model.RoleType = "Teacher";
                    }

                    List<Administrator> adminsResult = this._httpClient
                        .GetFromJsonAsync<List<Administrator>>($"Administrator")
                        .Result;


                    if (adminsResult.Count >= 1)
                        adminsResult.ForEach(admin =>
                        {
                            if (admin.Email != email)
                            {
                                model.Administrators.Add(new AdministratorViewModel
                                {
                                    Id = admin.Id,
                                    Email = admin.Email,
                                    Token = admin.Token,
                                    FirstName = admin.FirstName,
                                    LastName = admin.LastName,
                                    Username = admin.Username
                                });
                            }
                        });
                    else
                    {
                        RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                            new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                            "RPLPController - getGestionDonneeModel - la variable adminsResult est null ou vide", 0));
                    }

                    List<Organisation> organisationsResult = new List<Organisation>();
                    List<Classroom> classroomsResult = new List<Classroom>();

                    if (userType == typeof(Administrator).ToString())
                    {
                        Administrator administrator = this._httpClient
                            .GetFromJsonAsync<Administrator>($"Administrator/Email/{email}")
                            .Result;

                        organisationsResult = this._httpClient
                            .GetFromJsonAsync<List<Organisation>>($"Administrator/Email/{email}/Organisations")
                            .Result;

                        List<Organisation> allOrgs = this._httpClient
                            .GetFromJsonAsync<List<Organisation>>($"Organisation")
                            .Result;

                        classroomsResult = this._httpClient
                            .GetFromJsonAsync<List<Classroom>>($"Classroom")
                            .Result;

                        allOrgs.ForEach(organisation => model.AllOrgs.Add(new OrganisationViewModel
                            { Id = organisation.Id, Name = organisation.Name }));

                        model.DeactivatedAdministrators = GetDeactivatedAdministratorsList();
                        model.DeactivatedStudents = GetDeactivatedStudentsList();
                        model.DeactivatedTeachers = GetDeactivatedTeachersList();
                        model.DeactivateOrganisations = GetDeactivatedOrganisationsList();
                    }
                    else if (userType == typeof(Teacher).ToString())
                    {
                        Teacher teacher = this._httpClient
                            .GetFromJsonAsync<Teacher>($"Teacher/Email/{email}")
                            .Result;

                        organisationsResult = this._httpClient
                            .GetFromJsonAsync<List<Organisation>>($"Teacher/Username/{teacher.Username}/Organisations")
                            .Result;

                        classroomsResult = this._httpClient
                            .GetFromJsonAsync<List<Classroom>>($"Teacher/Username/{teacher.Username}/Classrooms")
                            .Result;
                    }

                    if (organisationsResult.Count >= 1)
                        organisationsResult.ForEach(organisation =>
                        {
                            model.Organisations.Add(new OrganisationViewModel
                                { Id = organisation.Id, Name = organisation.Name });
                        });
                    //else
                    //{
                    //    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    //         "RPLPController - getGestionDonneeModel - la variable organisationsResult est null ou vide"));
                    //}

                    if (classroomsResult.Count >= 1)
                        classroomsResult.ForEach(classroom =>
                        {
                            model.Classes.Add(new ClassroomViewModel { Id = classroom.Id, Name = classroom.Name });
                        });
                    //else
                    //{
                    //    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    //         "RPLPController - getGestionDonneeModel - la variable classroomsResult est null ou vide"));
                    //}


                    List<Teacher> teachersResult = this._httpClient
                        .GetFromJsonAsync<List<Teacher>>($"Teacher")
                        .Result;

                    if (teachersResult.Count >= 1)
                        teachersResult.ForEach(teacher =>
                        {
                            model.Teachers.Add(new TeacherViewModel
                            {
                                Id = teacher.Id,
                                Email = teacher.Email,
                                FirstName = teacher.FirstName,
                                LastName = teacher.LastName,
                                Username = teacher.Username
                            });
                        });
                    //else
                    //{
                    //    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    //         "RPLPController - getGestionDonneeModel - la variable teachersResult est null ou vide"));
                    //}

                    List<Assignment> assignmentsResult = this._httpClient
                        .GetFromJsonAsync<List<Assignment>>($"Assignment")
                        .Result;

                    if (assignmentsResult.Count >= 1)
                        assignmentsResult.ForEach(assignment =>
                        {
                            model.Assignments.Add(new AssignmentViewModel
                            {
                                Id = assignment.Id,
                                Name = assignment.Name,
                                Deadline = assignment.DeliveryDeadline,
                                Description = assignment.Description
                            });
                        });
                    //else
                    //{
                    //    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    //         "RPLPController - getGestionDonneeModel - la variable assignmentsResult est null ou vide"));
                    //}

                    List<Student> studentsResult = this._httpClient
                        .GetFromJsonAsync<List<Student>>($"Student")
                        .Result;

                    if (studentsResult.Count >= 1)
                        studentsResult.ForEach(student =>
                        {
                            model.Students.Add(new StudentViewModel
                            {
                                Id = student.Id,
                                Username = student.Username,
                                Email = student.Email,
                                FirstName = student.FirstName,
                                LastName = student.LastName,
                                IsTuteur = student.IsTutor,
                                Matricule = student.Matricule
                            });
                        });
                    //else
                    //{
                    //    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    //         "RPLPController - getGestionDonneeModel - la variable studentsResult est null ou vide"));
                    //}

                    return model;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private List<AdministratorViewModel> GetDeactivatedAdministratorsList()
        {
            try
            {
                List<Administrator> deactivatedAdministrators = this._httpClient
                    .GetFromJsonAsync<List<Administrator>>("Administrator/Deactivated")
                    .Result;

                List<AdministratorViewModel> administratorViewModels = deactivatedAdministrators
                    .Select(a => new AdministratorViewModel(a))
                    .ToList();

                return administratorViewModels;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private List<OrganisationViewModel> GetDeactivatedOrganisationsList()
        {
            try
            {
                List<Organisation> deactivatedOrganisations = this._httpClient
                    .GetFromJsonAsync<List<Organisation>>("Organisation/Deactivated")
                    .Result;
                List<OrganisationViewModel> organisationViewModels = new List<OrganisationViewModel>();

                deactivatedOrganisations.ForEach(org => organisationViewModels.Add(new OrganisationViewModel
                    { Id = org.Id, Name = org.Name }));

                return organisationViewModels;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private List<StudentViewModel> GetDeactivatedStudentsList()
        {
            try
            {
                List<Student> deactivatedStudents = this._httpClient
                    .GetFromJsonAsync<List<Student>>("Student/Deactivated")
                    .Result;

                List<StudentViewModel> studentViewModels = deactivatedStudents
                    .Select(s => new StudentViewModel(s))
                    .ToList();

                return studentViewModels;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private List<TeacherViewModel> GetDeactivatedTeachersList()
        {
            try
            {
                List<Teacher> deactivatedTeachers = this._httpClient
                    .GetFromJsonAsync<List<Teacher>>("Teacher/Deactivated")
                    .Result;

                List<TeacherViewModel> teacherViewModels = deactivatedTeachers
                    .Select(t => new TeacherViewModel(t))
                    .ToList();

                return teacherViewModels;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private List<OrganisationViewModel> GetOrganisations()
        {
            try
            {
                List<OrganisationViewModel> organisations = new List<OrganisationViewModel>();
                List<Organisation>? databaseOrganisations = this._httpClient
                    .GetFromJsonAsync<List<Organisation>>("Organisation")
                    .Result;

                if (databaseOrganisations == null)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - GetOrganisations - la liste databaseOrganisations assignée à partir de la méthode .GetFromJsonAsync<List<Organisation>>(\"Organisation\").Result est null ",
                        0));
                }

                foreach (Organisation org in databaseOrganisations)
                {
                    organisations.Add(new OrganisationViewModel { Name = org.Name });
                }

                return organisations;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region ActionGet

        [HttpGet]
        public ActionResult<AllocationsViewModel> GetAllocationsInformations(string classroomName,
            string assignementName)
        {
            List<AllocationViewModel> allocations = new List<AllocationViewModel>();

            try
            {
                if (!String.IsNullOrEmpty(classroomName) && !String.IsNullOrEmpty(assignementName))
                {
                    List<Allocation>? allocationsInDB =
                        this._httpClient.GetFromJsonAsync<List<Allocation>>($"/api/Allocation").Result;

                    List<Student>? students = _httpClient.GetFromJsonAsync<List<Student>>($"/api/Student").Result;

                    List<Teacher>? teachers = _httpClient.GetFromJsonAsync<List<Teacher>>($"/api/Teacher").Result;

                    List<Student>? classroomsStudent = _httpClient
                        .GetFromJsonAsync<List<Student>>($"/api/Classroom/Name/{classroomName}/Students").Result;

                    List<Repository>? repositories =
                        _httpClient.GetFromJsonAsync<List<Repository>>($"/api/Repository").Result;

                    if (allocationsInDB is not null)
                    {
                        foreach (Allocation allocation in allocationsInDB)
                        {
                            Repository? repository = repositories.Where(r => r.Id == allocation.RepositoryId)
                                .FirstOrDefault();

                            if (repository.Name.ToLower().Contains(assignementName.ToLower()))
                            {
                                Student? student = students.Where(s => s.Id == allocation.StudentId).FirstOrDefault();
                                Teacher? teacher = teachers.Where(t => t.Id == allocation.TeacherId).FirstOrDefault();
                                AllocationViewModel allocationViewModel;

                                if (teacher != null)
                                {
                                    allocationViewModel = new AllocationViewModel(allocation.Id, repository.Name, null,
                                        teacher.Username, null, allocation.Status);
                                }
                                else if (classroomsStudent.Find(s => s.Id == student.Id) != null)
                                {
                                    allocationViewModel = new AllocationViewModel(allocation.Id, repository.Name,
                                        student.Username, null, null, allocation.Status);
                                }
                                else
                                {
                                    allocationViewModel = new AllocationViewModel(allocation.Id, repository.Name, null,
                                        null, student.Username, allocation.Status);
                                }

                                allocations.Add(allocationViewModel);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            AllocationsViewModel allocationsViewModel = new AllocationsViewModel(allocations, assignementName);
            return allocationsViewModel;
        }

        [HttpGet]
        public ActionResult<int> GetValidReposByAssignmentName(string assignmentName)
        {
            int numberOfValidRepos;
            try
            {
                if (string.IsNullOrWhiteSpace(assignmentName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - GetValidReposByAssignmentName - assignmentName passé en paramètre est vide",
                        0));
                }

                numberOfValidRepos = this._httpClient
                    .GetFromJsonAsync<List<Repository>>("Repository")
                    .Result
                    .Where(repo => repo.Name.ToLower().Contains(assignmentName.ToLower())
                                   && repo.Name.Length > assignmentName.Length)
                    .ToList()
                    .Count;
            }
            catch (Exception e)
            {
                throw;
            }

            return numberOfValidRepos;
        }

        [HttpGet]
        public ActionResult<List<ClassroomViewModel>> GetClassroomsOfOrganisationByName(string orgName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(orgName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - GetClassroomsOfOrganisationByName - orgName passé en paramètre est vide", 0));
                }

                Logging.Instance.Journal(new Log("api", 0,
                    $"RPLPController - GET méthode GetClassroomsOfOrganisationByName(string orgName = {orgName})"));

                List<ClassroomViewModel> classes = new List<ClassroomViewModel>();

                string email = User.FindFirst(u => u.Type == ClaimTypes.Email)?.Value;
                string? userType = this._httpClient
                    .GetFromJsonAsync<string>($"Verificator/UserType/{email}")
                    .Result;

                if (userType == typeof(Administrator).ToString())
                {
                    List<Classroom>? databaseClasses = this._httpClient
                        .GetFromJsonAsync<List<Classroom>>($"Classroom/Organisation/{orgName}/Classroom")
                        .Result;

                    if (databaseClasses == null)
                    {
                        RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                            new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                            "RPLPController - GetClassroomsOfOrganisationByName - la liste databaseClasses assigné à partir de la méthode .GetFromJsonAsync<List<Classroom>>($\"Classroom/Organisation/{orgName}/Classroom\") est null",
                            0));
                    }

                    foreach (Classroom classroom in databaseClasses)
                    {
                        classes.Add(new ClassroomViewModel
                        {
                            Id = classroom.Id,
                            Name = classroom.Name,
                            OrganisationName = classroom.OrganisationName
                        });
                    }
                }
                else if (userType == typeof(Teacher).ToString())
                {
                    Teacher? teacher = this._httpClient
                        .GetFromJsonAsync<Teacher>($"Teacher/Email/{email}")
                        .Result;

                    classes = GetClassroomsOfTeacherInOrganisation(teacher.Username, orgName);
                }

                return classes;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult<List<AssignmentViewModel>> GetAssignmentOfClassroomByName(string classroomName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - GetAssignmentOfClassroomByName - classroomName passé en paramètre est vide",
                        0));
                }

                Logging.Instance.Journal(new Log("api", 0,
                    $"RPLPController - GET méthode GetAssignmentOfClassroomByName(string classroomName = {classroomName})"));

                List<AssignmentViewModel> assignments = new List<AssignmentViewModel>();
                List<Assignment> databaseAssignments = this._httpClient
                    .GetFromJsonAsync<List<Assignment>>($"Classroom/Assignments/{classroomName}")
                    .Result;

                if (databaseAssignments == null)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - GetAssignmentOfClassroomByName - la liste databaseAssignments assigné à partir de la méthode this._httpClient.GetFromJsonAsync<List<Assignment>>($\"Classroom/Assignments/{classroomName}\") est null",
                        0));
                }

                if (databaseAssignments.Count >= 1)
                {
                    foreach (Assignment assignment in databaseAssignments)
                    {
                        assignments.Add(new AssignmentViewModel
                            { Id = assignment.Id, Name = assignment.Name, Deadline = assignment.DeliveryDeadline });
                    }
                }

                return assignments;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public List<ClassroomViewModel> GetClassroomsOfTeacherInOrganisation(string p_teacherUsername,
            string p_organisationName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(p_teacherUsername))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - GetClassroomsOfTeacherInOrganisation - p_teacherUsername passé en paramètre est vide",
                        0));
                }

                if (string.IsNullOrWhiteSpace(p_organisationName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - GetClassroomsOfTeacherInOrganisation - p_organisationName passé en paramètre est vide",
                        0));
                }

                Logging.Instance.Journal(new Log("api", 0,
                    $"RPLPController - GET méthode GetClassroomsOfTeacherInOrganisation(string p_teacherUsername = {p_teacherUsername}, string p_organisationName = {p_organisationName})"));

                var classes = new List<ClassroomViewModel>();
                string? teacherEmail = User.FindFirst(c => c.Type == ClaimTypes.Email)?.Value;

                List<Classroom>? databaseClasses = this._httpClient
                    .GetFromJsonAsync<List<Classroom>>(
                        $"Teacher/Email/{teacherEmail}/Organisation/{p_organisationName}/Classrooms")
                    .Result;

                if (databaseClasses == null)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - GetClassroomsOfTeacherInOrganisation - la liste databaseClasses assigné à partir de la méthode this._httpClient.GetFromJsonAsync<List<Classroom>>($\"Teacher/Email/{teacherEmail}/Organisation/{p_organisationName}/Classrooms\") est null",
                        0));
                }

                foreach (Classroom classroom in databaseClasses)
                {
                    classes.Add(new ClassroomViewModel
                        { Id = classroom.Id, Name = classroom.Name, OrganisationName = classroom.OrganisationName });
                }

                return classes;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult<List<AssignmentViewModel>> GetAssignmentsOfClassroomByName(string classroomName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - GetAssignmentsOfClassroomByName - classroomName passé en paramètre est vide",
                        0));
                }

                Logging.Instance.Journal(new Log("api", 0,
                    $"RPLPController - GET méthode GetAssignmentsOfClassroomByName(string classroomName = {classroomName})"));

                List<AssignmentViewModel> assignments = new List<AssignmentViewModel>();
                List<Assignment>? databaseAssignments = this._httpClient
                    .GetFromJsonAsync<List<Assignment>>($"Assignment/Classroom/{classroomName}/Assignments")
                    .Result;

                if (databaseAssignments == null)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - GetAssignmentsOfClassroomByName - la liste databaseAssignments assigné à partir de la méthode this._httpClient.GetFromJsonAsync<List<Assignment>>($\"Assignment/Classroom/{classroomName}/Assignments\") est null",
                        0));
                }

                if (databaseAssignments.Count >= 1)
                {
                    foreach (Assignment assignment in databaseAssignments)
                    {
                        assignments.Add(new AssignmentViewModel { Id = assignment.Id, Name = assignment.Name });
                    }
                }

                return assignments;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult<List<AdministratorViewModel>> GetAdminsNotInOrganisationByName(string orgName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(orgName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - GetAdminsNotInOrganisationByName - orgName passé en paramètre est vide", 0));
                }

                Logging.Instance.Journal(new Log("api", 0,
                    $"RPLPController - GET méthode GetAdminsNotInOrganisationByName(string orgName = {orgName})"));

                List<AdministratorViewModel> admins = new List<AdministratorViewModel>();

                List<Administrator> databaseAdminInOrg = this._httpClient
                    .GetFromJsonAsync<List<Administrator>>($"Organisation/Name/{orgName}/Administrators")
                    .Result;

                List<Administrator> databaseAdmin = this._httpClient
                    .GetFromJsonAsync<List<Administrator>>($"Administrator")
                    .Result;

                if (databaseAdminInOrg == null)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - GetAdminsNotInOrganisationByName - la liste databaseAdminInOrg assigné à partir de la méthode  this._httpClient.GetFromJsonAsync<List<Administrator>>($\"Organisation/Name/{orgName}/Administrators\").Result; est null",
                        0));
                }

                if (databaseAdmin == null)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - GetAdminsNotInOrganisationByName - la liste databaseAdmin assigné à partir de la méthode this._httpClient.GetFromJsonAsync<List<Administrator>>($\"Administrator\").Result; est null",
                        0));
                }

                foreach (Administrator admin in databaseAdmin)
                {
                    if (!databaseAdminInOrg.Any(a => a.Username == admin.Username))
                    {
                        admins.Add(new AdministratorViewModel
                        {
                            Id = admin.Id,
                            Username = admin.Username,
                            Token = admin.Token,
                            FirstName = admin.FirstName,
                            LastName = admin.LastName,
                            Email = admin.Email
                        });
                    }
                }

                return admins;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult<List<AdministratorViewModel>> GetAdminsInOrganisationByName(string orgName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(orgName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - GetAdminsInOrganisationByName - orgName passé en paramètre est vide", 0));
                }

                Logging.Instance.Journal(new Log("api", 0,
                    $"RPLPController - GET méthode GetAdminsInOrganisationByName(string orgName = {orgName})"));

                List<AdministratorViewModel> admins = new List<AdministratorViewModel>();

                List<Administrator> databaseAdminInOrg = this._httpClient
                    .GetFromJsonAsync<List<Administrator>>($"Organisation/Name/{orgName}/Administrators")
                    .Result;

                if (databaseAdminInOrg == null)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - GetAdminsInOrganisationByName - la liste databaseAdminInOrg assigné à partir de la méthode this._httpClient.GetFromJsonAsync<List<Administrator>>($\"Organisation/Name/{orgName}/Administrators\").Result; est null",
                        0));
                }

                foreach (Administrator admin in databaseAdminInOrg)
                {
                    admins.Add(new AdministratorViewModel
                    {
                        Id = admin.Id,
                        Token = admin.Token,
                        Username = admin.Username,
                        FirstName = admin.FirstName,
                        LastName = admin.LastName,
                        Email = admin.Email
                    });
                }

                return admins;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult<List<TeacherViewModel>> GetTeacherNotInClassroomByClassroomName(string classroomName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - GetTeacherNotInClassroomByClassroomName - classroomName passé en paramètre est vide",
                        0));
                }

                Logging.Instance.Journal(new Log("api", 0,
                    $"RPLPController - GET méthode GetTeacherNotInClassroomByClassroomName(string classroomName = {classroomName})"));

                List<TeacherViewModel> teachers = new List<TeacherViewModel>();

                List<Teacher> databaseTeacherInClassroom = this._httpClient
                    .GetFromJsonAsync<List<Teacher>>($"Teacher")
                    .Result.Where(teacher => teacher.Classes.Any(classroom => classroom.Name == classroomName))
                    .ToList();

                List<Teacher> databaseTeacher = this._httpClient
                    .GetFromJsonAsync<List<Teacher>>($"Teacher")
                    .Result;

                if (databaseTeacherInClassroom == null)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - GetTeacherNotInClassroomByClassroomName - La liste databaseTeacherInClassroom assignée à partir de la méthode this._httpClient.GetFromJsonAsync<List<Teacher>>($\"Teacher\").Result.Where(teacher => teacher.Classes.Any(classroom => classroom.Name == classroomName)).ToList(); est null",
                        0));
                }

                if (databaseTeacher == null)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - GetTeacherNotInClassroomByClassroomName - La liste databaseTeacher assignée à partir de la méthode  this._httpClient.GetFromJsonAsync<List<Teacher>>($\"Teacher\").Result; est null",
                        0));
                }

                if (databaseTeacherInClassroom.Count >= 1 || databaseTeacher.Count >= 1)
                {
                    foreach (Teacher teacher in databaseTeacher)
                    {
                        if (!databaseTeacherInClassroom.Any(t => t.Username == teacher.Username))
                        {
                            teachers.Add(new TeacherViewModel
                            {
                                Id = teacher.Id,
                                Username = teacher.Username,
                                FirstName = teacher.FirstName,
                                LastName = teacher.LastName,
                                Email = teacher.Email
                            });
                        }
                    }
                }

                return teachers;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult<List<TeacherViewModel>> GetTeacherInClassroomByClassroomName(string classroomName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - GetTeacherInClassroomByClassroomName - classroomName passé en paramètre est vide",
                        0));
                }

                Logging.Instance.Journal(new Log("api", 0,
                    $"RPLPController - GET méthode GetTeacherInClassroomByClassroomName(string classroomName = {classroomName})"));

                List<TeacherViewModel> teachers = new List<TeacherViewModel>();

                List<Teacher> databaseTeacherInClassroom = this._httpClient
                    .GetFromJsonAsync<List<Teacher>>($"Teacher")
                    .Result.Where(teacher => teacher.Classes.Any(classroom => classroom.Name == classroomName))
                    .ToList();

                if (databaseTeacherInClassroom == null)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - GetTeacherInClassroomByClassroomName - La liste databaseTeacherInClassroom assignée à partir de la méthode  this._httpClient.GetFromJsonAsync<List<Teacher>>($\"Teacher\").Result.Where(teacher => teacher.Classes.Any(classroom => classroom.Name == classroomName)).ToList(); est null",
                        0));
                }

                if (databaseTeacherInClassroom.Count >= 1)
                    foreach (Teacher teacher in databaseTeacherInClassroom)
                    {
                        teachers.Add(new TeacherViewModel
                        {
                            Id = teacher.Id,
                            Username = teacher.Username,
                            FirstName = teacher.FirstName,
                            LastName = teacher.LastName,
                            Email = teacher.Email
                        });
                    }

                return teachers;
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpGet]
        public ActionResult<List<StudentViewModel>> GetTutors(string classroomName)
        {
            try
            {
                Logging.Instance.Journal(new Log("api", 0,
                    $"RPLPController - GET méthode GetTutors(string classroomName = {classroomName})"));

                List<StudentViewModel> tutors = new List<StudentViewModel>();

                List<Student> databaseTutor = this._httpClient
                    .GetFromJsonAsync<List<Student>>($"Student/Tutors")
                    .Result.Where(s => s.Classes.All(classroom => classroom.Name != classroomName))
                    .ToList();

                if (databaseTutor == null)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - GetTutors - La liste databaseTutor assignée à partir de la méthode  this._httpClient.GetFromJsonAsync<List<Student>>($\"Student\").Result; est null",
                        0));
                }

                foreach (var tutor in databaseTutor)
                {
                    tutors.Add(new StudentViewModel
                    {
                        Id = tutor.Id,
                        Username = tutor.Username,
                        FirstName = tutor.FirstName,
                        LastName = tutor.LastName,
                        Email = tutor.Email,
                        Matricule = tutor.Matricule,
                        IsTuteur = tutor.IsTutor
                    });
                }

                return tutors;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult<List<AssignmentViewModel>> GetAssignmentInClassroomByClassroomName(string classroomName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - GetAssignmentInClassroomByClassroomName - classroomName passé en paramètre est vide",
                        0));
                }

                Logging.Instance.Journal(new Log("api", 0,
                    $"RPLPController - GET méthode GetAssignmentInClassroomByClassroomName(string classroomName = {classroomName})"));

                List<AssignmentViewModel> assignments = new List<AssignmentViewModel>();

                List<Assignment> databaseAssignmentInClassroom = this._httpClient
                    .GetFromJsonAsync<List<Assignment>>($"Classroom/Name/{classroomName}/Assignments")
                    .Result;


                if (databaseAssignmentInClassroom == null)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - GetAssignmentInClassroomByClassroomName - La liste databaseAssignmentInClassroom assignée à partir de la méthode  this._httpClient.GetFromJsonAsync<List<Assignment>>($\"Classroom/Name/{classroomName}/Assignments\").Result;  est null",
                        0));
                }

                if (databaseAssignmentInClassroom.Count >= 1)
                    foreach (Assignment assignment in databaseAssignmentInClassroom)
                    {
                        assignments.Add(new AssignmentViewModel
                            { Id = assignment.Id, Name = assignment.Name, Deadline = assignment.DeliveryDeadline });
                    }

                return assignments;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult<List<StudentViewModel>> GetStudentsInClassroomByClassroomName(string ClassroomName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ClassroomName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - GetStudentsInClassroomByClassroomName - ClassroomName passé en paramètre est vide",
                        0));
                }

                Logging.Instance.Journal(new Log("api", 0,
                    $"RPLPController - GET méthode GetStudentsInClassroomByClassroomName(string ClassroomName = {ClassroomName})"));

                List<StudentViewModel> students = new List<StudentViewModel>();
                List<Student> databaseStudents = this._httpClient
                    .GetFromJsonAsync<List<Student>>($"Classroom/Name/{ClassroomName}/Students")
                    .Result;

                if (databaseStudents == null)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - GetStudentsInClassroomByClassroomName - La liste databaseStudents assignée à partir de la méthode  this._httpClient.GetFromJsonAsync<List<Student>>($\"Classroom/Name/{ClassroomName}/Students\").Result;  est null",
                        0));
                }

                if (databaseStudents.Count >= 1)
                    foreach (Student student in databaseStudents)
                    {
                        students.Add(new StudentViewModel
                        {
                            Id = student.Id,
                            Email = student.Email,
                            FirstName = student.FirstName,
                            LastName = student.LastName,
                            Username = student.Username,
                            IsTuteur = student.IsTutor,
                            Matricule = student.Matricule
                        });
                    }

                return students;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult<List<StudentViewModel>> GetStudentsNotInClassroomByClassroomName(string ClassroomName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ClassroomName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - GetStudentsNotInClassroomByClassroomName - ClassroomName passé en paramètre est vide",
                        0));
                }

                Logging.Instance.Journal(new Log("api", 0,
                    $"RPLPController - GET méthode GetStudentsNotInClassroomByClassroomName(string ClassroomName = {ClassroomName})"));

                List<StudentViewModel> students = new List<StudentViewModel>();
                List<Student> databaseStudentsInClassroom = this._httpClient
                    .GetFromJsonAsync<List<Student>>($"Classroom/Name/{ClassroomName}/Students")
                    .Result;

                List<Student> databaseStudents = this._httpClient
                    .GetFromJsonAsync<List<Student>>($"Student")
                    .Result;

                if (databaseStudentsInClassroom == null)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - GetStudentsNotInClassroomByClassroomName - La liste databaseStudentsInClassroom assignée à partir de la méthode this._httpClient.GetFromJsonAsync<List<Student>>($\"Classroom/Name/{ClassroomName}/Students\").Result; est null",
                        0));
                }

                if (databaseStudents == null)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - GetStudentsNotInClassroomByClassroomName - La liste databaseTeacher assignée à partir de la méthode  this._httpClient.GetFromJsonAsync<List<Student>>($\"Student\").Result; est null",
                        0));
                }

                if (databaseStudentsInClassroom.Count >= 1 || databaseStudents.Count >= 1)
                {
                    foreach (Student student in databaseStudents)
                    {
                        if (!databaseStudentsInClassroom.Any(a => a.Username == student.Username))
                        {
                            students.Add(new StudentViewModel
                            {
                                Id = student.Id,
                                Email = student.Email,
                                FirstName = student.FirstName,
                                LastName = student.LastName,
                                Username = student.Username,
                                IsTuteur = student.IsTutor,
                                Matricule = student.Matricule
                            });
                        }
                    }
                }

                return students;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult StartStudentAssignationScript(string organisationName, string classroomName,
            string assignmentName, int numberOfReviews)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(organisationName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - StartStudentAssignationScript - organisationName passé en paramètre est vide",
                        0));
                }

                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - StartStudentAssignationScript - classroomName passé en paramètre est vide",
                        0));
                }

                if (string.IsNullOrWhiteSpace(assignmentName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - StartStudentAssignationScript - assignmentName passé en paramètre est vide",
                        0));
                }

                if (numberOfReviews <= 0)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentOutOfRangeException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - StartStudentAssignationScript - numberOfReviews passé en paramètre est hors des limites",
                        0));
                }

                Logging.Instance.Journal(new Log("api", 200,
                    $"RPLPController - GET méthode StartStudentAssignationScript(string organisationName = {organisationName}, string classroomName = {classroomName}, string assignmentName = {assignmentName}, int numberOfReviews = {numberOfReviews})"));

                _scriptGithub.ScriptAssignStudentToAssignmentReview(organisationName, classroomName, assignmentName,
                    numberOfReviews);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult StartTeachertAssignationScript(string organisationName, string classroomName,
            string assignmentName, string teacherUsername)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(organisationName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - StartTeachertAssignationScript - organisationName passé en paramètre est vide",
                        0));
                }

                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - StartTeachertAssignationScript - classroomName passé en paramètre est vide",
                        0));
                }

                if (string.IsNullOrWhiteSpace(assignmentName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - StartTeachertAssignationScript - assignmentName passé en paramètre est vide",
                        0));
                }

                if (string.IsNullOrWhiteSpace(teacherUsername))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentOutOfRangeException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - StartTeachertAssignationScript - teacherUsername passé en paramètre est vide",
                        0));
                }

                Logging.Instance.Journal(new Log("api", 200,
                    $"RPLPController - GET méthode StartTeachertAssignationScript(string organisationName = {organisationName}, string classroomName = {classroomName}, string assignmentName = {assignmentName}, string teacherUsername = {teacherUsername})"));

                _scriptGithub.ScriptAssignTeacherToAssignmentReview(organisationName, classroomName, assignmentName,
                    teacherUsername);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult StartTutortAssignationScript(string organisationName, string classroomName,
            string assignmentName, string tutorUsername)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(organisationName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - StartTutortAssignationScript - organisationName passé en paramètre est vide",
                        0));
                }

                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - StartTutortAssignationScript - classroomName passé en paramètre est vide",
                        0));
                }

                if (string.IsNullOrWhiteSpace(assignmentName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - StartTutortAssignationScript - assignmentName passé en paramètre est vide",
                        0));
                }

                if (string.IsNullOrWhiteSpace(tutorUsername))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentOutOfRangeException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - StartTutortAssignationScript - teacherUsername passé en paramètre est vide",
                        0));
                }

                Logging.Instance.Journal(new Log("api", 200,
                    $"RPLPController - GET méthode StartTutortAssignationScript(string organisationName = {organisationName}, string classroomName = {classroomName}, string assignmentName = {assignmentName}, string tutorUsername = {tutorUsername})"));

                _scriptGithub.ScriptAssignTutorToAssignmentReview(organisationName, classroomName, assignmentName,
                    tutorUsername);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> DownloadCommentsOfPullRequestByAssignment(string organisationName,
            string classroomName, string assignmentName)
        {
            Stream stream;
            FileStreamResult fileStreamResult;

            try
            {
                if (string.IsNullOrWhiteSpace(organisationName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - DownloadCommentsOfPullRequestByAssignment - organisationName passé en paramètre est vide",
                        0));
                }

                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - DownloadCommentsOfPullRequestByAssignment - classroomName passé en paramètre est vide",
                        0));
                }

                if (string.IsNullOrWhiteSpace(assignmentName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - DownloadCommentsOfPullRequestByAssignment - assignmentName passé en paramètre est vide",
                        0));
                }

                Logging.Instance.Journal(new Log("api", 200,
                    $"RPLPController - GET méthode DownloadCommentsOfPullRequestByAssignment(string organisationName = {organisationName}, string classroomName = {classroomName}, string assignmentName = {assignmentName})"));

                stream = await this._httpClient.GetStreamAsync(
                    $"Github/{organisationName}/{classroomName}/{assignmentName}/PullRequests/Comments/File");
                fileStreamResult = new FileStreamResult(stream, "application/octet-stream");
                fileStreamResult.FileDownloadName = $"Comments_{assignmentName}_{DateTime.Now}.json";
            }
            catch (Exception)
            {
                return NotFound(
                    "Un ou plusieurs dépôts n'ont pas pu être trouvés. Il est peut-être privé et inaccessible à l'utilisateur.");
            }

            return fileStreamResult;
        }

        #endregion

        #region ActionPOST

        [HttpPost]
        public ActionResult<string> POSTUpsertAdmin(int Id, string Username, string Token, string FirstName,
            string LastName, string Email)
        {
            try
            {
                if (Id < 0)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentOutOfRangeException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTUpsertAdmin - Id passé en paramètre est hors des limites", 0));
                }

                if (string.IsNullOrWhiteSpace(Username))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTUpsertAdmin - Username passé en paramètre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(Token))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTUpsertAdmin - Token passé en paramètre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(FirstName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTUpsertAdmin - FirstName passé en paramètre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(LastName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTUpsertAdmin - LastName passé en paramètre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(Email))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTUpsertAdmin - Email passé en paramètre est vide", 0));
                }

                Administrator admin = new Administrator
                {
                    Id = Id,
                    Username = Username,
                    FirstName = FirstName,
                    LastName = LastName,
                    Email = Email,
                    Token = Token
                };

                Task<HttpResponseMessage> response = this._httpClient
                    .PostAsJsonAsync<Administrator>($"Administrator", admin);
                response.Wait();

                Logging.Instance.Journal(new Log("api", (int)response.Result.StatusCode,
                    $"RPLPController - POST méthode POSTUpsertAdmin(int Id = {Id}, string Username = {Username}, string Token = {Token}, string FirstName = {FirstName}, string LastName = {LastName}, string Email = {Email})"));

                return response.Result.StatusCode.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult<string> POSTUpsertOrg(int Id, string OrgName)
        {
            try
            {
                if (Id < 0)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentOutOfRangeException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTUpsertOrg - Id passé en paramètre est hors des limites", 0));
                }

                if (string.IsNullOrWhiteSpace(OrgName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTUpsertOrg - OrgName passé en paramètre est vide", 0));
                }

                Organisation org = new Organisation { Id = Id, Name = OrgName };

                Task<HttpResponseMessage> response = this._httpClient
                    .PostAsJsonAsync<Organisation>($"Organisation", org);
                response.Wait();

                Logging.Instance.Journal(new Log("api", (int)response.Result.StatusCode,
                    $"RPLPController - POST méthode POSTUpsertOrg(int Id = {Id}, string OrgName = {OrgName})"));

                return response.Result.StatusCode.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult<string> POSTUpsertClassroom(int Id, string ClassroomName, string OrganisationName)
        {
            try
            {
                if (Id < 0)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentOutOfRangeException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTUpsertClassroom - Id passé en paramètre est hors des limites", 0));
                }

                if (string.IsNullOrWhiteSpace(ClassroomName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTUpsertClassroom - ClassroomName passé en paramètre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(OrganisationName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTUpsertClassroom - OrganisationName passé en paramètre est vide", 0));
                }

                Classroom classroom = new Classroom
                {
                    Id = Id,
                    Name = ClassroomName,
                    OrganisationName = OrganisationName,
                    Assignments = new List<Assignment>(),
                    Students = new List<Student>(),
                    Teachers = new List<Teacher>(),
                    ActiveAssignment = null
                };

                if (Id != 0)
                {
                    Classroom databaseClassroom = this._httpClient
                        .GetFromJsonAsync<Classroom>($"Classroom/Id/{Id}").Result;

                    List<Assignment> databaseAssignments = this._httpClient
                        .GetFromJsonAsync<List<Assignment>>($"Classroom/Assignments/{databaseClassroom.Name}")
                        .Result;

                    if (databaseClassroom == null)
                    {
                        RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                            new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                            "RPLPController - POSTUpsertClassroom - La liste databaseClassroom assignée à partir de la méthode this._httpClient.GetFromJsonAsync<Classroom>($\"Classroom/Id/{Id}\").Result; est null",
                            0));
                    }

                    if (databaseAssignments == null)
                    {
                        RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                            new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                            "RPLPController - POSTUpsertClassroom - La liste databaseAssignments assignée à partir de la méthode   this._httpClient.GetFromJsonAsync<List<Assignment>>($\"Classroom/Assignments/{databaseClassroom.Name}\").Result; est null",
                            0));
                    }

                    if (databaseAssignments.Count >= 1)
                        foreach (Assignment assignment in databaseAssignments)
                        {
                            POSTModifyAssignment(assignment.Id, assignment.Name, ClassroomName, assignment.Description,
                                assignment.DeliveryDeadline);
                        }
                }

                Logging.Instance.Journal(new Log(
                    $"new classroomName = {classroom.Name}, new classroomOrganisation = {classroom.OrganisationName}"));

                Task<HttpResponseMessage> response = this._httpClient
                    .PostAsJsonAsync<Classroom>($"Classroom", classroom);
                response.Wait();

                Logging.Instance.Journal(new Log("api", (int)response.Result.StatusCode,
                    $"RPLPController - POST méthode POSTUpsertClassroom(int Id = {Id}, string ClassroomName = {ClassroomName}, string OrganisationName = {OrganisationName})"));

                return response.Result.StatusCode.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult<string> POSTUpsertStudent(int Id, string Email, string FirstName, string LastName,
            string Username, bool IsTuteur, string Matricule)
        {
            try
            {
                if (Id < 0)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentOutOfRangeException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTUpsertStudent - Id passé en paramètre est hors des limites", 0));
                }

                if (string.IsNullOrWhiteSpace(Username))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTUpsertStudent - Username passé en paramètre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(FirstName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTUpsertStudent - FirstName passé en paramètre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(LastName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTUpsertStudent - LastName passé en paramètre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(Email))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTUpsertStudent - Email passé en paramètre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(Matricule))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTUpsertStudent - Matricule passé en paramètre est vide", 0));
                }

                Student student = new Student
                {
                    Id = Id,
                    Email = Email,
                    FirstName = FirstName,
                    LastName = LastName,
                    Username = Username,
                    Classes = new List<Classroom>(),
                    IsTutor = IsTuteur,
                    Matricule = Matricule
                };

                Task<HttpResponseMessage> response = this._httpClient
                    .PostAsJsonAsync<Student>($"Student", student);
                response.Wait();

                Logging.Instance.Journal(new Log("api", (int)response.Result.StatusCode,
                    $"RPLPController - POST méthode POSTUpsertStudent(int Id = {Id}, string Email = {Email}, string FirstName = {FirstName}, string LastName = {LastName}, string Username = {Username}, string Matricule = {Matricule})"));

                return response.Result.StatusCode.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult<string> POSTUpsertBatchStudent(string students)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(students))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTUpsertBatchStudent - students passé en paramètre est vide", 0));
                }

                List<Student_JSONDTO> studentsJson =
                    JsonConvert.DeserializeObject<List<Student_JSONDTO>>(students);

                HttpResponseMessage result = new HttpResponseMessage();

                foreach (Student_JSONDTO student in studentsJson)
                {
                    string studentMatricule = student.studentId;
                    string studentLastName = student.lastName;
                    string studentFirstName = student.firstName;
                    string studentUsername = student.username;
                    string studentEmail = studentMatricule + "@csfoy.ca";

                    Student studentObj = new Student
                    {
                        Id = 0,
                        Email = studentEmail,
                        FirstName = studentFirstName,
                        LastName = studentLastName,
                        Username = studentUsername,
                        Classes = new List<Classroom>(),
                        Matricule = studentMatricule
                    };

                    Task<HttpResponseMessage> response = this._httpClient
                        .PostAsJsonAsync<Student>($"Student", studentObj);

                    Logging.Instance.Journal(new Log("api", (int)response.Result.StatusCode,
                        $"RPLPController - POST méthode POSTUpsertBatchStudent(string StudentString = {students})"));

                    result = response.Result;
                }

                return result.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> DownloadSingleRepository(string organisationName, string classroomName,
            string assignmentName, string studentUsername)
        {
            Stream stream;
            FileStreamResult fileStreamResult;

            try
            {
                if (string.IsNullOrWhiteSpace(organisationName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - DownloadSingleRepository - organisationName passé en paramètre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - DownloadSingleRepository - classroomName passé en paramètre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(assignmentName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - DownloadSingleRepository - assignmentName passé en paramètre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(studentUsername))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - DownloadSingleRepository - studentUsername passé en paramètre est vide", 0));
                }

                Logging.Instance.Journal(new Log("api", 0,
                    $"RPLPController - GET méthode DownloadSingleRepository(string organisationName = {organisationName}, string classroomName = {classroomName}, string assignmentName = {assignmentName}, string studentUsername = {studentUsername})"));

                string url =
                    $"Github/Telechargement/{organisationName}/{classroomName}/{assignmentName}/{studentUsername}";
                Console.Out.WriteLine($"Trying to download single repository. URL : {url}");
                stream = await this._httpClient.GetStreamAsync(url);
                fileStreamResult = new FileStreamResult(stream, "application/octet-stream");
                fileStreamResult.FileDownloadName = $"{assignmentName}-{studentUsername}.zip";
            }
            catch (Exception)
            {
                return NotFound(
                    "Le dépôt na pas pu être trouvé. Il est peut-être privé et inaccessible à l'utilisateur.");
            }

            return fileStreamResult;
        }

        [HttpGet]
        public async Task<IActionResult> DownloadAllRepositoriesForAssignment(string organisationName,
            string classroomName, string assignmentName)
        {
            Stream stream;
            FileStreamResult fileStreamResult;

            try
            {
                Logging.Instance.Journal(new Log("api", 0,
                    $"RPLPController - GET méthode DownloadAllRepositoriesForAssignment(string organisationName = {organisationName}, string classroomName = {classroomName}, string assignmentName = {assignmentName})"));

                string url = $"Github/Telechargement/{organisationName}/{classroomName}/{assignmentName}";
                Console.Out.WriteLine($"Trying to DownloadAllRepositoriesForAssignment - URL : {url}");
                stream = await this._httpClient.GetStreamAsync(url);
                fileStreamResult = new FileStreamResult(stream, "application/octet-stream");
                fileStreamResult.FileDownloadName = $"{assignmentName}-{classroomName}.zip";
            }
            catch (Exception ex)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(ex.ToString(),
                    ex.StackTrace.ToString().Replace(System.Environment.NewLine, "."),
                    $"RPLPController - DownloadAllRepositoriesForAssignment - {ex.Message}", 0));

                Console.Error.WriteLine($"Error DownloadAllRepositoriesForAssignment - Message : {ex.Message}");
                return NotFound(
                    "Un ou plusieurs dépôts n'ont pas pu être trouvé. Ils sont peut-être privés et inaccessibles à l'utilisateur.");
            }

            return fileStreamResult;
        }

        [HttpGet]
        public ActionResult RemoveCollaboratorsFromAssignmentRepositories(string organisationName, string classroomName,
            string assignmentName)
        {
            if (string.IsNullOrWhiteSpace(organisationName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "RPLPController - RemoveCollaboratorsFromAssignmentRepositories - organisationName passé en paramètre est vide",
                    0));
            }

            if (string.IsNullOrWhiteSpace(classroomName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "RPLPController - RemoveCollaboratorsFromAssignmentRepositories - classroomName passé en paramètre est vide",
                    0));
            }

            if (string.IsNullOrWhiteSpace(assignmentName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "RPLPController - RemoveCollaboratorsFromAssignmentRepositories - assignmentName passé en paramètre est vide",
                    0));
            }

            Logging.Instance.Journal(new Log("api", 0,
                $"RPLPController - GET méthode RemoveCollaboratorsFromAssignmentRepositories(string organisationName = {organisationName}, string classroomName = {classroomName}, string assignmentName = {assignmentName})"));

            _scriptGithub.ScriptRemoveStudentCollaboratorsFromAssignment(organisationName, classroomName,
                assignmentName);
            return Ok();
        }


        [HttpPost]
        public ActionResult<string> POSTUpsertTeacher(int Id, string Email, string FirstName, string LastName,
            string Username)
        {
            try
            {
                if (Id < 0)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentOutOfRangeException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTUpsertTeacher - Id passé en paramètre est hors des limites", 0));
                }

                if (string.IsNullOrWhiteSpace(Username))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTUpsertTeacher - Username passé en paramètre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(FirstName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTUpsertTeacher - FirstName passé en paramètre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(LastName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTUpsertTeacher - LastName passé en paramètre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(Email))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTUpsertTeacher - Email passé en paramètre est vide", 0));
                }

                Teacher teacher = new Teacher
                {
                    Id = Id,
                    Email = Email,
                    FirstName = FirstName,
                    LastName = LastName,
                    Username = Username,
                    Classes = new List<Classroom>()
                };

                Task<HttpResponseMessage> response = this._httpClient
                    .PostAsJsonAsync<Teacher>($"Teacher", teacher);
                response.Wait();

                Logging.Instance.Journal(new Log("api", (int)response.Result.StatusCode,
                    $"RPLPController - POST méthode POSTUpsertTeacher(int Id = {Id}, string Email = {Email}, string FirstName = {FirstName}, string LastName = {LastName}, string Username = {Username})"));

                return response.Result.StatusCode.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult<string> POSTNewAssignment(string Name, string ClassroomName, string Description,
            DateTime? DeliveryDeadline)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Name))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTNewAssignment - Name passé en paramètre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(ClassroomName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTNewAssignment - ClassroomName passé en paramètre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(Description))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTNewAssignment - Description passé en paramètre est vide", 0));
                }

                if (DeliveryDeadline == DateTime.MinValue)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentOutOfRangeException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTNewAssignment - DeliveryDeadline passé en paramètre n'est pas conforme",
                        0));
                }

                Logging.Instance.Journal(new Log("api", 0,
                    $"RPLPController - POST méthode POSTNewAssignment(string Name = {Name}, string ClassroomName = {ClassroomName}, string Description = {Description}, DateTime? DeliveryDeadline = {DeliveryDeadline})"));

                Assignment newAssignment = new Assignment
                {
                    Id = 0,
                    Name = Name,
                    ClassroomName = ClassroomName,
                    Description = Description,
                    DeliveryDeadline = DeliveryDeadline,
                    DistributionDate = DateTime.Now
                };

                Task<HttpResponseMessage> response = this._httpClient
                    .PostAsJsonAsync<Assignment>($"Assignment", newAssignment);
                response.Wait();

                if (!response.IsCompleted)
                {
                    return response.Result.StatusCode.ToString();
                }

                response = this._httpClient
                    .PostAsJsonAsync($"Classroom/Name/{ClassroomName}/Assignments/Add/{Name}", "");

                response.Wait();

                return response.Result.StatusCode.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult<string> POSTModifyAssignment(int Id, string Name, string ClassroomName, string Description,
            DateTime? DeliveryDeadline)
        {
            try
            {
                if (Id < 0)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentOutOfRangeException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTModifyAssignment - Id passé en paramètre est hors des limites", 0));
                }

                if (string.IsNullOrWhiteSpace(Name))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTModifyAssignment - Name passé en paramètre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(ClassroomName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTModifyAssignment - ClassroomName passé en paramètre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(Description))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTModifyAssignment - Description passé en paramètre est vide", 0));
                }

                if (DeliveryDeadline == DateTime.MinValue)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentOutOfRangeException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTModifyAssignment - DeliveryDeadline passé en paramètre n'est pas conforme",
                        0));
                }

                Assignment Assignment = new Assignment
                {
                    Id = Id,
                    Name = Name,
                    Description = Description,
                    DeliveryDeadline = DeliveryDeadline,
                    ClassroomName = ClassroomName,
                    DistributionDate = DateTime.Now
                };

                Task<HttpResponseMessage> response = this._httpClient
                    .PostAsJsonAsync<Assignment>($"Assignment", Assignment);
                response.Wait();

                Logging.Instance.Journal(new Log("api", (int)response.Result.StatusCode,
                    $"RPLPController - POST méthode POSTModifyAssignment(int Id = {Id}, string Name = {Name}, string ClassroomName = {ClassroomName}, string Description = {Description}, DateTime? DeliveryDeadline = {DeliveryDeadline})"));

                return response.Result.StatusCode.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult<string> POSTAddAdminToOrg(string OrgName, string AdminUsername)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(OrgName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTAddAdminToOrg - OrgName passé en paramètre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(AdminUsername))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTAddAdminToOrg - AdminUsername passé en paramètre est vide", 0));
                }

                Task<HttpResponseMessage> response = this._httpClient
                    .PostAsJsonAsync($"Administrator/Username/{AdminUsername}/Orgs/Add/{OrgName}", "");
                response.Wait();

                Logging.Instance.Journal(new Log("api", (int)response.Result.StatusCode,
                    $"RPLPController - POST méthode POSTAddAdminToOrg(string OrgName = {OrgName}, string AdminUsername = {AdminUsername})"));

                return response.Result.StatusCode.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult<string> POSTAddStudentToClassroom(string ClassroomName, string StudentUsername)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ClassroomName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTAddStudentToClassroom - ClassroomName passé en paramètre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(StudentUsername))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTAddStudentToClassroom - StudentUsername passé en paramètre est vide", 0));
                }

                Task<HttpResponseMessage> response = this._httpClient
                    .PostAsJsonAsync($"Classroom/Name/{ClassroomName}/Students/Add/{StudentUsername}", "");
                response.Wait();

                Logging.Instance.Journal(new Log("api", (int)response.Result.StatusCode,
                    $"RPLPController - POST méthode POSTAddStudentToClassroom(string ClassroomName = {ClassroomName}, string StudentUsername = {StudentUsername})"));

                return response.Result.StatusCode.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult<string> POSTAddStudentToClassroomBatch(string ClassroomName, string StudentString)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ClassroomName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTAddStudentToClassroomBatch - ClassroomName passé en paramètre est vide",
                        0));
                }

                if (string.IsNullOrWhiteSpace(StudentString))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTAddStudentToClassroomBatch - StudentString passé en paramètre est vide",
                        0));
                }

                Logging.Instance.Journal(new Log("api", 0,
                    $"RPLPController - POST méthode POSTAddStudentToClassroomBatch(string ClassroomName = {ClassroomName}, string StudentString = {StudentString})"));

                List<string> SplitStudents = JsonConvert.DeserializeObject<List<string>>(StudentString);

                HttpResponseMessage result = new HttpResponseMessage();

                foreach (string studentId in SplitStudents)
                {
                    Task<HttpResponseMessage> response = this._httpClient
                        .PostAsJsonAsync($"Classroom/Name/{ClassroomName}/Students/Add/Matricule/{studentId}",
                            "");

                    result = response.Result;
                }

                return result.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult<string> POSTAddTeacherToClassroom(string ClassroomName, string TeacherUsername)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ClassroomName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTAddTeacherToClassroom - ClassroomName passé en paramètre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(TeacherUsername))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTAddTeacherToClassroom - TeacherUsername passé en paramètre est vide", 0));
                }

                Task<HttpResponseMessage> response = this._httpClient
                    .PostAsJsonAsync($"Classroom/Name/{ClassroomName}/Teachers/Add/{TeacherUsername}", "");
                response.Wait();

                Logging.Instance.Journal(new Log("api", (int)response.Result.StatusCode,
                    $"RPLPController - POST méthode POSTAddTeacherToClassroom(string ClassroomName = {ClassroomName}, string TeacherUsername = {TeacherUsername})"));

                return response.Result.StatusCode.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult<string> POSTRemoveTeacherFromClassroom(string ClassroomName, string TeacherUsername)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ClassroomName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTRemoveTeacherFromClassroom - ClassroomName passé en paramètre est vide",
                        0));
                }

                if (string.IsNullOrWhiteSpace(TeacherUsername))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTRemoveTeacherFromClassroom - TeacherUsername passé en paramètre est vide",
                        0));
                }

                Task<HttpResponseMessage> response = this._httpClient
                    .PostAsJsonAsync($"Classroom/Name/{ClassroomName}/Teachers/Remove/{TeacherUsername}", "");
                response.Wait();

                Logging.Instance.Journal(new Log("api", (int)response.Result.StatusCode,
                    $"RPLPController - POST méthode POSTRemoveTeacherFromClassroom(string ClassroomName = {ClassroomName}, string TeacherUsername = {TeacherUsername})"));

                return response.Result.StatusCode.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult<string> POSTRemoveAssignmentFromClassroom(string ClassroomName, string AssignmentName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ClassroomName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTRemoveAssignmentFromClassroom - ClassroomName passé en paramètre est vide",
                        0));
                }

                if (string.IsNullOrWhiteSpace(AssignmentName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTRemoveAssignmentFromClassroom - AssignmentName passé en paramètre est vide",
                        0));
                }

                Task<HttpResponseMessage> response = this._httpClient
                    .PostAsJsonAsync($"Classroom/Name/{ClassroomName}/Assignments/Remove/{AssignmentName}", "");
                response.Wait();

                Logging.Instance.Journal(new Log("api", (int)response.Result.StatusCode,
                    $"RPLPController - POST méthode POSTRemoveAssignmentFromClassroom(string ClassroomName = {ClassroomName}, string AssignmentName = {AssignmentName})"));

                return response.Result.StatusCode.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult<string> POSTRemoveStudentFromClassroom(string ClassroomName, string StudentUsername)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ClassroomName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTRemoveStudentFromClassroom - ClassroomName passé en paramètre est vide",
                        0));
                }

                if (string.IsNullOrWhiteSpace(StudentUsername))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTRemoveStudentFromClassroom - StudentUsername passé en paramètre est vide",
                        0));
                }

                Task<HttpResponseMessage> response = this._httpClient
                    .PostAsJsonAsync($"Classroom/Name/{ClassroomName}/Students/Remove/{StudentUsername}", "");
                response.Wait();

                Logging.Instance.Journal(new Log("api", (int)response.Result.StatusCode,
                    $"RPLPController - POST méthode POSTRemoveStudentFromClassroom(string ClassroomName = {ClassroomName}, string StudentUsername = {StudentUsername})"));

                return response.Result.StatusCode.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult<string> POSTRemoveAdminFromOrg(string OrgName, string AdminUsername)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(OrgName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTRemoveAdminFromOrg - OrgName passé en paramètre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(AdminUsername))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTRemoveAdminFromOrg - AdminUsername passé en paramètre est vide", 0));
                }

                Task<HttpResponseMessage> response = this._httpClient
                    .PostAsJsonAsync($"Administrator/Username/{AdminUsername}/Orgs/Remove/{OrgName}", "");
                response.Wait();

                Logging.Instance.Journal(new Log("api", (int)response.Result.StatusCode,
                    $"RPLPController - POST méthode POSTRemoveAdminFromOrg(string OrgName = {OrgName}, string AdminUsername = {AdminUsername})"));

                return response.Result.StatusCode.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult<string> POSTDeleteAdmin(string Username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Username))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTDeleteAdmin - Username passé en paramètre est vide", 0));
                }

                Task<HttpResponseMessage> response = this._httpClient
                    .DeleteAsync($"Administrator/Username/{Username}");
                response.Wait();

                Logging.Instance.Journal(new Log("api", (int)response.Result.StatusCode,
                    $"RPLPController - POST méthode POSTDeleteAdmin(string Username = {Username})"));

                return response.Result.StatusCode.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult<string> POSTDeleteAssignment(string AssignmentName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(AssignmentName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTDeleteAssignment - AssignmentName passé en paramètre est vide", 0));
                }

                Task<HttpResponseMessage> response = this._httpClient
                    .DeleteAsync($"Assignment/Name/{AssignmentName}");
                response.Wait();

                Logging.Instance.Journal(new Log("api", (int)response.Result.StatusCode,
                    $"RPLPController - POST méthode POSTDeleteAssignment(string AssignmentName = {AssignmentName})"));

                return response.Result.StatusCode.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult<string> POSTDeleteOrg(string OrgName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(OrgName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTDeleteOrg - OrgName passé en paramètre est vide", 0));
                }

                Task<HttpResponseMessage> response = this._httpClient
                    .DeleteAsync($"Organisation/Name/{OrgName}");
                response.Wait();

                Logging.Instance.Journal(new Log("api", (int)response.Result.StatusCode,
                    $"RPLPController - POST méthode POSTDeleteOrg(string OrgName = {OrgName})"));

                return response.Result.StatusCode.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult<string> POSTDeleteClassroom(string ClassroomName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ClassroomName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTDeleteClassroom - ClassroomName passé en paramètre est vide", 0));
                }

                Task<HttpResponseMessage> response = this._httpClient
                    .DeleteAsync($"Classroom/Name/{ClassroomName}");
                response.Wait();

                Logging.Instance.Journal(new Log("api", (int)response.Result.StatusCode,
                    $"RPLPController - POST méthode POSTDeleteClassroom(string ClassroomName = {ClassroomName})"));

                return response.Result.StatusCode.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult<string> POSTDeleteStudent(string StudentUsername)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(StudentUsername))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTDeleteStudent - StudentUsername passé en paramètre est vide", 0));
                }

                Task<HttpResponseMessage> response = this._httpClient
                    .DeleteAsync($"Student/Username/{StudentUsername}");
                response.Wait();

                Logging.Instance.Journal(new Log("api", (int)response.Result.StatusCode,
                    $"RPLPController - POST méthode POSTDeleteStudent(string StudentUsername = {StudentUsername})"));

                return response.Result.StatusCode.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult<string> POSTDeleteTeacher(string TeacherUsername)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TeacherUsername))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTDeleteTeacher - TeacherUsername passé en paramètre est vide", 0));
                }

                Task<HttpResponseMessage> response = this._httpClient
                    .DeleteAsync($"Teacher/Username/{TeacherUsername}");
                response.Wait();

                Logging.Instance.Journal(new Log("api", (int)response.Result.StatusCode,
                    $"RPLPController - POST méthode POSTDeleteTeacher(string TeacherUsername = {TeacherUsername})"));

                return response.Result.StatusCode.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult<string> POSTReactivateAdmin(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTReactivateAdmin - username passé en paramètre est vide", 0));
                }

                Task<HttpResponseMessage> response = this._httpClient
                    .GetAsync($"Administrator/Reactivate/{username}");
                response.Wait();

                Logging.Instance.Journal(new Log("api", (int)response.Result.StatusCode,
                    $"RPLPController - POST méthode POSTReactivateAdmin(string username = {username})"));

                return response.Result.StatusCode.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult<string> POSTReactivateOrg(string orgName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(orgName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTReactivateOrg - orgName passé en paramètre est vide", 0));
                }

                Task<HttpResponseMessage> response = this._httpClient
                    .GetAsync($"Organisation/Reactivate/{orgName}");
                response.Wait();

                Logging.Instance.Journal(new Log("api", (int)response.Result.StatusCode,
                    $"RPLPController - POST méthode POSTReactivateOrg(string orgName = {orgName})"));

                return response.Result.StatusCode.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult<string> POSTReactivateStudent(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTReactivateStudent - username passé en paramètre est vide", 0));
                }

                Task<HttpResponseMessage> response = this._httpClient
                    .GetAsync($"Student/Reactivate/{username}");
                response.Wait();

                Logging.Instance.Journal(new Log("api", (int)response.Result.StatusCode,
                    $"RPLPController - POST méthode POSTReactivateStudent(string username = {username})"));

                return response.Result.StatusCode.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult<string> POSTReactivateTeacher(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "RPLPController - POSTReactivateTeacher - username passé en paramètre est vide", 0));
                }

                Task<HttpResponseMessage> response = this._httpClient
                    .GetAsync($"Teacher/Reactivate/{username}");
                response.Wait();

                Logging.Instance.Journal(new Log("api", (int)response.Result.StatusCode,
                    $"RPLPController - POST méthode POSTReactivateTeacher(string username = {username})"));

                return response.Result.StatusCode.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Coherence

        [HttpGet]
        public ActionResult StartScriptCoherence()
        {
            Logging.Instance.Journal(new Log("api", 200, $"RPLPController - POST méthode StartScriptCoherence"));

            this._scriptGithub.EnsureOrganisationRepositoriesAreInDB();
            this._scriptGithub.ValidateAllRepositoriesHasBranch();
            return Ok();
        }

        #endregion
    }
}