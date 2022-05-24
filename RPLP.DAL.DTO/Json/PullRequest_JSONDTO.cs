using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.DAL.DTO.Json
{
    public class PullRequest_JSONDTO
    {
        public int number { get; set; }
        public string state { get; set; }
        public string title { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public List<User_JSONDTO> requested_reviewers { get; set; }
    }
}
