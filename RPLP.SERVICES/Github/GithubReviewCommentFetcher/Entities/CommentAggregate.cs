using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.SERVICES.Github.GithubReviewCommentFetcher.Entities
{
    public class CommentAggregate
    {
        public string Username { get; set; }
        public List<Issue> Issues { get; set; }
        public List<Review> Reviews { get; set; }
        public List<CodeComment> Comments { get; set; }
    }
}
