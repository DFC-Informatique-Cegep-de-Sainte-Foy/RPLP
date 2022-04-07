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

        [HttpGet("Username/{studerUsername}")]
        public ActionResult<Student> GetStudentByUsername(string studentUsername)
        {
            return Ok(this._depot.GetStudentByUsername(studentUsername));
        }

        [HttpGet("Username/{studerUsername}/Classrooms")]
        public ActionResult<List<Classroom>> GetStudentClasses(string studentUsername)
        {
            return Ok(this._depot.GetStudentClasses(studentUsername));
        }              
               

        [HttpPost]
        public ActionResult Post([FromBody] Student p_student)
        {
            if (p_student == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                this._depot.UpsertStudent(p_student);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Created(nameof(this.Post), p_student);
        }

        [HttpDelete("Username/{username}")]
        public ActionResult DeleteStudent(string studentUsername)
        {
            this._depot.DeleteStudent(studentUsername);
            return NoContent();
        }
    }
}
