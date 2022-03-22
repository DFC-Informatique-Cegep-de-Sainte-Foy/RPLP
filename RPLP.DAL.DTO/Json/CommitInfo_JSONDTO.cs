using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.DAL.DTO.Json
{
    public class CommitInfo_JSONDTO
    {
        public Author_JSONDTO author { get; set; }
        public string message { get; set; }
        public int comment_count { get; set; }
    }
}

