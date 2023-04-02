using Microsoft.EntityFrameworkCore;
using RPLP.DAL.DTO.Sql;
using RPLP.DAL.SQL;
using RPLP.DAL.SQL.Depots;
using RPLP.ENTITES;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RPLP.UnitTesting.DepotTests
{
    //[Collection("DatabaseTests")]
    //public class TestsDepotComment
    //{
    //    private static readonly DbContextOptions<RPLPDbContext> options = new DbContextOptionsBuilder<RPLPDbContext>()
    //            .UseSqlServer("Server=localhost,1434; Database=RPLP; User Id=sa; password=Cad3pend86!")
    //            //.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
    //            .Options;

    //    private void DeleteCommentsTableContent()
    //    {
    //        using (var context = new RPLPDbContext(options))
    //        {
    //            context.Database.ExecuteSqlRaw("DELETE from Comments;");
    //        }
    //    }

    //    private void InsertComment(Classroom_SQLDTO p_classroom)
    //    {
    //        using (var context = new RPLPDbContext(options))
    //        {
    //            context.Classrooms.Add(p_classroom);
    //            context.SaveChanges();
    //        }
    //    }

    //    private void InsertPremadeComment()
    //    {
    //        Comment_SQLDTO comment = new Comment_SQLDTO()
    //        {
    //            Active = true,
    //            Diff_Hunk = "hunk",
    //            In_Reply_To_Id = 1,
    //            Original_Position = 2,
    //            Path = "path",
    //            Updated_at = System.DateTime.Now,
    //            Created_at = System.DateTime.Now,
    //            Body = "This is a comment",
    //            RepositoryName = "RPLP",
    //            WrittenBy = "ThPaquet"
    //        };

    //        using (var context = new RPLPDbContext(options))
    //        {
    //            context.Comments.Add(comment);
    //            context.SaveChanges();
    //        }
    //    }

    //    private void InsertMultiplePremadeComments()
    //    {
    //        List<Comment_SQLDTO> comments = new List<Comment_SQLDTO>()
    //        {
    //            new Comment_SQLDTO()
    //            {
    //                Active = true,
    //                Created_at = System.DateTime.Now,
    //                Body = "This is a comment",
    //                RepositoryName = "RPLP",
    //                WrittenBy = "ThPaquet"
    //            },
    //            new Comment_SQLDTO()
    //            {
    //                Active = true,
    //                Created_at = System.DateTime.Now,
    //                Body = "This is another comment",
    //                RepositoryName = "RPLP",
    //                WrittenBy = "ikeameatbol"
    //            },
    //            new Comment_SQLDTO()
    //            {
    //                Active = false,
    //                Created_at = System.DateTime.Now,
    //                Body = "You should not see this",
    //                RepositoryName = "RPLP",
    //                WrittenBy = "BACenComm"
    //            }
    //        };

    //        using (var context = new RPLPDbContext(options))
    //        {
    //            context.Comments.AddRange(comments);
    //            context.SaveChanges();
    //        }
    //    }

    //    [Fact]
    //    public void Test_GetComments()
    //    {
    //        this.DeleteCommentsTableContent();
    //        this.InsertMultiplePremadeComments();

    //        using (var context = new RPLPDbContext(options))
    //        {
    //            DepotComment depot = new DepotComment(context);
    //            List<Comment> comments = depot.GetComments();

    //            Assert.Equal(2, comments.Count);
    //            Assert.NotNull(comments.SingleOrDefault(c => c.Body == "This is a comment" && c.WrittenBy == "ThPaquet"));
    //            Assert.NotNull(comments.SingleOrDefault(c => c.Body == "This is another comment" && c.WrittenBy == "ikeameatbol"));
    //            Assert.Null(comments.SingleOrDefault(c => c.Body == "You should not see this" && c.WrittenBy == "BACenComm"));
    //        }

    //        this.DeleteCommentsTableContent();
    //    }

    //    [Fact]
    //    public void Test_GetCommentById()
    //    {
    //        this.DeleteCommentsTableContent();
    //        this.InsertMultiplePremadeComments();

    //        using (var context = new RPLPDbContext(options))
    //        {
    //            DepotComment depot = new DepotComment(context);

    //            Comment_SQLDTO commentFromContextThPaquet = context.Comments.SingleOrDefault(c => c.WrittenBy == "ThPaquet");
    //            Comment_SQLDTO commentFromContextBACenComm = context.Comments.SingleOrDefault(c => c.WrittenBy == "BACenComm");

    //            Assert.NotNull(commentFromContextThPaquet);
    //            Assert.NotNull(commentFromContextBACenComm);

    //            int idCommentThPaquet = commentFromContextThPaquet.Id;
    //            int idCommentBACenComm = commentFromContextBACenComm.Id;

    //            Comment? commentThPaquet = depot.GetCommentById(idCommentThPaquet);
    //            Comment? commentBACenComm = depot.GetCommentById(idCommentBACenComm);

    //            Assert.NotNull(commentThPaquet);
    //            Assert.Null(commentBACenComm.Body);
    //            Assert.Null(commentBACenComm.In_Reply_To_Id);
    //        }

    //        this.DeleteCommentsTableContent();
    //    }

    //    [Fact]
    //    public void Test_UpsertComment_Inserts()
    //    {
    //        this.DeleteCommentsTableContent();

    //        Comment_SQLDTO commentPreInsert = new Comment_SQLDTO()
    //        {
    //            Active = true,
    //            Diff_Hunk = "hunk",
    //            In_Reply_To_Id = 1,
    //            Original_Position = 2,
    //            Path = "path",
    //            Updated_at = System.DateTime.Now,
    //            Created_at = System.DateTime.Now,
    //            Body = "This is a comment",
    //            RepositoryName = "RPLP",
    //            WrittenBy = "ThPaquet"
    //        };

    //        using (var context = new RPLPDbContext(options))
    //        {
    //            DepotComment depot = new DepotComment(context);

    //            Comment_SQLDTO? comment = context.Comments.FirstOrDefault(c => c.WrittenBy == "ThPaquet");
    //            Assert.Null(comment);

    //            depot.UpsertComment(commentPreInsert.ToEntity());
    //        }

    //        using (var context = new RPLPDbContext(options))
    //        {
    //            Comment_SQLDTO? comment = context.Comments.FirstOrDefault(c => c.WrittenBy == "ThPaquet");
    //            Assert.NotNull(comment);
    //            Assert.Equal(commentPreInsert.Diff_Hunk, comment.Diff_Hunk);
    //            Assert.Equal(commentPreInsert.In_Reply_To_Id, comment.In_Reply_To_Id);
    //            Assert.Equal(commentPreInsert.Original_Position, comment.Original_Position);
    //            Assert.Equal(commentPreInsert.Path, comment.Path);
    //            Assert.Equal(commentPreInsert.Created_at, comment.Created_at);
    //            Assert.Equal(commentPreInsert.Updated_at, comment.Updated_at);
    //            Assert.Equal(commentPreInsert.Body, comment.Body);
    //            Assert.Equal(commentPreInsert.RepositoryName, comment.RepositoryName);
    //            Assert.Equal(commentPreInsert.WrittenBy, comment.WrittenBy);

    //        }

    //        this.DeleteCommentsTableContent();
    //    }

    //    [Fact]
    //    public void Test_UpsertComment_Updates()
    //    {
    //        this.DeleteCommentsTableContent();
    //        this.InsertPremadeComment();

    //        Comment_SQLDTO? commentPreUpsert = new Comment_SQLDTO();

    //        using (var context = new RPLPDbContext(options))
    //        {
    //            DepotComment depot = new DepotComment(context);

    //            Comment_SQLDTO? comment = context.Comments.FirstOrDefault(c => c.WrittenBy == "ThPaquet");
    //            Assert.NotNull(comment);

    //            commentPreUpsert.Diff_Hunk = comment.Diff_Hunk;
    //            commentPreUpsert.In_Reply_To_Id = comment.In_Reply_To_Id;
    //            commentPreUpsert.Original_Position = comment.Original_Position;
    //            commentPreUpsert.Path = comment.Path;
    //            commentPreUpsert.Updated_at = comment.Updated_at;
    //            commentPreUpsert.Created_at = comment.Created_at;
    //            commentPreUpsert.Body = comment.Body;
    //            commentPreUpsert.RepositoryName = comment.RepositoryName;
    //            commentPreUpsert.WrittenBy = comment.WrittenBy;

    //            comment.Diff_Hunk = "diff";
    //            comment.In_Reply_To_Id = 2;
    //            comment.Original_Position = 3;
    //            comment.Path = "anotherpath";
    //            comment.Updated_at = System.DateTime.Now;
    //            comment.Created_at = System.DateTime.Now;
    //            comment.Body = "This is a different comment";
    //            comment.RepositoryName = "ProjetSynthese";
    //            comment.WrittenBy = "ikeameatbol";

    //            depot.UpsertComment(comment.ToEntity());
    //        }

    //        using (var context = new RPLPDbContext(options))
    //        {
    //            Comment_SQLDTO? preUpdatedComment = context.Comments.SingleOrDefault(c => c.WrittenBy == "ThPaquet");
    //            Comment_SQLDTO? comment = context.Comments.SingleOrDefault(c => c.WrittenBy == "ikeameatbol" && c.Path == "anotherpath");

    //            Assert.Null(preUpdatedComment);
    //            Assert.NotNull(comment);
    //            Assert.NotEqual(commentPreUpsert.Diff_Hunk, comment.Diff_Hunk);
    //            Assert.NotEqual(commentPreUpsert.In_Reply_To_Id, comment.In_Reply_To_Id);
    //            Assert.NotEqual(commentPreUpsert.Original_Position, comment.Original_Position);
    //            Assert.NotEqual(commentPreUpsert.Path, comment.Path);
    //            Assert.NotEqual(commentPreUpsert.Created_at, comment.Created_at);
    //            Assert.NotEqual(commentPreUpsert.Updated_at, comment.Updated_at);
    //            Assert.NotEqual(commentPreUpsert.Body, comment.Body);
    //            Assert.NotEqual(commentPreUpsert.RepositoryName, comment.RepositoryName);
    //            Assert.NotEqual(commentPreUpsert.WrittenBy, comment.WrittenBy);
    //        }

    //        this.DeleteCommentsTableContent();
    //    }

    //    [Fact]
    //    public void Test_DeleteComment()
    //    {
    //        this.DeleteCommentsTableContent();
    //        this.InsertPremadeComment();

    //        using (var context = new RPLPDbContext(options))
    //        {
    //            Comment_SQLDTO? comment = context.Comments.SingleOrDefault(c => c.WrittenBy == "ThPaquet" && c.Active);
    //            Assert.NotNull(comment);

    //            int commentId = comment.Id;

    //            DepotComment depot = new DepotComment(context);

    //            depot.DeleteComment(commentId);
    //        }

    //        using (var context = new RPLPDbContext(options))
    //        {
    //            Comment_SQLDTO? comment = context.Comments.SingleOrDefault(c => c.WrittenBy == "ThPaquet" && c.Active == true);
    //            Assert.Null(comment);
    //        }

    //        this.DeleteCommentsTableContent();
    //    }
    //}
}
