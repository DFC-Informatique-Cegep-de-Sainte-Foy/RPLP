using Microsoft.AspNetCore.Mvc;
using Moq;
using RPLP.API.Controllers;
using RPLP.ENTITES;
using RPLP.ENTITES.InterfacesDepots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RPLP.UnitTesting.APITests
{
    public class TestsAPIOrganisationController
    {
        [Fact]
        public void Test_Get()
        {
            Mock<IDepotOrganisation> depot = new Mock<IDepotOrganisation>();
            OrganisationController controller = new OrganisationController(depot.Object);

            List<Organisation> organisationsInMockDepot = new List<Organisation>()
            {
                new Organisation()
                {
                    Id = 1,
                    Name = "CEGEP Ste-Foy"
                },
                new Organisation()
                {
                    Id = 2,
                    Name = "RPLP"
                }
            };

            depot.Setup(d => d.GetOrganisations()).Returns(organisationsInMockDepot);

            var response = controller.Get();
            var result = Assert.IsType<OkObjectResult>(response.Result);
            List<Organisation> organisations = result.Value as List<Organisation>;

            depot.Verify(d => d.GetOrganisations(), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(2, organisations.Count);
            Assert.Contains(organisations, o => o.Name == "RPLP");
        }

        [Fact]
        public void Test_GetOrganisationById()
        {
            Mock<IDepotOrganisation> depot = new Mock<IDepotOrganisation>();
            OrganisationController controller = new OrganisationController(depot.Object);

            Organisation organisationInMockDepot = new Organisation()
            {
                Id = 1,
                Name = "CEGEP Ste-Foy"
            };

            depot.Setup(d => d.GetOrganisationById(1)).Returns(organisationInMockDepot);

            var response = controller.GetOrganisationById(1);
            var result = Assert.IsType<OkObjectResult>(response.Result);
            Organisation organisation = result.Value as Organisation;

            depot.Verify(d => d.GetOrganisationById(1), Times.Once);
            Assert.NotNull(result);
            Assert.Equal("CEGEP Ste-Foy", organisation.Name);
        }

        [Fact]
        public void Test_GetOrganisationByName()
        {
            Mock<IDepotOrganisation> depot = new Mock<IDepotOrganisation>();
            OrganisationController controller = new OrganisationController(depot.Object);

            Organisation organisationInMockDepot = new Organisation()
            {
                Id = 1,
                Name = "CEGEP Ste-Foy"
            };

            depot.Setup(d => d.GetOrganisationByName("CEGEP Ste-Foy")).Returns(organisationInMockDepot);

            var response = controller.GetOrganisationByName("CEGEP Ste-Foy");
            var result = Assert.IsType<OkObjectResult>(response.Result);
            Organisation organisation = result.Value as Organisation;

            depot.Verify(d => d.GetOrganisationByName("CEGEP Ste-Foy"), Times.Once);
            Assert.NotNull(result);
            Assert.Equal("CEGEP Ste-Foy", organisation.Name);
        }

        [Fact]
        public void Test_GetAdministratorsByOrganisation()
        {
            Mock<IDepotOrganisation> depot = new Mock<IDepotOrganisation>();
            OrganisationController controller = new OrganisationController(depot.Object);

            List<Administrator> administratorsInMockDepot = new List<Administrator>()
            {
                new Administrator()
                {
                    Id = 1,
                    Username = "ThPaquet"
                },
                new Administrator()
                {
                    Id = 2,
                    Username = "BACenComm"
                }
            };

            depot.Setup(d => d.GetAdministratorsByOrganisation("CEGEP Ste-Foy")).Returns(administratorsInMockDepot);

            var response = controller.GetAdministratorsByOrganisation("CEGEP Ste-Foy");
            var result = Assert.IsType<OkObjectResult>(response.Result);
            List<Administrator> administrators = result.Value as List<Administrator>;

            depot.Verify(d => d.GetAdministratorsByOrganisation("CEGEP Ste-Foy"), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(2, administrators.Count);
            Assert.Contains(administrators, a => a.Username == "ThPaquet");
        }

        [Fact]
        public void Test_AddAdministratorToOrganisation_OrganisationIsWhiteSpace()
        {
            Mock<IDepotOrganisation> depot = new Mock<IDepotOrganisation>();
            OrganisationController controller = new OrganisationController(depot.Object);

            var response = controller.AddAdministratorToOrganisation("", "RPLP");
            var result = Assert.IsType<BadRequestResult>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_AddAdministratorToOrganisation_AdministratorIsWhiteSpace()
        {
            Mock<IDepotOrganisation> depot = new Mock<IDepotOrganisation>();
            OrganisationController controller = new OrganisationController(depot.Object);

            var response = controller.AddAdministratorToOrganisation("ThPaquet", "");
            var result = Assert.IsType<BadRequestResult>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_AddAdministratorToOrganisation_Created()
        {
            Mock<IDepotOrganisation> depot = new Mock<IDepotOrganisation>();
            OrganisationController controller = new OrganisationController(depot.Object);

            var response = controller.AddAdministratorToOrganisation("ThPaquet", "RPLP");
            
            var result = Assert.IsType<CreatedResult>(response);
            depot.Verify(d => d.AddAdministratorToOrganisation("ThPaquet", "RPLP"), Times.Once);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_RemoveAdministratorFromOrganisation_OrganisationIsWhiteSpace()
        {
            Mock<IDepotOrganisation> depot = new Mock<IDepotOrganisation>();
            OrganisationController controller = new OrganisationController(depot.Object);

            var response = controller.RemoveAdministratorToOrganisation("", "RPLP");
            var result = Assert.IsType<BadRequestResult>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_RemoveAdministratorFromOrganisation_AdministratorIsWhiteSpace()
        {
            Mock<IDepotOrganisation> depot = new Mock<IDepotOrganisation>();
            OrganisationController controller = new OrganisationController(depot.Object);

            var response = controller.RemoveAdministratorToOrganisation("ThPaquet", "");
            var result = Assert.IsType<BadRequestResult>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_RemoveAdministratorFromOrganisation_NoContent()
        {
            Mock<IDepotOrganisation> depot = new Mock<IDepotOrganisation>();
            OrganisationController controller = new OrganisationController(depot.Object);

            var response = controller.RemoveAdministratorToOrganisation("ThPaquet", "RPLP");

            var result = Assert.IsType<NoContentResult>(response);
            depot.Verify(d => d.RemoveAdministratorFromOrganisation("ThPaquet", "RPLP"), Times.Once);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_Post_Null()
        {
            Mock<IDepotOrganisation> depot = new Mock<IDepotOrganisation>();
            OrganisationController controller = new OrganisationController(depot.Object);

            var response = controller.Post(null);

            var result = Assert.IsType<BadRequestResult>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_Post_ModelStateIsNotValid()
        {
            Mock<IDepotOrganisation> depot = new Mock<IDepotOrganisation>();
            OrganisationController controller = new OrganisationController(depot.Object);
            controller.ModelState.AddModelError("", "Mock ModelState not valid");
            var response = controller.Post(new Organisation());

            var result = Assert.IsType<BadRequestResult>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_Post_Created()
        {
            Mock<IDepotOrganisation> depot = new Mock<IDepotOrganisation>();
            OrganisationController controller = new OrganisationController(depot.Object);
            Organisation organisation = new Organisation();

            var response = controller.Post(organisation);

            depot.Verify(d => d.UpsertOrganisation(organisation), Times.Once);
            var result = Assert.IsType<CreatedResult>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_DeleteOrganisation()
        {
            Mock<IDepotOrganisation> depot = new Mock<IDepotOrganisation>();
            OrganisationController controller = new OrganisationController(depot.Object);

            var response = controller.DeleteOrganisation("RPLP");

            depot.Verify(d => d.DeleteOrganisation("RPLP"), Times.Once);
            var result = Assert.IsType<NoContentResult>(response);
            Assert.NotNull(result);
        }
    }
}
