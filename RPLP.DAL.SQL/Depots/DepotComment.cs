using RPLP.DAL.DTO.Sql;
using RPLP.ENTITES;
using RPLP.SERVICES.InterfacesDepots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.DAL.SQL.Depots
{
    public class DepotComment : IDepotComment
    {
        private readonly RPLPDbContext _context;

        public DepotComment()
        {
            this._context = new RPLPDbContext();
        }

        public Comment GetCommentById(int p_id)
        {
            Comment comment = this._context.Comments.Where(comment => comment.Id == p_id).Select(comment => comment.ToEntity()).FirstOrDefault();

            if (comment == null)
                return new Comment();

            return comment;
        }

        public List<Comment> GetComments()
        {
            return this._context.Comments.Select(comment => comment.ToEntity()).ToList();
        }

        public void UpsertComment(Comment p_comment)
        {
            Comment_SQLDTO commentResult = this._context.Comments.Where(comment => comment.Id == p_comment.Id).FirstOrDefault();

            if (commentResult != null)
            {
                commentResult.RepositoryName = p_comment.RepositoryName;
                commentResult.Diff_Hunk = p_comment.Diff_Hunk;
                commentResult.Path = p_comment.Path;
                commentResult.WrittenBy = p_comment.WrittenBy;
                commentResult.Original_Position = p_comment.Original_Position;
                commentResult.In_Reply_To_Id = p_comment.In_Reply_To_Id;
                commentResult.Created_at = p_comment.Created_at;
                commentResult.Updated_at = p_comment.Updated_at;

                this._context.Update(commentResult);
                this._context.SaveChanges();
            }
            else
            {
                Comment_SQLDTO comment = new Comment_SQLDTO();
                comment.RepositoryName = p_comment.RepositoryName;
                comment.Diff_Hunk = p_comment.Diff_Hunk;
                comment.Path = p_comment.Path;
                comment.WrittenBy = p_comment.WrittenBy;
                comment.Original_Position = p_comment.Original_Position;
                comment.In_Reply_To_Id = p_comment.In_Reply_To_Id;
                comment.Created_at = p_comment.Created_at;
                comment.Updated_at = p_comment.Updated_at;

                this._context.Comments.Add(comment);
                this._context.SaveChanges();
            }
        }
    }
}
