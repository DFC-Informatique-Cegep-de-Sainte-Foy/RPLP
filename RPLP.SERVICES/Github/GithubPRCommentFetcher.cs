using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RPLP.DAL.DTO.Json;
using RPLP.ENTITES;
using RPLP.SERVICES.InterfacesDepots;
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
        private readonly IDepotClassroom _depotClassroom;
        private readonly IDepotRepository _depotRepository;

        public GithubPRCommentFetcher(string p_token)
        {
            this._client = new HttpClient();
            this._client.BaseAddress = new Uri("https://api.github.com");
            this._client.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            this._client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("RPLP", "1"));
            this._client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", p_token);
        }

        public async Task<List<CommentAggregate>> GetMultipleUsersCommentsReviewsAndIssues(List<Pull> p_pulls, List<string> p_usernames)
        {
            List<CommentAggregate> comments = new List<CommentAggregate>();

            foreach (string username in p_usernames)
            {
                List<CodeComment> codeComments = new List<CodeComment>();
                List<Issue> issues = new List<Issue>();
                List<Review> reviews = new List<Review>();

                foreach (Pull pull in p_pulls)
                {
                    codeComments.AddRange(pull.Comments.Where(c => c.Username.ToLower() == username.ToLower()));
                    issues.AddRange(pull.Issues.Where(i => i.Username.ToLower() == username.ToLower()));
                    reviews.AddRange(pull.Reviews.Where(r => r.Username.ToLower() == username.ToLower()));
                }

                CommentAggregate commentAggregate = new CommentAggregate()
                {
                    Username = username,
                    Comments = codeComments,
                    Reviews = reviews,
                    Issues = issues
                };

                comments.Add(commentAggregate);
            }  

            return comments;
        }

        public async Task<CommentAggregate> GetUserCommentsReviewsAndIssues(List<Pull> p_pulls, string p_username)
        {
            List<CodeComment> codeComments = new List<CodeComment>();
            List<Issue> issues = new List<Issue>();
            List<Review> reviews = new List<Review>();

            foreach (Pull pull in p_pulls)
            {
                codeComments.AddRange(pull.Comments.Where(c => c.Username.ToLower() == p_username.ToLower()));
                issues.AddRange(pull.Issues.Where(i => i.Username.ToLower() == p_username.ToLower()));
                reviews.AddRange(pull.Reviews.Where(r => r.Username.ToLower() == p_username.ToLower()));
            }

            CommentAggregate commentAggregate = new CommentAggregate()
            {
                Username = p_username,
                Comments = codeComments,
                Reviews = reviews,
                Issues = issues
            };

            return commentAggregate;
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

        public async Task<List<CodeComment>> GetPullRequestCommentsAsync(string p_owner, string p_repository)
        {
            List<CodeComment> comments = new List<CodeComment>();

            var jsonComments = JArray.Parse(
                GetPullRequestCommentsJSONStringAsync(p_owner, p_repository).Result);

            foreach (var comment in jsonComments)
            {
                comments.Add(new CodeComment
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
                int prNumber = Convert.ToInt32(pull["number"].ToString());

                pulls.Add(new Pull(p_owner, p_repository)
                {
                    Username = pull["user"]["login"].ToString(),
                    HTML_Url = pull["html_url"].ToString(),
                    Number = prNumber,
                    Issues = this.GetPullRequestIssueAsync(p_owner, p_repository).Result,
                    Reviews = this.GetPullRequestReviewCommentsAsync(p_owner, p_repository, prNumber).Result,
                    Comments = this.GetPullRequestCommentsAsync(p_owner, p_repository).Result
                });
            }

            return pulls;
        }

        public async Task<List<Pull>> GetPullRequestsFromRepositoriesAsync(string p_owner, string p_classroom, string p_assignment)
        {
            List<Pull> pulls = new List<Pull>();

            var jsons = GetPullRequestsJSONFromRepositoriesAsync(p_owner, p_classroom, p_assignment).Result;
            var jsonPulls = jsons.Select(j => JArray.Parse(j));

            foreach (var pull in jsonPulls)
            {
                int prNumber = Convert.ToInt32(pull["number"].ToString());

                pulls.Add(new Pull(p_owner, p_assignment)
                {
                    Username = pull["user"]["login"].ToString(),
                    HTML_Url = pull["html_url"].ToString(),
                    Number = prNumber,
                    Issues = this.GetPullRequestIssueAsync(p_owner, p_assignment).Result,
                    Reviews = this.GetPullRequestReviewCommentsAsync(p_owner, p_assignment, prNumber).Result,
                    Comments = this.GetPullRequestCommentsAsync(p_owner, p_assignment).Result
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

        public async Task<List<string>> GetPullRequestsJSONFromRepositoriesAsync(string p_organisation, string p_classroomName, string p_assignment)
        {
            List<string> jsons = new List<string>();
            List<Repository> repositories = this.GetRepositoriesForAssignment(p_organisation, p_classroomName, p_assignment);

            foreach (Repository repository in repositories)
            {
                jsons.Add(await this._client.GetAsync($"/repos/{p_organisation}/{repository.Name}/pulls")
                .Result.Content.ReadAsStringAsync());
            }

            return jsons;
        }

        private List<Repository> GetRepositoriesForAssignment(string p_organisationName, string p_classroomName, string p_assignmentName)
        {
            List<Assignment> assignmentsResult = _depotClassroom.GetAssignmentsByClassroomName(p_classroomName);
            List<Student> studentsResult = this._depotClassroom.GetStudentsByClassroomName(p_organisationName);

            if (assignmentsResult.Count < 1)
                throw new ArgumentException($"No assignment in {p_classroomName}");

            Assignment assignment = assignmentsResult.SingleOrDefault(assignment => assignment.Name == p_assignmentName);

            if (assignment == null)
                throw new ArgumentException($"no assignment with name {p_assignmentName}");

            List<Repository> repositories = new List<Repository>();
            List<Repository> repositoriesResult = this._depotRepository.GetRepositoriesFromOrganisationName(p_organisationName);

            foreach (Repository repository in repositoriesResult)
            {
                string[] splitRepository = repository.Name.Split('-');

                if (splitRepository[0] == assignment.Name)
                {
                    foreach (Student student in studentsResult)
                    {
                        if (splitRepository[1] == student.Username)
                        {
                            repositories.Add(repository);
                            break;
                        }
                    }
                }
            }

            return repositories;
        }
    }
}
