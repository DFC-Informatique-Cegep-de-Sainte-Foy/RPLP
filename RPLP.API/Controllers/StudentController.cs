using Microsoft.AspNetCore.Mvc;
using RPLP.ENTITES;
using RPLP.JOURNALISATION;
using RPLP.SERVICES.InterfacesDepots;
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
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                "StudentController - Constructeur - p_depot passé en paramêtre est null"));
            }

            this._depot = p_depot;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Student>> Get()
        {
            try
            {
                return Ok(this._depot.GetStudents());
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
                if (id <= 0)
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString(),
                    "StudentController - GetStudentById - id passé en paramêtre hors limites"));
                }

                return Ok(this._depot.GetStudentById(id));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Username/{studerUsername}")]
        public ActionResult<Student> GetStudentByUsername(string studentUsername)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(studentUsername))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "StudentController - GetStudentByUsername - studentUsername passé en paramêtre est vide"));
                }

                return Ok(this._depot.GetStudentByUsername(studentUsername));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Username/{studerUsername}/Classrooms")]
        public ActionResult<List<Classroom>> GetStudentClasses(string studentUsername)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(studentUsername))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "StudentController - GetStudentClasses - studentUsername passé en paramêtre est vide"));
                }

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
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "StudentController - Post - p_student passé en paramêtre est null"));

                    return BadRequest();
                }

                if(p_student.Id <= 0)
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString(),
                    "StudentController - Post - Le id du paramêtre p_student passé en paramêtre est vide"));

                    return BadRequest();
                }

                if(!ModelState.IsValid)
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentException().ToString(), new StackTrace().ToString(),
                    "StudentController - Post - p_student passé en paramêtre n'est pas un modèle valide"));

                    return BadRequest();
                }

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
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "StudentController - DeleteStudent - studentUsername passé en paramêtre est vide"));
                }

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
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "StudentController - ReactivateStudent - username passé en paramêtre est vide"));
                }

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
