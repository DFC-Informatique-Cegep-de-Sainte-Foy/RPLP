using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.DAL.DTO.Json
{
    public class Comment_JSONDTO
    {
        public int id { get; set; }
        public string repositoryName { get; set; }
        public string? diff_hunk { get; set; }
        public string? path { get; set; }
        public int? original_position { get; set; }
        public User_JSONDTO user { get; set; }
        public string body { get; set; }
        public int? in_reply_to_id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}
