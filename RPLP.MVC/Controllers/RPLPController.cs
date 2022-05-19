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
        private object classroomName;

        public RPLPController()
        {
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

        #region Views

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

        #endregion

        #region Methodes prive

        private GestionDonneeViewModel getGestionDonneeModel()
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

                allOrgs.ForEach(organisation => model.AllOrgs.Add(new OrganisationViewModel { Id = organisation.Id, Name = organisation.Name }));
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
                    model.Organisations.Add(new OrganisationViewModel { Id = organisation.Id, Name = organisation.Name });
                });

            if (classroomsResult.Count >= 1)
                classroomsResult.ForEach(classroom =>
                {
                    model.Classes.Add(new ClassroomViewModel { Id = classroom.Id, Name = classroom.Name });
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
                    model.Assignments.Add(new AssignmentViewModel { Id = assignment.Id, Name = assignment.Name, Deadline = assignment.DeliveryDeadline, Description = assignment.Description });
                });

            List<Student> studentsResult = this._httpClient
                    .GetFromJsonAsync<List<Student>>($"Student")
                    .Result;
            if (studentsResult.Count >= 1)
                studentsResult.ForEach(student =>
                {
                    model.Students.Add(new StudentViewModel { Id = student.Id, Username = student.Username, Email = student.Email, FirstName = student.FirstName, LastName = student.LastName });
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

        #endregion

        #region ActionGet

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
                    classes.Add(new ClassroomViewModel { Id = classroom.Id, Name = classroom.Name, OrganisationName = classroom.OrganisationName });
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
                    assignments.Add(new AssignmentViewModel { Id = assignment.Id, Name = assignment.Name, Deadline = assignment.DeliveryDeadline });
                }
            }

            return assignments;
        }

        [HttpGet]
        public List<ClassroomViewModel> GetClassroomsOfTeacherInOrganisation(string p_teacherUsername, string p_organisationName)
        {
            var classes = new List<ClassroomViewModel>();
            string? teacherEmail = User.FindFirst(c => c.Type == ClaimTypes.Email)?.Value;

            List<Classroom>? databaseClasses = this._httpClient
                .GetFromJsonAsync<List<Classroom>>($"Teacher/Email/{teacherEmail}/Organisation/{p_organisationName}/Classrooms")
                .Result;

            foreach (Classroom classroom in databaseClasses)
            {
                classes.Add(new ClassroomViewModel { Id = classroom.Id, Name = classroom.Name, OrganisationName = classroom.OrganisationName });
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


            if (databaseAssignments.Count >= 1)
            {
                foreach (Assignment assignment in databaseAssignments)
                {
                    assignments.Add(new AssignmentViewModel { Id = assignment.Id, Name = assignment.Name });
                }
            }

            return assignments;
        }

        [HttpGet]
        public ActionResult<List<AdministratorViewModel>> GetAdminsNotInOrganisationByName(string orgName)
        {
            List<AdministratorViewModel> admins = new List<AdministratorViewModel>();

            List<Administrator> databaseAdminInOrg = this._httpClient
                .GetFromJsonAsync<List<Administrator>>($"Organisation/Name/{orgName}/Administrators")
                .Result;

            List<Administrator> databaseAdmin = this._httpClient
                .GetFromJsonAsync<List<Administrator>>($"Administrator")
                .Result;


            foreach (Administrator admin in databaseAdmin)
            {
                if (!databaseAdminInOrg.Any(a => a.Username == admin.Username))
                {
                    admins.Add(new AdministratorViewModel { Id = admin.Id, Username = admin.Username, Token = admin.Token, FirstName = admin.FirstName, LastName = admin.LastName, Email = admin.Email });
                }
            }

            return admins;
        }

        [HttpGet]
        public ActionResult<List<AdministratorViewModel>> GetAdminsInOrganisationByName(string orgName)
        {
            List<AdministratorViewModel> admins = new List<AdministratorViewModel>();

            List<Administrator> databaseAdminInOrg = this._httpClient
                .GetFromJsonAsync<List<Administrator>>($"Organisation/Name/{orgName}/Administrators")
                .Result;

            foreach (Administrator admin in databaseAdminInOrg)
            {
                admins.Add(new AdministratorViewModel { Id = admin.Id, Token = admin.Token, Username = admin.Username, FirstName = admin.FirstName, LastName = admin.LastName, Email = admin.Email });
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
                        teachers.Add(new TeacherViewModel { Id = teacher.Id, Username = teacher.Username, FirstName = teacher.FirstName, LastName = teacher.LastName, Email = teacher.Email });
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
                    teachers.Add(new TeacherViewModel { Id = teacher.Id, Username = teacher.Username, FirstName = teacher.FirstName, LastName = teacher.LastName, Email = teacher.Email });
                }

            return teachers;
        }

        [HttpGet]
        public ActionResult<List<AssignmentViewModel>> GetAssignmentInClassroomByClassroomName(string classroomName)
        {
            List<AssignmentViewModel> assignments = new List<AssignmentViewModel>();

            List<Assignment> databaseAssignmentInClassroom = this._httpClient
                .GetFromJsonAsync<List<Assignment>>($"Classroom/Name/{classroomName}/Assignments")
                .Result; ;

            if (databaseAssignmentInClassroom.Count >= 1)
                foreach (Assignment assignment in databaseAssignmentInClassroom)
                {
                    assignments.Add(new AssignmentViewModel { Id = assignment.Id, Name = assignment.Name, Deadline = assignment.DeliveryDeadline });
                }

            return assignments;
        }

        [HttpGet]
        public ActionResult<List<StudentViewModel>> GetStudentsInClassroomByClassroomName(string ClassroomName)
        {
            List<StudentViewModel> students = new List<StudentViewModel>();
            List<Student> databaseStudents = this._httpClient
                .GetFromJsonAsync<List<Student>>($"Classroom/Name/{ClassroomName}/Students")
                .Result;

            if (databaseStudents.Count >= 1)
                foreach (Student student in databaseStudents)
                {
                    students.Add(new StudentViewModel { Id = student.Id, Email = student.Email, FirstName = student.FirstName, LastName = student.LastName, Username = student.Username });
                }

            return students;
        }

        [HttpGet]
        public ActionResult<List<StudentViewModel>> GetStudentsNotInClassroomByClassroomName(string ClassroomName)
        {
            List<StudentViewModel> students = new List<StudentViewModel>();
            List<Student> databaseStudentsInClassroom = this._httpClient
                .GetFromJsonAsync<List<Student>>($"Classroom/Name/{ClassroomName}/Students")
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
                        students.Add(new StudentViewModel { Id = student.Id, Email = student.Email, FirstName = student.FirstName, LastName = student.LastName, Username = student.Username });
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

        #endregion

        #region ActionPOST

        [HttpPost]
        public ActionResult<string> POSTUpsertAdmin(int Id, string Username, string Token, string FirstName, string LastName, string Email)
        {
            Administrator admin = new Administrator { Id = Id, Username = Username, FirstName = FirstName, LastName = LastName, Email = Email, Token = Token };

            Task<HttpResponseMessage> response = this._httpClient
                                                 .PostAsJsonAsync<Administrator>($"Administrator", admin);
            response.Wait();

            return response.Result.StatusCode.ToString();
        }

        [HttpPost]
        public ActionResult<string> POSTUpsertOrg(int Id, string OrgName)
        {
            Organisation org = new Organisation { Id = Id, Name = OrgName };

            Task<HttpResponseMessage> response = this._httpClient
                                                 .PostAsJsonAsync<Organisation>($"Organisation", org);
            response.Wait();

            return response.Result.StatusCode.ToString();
        }

        [HttpPost]
        public ActionResult<string> POSTUpsertClassroom(int Id, string ClassroomName, string OrganisationName)
        {
            Classroom classroom = new Classroom { Id = Id, Name = ClassroomName, OrganisationName = OrganisationName, Assignments = new List<Assignment>(), Students = new List<Student>(), Teachers = new List<Teacher>() };

            if (Id != 0)
            {
                Classroom databaseClassroom = this._httpClient
                    .GetFromJsonAsync<Classroom>($"Classroom/Id/{Id}").Result;

                List<Assignment> databaseAssignments = this._httpClient
                    .GetFromJsonAsync<List<Assignment>>($"Classroom/Assignments/{databaseClassroom.Name}")
                    .Result;

                if (databaseAssignments.Count >= 1)
                    foreach (Assignment assignment in databaseAssignments)
                    {
                        POSTModifyAssignment(assignment.Id, assignment.Name, ClassroomName, assignment.Description, assignment.DeliveryDeadline);
                    }
            }

            Task<HttpResponseMessage> response = this._httpClient
                                                 .PostAsJsonAsync<Classroom>($"Classroom", classroom);
            response.Wait();

            return response.Result.StatusCode.ToString();
        }

        [HttpPost]
        public ActionResult<string> POSTUpsertStudent(int Id, string Email, string FirstName, string LastName, string Username)
        {
            Student student = new Student { Id = Id, Email = Email, FirstName = FirstName, LastName = LastName, Username = Username, Classes = new List<Classroom>() };

            Task<HttpResponseMessage> response = this._httpClient
                                                 .PostAsJsonAsync<Student>($"Student", student);
            response.Wait();

            return response.Result.StatusCode.ToString();
        }

        [HttpPost]
        public ActionResult<string> POSTUpsertBatchStudent(string StudentString)
        {
            string[] SplitStudents = StudentString.Split("\n");
            HttpResponseMessage result = new HttpResponseMessage();

            foreach (string rawStudent in SplitStudents)
            {
                string[] student = rawStudent.Split("=");

                string studentUsername = JsonConvert.DeserializeObject<string>(student[1].Replace(";", ""));
                string studentLastName = JsonConvert.DeserializeObject<string>(student[2].Replace(";", ""));
                string studentFirstName = JsonConvert.DeserializeObject<string>(student[3].Replace(";", ""));
                string studentEmail = studentUsername + "@csfoy.ca";

                Student studentObj = new Student { Id = 0, Email = studentEmail, FirstName = studentFirstName, LastName = studentLastName, Username = studentUsername, Classes = new List<Classroom>() };

                Task<HttpResponseMessage> response = this._httpClient
                                                        .PostAsJsonAsync<Student>($"Student", studentObj);

                result = response.Result;

            }
            return result.ToString();
        }

        [HttpGet]
        public async Task<IActionResult> DownloadSingleRepository(string organisationName, string classroomName, string assignmentName, string studentUsername)
        {
            Stream stream;
            FileStreamResult fileStreamResult;

            try
            {
                stream = await this._httpClient.GetStreamAsync($"Github/Telechargement/{organisationName}/{classroomName}/{assignmentName}/{studentUsername}");
                fileStreamResult = new FileStreamResult(stream, "application/octet-stream");
                fileStreamResult.FileDownloadName = $"{assignmentName}-{studentUsername}.zip";
            }
            catch (Exception)
            {
                return NotFound("Le dépôt na pas pu être trouvé. Il est peut-être privé et inaccessible à l'utilisateur.");
            }

            return fileStreamResult;
        }

        [HttpGet]
        public async Task<IActionResult> DownloadAllRepositoriesForAssignment(string organisationName, string classroomName, string assignmentName)
        {
            Stream stream;
            FileStreamResult fileStreamResult;

            try
            {
                stream = await this._httpClient.GetStreamAsync($"Github/Telechargement/{organisationName}/{classroomName}/{assignmentName}");
                fileStreamResult = new FileStreamResult(stream, "application/octet-stream");
                fileStreamResult.FileDownloadName = $"{assignmentName}-{classroomName}.zip";
            }
            catch (Exception)
            {
                return NotFound("Un ou plusieurs dépôts n'ont pas pu être trouvé. Ils sont peut-être privés et inaccessibles à l'utilisateur.");
            }

            return fileStreamResult;

        }

        [HttpPost]
        public ActionResult<string> POSTUpsertTeacher(int Id, string Email, string FirstName, string LastName, string Username)
        {
            Teacher teacher = new Teacher { Id = Id, Email = Email, FirstName = FirstName, LastName = LastName, Username = Username, Classes = new List<Classroom>() };

            Task<HttpResponseMessage> response = this._httpClient
                                                 .PostAsJsonAsync<Teacher>($"Teacher", teacher);
            response.Wait();

            return response.Result.StatusCode.ToString();
        }

        [HttpPost]
        public ActionResult<string> POSTNewAssignment(string Name, string ClassroomName, string Description, DateTime? DeliveryDeadline)
        {
            Assignment newAssignment = new Assignment { Id = 0, Name = Name, ClassroomName = ClassroomName, Description = Description, DeliveryDeadline = DeliveryDeadline, DistributionDate = DateTime.Now };

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

        [HttpPost]
        public ActionResult<string> POSTModifyAssignment(int Id, string Name, string ClassroomName, string Description, DateTime? DeliveryDeadline)
        {
            Assignment Assignment = new Assignment { Id = Id, Name = Name, Description = Description, DeliveryDeadline = DeliveryDeadline, ClassroomName = ClassroomName, DistributionDate = DateTime.Now };

            Task<HttpResponseMessage> response = this._httpClient
                                                 .PostAsJsonAsync<Assignment>($"Assignment", Assignment);
            response.Wait();

            return response.Result.StatusCode.ToString();
        }

        [HttpPost]
        public ActionResult<string> POSTAddAdminToOrg(string OrgName, string AdminUsername)
        {
            Task<HttpResponseMessage> response = this._httpClient
                                                 .PostAsJsonAsync($"Administrator/Username/{AdminUsername}/Orgs/Add/{OrgName}", "");
            response.Wait();

            return response.Result.StatusCode.ToString();
        }

        [HttpPost]
        public ActionResult<string> POSTAddStudentToClassroom(string ClassroomName, string StudentUsername)
        {
            Task<HttpResponseMessage> response = this._httpClient
                                                 .PostAsJsonAsync($"Classroom/Name/{ClassroomName}/Students/Add/{StudentUsername}", "");
            response.Wait();

            return response.Result.StatusCode.ToString();
        }

        [HttpPost]
        public ActionResult<string> POSTAddStudentToClassroomBatch(string ClassroomName, string StudentString)
        {
            string[] SplitStudents = StudentString.Split("\n");
            HttpResponseMessage result = new HttpResponseMessage();

            foreach (string rawStudent in SplitStudents)
            {
                string[] student = rawStudent.Split("=");

                string studentUsername = JsonConvert.DeserializeObject<string>(student[1].Replace(";", ""));

                Task<HttpResponseMessage> response = this._httpClient
                                                 .PostAsJsonAsync($"Classroom/Name/{ClassroomName}/Students/Add/{studentUsername}", "");

                result = response.Result;
            }

            return result.ToString();
        }

        [HttpPost]
        public ActionResult<string> POSTAddTeacherToClassroom(string ClassroomName, string TeacherUsername)
        {
            Task<HttpResponseMessage> response = this._httpClient
                                                 .PostAsJsonAsync($"Classroom/Name/{ClassroomName}/Teachers/Add/{TeacherUsername}", "");
            response.Wait();

            return response.Result.StatusCode.ToString();
        }

        [HttpPost]
        public ActionResult<string> POSTRemoveTeacherFromClassroom(string ClassroomName, string TeacherUsername)
        {
            Task<HttpResponseMessage> response = this._httpClient
                                                 .PostAsJsonAsync($"Classroom/Name/{ClassroomName}/Teachers/Remove/{TeacherUsername}", "");
            response.Wait();

            return response.Result.StatusCode.ToString();
        }

        [HttpPost]
        public ActionResult<string> POSTRemoveAssignmentFromClassroom(string ClassroomName, string AssignmentName)
        {
            Task<HttpResponseMessage> response = this._httpClient
                                                 .PostAsJsonAsync($"Classroom/Name/{ClassroomName}/Assignments/Remove/{AssignmentName}", "");
            response.Wait();

            return response.Result.StatusCode.ToString();
        }

        [HttpPost]
        public ActionResult<string> POSTRemoveStudentFromClassroom(string ClassroomName, string StudentUsername)
        {
            Task<HttpResponseMessage> response = this._httpClient
                                                 .PostAsJsonAsync($"Classroom/Name/{ClassroomName}/Students/Remove/{StudentUsername}", "");
            response.Wait();

            return response.Result.StatusCode.ToString();
        }

        [HttpPost]
        public ActionResult<string> POSTRemoveAdminFromOrg(string OrgName, string AdminUsername)
        {
            Task<HttpResponseMessage> response = this._httpClient
                                                 .PostAsJsonAsync($"Administrator/Username/{AdminUsername}/Orgs/Remove/{OrgName}", "");
            response.Wait();

            return response.Result.StatusCode.ToString();
        }

        [HttpPost]
        public ActionResult<string> POSTDeleteAdmin(string Username)
        {
            Task<HttpResponseMessage> response = this._httpClient
                                                 .DeleteAsync($"Administrator/Username/{Username}");
            response.Wait();

            return response.Result.StatusCode.ToString();
        }

        [HttpPost]
        public ActionResult<string> POSTDeleteAssignment(string AssignmentName)
        {
            Task<HttpResponseMessage> response = this._httpClient
                                                 .DeleteAsync($"Assignment/Name/{AssignmentName}");
            response.Wait();

            return response.Result.StatusCode.ToString();
        }

        [HttpPost]
        public ActionResult<string> POSTDeleteOrg(string OrgName)
        {
            Task<HttpResponseMessage> response = this._httpClient
                                                 .DeleteAsync($"Organisation/Name/{OrgName}");
            response.Wait();

            return response.Result.StatusCode.ToString();
        }

        [HttpPost]
        public ActionResult<string> POSTDeleteClassroom(string ClassroomName)
        {
            Task<HttpResponseMessage> response = this._httpClient
                                                 .DeleteAsync($"Classroom/Name/{ClassroomName}");
            response.Wait();

            return response.Result.StatusCode.ToString();
        }

        [HttpPost]
        public ActionResult<string> POSTDeleteStudent(string StudentUsername)
        {
            Task<HttpResponseMessage> response = this._httpClient
                                                 .DeleteAsync($"Student/Username/{StudentUsername}");
            response.Wait();

            return response.Result.StatusCode.ToString();
        }

        [HttpPost]
        public ActionResult<string> POSTDeleteTeacher(string TeacherUsername)
        {
            Task<HttpResponseMessage> response = this._httpClient
                                                 .DeleteAsync($"Teacher/Username/{TeacherUsername}");
            response.Wait();

            return response.Result.StatusCode.ToString();
        }

        #endregion
    }
}
