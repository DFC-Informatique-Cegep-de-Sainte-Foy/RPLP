using Microsoft.AspNetCore.Mvc;
using RPLP.ENTITES;
using RPLP.JOURNALISATION;
using RPLP.ENTITES.InterfacesDepots;
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
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "AdministratorController - Constructeur - Dépot passé en paramêtre null", 0));
            }

            this._depot = p_depotAdmin;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Administrator>> Get()
        {
            try
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log("api/Administrator", 200, "AdministratorController - GET méthode Get"));
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
                RPLP.JOURNALISATION.Logging.Journal(new Log("api/Administrator/Deactivated", 200, "AdministratorController - GET méthode GetDeactivated"));
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
                if (id < 0)
                {
                    RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "AdministratorController - GetAdministratorById - id passé en paramêtre n'est pas valide", 0));
                }

                RPLP.JOURNALISATION.Logging.Journal(new Log($"api/Administrator/Id/{id}", 200, "AdministratorController - GET méthode GetAdministratorById"));

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
                    RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "AdministratorController - GetAdministratorByUsername - username passé en paramêtre vide", 0));
                }

                RPLP.JOURNALISATION.Logging.Journal(new Log($"api/Administrator/Username/{username}", 200, "AdministratorController - GET méthode GetAdministratorByUsername"));

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
                    RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "AdministratorController - GetAdministratorByEmail - email passé en paramêtre vide", 0));
                }

                RPLP.JOURNALISATION.Logging.Journal(new Log($"api/Administrator/Email/{email}", 200, "AdministratorController - GET méthode GetAdministratorByEmail"));

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
                    RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                   "AdministratorController - GetAdminOrganisations - username passé en paramêtre vide", 0));
                }

                RPLP.JOURNALISATION.Logging.Journal(new Log($"api/Administrator/Username/{username}/Organisations", 200, "AdministratorController - GET méthode GetAdminOrganisations"));

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
                    RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "AdministratorController - GetAdminOrganisationsByEmail - email passé en paramêtre vide", 0));
                }

                RPLP.JOURNALISATION.Logging.Journal(new Log($"api/Administrator/Email/{email}/Organisations", 200, "AdministratorController - GET méthode GetAdminOrganisationsByEmail"));

                string? username = this._depot.GetAdministratorByEmail(email)?.Username;

                if (string.IsNullOrWhiteSpace(username))
                {
                    RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "AdministratorController - GetAdminOrganisationsByEmail - username récupéré à partir du email est vide", 0));

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
                    RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                   "AdministratorController - AddAdminToOrganisation - adminUsername passé en paramêtre est vide", 0));

                    return BadRequest();
                }

                if (string.IsNullOrWhiteSpace(organisationName))
                {
                    RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "AdministratorController - AddAdminToOrganisation - organisationName passé en paramêtre est vide", 0));

                    return BadRequest();
                }

                RPLP.JOURNALISATION.Logging.Journal(new Log($"api/Administrator/Username/{adminUsername}/Orgs/Add/{organisationName}", 201, "AdministratorController - POST méthode AddAdminToOrganisation"));

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
                    RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                  "AdministratorController - RemoveAdminFromOrganisation - adminUsername passé en paramêtre est vide", 0));

                    return BadRequest();
                }


                if (string.IsNullOrWhiteSpace(organisationName))
                {
                    RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "AdministratorController - RemoveAdminFromOrganisation - organisationName passé en paramêtre est vide", 0));

                    return BadRequest();
                }

                RPLP.JOURNALISATION.Logging.Journal(new Log($"api/Administrator/Username/{adminUsername}/Orgs/Remove/{organisationName}", 204, "AdministratorController - POST méthode RemoveAdminFromOrganisation"));

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
            if (p_admin == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log($"api/Administrator/", 201, "AdministratorController - POST méthode UpsertAdmin"));

                this._depot.UpsertAdministrator(p_admin);
            }

            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            return Created(nameof(this.UpsertAdmin), p_admin);
            //try
            //{
            //    if (p_admin == null)
            //    {
            //        RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
            //        "AdministratorController - UpsertAdmin - p_admin passé en paramêtre est null"));

            //        return BadRequest();
            //    }

            //    if (!ModelState.IsValid)
            //    {
            //        RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
            //       "AdministratorController - UpsertAdmin - p_admin n'est pas un model valide"));

            //        return BadRequest();
            //    }

            //    this._depot.UpsertAdministrator(p_admin);

            //    return Created(nameof(this.UpsertAdmin), p_admin);
            //}
            //catch (Exception)
            //{
            //    throw;
            //}

        }

        [HttpDelete("Username/{username}")]
        public ActionResult DeleteAdmin(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                   "AdministratorController - DeleteAdmin - username passé en paramêtre vide", 0));
                }

                RPLP.JOURNALISATION.Logging.Journal(new Log($"api/Administrator/Username/{username}", 204, "AdministratorController - DELETE méthode DeleteAdmin"));

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
                    RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "AdministratorController - ReactivateAdmin - username passé en paramêtre vide", 0));
                }

                RPLP.JOURNALISATION.Logging.Journal(new Log($"api/Administrator/Reactivate/{username}", 204, "AdministratorController - GET méthode ReactivateAdmin"));

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

