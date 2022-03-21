using Microsoft.AspNetCore.Mvc;
using RPLP.DAL.DTO.Json;
using RPLP.SERVICES.Github;

namespace RPLP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GithubController : ControllerBase
    {
        private GithubApiAction _githubAction;

        public GithubController()
        {
            _githubAction = new GithubApiAction("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");
        }

        [HttpGet]
        public ActionResult<IEnumerable<Repository_JSONDTO>> Get()
        {
            return Ok(this._githubAction.GetOrganisationRepositoriesGithub("TestingRPLP"));
        }
    }
}
