using RPLP.DAL.DTO.Sql;
using RPLP.ENTITES;
using RPLP.JOURNALISATION;
using RPLP.ENTITES.InterfacesDepots;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.DAL.SQL.Depots
{
    public class DepotComment : IDepotComment
    {
        private readonly RPLPDbContext _context;

        public DepotComment(RPLPDbContext p_context)
        {
            if (p_context == null)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                               "DepotComment - DepotComment(RPLPDbContext p_context) - p_context de type RPLPDbContext passé en paramètre est null", 0));
            }

            this._context = p_context;
        }

        public List<Comment> GetComments()
        {
            RPLP.JOURNALISATION.Logging.Journal(new Log("Comment", $"DepotComment - Method - GetComments() - Return List<Comment>"));

            return this._context.Comments.Where(comment => comment.Active)
                                         .Select(comment => comment.ToEntity()).ToList();
        }

        public Comment GetCommentById(int p_id)
        {
            if (p_id < 0)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "DepotComment - GetCommentById - p_id passé en paramêtre est hors des limites", 0));
            }

            Comment_SQLDTO? comment = this._context.Comments
                                            .FirstOrDefault(comment => comment.Id == p_id && comment.Active);

            if (comment == null)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log("Comment", $"DepotComment - Method - GetComments(int p_id) - Return Comment - comment est null",0));

                return new Comment();
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log("Comment", $"DepotComment - Method - GetComments(int p_id) - Return Comment - comment != null"));
            }

            return comment.ToEntity();
        }

        public void UpsertComment(Comment p_comment)
        {
            if(p_comment == null)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "DepotComment - UpsertComment - p_comment passé en paramêtre est null", 0));
            }

            Comment_SQLDTO commentResult = this._context.Comments.Where(comment => comment.Active)
                                                                 .FirstOrDefault(comment => comment.Id == p_comment.Id);

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
                commentResult.Body = p_comment.Body;

                this._context.Update(commentResult);
                this._context.SaveChanges();

                RPLP.JOURNALISATION.Logging.Journal(new Log("Comment", $"DepotComment - Method - UpsertComment(Comment p_comment) - Void - Update Comment"));
            }
            else
            {
                Comment_SQLDTO comment = new Comment_SQLDTO();
                comment.Body = p_comment.Body;
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

                RPLP.JOURNALISATION.Logging.Journal(new Log("Comment", $"DepotComment - Method - UpsertComment(Comment p_comment) - Void - Add Comment"));
            }

            
        }

        public void DeleteComment(int p_commentId)
        {
            if (p_commentId < 0)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "DepotComment - DeleteComment - p_commentId passé en paramêtre est hors des limites", 0));
            }

            Comment_SQLDTO commentResult = this._context.Comments.SingleOrDefault(comment => comment.Id == p_commentId && comment.Active);
            if (commentResult != null)
            {
                commentResult.Active = false;

                this._context.Update(commentResult);
                this._context.SaveChanges();

                RPLP.JOURNALISATION.Logging.Journal(new Log("Comment", $"DepotComment - Method - DeleteComment(int p_commentId) - Void - delete comment"));
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                      "DepotComment - DeleteComment(int p_commentId) - commentResult est null", 0));
            }
        }
    }
}
