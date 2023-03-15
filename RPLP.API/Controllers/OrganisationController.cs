using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RPLP.ENTITES;
using RPLP.JOURNALISATION;
using RPLP.SERVICES.InterfacesDepots;
using System.Diagnostics;

namespace RPLP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganisationController : ControllerBase
    {
        private readonly IDepotOrganisation _depot;

        public OrganisationController(IDepotOrganisation p_depot)
        {
            if (p_depot == null)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                "OrganisationController - Constructeur - p_depot passé en paramêtre est null", 0));
            }

            this._depot = p_depot;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Organisation>> Get()
        {
            try
            {
                Logging.Journal(new Log("api/Organisation", 200, "OrganisationController - GET méthode Get"));

                return Ok(this._depot.GetOrganisations());
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Id/{id}")]
        public ActionResult<Organisation> GetOrganisationById(int id)
        {
            try
            {
                if (id < 0)
                {
                    RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "OrganisationController - GetOrganisationById - id passé en paramêtre est hors limites", 0));
                }

                Logging.Journal(new Log($"api/Organisation/Id/{id}", 200, "OrganisationController - GET méthode GetOrganisationById"));

                return Ok(this._depot.GetOrganisationById(id));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Name/{organisationName}")]
        public ActionResult<Organisation> GetOrganisationByName(string organisationName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(organisationName))
                {
                    RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "OrganisationController - GetOrganisationByName - organisationName passé en paramêtre est vide", 0));
                }

                Logging.Journal(new Log($"api/Organisation/Name/{organisationName}", 200, "OrganisationController - GET méthode GetOrganisationByName"));

                return Ok(this._depot.GetOrganisationByName(organisationName));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Name/{organisationName}/Administrators")]
        public ActionResult<List<Administrator>> GetAdministratorsByOrganisation(string organisationName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(organisationName))
                {
                    RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "OrganisationController - GetAdministratorsByOrganisation - organisationName passé en paramêtre est vide", 0));
                }

                Logging.Journal(new Log($"api/Organisation/Name/{organisationName}/Administrators", 200, "OrganisationController - GET méthode GetAdministratorsByOrganisation"));

                return Ok(this._depot.GetAdministratorsByOrganisation(organisationName));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("Name/{organisationName}/Administrators/Add/{adminUsername}")]
        public ActionResult AddAdministratorToOrganisation(string organisationName, string adminUsername)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(organisationName))
                {
                    RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "OrganisationController - AddAdministratorToOrganisation - organisationName passé en paramêtre est vide", 0));

                    return BadRequest();
                }

                if (string.IsNullOrWhiteSpace(adminUsername))
                {
                    RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "OrganisationController - AddAdministratorToOrganisation - adminUsername passé en paramêtre est vide", 0));

                    return BadRequest();
                }

                Logging.Journal(new Log($"api/Organisation/Name/{organisationName}/Administrators/Add/{adminUsername}", 201, "OrganisationController - POST méthode AddAdministratorToOrganisation"));

                this._depot.AddAdministratorToOrganisation(organisationName, adminUsername);

                return Created(nameof(this.AddAdministratorToOrganisation), organisationName);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("Name/{organisationName}/Administrators/Remove/{adminUsername}")]
        public ActionResult RemoveAdministratorToOrganisation(string organisationName, string adminUsername)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(organisationName))
                {
                    RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                           "OrganisationController - RemoveAdministratorToOrganisation - organisationName passé en paramêtre est vide", 0));

                    return BadRequest();
                }

                if (string.IsNullOrWhiteSpace(adminUsername))
                {
                    RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                           "OrganisationController - RemoveAdministratorToOrganisation - adminUsername passé en paramêtre est vide", 0));

                    return BadRequest();
                }

                Logging.Journal(new Log($"api/Organisation/Name/{organisationName}/Administrators/Remove/{adminUsername}", 204, "OrganisationController - POST méthode RemoveAdministratorToOrganisation"));

                this._depot.RemoveAdministratorFromOrganisation(organisationName, adminUsername);

                return NoContent();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult Post([FromBody] Organisation p_organisation)
        {
            try
            {
                if (p_organisation == null)
                {
                    RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                               "OrganisationController - Post - p_organisation passé en paramêtre est vide", 0));

                    return BadRequest();
                }

                if (!ModelState.IsValid)
                {
                    RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                               "OrganisationController - Post - p_organisation passé en paramêtre n'est pas un modèle valide", 0));

                    return BadRequest();
                }

                Logging.Journal(new Log($"api/Organisation", 201, "OrganisationController - POST méthode Post"));

                this._depot.UpsertOrganisation(p_organisation);

                return Created(nameof(this.Post), p_organisation);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpDelete("Name/{organisationName}")]
        public ActionResult DeleteOrganisation(string organisationName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(organisationName))
                {
                    RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                           "OrganisationController - DeleteOrganisation - organisationName passé en paramêtre est vide", 0));

                    return BadRequest();
                }

                Logging.Journal(new Log($"api/Organisation/Name/{organisationName}", 204, "OrganisationController - DELETE méthode DeleteOrganisation"));

                this._depot.DeleteOrganisation(organisationName);
                return NoContent();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
