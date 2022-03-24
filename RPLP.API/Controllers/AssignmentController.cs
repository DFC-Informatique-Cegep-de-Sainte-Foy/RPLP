using Microsoft.AspNetCore.Mvc;
using RPLP.ENTITES;
using RPLP.SERVICES.InterfacesDepots;

namespace RPLP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentController : ControllerBase
    {
        private readonly IDepotAssignment _depot;

        public AssignmentController(IDepotAssignment p_depot)
        {
            this._depot = p_depot;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Assignment>> Get()
        {
            return Ok(this._depot.GetAssignments());
        }

        [HttpGet("Id/{id}")]
        public ActionResult<Assignment> GetAssignmentById(int id)
        {
            return Ok(this._depot.GetAssignmentById(id));
        }

        [HttpGet("Name/{assignmentName}")]
        public ActionResult<Assignment> GetAssignmentByName(string assignmentName)
        {
            return Ok(this._depot.GetAssignmentByName(assignmentName));
        }

        [HttpPost]
        public ActionResult Post([FromBody] Assignment p_assignment)
        {
            if (p_assignment == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            this._depot.UpsertAssignment(p_assignment);

            return Created(nameof(this.Post), p_assignment);
        }

        [HttpDelete("Name/{assignmentName}")]
        public ActionResult DeleteAssignment(string assignmentName)
        {
            this._depot.DeleteAssignment(assignmentName);
            return NoContent();
        }
    }
}
