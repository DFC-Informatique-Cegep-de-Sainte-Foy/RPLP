using Microsoft.AspNetCore.Mvc;
using RPLP.ENTITES;
using RPLP.SERVICES.InterfacesDepots;

namespace RPLP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdministratorController : ControllerBase
    {
        private readonly IDepotAdminitrator _depotAdmin;

        public AdministratorController(IDepotAdminitrator p_depotAdmin)
        {
            this._depotAdmin = p_depotAdmin;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Administrator>> Get()
        {
            return Ok(this._depotAdmin.GetAdministrators());
        }

        [HttpGet("Id/{id}")]
        public ActionResult<Administrator> GetById(int id)
        {
            return Ok(this._depotAdmin.GetAdministratorById(id));
        }

        [HttpGet("Username/{username}")]
        public ActionResult<Administrator> GetByName(string username)
        {
            return Ok(this._depotAdmin.GetAdministratorByName(username));
        }

        [HttpGet("Username/{username}/Organisations")]
        public ActionResult<List<Organisation>> GetOrganisations(string username)
        {
            return Ok(this._depotAdmin.GetAdminOrganisations(username));
        }


        [HttpPost("Username/{adminUsername}/Orgs/Add/{organisationName}")]
        public ActionResult AddAdminToOrganisation(string adminUsername, string organisationName)
        {
            if (string.IsNullOrWhiteSpace(adminUsername) || string.IsNullOrWhiteSpace(organisationName))
            {
                return BadRequest();
            }

            this._depotAdmin.JoinOrganisation(adminUsername, organisationName);

            return Created(nameof(this.AddAdminToOrganisation), adminUsername);
        }

        [HttpPost("Username/{adminUsername}/Orgs/Remove/{organisationName}")]
        public ActionResult RemoveAdminFromOrganisation(string adminUsername, string organisationName)
        {
            if (string.IsNullOrWhiteSpace(adminUsername) || string.IsNullOrWhiteSpace(organisationName))
            {
                return BadRequest();
            }

            this._depotAdmin.LeaveOrganisation(adminUsername, organisationName);

            return Created(nameof(this.RemoveAdminFromOrganisation), adminUsername);
        }

        [HttpPost]
        public ActionResult UpsertAdmin([FromBody] Administrator p_admin)
        {
            if (p_admin == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            this._depotAdmin.UpsertAdministrator(p_admin);

            return Created(nameof(this.UpsertAdmin), p_admin);
        }

        [HttpDelete("Username/{username}")]
        public ActionResult DeleteAdmin(string username)
        {
            this._depotAdmin.DeleteAdministrator(username);
            return NoContent();
        }
    }
}

