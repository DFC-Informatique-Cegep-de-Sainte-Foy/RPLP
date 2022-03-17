using Microsoft.AspNetCore.Mvc;
using RPLP.ENTITES;
using RPLP.SERVICES.InterfacesDepots;

namespace RPLP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdministratorController : ControllerBase
    {
        private readonly IDepotAdminitrator _depot;

        public AdministratorController(IDepotAdminitrator p_depot)
        {
            this._depot = p_depot;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Administrator>> Get()
        {
            return Ok(this._depot.GetAdministrators());
        }

        [HttpGet("{id}")]
        public ActionResult<Administrator> Get(int id)
        {
            return Ok(this._depot.GetAdministratorById(id));
        }

        [HttpPost]
        public ActionResult Post([FromBody] Administrator p_admin)
        {
            if (p_admin == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            this._depot.UpsertAdministrator(p_admin);

            return Created(nameof(this.Post), p_admin);
        }
    }
}
