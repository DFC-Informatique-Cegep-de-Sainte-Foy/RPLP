using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.DAL.DTO.Json
{
    public class Branch_JSONDTO
    {
        [JsonProperty("ref")]
        public string reference { get; set; }

        [JsonProperty("object")]
        public Object_JSONDTO gitObject { get; set; }
    }
}
