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

    public class TestsDepotOrganisations
    {
        [Fact]
        public void Test_GetOrganisations()
        {
            List<Administrator_SQLDTO> administratorsDB = new List<Administrator_SQLDTO>()
            {
                new Administrator_SQLDTO()
                {
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Token = "token",
                    Active = true
                },
                new Administrator_SQLDTO()
                {
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Token = "token",
                    Active = true
                },
                new Administrator_SQLDTO()
                {
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Token = "token",
                    Active = false
                }
            };

            List<Organisation_SQLDTO> organisationsDB = new List<Organisation_SQLDTO>()
            {
                new Organisation_SQLDTO()
                {
                    Name = "CEGEP Ste-Foy",
                    Administrators = administratorsDB,
                    Active = true
                },
                new Organisation_SQLDTO()
                {
                    Name = "College Edouard-Montpetit",
                    Administrators = administratorsDB,
                    Active = true
                },
                new Organisation_SQLDTO()
                {
                    Name = "Universite Laval",
                    Administrators = administratorsDB,
                    Active = false
                },
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsDB);
            context.Setup(x => x.Organisations).ReturnsDbSet(organisationsDB);
            DepotOrganisation depot = new DepotOrganisation(context.Object);

            List<Organisation> organisations = depot.GetOrganisations();

            Assert.NotNull(organisations);
            Assert.Equal(2, organisations.Count);
            Assert.NotNull(organisations.FirstOrDefault(o => o.Name == "CEGEP Ste-Foy"));
            Assert.Null(organisations.FirstOrDefault(o => o.Name == "Universite Laval"));
            Assert.Equal(3, organisations.FirstOrDefault(o => o.Name == "CEGEP Ste-Foy").Administrators.Count);
           
            
        }

        [Fact]
        public void Test_GetOrganisationById()
        {
            List<Administrator_SQLDTO> administratorsDB = new List<Administrator_SQLDTO>()
            {
                new Administrator_SQLDTO()
                {
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Token = "token",
                    Active = true
                },
                new Administrator_SQLDTO()
                {
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Token = "token",
                    Active = true
                },
                new Administrator_SQLDTO()
                {
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Token = "token",
                    Active = false
                }
            };

            List<Organisation_SQLDTO> organisationsDB = new List<Organisation_SQLDTO>()
            {
                new Organisation_SQLDTO()
                {
                    Name = "CEGEP Ste-Foy",
                    Administrators = administratorsDB,
                    Active = true
                },
                new Organisation_SQLDTO()
                {
                    Name = "College Edouard-Montpetit",
                    Administrators = administratorsDB,
                    Active = true
                },
                new Organisation_SQLDTO()
                {
                    Name = "Universite Laval",
                    Administrators = administratorsDB,
                    Active = false
                },
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsDB);
            context.Setup(x => x.Organisations).ReturnsDbSet(organisationsDB);
            DepotOrganisation depot = new DepotOrganisation(context.Object);

            int organisationId = organisationsDB.FirstOrDefault(o => o.Name == "CEGEP Ste-Foy").Id;

            Organisation organisation = depot.GetOrganisationById(organisationId);

            Assert.NotNull(organisation);
            Assert.Equal(3, organisation.Administrators.Count);
           
            
        }

        [Fact]
        public void Test_GetOrganisationByName()
        {
            List<Administrator_SQLDTO> administratorsDB = new List<Administrator_SQLDTO>()
            {
                new Administrator_SQLDTO()
                {
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Token = "token",
                    Active = true
                },
                new Administrator_SQLDTO()
                {
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Token = "token",
                    Active = true
                },
                new Administrator_SQLDTO()
                {
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Token = "token",
                    Active = false
                }
            };

            List<Organisation_SQLDTO> organisationsDB = new List<Organisation_SQLDTO>()
            {
                new Organisation_SQLDTO()
                {
                    Name = "CEGEP Ste-Foy",
                    Administrators = administratorsDB,
                    Active = true
                },
                new Organisation_SQLDTO()
                {
                    Name = "College Edouard-Montpetit",
                    Administrators = administratorsDB,
                    Active = true
                },
                new Organisation_SQLDTO()
                {
                    Name = "Universite Laval",
                    Administrators = administratorsDB,
                    Active = false
                },
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsDB);
            context.Setup(x => x.Organisations).ReturnsDbSet(organisationsDB);
            DepotOrganisation depot = new DepotOrganisation(context.Object);

            Organisation organisation = depot.GetOrganisationByName("CEGEP Ste-Foy");

            Assert.NotNull(organisation);
            Assert.Equal(3, organisation.Administrators.Count);                   
        }

        [Fact]
        public void Test_GetAdministratorsByOrganisation()
        {
            List<Administrator_SQLDTO> administratorsDB = new List<Administrator_SQLDTO>()
            {
                new Administrator_SQLDTO()
                {
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Token = "token",
                    Active = true
                },
                new Administrator_SQLDTO()
                {
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Token = "token",
                    Active = true
                },
                new Administrator_SQLDTO()
                {
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Token = "token",
                    Active = false
                }
            };

            List<Organisation_SQLDTO> organisationsDB = new List<Organisation_SQLDTO>()
            {
                new Organisation_SQLDTO()
                {
                    Name = "CEGEP Ste-Foy",
                    Administrators = administratorsDB,
                    Active = true
                },
                new Organisation_SQLDTO()
                {
                    Name = "College Edouard-Montpetit",
                    Administrators = administratorsDB,
                    Active = true
                },
                new Organisation_SQLDTO()
                {
                    Name = "Universite Laval",
                    Administrators = administratorsDB,
                    Active = false
                },
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsDB);
            context.Setup(x => x.Organisations).ReturnsDbSet(organisationsDB);
            DepotOrganisation depot = new DepotOrganisation(context.Object);

            List<Administrator> administrators = depot.GetAdministratorsByOrganisation("CEGEP Ste-Foy");

            Assert.NotNull(administrators);
            Assert.Equal(3, administrators.Count);
           
            
        }

        [Fact]
        public void Test_AddAdministratorToOrganisation()
        {
            List<Administrator_SQLDTO> administratorsDB = new List<Administrator_SQLDTO>()
            {
                new Administrator_SQLDTO()
                {
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Token = "token",
                    Active = true
                },
                new Administrator_SQLDTO()
                {
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Token = "token",
                    Active = true
                },
                new Administrator_SQLDTO()
                {
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Token = "token",
                    Active = false
                }
            };

            List<Organisation_SQLDTO> organisationsDB = new List<Organisation_SQLDTO>()
            {
                new Organisation_SQLDTO()
                {
                    Name = "CEGEP Ste-Foy",
                    Administrators = new List<Administrator_SQLDTO>()
            {
                new Administrator_SQLDTO()
                {
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Token = "token",
                    Active = true
                },
                new Administrator_SQLDTO()
                {
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Token = "token",
                    Active = true
                },
                new Administrator_SQLDTO()
                {
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Token = "token",
                    Active = false
                }
            },
                    Active = true
                },
                new Organisation_SQLDTO()
                {
                    Name = "College Edouard-Montpetit",
                    Administrators = administratorsDB,
                    Active = true
                },
                new Organisation_SQLDTO()
                {
                    Name = "Universite Laval",
                    Administrators = administratorsDB,
                    Active = false
                },
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsDB);
            context.Setup(x => x.Organisations).ReturnsDbSet(organisationsDB);
            context.Setup(m => m.Organisations.Add(It.IsAny<Organisation_SQLDTO>())).Callback<Organisation_SQLDTO>(organisationsDB.Add);
            DepotOrganisation depot = new DepotOrganisation(context.Object);

            Administrator_SQLDTO newAdministrator = new Administrator_SQLDTO()
            {
                Username = "PiFou86",
                FirstName = "Pierre-Francois",
                LastName = "Leon",
                Token = "token",
                Email = "email",
                Active = true
            };

            administratorsDB.Add(newAdministrator);

            Administrator_SQLDTO? administratorInContext = administratorsDB.FirstOrDefault(a => a.Username == "PiFou86");
            Assert.NotNull(administratorInContext);

            Organisation_SQLDTO? organisation = organisationsDB.FirstOrDefault(o => o.Name == "CEGEP Ste-Foy");

            Administrator_SQLDTO? administratorInOrganisation = organisation.Administrators.FirstOrDefault(a => a.Username == "PiFou86");

            Assert.Null(administratorInOrganisation);

            depot.AddAdministratorToOrganisation(organisation.Name, administratorInContext.Username);


            administratorInContext = administratorsDB.FirstOrDefault(a => a.Username == "PiFou86");
            organisation = administratorInContext.Organisations.FirstOrDefault(a => a.Name == "CEGEP Ste-Foy");

            Assert.NotNull(administratorInContext);
            Assert.NotNull(organisation);                
        }

        [Fact]
        public void Test_RemoveAdministratorFromOrganisation()
        {
            List<Administrator_SQLDTO> administratorsDB = new List<Administrator_SQLDTO>()
            {
                new Administrator_SQLDTO()
                {
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Token = "token",
                    Active = true
                },
                new Administrator_SQLDTO()
                {
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Token = "token",
                    Active = true
                },
                new Administrator_SQLDTO()
                {
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Token = "token",
                    Active = false
                }
            };

            List<Organisation_SQLDTO> organisationsDB = new List<Organisation_SQLDTO>()
            {
                new Organisation_SQLDTO()
                {
                    Name = "CEGEP Ste-Foy",
                    Administrators = administratorsDB,
                    Active = true
                },
                new Organisation_SQLDTO()
                {
                    Name = "College Edouard-Montpetit",
                    Administrators = administratorsDB,
                    Active = true
                },
                new Organisation_SQLDTO()
                {
                    Name = "Universite Laval",
                    Administrators = administratorsDB,
                    Active = false
                },
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsDB);
            context.Setup(x => x.Organisations).ReturnsDbSet(organisationsDB);
            DepotOrganisation depot = new DepotOrganisation(context.Object);

            string administratorUserName = "ThPaquet";

            Administrator_SQLDTO? administratorInContext = administratorsDB.FirstOrDefault(a => a.Username == administratorUserName);
            Assert.NotNull(administratorInContext);

            Organisation_SQLDTO? organisation = organisationsDB.FirstOrDefault(o => o.Name == "CEGEP Ste-Foy");
            Administrator_SQLDTO? administratorInOrganisation = organisation.Administrators.FirstOrDefault(a => a.Username == administratorUserName);

            Assert.NotNull(administratorInOrganisation);

            depot.RemoveAdministratorFromOrganisation(organisation.Name, administratorInContext.Username);

            administratorInContext = administratorsDB.FirstOrDefault(a => a.Username == administratorUserName);
            organisation = administratorInContext.Organisations.FirstOrDefault(a => a.Name == "CEGEP Ste-Foy");

            Assert.NotNull(administratorInContext);
            Assert.Null(organisation);           
        }

        [Fact]
        public void Test_UpsertOrganisation_Inserts()
        {
            List<Administrator_SQLDTO> administratorsDB = new List<Administrator_SQLDTO>()
            {
                new Administrator_SQLDTO()
                {
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Token = "token",
                    Active = true
                },
                new Administrator_SQLDTO()
                {
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Token = "token",
                    Active = true
                },
                new Administrator_SQLDTO()
                {
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Token = "token",
                    Active = false
                }
            };

            List<Organisation_SQLDTO> organisationsDB = new List<Organisation_SQLDTO>()
            {
                new Organisation_SQLDTO()
                {
                    Name = "CEGEP Ste-Foy",
                    Administrators = administratorsDB,
                    Active = true
                },
                new Organisation_SQLDTO()
                {
                    Name = "College Edouard-Montpetit",
                    Administrators = administratorsDB,
                    Active = true
                },
                new Organisation_SQLDTO()
                {
                    Name = "Universite Laval",
                    Administrators = administratorsDB,
                    Active = false
                },
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsDB);
            context.Setup(x => x.Organisations).ReturnsDbSet(organisationsDB);
            context.Setup(m => m.Organisations.Add(It.IsAny<Organisation_SQLDTO>())).Callback<Organisation_SQLDTO>(organisationsDB.Add);
            DepotOrganisation depot = new DepotOrganisation(context.Object);

            string organisationName = "RPLP";
            string administratorUserName = "ThPaquet";
            string administratorFirstName = "Thierry";
            string administratorLastName = "Paquet";
            string administratorEmail = "email";
            string administratorToken = "token";

            Organisation_SQLDTO? organisation = organisationsDB.FirstOrDefault(o => o.Name == "RPLP");
            Assert.Null(organisation);

            Organisation newOrganisation = new Organisation()
            {
                Name = organisationName,
                Administrators = new List<Administrator>()
                        {
                            new Administrator()
                            {
                                Username = administratorUserName,
                                FirstName = administratorFirstName,
                                LastName = administratorLastName,
                                Email = administratorEmail,
                                Token = administratorToken
                            }
                        }
            };

            depot.UpsertOrganisation(newOrganisation);

            organisation = organisationsDB.FirstOrDefault(o => o.Name == "RPLP");

            Assert.NotNull(organisation);
            Assert.Equal(1, organisation.Administrators.Count);
            Assert.Equal(organisationName, organisation.Name);
            Assert.NotNull(organisation.Administrators.FirstOrDefault(a => a.Username == administratorUserName));
            Assert.NotNull(organisation.Administrators.FirstOrDefault(a => a.FirstName == administratorFirstName));
            Assert.NotNull(organisation.Administrators.FirstOrDefault(a => a.LastName == administratorLastName));
            Assert.NotNull(organisation.Administrators.FirstOrDefault(a => a.Token == administratorToken));
           
            
        }

        [Fact]
        public void Test_UpsertOrganisation_Updates()
        {
            List<Administrator_SQLDTO> administratorsDB = new List<Administrator_SQLDTO>()
            {
                new Administrator_SQLDTO()
                {
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Token = "token",
                    Active = true
                },
                new Administrator_SQLDTO()
                {
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Token = "token",
                    Active = true
                },
                new Administrator_SQLDTO()
                {
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Token = "token",
                    Active = false
                }
            };

            List<Organisation_SQLDTO> organisationsDB = new List<Organisation_SQLDTO>()
            {
                new Organisation_SQLDTO()
                {
                    Name = "CEGEP Ste-Foy",
                    Administrators = administratorsDB,
                    Active = true
                },
                new Organisation_SQLDTO()
                {
                    Name = "College Edouard-Montpetit",
                    Administrators = administratorsDB,
                    Active = true
                },
                new Organisation_SQLDTO()
                {
                    Name = "Universite Laval",
                    Administrators = administratorsDB,
                    Active = false
                },
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsDB);
            context.Setup(x => x.Organisations).ReturnsDbSet(organisationsDB);
            DepotOrganisation depot = new DepotOrganisation(context.Object);

            string name = "testOrg";

            Organisation_SQLDTO? organisation = organisationsDB.FirstOrDefault(o => o.Name == "CEGEP Ste-Foy");
            Assert.NotNull(organisation);

            organisation.Name = name;

            depot.UpsertOrganisation(organisation.ToEntity());

            organisation = organisationsDB.FirstOrDefault(o => o.Name == "RPLP");
            Assert.Null(organisation);

            organisation = organisationsDB.FirstOrDefault(o => o.Name == name);
            Assert.NotNull(organisation);

            Assert.Equal(3, organisation.Administrators.Count);
            Assert.Equal(name, organisation.Name);
           
            
        }

        [Fact]
        public void Test_DeleteOrganisation()
        {
            List<Administrator_SQLDTO> administratorsDB = new List<Administrator_SQLDTO>()
            {
                new Administrator_SQLDTO()
                {
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Token = "token",
                    Active = true
                },
                new Administrator_SQLDTO()
                {
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Token = "token",
                    Active = true
                },
                new Administrator_SQLDTO()
                {
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Token = "token",
                    Active = false
                }
            };

            List<Organisation_SQLDTO> organisationsDB = new List<Organisation_SQLDTO>()
            {
                new Organisation_SQLDTO()
                {
                    Name = "CEGEP Ste-Foy",
                    Administrators = administratorsDB,
                    Active = true
                },
                new Organisation_SQLDTO()
                {
                    Name = "College Edouard-Montpetit",
                    Administrators = administratorsDB,
                    Active = true
                },
                new Organisation_SQLDTO()
                {
                    Name = "Universite Laval",
                    Administrators = administratorsDB,
                    Active = false
                },
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsDB);
            context.Setup(x => x.Organisations).ReturnsDbSet(organisationsDB);
            DepotOrganisation depot = new DepotOrganisation(context.Object);

            Organisation_SQLDTO? organisation = organisationsDB.FirstOrDefault(o => o.Name == "CEGEP Ste-Foy");
            Assert.NotNull(organisation);

            depot.DeleteOrganisation("CEGEP Ste-Foy");

            organisation = organisationsDB.FirstOrDefault(o => o.Name == "CEGEP Ste-Foy" && o.Active == true);
            Assert.Null(organisation);
           
            
        }
    }
}

