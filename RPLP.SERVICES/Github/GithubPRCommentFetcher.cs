﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RPLP.DAL.DTO.Json;
using RPLP.ENTITES;
using RPLP.JOURNALISATION;
using RPLP.SERVICES.InterfacesDepots;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
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
            if (string.IsNullOrWhiteSpace(p_token))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - Constructeur - p_token passé en paramètre est vide"));
            }

            if (p_depotClassroom == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - Constructeur - p_depotClassroom passé en paramètre est null"));
            }

            if (p_depotRepository == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - Constructeur - p_depotClassroom passé en paramètre est null"));
            }

            this._depotClassroom = p_depotClassroom;
            this._depotRepository = p_depotRepository;

            this._client = new HttpClient();
            this._client.BaseAddress = new Uri("https://api.github.com");
            this._client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            this._client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("RPLP", "1"));
            this._client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", p_token);
        }

        public async Task<List<CommentAggregate>> GetMultipleUsersCommentsReviewsAndIssues(List<Pull> p_pulls, List<string> p_usernames)
        {
            if (p_pulls == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - GetMultipleUsersCommentsReviewsAndIssues - p_pulls passé en paramètre est null"));
            }

            if (p_usernames == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - GetMultipleUsersCommentsReviewsAndIssues - p_usernames passé en paramètre est null"));
            }

            List<CommentAggregate> comments = new List<CommentAggregate>();

            foreach (string username in p_usernames)
            {
                CommentAggregate commentAggregate = GetCommentAggregateFromPulls(p_pulls, username);
                comments.Add(commentAggregate);
            }  

            return comments;
        }

        public async Task<CommentAggregate> GetUserCommentsReviewsAndIssues(List<Pull> p_pulls, string p_username)
        {
            if (p_pulls == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - GetUserCommentsReviewsAndIssues - p_pulls passé en paramètre est null"));
            }

            if (string.IsNullOrWhiteSpace(p_username))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - GetUserCommentsReviewsAndIssues - p_username passé en paramètre est null"));
            }

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
            if (string.IsNullOrWhiteSpace(p_owner))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - GetPullRequestIssueAsync - p_owner passé en paramètre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_repository))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - GetPullRequestIssueAsync - p_repository passé en paramètre est vide"));
            }

            List<Issue> issues = new List<Issue>();

            var jsonIssues = JArray.Parse(
                GetRepositoryIssueCommentsJSONStringAsync(p_owner, p_repository).Result);

            if(jsonIssues.Count == 0)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - GetPullRequestIssueAsync - la variable jsonIssues assigné par la méthode  GetRepositoryIssueCommentsJSONStringAsync(p_owner, p_repository).Result) est null"));
            }

            foreach (var issueJToken in jsonIssues)
            {
                Issue issue = GetIssueFromJToken(issueJToken);
                issues.Add(issue);
            }

            return issues;
        }

        public async Task<List<Review>> GetPullRequestReviewCommentsAsync(string p_owner, string p_repository, int p_pullNumber)
        {
            if (string.IsNullOrWhiteSpace(p_owner))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - GetPullRequestReviewCommentsAsync - p_owner passé en paramètre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_repository))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - GetPullRequestReviewCommentsAsync - p_repository passé en paramètre est vide"));
            }

            if(p_pullNumber <= 0)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - GetPullRequestReviewCommentsAsync - p_pullNumber passé en paramètre est hors des limites"));
            }

            List<Review> reviews = new List<Review>();

            var jsonReviews = JArray.Parse(
                GetPullRequestReviewsJSONStringAsync(p_owner, p_repository, p_pullNumber).Result);

            if (jsonReviews.Count == 0)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - GetPullRequestReviewCommentsAsync - la variable jsonReviews assignée par la méthode  GetPullRequestReviewsJSONStringAsync(p_owner, p_repository, p_pullNumber).Result) est null"));
            }

            foreach (var reviewJToken in jsonReviews)
            {
                Review review = GetReviewFromJToken(reviewJToken);
                reviews.Add(review);
            }

            return reviews;
        }

        public async Task<List<CodeComment>> GetPullRequestCommentsAsync(string p_owner, string p_repository)
        {
            if (string.IsNullOrWhiteSpace(p_owner))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - GetPullRequestCommentsAsync - p_owner passé en paramètre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_repository))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - GetPullRequestCommentsAsync - p_repository passé en paramètre est vide"));
            }

            List<CodeComment> comments = new List<CodeComment>();

            var jsonComments = JArray.Parse(
                GetPullRequestCommentsJSONStringAsync(p_owner, p_repository).Result);

            if (jsonComments.Count == 0)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - GetPullRequestCommentsAsync - la variable jsonComments assignée par la méthode  GetPullRequestCommentsJSONStringAsync(p_owner, p_repository) est vide"));
            }

            foreach (var commentJToken in jsonComments)
            {
                CodeComment codeComment = GetCodeCommentFromJToken(commentJToken);
                comments.Add(codeComment);
            }

            return comments;
        }

        public async Task<List<Pull>> GetPullRequestsFromRepositoryAsync(string p_owner, string p_repository)
        {
            if (string.IsNullOrWhiteSpace(p_owner))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - GetPullRequestsFromRepositoryAsync - p_owner passé en paramètre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_repository))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - GetPullRequestsFromRepositoryAsync - p_repository passé en paramètre est vide"));
            }

            List<Pull> pulls = new List<Pull>();

            var jsonPulls = JArray.Parse(
                GetPullRequestsJSONFromRepositoryAsync(p_owner, p_repository).Result);

            if (jsonPulls.Count == 0)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - GetPullRequestsFromRepositoryAsync - la variable jsonPulls assignée par la méthode  GetPullRequestsJSONFromRepositoryAsync(p_owner, p_repository) est vide"));
            }

            foreach (var pullJToken in jsonPulls)
            {
                Pull pull = GetPullFromJToken(pullJToken, p_owner, p_repository);
                pulls.Add(pull);
            }

            return pulls;
        }

        public async Task<List<Pull>> GetPullRequestsFromRepositoriesAsync(string p_owner, string p_classroom, string p_assignment)
        {
            if (string.IsNullOrWhiteSpace(p_owner))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - GetPullRequestsFromRepositoriesAsync - p_owner passé en paramètre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_classroom))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - GetPullRequestsFromRepositoriesAsync - p_classroom passé en paramètre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_assignment))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - GetPullRequestsFromRepositoriesAsync - p_assignment passé en paramètre est vide"));
            }

            List<Pull> pulls = new List<Pull>();

            List<string> jsons = GetPullRequestsJSONFromRepositoriesAsync(p_owner, p_classroom, p_assignment).ToList();

            if(jsons.Count == 0)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "GithubPRCommentFetcher - GetPullRequestsFromRepositoriesAsync - la variable jsons assignée à partir de GetPullRequestsJSONFromRepositoriesAsync est vide ou null"));
            }

            foreach (var json in jsons)
            {
                var jArray = JArray.Parse(json);

                if (jArray.Count == 0)
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "GithubPRCommentFetcher - GetPullRequestsFromRepositoriesAsync - la variable jArray assignée à partir de JArray.Parse(json) est vide ou null"));
                }

                foreach (var pullJToken in jArray)
                {
                    Pull pull = GetPullFromJToken(pullJToken, p_owner);
                    pulls.Add(pull);
                }
                
            }
            return pulls;
        }

        public async Task<List<ReviewerUser>> GetCommentsReviewsAndIssuesByReviewersAsync(string p_owner, string p_classroom, string p_assignment)
        {
            if (string.IsNullOrWhiteSpace(p_owner))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - GetCommentsReviewsAndIssuesByReviewersAsync - p_owner passé en paramètre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_classroom))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - GetCommentsReviewsAndIssuesByReviewersAsync - p_classroom passé en paramètre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_assignment))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - GetCommentsReviewsAndIssuesByReviewersAsync - p_assignment passé en paramètre est vide"));
            }

            List<ReviewerUser> reviewerUsers = new List<ReviewerUser>();
            List<Pull>? pulls = await this.GetPullRequestsFromRepositoriesAsync(p_owner, p_classroom, p_assignment);

            if (pulls.Count == 0 || pulls == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - GetCommentsReviewsAndIssuesByReviewersAsync - variable pulls assignée à partir de this.GetPullRequestsFromRepositoriesAsync(p_owner, p_classroom, p_assignment) est null ou vide"));
            }

            HashSet<string> reviewerUsernames = await this.GetAllUniqueCommenterUsernamesFromPullListAsync(pulls);

            if (reviewerUsernames.Count == 0 || reviewerUsernames == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - GetCommentsReviewsAndIssuesByReviewersAsync - variable reviewerUsernames assignée à partir de this.GetAllUniqueCommenterUsernamesFromPullListAsync(pulls) est null ou vide"));
            }

            foreach (string reviewer in reviewerUsernames)
            {
                ReviewerUser reviewerUser = new ReviewerUser() { Username = reviewer };
                List<Pull> pullsCommentedByReviewer = new List<Pull>();

                pullsCommentedByReviewer = GetPullsCommentedByReviewer(pulls, reviewer);

                if (pullsCommentedByReviewer.Count == 0 || pullsCommentedByReviewer == null)
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - GetCommentsReviewsAndIssuesByReviewersAsync - variable pullsCommentedByReviewer assignée à partir de GetPullsCommentedByReviewer(pulls, reviewer) est null ou vide"));
                }

                reviewerUser = AddMissingRepositoriesFromPullList(reviewerUser, pullsCommentedByReviewer);
                reviewerUser.Repositories = AddCommentsIssuesAndReviewsFromPullsIntoRepositories(reviewerUser.Repositories, pullsCommentedByReviewer, reviewer);
                reviewerUser.Repositories = GetOnlyUniqueCommentsReviewsAndIssuesFromRepositories(reviewerUser.Repositories);

                reviewerUsers.Add(reviewerUser);
            }

            return reviewerUsers;
        }

        public async Task<string> GetRepositoryIssueCommentsJSONStringAsync(string p_owner, string p_repository)
        {
            if (string.IsNullOrWhiteSpace(p_owner))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - GetRepositoryIssueCommentsJSONStringAsync - p_owner passé en paramètre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_repository))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - GetRepositoryIssueCommentsJSONStringAsync - p_repository passé en paramètre est vide"));
            }

            //Gérer le client http
             return await this._client.GetAsync($"/repos/{p_owner}/{p_repository}/issues/comments")
                .Result.Content.ReadAsStringAsync();

        }

        public async Task<string> GetPullRequestReviewsJSONStringAsync(string p_owner, string p_repository, int p_pullNumber)
        {
            if (string.IsNullOrWhiteSpace(p_owner))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - GetPullRequestReviewsJSONStringAsync - p_owner passé en paramètre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_repository))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - GetPullRequestReviewsJSONStringAsync - p_repository passé en paramètre est vide"));
            }

            if (p_pullNumber <= 0)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - GetPullRequestReviewsJSONStringAsync - p_pullNumber passé en paramètre est hors des limites"));
            }

            //Gérer le client http
            return await this._client.GetAsync($"/repos/{p_owner}/{p_repository}/pulls/{p_pullNumber}/reviews")
                    .Result.Content.ReadAsStringAsync();


        }

        public async Task<string> GetPullRequestCommentsJSONStringAsync(string p_owner, string p_repository)
        {
            if (string.IsNullOrWhiteSpace(p_owner))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - GetPullRequestCommentsJSONStringAsync - p_owner passé en paramètre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_repository))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - GetPullRequestCommentsJSONStringAsync - p_repository passé en paramètre est vide"));
            }

            //Gérer le client http
            return await this._client.GetAsync($"/repos/{p_owner}/{p_repository}/pulls/comments")
                .Result.Content.ReadAsStringAsync();
        }

        public async Task<string> GetPullRequestsJSONFromRepositoryAsync(string p_owner, string p_repository)
        {
            if (string.IsNullOrWhiteSpace(p_owner))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - GetPullRequestsJSONFromRepositoryAsync - p_owner passé en paramètre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_repository))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - GetPullRequestsJSONFromRepositoryAsync - p_repository passé en paramètre est vide"));
            }

            //Gérer le client http
            return await this._client.GetAsync($"/repos/{p_owner}/{p_repository}/pulls")
                .Result.Content.ReadAsStringAsync();
        }

        public IEnumerable<string> GetPullRequestsJSONFromRepositoriesAsync(string p_organisation, string p_classroomName, string p_assignment)
        {
            if (string.IsNullOrWhiteSpace(p_organisation))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - GetPullRequestsJSONFromRepositoriesAsync - p_organisation passé en paramètre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_classroomName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - GetPullRequestsJSONFromRepositoriesAsync - p_classroomName passé en paramètre est vide"));
            }

            if (string.IsNullOrWhiteSpace(p_assignment))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - GetPullRequestsJSONFromRepositoriesAsync - p_assignment passé en paramètre est vide"));
            }

            List<Repository> repositories = this.GetRepositoriesForAssignment(p_organisation, p_classroomName, p_assignment);

            if (repositories.Count == 0 || repositories == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                            "GithubPRCommentFetcher - GetPullRequestsJSONFromRepositoriesAsync - la variable repositories assignée à partir de this.GetRepositoriesForAssignment(p_organisation, p_classroomName, p_assignment) est vide ou null"));
            }

            List<string> jsons = repositories
                .Select(r =>
                {
                    Task<string> a = this._client.GetStringAsync($"/repos/{r.OrganisationName}/{r.Name}/pulls");
                    a.Wait();
                    return a.Result;
                }).ToList();

            return jsons;
        }

        #region Private Methods

        private List<Repository> GetRepositoriesForAssignment(string p_organisationName, string p_classroomName, string p_assignmentName)
        {
            List<Assignment> assignmentsResult = this._depotClassroom.GetAssignmentsByClassroomName(p_classroomName);
            List<Student> studentsResult = this._depotClassroom.GetStudentsByClassroomName(p_classroomName);

            if (assignmentsResult.Count == 0)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                           "GithubPRCommentFetcher - GetRepositoriesForAssignment - la variable assignmentsResult assigné à partir de this._depotClassroom.GetAssignmentsByClassroomName(p_classroomName); est null ou vide"));

                throw new ArgumentException($"No assignment in {p_classroomName}");
            }

            Assignment assignment = assignmentsResult.SingleOrDefault(assignment => assignment.Name == p_assignmentName);

            if (assignment == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                          "GithubPRCommentFetcher - GetRepositoriesForAssignment - la variable assignment assignée à partir de la méthode assignmentsResult.SingleOrDefault(assignment => assignment.Name == p_assignmentName) est null"));

                throw new ArgumentException($"no assignment with name {p_assignmentName}");
            }

            List<Repository> repositoriesResult = this._depotRepository.GetRepositoriesFromOrganisationName(p_organisationName);

            if (repositoriesResult == null || repositoriesResult.Count == 0)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                          "GithubPRCommentFetcher - GetRepositoriesForAssignment - variable repositoriesResult assignée à partir de la méthode this._depotRepository.GetRepositoriesFromOrganisationName(p_organisationName) est null ou vide"));

                throw new ArgumentException($"no assignment with name {p_assignmentName}");
            }

            List<Repository> repositories = GetRepositoriesOfStudentsForAssignment(repositoriesResult, studentsResult, assignment.Name);

            if (repositories == null || repositories.Count == 0)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                          "GithubPRCommentFetcher - GetRepositoriesForAssignment - variable repositories assignée à partir de la méthode GetRepositoriesOfStudentsForAssignment(repositoriesResult, studentsResult, assignment.Name) est null ou vide"));

                throw new ArgumentException($"no assignment with name {p_assignmentName}");
            }

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

        private CommentAggregate GetCommentAggregateFromPulls(List<Pull> p_pulls, string p_username)
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

        private Issue GetIssueFromJToken(JToken jsonIssue)
        {
            if (jsonIssue == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                          "GithubPRCommentFetcher - GetIssueFromJToken - jsonIssue passé en paramètre est null"));
            }

            Issue issue = new Issue
            {
                Username = jsonIssue["user"]["login"].ToString(),
                Body = jsonIssue["body"].ToString(),
                HTML_Url = jsonIssue["html_url"].ToString(),
            };

            return issue;
        }

        private Review GetReviewFromJToken(JToken jsonReview)
        {
            if (jsonReview == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                          "GithubPRCommentFetcher - GetReviewFromJToken - jsonReview passé en paramètre est null"));
            }

            Review review = new Review
            {
                Username = jsonReview["user"]["login"].ToString(),
                Body = jsonReview["body"].ToString(),
                HTML_Url = jsonReview["html_url"].ToString(),
                Pull_Request_Url = jsonReview["pull_request_url"].ToString()
            };

            return review;
        }

        private CodeComment GetCodeCommentFromJToken(JToken jsonComment)
        {
            if (jsonComment == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                          "GithubPRCommentFetcher - GetCodeCommentFromJToken - jsonComment passé en paramètre est null"));
            }

            CodeComment codeComment = new CodeComment
            {
                Username = jsonComment["user"]["login"].ToString(),
                Diff_Hunk = jsonComment["diff_hunk"].ToString(),
                Position = Convert.ToInt32(jsonComment["position"].ToString()),
                Body = jsonComment["body"].ToString(),
                Path = jsonComment["path"].ToString(),
                PullRequestURL = jsonComment["url"].ToString()
            };

            return codeComment;
        }

        private Pull GetPullFromJToken(JToken jsonPull, string p_owner)
        {
            if (jsonPull == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                          "GithubPRCommentFetcher - GetPullFromJToken - jsonPull passé en paramètre est null"));
            }

            int prNumber = Convert.ToInt32(jsonPull["number"].ToString());
            string repository = jsonPull["head"]["repo"]["name"].ToString();

            Pull pull = new Pull(p_owner, repository)
            {
                Username = jsonPull["user"]["login"].ToString(),
                HTML_Url = jsonPull["html_url"].ToString(),
                Number = prNumber,
                Issues = this.GetPullRequestIssueAsync(p_owner, repository).Result,
                Reviews = this.GetPullRequestReviewCommentsAsync(p_owner, repository, prNumber).Result,
                Comments = this.GetPullRequestCommentsAsync(p_owner, repository).Result
            };

            return pull;
        }

        private Pull GetPullFromJToken(JToken jsonPull, string p_owner, string p_repository)
        {
            if (jsonPull == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                          "GithubPRCommentFetcher - GetPullFromJToken(JToken jsonPull, string p_owner, string p_repository)  - jsonPull passé en paramètre est null"));
            }

            int prNumber = Convert.ToInt32(jsonPull["number"].ToString());

            Pull pull = new Pull(p_owner, p_repository)
            {
                Username = jsonPull["user"]["login"].ToString(),
                HTML_Url = jsonPull["html_url"].ToString(),
                Number = prNumber,
                Issues = this.GetPullRequestIssueAsync(p_owner, p_repository).Result,
                Reviews = this.GetPullRequestReviewCommentsAsync(p_owner, p_repository, prNumber).Result,
                Comments = this.GetPullRequestCommentsAsync(p_owner, p_repository).Result
            };

            return pull;
        }

        private List<Pull> GetPullsCommentedByReviewer(List<Pull> p_pulls, string p_reviewer)
        {
            List<Pull> pullsCommentedByReviewer = new List<Pull>();

            pullsCommentedByReviewer.AddRange(p_pulls
                   .Where(p =>
                       p.Comments.Any(c => c.Username == p_reviewer) ||
                       p.Reviews.Any(r => r.Username == p_reviewer) ||
                       p.Issues.Any(r => r.Username == p_reviewer)));

            return pullsCommentedByReviewer;
        }

        private ReviewerUser AddMissingRepositoriesFromPullList(ReviewerUser p_reviewerUser, List<Pull> p_pulls)
        {
            p_pulls.ForEach(p =>
            {
                if (p_reviewerUser.Repositories.Any(r => r.RepositoryName == p.Repository) == false)
                {
                    p_reviewerUser.Repositories.Add(new RepositoryReview(p.Repository));
                }
            });

            return p_reviewerUser;
        }

        private List<RepositoryReview> GetOnlyUniqueCommentsReviewsAndIssuesFromRepositories(List<RepositoryReview> p_repositories)
        {
            p_repositories.ForEach(r =>
            {
                r.Comments = r.Comments.GroupBy(c => c.Position).Select(g => g.First()).ToList();
                r.Reviews = r.Reviews.GroupBy(r => r.HTML_Url).Select(g => g.First()).ToList();
                r.Issues = r.Issues.GroupBy(i => i.HTML_Url).Select(g => g.First()).ToList();
            });

            return p_repositories;
        }

        public List<RepositoryReview> AddCommentsIssuesAndReviewsFromPullsIntoRepositories(List<RepositoryReview> p_repositories, List<Pull> p_pulls, string p_reviewer)
        {
            p_repositories.ForEach(r =>
            {
                p_pulls
                    .Where(p => p.Repository == r.RepositoryName)
                    .Select(p => p.Comments
                        .Where(c => c.Username == p_reviewer))
                    .ToList()
                    .ForEach(l => r.Comments.AddRange(l));

                p_pulls
                    .Where(p => p.Repository == r.RepositoryName)
                    .Select(p => p.Reviews
                        .Where(rev => rev.Username == p_reviewer))
                    .ToList()
                    .ForEach(l => r.Reviews.AddRange(l));

                p_pulls
                    .Where(p => p.Repository == r.RepositoryName)
                    .Select(p => p.Issues
                        .Where(i => i.Username == p_reviewer))
                    .ToList()
                    .ForEach(l => r.Issues.AddRange(l));
            });

            return p_repositories;
        }

        public List<Repository> GetRepositoriesOfStudentsForAssignment(List<Repository> p_repositories, List<Student> students, string assignmentName)
        {
            List<Repository> repositories = new List<Repository>();

            foreach (Repository repository in p_repositories)
            {
                string[] splitRepository = repository.Name.Split('-');

                if (splitRepository[0] == assignmentName)
                {
                    foreach (Student student in students)
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

        #endregion
    }
}
