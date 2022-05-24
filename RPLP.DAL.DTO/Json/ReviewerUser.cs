using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.DAL.DTO.Json
{
    public class ReviewerUser
    {
        public string Username { get; set; }
        public List<RepositoryReview> Repositories { get; set; }

        public ReviewerUser()
        {
            this.Repositories = new List<RepositoryReview>();
        }
    }
}
