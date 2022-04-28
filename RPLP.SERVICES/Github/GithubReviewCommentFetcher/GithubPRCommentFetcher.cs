using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RPLP.SERVICES.Github.GithubReviewCommentFetcher.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.SERVICES.Github.GithubReviewCommentFetcher
{
    public class GithubPRCommentFetcher
    {
        private HttpClient _client;
        public GithubPRCommentFetcher()
        {
            this._client = new HttpClient();
            this._client.BaseAddress = new Uri("https://api.github.com");
            this._client.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            this._client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("RPLP", "1"));
        }



        public async Task<List<Issue>> GetPullRequestIssueAsync(string p_owner, string p_repository)
        {
            List<Issue> issues = new List<Issue>();

            var jsonIssues = JArray.Parse(
                GetRepositoryIssueCommentsJSONStringAsync(p_owner, p_repository).Result);

            foreach (var issue in jsonIssues)
            {
                issues.Add(new Issue
                {
                    Username = issue["user"]["login"].ToString(),
                    Body = issue["body"].ToString(),
                    HTML_Url = issue["html_url"].ToString(),
                });
            }

            return issues;
        }

        public async Task<List<Review>> GetPullRequestReviewCommentsAsync(string p_owner, string p_repository, int p_pullNumber)
        {
            List<Review> reviews = new List<Review>();

            var jsonReviews = JArray.Parse(
                GetPullRequestReviewsJSONStringAsync(p_owner, p_repository, p_pullNumber).Result);

            foreach (var review in jsonReviews)
            {
                reviews.Add(new Review
                {
                    Username = review["user"]["login"].ToString(),
                    Body = review["body"].ToString(),
                    HTML_Url = review["html_url"].ToString(),
                    Pull_Request_Url = review["pull_request_url"].ToString()
                });
            }

            return reviews;
        }

        public async Task<List<Comment>> GetPullRequestCommentsAsync(string p_owner, string p_repository)
        {
            List<Comment> comments = new List<Comment>();

            var jsonComments = JArray.Parse(
                GetPullRequestCommentsJSONStringAsync(p_owner, p_repository).Result);

            foreach (var comment in jsonComments)
            {
                comments.Add(new Comment
                {
                    Username = comment["user"]["login"].ToString(),
                    Diff_Hunk = comment["diff_hunk"].ToString(),
                    Position = Convert.ToInt32(comment["position"].ToString()),
                    Body = comment["body"].ToString(),
                    Path = comment["path"].ToString(),
                    PullRequestURL = comment["url"].ToString()
                });
            }

            return comments;
        }

        public async Task<List<Pull>> GetPullRequestsFromRepositoryAsync(string p_owner, string p_repository)
        {
            List<Pull> pulls = new List<Pull>();

            var jsonPulls = JArray.Parse(
                GetPullRequestsJSONFromRepositoryAsync(p_owner, p_repository).Result);

            foreach (var pull in jsonPulls)
            {
                pulls.Add(new Pull(p_owner, p_repository, Convert.ToInt32(pull["number"].ToString()))
                {
                    Username = pull["user"]["login"].ToString(),
                    HTML_Url = pull["html_url"].ToString()
                });
            }

            return pulls;
        }

        public async Task<string> GetRepositoryIssueCommentsJSONStringAsync(string p_owner, string p_repository)
        {
            return await this._client.GetAsync($"/repos/{p_owner}/{p_repository}/issues/comments")
                .Result.Content.ReadAsStringAsync();
        }

        public async Task<string> GetPullRequestReviewsJSONStringAsync(string p_owner, string p_repository, int p_pullNumber)
        {
            return await this._client.GetAsync($"/repos/{p_owner}/{p_repository}/pulls/{p_pullNumber}/reviews")
                .Result.Content.ReadAsStringAsync();
        }

        public async Task<string> GetPullRequestCommentsJSONStringAsync(string p_owner, string p_repository)
        {
            return await this._client.GetAsync($"/repos/{p_owner}/{p_repository}/pulls/comments")
                .Result.Content.ReadAsStringAsync();
        }

        public async Task<string> GetPullRequestsJSONFromRepositoryAsync(string p_owner, string p_repository)
        {
            return await this._client.GetAsync($"/repos/{p_owner}/{p_repository}/pulls")
                .Result.Content.ReadAsStringAsync();
        }
    }
}
