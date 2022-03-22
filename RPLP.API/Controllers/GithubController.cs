﻿using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("{organisationName}/{repositoryName}/Commit/Ref")]
        public ActionResult<IEnumerable<Repository_JSONDTO>> GetRepositoryCommitRef(string organisationName, string repositoryName)
        {
            return Ok(this._githubAction.GetRepositoryCommitRefGithub(organisationName, repositoryName));
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

        [HttpPost("{organisationName}/{repositoryName}/Assign/Branch/{branchName}/Student/{studentUsername}")]
        public ActionResult<string> AssignStidentToPR(string organisationName, string repositoryName, string branchName, string studentUsername)
        {
            return Ok(this._githubAction.AssignReviewerToPullRequestGitHub(organisationName, repositoryName, branchName, studentUsername));
        }

    }
}
