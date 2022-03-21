using Microsoft.AspNetCore.Mvc;
using RPLP.ENTITES;
using RPLP.SERVICES.InterfacesDepots;

namespace RPLP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly IDepotComment _depot;

        public CommentController(IDepotComment p_depot)
        {
            this._depot = p_depot;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Comment>> Get()
        {
            return Ok(this._depot.GetComments());
        }

        [HttpGet("Id/{id}")]
        public ActionResult<Comment> GetCommentById(int id)
        {
            return Ok(this._depot.GetCommentById(id));
        }

        [HttpPost]
        public ActionResult Post([FromBody] Comment p_comment)
        {
            if (p_comment == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            this._depot.UpsertComment(p_comment);

            return Created(nameof(this.Post), p_comment);
        }

        [HttpDelete("Id/{id}")]
        public ActionResult DeleteComment(int id)
        {
            this._depot.Delete(id);
            return NoContent();
        }
    }
}
