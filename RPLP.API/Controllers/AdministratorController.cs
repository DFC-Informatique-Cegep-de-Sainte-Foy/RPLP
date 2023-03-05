using Microsoft.AspNetCore.Mvc;
using RPLP.ENTITES;
using RPLP.JOURNALISATION;
using RPLP.SERVICES.InterfacesDepots;
using System.Diagnostics;

namespace RPLP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdministratorController : ControllerBase
    {
        private readonly IDepotAdministrator _depot;

        public AdministratorController(IDepotAdministrator p_depotAdmin)
        {
            if (p_depotAdmin == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "AdministratorController - Constructeur - Dépot passé en paramêtre null"));
            }

            this._depot = p_depotAdmin;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Administrator>> Get()
        {
            try
            {
                return Ok(this._depot.GetAdministrators());
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Deactivated")]
        public ActionResult<IEnumerable<Administrator>> GetDeactivated()
        {
            try
            {
                return Ok(this._depot.GetDeactivatedAdministrators());
            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpGet("Id/{id}")]
        public ActionResult<Administrator> GetAdministratorById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                        "AdministratorController - GetAdministratorById - id passé en paramêtre n'est pas valide"));
                }

                return Ok(this._depot.GetAdministratorById(id));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Username/{username}")]
        public ActionResult<Administrator> GetAdministratorByUsername(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                        "AdministratorController - GetAdministratorByUsername - username passé en paramêtre vide"));
                }

                return Ok(this._depot.GetAdministratorByUsername(username));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Email/{email}")]
        public ActionResult<Administrator> GetAdministratorByEmail(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "AdministratorController - GetAdministratorByEmail - email passé en paramêtre vide"));
                }

                return Ok(this._depot.GetAdministratorByEmail(email));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Username/{username}/Organisations")]
        public ActionResult<List<Organisation>> GetAdminOrganisations(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "AdministratorController - GetAdminOrganisations - username passé en paramêtre vide"));
                }

                return Ok(this._depot.GetAdminOrganisations(username));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Email/{email}/Organisations")]
        public ActionResult<List<Organisation>> GetAdminOrganisationsByEmail(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "AdministratorController - GetAdminOrganisationsByEmail - email passé en paramêtre vide"));
                }

                string? username = this._depot.GetAdministratorByEmail(email)?.Username;

                if (string.IsNullOrWhiteSpace(username))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "AdministratorController - GetAdminOrganisationsByEmail - username récupéré à partir du email est vide"));

                    return BadRequest();
                }

                return Ok(this._depot.GetAdminOrganisations(username));
            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpPost("Username/{adminUsername}/Orgs/Add/{organisationName}")]
        public ActionResult AddAdminToOrganisation(string adminUsername, string organisationName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(adminUsername))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "AdministratorController - AddAdminToOrganisation - adminUsername passé en paramêtre est vide"));

                    return BadRequest();
                }

                if (string.IsNullOrWhiteSpace(organisationName))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "AdministratorController - AddAdminToOrganisation - organisationName passé en paramêtre est vide"));

                    return BadRequest();
                }

                this._depot.JoinOrganisation(adminUsername, organisationName);

                return Created(nameof(this.AddAdminToOrganisation), adminUsername);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("Username/{adminUsername}/Orgs/Remove/{organisationName}")]
        public ActionResult RemoveAdminFromOrganisation(string adminUsername, string organisationName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(adminUsername))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                  "AdministratorController - RemoveAdminFromOrganisation - adminUsername passé en paramêtre est vide"));

                    return BadRequest();
                }


                if (string.IsNullOrWhiteSpace(organisationName))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "AdministratorController - RemoveAdminFromOrganisation - organisationName passé en paramêtre est vide"));

                    return BadRequest();
                }

                this._depot.LeaveOrganisation(adminUsername, organisationName);

                return NoContent();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult UpsertAdmin([FromBody] Administrator p_admin)
        {
            try
            {
                if (p_admin == null)
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "AdministratorController - UpsertAdmin - p_admin passé en paramêtre est null"));

                    return BadRequest();
                }

                if (!ModelState.IsValid)
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "AdministratorController - UpsertAdmin - p_admin n'est pas un model valide"));

                    return BadRequest();
                }

                this._depot.UpsertAdministrator(p_admin);

                return Created(nameof(this.UpsertAdmin), p_admin);
            }
            catch (Exception)
            {
                throw;
            }
           
        }

        [HttpDelete("Username/{username}")]
        public ActionResult DeleteAdmin(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "AdministratorController - DeleteAdmin - username passé en paramêtre vide"));
                }

                this._depot.DeleteAdministrator(username);
                return NoContent();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Reactivate/{username}")]
        public ActionResult ReactivateAdmin(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "AdministratorController - ReactivateAdmin - username passé en paramêtre vide"));
                }

                this._depot.ReactivateAdministrator(username);
                return NoContent();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

