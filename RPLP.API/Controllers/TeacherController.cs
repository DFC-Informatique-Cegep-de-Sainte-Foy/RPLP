using Microsoft.AspNetCore.Mvc;
using RPLP.ENTITES;
using RPLP.SERVICES.InterfacesDepots;

namespace RPLP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private readonly IDepotTeacher _depot;

        public TeacherController(IDepotTeacher p_depot)
        {
            this._depot = p_depot;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Teacher>> Get()
        {
            return Ok(this._depot.GetTeachers());
        }

        [HttpGet("Id/{id}")]
        public ActionResult<Teacher> GetById(int id)
        {
            return Ok(this._depot.GetTeacherById(id));
        }

        [HttpGet("Username/{username}")]
        public ActionResult<Teacher> GetByName(string username)
        {
            return Ok(this._depot.GetTeacherByName(username));
        }

        [HttpGet("Username/{username}/Classrooms")]
        public ActionResult<List<Organisation>> GetClassrooms(string username)
        {
            return Ok(this._depot.GetTeacherClasses(username));
        }

        [HttpPost]
        public ActionResult UpsertTeacher([FromBody] Teacher p_teacher)
        {
            if (p_teacher == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            this._depot.UpsertTeacher(p_teacher);

            return Created(nameof(this.UpsertTeacher), p_teacher);
        }

        [HttpDelete("Username/{username}")]
        public ActionResult DeleteTeacher(string username)
        {
            this._depot.DeleteTeacher(username);
            return NoContent();
        }
    }
}
