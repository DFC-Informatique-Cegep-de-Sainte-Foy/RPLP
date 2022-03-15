using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.ENTITES
{
    internal class Comment
    {
        public int id { get; set; }
        public string writtenBy { get; set; }
        public string repositoryName { get; set; }
        public string? diff_hunk { get; set; }
        public string? path { get; set; }
        public int? original_position { get; set; }
        public string body { get; set; }
        public int? in_reply_to_id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }

        public Comment()
        {

        }

        public Comment(int p_id, string p_writtenBy, string p_repositoryName, string p_body, DateTime p_created_at, DateTime p_updated_by)
        {
            this.id = p_id;
            this.writtenBy = p_writtenBy;
            this.repositoryName = p_repositoryName;
            this.body = p_body;
            this.created_at = p_created_at;
            this.updated_at = p_updated_by;
        }

        public Comment(int p_id, string p_writtenBy, string p_repositoryName, string p_diff_hunk, string p_path, int p_original_position, int p_in_reply_to_id, string p_body, DateTime p_created_at, DateTime p_updated_by)
        {
            this.id = p_id;
            this.writtenBy = p_writtenBy;
            this.repositoryName = p_repositoryName;
            this.diff_hunk = p_diff_hunk;
            this.path = p_path;
            this.original_position = p_original_position;
            this.body = p_body;
            this.in_reply_to_id = p_in_reply_to_id;
            this.created_at = p_created_at;
            this.updated_at = p_updated_by;
        }

    }
}
