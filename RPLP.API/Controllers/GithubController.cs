using Microsoft.AspNetCore.Mvc;
using RPLP.DAL.DTO.Json;
using RPLP.DAL.DTO.Sql;
using RPLP.DAL.SQL.Depots;
using RPLP.SERVICES.Github;
using System.IO.Compression;
using System.Text;

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
            string token = "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";
            _githubAction = new GithubApiAction(token);
            _scriptGithub = new ScriptGithubRPLP(new DepotClassroom(), new DepotRepository(), token);
        }

        [HttpGet("/test/{organisationName}/{classroomName}/{assignmentName}/{numberOfReviews}")]
        public ActionResult StartScript(string organisationName, string classroomName, string assignmentName, int numberOfReviews)
        {
            try
            {
                this._scriptGithub.ScriptAssignStudentToAssignmentReview(organisationName, classroomName, assignmentName, numberOfReviews);
                return Ok("Assigned successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("/telechargement/{organisationName}/{classroomName}/{assignmentName}")]
        public FileStreamResult StartScriptDownloadAllRepositoriesForAssignment (string organisationName, string classroomName, string assignmentName)
        {
            string path = _scriptGithub.ScriptDownloadAllRepositoriesForAssignment(organisationName, classroomName, assignmentName);
            
            FileStream file = System.IO.File.OpenRead(path);
            FileStreamResult fileStreamResult = new FileStreamResult(file, "application/octet-stream");
            fileStreamResult.FileDownloadName = $"{assignmentName}_{DateTime.Now}.zip";
            
            return fileStreamResult;
        }

        [HttpGet("/telechargement/{organisationName}/{classroomName}/{assignmentName}/{studentUsername}")]
        public FileStreamResult StartScriptDownloadOneRepositoriesForAssignment(string organisationName, string classroomName, string assignmentName, string studentUsername)
        {
            string repositoryName = $"{assignmentName}-{studentUsername}";
            string path = _scriptGithub.ScriptDownloadOneRepositoryForAssignment(organisationName, classroomName, assignmentName, repositoryName);
            
            FileStream file = System.IO.File.OpenRead(path);
            FileStreamResult fileStreamResult = new FileStreamResult(file, "application/octet-stream");
            fileStreamResult.FileDownloadName = $"{assignmentName}_{DateTime.Now}.zip";
            
            return fileStreamResult;
        }


        //[HttpGet("/teachers/{organisationName}/{classroomName}/{assignmentName}/{numberOfReviews}")]
        //public ActionResult StartScriptAssignTeachers(string organisationName, string classroomName, string assignmentName, int numberOfReviews)
        //{
        //    try
        //    {
        //        this._scriptGithub.ScriptAssignTeacherToAssignmentReview(organisationName, classroomName, assignmentName, numberOfReviews);
        //        return Ok("Assigned successfully");
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }

        //}

        [HttpGet("/test/{organisationName}/{classroomName}/{assignmentName}")]
        public ActionResult StartScriptProf(string organisationName, string classroomName, string assignmentName)
        {
            try
            {
                this._scriptGithub.ScriptAssignTeacherToAssignmentReview(organisationName, classroomName, assignmentName);
                return Ok("Assigned successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

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

        [HttpGet("{organisationName}/{repositoryName}/Branches/")]
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
        public ActionResult<string> AssignStudentToPR(string organisationName, string repositoryName, string pullRequest, string studentUsername)
        {
            return Ok(this._githubAction.AssignReviewerToPullRequestGitHub(organisationName, repositoryName, pullRequest, studentUsername));
        }


        [HttpPut("{organisationName}/{repositoryName}/Add/Collaborator/Student/{studentUsername}")]
        public ActionResult<string> AssignStudentToPR(string organisationName, string repositoryName, string studentUsername)
        {
            return Ok(this._githubAction.AddStudentAsCollaboratorToPeerRepositoryGithub(organisationName, repositoryName, studentUsername));
        }
    }
}
