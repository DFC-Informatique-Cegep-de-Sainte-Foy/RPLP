using Microsoft.AspNetCore.Mvc;
using RPLP.ENTITES;
using RPLP.SERVICES.InterfacesDepots;

namespace RPLP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdministratorController : ControllerBase
    {
        private readonly IDepotAdministrator _depot;

        public AdministratorController(IDepotAdministrator p_depotAdmin)
        {
            this._depot = p_depotAdmin;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Administrator>> Get()
        {
            return Ok(this._depot.GetAdministrators());
        }

        [HttpGet("Id/{id}")]
        public ActionResult<Administrator> GetAdministratorById(int id)
        {
            return Ok(this._depot.GetAdministratorById(id));
        }

        [HttpGet("Username/{username}")]
        public ActionResult<Administrator> GetAdministratorByUsername(string username)
        {
            return Ok(this._depot.GetAdministratorByUsername(username));
        }

        [HttpGet("Email/{email}")]
        public ActionResult<Administrator> GetAdministratorByEmail(string email)
        {
            return Ok(this._depot.GetAdministratorByEmail(email));
        }

        [HttpGet("Username/{username}/Organisations")]
        public ActionResult<List<Organisation>> GetAdminOrganisations(string username)
        {
            return Ok(this._depot.GetAdminOrganisations(username));
        }

        [HttpGet("Email/{email}/Organisations")]
        public ActionResult<List<Organisation>> GetAdminOrganisationsByEmail(string email)
        {
            string? username = this._depot.GetAdministratorByEmail(email)?.Username;

            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest();
            }

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

            try
            {
                this._depot.UpsertAdministrator(p_admin);
            }
            
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

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

