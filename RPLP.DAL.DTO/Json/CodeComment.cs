using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.DAL.DTO.Json
{
    public class CodeComment
    {
        public string Username { get; set; }
        public string Body { get; set; }
        public string Diff_Hunk { get; set; }
        public string Path { get; set; }
        public int Position { get; set; }
        public string PullRequestURL { get; set; }

        public override string ToString()
        {
            return $"{{\n" +
                $"  \"Username\" : \"{this.Username}\"\n" +
                $"  \"Body\" : \"{this.Body}\"\n" +
                $"  \"Diff_Hunk\" : \"{this.Diff_Hunk}\"\n" +
                $"  \"Path\" : \"{this.Path}\"\n" +
                $"  \"Position\" : {this.Position}\n" +
                $"  \"PullRequestURL\" : \"{this.PullRequestURL}\"\n" +
                $"}}";
        }
    }
}
