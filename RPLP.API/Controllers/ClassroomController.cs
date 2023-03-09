using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RPLP.ENTITES;
using RPLP.JOURNALISATION;
using RPLP.SERVICES.InterfacesDepots;
using System.Diagnostics;

namespace RPLP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassroomController : ControllerBase
    {
        private readonly IDepotClassroom _depot;

        public ClassroomController(IDepotClassroom p_depotClassroom)
        {
            if (p_depotClassroom == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ClassroomController - Constructeur - p_depotClassroom passé en paramêtre null", 0));
            }

            this._depot = p_depotClassroom;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Classroom>> Get()
        {
            try
            {
                Journalisation.Journaliser(new Log("api/Classroom", 200, "ClassroomController - GET méthode Get"));

                return Ok(this._depot.GetClassrooms());
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Id/{id}")]
        public ActionResult<Classroom> GetClassroomById(int id)
        {
            try
            {
                if (id < 0)
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ClassroomController - GetClassroomById - id passé en paramêtre est hors limites", 0));
                }

                Journalisation.Journaliser(new Log($"api/Classroom/Id/{id}", 200, "ClassroomController - GET méthode GetClassroomById"));

                return Ok(this._depot.GetClassroomById(id));
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet("Name/{classroomName}")]
        public ActionResult<Classroom> GetClassroomByName(string classroomName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ClassroomController - GetClassroomByName - classroomName passé en paramêtre est vide", 0));
                }

                Journalisation.Journaliser(new Log($"api/Classroom/Name/{classroomName}", 200, "ClassroomController - GET méthode GetClassroomByName"));

                return Ok(this._depot.GetClassroomByName(classroomName));
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet("Organisation/{organisationName}/Classroom")]
        public ActionResult<Classroom> GetClassroomsByOrganisationName(string organisationName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(organisationName))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ClassroomController - GetClassroomsByOrganisationName - organisationName passé en paramêtre est vide", 0));
                }

                Journalisation.Journaliser(new Log($"api/Classroom/Organisation/{organisationName}/Classroom", 200, "ClassroomController - GET méthode GetClassroomsByOrganisationName"));

                return Ok(this._depot.GetClassroomsByOrganisationName(organisationName));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Assignments/{classroomName}")]
        public ActionResult<List<Assignment>> GetAssignmentsByClassroomName(string classroomName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ClassroomController - GetAssignmentsByClassroomName - classroomName passé en paramêtre est vide", 0));
                }

                Journalisation.Journaliser(new Log($"api/Classroom/Assignments/{classroomName}", 200, "ClassroomController - GET méthode GetAssignmentsByClassroomName"));

                return Ok(this._depot.GetAssignmentsByClassroomName(classroomName));
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet("Name/{classroomName}/Teachers")]
        public ActionResult<List<Teacher>> GetTeachers(string classroomName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ClassroomController - GetTeachers - classroomName passé en paramêtre est vide", 0));
                }

                Journalisation.Journaliser(new Log($"api/Classroom/Name/{classroomName}/Teachers", 200, "ClassroomController - GET méthode GetTeachers"));

                return Ok(this._depot.GetTeachersByClassroomName(classroomName));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Name/{classroomName}/Students")]
        public ActionResult<List<Student>> GetStudents(string classroomName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ClassroomController - GetStudents - classroomName passé en paramêtre est vide", 0));
                }

                Journalisation.Journaliser(new Log($"api/Classroom/Name/{classroomName}/Students", 200, "ClassroomController - GET méthode GetStudents"));

                return Ok(this._depot.GetStudentsByClassroomName(classroomName));
            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpGet("Name/{classroomName}/Assignments")]
        public ActionResult<List<Assignment>> GetAssignments(string classroomName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ClassroomController - GetAssignments - classroomName passé en paramêtre est vide", 0));
                }

                Journalisation.Journaliser(new Log($"api/Classroom/Name/{classroomName}/Assignments", 200, "ClassroomController - GET méthode GetAssignments"));

                return Ok(this._depot.GetAssignmentsByClassroomName(classroomName));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("Name/{classroomName}/Teachers/Add/{teacherUsername}")]
        public ActionResult AddTeacherToClassroom(string classroomName, string teacherUsername)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "ClassroomController - AddTeacherToClassroom - classroomName passé en paramêtre est vide", 0));

                    return BadRequest();
                }

                if (string.IsNullOrWhiteSpace(teacherUsername))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "ClassroomController - AddTeacherToClassroom - teacherUsername passé en paramêtre est vide", 0));

                    return BadRequest();
                }

                Journalisation.Journaliser(new Log($"api/Classroom/Name/{classroomName}/Teachers/Add/{teacherUsername}", 201, "ClassroomController - POST méthode AddTeacherToClassroom"));

                this._depot.AddTeacherToClassroom(classroomName, teacherUsername);

                return Created(nameof(this.AddTeacherToClassroom), classroomName);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("Name/{classroomName}/Students/Add/{studentUsername}")]
        public ActionResult AddStudentToClassroom(string classroomName, string studentUsername)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                           "ClassroomController - AddStudentToClassroom - classroomName passé en paramêtre est vide", 0));

                    return BadRequest();
                }

                if (string.IsNullOrWhiteSpace(studentUsername))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                           "ClassroomController - AddStudentToClassroom - studentUsername passé en paramêtre est vide", 0));

                    return BadRequest();
                }

                Journalisation.Journaliser(new Log($"api/Classroom/Name/{classroomName}/Students/Add/{studentUsername}", 201, "ClassroomController - POST méthode AddStudentToClassroom"));

                this._depot.AddStudentToClassroom(classroomName, studentUsername);

                return Created(nameof(this.AddStudentToClassroom), classroomName);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("Name/{classroomName}/Students/Add/Matricule/{studentMatricule}")]
        public ActionResult AddStudentToClassroomMatricule(string classroomName, string studentMatricule)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                              "ClassroomController - AddStudentToClassroomMatricule - classroomName passé en paramêtre est vide", 0));

                    return BadRequest();
                }

                if (string.IsNullOrWhiteSpace(studentMatricule))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                              "ClassroomController - AddStudentToClassroomMatricule - classroomName passé en paramêtre est vide", 0));

                    return BadRequest();
                }

                Journalisation.Journaliser(new Log($"api/Classroom/Name/{classroomName}/Students/Add/Matricule/{studentMatricule}", 201, "ClassroomController - POST méthode AddStudentToClassroomMatricules"));

                this._depot.AddStudentToClassroomMatricule(classroomName, studentMatricule);

                return Created(nameof(this.AddStudentToClassroom), classroomName);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("Name/{classroomName}/Assignments/Add/{assignmentName}")]
        public ActionResult AddAssignmentToClassroom(string classroomName, string assignmentName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                                  "ClassroomController - AddAssignmentToClassroom - classroomName passé en paramêtre est vide", 0));

                    return BadRequest();
                }

                if (string.IsNullOrWhiteSpace(assignmentName))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                                  "ClassroomController - AddAssignmentToClassroom - assignmentName passé en paramêtre est vide", 0));

                    return BadRequest();
                }

                Journalisation.Journaliser(new Log($"api/Classroom/Name/{classroomName}/Assignments/Add/{assignmentName}", 201, "ClassroomController - POST méthode AddAssignmentToClassroom"));

                this._depot.AddAssignmentToClassroom(classroomName, assignmentName);

                return Created(nameof(this.AddAssignmentToClassroom), classroomName);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("Name/{classroomName}/Teachers/Remove/{teacherUsername}")]
        public ActionResult RemoveTeacherFromClassroom(string classroomName, string teacherUsername)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                                  "ClassroomController - RemoveTeacherFromClassroom - classroomName passé en paramêtre est vide", 0));

                    return BadRequest();
                }

                if (string.IsNullOrWhiteSpace(teacherUsername))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                                  "ClassroomController - RemoveTeacherFromClassroom - teacherUsername passé en paramêtre est vide"));

                    return BadRequest();
                }

                Journalisation.Journaliser(new Log($"api/Classroom/Name/{classroomName}/Teachers/Remove/{teacherUsername}", 204, "ClassroomController - POST méthode RemoveTeacherFromClassroom"));

                this._depot.RemoveTeacherFromClassroom(classroomName, teacherUsername);

                return NoContent();
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost("Name/{classroomName}/Students/Remove/{studentUsername}")]
        public ActionResult RemoveStudentFromClassroom(string classroomName, string studentUsername)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                                      "ClassroomController - RemoveStudentFromClassroom - classroomName passé en paramêtre est vide", 0));

                    return BadRequest();
                }

                if (string.IsNullOrWhiteSpace(studentUsername))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                                      "ClassroomController - RemoveStudentFromClassroom - studentUsername passé en paramêtre est vide", 0));

                    return BadRequest();
                }

                Journalisation.Journaliser(new Log($"api/Classroom/Name/{classroomName}/Students/Remove/{studentUsername}", 204, "ClassroomController - POST méthode RemoveStudentFromClassroom"));

                this._depot.RemoveStudentFromClassroom(classroomName, studentUsername);

                return NoContent();
            }
            catch (Exception)
            {
                throw;
            }
          
        }

        [HttpPost("Name/{classroomName}/Assignments/Remove/{assignmentName}")]
        public ActionResult RemoveAssignmentFromClassroom(string classroomName, string assignmentName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                                          "ClassroomController - RemoveAssignmentFromClassroom - classroomName passé en paramêtre est vide", 0));

                    return BadRequest();
                }

                if (string.IsNullOrWhiteSpace(assignmentName))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                                          "ClassroomController - RemoveAssignmentFromClassroom - assignmentName passé en paramêtre est vide", 0));

                    return BadRequest();
                }

                Journalisation.Journaliser(new Log($"api/Classroom/Name/{classroomName}/Assignments/Remove/{assignmentName}", 204, "ClassroomController - POST méthode RemoveAssignmentFromClassroom"));

                this._depot.RemoveAssignmentFromClassroom(classroomName, assignmentName);

                return NoContent();
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public ActionResult UpsertClassroom([FromBody] Classroom p_classroom)
        {
            try
            {
                if (p_classroom == null)
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                                             "ClassroomController - UpsertClassroom - p_classroom passé en paramêtre est null", 0));

                    return BadRequest();
                }

                if (!ModelState.IsValid)
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                                             "ClassroomController - UpsertClassroom - p_classroom passé en paramêtre n'est pas valide", 0));

                    return BadRequest();
                }

                Journalisation.Journaliser(new Log($"api/Classroom", 201, "ClassroomController - POST méthode UpsertClassroom"));

                this._depot.UpsertClassroom(p_classroom);

                return Created(nameof(this.UpsertClassroom), p_classroom);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpDelete("Name/{classroomName}")]
        public ActionResult DeleteClassroom(string classroomName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                                          "ClassroomController - DeleteClassroom - classroomName passé en paramêtre est vide", 0));

                    return BadRequest();
                }

                Journalisation.Journaliser(new Log($"api/Classroom/Name/{classroomName}", 204, "ClassroomController - DELETE méthode DeleteClassroom"));

                this._depot.DeleteClassroom(classroomName);
                return NoContent();
            }
            catch (Exception)
            {
                throw;
            }
           
        }
    }
}
