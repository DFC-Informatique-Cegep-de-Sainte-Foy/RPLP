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

        public GithubPRCommentFetcher(string p_token, IDepotClassroom p_depotClassroom, IDepotRepository p_depotRepository)
        {
            this._depotClassroom = p_depotClassroom;
            this._depotRepository = p_depotRepository;

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

            List<string> jsons = GetPullRequestsJSONFromRepositoriesAsync(p_owner, p_classroom, p_assignment).ToList();

            foreach (var json in jsons)
            {
                var jArray = JArray.Parse(json);

                foreach (var pull in jArray)
                {
                    int prNumber = Convert.ToInt32(pull["number"].ToString());
                    string repository = pull["head"]["repo"]["name"].ToString();

                    pulls.Add(new Pull(p_owner, repository)
                    {
                        Username = pull["user"]["login"].ToString(),
                        HTML_Url = pull["html_url"].ToString(),
                        Number = prNumber,
                        Issues = this.GetPullRequestIssueAsync(p_owner, repository).Result,
                        Reviews = this.GetPullRequestReviewCommentsAsync(p_owner, repository, prNumber).Result,
                        Comments = this.GetPullRequestCommentsAsync(p_owner, repository).Result
                    });
                }
                
            }
            return pulls;
        }

        public async Task<List<ReviewerUser>> GetCommentsReviewsAndIssuesByReviewersAsync(string p_owner, string p_classroom, string p_assignment)
        {
            List<ReviewerUser> reviewerUsers = new List<ReviewerUser>();
            List<Pull>? pulls = await this.GetPullRequestsFromRepositoriesAsync(p_owner, p_classroom, p_assignment);
            HashSet<string> reviewerUsernames = await this.GetAllUniqueCommenterUsernamesFromPullListAsync(pulls);

            foreach (string reviewer in reviewerUsernames)
            {
                ReviewerUser reviewerUser = new ReviewerUser() { Username = reviewer };

                List<Pull> pullsCommentedByReviewer = new List<Pull>();

                pullsCommentedByReviewer.AddRange(pulls
                    .Where(p => 
                        p.Comments.Any(c => c.Username == reviewer) ||
                        p.Reviews.Any(r => r.Username == reviewer) ||
                        p.Issues.Any(r => r.Username == reviewer)));

                pullsCommentedByReviewer.ForEach(p =>
                {
                    if (reviewerUser.Repositories.Any(r => r.RepositoryName == p.Repository) == false)
                    {
                        reviewerUser.Repositories.Add(new RepositoryReview(p.Repository));
                    }
                });

                reviewerUser.Repositories.ForEach(r =>
                {
                    pullsCommentedByReviewer
                        .Where(p => p.Repository == r.RepositoryName)
                        .Select(p => p.Comments
                            .Where(c => c.Username == reviewer))
                        .ToList()
                        .ForEach(l => r.Comments.AddRange(l));

                    pullsCommentedByReviewer
                        .Where(p => p.Repository == r.RepositoryName)
                        .Select(p => p.Reviews
                            .Where(rev => rev.Username == reviewer))
                        .ToList()
                        .ForEach(l => r.Reviews.AddRange(l));

                    pullsCommentedByReviewer
                        .Where(p => p.Repository == r.RepositoryName)
                        .Select(p => p.Issues
                            .Where(i => i.Username == reviewer))
                        .ToList()
                        .ForEach(l => r.Issues.AddRange(l));

                });

                reviewerUser.Repositories.ForEach(r =>
                {
                    r.Comments = r.Comments.GroupBy(c => c.Position).Select(g => g.First()).ToList();
                    r.Reviews = r.Reviews.GroupBy(r => r.HTML_Url).Select(g => g.First()).ToList();
                    r.Issues = r.Issues.GroupBy(i => i.HTML_Url).Select(g => g.First()).ToList();
                });

                reviewerUsers.Add(reviewerUser);
            }

            return reviewerUsers;
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

        public IEnumerable<string> GetPullRequestsJSONFromRepositoriesAsync(string p_organisation, string p_classroomName, string p_assignment)
        {
            List<Repository> repositories = this.GetRepositoriesForAssignment(p_organisation, p_classroomName, p_assignment);

            List<string> jsons = repositories
                .Select(r =>
                {
                    Task<string> a = this._client.GetStringAsync($"/repos/{r.OrganisationName}/{r.Name}/pulls");
                    a.Wait();
                    return a.Result;
                }).ToList();

            return jsons;
        }

        private List<Repository> GetRepositoriesForAssignment(string p_organisationName, string p_classroomName, string p_assignmentName)
        {
            List<Assignment> assignmentsResult = this._depotClassroom.GetAssignmentsByClassroomName(p_classroomName);
            List<Student> studentsResult = this._depotClassroom.GetStudentsByClassroomName(p_classroomName);

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

        private async Task<HashSet<string>> GetAllUniqueCommenterUsernamesFromPullListAsync(List<Pull> pulls)
        {
            HashSet<string> reviewerUsernames = new HashSet<string>();

            pulls.ForEach(p =>
            {
                p.Comments.ForEach(c => reviewerUsernames.Add(c.Username));
                p.Issues.ForEach(c => reviewerUsernames.Add(c.Username));
                p.Reviews.ForEach(c => reviewerUsernames.Add(c.Username));
            });

            return reviewerUsernames;
        }
    }
}
