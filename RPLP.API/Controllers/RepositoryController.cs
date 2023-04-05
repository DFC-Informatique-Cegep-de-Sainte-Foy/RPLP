using Microsoft.AspNetCore.Mvc;
using RPLP.ENTITES;
using RPLP.JOURNALISATION;
using RPLP.ENTITES.InterfacesDepots;
using System.Diagnostics;

namespace RPLP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepositoryController : ControllerBase
    {
        private readonly IDepotRepository _depot;

        public RepositoryController(IDepotRepository p_depot)
        {
            if (p_depot == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                "RepositoryController - Constructeur - p_depot passé en paramêtre est null", 0));
            }

            this._depot = p_depot;
        }

        [HttpGet("Id/{id}")]
        public ActionResult<Repository> GetRepositoryById(int id)
        {
            try
            {
                if(id < 0)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "RepositoryController - GetRepositoryById - id passé en paramêtre est hors limites", 0));
                }

                Logging.Instance.Journal(new Log($"api/Repository/Id/{id}", 200, "RepositoryController - GET méthode GetRepositoryById"));

                return Ok(this._depot.GetRepositoryById(id));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Name/{repositoryName}")]
        public ActionResult<Repository> GetRepositoryByName(string repositoryName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(repositoryName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "RepositoryController - GetRepositoryByName - repositoryName passé en paramêtre est vide", 0));
                }

                Logging.Instance.Journal(new Log($"api/Repository/Name/{repositoryName}", 200, "RepositoryController - GET méthode GetRepositoryByName"));

                return Ok(this._depot.GetRepositoryByName(repositoryName));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult Post([FromBody] Repository p_repository)
        {
            try
            {
                if (p_repository == null)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "RepositoryController - Post - p_repository passé en paramêtre est vide", 0));

                    return BadRequest();
                }

                if (!ModelState.IsValid)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                      "RepositoryController - Post - p_repository passé en paramêtre n'est pas un modèle valide", 0));

                    return BadRequest();
                }

                Logging.Instance.Journal(new Log($"api/Repository", 201, "RepositoryController - POST méthode Post"));

                this._depot.UpsertRepository(p_repository);

                return Created(nameof(this.Post), p_repository);
            }
            catch (Exception)
            {

                throw;
            }
        }
        
        [HttpDelete("Name/{repositoryName}")]
        public ActionResult DeleteRepository(string repositoryName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(repositoryName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "RepositoryController - DeleteRepository - repositoryName passé en paramêtre est vide", 0));
                }

                Logging.Instance.Journal(new Log($"api/Repository/Name/{repositoryName}", 204, "RepositoryController - DELETE méthode DeleteRepository"));

                this._depot.DeleteRepository(repositoryName);
                return NoContent();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

