using Microsoft.AspNetCore.Mvc;
using RPLP.DAL.DTO.Json;
using RPLP.DAL.DTO.Sql;
using RPLP.DAL.SQL.Depots;
using RPLP.SERVICES.Github;

namespace RPLP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GithubController : ControllerBase
    {
        private GithubApiAction _githubAction;
        private ScriptGithubRPLP _scriptGithub;

        public GithubController()
        {
            _githubAction = new GithubApiAction("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");
            _scriptGithub = new ScriptGithubRPLP(new DepotClassroom());
        }

        [HttpGet("/test")]
        public ActionResult<IEnumerable<Classroom_SQLDTO>> GetClassrooms(string organisationName)
        {
            return Ok(this._scriptGithub.ScriptAssignStudentToAssignmentReview());
        }

        [HttpGet("{organisationName}")]
        public ActionResult<IEnumerable<Repository_JSONDTO>> GetRepositories(string organisationName)
        {
            return Ok(this._githubAction.GetOrganisationRepositoriesGithub(organisationName));
        }

        [HttpGet("{organisationName}/{repositoryName}")]
        public ActionResult<IEnumerable<Repository_JSONDTO>> GetRepository(string organisationName, string repositoryName)
        {
            return Ok(this._githubAction.GetRepositoryInfoGithub(organisationName, repositoryName));
        }


        [HttpGet("{organisationName}/{repositoryName}/Commit/")]
        public ActionResult<IEnumerable<Repository_JSONDTO>> GetRepositoryCommits(string organisationName, string repositoryName)
        {
            return Ok(this._githubAction.GetRepositoryCommitsGithub(organisationName, repositoryName));
        }

        [HttpGet("{organisationName}/{repositoryName}/BRanches/")]
        public ActionResult<IEnumerable<Repository_JSONDTO>> GetRepositoryBranches(string organisationName, string repositoryName)
        {
            return Ok(this._githubAction.GetRepositoryBranchesGithub(organisationName, repositoryName));
        }

        [HttpPost("{organisationName}/{repositoryName}/Create/Branch/{branchName}/From/{sha}")]
        public ActionResult<string> CreateNewBranch(string organisationName, string repositoryName, string branchName, string sha)
        {
            return Ok(this._githubAction.CreateNewBranchForFeedbackGitHub(organisationName, repositoryName, sha, branchName));
        }

        [HttpPost("{organisationName}/{repositoryName}/Create/PullRequest/{branchName}/Title/{title}/Body/{body}")]
        public ActionResult<string> CreateNewPR(string organisationName, string repositoryName, string branchName, string title, string body)
        {
            return Ok(this._githubAction.CreateNewPullRequestFeedbackGitHub(organisationName, repositoryName, branchName, title, body));
        }

        [HttpPost("{organisationName}/{repositoryName}/Assign/PullRequest/{pullRequest}/Student/{studentUsername}")]
        public ActionResult<string> AssignStidentToPR(string organisationName, string repositoryName, string pullRequest, string studentUsername)
        {
            return Ok(this._githubAction.AssignReviewerToPullRequestGitHub(organisationName, repositoryName, pullRequest, studentUsername));
        }

    }
}
