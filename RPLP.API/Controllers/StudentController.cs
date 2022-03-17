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

        [HttpGet("{id}")]
        public ActionResult<Student> Get(int id)
        {
            return Ok(this._depot.GetStudentById(id));
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
    }
}
