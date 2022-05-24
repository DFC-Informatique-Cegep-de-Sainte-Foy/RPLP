using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.DAL.DTO.Json
{
    public class RepositoryReview
    {
        public string RepositoryName { get; set; }
        public List<CodeComment> Comments { get; set; }
        public List<Review> Reviews { get; set; }
        public List<Issue> Issues { get; set; }

        public RepositoryReview(string repositoryName)
        {
            this.RepositoryName = repositoryName;
            this.Comments = new List<CodeComment>();
            this.Reviews = new List<Review>();
            this.Issues = new List<Issue>();
        }
    }
}
