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
        private string _getOrganisationRepositoriesGithub = "/orgs/{organisationName}/repos?page={page}";
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
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://api.github.com");
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("RPLP", "1.0"));
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Token", p_token);
        }

        public List<Repository_JSONDTO> GetOrganisationRepositoriesGithub(string p_organisationName)
        {
            Dictionary<string, Repository_JSONDTO> repositories = new Dictionary<string, Repository_JSONDTO>();
            int callCount = 0;
            int maxCalls = 50;

            //Console.Out.WriteLine($"GetOrganisationRepositoriesGithub({organisationName} -> {p_organisationName}) : (_getOrganisationRepositoriesGithub)");
            string fullPath = _getOrganisationRepositoriesGithub.Replace(organisationName, p_organisationName);
            try {
                for (int i = 0; i < maxCalls; i++)
                {
                    Task<List<Repository_JSONDTO>> RepositoriesJSON = OrganisationRepositoryGithubApiRequest(fullPath.Replace("{page}", (callCount + 1).ToString()));
                    RepositoriesJSON.Wait();
                    //Console.Out.WriteLine($"Count == {RepositoriesJSON.Result.Count}");

                    RepositoriesJSON.Result.ForEach(rJson => {
                        if (!repositories.ContainsKey(rJson.name)) {
                            repositories.Add(rJson.name, rJson);
                        }
                    });
                    callCount++;
                    if (RepositoriesJSON.Result.Count == 0) {
                        //Console.Out.WriteLine("Compte avec == 0");
                        break;
                    }
                }
            }
            catch (InvalidOperationException ex) {
                Console.Error.WriteLine($"Impossible de récupérer les dépôts sur Git : {ex.Message}");
            }


            Console.Out.WriteLine(repositories.Count);

            return repositories.Values.ToList();
        }

        private static async Task<List<Repository_JSONDTO>> OrganisationRepositoryGithubApiRequest(string p_githubLink)
        {
            List<Repository_JSONDTO> repositories = new List<Repository_JSONDTO>();
            HttpResponseMessage response = await _httpClient.GetAsync(p_githubLink);

            //Console.Out.WriteLine($"OrganisationRepositoryGithubApiRequest ({p_githubLink})");

            if (response.IsSuccessStatusCode)
            {
                //Console.Out.WriteLine("OrganisationRepositoryGithubApiRequest : 200");

                string JSONContent = await response.Content.ReadAsStringAsync();
                repositories = JsonConvert.DeserializeObject<List<Repository_JSONDTO>>(JSONContent);
            } else {
                throw new InvalidOperationException($"Erreur lors de la requête vers les dépots (url : {p_githubLink}, code : {response.StatusCode})");
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
            Branch_JSONRequest branch_request = new Branch_JSONRequest
            {
                reference = $"refs/heads/{p_newBranchName}",
                sha = p_sha,
            };

            string branchRequest = JsonConvert.SerializeObject(branch_request);

            HttpResponseMessage response = _httpClient.PostAsync(p_githubLink, new StringContent(branchRequest, Encoding.UTF8, "application/json")).Result;

            return response.StatusCode.ToString();
        }

        public string CreateNewPullRequestFeedbackGitHub(string p_organisationName, string p_repositoryName, string p_BranchName, string p_title, string p_body)
        {
            Console.Out.WriteLine($"CreateNewPullRequestFeedbackGitHub({p_organisationName}, {p_repositoryName}, {p_BranchName}, {p_title}, {p_body})");
            string fullPath = _createPullRequestOnRepositoryBranchGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName);
            Console.Out.WriteLine($"requesting {fullPath}");
            Task<string> statusCode = NewPullRequestFeedbackGitHubApiRequest(fullPath, p_BranchName, p_title, p_body);
            statusCode.Wait();
            Console.Out.WriteLine($"result {statusCode.Result}");

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
            string fullPath = _addorRemoveCollaboratorToRepositoryGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName).Replace(studentUsername, p_studentUsername);
            Task<string> statusCode = AddStudentAsCollaboratorToPeerRepositoryGithubApiRequest(fullPath);
            statusCode.Wait();

            return statusCode.Result;
        }

        public List<Collaborator_JSONDTO> GetCollaboratorFromStudentRepositoryGithub(string p_organisationName, string p_repositoryName)
        {
            string fullPath = _getCollaboratorfromRepositoryGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName);
            Task<List<Collaborator_JSONDTO>> collaboratorsResult = GetCollaboratorFromStudentRepositoryGithubGithubApiRequest(fullPath);
            collaboratorsResult.Wait();

            return collaboratorsResult.Result;
        }

        private async Task<List<Collaborator_JSONDTO>> GetCollaboratorFromStudentRepositoryGithubGithubApiRequest(string p_githubLink)
        {
            List<Collaborator_JSONDTO> collaborators = new List<Collaborator_JSONDTO>();
            HttpResponseMessage response = await _httpClient.GetAsync(p_githubLink);

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
            HttpResponseMessage response = _httpClient.PutAsync(p_githubLink, new StringContent(permission, Encoding.UTF8, "application/json")).Result;

            return response.StatusCode.ToString();
        }

        public string RemoveStudentAsCollaboratorFromPeerRepositoryGithub(string p_organisationName, string p_repositoryName, string p_studentUsername)
        {
            string fullPath = _addorRemoveCollaboratorToRepositoryGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName).Replace(studentUsername, p_studentUsername);
            Task<string> statusCode = RemoveStudentAsCollaboratorFromPeerRepositoryGithubApiRequest(fullPath);
            statusCode.Wait();

            return statusCode.Result;
        }

        private async Task<string> RemoveStudentAsCollaboratorFromPeerRepositoryGithubApiRequest(string p_githubLink)
        {
            HttpResponseMessage response = _httpClient.DeleteAsync(p_githubLink).Result;

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


        public string AddFileToContentsGitHub(string p_organisationName, string p_repositoryName, string p_branchName, string p_newFileName, string p_content, string p_message)
        {
            Console.Out.WriteLine($"Trying to AddFileToContentsGitHub({p_organisationName}, {p_repositoryName}, {p_branchName}, {p_newFileName}, {p_content}, {p_message})");
            string fullPath = _addFileToContentsGithub.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName).Replace(newFileName, p_newFileName);
            Console.Out.WriteLine($"URL - {fullPath}");
            Task<string> statusCode = addFileToContentsGithubApiRequest(fullPath, p_branchName, p_content, p_message);
            statusCode.Wait();

            Console.Out.WriteLine($"Result : {statusCode.Result}");

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

            HttpResponseMessage response = _httpClient.PutAsync(p_githubLink, new StringContent(contentRequest, Encoding.UTF8, "application/json")).Result;

            return response.StatusCode.ToString();
        }

        public HttpResponseMessage DownloadRepository(string p_organisationName, string p_repositoryName)
        {
            string fullPath = _downloadRepository.Replace(organisationName, p_organisationName).Replace(repositoryName, p_repositoryName);

            Console.Out.WriteLine($"Trying to DownloadRepository - URL : {fullPath}");

            Task<HttpResponseMessage> task = GetRepositoryDownloadTask(fullPath);
            task.Wait();

            return task.Result;
        }

        private Task<HttpResponseMessage> GetRepositoryDownloadTask(string p_githubLink)
        {
            Task<HttpResponseMessage> response = _httpClient.GetAsync(p_githubLink);

            return response;
        }

    }
}


