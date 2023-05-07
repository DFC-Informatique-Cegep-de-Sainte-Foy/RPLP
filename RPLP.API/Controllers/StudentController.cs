using Microsoft.AspNetCore.Mvc;
using RPLP.ENTITES;
using RPLP.JOURNALISATION;
using RPLP.ENTITES.InterfacesDepots;
using System.Diagnostics;

namespace RPLP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IDepotStudent _depot;

        public StudentController(IDepotStudent p_depot)
        {
            if(p_depot == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                "StudentController - Constructeur - p_depot passé en paramêtre est null", 0));
            }

            this._depot = p_depot;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Student>> Get()
        {
            try
            {
                Logging.Instance.Journal(new Log("api/Student", 200, "StudentController - GET méthode Get"));

                return Ok(this._depot.GetStudents());
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        [HttpGet("Tutors")]
        public ActionResult<IEnumerable<Student>> GetTutors()
        {
            try
            {
                Logging.Instance.Journal(new Log("api/Student/Tutors", 200, "StudentController - GET méthode Get"));

                return Ok(this._depot.GetStudents().Where(s => s.Matricule is null or ""));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Deactivated")]
        public ActionResult<IEnumerable<Student>> GetDeactivated()
        {
            try
            {
                Logging.Instance.Journal(new Log("api/Student/Deactivated", 200, "StudentController - GET méthode GetDeactivated"));

                return Ok(this._depot.GetDeactivatedStudents());
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Id/{id}")]
        public ActionResult<Student> GetStudentById(int id)
        {
            try
            {
                if (id < 0)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "StudentController - GetStudentById - id passé en paramêtre hors limites", 0));
                }

                Logging.Instance.Journal(new Log($"api/Student/Id/{id}", 200, "StudentController - GET méthode GetStudentById"));

                return Ok(this._depot.GetStudentById(id));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Username/{studentUsername}")]
        public ActionResult<Student> GetStudentByUsername(string studentUsername)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(studentUsername))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "StudentController - GetStudentByUsername - studentUsername passé en paramêtre est vide", 0));
                }

                Logging.Instance.Journal(new Log($"api/Student/Username/{studentUsername}", 200, "StudentController - GET méthode GetStudentByUsername"));

                return Ok(this._depot.GetStudentByUsername(studentUsername));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Username/{studentUsername}/Classrooms")]
        public ActionResult<List<Classroom>> GetStudentClasses(string studentUsername)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(studentUsername))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "StudentController - GetStudentClasses - studentUsername passé en paramêtre est vide", 0));
                }

                Logging.Instance.Journal(new Log($"api/Student/Username/{studentUsername}/Classrooms", 200, "StudentController - GET méthode GetStudentClasses"));

                return Ok(this._depot.GetStudentClasses(studentUsername));
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        [HttpPost]
        public ActionResult Post([FromBody] Student p_student)
        {
            try
            {
                if (p_student == null)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "StudentController - Post - p_student passé en paramêtre est null", 0));

                    return BadRequest();
                }

                if(p_student.Id < 0)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "StudentController - Post - Le id du paramêtre p_student passé en paramêtre est vide", 0));

                    return BadRequest();
                }

                if(!ModelState.IsValid)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "StudentController - Post - p_student passé en paramêtre n'est pas un modèle valide", 0));

                    return BadRequest();
                }

                Logging.Instance.Journal(new Log($"api/Student", 201, "StudentController - POST méthode Post"));

                this._depot.UpsertStudent(p_student);

                return Created(nameof(this.Post), p_student);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpDelete("Username/{studentUsername}")]
        public ActionResult DeleteStudent(string studentUsername)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(studentUsername))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "StudentController - DeleteStudent - studentUsername passé en paramêtre est vide", 0));
                }

                Logging.Instance.Journal(new Log($"api/Student/Username/{studentUsername}", 204, "StudentController - DELETE méthode DeleteStudent"));

                this._depot.DeleteStudent(studentUsername);
                return NoContent();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Reactivate/{username}")]
        public ActionResult ReactivateStudent(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "StudentController - ReactivateStudent - username passé en paramêtre est vide", 0));
                }

                Logging.Instance.Journal(new Log($"api/Student/Reactivate/{username}", 204, "StudentController - GET méthode ReactivateStudent"));

                this._depot.ReactivateStudent(username);
                return NoContent();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
