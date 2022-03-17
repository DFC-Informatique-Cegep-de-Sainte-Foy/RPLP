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
        private readonly IDepotClassroom _depot;

        public ClassroomController(IDepotClassroom p_depotClassroom)
        {
            this._depot = p_depotClassroom;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Classroom>> Get()
        {
            return Ok(this._depot.GetClassrooms());
        }

        [HttpGet("{id}")]
        public ActionResult<Classroom> Get(int id)
        {
            return Ok(this._depot.GetClassroomById(id));
        }

        [HttpPost]
        public ActionResult Post([FromBody] Classroom p_classroom)
        {
            if (p_classroom == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            this._depot.UpsertClassroom(p_classroom);

            return Created(nameof(this.Post), p_classroom);
        }
    }
}
