using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using RPLP.API.Controllers;
using RPLP.DAL.DTO.Sql;
using RPLP.DAL.SQL;
using RPLP.DAL.SQL.Depots;
using RPLP.ENTITES;
using RPLP.SERVICES.InterfacesDepots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RPLP.UnitTesting.APITests
{
    public class TestsAPIAdministratorController
    {
        [Fact]
        public void Test_Get()
        {
            var mockDepotAdministrator = new Mock<IDepotAdministrator>();
            List<Administrator> adminsInMock = new List<Administrator>()
            {
                new Administrator
                {
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Token = "token"
                },
                new Administrator
                {
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Token = "token"
                }
            };

            mockDepotAdministrator.Setup(m => m.GetAdministrators()).Returns(adminsInMock);
                
            AdministratorController controller = new AdministratorController(mockDepotAdministrator.Object);
            
            var response = controller.Get();

            var result = Assert.IsType<OkObjectResult>(response.Result);
            mockDepotAdministrator.Verify(m => m.GetAdministrators(), Times.Once());
            Assert.NotNull(result);

            List<Administrator> administrators = result.Value as List<Administrator>;

            Assert.Equal(2, administrators.Count);
        }

        [Fact]
        public void Test_GetAdministratorById()
        {
            var mockDepotAdministrator = new Mock<IDepotAdministrator>();
            Administrator adminInMock = new Administrator
            {
                Id = 1,
                Username = "ThPaquet",
                FirstName = "Thierry",
                LastName = "Paquet",
                Token = "token"
            };

            mockDepotAdministrator.Setup(m => m.GetAdministratorById(1)).Returns(adminInMock);

            AdministratorController controller = new AdministratorController(mockDepotAdministrator.Object);

            var response = controller.GetAdministratorById(1);

            var result = Assert.IsType<OkObjectResult>(response.Result);
            

            Administrator administrator = result.Value as Administrator;

            mockDepotAdministrator.Verify(m => m.GetAdministratorById(1), Times.Once());
            Assert.NotNull(result);
            Assert.Equal("ThPaquet", administrator.Username);
        }

        [Fact]
        public void Test_GetAdministratorByName()
        {
            var mockDepotAdministrator = new Mock<IDepotAdministrator>();
            Administrator adminInMock = new Administrator
            {
                Id = 1,
                Username = "ThPaquet",
                FirstName = "Thierry",
                LastName = "Paquet",
                Token = "token"
            };

            mockDepotAdministrator.Setup(m => m.GetAdministratorByUsername("ThPaquet")).Returns(adminInMock);

            AdministratorController controller = new AdministratorController(mockDepotAdministrator.Object);

            var response = controller.GetAdministratorByUsername("ThPaquet");

            var result = Assert.IsType<OkObjectResult>(response.Result);

            Administrator administrator = result.Value as Administrator;

            mockDepotAdministrator.Verify(m => m.GetAdministratorByUsername("ThPaquet"), Times.Once());
            Assert.NotNull(result);
            Assert.Equal("ThPaquet", administrator.Username);
        }

        [Fact]
        public void Test_GetAdminOrganisations()
        {
            var mockDepotAdministrator = new Mock<IDepotAdministrator>();
            List<Organisation> organisationsInMock = new List<Organisation>()
            {
                new Organisation()
                {
                    Id = 1, 
                    Name = "CEGEP Ste-Foy"
                },

                new Organisation
                {
                    Id = 2,
                    Name = "RPLP"
                }
            };

            mockDepotAdministrator.Setup(m => m.GetAdminOrganisations("ThPaquet")).Returns(organisationsInMock);

            AdministratorController controller = new AdministratorController(mockDepotAdministrator.Object);

            var response = controller.GetAdminOrganisations("ThPaquet");

            var result = Assert.IsType<OkObjectResult>(response.Result);

            List<Organisation> organisations = result.Value as List<Organisation>;

            mockDepotAdministrator.Verify(m => m.GetAdminOrganisations("ThPaquet"), Times.Once());
            Assert.NotNull(result);
            Assert.Equal(2, organisations.Count);
        }

        [Fact]
        public void Test_AddAdminToOrganisation_AdminNullOrWhiteSpace()
        {
            var mockDepotAdministrator = new Mock<IDepotAdministrator>();
            AdministratorController controller = new AdministratorController(mockDepotAdministrator.Object);

            var response = controller.AddAdminToOrganisation("", "RPLP");

            var result = Assert.IsType<BadRequestResult>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_AddAdminToOrganisation_OrganisationNullOrWhiteSpace()
        {
            var mockDepotAdministrator = new Mock<IDepotAdministrator>();
            AdministratorController controller = new AdministratorController(mockDepotAdministrator.Object);

            var response = controller.AddAdminToOrganisation("ThPaquet", "");

            var result = Assert.IsType<BadRequestResult>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_AddAdminToOrganisation_Created()
        {
            var mockDepotAdministrator = new Mock<IDepotAdministrator>();
            AdministratorController controller = new AdministratorController(mockDepotAdministrator.Object);

            var response = controller.AddAdminToOrganisation("ThPaquet", "RPLP");

            var result = Assert.IsType<CreatedResult>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_RemoveAdminFromOrganisation_AdminNullOrWhiteSpace()
        {
            var mockDepotAdministrator = new Mock<IDepotAdministrator>();
            AdministratorController controller = new AdministratorController(mockDepotAdministrator.Object);

            var response = controller.RemoveAdminFromOrganisation("", "RPLP");

            var result = Assert.IsType<BadRequestResult>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_RemoveAdminFromOrganisation_OrganisationNullOrWhiteSpace()
        {
            var mockDepotAdministrator = new Mock<IDepotAdministrator>();
            AdministratorController controller = new AdministratorController(mockDepotAdministrator.Object);

            var response = controller.RemoveAdminFromOrganisation("ThPaquet", "");

            var result = Assert.IsType<BadRequestResult>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_RemoveAdminFromOrganisation_NoContent()
        {
            var mockDepotAdministrator = new Mock<IDepotAdministrator>();
            AdministratorController controller = new AdministratorController(mockDepotAdministrator.Object);

            var response = controller.RemoveAdminFromOrganisation("ThPaquet", "RPLP");

            var result = Assert.IsType<NoContentResult>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_UpsertAdmin_AdminNull()
        {
            var mockDepotAdministrator = new Mock<IDepotAdministrator>();
            AdministratorController controller = new AdministratorController(mockDepotAdministrator.Object);

            var response = controller.UpsertAdmin(null);

            var result = Assert.IsType<BadRequestResult>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_UpsertAdmin_NotValidModelState()
        {
            var mockDepotAdministrator = new Mock<IDepotAdministrator>();
            AdministratorController controller = new AdministratorController(mockDepotAdministrator.Object);

            Administrator administrator = new Administrator()
            {
                Id = 1,
                Username = "ThPaquet"
            };

            controller.ModelState.AddModelError("", "Invalid model state mock");

            var response = controller.UpsertAdmin(administrator);

            var result = Assert.IsType<BadRequestResult>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_UpsertAdmin_Created()
        {
            var mockDepotAdministrator = new Mock<IDepotAdministrator>();
            AdministratorController controller = new AdministratorController(mockDepotAdministrator.Object);

            Administrator administrator = new Administrator()
            {
                Id = 1,
                Username = "ThPaquet"
            };

            var response = controller.UpsertAdmin(administrator);

            var result = Assert.IsType<CreatedResult>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_DeleteAdmin_NoContent()
        {
            var mockDepotAdministrator = new Mock<IDepotAdministrator>();
            AdministratorController controller = new AdministratorController(mockDepotAdministrator.Object);

            var response = controller.DeleteAdmin("ThPaquet");

            var result = Assert.IsType<NoContentResult>(response);
            Assert.NotNull(result);
        }
    }
}
