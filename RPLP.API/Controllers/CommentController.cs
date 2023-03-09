using Microsoft.AspNetCore.Mvc;
using RPLP.DAL.SQL.Depots;
using RPLP.ENTITES;
using RPLP.JOURNALISATION;
using RPLP.SERVICES.InterfacesDepots;
using System.Diagnostics;

namespace RPLP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly IDepotComment _depot;

        public CommentController(IDepotComment p_depot)
        {
            if (p_depot == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "CommentController - Constructeur - p_depot passé en paramêtre null", 0));
            }
            this._depot = p_depot;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Comment>> Get()
        {
            try
            {
                Journalisation.Journaliser(new Log("api/Comment", 200, "CommentController - GET méthode Get"));

                return Ok(this._depot.GetComments());
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Id/{id}")]
        public ActionResult<Comment> GetCommentById(int id)
        {
            try
            {
                if (id < 0)
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "CommentController - GetCommentById - id passé en paramêtre est hors limites", 0));
                }

                Journalisation.Journaliser(new Log($"api/Comment/Id/{id}", 200, "CommentController - GET méthode GetCommentById"));

                return Ok(this._depot.GetCommentById(id));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult Post([FromBody] Comment p_comment)
        {
            try
            {
                if (p_comment == null)
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "CommentController - Post - p_comment passé en paramêtre est vide", 0));

                    return BadRequest();
                }

                if (!ModelState.IsValid)
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "CommentController - Post - p_comment passé en paramêtre n'est pas valide", 0));

                    return BadRequest();
                }

                Journalisation.Journaliser(new Log($"api/Comment", 201, "CommentController - POST méthode Post"));

                this._depot.UpsertComment(p_comment);

                return Created(nameof(this.Post), p_comment);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpDelete("Id/{id}")]
        public ActionResult DeleteComment(int id)
        {
            try
            {
                if (id < 0)
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "CommentController - DeleteComment - id passé en paramêtre est hors limites", 0));
                }

                Journalisation.Journaliser(new Log($"api/Comment/Id/{id}", 204, "CommentController - GET méthode DeleteComment"));

                this._depot.DeleteComment(id);
                return NoContent();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
