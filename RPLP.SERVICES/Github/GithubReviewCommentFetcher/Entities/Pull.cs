using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.SERVICES.Github.GithubReviewCommentFetcher.Entities
{
    public class Pull
    {
        public string Username { get; set; }
        public int Number { get; set; }
        public string HTML_Url { get; set; }
        public List<Issue> Issues { get; set; }
        public List<Review> Reviews { get; set; }
        public List<Comment> Comments { get; set; }

        public Pull(string p_owner, string p_repository, int p_number)
        {
            GithubPRCommentFetcher fetcher = new GithubPRCommentFetcher();

            this.Issues = fetcher.GetPullRequestIssueAsync(p_owner, p_repository).Result;
            this.Reviews = fetcher.GetPullRequestReviewCommentsAsync(p_owner, p_repository, p_number).Result;
            this.Comments = fetcher.GetPullRequestCommentsAsync(p_owner, p_repository).Result;
        }

        public override string ToString()
        {
            return $"{{\n" +
                $"  \"Username\" : \"{this.Username}\"\n" +
                $"  \"Number\" : {this.Number}\n" +
                $"  \"HTML_Url\" : \"{this.HTML_Url}\"\n" +
                $"}}";
        }
    }
}
