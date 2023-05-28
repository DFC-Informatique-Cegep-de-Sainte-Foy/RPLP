using Microsoft.AspNetCore.Mvc;
using Moq;
using RPLP.API.Controllers;
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
    public class TestsAPIRepositoryController
    {
        [Fact]
        public void Test_GetRepositoryById()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotRepository> depot = new Mock<IDepotRepository>();
            RepositoryController controller = new RepositoryController(depot.Object);

            Repository repositoryInMockDepot = new Repository()
            {
                Id = 1,
                Name = "ThPaquet"
            };

            depot.Setup(d => d.GetRepositoryById(1)).Returns(repositoryInMockDepot);

            var response = controller.GetRepositoryById(1);

            var result = Assert.IsType<OkObjectResult>(response.Result);
            Repository repository = result.Value as Repository;

            depot.Verify(d => d.GetRepositoryById(1), Times.Once);
            Assert.NotNull(result);
            Assert.Equal("ThPaquet", repository.Name);
        }

        [Fact]
        public void Test_GetRepositoryByName()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotRepository> depot = new Mock<IDepotRepository>();
            RepositoryController controller = new RepositoryController(depot.Object);

            Repository repositoryInMockDepot = new Repository()
            {
                Id = 1,
                Name = "ThPaquet"
            };

            depot.Setup(d => d.GetRepositoryByName("ThPaquet")).Returns(repositoryInMockDepot);

            var response = controller.GetRepositoryByName("ThPaquet");

            var result = Assert.IsType<OkObjectResult>(response.Result);
            Repository repository = result.Value as Repository;

            depot.Verify(d => d.GetRepositoryByName("ThPaquet"), Times.Once);
            Assert.NotNull(result);
            Assert.Equal("ThPaquet", repository.Name);
        }

        [Fact]
        public void Test_Post_RepositoryNull()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotRepository> depot = new Mock<IDepotRepository>();
            RepositoryController controller = new RepositoryController(depot.Object);

            var response = controller.Post(null);

            var result = Assert.IsType<BadRequestResult>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_Post_ModelStateNotValid()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotRepository> depot = new Mock<IDepotRepository>();
            RepositoryController controller = new RepositoryController(depot.Object);
            controller.ModelState.AddModelError("", "Mock Modelstate Not Valid");

            var response = controller.Post(new Repository());

            var result = Assert.IsType<BadRequestResult>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_Post_NoContent()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotRepository> depot = new Mock<IDepotRepository>();
            RepositoryController controller = new RepositoryController(depot.Object);
            Repository repository = new Repository();

            var response = controller.Post(repository);

            var result = Assert.IsType<CreatedResult>(response);
            depot.Verify(d => d.UpsertRepository(repository));
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_DeleteRepository()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotRepository> depot = new Mock<IDepotRepository>();
            RepositoryController controller = new RepositoryController(depot.Object);

            var response = controller.DeleteRepository("ThPaquet");

            var result = Assert.IsType<NoContentResult>(response);
            depot.Verify(d => d.DeleteRepositoryParRepoName("ThPaquet"));
            Assert.NotNull(result);
        }
    }
}
