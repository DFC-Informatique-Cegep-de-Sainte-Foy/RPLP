using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.DAL.DTO.Json
{
    public class Pull
    {
        public string Username { get; set; }
        public int Number { get; set; }
        public string HTML_Url { get; set; }
        public string Repository { get; set; }
        public List<Issue> Issues { get; set; }
        public List<Review> Reviews { get; set; }
        public List<CodeComment> Comments { get; set; }

        public Pull(string p_owner, string p_repository)
        {
            this.Username = p_owner;
            this.Repository = p_repository;
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
