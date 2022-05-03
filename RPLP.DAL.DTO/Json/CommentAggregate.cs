using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.DAL.DTO.Json
{
    public class CommentAggregate
    {
        public string Username { get; set; }
        public List<Issue> Issues { get; set; }
        public List<Review> Reviews { get; set; }
        public List<CodeComment> Comments { get; set; }
    }
}
