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

        [HttpGet("Id/{id}")]
        public ActionResult<Organisation> GetOrganisationById(int id)
        {
            return Ok(this._depot.GetOrganisationById(id));
        }

        [HttpGet("Name/{organisationName}")]
        public ActionResult<Organisation> GetOrganisationByName(string organisationName)
        {
            return Ok(this._depot.GetOrganisationByName(organisationName));
        }

        [HttpGet("Organisations/{organisationName}/Administrators")]
        public ActionResult<Administrator> GetAdministratorsByOrganisation(string organisationName)
        {
            return Ok(this._depot.GetAdministratorsByOrganisation(organisationName));
        }

        [HttpPost("Name/{organisationName}/Administrators/Add/{adminUsername}")]
        public ActionResult AddAdministratorToOrganisation(string organisationName, string adminUsername)
        {
            if (string.IsNullOrWhiteSpace(organisationName) || string.IsNullOrWhiteSpace(adminUsername))
            {
                return BadRequest();
            }

            this._depot.AddAdministratorToOrganisation(organisationName, adminUsername);

            return Created(nameof(this.AddAdministratorToOrganisation), organisationName);
        }

        [HttpPost("Name/{organisationName}/Administrators/Remove/{adminUsername}")]
        public ActionResult RemoveAdministratorToOrganisation(string organisationName, string adminUsername)
        {
            if (string.IsNullOrWhiteSpace(organisationName) || string.IsNullOrWhiteSpace(adminUsername))
            {
                return BadRequest();
            }

            this._depot.RemoveAdministratorFromOrganisation(organisationName, adminUsername);

            return NoContent();
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

        [HttpDelete("Name/{organisationName}")]
        public ActionResult DeleteOrganisation(string organisationName)
        {
            this._depot.DeleteOrganisation(organisationName);
            return NoContent();
        }
    }
}
