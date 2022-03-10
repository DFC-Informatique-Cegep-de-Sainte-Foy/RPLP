using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RPLP.ENTITES;
using RPLP.SERVICES.InterfacesDepots;

namespace RPLP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassroomController : ControllerBase
    {
        private readonly IDepotClassroom _depotClassroom;

        public ClassroomController(IDepotClassroom p_depotClassroom)
        {
            this._depotClassroom = p_depotClassroom;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Classroom>> Get()
        {
            return Ok(this._depotClassroom.GetClassrooms());
        }

        [HttpGet("{id}")]
        public ActionResult<Classroom> Get(int id)
        {
            return Ok(this._depotClassroom.GetClassroomById(id));
        }

        [HttpPost]
        public ActionResult Post([FromBody] Classroom p_classroom)
        {
            if (p_classroom == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            this._depotClassroom.UpsertClassroom(p_classroom);

            return Created(nameof(this.Post), p_classroom);
        }
    }
}
