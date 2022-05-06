using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.ENTITES
{
    public class Comment
    {
        public int Id { get; set; }
        public string WrittenBy { get; set; }
        public string RepositoryName { get; set; }
        public string? Diff_Hunk { get; set; }
        public string? Path { get; set; }
        public int Position { get; set; }
        public int? Original_Position { get; set; }
        public string Body { get; set; }
        public int? In_Reply_To_Id { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }
        public string PullRequestURL { get; set; }

        public Comment()
        {

        }

        public Comment(int p_id, string p_writtenBy, string p_repositoryName, string p_body, DateTime p_created_at, DateTime p_updated_by)
        {
            this.Id = p_id;
            this.WrittenBy = p_writtenBy;
            this.RepositoryName = p_repositoryName;
            this.Body = p_body;
            this.Created_at = p_created_at;
            this.Updated_at = p_updated_by;
        }

        public Comment(int p_id, string p_writtenBy, string p_repositoryName, string? p_diff_hunk, string? p_path, int? p_original_position, int? p_in_reply_to_id, string p_body, DateTime p_created_at, DateTime p_updated_by)
        {
            this.Id = p_id;
            this.WrittenBy = p_writtenBy;
            this.RepositoryName = p_repositoryName;
            this.Diff_Hunk = p_diff_hunk;
            this.Path = p_path;
            this.Original_Position = p_original_position;
            this.Body = p_body;
            this.In_Reply_To_Id = p_in_reply_to_id;
            this.Created_at = p_created_at;
            this.Updated_at = p_updated_by;
        }

        public override string ToString()
        {
            return $"{{\n" +
                $"  \"WrittenBy\" : \"{this.WrittenBy}\"\n" +
                $"  \"Body\" : \"{this.Body}\"\n" +
                $"  \"Diff_Hunk\" : \"{this.Diff_Hunk}\"\n" +
                $"  \"Path\" : \"{this.Path}\"\n" +
                $"  \"Position\" : {this.Position}\n" +
                $"  \"PullRequestURL\" : \"{this.PullRequestURL}\"\n" +
                $"}}";
        }
    }
}
