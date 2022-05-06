using Microsoft.AspNetCore.Authorization;
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
        private readonly ScriptGithubRPLP _scriptGithub;

        public RPLPController()
        {
            string token = "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";
            GithubApiAction _githubAction = new GithubApiAction(token);
            _scriptGithub = new ScriptGithubRPLP(new DepotClassroom(), new DepotRepository(), token);

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
            string? userType = this._httpClient
                .GetFromJsonAsync<string>($"Verificator/UserType/{email}")
                .Result;

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
                        model.Administrators.Add(new AdministratorViewModel { Id = admin.Id, Email = admin.Email, Token = admin.Token, FirstName = admin.FirstName, LastName = admin.LastName, Username = admin.Username });
                    }
                });

            List<Organisation> organisationsResult = new List<Organisation>();

            if (userType == typeof(Administrator).ToString())
            {
                Administrator administrator = this._httpClient
                    .GetFromJsonAsync<Administrator>($"Administrator/Email/{email}")
                    .Result;

                organisationsResult = this._httpClient
                    .GetFromJsonAsync<List<Organisation>>($"Administrator/Email/{email}/Organisations")
                    .Result;
            }
            else if (userType == typeof(Teacher).ToString())
            {
                Teacher teacher = this._httpClient
                    .GetFromJsonAsync<Teacher>($"Teacher/Email/{email}")
                    .Result;

                organisationsResult = this._httpClient
                    .GetFromJsonAsync<List<Organisation>>($"Teacher/Username/{teacher.Username}/Organisations")
                    .Result;
            }

            if (organisationsResult.Count >= 1)
                organisationsResult.ForEach(organisation =>
                {
                    model.Organisations.Add(new OrganisationViewModel { Id = organisation.Id, Name = organisation.Name });
                });


            List<Teacher> teachersResult = this._httpClient
                    .GetFromJsonAsync<List<Teacher>>($"Teacher")
                    .Result;
            if (teachersResult.Count >= 1)
                teachersResult.ForEach(teacher =>
                {
                    model.Teachers.Add(new TeacherViewModel { Id = teacher.Id, Email = teacher.Email, FirstName = teacher.FirstName, LastName = teacher.LastName, Username = teacher.Username });
                });

            List<Assignment> assignmentsResult = this._httpClient
                    .GetFromJsonAsync<List<Assignment>>($"Assignment")
                    .Result;
            if (assignmentsResult.Count >= 1)
                assignmentsResult.ForEach(assignment =>
                {
                    model.Assignments.Add(new AssignmentViewModel { Name = assignment.Name, Deadline = assignment.DeliveryDeadline });
                });

            List<Student> studentsResult = this._httpClient
                    .GetFromJsonAsync<List<Student>>($"Student")
                    .Result;
            if (studentsResult.Count >= 1)
                studentsResult.ForEach(student =>
                {
                    model.Students.Add(new StudentViewModel { Username = student.Username, Email = student.Email, FirstName = student.FirstName, LastName = student.LastName });
                });

            return model;
        }

        private List<OrganisationViewModel> GetOrganisations()
        {
            List<OrganisationViewModel> organisations = new List<OrganisationViewModel>();
            List<Organisation>? databaseOrganisations = this._httpClient
                .GetFromJsonAsync<List<Organisation>>("Organisation")
                .Result;

            foreach (Organisation org in databaseOrganisations)
            {
                organisations.Add(new OrganisationViewModel { Name = org.Name });
            }

            return organisations;
        }

        [HttpGet]
        public ActionResult<List<ClassroomViewModel>> GetClassroomsOfOrganisationByName(string orgName)
        {
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

                classes = GetClassroomsOfTeacherInOrganisation(teacher.Username, orgName);
            }

            return classes;
        }

        [HttpGet]
        public ActionResult<List<AssignmentViewModel>> GetAssignmentOfClassroomByName(string classroomName)
        {
            List<AssignmentViewModel> assignments = new List<AssignmentViewModel>();
            List<Assignment> databaseAssignments = this._httpClient
                    .GetFromJsonAsync<List<Assignment>>($"Classroom/Assignments/{classroomName}")
                    .Result;

            if (databaseAssignments.Count >= 1)
            {
                foreach (Assignment assignment in databaseAssignments)
                {
                    assignments.Add(new AssignmentViewModel { Name = assignment.Name, Deadline = assignment.DeliveryDeadline });
                }
            }

            return assignments;
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

        //DANS LA METHODE POUR AJOUTER UN ASSIGNEMENT, FAIRE QUE SA CREE L'ASSIGNMENT ET QUE SA L'ASSIGNE A LA CLASSE PAR LA SUITE

        [HttpGet]
        public ActionResult<List<AssignmentViewModel>> GetAssignmentsOfClassroomByName(string classroomName)
        {
            List<AssignmentViewModel> assignments = new List<AssignmentViewModel>();
            List<Assignment>? databaseAssignments = this._httpClient
                .GetFromJsonAsync<List<Assignment>>($"Assignment/Classroom/{classroomName}/Assignments")
                .Result;

            foreach (Assignment assignment in databaseAssignments)
            {
                assignments.Add(new AssignmentViewModel { Name = assignment.Name });
            }
            if (databaseAssignments.Count >= 1)
            {
                foreach (Assignment assignment in databaseAssignments)
                {
                    assignments.Add(new AssignmentViewModel { Name = assignment.Name });
                }
            }

            return assignments;
        }

        [HttpGet]
        public ActionResult<List<AdministratorViewModel>> GetAdminsNotInOrganisationByName(string orgName)
        {
            List<AdministratorViewModel> admins = new List<AdministratorViewModel>();

            List<Administrator> databaseAdminInOrg = this._httpClient
                .GetFromJsonAsync<List<Administrator>>($"Administrator/Organisations/{orgName}/Administrators")
                .Result;
            List<Administrator> databaseAdmin = this._httpClient
                .GetFromJsonAsync<List<Administrator>>($"Administrator")
                .Result;

            if (databaseAdminInOrg.Count >= 1 || databaseAdmin.Count >= 1)
            {
                foreach (Administrator admin in databaseAdmin)
                {
                    if (!databaseAdminInOrg.Any(a => a.Username == admin.Username))
                    {
                        admins.Add(new AdministratorViewModel { Id = admin.Id, Token = admin.Token, FirstName = admin.FirstName, LastName = admin.LastName, Email = admin.Email });
                    }
                }
            }

            return admins;
        }

        [HttpGet]
        public ActionResult<List<TeacherViewModel>> GetTeacherNotInClassroomByClassroomName(string classroomName)
        {
            List<TeacherViewModel> teachers = new List<TeacherViewModel>();

            List<Teacher> databaseTeacherInClassroom = this._httpClient
                .GetFromJsonAsync<List<Teacher>>($"Teacher")
                .Result.Where(teacher => teacher.Classes.Any(classroom => classroom.Name == classroomName)).ToList();

            List<Teacher> databaseTeacher = this._httpClient
                .GetFromJsonAsync<List<Teacher>>($"Teacher")
                .Result;

            if (databaseTeacherInClassroom.Count >= 1 || databaseTeacher.Count >= 1)
            {
                foreach (Teacher teacher in databaseTeacher)
                {
                    if (!databaseTeacherInClassroom.Any(t => t.Username == teacher.Username))
                    {
                        teachers.Add(new TeacherViewModel { Id = teacher.Id, FirstName = teacher.FirstName, LastName = teacher.LastName, Email = teacher.Email });
                    }
                }
            }

            return teachers;
        }

        [HttpGet]
        public ActionResult<List<TeacherViewModel>> GetTeacherInClassroomByClassroomName(string classroomName)
        {
            List<TeacherViewModel> teachers = new List<TeacherViewModel>();

            List<Teacher> databaseTeacherInClassroom = this._httpClient
                .GetFromJsonAsync<List<Teacher>>($"Teacher")
                .Result.Where(teacher => teacher.Classes.Any(classroom => classroom.Name == classroomName)).ToList();

            if (databaseTeacherInClassroom.Count >= 1)
                foreach (Teacher teacher in databaseTeacherInClassroom)
                {
                    teachers.Add(new TeacherViewModel { Id = teacher.Id, FirstName = teacher.FirstName, LastName = teacher.LastName, Email = teacher.Email });
                }

            return teachers;
        }

        [HttpGet]
        public ActionResult<List<AssignmentViewModel>> GetAssignmentInClassroomByClassroomName(string classroomName)
        {
            List<AssignmentViewModel> assignments = new List<AssignmentViewModel>();

            List<Assignment> databaseAssignmentInClassroom = this._httpClient
                .GetFromJsonAsync<List<Assignment>>($"Assignments/{classroomName}")
                .Result; ;

            if (databaseAssignmentInClassroom.Count >= 1)
                foreach (Assignment assignment in databaseAssignmentInClassroom)
                {
                    assignments.Add(new AssignmentViewModel { Name = assignment.Name, Deadline = assignment.DeliveryDeadline });
                }

            return assignments;
        }

        [HttpGet]
        public ActionResult<List<AdministratorViewModel>> GetAdminsInOrganisationByName(string orgName)
        {
            List<AdministratorViewModel> admins = new List<AdministratorViewModel>();
            List<Administrator> databaseAdminInOrg = this._httpClient
                .GetFromJsonAsync<List<Administrator>>($"Administrator/{orgName}")
                .Result;


            if (databaseAdminInOrg.Count >= 1)
                foreach (Administrator admin in databaseAdminInOrg)
                {
                    admins.Add(new AdministratorViewModel { Id = admin.Id, Token = admin.Token, FirstName = admin.FirstName, LastName = admin.LastName, Email = admin.Email });
                }

            return admins;
        }

        [HttpGet]
        public ActionResult<List<StudentViewModel>> GetStudentsInClassroomByClassroomName(string classroomName)
        {
            List<StudentViewModel> students = new List<StudentViewModel>();
            List<Student> databaseStudents = this._httpClient
                .GetFromJsonAsync<List<Student>>($"Student/{classroomName}")
                .Result;

            if (databaseStudents.Count >= 1)
                foreach (Student student in databaseStudents)
                {
                    students.Add(new StudentViewModel { Email = student.Email, FirstName = student.FirstName, LastName = student.LastName, Username = student.Username });
                }

            return students;
        }

        [HttpGet]
        public ActionResult<List<StudentViewModel>> GetStudentsNotInClassroomByClassroomName(string classroomName)
        {
            List<StudentViewModel> students = new List<StudentViewModel>();
            List<Student> databaseStudentsInClassroom = this._httpClient
                .GetFromJsonAsync<List<Student>>($"Student/{classroomName}")
                .Result;

            List<Student> databaseStudents = this._httpClient
                .GetFromJsonAsync<List<Student>>($"Student")
                .Result;

            if (databaseStudentsInClassroom.Count >= 1 || databaseStudents.Count >= 1)
            {
                foreach (Student student in databaseStudents)
                {
                    if (!databaseStudentsInClassroom.Any(a => a.Username == student.Username))
                    {
                        students.Add(new StudentViewModel { Email = student.Email, FirstName = student.FirstName, LastName = student.LastName, Username = student.Username });
                    }
                }
            }

            return students;
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
