using Newtonsoft.Json;
using RPLP.DAL.DTO.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
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
        private string _getRefAndSHAFromRepositoryCommitGithub = "/repos/{organisationName}/{repositoryName}/git/refs/heads";
        private string _createNewBranchInRepositoryGithub = "/repos/{organisationName}/{repositoryName}/git/refs";
        private string _createPullRequestOnRepositoBranch = "/repos/{organisationName}/{repositoryName}/pulls";

        private const string organisationName = "{organisationName}";
        private const string repositoryName = "{repositoryName}";

        public GithubApiAction(string p_token)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://api.github.com");
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("AppName", "1.0"));
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Token", p_token);
        }

        //Change return type to organisation when json are made
        public List<Repository_JSONDTO> GetOrganisationRepositoriesGithub(string p_organisationName)
        {
            Task<List<Repository_JSONDTO>> RepositoryJSON = OrganisationRepositoryGithubApiRequest(_getOrganisationRepositoriesGithub, p_organisationName);
            RepositoryJSON.Wait();

            return RepositoryJSON.Result;
        }

        //Change return type to organisation when json are made
        public static async Task<List<Repository_JSONDTO>> OrganisationRepositoryGithubApiRequest(string p_githubLink, string p_organisationName)
        {
            List<Repository_JSONDTO> repositories = new List<Repository_JSONDTO>();
            string fullPath = p_githubLink.Replace(organisationName, p_organisationName);
            HttpResponseMessage response = await _httpClient.GetAsync(fullPath);

            if (response.IsSuccessStatusCode)
            {
                string JSONContent = await response.Content.ReadAsStringAsync();
                repositories = JsonConvert.DeserializeObject<List<Repository_JSONDTO>>(JSONContent);
            }

            return repositories;
        }

        //Change return type to RepositoryInfo when json are made
        public static async Task GetRepositoryInfoGithub(string p_organisationName, string p_repositoryName)
        {

        }

        //Change return type to List<CommitInfo> when json are made
        public static async Task GetRefAndSHAFromRepositoryCommitGithub(string p_organisationName, string p_repositoryName)
        {

        }
    }
}
