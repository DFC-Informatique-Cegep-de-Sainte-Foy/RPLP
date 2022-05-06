using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.DAL.DTO.Json
{
    public class Issue
    {
        public string Username { get; set; }
        public string Body { get; set; }
        public string HTML_Url { get; set; }

        public override string ToString()
        {
            return $"{{\n" +
               $"  \"Username\" : \"{this.Username}\"\n" +
               $"  \"Body\" : \"{this.Body}\"\n" +
               $"  \"HTML_Url\" : \"{this.HTML_Url}\"\n" +
               $"}}";
        }
    }
}
