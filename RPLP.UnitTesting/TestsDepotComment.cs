using Microsoft.EntityFrameworkCore;
using RPLP.DAL.DTO.Sql;
using RPLP.DAL.SQL;
using RPLP.DAL.SQL.Depots;
using RPLP.ENTITES;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RPLP.UnitTesting
{
    [Collection ("DatabaseTests")]
    public class TestsDepotComment
    {
        private static readonly DbContextOptions<RPLPDbContext> options = new DbContextOptionsBuilder<RPLPDbContext>()
                .UseSqlServer("Server=localhost,1434; Database=RPLP; User Id=sa; password=Cad3pend86!")
                .Options;

        private void DeleteCommentsTableContent()
        {
            using (var context = new RPLPDbContext(options))
            {
                context.Database.ExecuteSqlRaw("DELETE from Comments;");
            }
        }

        private void InsertComment(Classroom_SQLDTO p_classroom)
        {
            using (var context = new RPLPDbContext(options))
            {
                context.Classrooms.Add(p_classroom);
                context.SaveChanges();
            }
        }

        private void InsertPremadeComment()
        {
            Comment_SQLDTO comment = new Comment_SQLDTO()
            {
                Active = true,
                Created_at = System.DateTime.Now,
                Body = "This is a comment",
                RepositoryName = "RPLP",
                WrittenBy = "ThPaquet"
            };

            using (var context = new RPLPDbContext(options))
            {
                context.Comments.Add(comment);
                context.SaveChanges();
            }
        }

        private void InsertMultiplePremadeComments()
        {
            List<Comment_SQLDTO> comments = new List<Comment_SQLDTO>()
            {
                new Comment_SQLDTO()
                {
                    Active = true,
                    Created_at = System.DateTime.Now,
                    Body = "This is a comment",
                    RepositoryName = "RPLP",
                    WrittenBy = "ThPaquet"
                },
                new Comment_SQLDTO()
                {
                    Active = true,
                    Created_at = System.DateTime.Now,
                    Body = "This is another comment",
                    RepositoryName = "RPLP",
                    WrittenBy = "ikeameatbol"
                },
                new Comment_SQLDTO()
                {
                    Active = false,
                    Created_at = System.DateTime.Now,
                    Body = "You should not see this",
                    RepositoryName = "RPLP",
                    WrittenBy = "BACenComm"
                }
            };

            using (var context = new RPLPDbContext(options))
            {
                context.Comments.AddRange(comments);
                context.SaveChanges();
            }
        }

        [Fact]
        public void Test_GetComments()
        {
            this.DeleteCommentsTableContent();
            this.InsertMultiplePremadeComments();

            using (var context = new RPLPDbContext(options))
            {
                DepotComment depot = new DepotComment(context);
                List<Comment> comments = depot.GetComments();

                Assert.Equal(2, comments.Count);
                Assert.NotNull(comments.SingleOrDefault(c => c.Body == "This is a comment" && c.WrittenBy == "ThPaquet"));
                Assert.NotNull(comments.SingleOrDefault(c => c.Body == "This is another comment" && c.WrittenBy == "ikeameatbol"));
                Assert.Null(comments.SingleOrDefault(c => c.Body == "You should not see this" && c.WrittenBy == "BACenComm"));
            }

            this.DeleteCommentsTableContent();
        }

        [Fact]
        public void Test_GetCommentById()
        {
            this.DeleteCommentsTableContent();
            this.InsertMultiplePremadeComments();

            using (var context = new RPLPDbContext(options))
            {
                DepotComment depot = new DepotComment(context);

                int idCommentThPaquet = context.Comments.SingleOrDefault(c => c.WrittenBy == "ThPaquet").Id;
                int idCommentBACenComm = context.Comments.SingleOrDefault(c => c.WrittenBy == "BACenComm").Id;

                Comment? commentThPaquet = depot.GetCommentById(idCommentThPaquet);
                Comment? commentBACenComm = depot.GetCommentById(idCommentBACenComm);

                Assert.NotNull(commentThPaquet);
                Assert.Null(commentBACenComm);
            }

            this.DeleteCommentsTableContent();
        }

        [Fact]
        public void Test_UpsertComment_Inserts()
        {
            this.DeleteCommentsTableContent();

            using (var context = new RPLPDbContext(options))
            {
                DepotComment depot = new DepotComment(context);

                Comment_SQLDTO? comment = context.Comments.FirstOrDefault(c => c.WrittenBy == "ThPaquet");
                Assert.Null(comment);

                comment = new Comment_SQLDTO()
                {
                    Active = true,
                    Created_at = System.DateTime.Now,
                    Body = "This is a comment",
                    RepositoryName = "RPLP",
                    WrittenBy = "ThPaquet"
                };

                depot.UpsertComment(comment.ToEntity());
            }

            using (var context = new RPLPDbContext(options))
            {
                Comment_SQLDTO? comment = context.Comments.FirstOrDefault(c => c.WrittenBy == "ThPaquet");
                Assert.NotNull(comment);
            }

            this.DeleteCommentsTableContent();
        }

        [Fact]
        public void Test_UpsertComment_Updates()
        {
            this.DeleteCommentsTableContent();
            this.InsertPremadeComment();

            string newBodyContent = "This comment has seen its body change";

            using (var context = new RPLPDbContext(options))
            {
                DepotComment depot = new DepotComment(context);

                Comment_SQLDTO? comment = context.Comments.FirstOrDefault(c => c.WrittenBy == "ThPaquet");
                Assert.NotNull(comment);

                comment.Body = newBodyContent;

                depot.UpsertComment(comment.ToEntity());
            }

            using (var context = new RPLPDbContext(options))
            {
                Comment_SQLDTO? comment = context.Comments.SingleOrDefault(c => c.WrittenBy == "ThPaquet");
                Assert.NotNull(comment);
                Assert.Equal(newBodyContent, comment.Body);
            }

            this.DeleteCommentsTableContent();
        }

        [Fact]
        public void Test_DeleteComment()
        {
            this.DeleteCommentsTableContent();
            this.InsertPremadeComment();

            using (var context = new RPLPDbContext(options))
            {
                Comment_SQLDTO? comment = context.Comments.SingleOrDefault(c => c.WrittenBy == "ThPaquet" && c.Active);
                Assert.NotNull(comment);

                int commentId = comment.Id;

                DepotComment depot = new DepotComment(context);

                depot.DeleteComment(commentId);
            }

            using (var context = new RPLPDbContext(options))
            {
                Comment_SQLDTO? comment = context.Comments.SingleOrDefault(c => c.WrittenBy == "ThPaquet" && c.Active == false);
                Assert.Null(comment);
            }

            this.DeleteCommentsTableContent();
        }
    }
}
