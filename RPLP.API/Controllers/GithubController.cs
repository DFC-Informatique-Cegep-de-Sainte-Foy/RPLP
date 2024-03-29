﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RPLP.DAL.DTO.Json;
using RPLP.DAL.DTO.Sql;
using RPLP.DAL.SQL.Depots;
using RPLP.JOURNALISATION;
using RPLP.SERVICES.Github;
using RPLP.SERVICES.Github.GithubReviewCommentFetcher;
using RPLP.ENTITES.InterfacesDepots;
using System.Diagnostics;
using System.Text;
using RPLP.SERVICES.InterfacesDepots;

namespace RPLP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GithubController : ControllerBase
    {
        private GithubApiAction _githubAction;
        private GithubPRCommentFetcher _githubPRCommentFetcher;
        private ScriptGithubRPLP _scriptGithub;

        public GithubController(IConfiguration configuration, IDepotClassroom depotClassroom, IDepotRepository depotRepository, IDepotOrganisation depotOrganisation, IDepotAllocation depotAllocation, IDepotStudent depotStudent)
        {
            if (configuration == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                "GithubController - Constructeur - configuration passé en paramêtre est null", 0));
            }

            string token = configuration.GetValue<string>("Token");

            _githubAction = new GithubApiAction(token);
            _githubPRCommentFetcher = new GithubPRCommentFetcher(token, depotClassroom, depotRepository);
            _scriptGithub = new ScriptGithubRPLP(depotClassroom, depotRepository, depotOrganisation, depotAllocation, depotStudent, token);
        }

        [HttpGet("/test/{organisationName}/{classroomName}/{assignmentName}/{numberOfReviews}")]
        public ActionResult StartScript(string organisationName, string classroomName, string assignmentName, int numberOfReviews)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(organisationName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - StartScript - organisationName passé en paramêtre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - StartScript - classroomName passé en paramêtre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(assignmentName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - StartScript - assignmentName passé en paramêtre est vide", 0));
                }

                if (numberOfReviews < 0)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - StartScript - numberOfReviews passé en paramêtre n'est pas valide", 0));
                }

                Logging.Instance.Journal(new Log($"api/Github/test/{organisationName}/{classroomName}/{assignmentName}/{numberOfReviews}", 200, "GithubController - GET méthode StartScript"));

                this._scriptGithub.ScriptAssignStudentToAssignmentReview(organisationName, classroomName, assignmentName, numberOfReviews);
                return Ok("Assigned successfully");
            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpGet("Telechargement/{organisationName}/{classroomName}/{assignmentName}")]
        public FileStreamResult StartScriptDownloadAllRepositoriesForAssignment(string organisationName, string classroomName, string assignmentName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(organisationName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - StartScriptDownloadAllRepositoriesForAssignment - organisationName passé en paramêtre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - StartScriptDownloadAllRepositoriesForAssignment - classroomName passé en paramêtre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(assignmentName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - StartScriptDownloadAllRepositoriesForAssignment - assignmentName passé en paramêtre est vide", 0));
                }

                Logging.Instance.Journal(new Log($"api/Github/Telechargement/{organisationName}/{classroomName}/{assignmentName}", 0, "GithubController - GET méthode StartScriptDownloadAllRepositoriesForAssignment"));

                Console.Out.WriteLine($"Trying to call StartScriptDownloadAllRepositoriesForAssignment({organisationName}, {classroomName}, {assignmentName})");
                string path = _scriptGithub.ScriptDownloadAllRepositoriesForAssignment(organisationName, classroomName, assignmentName);

                if (string.IsNullOrWhiteSpace(path))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                  "GithubController - StartScriptDownloadAllRepositoriesForAssignment - variable 'path' assigné est vide", 0));
                }

                FileStream file = System.IO.File.OpenRead(path);

                if (file == null)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                  "GithubController - StartScriptDownloadAllRepositoriesForAssignment - variable 'file' assigné est null", 0));
                }

                FileStreamResult fileStreamResult = new FileStreamResult(file, "application/octet-stream");

                if (fileStreamResult == null)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                  "GithubController - StartScriptDownloadAllRepositoriesForAssignment - variable 'fileStreamResult' assigné est null", 0));
                }

                fileStreamResult.FileDownloadName = $"{assignmentName}_{DateTime.Now}.zip";

                return fileStreamResult;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Telechargement/{organisationName}/{classroomName}/{assignmentName}/{studentUsername}")]
        public FileStreamResult StartScriptDownloadOneRepositoriesForAssignment(string organisationName, string classroomName, string assignmentName, string studentUsername)
        {
            try
            {
                if(string.IsNullOrWhiteSpace(organisationName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - StartScriptDownloadOneRepositoriesForAssignment - organisationName passé en paramêtre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - StartScriptDownloadOneRepositoriesForAssignment - classroomName passé en paramêtre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(assignmentName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - StartScriptDownloadOneRepositoriesForAssignment - assignmentName passé en paramêtre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(studentUsername))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - StartScriptDownloadOneRepositoriesForAssignment - studentUsername passé en paramêtre est vide"));
                }

                Logging.Instance.Journal(new Log($"api/Github/Telechargement/{organisationName}/{classroomName}/{assignmentName}/{studentUsername}", 0, "GithubController - GET méthode StartScriptDownloadOneRepositoriesForAssignment"));

                string repositoryName = $"{assignmentName}-{studentUsername}";
                Console.Out.WriteLine($"Trying to call ScriptDownloadOneRepositoryForAssignment({organisationName}, {classroomName}, {assignmentName}, {repositoryName})");
                string path = _scriptGithub.ScriptDownloadOneRepositoryForAssignment(organisationName, classroomName, assignmentName, repositoryName);

                if (string.IsNullOrWhiteSpace(path))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                  "GithubController - StartScriptDownloadOneRepositoriesForAssignment - variable 'path' assigné est vide", 0));
                }

                FileStream file = System.IO.File.OpenRead(path);

                if (file == null)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                  "GithubController - StartScriptDownloadOneRepositoriesForAssignment - variable 'file' assigné est null", 0));
                }

                FileStreamResult fileStreamResult = new FileStreamResult(file, "application/octet-stream");

                if (fileStreamResult == null)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                  "GithubController - StartScriptDownloadOneRepositoriesForAssignment - variable 'fileStreamResult' assigné est null", 0));
                }

                fileStreamResult.FileDownloadName = $"{assignmentName}_{DateTime.Now}.zip";

                return fileStreamResult;
            }
            catch (Exception)
            {
                throw;
            }
          
        }

        [HttpGet("/test/{organisationName}/{classroomName}/{assignmentName}")]
        public ActionResult StartScriptProf(string organisationName, string classroomName, string assignmentName, string teacherUsername)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(organisationName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - StartScriptProf - organisationName passé en paramêtre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - StartScriptProf - classroomName passé en paramêtre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(assignmentName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - StartScriptProf - assignmentName passé en paramêtre est vide"));
                }

                if (string.IsNullOrWhiteSpace(teacherUsername))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - StartScriptProf - teacherUsername passé en paramêtre est vide", 0));
                }

                Logging.Instance.Journal(new Log($"api/Github/test/{organisationName}/{classroomName}/{assignmentName}", 200, "GithubController - GET méthode StartScriptProf"));

                this._scriptGithub.ScriptAssignTeacherToAssignmentReview(organisationName, classroomName, assignmentName, teacherUsername);

                return Ok("Assigned successfully");
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        [HttpGet("{organisationName}")]
        public ActionResult<IEnumerable<Repository_JSONDTO>> GetRepositories(string organisationName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(organisationName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - GetRepositories - organisationName passé en paramêtre est vide", 0));
                }

                Logging.Instance.Journal(new Log($"api/Github/{organisationName}", 200, "GithubController - GET méthode GetRepositories"));

                return Ok(this._githubAction.GetOrganisationRepositoriesGithub(organisationName));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("{organisationName}/{repositoryName}")]
        public ActionResult<IEnumerable<Repository_JSONDTO>> GetRepository(string organisationName, string repositoryName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(organisationName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - GetRepository - organisationName passé en paramêtre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(repositoryName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - GetRepository - repositoryName passé en paramêtre est vide", 0));
                }

                Logging.Instance.Journal(new Log($"api/Github/{organisationName}/{repositoryName}", 200, "GithubController - GET méthode GetRepository"));

                return Ok(this._githubAction.GetRepositoryInfoGithub(organisationName, repositoryName));
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpGet("{organisationName}/{repositoryName}/Commit/")]
        public ActionResult<IEnumerable<Repository_JSONDTO>> GetRepositoryCommits(string organisationName, string repositoryName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(organisationName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - GetRepositoryCommits - organisationName passé en paramêtre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(repositoryName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - GetRepositoryCommits - repositoryName passé en paramêtre est vide", 0));
                }

                Logging.Instance.Journal(new Log($"api/Github/{organisationName}/{repositoryName}/Commit/", 200, "GithubController - GET méthode GetRepositoryCommits"));

                return Ok(this._githubAction.GetRepositoryCommitsGithub(organisationName, repositoryName));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("{organisationName}/{repositoryName}/Branches/")]
        public ActionResult<IEnumerable<Repository_JSONDTO>> GetRepositoryBranches(string organisationName, string repositoryName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(organisationName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - GetRepositoryBranches - organisationName passé en paramêtre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(repositoryName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - GetRepositoryBranches - repositoryName passé en paramêtre est vide", 0));
                }

                Logging.Instance.Journal(new Log($"api/Github/{organisationName}/{repositoryName}/Branches/", 200, "GithubController - GET méthode GetRepositoryBranches"));

                return Ok(this._githubAction.GetRepositoryBranchesGithub(organisationName, repositoryName));
            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpPost("{organisationName}/{repositoryName}/Create/Branch/{branchName}/From/{sha}")]
        public ActionResult<string> CreateNewBranch(string organisationName, string repositoryName, string branchName, string sha)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(organisationName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - CreateNewBranch - organisationName passé en paramêtre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(repositoryName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - CreateNewBranch - repositoryName passé en paramêtre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(branchName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - CreateNewBranch - branchName passé en paramêtre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(sha))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - CreateNewBranch - sha passé en paramêtre est vide"));
                }

                Logging.Instance.Journal(new Log($"api/Github/{organisationName}/{repositoryName}/Create/Branch/{branchName}/From/{sha}", 200, "GithubController - POST méthode CreateNewBranch"));

                return Ok(this._githubAction.CreateNewBranchGitHub(organisationName, repositoryName, sha, branchName));
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost("{organisationName}/{repositoryName}/Create/PullRequest/{branchName}/Title/{title}/Body/{body}")]
        public ActionResult<string> CreateNewPR(string organisationName, string repositoryName, string branchName, string title, string body)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(organisationName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - CreateNewPR - organisationName passé en paramêtre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(repositoryName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - CreateNewPR - repositoryName passé en paramêtre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(branchName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - CreateNewPR - branchName passé en paramêtre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(title))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - CreateNewPR - title passé en paramêtre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(body))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - CreateNewPR - body passé en paramêtre est vide", 0));
                }

                Logging.Instance.Journal(new Log($"api/Github/{organisationName}/{repositoryName}/Create/PullRequest/{branchName}/Title/{{title}}/Body/{body}", 200, "GithubController - POST méthode CreateNewPR"));

                return Ok(this._githubAction.CreateNewPullRequestGitHub(organisationName, repositoryName, branchName, title, body));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("{organisationName}/{repositoryName}/Assign/PullRequest/{pullRequest}/Student/{studentUsername}")]
        public ActionResult<string> AssignStudentToPR(string organisationName, string repositoryName, string pullRequest, string studentUsername)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(organisationName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - AssignStudentToPR (POST :{organisationName}/{repositoryName}/Assign/PullRequest/{pullRequest}/Student/{studentUsername}) - organisationName passé en paramêtre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(repositoryName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - AssignStudentToPR (POST :{organisationName}/{repositoryName}/Assign/PullRequest/{pullRequest}/Student/{studentUsername}) - repositoryName passé en paramêtre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(pullRequest))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - AssignStudentToPR (POST :{organisationName}/{repositoryName}/Assign/PullRequest/{pullRequest}/Student/{studentUsername}) - pullRequest passé en paramêtre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(studentUsername))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - AssignStudentToPR (POST :{organisationName}/{repositoryName}/Assign/PullRequest/{pullRequest}/Student/{studentUsername}) - studentUsername passé en paramêtre est vide", 0));
                }

                Logging.Instance.Journal(new Log($"api/Github/{organisationName}/{repositoryName}/Assign/PullRequest/{pullRequest}/Student/{studentUsername}", 200, "GithubController - POST méthode AssignStudentToPR"));

                return Ok(this._githubAction.AssignReviewerToPullRequestGitHub(organisationName, repositoryName, pullRequest, studentUsername));
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPut("{organisationName}/{repositoryName}/Add/Collaborator/Student/{studentUsername}")]
        public ActionResult<string> AssignStudentToPR(string organisationName, string repositoryName, string studentUsername)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(organisationName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - AssignStudentToPR (PUT : {organisationName}/{repositoryName}/Add/Collaborator/Student/{studentUsername}) - organisationName passé en paramêtre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(repositoryName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - AssignStudentToPR (PUT : {organisationName}/{repositoryName}/Add/Collaborator/Student/{studentUsername}) - repositoryName passé en paramêtre est vide" , 0));
                }

                if (string.IsNullOrWhiteSpace(studentUsername))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - AssignStudentToPR (PUT : {organisationName}/{repositoryName}/Add/Collaborator/Student/{studentUsername}) - studentUsername passé en paramêtre est vide", 0));
                }

                Logging.Instance.Journal(new Log($"api/Github/{organisationName}/{repositoryName}/Add/Collaborator/Student/{studentUsername}", 200, "GithubController - PUT méthode AssignStudentToPR"));

                return Ok(this._githubAction.AddStudentAsCollaboratorToPeerRepositoryGithub(organisationName, repositoryName, studentUsername));
            }
            catch (Exception)
            {
                throw;
            }

        }

        #region CommentFetcher

        [HttpGet("{teacherUsername}/{repositoryName}/PullRequests")]
        public ActionResult<List<Pull>> GetPullRequestsFromRepository(string teacherUsername, string repositoryName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(repositoryName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - GetPullRequestsFromRepository - repositoryName passé en paramêtre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(teacherUsername))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - GetPullRequestsFromRepository - teacherUsername passé en paramêtre est vide", 0));
                }

                Logging.Instance.Journal(new Log($"api/Github/{teacherUsername}/{repositoryName}/PullRequests", 200, "GithubController - GET méthode GetPullRequestsFromRepository"));

                return Ok(this._githubPRCommentFetcher.GetPullRequestsFromRepositoryAsync(teacherUsername, repositoryName).Result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("{teacherUsername}/{repositoryName}/PullRequests/Comments/Students")]
        public ActionResult<CommentAggregate> GetIssuesReviewsAndCommentsByStudentOnAssignment(
            string teacherUsername, string repositoryName, [FromBody] List<string> studentNames)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(repositoryName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - GetIssuesReviewsAndCommentsByStudentOnAssignment (POST : {teacherUsername}/{repositoryName}/PullRequests/Comments/Students) - repositoryName passé en paramêtre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(teacherUsername))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - GetIssuesReviewsAndCommentsByStudentOnAssignment (POST : {teacherUsername}/{repositoryName}/PullRequests/Comments/Students) - teacherUsername passé en paramêtre est vide", 0));
                }

                if(studentNames == null)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - GetIssuesReviewsAndCommentsByStudentOnAssignment (POST : {teacherUsername}/{repositoryName}/PullRequests/Comments/Students) - studentNames passé en paramêtre est null", 0));
                }

                if(!ModelState.IsValid)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - GetIssuesReviewsAndCommentsByStudentOnAssignment (POST : {teacherUsername}/{repositoryName}/PullRequests/Comments/Students) - studentNames passé en paramêtre n'est pas un model valide", 0));
                }

                List<Pull> pulls = this._githubPRCommentFetcher.GetPullRequestsFromRepositoryAsync(teacherUsername, repositoryName).Result;

                if(pulls == null)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - GetIssuesReviewsAndCommentsByStudentOnAssignment (POST : {teacherUsername}/{repositoryName}/PullRequests/Comments/Students) - variable pulls assigné de la méthode GetPullRequestsFromRepositoryAsyncest null", 0));
                }

                Logging.Instance.Journal(new Log($"api/Github/{teacherUsername}/{repositoryName}/PullRequests/Comments/Students", 200, "GithubController - POST méthode GetIssuesReviewsAndCommentsByStudentOnAssignment"));

                return Ok(this._githubPRCommentFetcher.GetMultipleUsersCommentsReviewsAndIssues(pulls, studentNames).Result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("{teacherUsername}/{repositoryName}/PullRequests/Comments/{studentName}")]
        public ActionResult<CommentAggregate> GetIssuesReviewsAndCommentsByStudentOnAssignment(
            string teacherUsername, string repositoryName, string studentName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(repositoryName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - GetIssuesReviewsAndCommentsByStudentOnAssignment (GET : {teacherUsername}/{repositoryName}/PullRequests/Comments/{studentName}) - repositoryName passé en paramêtre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(teacherUsername))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - GetIssuesReviewsAndCommentsByStudentOnAssignment (GET : {teacherUsername}/{repositoryName}/PullRequests/Comments/{studentName}) - teacherUsername passé en paramêtre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(studentName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - GetIssuesReviewsAndCommentsByStudentOnAssignment (GET - {teacherUsername}/{repositoryName}/PullRequests/Comments/{studentName}) - studentName passé en paramêtre est null", 0));
                }

                List<Pull> pulls = this._githubPRCommentFetcher.GetPullRequestsFromRepositoryAsync(teacherUsername, repositoryName).Result;

                if (pulls == null)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - GetIssuesReviewsAndCommentsByStudentOnAssignment (GET - {teacherUsername}/{repositoryName}/PullRequests/Comments/{studentName}) - variable pulls assigné de la méthode GetPullRequestsFromRepositoryAsync est null", 0));
                }

                Logging.Instance.Journal(new Log($"api/Github/{teacherUsername}/{repositoryName}/PullRequests/Comments/{studentName}", 200, "GithubController - GET méthode GetIssuesReviewsAndCommentsByStudentOnAssignment"));

                return Ok(this._githubPRCommentFetcher.GetUserCommentsReviewsAndIssues(pulls, studentName).Result);
            }
            catch (Exception)
            {
                throw;
            }
           
        }

        [HttpGet("{organisationName}/{repositoryName}/PullRequests/Comments/File")]
        public FileStreamResult GetFileWithCommentsOfPullRequestByAssignmentForSingleRepository(string organisationName, string repositoryName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(organisationName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - GetFileWithCommentsOfPullRequestByAssignmentForSingleRepository - organisationName passé en paramêtre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(repositoryName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - GetFileWithCommentsOfPullRequestByAssignmentForSingleRepository - repositoryName passé en paramêtre est vide", 0));
                }

                Logging.Instance.Journal(new Log($"api/Github/{organisationName}/{repositoryName}/PullRequests/Comments/File", 0, "GithubController - GET méthode GetFileWithCommentsOfPullRequestByAssignmentForSingleRepository"));

                List<Pull>? pull = this._githubPRCommentFetcher.GetPullRequestsFromRepositoryAsync(organisationName, repositoryName).Result;

                if (pull == null)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - GetFileWithCommentsOfPullRequestByAssignmentForSingleRepository - variable pulls assigné de la méthode GetPullRequestsFromRepositoryAsync est null", 0));
                }

                var stream = new MemoryStream(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(pull)));

                if(stream.Length <= 0)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                   "GithubController - GetFileWithCommentsOfPullRequestByAssignmentForSingleRepository - variable stream est vide", 0));
                }

                FileStreamResult fileStreamResult = new FileStreamResult(stream, "application/octet-stream");

                if(fileStreamResult == null)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - GetFileWithCommentsOfPullRequestByAssignmentForSingleRepository - variable fileStreamResult est null"));
                }

                fileStreamResult.FileDownloadName = $"Comments_{repositoryName}_{DateTime.Now}.json";

                return fileStreamResult;
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpGet("{organisationName}/{classroomName}/{assignmentName}/PullRequests/Comments/File")]
        public FileStreamResult GetFileWithCommentsOfPullRequestByAssignmentForAllRepositories(string organisationName, string classroomName, string assignmentName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(organisationName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - GetFileWithCommentsOfPullRequestByAssignmentForAllRepositories - organisationName passé en paramêtre est vide", 0));
                }

                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - GetFileWithCommentsOfPullRequestByAssignmentForAllRepositories - classroomName passé en paramêtre est vide", 0));
                }
                if (string.IsNullOrWhiteSpace(assignmentName))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - GetFileWithCommentsOfPullRequestByAssignmentForAllRepositories - assignmentName passé en paramêtre est vide", 0));
                }

                Logging.Instance.Journal(new Log($"api/Github/{organisationName}/{classroomName}/{assignmentName}/PullRequests/Comments/File", 0, "GithubController - GET méthode GetFileWithCommentsOfPullRequestByAssignmentForAllRepositories"));

                List<ReviewerUser>? reviewerUsers = this._githubPRCommentFetcher.GetCommentsReviewsAndIssuesByReviewersAsync(organisationName, classroomName, assignmentName).Result;
                
                if(reviewerUsers == null)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - GetFileWithCommentsOfPullRequestByAssignmentForAllRepositories - variable reviewerUsers assigné de la méthode GetCommentsReviewsAndIssuesByReviewersAsync est null", 0));
                }

                var stream = new MemoryStream(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(reviewerUsers)));

                if (stream.Length <= 0)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                   "GithubController - GetFileWithCommentsOfPullRequestByAssignmentForAllRepositories - variable stream est vide", 0));
                }

                FileStreamResult fileStreamResult = new FileStreamResult(stream, "application/octet-stream");

                if (fileStreamResult == null)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubController - GetFileWithCommentsOfPullRequestByAssignmentForAllRepositories - variable fileStreamResult est null", 0));
                }

                fileStreamResult.FileDownloadName = $"Comments_{assignmentName}_{DateTime.Now}.json";

                return fileStreamResult;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
#endregion