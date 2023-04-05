using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using RPLP.DAL.DTO.Sql;
using RPLP.DAL.SQL;
using RPLP.DAL.SQL.Depots;
using RPLP.ENTITES;
using RPLP.JOURNALISATION;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RPLP.UnitTesting.DepotTests
{
    public class TestsDepotComment
    {
        [Fact]
        public void Test_GetComments()
        {
            List<Comment_SQLDTO> commentsDB = new List<Comment_SQLDTO>()
            {
                new Comment_SQLDTO()
                {
                    Id= 1,
                    Active = true,
                    Created_at = System.DateTime.Now,
                    Body = "This is a comment",
                    RepositoryName = "RPLP",
                    WrittenBy = "ThPaquet"
                },
                new Comment_SQLDTO()
                {
                    Id= 2,
                    Active = true,
                    Created_at = System.DateTime.Now,
                    Body = "This is another comment",
                    RepositoryName = "RPLP",
                    WrittenBy = "ikeameatbol"
                },
                new Comment_SQLDTO()
                {
                    Id= 3,
                    Active = false,
                    Created_at = System.DateTime.Now,
                    Body = "You should not see this",
                    RepositoryName = "RPLP",
                    WrittenBy = "BACenComm"
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Comments).ReturnsDbSet(commentsDB);
            DepotComment depot = new DepotComment(context.Object);

            List<Comment> comments = depot.GetComments();

            Assert.Equal(2, comments.Count);
            Assert.NotNull(comments.SingleOrDefault(c => c.Body == "This is a comment" && c.WrittenBy == "ThPaquet"));
            Assert.NotNull(comments.SingleOrDefault(c => c.Body == "This is another comment" && c.WrittenBy == "ikeameatbol"));
            Assert.Null(comments.SingleOrDefault(c => c.Body == "You should not see this" && c.WrittenBy == "BACenComm"));
           
            
        }

        [Fact]
        public void Test_GetCommentById()
        {
            List<Comment_SQLDTO> commentsDB = new List<Comment_SQLDTO>()
            {
                new Comment_SQLDTO()
                {
                    Id= 1,
                    Active = true,
                    Created_at = System.DateTime.Now,
                    Body = "This is a comment",
                    RepositoryName = "RPLP",
                    WrittenBy = "ThPaquet"
                },
                new Comment_SQLDTO()
                {
                    Id= 2,
                    Active = true,
                    Created_at = System.DateTime.Now,
                    Body = "This is another comment",
                    RepositoryName = "RPLP",
                    WrittenBy = "ikeameatbol"
                },
                new Comment_SQLDTO()
                {
                    Id= 3,
                    Active = false,
                    Created_at = System.DateTime.Now,
                    Body = "You should not see this",
                    RepositoryName = "RPLP",
                    WrittenBy = "BACenComm"
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Comments).ReturnsDbSet(commentsDB);
            DepotComment depot = new DepotComment(context.Object);

            Comment_SQLDTO commentFromContextThPaquet = commentsDB.SingleOrDefault(c => c.WrittenBy == "ThPaquet");
            Comment_SQLDTO commentFromContextBACenComm = commentsDB.SingleOrDefault(c => c.WrittenBy == "BACenComm");

            Assert.NotNull(commentFromContextThPaquet);
            Assert.NotNull(commentFromContextBACenComm);

            int idCommentThPaquet = commentFromContextThPaquet.Id;
            int idCommentBACenComm = commentFromContextBACenComm.Id;

            Comment? commentThPaquet = depot.GetCommentById(idCommentThPaquet);
            Comment? commentBACenComm = depot.GetCommentById(idCommentBACenComm);

            Assert.NotNull(commentThPaquet);
            Assert.Null(commentBACenComm.Body);
            Assert.Null(commentBACenComm.In_Reply_To_Id);
            
            
        }

        [Fact]
        public void Test_UpsertComment_Inserts()
        {
            List<Comment_SQLDTO> commentsDB = new List<Comment_SQLDTO>()
            {
                new Comment_SQLDTO()
                {
                    Id= 1,
                    Active = true,
                    Created_at = System.DateTime.Now,
                    Body = "This is another comment",
                    RepositoryName = "RPLP",
                    WrittenBy = "ikeameatbol"
                },
                new Comment_SQLDTO()
                {
                    Id= 2,
                    Active = false,
                    Created_at = System.DateTime.Now,
                    Body = "You should not see this",
                    RepositoryName = "RPLP",
                    WrittenBy = "BACenComm"
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Comments).ReturnsDbSet(commentsDB);
            context.Setup(m => m.Comments.Add(It.IsAny<Comment_SQLDTO>())).Callback<Comment_SQLDTO>(commentsDB.Add);
            DepotComment depot = new DepotComment(context.Object);

            Comment_SQLDTO commentPreInsert = new Comment_SQLDTO()
            {
                Active = true,
                Diff_Hunk = "hunk",
                In_Reply_To_Id = 1,
                Original_Position = 2,
                Path = "path",
                Updated_at = System.DateTime.Now,
                Created_at = System.DateTime.Now,
                Body = "This is a comment",
                RepositoryName = "RPLP",
                WrittenBy = "ThPaquet"
            };
            Comment_SQLDTO? comment = commentsDB.FirstOrDefault(c => c.WrittenBy == "ThPaquet");
            Assert.Null(comment);

            depot.UpsertComment(commentPreInsert.ToEntity());

            comment = commentsDB.FirstOrDefault(c => c.WrittenBy == "ThPaquet");
            Assert.NotNull(comment);

            Assert.Equal(commentPreInsert.Diff_Hunk, comment.Diff_Hunk);
            Assert.Equal(commentPreInsert.In_Reply_To_Id, comment.In_Reply_To_Id);
            Assert.Equal(commentPreInsert.Original_Position, comment.Original_Position);
            Assert.Equal(commentPreInsert.Path, comment.Path);
            Assert.Equal(commentPreInsert.Created_at, comment.Created_at);
            Assert.Equal(commentPreInsert.Updated_at, comment.Updated_at);
            Assert.Equal(commentPreInsert.Body, comment.Body);
            Assert.Equal(commentPreInsert.RepositoryName, comment.RepositoryName);
            Assert.Equal(commentPreInsert.WrittenBy, comment.WrittenBy);
           
            
        }

        [Fact]
        public void Test_UpsertComment_Updates()
        {
            List<Comment_SQLDTO> commentsDB = new List<Comment_SQLDTO>()
            {
                new Comment_SQLDTO()
                {
                    Id= 1,
                    Active = true,
                    Created_at = System.DateTime.Now,
                    Body = "This is a comment",
                    RepositoryName = "RPLP",
                    WrittenBy = "ThPaquet"
                },
                new Comment_SQLDTO()
                {
                    Id= 2,
                    Active = true,
                    Created_at = System.DateTime.Now,
                    Body = "This is another comment",
                    RepositoryName = "RPLP",
                    WrittenBy = "ikeameatbol"
                },
                new Comment_SQLDTO()
                {
                    Id= 3,
                    Active = false,
                    Created_at = System.DateTime.Now,
                    Body = "You should not see this",
                    RepositoryName = "RPLP",
                    WrittenBy = "BACenComm"
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Comments).ReturnsDbSet(commentsDB);
            DepotComment depot = new DepotComment(context.Object);

            Comment_SQLDTO? commentPreUpsert = new Comment_SQLDTO();

            Comment_SQLDTO? comment = commentsDB.FirstOrDefault(c => c.WrittenBy == "ThPaquet");
            Assert.NotNull(comment);

            commentPreUpsert.Diff_Hunk = comment.Diff_Hunk;
            commentPreUpsert.In_Reply_To_Id = comment.In_Reply_To_Id;
            commentPreUpsert.Original_Position = comment.Original_Position;
            commentPreUpsert.Path = comment.Path;
            commentPreUpsert.Updated_at = comment.Updated_at;
            commentPreUpsert.Created_at = comment.Created_at;
            commentPreUpsert.Body = comment.Body;
            commentPreUpsert.RepositoryName = comment.RepositoryName;
            commentPreUpsert.WrittenBy = comment.WrittenBy;

            comment.Diff_Hunk = "diff";
            comment.In_Reply_To_Id = 2;
            comment.Original_Position = 3;
            comment.Path = "anotherpath";
            comment.Updated_at = System.DateTime.Now;
            comment.Created_at = System.DateTime.Now;
            comment.Body = "This is a different comment";
            comment.RepositoryName = "ProjetSynthese";
            comment.WrittenBy = "ikeameatbol";

            depot.UpsertComment(comment.ToEntity());

            Comment_SQLDTO? preUpdatedComment = commentsDB.SingleOrDefault(c => c.WrittenBy == "ThPaquet");
            comment = commentsDB.SingleOrDefault(c => c.WrittenBy == "ikeameatbol" && c.Path == "anotherpath");

            Assert.Null(preUpdatedComment);
            Assert.NotNull(comment);
            Assert.NotEqual(commentPreUpsert.Diff_Hunk, comment.Diff_Hunk);
            Assert.NotEqual(commentPreUpsert.In_Reply_To_Id, comment.In_Reply_To_Id);
            Assert.NotEqual(commentPreUpsert.Original_Position, comment.Original_Position);
            Assert.NotEqual(commentPreUpsert.Path, comment.Path);
            Assert.NotEqual(commentPreUpsert.Created_at, comment.Created_at);
            Assert.NotEqual(commentPreUpsert.Updated_at, comment.Updated_at);
            Assert.NotEqual(commentPreUpsert.Body, comment.Body);
            Assert.NotEqual(commentPreUpsert.RepositoryName, comment.RepositoryName);
            Assert.NotEqual(commentPreUpsert.WrittenBy, comment.WrittenBy);
           
            
        }

        [Fact]
        public void Test_DeleteComment()
        {
            List<Comment_SQLDTO> commentsDB = new List<Comment_SQLDTO>()
            {
                new Comment_SQLDTO()
                {
                    Id= 1,
                    Active = true,
                    Created_at = System.DateTime.Now,
                    Body = "This is a comment",
                    RepositoryName = "RPLP",
                    WrittenBy = "ThPaquet"
                },
                new Comment_SQLDTO()
                {
                    Id= 2,
                    Active = true,
                    Created_at = System.DateTime.Now,
                    Body = "This is another comment",
                    RepositoryName = "RPLP",
                    WrittenBy = "ikeameatbol"
                },
                new Comment_SQLDTO()
                {
                    Id= 3,
                    Active = false,
                    Created_at = System.DateTime.Now,
                    Body = "You should not see this",
                    RepositoryName = "RPLP",
                    WrittenBy = "BACenComm"
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Comments).ReturnsDbSet(commentsDB);
            DepotComment depot = new DepotComment(context.Object);

            Comment_SQLDTO? comment = commentsDB.SingleOrDefault(c => c.WrittenBy == "ThPaquet" && c.Active);
            Assert.NotNull(comment);

            int commentId = comment.Id;

            depot.DeleteComment(commentId);

            comment = commentsDB.SingleOrDefault(c => c.WrittenBy == "ThPaquet" && c.Active == true);
            Assert.Null(comment);
           
            
        }
    }
}
