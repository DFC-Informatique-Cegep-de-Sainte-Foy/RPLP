using Microsoft.AspNetCore.Mvc;
using RPLP.ENTITES;
using RPLP.SERVICES.InterfacesDepots;

namespace RPLP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepositoryController : ControllerBase
    {
        private readonly IDepotRepository _depot;

        public RepositoryController(IDepotRepository p_depot)
        {
            this._depot = p_depot;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Repository>> Get()
        {
            return Ok(this._depot.GetRepositories());
        }

        [HttpGet("{id}")]
        public ActionResult<Repository> Get(int id)
        {
            return Ok(this._depot.GetRepositoryById(id));
        }

        [HttpPost]
        public ActionResult Post([FromBody] Repository p_repository)
        {
            if (p_repository == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            this._depot.UpsertRepository(p_repository);

            return Created(nameof(this.Post), p_repository);
        }
    }
}
