using Microsoft.AspNetCore.Mvc;
using RPLP.ENTITES;
using RPLP.SERVICES.InterfacesDepots;

namespace RPLP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganisationController : ControllerBase
    {
        private readonly IDepotOrganisation _depot;

        public OrganisationController(IDepotOrganisation p_depot)
        {
            this._depot = p_depot;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Organisation>> Get()
        {
            return Ok(this._depot.GetOrganisations());
        }

        [HttpGet("{id}")]
        public ActionResult<Organisation> Get(int id)
        {
            return Ok(this._depot.GetOrganisationById(id));
        }

        [HttpPost]
        public ActionResult Post([FromBody] Organisation p_organisation)
        {
            if (p_organisation == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            this._depot.UpsertOrganisation(p_organisation);

            return Created(nameof(this.Post), p_organisation);
        }
    }
}
