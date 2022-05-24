using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.DAL.DTO.Json
{
    public class Commit_JSONDTO
    {
        public string sha { get; set; }
        public CommitInfo_JSONDTO commit { get; set; }
    }
}
