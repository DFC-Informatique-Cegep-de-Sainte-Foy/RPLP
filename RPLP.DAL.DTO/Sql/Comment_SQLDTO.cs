using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPLP.ENTITES;

namespace RPLP.DAL.DTO.Sql
{
    public class Comment_SQLDTO
    {
        public int Id { get; set; }
        public string WrittenBy { get; set; }
        public string RepositoryName { get; set; }
        public string? Diff_Hunk { get; set; }
        public string? Path { get; set; }
        public int? Original_Position { get; set; }
        public string Body { get; set; }
        public int? In_Reply_To_Id { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }
        public bool Active { get; set; }

        public Comment_SQLDTO()
        {

        }

        public Comment_SQLDTO(Comment p_comment)
        {
            this.Id = p_comment.Id;
            this.WrittenBy = p_comment.WrittenBy;
            this.RepositoryName = p_comment.RepositoryName;
            this.Diff_Hunk = p_comment.Diff_Hunk;
            this.Path = p_comment.Path;
            this.Original_Position = p_comment.Original_Position;
            this.Body = p_comment.Body;
            this.In_Reply_To_Id = p_comment.In_Reply_To_Id;
            this.Created_at = p_comment.Created_at;
            this.Updated_at = p_comment.Updated_at;
            this.Active = true;
        }

        public Comment ToEntity()
        {
            if (this.Diff_Hunk == null || this.Path == null || this.Original_Position == null || this.In_Reply_To_Id == null)
            {
                return new Comment(this.Id, this.WrittenBy, this.RepositoryName, this.Body, this.Created_at, this.Updated_at);
            }

            return new Comment(this.Id, this.WrittenBy, this.RepositoryName, this.Diff_Hunk, this.Path, this.Original_Position, this.In_Reply_To_Id, this.Body, this.Created_at, this.Updated_at);
        }
    }
}
