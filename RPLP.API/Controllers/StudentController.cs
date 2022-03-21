using Microsoft.AspNetCore.Mvc;
using RPLP.ENTITES;
using RPLP.SERVICES.InterfacesDepots;

namespace RPLP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IDepotStudent _depot;

        public StudentController(IDepotStudent p_depot)
        {
            this._depot = p_depot;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Student>> Get()
        {
            return Ok(this._depot.GetStudents());
        }

        [HttpGet("Id/{id}")]
        public ActionResult<Student> GetStudentById(int id)
        {
            return Ok(this._depot.GetStudentById(id));
        }

        [HttpGet("Username/{username}")]
        public ActionResult<Student> GetStudentByUsername(string username)
        {
            return Ok(this._depot.GetStudentByUsername(username));
        }

        [HttpGet("Username/{username}/Classrooms")]
        public ActionResult<List<Organisation>> GetStudentClasses(string username)
        {
            return Ok(this._depot.GetStudentClasses(username));
        }


        [HttpPost]
        public ActionResult Post([FromBody] Student p_student)
        {
            if (p_student == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            this._depot.UpsertStudent(p_student);

            return Created(nameof(this.Post), p_student);
        }

        [HttpDelete("Username/{username}")]
        public ActionResult DeleteStudent(string username)
        {
            this._depot.DeleteStudent(username);
            return NoContent();
        }
    }
}
