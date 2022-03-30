using Newtonsoft.Json;
using RPLP.DAL.DTO.Json;
using System;
using System.Collections.Generic;
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
        private string _createNewBranchInRepositoryGithub = "/repos/{organisationName}/{repositoryName}/git/refs";
        private string _createPullRequestOnRepositoBranchGithub = "/repos/{organisationName}/{repositoryName}/pulls";
        private string _addCollaboratorToRepositoryGithub = "/repos/{organisationName}/{repositoryName}/collaborators/{studentUsername}";
        private string _assignStudentToPullRequestGithub = "/repos/{organisationName}/{repositoryName}/pulls/{branchId}/requested_reviewers?";

        private const string organisationName = "{organisationName}";
        private const string repositoryName = "{repositoryName}";
        private const string branchId = "{branchId}";
        private const string studentUsername = "{studentUsername}";

        public GithubApiAction(string p_token)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://api.github.com");
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("RPLP"));
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Token", p_token);
        }

        public List<Repository_JSONDTO> GetOrganisationRepositoriesGithub(string p_organisationName)
        {
            string fullPath = _getOrganisationRepositoriesGithub.Replace(organisationName, p_organisationName);
            Task<List<Repository_JSONDTO>> RepositoriesJSON = OrganisationRepositoryGithubApiRequest(fullPath);
            RepositoriesJSON.Wait();

            return RepositoriesJSON.Result;
        }

        private static async Task<List<Repository_JSONDTO>> OrganisationRepositoryGithubApiRequest(string p_githubLink)
        {
            List<Repository_JSONDTO> repositories = new List<Repository_JSONDTO>();
            HttpResponseMessage response = await _httpClient.GetAsync(p_githubLink);

            if (response.IsSuccessStatusCode)
            {
                string JSONContent = await response.Content.ReadAsStringAsync();
                repositories = JsonConvert.DeserializeObject<List<Repository_JSONDTO>>(JSONContent);
            }

            return repositories;
        }

        public Repository_JSONDTO GetRepositoryInfoGithub(string p_organisationName, string p_repositoryName)
        {
            string fullPath = _getRepositoryInfoGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName);
            Task<Repository_JSONDTO> RepositoryJSON = RepositoryInfoGithubApiRequest(fullPath);
            RepositoryJSON.Wait();

            return RepositoryJSON.Result;
        }

        private static async Task<Repository_JSONDTO> RepositoryInfoGithubApiRequest(string p_githubLink)
        {
            Repository_JSONDTO repository = new Repository_JSONDTO();
            HttpResponseMessage response = await _httpClient.GetAsync(p_githubLink);

            if (response.IsSuccessStatusCode)
            {
                string JSONContent = await response.Content.ReadAsStringAsync();
                repository = JsonConvert.DeserializeObject<Repository_JSONDTO>(JSONContent);
            }

            return repository;
        }

        public List<Branch_JSONDTO> GetRepositoryBranchesGithub(string p_organisationName, string p_repositoryName)
        {
            string fullPath = _getBranchesFromRepositoryCommitGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName);
            Task<List<Branch_JSONDTO>> refsJSON = RepositoryBranchesGithubApiRequest(fullPath);
            refsJSON.Wait();

            return refsJSON.Result;
        }

        private static async Task<List<Branch_JSONDTO>> RepositoryBranchesGithubApiRequest(string p_githubLink)
        {
            List<Branch_JSONDTO> refs = new List<Branch_JSONDTO>();
            HttpResponseMessage response = await _httpClient.GetAsync(p_githubLink);

            if (response.IsSuccessStatusCode)
            {
                string JSONContent = await response.Content.ReadAsStringAsync();
                refs = JsonConvert.DeserializeObject<List<Branch_JSONDTO>>(JSONContent);
            }

            return refs;
        }

        public List<Commit_JSONDTO> GetRepositoryCommitsGithub(string p_organisationName, string p_repositoryName)
        {
            string fullPath = _getRepositoryCommitsGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName);
            Task<List<Commit_JSONDTO>> commitsJSON = RepositoryCommitsGithubApiRequest(fullPath);
            commitsJSON.Wait();

            return commitsJSON.Result;
        }

        private static async Task<List<Commit_JSONDTO>> RepositoryCommitsGithubApiRequest(string p_githubLink)
        {
            List<Commit_JSONDTO> commits = new List<Commit_JSONDTO>();
            HttpResponseMessage response = await _httpClient.GetAsync(p_githubLink);

            if (response.IsSuccessStatusCode)
            {
                string JSONContent = await response.Content.ReadAsStringAsync();
                commits = JsonConvert.DeserializeObject<List<Commit_JSONDTO>>(JSONContent);
            }

            return commits;
        }

        public List<PullRequest_JSONDTO> GetRepositoryPullRequestsGithub(string p_organisationName, string p_repositoryName)
        {
            string fullPath = _getPullRequestsFromRepositoryGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName);
            Task<List<PullRequest_JSONDTO>> pullRequests = RepositoryPullRequestsGithubApiRequest(fullPath);
            pullRequests.Wait();

            return pullRequests.Result;
        }

        private static async Task<List<PullRequest_JSONDTO>> RepositoryPullRequestsGithubApiRequest(string p_githubLink)
        {
            List<PullRequest_JSONDTO> branches = new List<PullRequest_JSONDTO>();
            HttpResponseMessage response = await _httpClient.GetAsync(p_githubLink);

            if (response.IsSuccessStatusCode)
            {
                string JSONContent = await response.Content.ReadAsStringAsync();
                branches = JsonConvert.DeserializeObject<List<PullRequest_JSONDTO>>(JSONContent);
            }

            return branches;
        }

        public string CreateNewBranchForFeedbackGitHub(string p_organisationName, string p_repositoryName, string p_sha, string p_newBranchName)
        {
            string fullPath = _createNewBranchInRepositoryGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName);
            Task<string> statusCode = NewBranchForFeedbackGithubApiRequest(fullPath, p_sha, p_newBranchName);
            statusCode.Wait();

            return statusCode.Result;
        }

        private async Task<string> NewBranchForFeedbackGithubApiRequest(string p_githubLink, string p_sha, string p_newBranchName)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(new HttpMethod("POST"), p_githubLink);

            requestMessage.Content = new StringContent("{\"ref\":\"refs/heads/" + p_newBranchName + "\",\"sha\":\"" + p_sha + "\"}", Encoding.UTF8, "application/json");

            HttpResponseMessage response = _httpClient.SendAsync(requestMessage).Result;

            return response.StatusCode.ToString();
        }

        public string CreateNewPullRequestFeedbackGitHub(string p_organisationName, string p_repositoryName, string p_BranchName, string p_title, string p_body)
        {
            string fullPath = _createPullRequestOnRepositoBranchGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName);
            Task<string> statusCode = NewPullRequestFeedbackGitHubApiRequest(fullPath, p_BranchName, p_title, p_body);
            statusCode.Wait();

            return statusCode.Result;
        }

        private async Task<string> NewPullRequestFeedbackGitHubApiRequest(string p_githubLink, string p_BranchName, string p_title, string p_body)
        {
            PullRequest_JSONRequest pullRequest_request = new PullRequest_JSONRequest { fromBranch = "main", targetBranch = p_BranchName, title = p_title, body = p_body };
            string pullRequest = JsonConvert.SerializeObject(pullRequest_request);
            HttpResponseMessage response = _httpClient.PostAsync(p_githubLink, new StringContent(pullRequest, Encoding.UTF8, "application/json")).Result;

            return response.StatusCode.ToString();
        }

        public string AddStudentAsCollaboratorToPeerRepositoryGithub(string p_organisationName, string p_repositoryName, string p_studentUsername)
        {
            string fullPath = _addCollaboratorToRepositoryGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName).Replace(studentUsername, p_studentUsername);
            Task<string> statusCode = AddStudentAsCollaboratorToPeerRepositoryGithubApiRequest(fullPath);
            statusCode.Wait();

            return statusCode.Result;
        }

        private async Task<string> AddStudentAsCollaboratorToPeerRepositoryGithubApiRequest(string p_githubLink)
        {
            string permission = "{\"permission\":\"Triage\"}";
            HttpResponseMessage response = _httpClient.PutAsync(p_githubLink, new StringContent(permission, Encoding.UTF8, "application/json")).Result;

            return response.StatusCode.ToString();
        }

        public string AssignReviewerToPullRequestGitHub(string p_organisationName, string p_repositoryName, string p_pullRequestName, string p_studentUsername)
        {
            List<PullRequest_JSONDTO> pullRequests = GetRepositoryPullRequestsGithub(p_organisationName, p_repositoryName);

            if (pullRequests.Count >= 1)
            {
                int branchIdResult = pullRequests.Where(pullRequest => pullRequest.title.ToLower() == p_pullRequestName.ToLower()).Select(branch => branch.number).SingleOrDefault();

                string fullPath = _assignStudentToPullRequestGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName).Replace(branchId, branchIdResult.ToString());

                Task<string> statusCode = AssignReviewerToPullRequestGitHubApiRequest(fullPath, p_studentUsername);
                statusCode.Wait();

                return statusCode.Result;
            }

            return "No Branches";

        }

        private async Task<string> AssignReviewerToPullRequestGitHubApiRequest(string p_githubLink, string p_studentUsername)
        {
            string[] reviewer = { p_studentUsername };
            Reviewer_JSONDTO reviewerJSON = new Reviewer_JSONDTO { reviewers = reviewer };
            string reviewers = JsonConvert.SerializeObject(reviewerJSON);
            HttpResponseMessage response = _httpClient.PostAsync(p_githubLink, new StringContent(reviewers, Encoding.UTF8, "application/json")).Result;

            return response.StatusCode.ToString();
        }



    }
}
