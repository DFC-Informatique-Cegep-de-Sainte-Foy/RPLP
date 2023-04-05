using Microsoft.AspNetCore.Mvc;
using Moq;
using RPLP.API.Controllers;
using RPLP.DAL.SQL.Depots;
using RPLP.ENTITES;
using RPLP.ENTITES.InterfacesDepots;
using RPLP.JOURNALISATION;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RPLP.UnitTesting.APITests
{
    public class TestsAPICommentController
    {
        [Fact]
        public void Test_Get()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotComment> depot = new Mock<IDepotComment>();
            CommentController controller = new CommentController(depot.Object);

            List<Comment> commentsInMockDepot = new List<Comment>()
            {
                new Comment()
                {
                    Id = 1,
                    Body = "Ready"
                },
                new Comment()
                {
                    Id = 2,
                    Body = "Not Ready"
                }
            };

            depot.Setup(d => d.GetComments()).Returns(commentsInMockDepot);

            var response = controller.Get();
            var result = Assert.IsType<OkObjectResult>(response.Result);
            List<Comment> comments = result.Value as List<Comment>;

            depot.Verify(d => d.GetComments(), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(2, comments.Count);
            Assert.Contains(comments, c => c.Body == "Ready");
        }

        [Fact]
        public void Test_GetCommentById()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotComment> depot = new Mock<IDepotComment>();
            CommentController controller = new CommentController(depot.Object);

            Comment commentInMockDepot = new Comment()
            {
                Id = 1,
                Body = "Ready"
            };

            depot.Setup(d => d.GetCommentById(1)).Returns(commentInMockDepot);

            var response = controller.GetCommentById(1);
            var result = Assert.IsType<OkObjectResult>(response.Result);
            Comment comment = result.Value as Comment;

            depot.Verify(d => d.GetCommentById(1), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(comment.Body, "Ready");
        }

        [Fact]
        public void Test_Post_NullComment()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotComment> depot = new Mock<IDepotComment>();
            CommentController controller = new CommentController(depot.Object);

            var response = controller.Post(null);
            var result = Assert.IsType<BadRequestResult>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_Post_Created()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotComment> depot = new Mock<IDepotComment>();
            CommentController controller = new CommentController(depot.Object);
            Comment comment = new Comment();

            var response = controller.Post(comment);
            var result = Assert.IsType<CreatedResult>(response);

            depot.Verify(d => d.UpsertComment(comment), Times.Once);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_Delete()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotComment> depot = new Mock<IDepotComment>();
            CommentController controller = new CommentController(depot.Object);

            var response = controller.DeleteComment(1);
            var result = Assert.IsType<NoContentResult>(response);

            depot.Verify(d => d.DeleteComment(1), Times.Once);
            Assert.NotNull(result);
        }
    }
}
