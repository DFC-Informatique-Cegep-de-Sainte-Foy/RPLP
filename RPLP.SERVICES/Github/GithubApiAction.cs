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
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                            "GithubApiAction - Constructeur - p_token passé en paramètre est vide", 0));
            }

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://api.github.com");
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("RPLP", "1.0"));
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/vnd.github+json")); //changed 30/03
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Token", p_token);
            _httpClient.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28"); //added 30/03
        }

        public List<Repository_JSONDTO> GetOrganisationRepositoriesGithub(string p_organisationName)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                            "GithubApiAction - GetOrganisationRepositoriesGithub - p_organisationName passé en paramètre est vide", 0));
            }

            string fullPath = _getOrganisationRepositoriesGithub.Replace(organisationName, p_organisationName);

            if (string.IsNullOrWhiteSpace(fullPath))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                            "GithubApiAction - GetOrganisationRepositoriesGithub - la variable fullPath assigné à partir de la méthode string.Replace est vide", 0));
            }

            Task<List<Repository_JSONDTO>> RepositoriesJSON = OrganisationRepositoryGithubApiRequest(fullPath);
            RepositoriesJSON.Wait();

            return RepositoriesJSON.Result;
        }

        private static async Task<List<Repository_JSONDTO>> OrganisationRepositoryGithubApiRequest(string p_githubLink)
        {
            List<Repository_JSONDTO> repositories = new List<Repository_JSONDTO>();
            HttpResponseMessage response = await _httpClient.GetAsync(p_githubLink);

            HttpHeaders headers = response.Headers;
            int remaining = 0;
            IEnumerable<string> headerValues;
            if (headers.TryGetValues("X-RateLimit-Remaining", out headerValues))
            {
                int.TryParse(headerValues.First(), out remaining);
            }

            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(p_githubLink, (int)response.StatusCode, remaining, "Requête GET pour les dépots d'une organisation"));

            if (response.IsSuccessStatusCode)
            {
                string JSONContent = await response.Content.ReadAsStringAsync();
                repositories = JsonConvert.DeserializeObject<List<Repository_JSONDTO>>(JSONContent);
            }

            return repositories;
        }

        public Repository_JSONDTO GetRepositoryInfoGithub(string p_organisationName, string p_repositoryName)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubApiAction - GetRepositoryInfoGithub - p_organisationName passé en paramètre est vide", 0));
            }

            if (string.IsNullOrWhiteSpace(p_repositoryName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubApiAction - GetRepositoryInfoGithub - p_repositoryName passé en paramètre est vide", 0));
            }

            string fullPath = _getRepositoryInfoGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName);

            if (string.IsNullOrWhiteSpace(fullPath))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubApiAction - GetRepositoryInfoGithub - la variable fullPath assigné par la méthode _getRepositoryInfoGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName) est vide", 0));
            }

            Task<Repository_JSONDTO> RepositoryJSON = RepositoryInfoGithubApiRequest(fullPath);
            RepositoryJSON.Wait();

            return RepositoryJSON.Result;
        }

        private static async Task<Repository_JSONDTO> RepositoryInfoGithubApiRequest(string p_githubLink)
        {
            Repository_JSONDTO repository = new Repository_JSONDTO();
            HttpResponseMessage response = await _httpClient.GetAsync(p_githubLink);

            HttpHeaders headers = response.Headers;
            int remaining = 0;
            IEnumerable<string> headerValues;
            if (headers.TryGetValues("X-RateLimit-Remaining", out headerValues))
            {
                int.TryParse(headerValues.First(), out remaining);
            }

            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(p_githubLink, (int)response.StatusCode, remaining, "Requête GET pour les infos d'un dépot"));

            if (response.IsSuccessStatusCode)
            {
                string JSONContent = await response.Content.ReadAsStringAsync();
                repository = JsonConvert.DeserializeObject<Repository_JSONDTO>(JSONContent);
            }

            return repository;
        }

        public List<Branch_JSONDTO> GetRepositoryBranchesGithub(string p_organisationName, string p_repositoryName)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                   "GithubApiAction - GetRepositoryBranchesGithub - p_organisationName passé en paramètre est vide", 0));
            }

            if (string.IsNullOrWhiteSpace(p_repositoryName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                   "GithubApiAction - GetRepositoryBranchesGithub - p_repositoryName passé en paramètre est vide", 0));
            }

            Logging.Instance.Journal(new Log($"GithubApiAction - GetRepositoryBranchesGithub - Début - p_organisationName -> {p_organisationName} / p_repositoryName -> {p_repositoryName}"));

            string fullPath = _getBranchesFromRepositoryCommitGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName);

            Logging.Instance.Journal(new Log($"GithubApiAction - GetRepositoryBranchesGithub - fullPath -> {fullPath}"));

            if (string.IsNullOrWhiteSpace(fullPath))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubApiAction - GetRepositoryBranchesGithub - la variable fullPath assigné par la méthode _getBranchesFromRepositoryCommitGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName) est vide", 0));
            }
            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log($"fullPath: {fullPath}"));
            Task <List<Branch_JSONDTO>> refsJSON = RepositoryBranchesGithubApiRequest(fullPath);
            refsJSON.Wait();

            // Logging.Instance.Journal(new Log($"GithubApiAction - GetRepositoryBranchesGithub - fin - refsJSON.Result[0].reference -> {refsJSON.Result[0].reference}"));

            return refsJSON.Result;
        }

        private static async Task<List<Branch_JSONDTO>> RepositoryBranchesGithubApiRequest(string p_githubLink)
        {
            List<Branch_JSONDTO> refs = new List<Branch_JSONDTO>();
            HttpResponseMessage response = await _httpClient.GetAsync(p_githubLink);

            HttpHeaders headers = response.Headers;
            int remaining = 0;
            IEnumerable<string> headerValues;
            if (headers.TryGetValues("X-RateLimit-Remaining", out headerValues))
            {
                int.TryParse(headerValues.First(), out remaining);
            }

            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(p_githubLink, (int)response.StatusCode, remaining, "Requête GET pour les branches d'un dépots"));

            if (response.IsSuccessStatusCode)
            {
                string JSONContent = await response.Content.ReadAsStringAsync();
                refs = JsonConvert.DeserializeObject<List<Branch_JSONDTO>>(JSONContent);
            }
            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log($"GithubApiAction - RepositoryBranchesGithubApiRequest(string p_githubLink: {p_githubLink}) - refs: {refs.Count}"));
            return refs;
        }

        public List<Commit_JSONDTO> GetRepositoryCommitsGithub(string p_organisationName, string p_repositoryName)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                   "GithubApiAction - GetRepositoryCommitsGithub - p_organisationName passé en paramètre est vide", 0));
            }

            if (string.IsNullOrWhiteSpace(p_repositoryName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                   "GithubApiAction - GetRepositoryCommitsGithub - p_repositoryName passé en paramètre est vide", 0));
            }

            string fullPath = _getRepositoryCommitsGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName);

            if (string.IsNullOrWhiteSpace(fullPath))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubApiAction - GetRepositoryCommitsGithub - la variable fullPath assigné par la méthode _getRepositoryCommitsGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName) est vide", 0));
            }

            Task<List<Commit_JSONDTO>> commitsJSON = RepositoryCommitsGithubApiRequest(fullPath);
            commitsJSON.Wait();

            return commitsJSON.Result;
        }

        private static async Task<List<Commit_JSONDTO>> RepositoryCommitsGithubApiRequest(string p_githubLink)
        {
            List<Commit_JSONDTO> commits = new List<Commit_JSONDTO>();
            HttpResponseMessage response = await _httpClient.GetAsync(p_githubLink);

            HttpHeaders headers = response.Headers;
            int remaining = 0;
            IEnumerable<string> headerValues;
            if (headers.TryGetValues("X-RateLimit-Remaining", out headerValues))
            {
                int.TryParse(headerValues.First(), out remaining);
            }

            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(p_githubLink, (int)response.StatusCode, remaining, "Requête GET pour les commits d'un dépots"));

            if (response.IsSuccessStatusCode)
            {
                string JSONContent = await response.Content.ReadAsStringAsync();
                commits = JsonConvert.DeserializeObject<List<Commit_JSONDTO>>(JSONContent);
            }

            return commits;
        }

        public List<PullRequest_JSONDTO> GetRepositoryPullRequestsGithub(string p_organisationName, string p_repositoryName)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                   "GithubApiAction - GetRepositoryPullRequestsGithub - p_organisationName passé en paramètre est vide", 0));
            }

            if (string.IsNullOrWhiteSpace(p_repositoryName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                   "GithubApiAction - GetRepositoryPullRequestsGithub - p_repositoryName passé en paramètre est vide", 0));
            }

            string fullPath = _getPullRequestsFromRepositoryGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName);

            if (string.IsNullOrWhiteSpace(fullPath))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubApiAction - GetRepositoryPullRequestsGithub - la variable fullPath assigné par la méthode _getPullRequestsFromRepositoryGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName) est vide", 0));
            }

            Task<List<PullRequest_JSONDTO>> pullRequests = RepositoryPullRequestsGithubApiRequest(fullPath);
            pullRequests.Wait();

            return pullRequests.Result;
        }

        private static async Task<List<PullRequest_JSONDTO>> RepositoryPullRequestsGithubApiRequest(string p_githubLink)
        {
            List<PullRequest_JSONDTO> branches = new List<PullRequest_JSONDTO>();
            HttpResponseMessage response = await _httpClient.GetAsync(p_githubLink);

            HttpHeaders headers = response.Headers;
            int remaining = 0;
            IEnumerable<string> headerValues;
            if (headers.TryGetValues("X-RateLimit-Remaining", out headerValues))
            {
                int.TryParse(headerValues.First(), out remaining);
            }

            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(p_githubLink, (int)response.StatusCode, remaining, "Requête GET pour les pull requests d'un dépots"));

            if (response.IsSuccessStatusCode)
            {
                string JSONContent = await response.Content.ReadAsStringAsync();
                branches = JsonConvert.DeserializeObject<List<PullRequest_JSONDTO>>(JSONContent);
            }

            return branches;
        }

        public string CreateNewBranchGitHub(string p_organisationName, string p_repositoryName, string p_sha, string p_newBranchName)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                   "GithubApiAction - CreateNewBranchForFeedbackGitHub - p_organisationName passé en paramètre est vide", 0));
            }

            if (string.IsNullOrWhiteSpace(p_repositoryName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                   "GithubApiAction - CreateNewBranchForFeedbackGitHub - p_repositoryName passé en paramètre est vide", 0));
            }

            if (string.IsNullOrWhiteSpace(p_sha))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                   "GithubApiAction - CreateNewBranchForFeedbackGitHub - p_sha passé en paramètre est vide", 0));
            }

            if (string.IsNullOrWhiteSpace(p_newBranchName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                   "GithubApiAction - CreateNewBranchForFeedbackGitHub - p_newBranchName passé en paramètre est vide", 0));
            }

            string fullPath = _createNewBranchInRepositoryGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName);

            if (string.IsNullOrWhiteSpace(fullPath))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubApiAction - CreateNewBranchForFeedbackGitHub - la variable fullPath assigné par la méthode _createNewBranchInRepositoryGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName) est vide", 0));
            }

            Task<string> statusCode = NewBranchGithubApiRequest(fullPath, p_sha, p_newBranchName);
            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log($"GithubApiAction - CreateNewBranchForFeedbackGitHub - statusCode: {statusCode.Result} - organisationName:{p_organisationName} - p_repositoryName:{p_repositoryName} - p_newBranchName: {p_newBranchName}"));
            statusCode.Wait();
            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log($"GithubApiAction - CreateNewBranchForFeedbackGitHub - statusCode.Wait() - statusCode: {statusCode.Result}"));

            return statusCode.Result;
        }


        private async Task<string> NewBranchGithubApiRequest(string p_githubLink, string p_sha, string p_newBranchName)
        {
            Branch_JSONRequest branch_request = new Branch_JSONRequest
            {
                reference = $"refs/heads/{p_newBranchName}",
                sha = p_sha,
            };
            
            string branchRequest = JsonConvert.SerializeObject(branch_request);
            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log($"GithubApiAction - NewBranchForFeedbackGithubApiRequest - branchRequest: {branchRequest} - p_githubLink: {p_githubLink} - p_newBranchName: {p_newBranchName}"));

            HttpResponseMessage response = await _httpClient.PostAsync(p_githubLink, new StringContent(branchRequest, Encoding.UTF8, "application/json"));

            HttpHeaders headers = response.Headers;
            int remaining = 0;
            IEnumerable<string> headerValues;
            if (headers.TryGetValues("X-RateLimit-Remaining", out headerValues))
            {
                int.TryParse(headerValues.First(), out remaining);
            }

            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(p_githubLink, (int)response.StatusCode, remaining, $"Requête POST pour la création de la branche \"{p_newBranchName}\" pour Feedback"));

            return response.StatusCode.ToString();
        }


        public string CreateNewPullRequestGitHub(string p_organisationName, string p_repositoryName, string p_targetBranch, string p_title, string p_body, string p_fromBranch = "main")
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                   "GithubApiAction - CreateNewPullRequestitHub - p_organisationName passé en paramètre est vide", 0));
            }

            if (string.IsNullOrWhiteSpace(p_repositoryName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                   "GithubApiAction - CreateNewPullRequestGitHub - p_repositoryName passé en paramètre est vide", 0));
            }

            if (string.IsNullOrWhiteSpace(p_targetBranch))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                   "GithubApiAction - CreateNewPullRequestGitHub - p_BranchName passé en paramètre est vide", 0));
            }

            if (string.IsNullOrWhiteSpace(p_title))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                   "GithubApiAction - CreateNewPullRequestGitHub - p_title passé en paramètre est vide", 0));
            }

            if (string.IsNullOrWhiteSpace(p_body))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                   "GithubApiAction - CreateNewPullRequestGitHub - p_body passé en paramètre est vide", 0));
            }

            string fullPath = _createPullRequestOnRepositoryBranchGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName);

            if (string.IsNullOrWhiteSpace(fullPath))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubApiAction - CreateNewPullRequestGitHub - la variable fullPath assigné par la méthode _createPullRequestOnRepositoryBranchGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName) est vide", 0));
            }

            Task<string> statusCode = NewPullRequestGitHubApiRequest(fullPath, p_targetBranch, p_title, p_body, p_fromBranch);
            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log($"GithubApiAction - CreateNewPullRequestGitHub - statusCode: {statusCode.Result} - organisationName:{p_organisationName} - p_repositoryName:{p_repositoryName} - p_body: {p_body}"));
            statusCode.Wait();
            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log($"GithubApiAction - CreateNewPullRequestGitHub - statusCode: {statusCode.Result} - organisationName:{p_organisationName} - p_repositoryName:{p_repositoryName} - p_body: {p_body}"));

            return statusCode.Result;
        }

        private async Task<string> NewPullRequestGitHubApiRequest(string p_githubLink, string p_targetBranch, string p_title, string p_body, string p_fromBranch)
        {
            PullRequest_JSONRequest pullRequest_request = new PullRequest_JSONRequest
            {
                fromBranch = p_fromBranch,
                targetBranch = p_targetBranch, 
                title = p_title, 
                body = p_body
            };
            string pullRequest = JsonConvert.SerializeObject(pullRequest_request);

            HttpResponseMessage response = await _httpClient.PostAsync(p_githubLink, new StringContent(pullRequest, Encoding.UTF8, "application/vnd.github+json"));

            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log($"GithubApiAction - NewPullRequestGitHubApiRequest - pullRequest: {pullRequest} - response: {response.StatusCode.ToString()}"));

            HttpHeaders headers = response.Headers;
            int remaining = 0;
            IEnumerable<string> headerValues;
            if (headers.TryGetValues("X-RateLimit-Remaining", out headerValues))
            {
                int.TryParse(headerValues.First(), out remaining);
            }

            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(p_githubLink, (int)response.StatusCode, remaining, $"Requête POST pour la création d'une pull request pour Feedback vers la branche \"{p_targetBranch}\""));

            return response.StatusCode.ToString();
        }

        public string AddStudentAsCollaboratorToPeerRepositoryGithub(string p_organisationName, string p_repositoryName, string p_studentUsername)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                   "GithubApiAction - AddStudentAsCollaboratorToPeerRepositoryGithub - p_organisationName passé en paramètre est vide", 0));
            }

            if (string.IsNullOrWhiteSpace(p_repositoryName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                   "GithubApiAction - AddStudentAsCollaboratorToPeerRepositoryGithub - p_repositoryName passé en paramètre est vide", 0));
            }

            if (string.IsNullOrWhiteSpace(p_studentUsername))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                   "GithubApiAction - AddStudentAsCollaboratorToPeerRepositoryGithub - p_studentUsername passé en paramètre est vide", 0));
            }

            string fullPath = _addorRemoveCollaboratorToRepositoryGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName).Replace(studentUsername, p_studentUsername);

            if (string.IsNullOrWhiteSpace(fullPath))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubApiAction - AddStudentAsCollaboratorToPeerRepositoryGithub - la variable fullPath assigné par la méthode _addorRemoveCollaboratorToRepositoryGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName).Replace(studentUsername, p_studentUsername) est vide", 0));
            }

            Task<string> statusCode = AddStudentAsCollaboratorToPeerRepositoryGithubApiRequest(fullPath);
            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log($"GithubApiAction - CreateNewPullRequestFeedbackGitHub - statusCode: {statusCode.Result} - organisationName:{p_organisationName} - p_repositoryName:{p_repositoryName} - p_studentUsername: {p_studentUsername}"));
            statusCode.Wait();
            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log($"GithubApiAction - CreateNewPullRequestFeedbackGitHub - statusCode: {statusCode.Result} - organisationName:{p_organisationName} - p_repositoryName:{p_repositoryName} - p_studentUsername: {p_studentUsername}"));


            return statusCode.Result;
        }

        public List<Collaborator_JSONDTO> GetCollaboratorFromStudentRepositoryGithub(string p_organisationName, string p_repositoryName)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                   "GithubApiAction - GetCollaboratorFromStudentRepositoryGithub - p_organisationName passé en paramètre est vide", 0));
            }

            if (string.IsNullOrWhiteSpace(p_repositoryName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                   "GithubApiAction - GetCollaboratorFromStudentRepositoryGithub - p_repositoryName passé en paramètre est vide", 0));
            }

            string fullPath = _getCollaboratorfromRepositoryGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName);

            if (string.IsNullOrWhiteSpace(fullPath))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubApiAction - GetCollaboratorFromStudentRepositoryGithub - la variable fullPath assigné par la méthode _getCollaboratorfromRepositoryGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName) est vide", 0));
            }

            Task<List<Collaborator_JSONDTO>> collaboratorsResult = GetCollaboratorFromStudentRepositoryGithubGithubApiRequest(fullPath);
            collaboratorsResult.Wait();

            return collaboratorsResult.Result;
        }


      
        private async Task<List<Collaborator_JSONDTO>> GetCollaboratorFromStudentRepositoryGithubGithubApiRequest(string p_githubLink)
        {
            List<Collaborator_JSONDTO> collaborators = new List<Collaborator_JSONDTO>();
            HttpResponseMessage response = await _httpClient.GetAsync(p_githubLink);

            HttpHeaders headers = response.Headers;
            int remaining = 0;
            IEnumerable<string> headerValues;
            if (headers.TryGetValues("X-RateLimit-Remaining", out headerValues))
            {
                int.TryParse(headerValues.First(), out remaining);
            }

            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(p_githubLink, (int)response.StatusCode, remaining, "Requête GET pour les collaborateurs d'un dépot étudiant"));

            if (response.IsSuccessStatusCode)
            {
                string JSONContent = await response.Content.ReadAsStringAsync();
                collaborators = JsonConvert.DeserializeObject<List<Collaborator_JSONDTO>>(JSONContent);
            }

            return collaborators;
        }


        private async Task<string> AddStudentAsCollaboratorToPeerRepositoryGithubApiRequest(string p_githubLink)
        {
            string permission = "{\"permission\":\"Triage\"}";
            HttpResponseMessage response = await _httpClient.PutAsync(p_githubLink, new StringContent(permission, Encoding.UTF8, "application/json"));

            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log($"GithubApiAction - AddStudentAsCollaboratorToPeerRepositoryGithubApiRequest - permission: {permission} - response: {response.StatusCode.ToString()}"));

            HttpHeaders headers = response.Headers;
            int remaining = 0;
            IEnumerable<string> headerValues;
            if (headers.TryGetValues("X-RateLimit-Remaining", out headerValues))
            {
                int.TryParse(headerValues.First(), out remaining);
            }

            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(p_githubLink, (int)response.StatusCode, remaining, "Requête PUT pour l'ajout d'un étudiant en tant que collaborateur à un dépot"));

            return response.StatusCode.ToString();
        }

        public string RemoveStudentAsCollaboratorFromPeerRepositoryGithub(string p_organisationName, string p_repositoryName, string p_studentUsername)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                   "GithubApiAction - RemoveStudentAsCollaboratorFromPeerRepositoryGithub - p_organisationName passé en paramètre est vide", 0));
            }

            if (string.IsNullOrWhiteSpace(p_repositoryName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                   "GithubApiAction - RemoveStudentAsCollaboratorFromPeerRepositoryGithub - p_repositoryName passé en paramètre est vide", 0));
            }

            if (string.IsNullOrWhiteSpace(p_studentUsername))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                   "GithubApiAction - RemoveStudentAsCollaboratorFromPeerRepositoryGithub - p_studentUsername passé en paramètre est vide", 0));
            }

            string fullPath = _addorRemoveCollaboratorToRepositoryGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName).Replace(studentUsername, p_studentUsername);

            if (string.IsNullOrWhiteSpace(fullPath))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubApiAction - RemoveStudentAsCollaboratorFromPeerRepositoryGithub - la variable fullPath assigné par la méthode _addorRemoveCollaboratorToRepositoryGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName).Replace(studentUsername, p_studentUsername) est vide", 0));
            }

            Task<string> statusCode = RemoveStudentAsCollaboratorFromPeerRepositoryGithubApiRequest(fullPath);
            statusCode.Wait();

            return statusCode.Result;
        }

        private async Task<string> RemoveStudentAsCollaboratorFromPeerRepositoryGithubApiRequest(string p_githubLink)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync(p_githubLink);

            HttpHeaders headers = response.Headers;
            int remaining = 0;
            IEnumerable<string> headerValues;
            if (headers.TryGetValues("X-RateLimit-Remaining", out headerValues))
            {
                int.TryParse(headerValues.First(), out remaining);
            }

            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(p_githubLink, (int)response.StatusCode, remaining, "Requête DELETE pour la suppression d'un étudiant comme collaborateur d'un dépot"));

            return response.StatusCode.ToString();
        }

        public string AssignReviewerToPullRequestGitHub(string p_organisationName, string p_repositoryName, string p_pullRequestName, string p_studentUsername)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                   "GithubApiAction - AssignReviewerToPullRequestGitHub - p_organisationName passé en paramètre est vide", 0));
            }

            if (string.IsNullOrWhiteSpace(p_repositoryName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                   "GithubApiAction - AssignReviewerToPullRequestGitHub - p_repositoryName passé en paramètre est vide", 0));
            }

            if (string.IsNullOrWhiteSpace(p_pullRequestName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                   "GithubApiAction - AssignReviewerToPullRequestGitHub - p_pullRequestName passé en paramètre est vide", 0));
            }

            if (string.IsNullOrWhiteSpace(p_studentUsername))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubApiAction - AssignReviewerToPullRequestGitHub - p_studentUsername passé en paramètre est vide", 0));
            }

            List<PullRequest_JSONDTO> pullRequests = GetRepositoryPullRequestsGithub(p_organisationName, p_repositoryName);

            if (pullRequests.Count >= 1)
            {
                int branchIdResult = pullRequests.Where(pullRequest => pullRequest.title.ToLower() == p_pullRequestName.ToLower()).Select(branch => branch.number).SingleOrDefault();

                string fullPath = _assignStudentToPullRequestGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName).Replace(branchId, branchIdResult.ToString());

                if (string.IsNullOrWhiteSpace(fullPath))
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "GithubApiAction - RemoveStudentAsCollaboratorFromPeerRepositoryGithub - la variable fullPath assigné par la méthode _addorRemoveCollaboratorToRepositoryGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName).Replace(studentUsername, p_studentUsername) est vide", 0));
                }

                Task<string> statusCode = AssignReviewerToPullRequestGitHubApiRequest(fullPath, p_studentUsername);
                statusCode.Wait();

                return statusCode.Result;
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubApiAction - AssignReviewerToPullRequestGitHub - la variable pullRequests assignée par la méthode GetRepositoryPullRequestsGithub(p_organisationName, p_repositoryName) est null", 0));
            }

            return "No Branches";
        }

        private async Task<string> AssignReviewerToPullRequestGitHubApiRequest(string p_githubLink, string p_studentUsername)
        {
            string[] reviewer = { p_studentUsername };
            Reviewer_JSONDTO reviewerJSON = new Reviewer_JSONDTO { reviewers = reviewer };
            string reviewers = JsonConvert.SerializeObject(reviewerJSON);

            if (string.IsNullOrWhiteSpace(reviewers))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                   "GithubApiAction - AssignReviewerToPullRequestGitHubApiRequest - la chaine de caractère reviewers assignée par la méthode JsonConvert.SerializeObject(reviewerJSON); est vide", 0));
            }

            HttpResponseMessage response = await _httpClient.PostAsync(p_githubLink, new StringContent(reviewers, Encoding.UTF8, "application/json"));

            HttpHeaders headers = response.Headers;
            int remaining = 0;
            IEnumerable<string> headerValues;
            if (headers.TryGetValues("X-RateLimit-Remaining", out headerValues))
            {
                int.TryParse(headerValues.First(), out remaining);
            }

            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(p_githubLink, (int)response.StatusCode, remaining, $"Requête POST pour l'assignation d'un reviewer (\"{p_studentUsername}\") à une pull request"));

            return response.StatusCode.ToString();
        }

        public string AddFileToContentsGitHub(string p_organisationName, string p_repositoryName, string p_branchName, string p_newFileName, string p_content, string p_message)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                   "GithubApiAction - AddFileToContentsGitHub - p_organisationName passé en paramètre est vide", 0));
            }

            if (string.IsNullOrWhiteSpace(p_repositoryName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                   "GithubApiAction - AddFileToContentsGitHub - p_repositoryName passé en paramètre est vide", 0));
            }

            if (string.IsNullOrWhiteSpace(p_branchName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                   "GithubApiAction - AddFileToContentsGitHub - p_branchName passé en paramètre est vide", 0));
            }

            if (string.IsNullOrWhiteSpace(p_newFileName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubApiAction - AddFileToContentsGitHub - p_newFileName passé en paramètre est vide", 0));
            }

            if (string.IsNullOrWhiteSpace(p_content))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubApiAction - AddFileToContentsGitHub - p_content passé en paramètre est vide", 0));
            }

            if (string.IsNullOrWhiteSpace(p_message))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubApiAction - AddFileToContentsGitHub - p_content passé en paramètre est vide", 0));
            }

            string fullPath = _addFileToContentsGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName).Replace(newFileName, p_newFileName);

            if (string.IsNullOrWhiteSpace(fullPath))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubApiAction - AddFileToContentsGitHub - la variable fullPath assigné par la méthode _addFileToContentsGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName).Replace(newFileName, p_newFileName) est vide", 0));
            }

            Task<string> statusCode = addFileToContentsGithubApiRequest(fullPath, p_branchName, p_content, p_message);
            statusCode.Wait();
            return statusCode.Result;
        }

        private async Task<string> addFileToContentsGithubApiRequest(string p_githubLink, string p_branchName, string p_content, string p_message)
        {
            Content_JSONDTO content_request = new Content_JSONDTO
            {
                branch = p_branchName,
                content = p_content,
                message = p_message
            };

            string contentRequest = JsonConvert.SerializeObject(content_request);

            if (string.IsNullOrWhiteSpace(contentRequest))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                   "GithubApiAction - addFileToContentsGithubApiRequest - la chaine de caractères contentRequest assignée par la méthode JsonConvert.SerializeObject(content_request) est vide", 0));
            }

            HttpResponseMessage response = await _httpClient.PutAsync(p_githubLink, new StringContent(contentRequest, Encoding.UTF8, "application/vnd.github+json"));

            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log($"GitHubApiAction - addFileToContentsGithubApiRequest" +
                                                        $"requete vers: {p_githubLink}" +
                                                        $"brachName: {p_branchName}" +
                                                        $"content: {p_content}" +
                                                        $"message: {p_message}" +
                                                        $"contentRequest: {contentRequest}"));

            
            HttpHeaders headers = response.Headers;
            int remaining = 0;
            IEnumerable<string> headerValues;
            if (headers.TryGetValues("X-RateLimit-Remaining", out headerValues))
            {
                int.TryParse(headerValues.First(), out remaining);
            }

            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(p_githubLink, (int)response.StatusCode, remaining, $"Requête PUT pour ajouter un fichier à un dépot - status : {response.StatusCode.ToString()}"));

            return response.StatusCode.ToString();
        }

        public HttpResponseMessage DownloadRepository(string p_organisationName, string p_repositoryName)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                   "GithubApiAction - DownloadRepository - p_organisationName passé en paramètre est vide", 0));
            }

            if (string.IsNullOrWhiteSpace(p_repositoryName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubApiAction - DownloadRepository - p_repositoryName passé en paramètre est vide", 0));
            }

            string fullPath = _downloadRepository.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName);

            if (string.IsNullOrWhiteSpace(fullPath))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "GithubApiAction - DownloadRepository - la variable fullPath assigné par la méthode _downloadRepository.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName) est vide", 0));
            }

            Task<HttpResponseMessage> task = GetRepositoryDownloadTask(fullPath);
            task.Wait();

            HttpHeaders headers = task.Result.Headers;
            int remaining = 0;
            IEnumerable<string> headerValues;
            if (headers.TryGetValues("X-RateLimit-Remaining", out headerValues))
            {
                int.TryParse(headerValues.First(), out remaining);
            }

            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(fullPath, (int)task.Result.StatusCode, remaining, "Requête GET pour télécharger un dépot"));


            return task.Result;
        }

        // méthode à vérifier, devrait-elle être async
        private Task<HttpResponseMessage> GetRepositoryDownloadTask(string p_githubLink)
        {
            Task<HttpResponseMessage> response = _httpClient.GetAsync(p_githubLink);

            return response;
        }
    }
}


