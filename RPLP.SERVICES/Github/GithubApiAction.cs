using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RPLP.DAL.DTO.Json;
using RPLP.JOURNALISATION;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.SERVICES.Github
{
    public class GithubApiAction
    {
        private static HttpClient _httpClient;
        private string _getOrganisationRepositoriesGithub = "/orgs/{organisationName}/repos";
        private string _getRepositoryInfoGithub = "/repos/{organisationName}/{repositoryName}";
        private string _getRepositoryCommitsGithub = "/repos/{organisationName}/{repositoryName}/commits";
        private string _getBranchesFromRepositoryCommitGithub = "/repos/{organisationName}/{repositoryName}/git/refs/heads";
        private string _getPullRequestsFromRepositoryGithub = "/repos/{organisationName}/{repositoryName}/pulls?";
        private string _getCollaboratorfromRepositoryGithub = "/repos/{organisationName}/{repositoryName}/collaborators";
        private string _createNewBranchInRepositoryGithub = "/repos/{organisationName}/{repositoryName}/git/refs";
        private string _createPullRequestOnRepositoryBranchGithub = "/repos/{organisationName}/{repositoryName}/pulls";
        private string _addorRemoveCollaboratorToRepositoryGithub = "/repos/{organisationName}/{repositoryName}/collaborators/{studentUsername}";
        private string _addFileToContentsGithub = "/repos/{organisationName}/{repositoryName}/contents/{newFileName}";
        private string _assignStudentToPullRequestGithub = "/repos/{organisationName}/{repositoryName}/pulls/{branchId}/requested_reviewers?";
        private string _downloadRepository = "/repos/{organisationName}/{repositoryName}/zipball";


        private const string organisationName = "{organisationName}";
        private const string repositoryName = "{repositoryName}";
        private const string branchId = "{branchId}";
        private const string studentUsername = "{studentUsername}";
        private const string newFileName = "{newFileName}";

        public GithubApiAction(string p_token)
        {
            if(string.IsNullOrWhiteSpace(p_token))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubApiAction - Constructeur - p_token passé en paramêtre est vide"));
            }

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://api.github.com");
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("RPLP", "1.0"));
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Token", p_token);
        }

        public List<Repository_JSONDTO> GetOrganisationRepositoriesGithub(string p_organisationName)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubApiAction - GetOrganisationRepositoriesGithub - p_organisationName passé en paramêtre est vide"));
            }

            string fullPath = _getOrganisationRepositoriesGithub.Replace(organisationName, p_organisationName);

            if (string.IsNullOrWhiteSpace(fullPath))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubApiAction - GetOrganisationRepositoriesGithub - la variable fullPath assigné à partir de la méthode string.Replace est vide"));
            }

            Task<List<Repository_JSONDTO>> RepositoriesJSON = OrganisationRepositoryGithubApiRequest(fullPath);
            RepositoriesJSON.Wait();

            return RepositoriesJSON.Result;
        }

        private static async Task<List<Repository_JSONDTO>> OrganisationRepositoryGithubApiRequest(string p_githubLink)
        {
            if (string.IsNullOrWhiteSpace(p_githubLink))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "GithubApiAction - OrganisationRepositoryGithubApiRequest - p_githubLink passé en paramêtre est vide"));
            }

            List<Repository_JSONDTO> repositories = new List<Repository_JSONDTO>();
            HttpResponseMessage response = await _httpClient.GetAsync(p_githubLink);

            if (response.IsSuccessStatusCode)
            {
                string JSONContent = await response.Content.ReadAsStringAsync();
                repositories = JsonConvert.DeserializeObject<List<Repository_JSONDTO>>(JSONContent);
            }
            else
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - OrganisationRepositoryGithubApiRequest - response.IsSuccessStatusCode == false suite à l'appel de la méthode  _httpClient.GetAsync(p_githubLink) "));
            }

            return repositories;
        }

        public Repository_JSONDTO GetRepositoryInfoGithub(string p_organisationName, string p_repositoryName)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "GithubApiAction - GetRepositoryInfoGithub - p_organisationName passé en paramêtre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_repositoryName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "GithubApiAction - GetRepositoryInfoGithub - p_repositoryName passé en paramêtre est vide"));
            }

            string fullPath = _getRepositoryInfoGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName);

            if (string.IsNullOrWhiteSpace(fullPath))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "GithubApiAction - GetRepositoryInfoGithub - la variable fullPath assigné par la méthode _getRepositoryInfoGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName) est vide"));
            }

            Task<Repository_JSONDTO> RepositoryJSON = RepositoryInfoGithubApiRequest(fullPath);
            RepositoryJSON.Wait();

            return RepositoryJSON.Result;
        }

        private static async Task<Repository_JSONDTO> RepositoryInfoGithubApiRequest(string p_githubLink)
        {
            if(string.IsNullOrWhiteSpace(p_githubLink))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - RepositoryInfoGithubApiRequest - p_githubLink passé en paramêtre est vide"));
            }

            Repository_JSONDTO repository = new Repository_JSONDTO();
            HttpResponseMessage response = await _httpClient.GetAsync(p_githubLink);

            if (response.IsSuccessStatusCode)
            {
                string JSONContent = await response.Content.ReadAsStringAsync();
                repository = JsonConvert.DeserializeObject<Repository_JSONDTO>(JSONContent);
            }
            else
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - RepositoryInfoGithubApiRequest - response.IsSuccessStatusCode == false suite à l'appel de la méthode _httpClient.GetAsync(p_githubLink)"));
            }

            return repository;
        }

        public List<Branch_JSONDTO> GetRepositoryBranchesGithub(string p_organisationName, string p_repositoryName)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - GetRepositoryBranchesGithub - p_organisationName passé en paramêtre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_repositoryName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - GetRepositoryBranchesGithub - p_repositoryName passé en paramêtre est vide"));
            }

            string fullPath = _getBranchesFromRepositoryCommitGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName);

            if (string.IsNullOrWhiteSpace(fullPath))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "GithubApiAction - GetRepositoryBranchesGithub - la variable fullPath assigné par la méthode _getBranchesFromRepositoryCommitGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName) est vide"));
            }

            Task<List<Branch_JSONDTO>> refsJSON = RepositoryBranchesGithubApiRequest(fullPath);
            refsJSON.Wait();

            return refsJSON.Result;
        }

        private static async Task<List<Branch_JSONDTO>> RepositoryBranchesGithubApiRequest(string p_githubLink)
        {
            if (string.IsNullOrWhiteSpace(p_githubLink))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - RepositoryBranchesGithubApiRequest - p_githubLink passé en paramêtre est vide"));
            }

            List<Branch_JSONDTO> refs = new List<Branch_JSONDTO>();
            HttpResponseMessage response = await _httpClient.GetAsync(p_githubLink);

            if (response.IsSuccessStatusCode)
            {
                string JSONContent = await response.Content.ReadAsStringAsync();
                refs = JsonConvert.DeserializeObject<List<Branch_JSONDTO>>(JSONContent);
            }
            else
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - RepositoryBranchesGithubApiRequest - response.IsSuccessStatusCode == false suite à l'appel de la méthode _httpClient.GetAsync(p_githubLink)"));
            }

            return refs;
        }

        public List<Commit_JSONDTO> GetRepositoryCommitsGithub(string p_organisationName, string p_repositoryName)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - GetRepositoryCommitsGithub - p_organisationName passé en paramêtre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_repositoryName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - GetRepositoryCommitsGithub - p_repositoryName passé en paramêtre est vide"));
            }

            string fullPath = _getRepositoryCommitsGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName);

            if (string.IsNullOrWhiteSpace(fullPath))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "GithubApiAction - GetRepositoryCommitsGithub - la variable fullPath assigné par la méthode _getRepositoryCommitsGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName) est vide"));
            }

            Task<List<Commit_JSONDTO>> commitsJSON = RepositoryCommitsGithubApiRequest(fullPath);
            commitsJSON.Wait();

            return commitsJSON.Result;
        }

        private static async Task<List<Commit_JSONDTO>> RepositoryCommitsGithubApiRequest(string p_githubLink)
        {
            if (string.IsNullOrWhiteSpace(p_githubLink))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - RepositoryCommitsGithubApiRequest - p_githubLink passé en paramêtre est vide"));
            }

            List<Commit_JSONDTO> commits = new List<Commit_JSONDTO>();
            HttpResponseMessage response = await _httpClient.GetAsync(p_githubLink);

            if (response.IsSuccessStatusCode)
            {
                string JSONContent = await response.Content.ReadAsStringAsync();
                commits = JsonConvert.DeserializeObject<List<Commit_JSONDTO>>(JSONContent);
            }
            else
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                  "GithubApiAction - RepositoryCommitsGithubApiRequest - response.IsSuccessStatusCode == false suite à l'appel de la méthode _httpClient.GetAsync(p_githubLink)"));
            }

            return commits;
        }

        public List<PullRequest_JSONDTO> GetRepositoryPullRequestsGithub(string p_organisationName, string p_repositoryName)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - GetRepositoryPullRequestsGithub - p_organisationName passé en paramêtre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_repositoryName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - GetRepositoryPullRequestsGithub - p_repositoryName passé en paramêtre est vide"));
            }

            string fullPath = _getPullRequestsFromRepositoryGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName);

            if (string.IsNullOrWhiteSpace(fullPath))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "GithubApiAction - GetRepositoryPullRequestsGithub - la variable fullPath assigné par la méthode _getPullRequestsFromRepositoryGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName) est vide"));
            }

            Task<List<PullRequest_JSONDTO>> pullRequests = RepositoryPullRequestsGithubApiRequest(fullPath);
            pullRequests.Wait();

            return pullRequests.Result;
        }

        private static async Task<List<PullRequest_JSONDTO>> RepositoryPullRequestsGithubApiRequest(string p_githubLink)
        {
            if (string.IsNullOrWhiteSpace(p_githubLink))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - RepositoryPullRequestsGithubApiRequest - p_githubLink passé en paramêtre est vide"));
            }

            List<PullRequest_JSONDTO> branches = new List<PullRequest_JSONDTO>();
            HttpResponseMessage response = await _httpClient.GetAsync(p_githubLink);

            if (response.IsSuccessStatusCode)
            {
                string JSONContent = await response.Content.ReadAsStringAsync();
                branches = JsonConvert.DeserializeObject<List<PullRequest_JSONDTO>>(JSONContent);
            }
            else
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                 "GithubApiAction - RepositoryPullRequestsGithubApiRequest - response.IsSuccessStatusCode == false suite à l'appel de la méthode _httpClient.GetAsync(p_githubLink)"));
            }

            return branches;
        }

        public string CreateNewBranchForFeedbackGitHub(string p_organisationName, string p_repositoryName, string p_sha, string p_newBranchName)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - CreateNewBranchForFeedbackGitHub - p_organisationName passé en paramêtre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_repositoryName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - CreateNewBranchForFeedbackGitHub - p_repositoryName passé en paramêtre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_sha))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - CreateNewBranchForFeedbackGitHub - p_sha passé en paramêtre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_newBranchName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - CreateNewBranchForFeedbackGitHub - p_newBranchName passé en paramêtre est vide"));
            }

            string fullPath = _createNewBranchInRepositoryGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName);

            if (string.IsNullOrWhiteSpace(fullPath))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "GithubApiAction - CreateNewBranchForFeedbackGitHub - la variable fullPath assigné par la méthode _createNewBranchInRepositoryGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName) est vide"));
            }

            Task<string> statusCode = NewBranchForFeedbackGithubApiRequest(fullPath, p_sha, p_newBranchName);
            statusCode.Wait();

            return statusCode.Result;
        }

        private async Task<string> NewBranchForFeedbackGithubApiRequest(string p_githubLink, string p_sha, string p_newBranchName)
        {
            if (string.IsNullOrWhiteSpace(p_githubLink))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - NewBranchForFeedbackGithubApiRequest - p_githubLink passé en paramêtre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_sha))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - NewBranchForFeedbackGithubApiRequest - p_sha passé en paramêtre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_newBranchName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - NewBranchForFeedbackGithubApiRequest - p_newBranchName passé en paramêtre est vide"));
            }

            Branch_JSONRequest branch_request = new Branch_JSONRequest
            {
                reference = $"refs/heads/{p_newBranchName}",
                sha = p_sha,
            };

            string branchRequest = JsonConvert.SerializeObject(branch_request);

            if (string.IsNullOrWhiteSpace(branchRequest))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - NewBranchForFeedbackGithubApiRequest - la variable branchRequest assigné à partir de la méthode JsonConvert.SerializeObject(branch_request) est vide"));
            }

            HttpResponseMessage response = _httpClient.PostAsync(p_githubLink, new StringContent(branchRequest, Encoding.UTF8, "application/json")).Result;

            return response.StatusCode.ToString();
        }

        public string CreateNewPullRequestFeedbackGitHub(string p_organisationName, string p_repositoryName, string p_BranchName, string p_title, string p_body)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - CreateNewPullRequestFeedbackGitHub - p_organisationName passé en paramêtre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_repositoryName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - CreateNewPullRequestFeedbackGitHub - p_repositoryName passé en paramêtre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_BranchName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - CreateNewPullRequestFeedbackGitHub - p_BranchName passé en paramêtre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_title))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - CreateNewPullRequestFeedbackGitHub - p_title passé en paramêtre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_body))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - CreateNewPullRequestFeedbackGitHub - p_body passé en paramêtre est vide"));
            }

            string fullPath = _createPullRequestOnRepositoryBranchGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName);

            if (string.IsNullOrWhiteSpace(fullPath))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "GithubApiAction - CreateNewPullRequestFeedbackGitHub - la variable fullPath assigné par la méthode _createPullRequestOnRepositoryBranchGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName) est vide"));
            }

            Task<string> statusCode = NewPullRequestFeedbackGitHubApiRequest(fullPath, p_BranchName, p_title, p_body);
            statusCode.Wait();

            return statusCode.Result;
        }

        private async Task<string> NewPullRequestFeedbackGitHubApiRequest(string p_githubLink, string p_BranchName, string p_title, string p_body)
        {
            if (string.IsNullOrWhiteSpace(p_githubLink))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - NewPullRequestFeedbackGitHubApiRequest - p_githubLink passé en paramêtre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_BranchName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - NewPullRequestFeedbackGitHubApiRequest - p_BranchName passé en paramêtre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_title))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - NewPullRequestFeedbackGitHubApiRequest - p_title passé en paramêtre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_body))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - NewPullRequestFeedbackGitHubApiRequest - p_body passé en paramêtre est vide"));
            }

            PullRequest_JSONRequest pullRequest_request = new PullRequest_JSONRequest { fromBranch = "main", targetBranch = p_BranchName, title = p_title, body = p_body };
            string pullRequest = JsonConvert.SerializeObject(pullRequest_request);

            if (string.IsNullOrWhiteSpace(pullRequest))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - NewPullRequestFeedbackGitHubApiRequest - la chaine de caractère pullRequest assignée à partir de la méthode JsonConvert.SerializeObject(pullRequest_request) est vide"));
            }

            HttpResponseMessage response = _httpClient.PostAsync(p_githubLink, new StringContent(pullRequest, Encoding.UTF8, "application/json")).Result;

            return response.StatusCode.ToString();
        }

        public string AddStudentAsCollaboratorToPeerRepositoryGithub(string p_organisationName, string p_repositoryName, string p_studentUsername)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - AddStudentAsCollaboratorToPeerRepositoryGithub - p_organisationName passé en paramêtre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_repositoryName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - AddStudentAsCollaboratorToPeerRepositoryGithub - p_repositoryName passé en paramêtre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_studentUsername))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - AddStudentAsCollaboratorToPeerRepositoryGithub - p_studentUsername passé en paramêtre est vide"));
            }

            string fullPath = _addorRemoveCollaboratorToRepositoryGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName).Replace(studentUsername, p_studentUsername);

            if (string.IsNullOrWhiteSpace(fullPath))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "GithubApiAction - AddStudentAsCollaboratorToPeerRepositoryGithub - la variable fullPath assigné par la méthode _addorRemoveCollaboratorToRepositoryGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName).Replace(studentUsername, p_studentUsername) est vide"));
            }

            Task<string> statusCode = AddStudentAsCollaboratorToPeerRepositoryGithubApiRequest(fullPath);
            statusCode.Wait();

            return statusCode.Result;
        }

        public List<Collaborator_JSONDTO> GetCollaboratorFromStudentRepositoryGithub(string p_organisationName, string p_repositoryName)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - GetCollaboratorFromStudentRepositoryGithub - p_organisationName passé en paramêtre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_repositoryName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - GetCollaboratorFromStudentRepositoryGithub - p_repositoryName passé en paramêtre est vide"));
            }

            string fullPath = _getCollaboratorfromRepositoryGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName);

            if (string.IsNullOrWhiteSpace(fullPath))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "GithubApiAction - GetCollaboratorFromStudentRepositoryGithub - la variable fullPath assigné par la méthode _getCollaboratorfromRepositoryGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName) est vide"));
            }

            Task<List<Collaborator_JSONDTO>> collaboratorsResult = GetCollaboratorFromStudentRepositoryGithubGithubApiRequest(fullPath);
            collaboratorsResult.Wait();

            return collaboratorsResult.Result;
        }

        private async Task<List<Collaborator_JSONDTO>> GetCollaboratorFromStudentRepositoryGithubGithubApiRequest(string p_githubLink)
        {
            if (string.IsNullOrWhiteSpace(p_githubLink))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - GetCollaboratorFromStudentRepositoryGithubGithubApiRequest - p_githubLink passé en paramêtre est vide"));
            }

            List<Collaborator_JSONDTO> collaborators = new List<Collaborator_JSONDTO>();
            HttpResponseMessage response = await _httpClient.GetAsync(p_githubLink);

            if (response.IsSuccessStatusCode)
            {
                string JSONContent = await response.Content.ReadAsStringAsync();
                collaborators = JsonConvert.DeserializeObject<List<Collaborator_JSONDTO>>(JSONContent);
            }
            else
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
               "GithubApiAction - GetCollaboratorFromStudentRepositoryGithubGithubApiRequest - response.IsSuccessStatusCode == false suite à l'appel de la méthode _httpClient.GetAsync(p_githubLink)"));
            }

            return collaborators;
        }

        private async Task<string> AddStudentAsCollaboratorToPeerRepositoryGithubApiRequest(string p_githubLink)
        {
            if (string.IsNullOrWhiteSpace(p_githubLink))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - AddStudentAsCollaboratorToPeerRepositoryGithubApiRequest - p_githubLink passé en paramêtre est vide"));
            }

            string permission = "{\"permission\":\"Triage\"}";
            HttpResponseMessage response = _httpClient.PutAsync(p_githubLink, new StringContent(permission, Encoding.UTF8, "application/json")).Result;

            return response.StatusCode.ToString();
        }

        public string RemoveStudentAsCollaboratorFromPeerRepositoryGithub(string p_organisationName, string p_repositoryName, string p_studentUsername)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - RemoveStudentAsCollaboratorFromPeerRepositoryGithub - p_organisationName passé en paramêtre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_repositoryName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - RemoveStudentAsCollaboratorFromPeerRepositoryGithub - p_repositoryName passé en paramêtre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_studentUsername))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - RemoveStudentAsCollaboratorFromPeerRepositoryGithub - p_studentUsername passé en paramêtre est vide"));
            }

            string fullPath = _addorRemoveCollaboratorToRepositoryGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName).Replace(studentUsername, p_studentUsername);

            if (string.IsNullOrWhiteSpace(fullPath))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "GithubApiAction - RemoveStudentAsCollaboratorFromPeerRepositoryGithub - la variable fullPath assigné par la méthode _addorRemoveCollaboratorToRepositoryGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName).Replace(studentUsername, p_studentUsername) est vide"));
            }

            Task<string> statusCode = RemoveStudentAsCollaboratorFromPeerRepositoryGithubApiRequest(fullPath);
            statusCode.Wait();

            return statusCode.Result;
        }

        private async Task<string> RemoveStudentAsCollaboratorFromPeerRepositoryGithubApiRequest(string p_githubLink)
        {
            if (string.IsNullOrWhiteSpace(p_githubLink))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - RemoveStudentAsCollaboratorFromPeerRepositoryGithubApiRequest - p_githubLink passé en paramêtre est vide"));
            }

            HttpResponseMessage response = _httpClient.DeleteAsync(p_githubLink).Result;

            return response.StatusCode.ToString();
        }

        public string AssignReviewerToPullRequestGitHub(string p_organisationName, string p_repositoryName, string p_pullRequestName, string p_studentUsername)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - AssignReviewerToPullRequestGitHub - p_organisationName passé en paramêtre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_repositoryName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - AssignReviewerToPullRequestGitHub - p_repositoryName passé en paramêtre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_pullRequestName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - AssignReviewerToPullRequestGitHub - p_pullRequestName passé en paramêtre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_studentUsername))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "GithubApiAction - AssignReviewerToPullRequestGitHub - p_studentUsername passé en paramêtre est vide"));
            }

            List<PullRequest_JSONDTO> pullRequests = GetRepositoryPullRequestsGithub(p_organisationName, p_repositoryName);

            if (pullRequests.Count >= 1)
            {
                int branchIdResult = pullRequests.Where(pullRequest => pullRequest.title.ToLower() == p_pullRequestName.ToLower()).Select(branch => branch.number).SingleOrDefault();

                string fullPath = _assignStudentToPullRequestGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName).Replace(branchId, branchIdResult.ToString());

                if (string.IsNullOrWhiteSpace(fullPath))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                        "GithubApiAction - RemoveStudentAsCollaboratorFromPeerRepositoryGithub - la variable fullPath assigné par la méthode _addorRemoveCollaboratorToRepositoryGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName).Replace(studentUsername, p_studentUsername) est vide"));
                }

                Task<string> statusCode = AssignReviewerToPullRequestGitHubApiRequest(fullPath, p_studentUsername);
                statusCode.Wait();

                return statusCode.Result;
            }
            else
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "GithubApiAction - AssignReviewerToPullRequestGitHub - la variable pullRequests assignée par la méthode GetRepositoryPullRequestsGithub(p_organisationName, p_repositoryName) est null"));
            }

            return "No Branches";
        }

        private async Task<string> AssignReviewerToPullRequestGitHubApiRequest(string p_githubLink, string p_studentUsername)
        {
            if (string.IsNullOrWhiteSpace(p_githubLink))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - AssignReviewerToPullRequestGitHubApiRequest - p_githubLink passé en paramêtre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_studentUsername))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - AssignReviewerToPullRequestGitHubApiRequest - p_studentUsername passé en paramêtre est vide"));
            }

            string[] reviewer = { p_studentUsername };
            Reviewer_JSONDTO reviewerJSON = new Reviewer_JSONDTO { reviewers = reviewer };
            string reviewers = JsonConvert.SerializeObject(reviewerJSON);

            if (string.IsNullOrWhiteSpace(reviewers))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - AssignReviewerToPullRequestGitHubApiRequest - la chaine de caractère reviewers assignée par la méthode JsonConvert.SerializeObject(reviewerJSON); est vide"));
            }

            HttpResponseMessage response = _httpClient.PostAsync(p_githubLink, new StringContent(reviewers, Encoding.UTF8, "application/json")).Result;

            return response.StatusCode.ToString();
        }


        public string AddFileToContentsGitHub(string p_organisationName, string p_repositoryName, string p_branchName, string p_newFileName, string p_content, string p_message)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - AddFileToContentsGitHub - p_organisationName passé en paramêtre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_repositoryName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - AddFileToContentsGitHub - p_repositoryName passé en paramêtre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_branchName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - AddFileToContentsGitHub - p_branchName passé en paramêtre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_newFileName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "GithubApiAction - AddFileToContentsGitHub - p_newFileName passé en paramêtre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_content))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "GithubApiAction - AddFileToContentsGitHub - p_content passé en paramêtre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_message))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "GithubApiAction - AddFileToContentsGitHub - p_content passé en paramêtre est vide"));
            }

            string fullPath = _addFileToContentsGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName).Replace(newFileName, p_newFileName);

            if (string.IsNullOrWhiteSpace(fullPath))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "GithubApiAction - AddFileToContentsGitHub - la variable fullPath assigné par la méthode _addFileToContentsGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName).Replace(newFileName, p_newFileName) est vide"));
            }

            Task<string> statusCode = addFileToContentsGithubApiRequest(fullPath, p_branchName, p_content, p_message);
            statusCode.Wait();

            return statusCode.Result;
        }

        private async Task<string> addFileToContentsGithubApiRequest(string p_githubLink, string p_branchName, string p_content, string p_message)
        {
            if (string.IsNullOrWhiteSpace(p_githubLink))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - addFileToContentsGithubApiRequest - p_githubLink passé en paramêtre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_branchName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - addFileToContentsGithubApiRequest - p_branchName passé en paramêtre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_content))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - addFileToContentsGithubApiRequest - p_content passé en paramêtre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_message))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "GithubApiAction - addFileToContentsGithubApiRequest - p_message passé en paramêtre est vide"));
            }

            Content_JSONDTO content_request = new Content_JSONDTO
            {
                branch = p_branchName,
                content = p_content,
                message = p_message
            };

            string contentRequest = JsonConvert.SerializeObject(content_request);

            if (string.IsNullOrWhiteSpace(contentRequest))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - addFileToContentsGithubApiRequest - la chaine de caractères contentRequest assignée par la méthode JsonConvert.SerializeObject(content_request) est vide"));
            }

            HttpResponseMessage response = _httpClient.PutAsync(p_githubLink, new StringContent(contentRequest, Encoding.UTF8, "application/json")).Result;

            return response.StatusCode.ToString();
        }

        public HttpResponseMessage DownloadRepository(string p_organisationName, string p_repositoryName)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "GithubApiAction - DownloadRepository - p_organisationName passé en paramêtre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_repositoryName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "GithubApiAction - DownloadRepository - p_repositoryName passé en paramêtre est vide"));
            }

            string fullPath = _downloadRepository.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName);

            if (string.IsNullOrWhiteSpace(fullPath))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "GithubApiAction - DownloadRepository - la variable fullPath assigné par la méthode _downloadRepository.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName) est vide"));
            }

            Task<HttpResponseMessage> task = GetRepositoryDownloadTask(fullPath);
            task.Wait();

            return task.Result;
        }

        private Task<HttpResponseMessage> GetRepositoryDownloadTask(string p_githubLink)
        {
            if (string.IsNullOrWhiteSpace(p_githubLink))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "GithubApiAction - GetRepositoryDownloadTask - p_githubLink passé en paramêtre est vide"));
            }

            Task<HttpResponseMessage> response = _httpClient.GetAsync(p_githubLink);

            return response;
        }
    }
}


