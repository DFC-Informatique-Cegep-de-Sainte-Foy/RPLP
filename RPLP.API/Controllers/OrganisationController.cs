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
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "replacement text"),
                "OrganisationController - Constructeur - p_depot passé en paramêtre est null", 0));
            }

            this._depot = p_depot;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Organisation>> Get()
        {
            try
            {
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
                if (id <= 0)
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "replacement text"),
                    "OrganisationController - GetOrganisationById - id passé en paramêtre est hors limites", 0));
                }

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
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "replacement text"),
                    "OrganisationController - GetOrganisationByName - organisationName passé en paramêtre est vide", 0));
                }

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
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "replacement text"),
                    "OrganisationController - GetAdministratorsByOrganisation - organisationName passé en paramêtre est vide", 0));
                }

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
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "replacement text"),
                       "OrganisationController - AddAdministratorToOrganisation - organisationName passé en paramêtre est vide", 0));

                    return BadRequest();
                }

                if (string.IsNullOrWhiteSpace(adminUsername))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "replacement text"),
                       "OrganisationController - AddAdministratorToOrganisation - adminUsername passé en paramêtre est vide", 0));

                    return BadRequest();
                }

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
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "replacement text"),
                           "OrganisationController - RemoveAdministratorToOrganisation - organisationName passé en paramêtre est vide", 0));

                    return BadRequest();
                }

                if (string.IsNullOrWhiteSpace(adminUsername))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "replacement text"),
                           "OrganisationController - RemoveAdministratorToOrganisation - adminUsername passé en paramêtre est vide", 0));

                    return BadRequest();
                }

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
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "replacement text"),
                               "OrganisationController - Post - p_organisation passé en paramêtre est vide", 0));

                    return BadRequest();
                }

                if (!ModelState.IsValid)
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "replacement text"),
                               "OrganisationController - Post - p_organisation passé en paramêtre n'est pas un modèle valide", 0));

                    return BadRequest();
                }

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
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "replacement text"),
                           "OrganisationController - DeleteOrganisation - organisationName passé en paramêtre est vide", 0));

                    return BadRequest();
                }

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
