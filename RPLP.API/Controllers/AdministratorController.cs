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

        public AdministratorController(IDepotAdminitrator p_depotAdmin)
        {
            this._depot = p_depotAdmin;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Administrator>> Get()
        {
            return Ok(this._depot.GetAdministrators());
        }

        [HttpGet("Id/{id}")]
        public ActionResult<Administrator> GetById(int id)
        {
            return Ok(this._depot.GetAdministratorById(id));
        }

        [HttpGet("Username/{username}")]
        public ActionResult<Administrator> GetByName(string username)
        {
            return Ok(this._depot.GetAdministratorByName(username));
        }

        [HttpGet("Username/{username}/Organisations")]
        public ActionResult<List<Organisation>> GetOrganisations(string username)
        {
            return Ok(this._depot.GetAdminOrganisations(username));
        }


        [HttpPost("Username/{adminUsername}/Orgs/Add/{organisationName}")]
        public ActionResult AddAdminToOrganisation(string adminUsername, string organisationName)
        {
            if (string.IsNullOrWhiteSpace(adminUsername) || string.IsNullOrWhiteSpace(organisationName))
            {
                return BadRequest();
            }

            this._depot.JoinOrganisation(adminUsername, organisationName);

            return Created(nameof(this.AddAdminToOrganisation), adminUsername);
        }

        [HttpPost("Username/{adminUsername}/Orgs/Remove/{organisationName}")]
        public ActionResult RemoveAdminFromOrganisation(string adminUsername, string organisationName)
        {
            if (string.IsNullOrWhiteSpace(adminUsername) || string.IsNullOrWhiteSpace(organisationName))
            {
                return BadRequest();
            }

            this._depot.LeaveOrganisation(adminUsername, organisationName);

            return NoContent(); 
        }

        [HttpPost]
        public ActionResult UpsertAdmin([FromBody] Administrator p_admin)
        {
            if (p_admin == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            this._depot.UpsertAdministrator(p_admin);

            return Created(nameof(this.UpsertAdmin), p_admin);
        }

        [HttpDelete("Username/{username}")]
        public ActionResult DeleteAdmin(string username)
        {
            this._depot.DeleteAdministrator(username);
            return NoContent();
        }
    }
}

