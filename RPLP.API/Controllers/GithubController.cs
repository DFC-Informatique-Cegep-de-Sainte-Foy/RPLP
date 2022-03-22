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
            //_githubAction = new GithubApiAction("");
        }

    }
}
