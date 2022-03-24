using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.DAL.DTO.Json
{
    public class PullRequest_JSONRequest
    {
        [JsonProperty("head")]
        public string fromBranch { get; set; }

        [JsonProperty("base")]
        public string targetBranch { get; set; }
        public string title { get; set; }
        public string body { get; set; }
    }
}
