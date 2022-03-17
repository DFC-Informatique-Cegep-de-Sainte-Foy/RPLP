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

        [HttpGet("{id}")]
        public ActionResult<Teacher> Get(int id)
        {
            return Ok(this._depot.GetTeacherById(id));
        }

        [HttpPost]
        public ActionResult Post([FromBody] Teacher p_teacher)
        {
            if (p_teacher == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            this._depot.UpsertTeacher(p_teacher);

            return Created(nameof(this.Post), p_teacher);
        }
    }
}
