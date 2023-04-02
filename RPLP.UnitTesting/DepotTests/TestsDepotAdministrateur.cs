using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq.EntityFrameworkCore;
using Moq;
using RPLP.DAL.DTO.Sql;
using RPLP.DAL.SQL;
using RPLP.DAL.SQL.Depots;
using RPLP.ENTITES;
using RPLP.ENTITES.InterfacesDepots;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using RPLP.JOURNALISATION;
using NSubstitute;
using System.Reflection.Metadata;

namespace RPLP.UnitTesting.DepotTests
{
    public class TestsDepotAdministrateur
    {

        [Fact]
        public void Test_GetAdministrators_CountIsGood()
        {
            List<Administrator_SQLDTO> administratorsBD = new List<Administrator_SQLDTO>
            {
                new Administrator_SQLDTO
                {
                    Id= 1,
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Token = "token",
                    Organisations = {
                    new Organisation_SQLDTO()
                            {
                                Name = "CEGEP Ste-Foy",
                                Active = true
                            }},
                    Active = true
                },
                new Administrator_SQLDTO
                {
                    Id= 2,
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = true
                },
                new Administrator_SQLDTO
                {
                    Id= 3,
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = false
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsBD);
            DepotAdministrator depot = new DepotAdministrator(context.Object);

            List<Administrator> administrators = depot.GetAdministrators();

            Assert.NotNull(administrators);
            Assert.Equal(2, administrators.Count);
            Assert.Contains(administrators, a => a.Username == "ThPaquet");
            Assert.DoesNotContain(administrators, a => a.Username == "BACenComm");
            Assert.NotEmpty(administrators.FirstOrDefault(a => a.Username == "ThPaquet").Organisations);
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Test_GetDeactivatedAdministrators()
        {
            List<Administrator_SQLDTO> administratorsBD = new List<Administrator_SQLDTO>
            {
                new Administrator_SQLDTO
                {
                    Id= 1,
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Token = "token",
                    Organisations = {
                    new Organisation_SQLDTO()
                            {
                                Name = "CEGEP Ste-Foy",
                                Active = true
                            }},
                    Active = true
                },
                new Administrator_SQLDTO
                {
                    Id= 2,
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = true
                },
                new Administrator_SQLDTO
                {
                    Id= 3,
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = false
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsBD);
            DepotAdministrator depot = new DepotAdministrator(context.Object);

            List<Administrator> administrators = depot.GetDeactivatedAdministrators();

            Assert.NotNull(administrators);
            Assert.Equal(1, administrators.Count);
            Assert.Contains(administrators, a => a.Username == "BACenComm");
            Assert.DoesNotContain(administrators, a => a.Username == "ikeameatbol");
            Assert.DoesNotContain(administrators, a => a.Username == "ThPaquet");
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Test_GetByUsername()
        {
            List<Administrator_SQLDTO> administratorsBD = new List<Administrator_SQLDTO>
            {
                new Administrator_SQLDTO
                {
                    Id= 1,
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Token = "token",
                    Organisations = {
                    new Organisation_SQLDTO()
                            {
                                Name = "CEGEP Ste-Foy",
                                Active = true
                            }},
                    Active = true
                },
                new Administrator_SQLDTO
                {
                    Id= 2,
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = true
                },
                new Administrator_SQLDTO
                {
                    Id= 3,
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = false
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsBD);
            DepotAdministrator depot = new DepotAdministrator(context.Object);

            Administrator administrator = depot.GetAdministratorByUsername("ThPaquet");

            Assert.NotNull(administrator);
            Assert.Equal("Thierry", administrator.FirstName);
            Assert.Equal("Paquet", administrator.LastName);
            Assert.Equal("token", administrator.Token);
            Assert.NotEmpty(administrator.Organisations);
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Test_GetByUsername_NotActive()
        {
            List<Administrator_SQLDTO> administratorsBD = new List<Administrator_SQLDTO>
            {
                new Administrator_SQLDTO
                {
                    Id= 1,
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Token = "token",
                    Organisations = {
                    new Organisation_SQLDTO()
                            {
                                Name = "CEGEP Ste-Foy",
                                Active = true
                            }},
                    Active = true
                },
                new Administrator_SQLDTO
                {
                    Id= 2,
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = true
                },
                new Administrator_SQLDTO
                {
                    Id= 3,
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = false
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsBD);
            DepotAdministrator depot = new DepotAdministrator(context.Object);

            Administrator_SQLDTO adminInContext = administratorsBD.FirstOrDefault(a => a.Username == "BACenComm" && a.Active == false);
            Administrator administrator = depot.GetAdministratorByUsername("BACenComm");

            Assert.NotNull(adminInContext);
            Assert.Null(administrator);
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Test_GetById()
        {
            List<Administrator_SQLDTO> administratorsBD = new List<Administrator_SQLDTO>
            {
                new Administrator_SQLDTO
                {
                    Id= 1,
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Token = "token",
                    Organisations = {
                    new Organisation_SQLDTO()
                            {
                                Name = "CEGEP Ste-Foy",
                                Active = true
                            }},
                    Active = true
                },
                new Administrator_SQLDTO
                {
                    Id= 2,
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = true
                },
                new Administrator_SQLDTO
                {
                    Id= 3,
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = false
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsBD);
            DepotAdministrator depot = new DepotAdministrator(context.Object);

            Administrator_SQLDTO? admin = administratorsBD.SingleOrDefault(a => a.Username == "ThPaquet");

            int id = admin.Id;

            Administrator administrator = depot.GetAdministratorById(id);

            Assert.NotNull(administrator);
            Assert.Equal("Thierry", administrator.FirstName);
            Assert.Equal("Paquet", administrator.LastName);
            Assert.Equal("token", administrator.Token);
            Assert.NotEmpty(administrator.Organisations);
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Test_GetById_NotActive()
        {
            List<Administrator_SQLDTO> administratorsBD = new List<Administrator_SQLDTO>
            {
                new Administrator_SQLDTO
                {
                    Id= 1,
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Token = "token",
                    Organisations = {
                    new Organisation_SQLDTO()
                            {
                                Name = "CEGEP Ste-Foy",
                                Active = true
                            }},
                    Active = true
                },
                new Administrator_SQLDTO
                {
                    Id= 2,
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = true
                },
                new Administrator_SQLDTO
                {
                    Id= 3,
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = false
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsBD);
            DepotAdministrator depot = new DepotAdministrator(context.Object);

            Administrator_SQLDTO? adminInContext = administratorsBD.SingleOrDefault(a => a.Username == "BACenComm" && a.Active == false);

            int id = adminInContext.Id;

            Administrator administrator = depot.GetAdministratorById(id);

            Assert.NotNull(adminInContext);
            Assert.Null(administrator);
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Test_GetByEmail()
        {
            List<Administrator_SQLDTO> administratorsBD = new List<Administrator_SQLDTO>
            {
                new Administrator_SQLDTO
                {
                    Id= 1,
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Token = "token",
                    Organisations = {
                    new Organisation_SQLDTO()
                            {
                                Name = "CEGEP Ste-Foy",
                                Active = true
                            }},
                    Active = true
                },
                new Administrator_SQLDTO
                {
                    Id= 2,
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = true
                },
                new Administrator_SQLDTO
                {
                    Id= 3,
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = false
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsBD);
            DepotAdministrator depot = new DepotAdministrator(context.Object);

            Administrator_SQLDTO? admin = administratorsBD.SingleOrDefault(a => a.Email == "ThPaquet@hotmail.com");

            Administrator administrator = depot.GetAdministratorByEmail("ThPaquet@hotmail.com");

            Assert.NotNull(administrator);
            Assert.Equal("Thierry", administrator.FirstName);
            Assert.Equal("Paquet", administrator.LastName);
            Assert.Equal("token", administrator.Token);
            Assert.NotEmpty(administrator.Organisations);
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Test_GetByEmail_NotActive()
        {
            List<Administrator_SQLDTO> administratorsBD = new List<Administrator_SQLDTO>
            {
                new Administrator_SQLDTO
                {
                    Id= 1,
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Token = "token",
                    Organisations = {
                    new Organisation_SQLDTO()
                            {
                                Name = "CEGEP Ste-Foy",
                                Active = true
                            }},
                    Active = true
                },
                new Administrator_SQLDTO
                {
                    Id= 2,
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = true
                },
                new Administrator_SQLDTO
                {
                    Id= 3,
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = false
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsBD);
            DepotAdministrator depot = new DepotAdministrator(context.Object);

            Administrator_SQLDTO? adminInContext = administratorsBD.SingleOrDefault(a => a.Email == "BACenComm@hotmail.com" && a.Active == false);

            Administrator administrator = depot.GetAdministratorByEmail("BACenComm@hotmail.com");

            Assert.NotNull(adminInContext);
            Assert.Null(administrator);
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Test_UpsertAdministrator_Inserts()
        {
            List<Administrator_SQLDTO> administratorsBD = new List<Administrator_SQLDTO>
            {
                new Administrator_SQLDTO
                {
                    Id= 1,
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Token = "token",
                    Organisations = {
                    new Organisation_SQLDTO()
                            {
                                Name = "CEGEP Ste-Foy",
                                Active = true
                            }},
                    Active = true
                },
                new Administrator_SQLDTO
                {
                    Id= 2,
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = true
                },
                new Administrator_SQLDTO
                {
                    Id= 3,
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = false
                }
            };
            List<Student_SQLDTO> students = new List<Student_SQLDTO>()
            {
                new Student_SQLDTO()
                {
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Matricule = "1141200",
                    Classes =
                    {
                        new Classroom_SQLDTO()
                        {
                            Name = "ProjetSynthese",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = true
                        },
                        new Classroom_SQLDTO()
                        {
                            Name = "RPLP",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = true
                        },
                        new Classroom_SQLDTO()
                        {
                            Name = "OOP",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = false
                        }
                    },
                    Active = true
                },
                new Student_SQLDTO()
                {
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Matricule = "1122334",
                    Active = true
                },
                new Student_SQLDTO()
                {
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Matricule = "1324354",
                    Active = false
                }
            };
            List<Teacher_SQLDTO> teachers = new List<Teacher_SQLDTO>()
            {
                new Teacher_SQLDTO()
                {
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Classes =
                    {
                        new Classroom_SQLDTO()
                        {
                            Name = "ProjetSynthese",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = true
                        },
                        new Classroom_SQLDTO()
                        {
                            Name = "RPLP",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = true
                        },
                        new Classroom_SQLDTO()
                        {
                            Name = "OOP",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = false
                        }
                    },
                    Active = true
                },
                new Teacher_SQLDTO()
                {
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Active = true
                },
                new Teacher_SQLDTO()
                {
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Active = false
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Students).ReturnsDbSet(students);
            context.Setup(x => x.Teachers).ReturnsDbSet(teachers);
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsBD);
            context.Setup(m => m.Administrators.Add(It.IsAny<Administrator_SQLDTO>())).Callback<Administrator_SQLDTO>(administratorsBD.Add);
            DepotAdministrator depot = new DepotAdministrator(context.Object);

            depot.UpsertAdministrator(new Administrator
            {
                Id = 4,
                Username = "Node",
                FirstName = "Dylan",
                LastName = "Veilleux",
                Token = "token",
                Email = "node@hotmail.com"
            });

            Administrator_SQLDTO admin = administratorsBD.SingleOrDefault(x => x.Username == "Node");

            Assert.NotNull(admin);
            Assert.Equal("Dylan", admin.FirstName);
            Assert.Equal("Veilleux", admin.LastName);
            Assert.Equal("token", admin.Token);
            Assert.Equal("node@hotmail.com", admin.Email);
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Exactly(4));
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Test_UpsertAdministrator_Updates()
        {
            List<Administrator_SQLDTO> administratorsBD = new List<Administrator_SQLDTO>
            {
                new Administrator_SQLDTO
                {
                    Id= 1,
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Token = "token",
                    Organisations = {
                    new Organisation_SQLDTO()
                            {
                                Name = "CEGEP Ste-Foy",
                                Active = true
                            }},
                    Active = true
                },
                new Administrator_SQLDTO
                {
                    Id= 2,
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = true
                },
                new Administrator_SQLDTO
                {
                    Id= 3,
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = false
                }
            };
            List<Student_SQLDTO> students = new List<Student_SQLDTO>()
            {
                new Student_SQLDTO()
                {
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Matricule = "1141200",
                    Classes =
                    {
                        new Classroom_SQLDTO()
                        {
                            Name = "ProjetSynthese",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = true
                        },
                        new Classroom_SQLDTO()
                        {
                            Name = "RPLP",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = true
                        },
                        new Classroom_SQLDTO()
                        {
                            Name = "OOP",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = false
                        }
                    },
                    Active = true
                },
                new Student_SQLDTO()
                {
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Matricule = "1122334",
                    Active = true
                },
                new Student_SQLDTO()
                {
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Matricule = "1324354",
                    Active = false
                }
            };
            List<Teacher_SQLDTO> teachers = new List<Teacher_SQLDTO>()
            {
                new Teacher_SQLDTO()
                {
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Classes =
                    {
                        new Classroom_SQLDTO()
                        {
                            Name = "ProjetSynthese",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = true
                        },
                        new Classroom_SQLDTO()
                        {
                            Name = "RPLP",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = true
                        },
                        new Classroom_SQLDTO()
                        {
                            Name = "OOP",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = false
                        }
                    },
                    Active = true
                },
                new Teacher_SQLDTO()
                {
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Active = true
                },
                new Teacher_SQLDTO()
                {
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Active = false
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Students).ReturnsDbSet(students);
            context.Setup(x => x.Teachers).ReturnsDbSet(teachers);
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsBD);
            context.Setup(m => m.Administrators.Add(It.IsAny<Administrator_SQLDTO>())).Callback<Administrator_SQLDTO>(administratorsBD.Add);
            DepotAdministrator depot = new DepotAdministrator(context.Object);

            Administrator_SQLDTO? admin = administratorsBD.SingleOrDefault(a => a.Username == "ThPaquet");

            admin.Username = "Upserted";
            admin.FirstName = "Upserty";
            admin.LastName = "McUpserton";
            admin.Email = "Upserted@hotmail.com";

            depot.UpsertAdministrator(admin.ToEntityWithoutList());

            admin = administratorsBD.SingleOrDefault(a => a.Username == "ThPaquet");
            Assert.Null(admin);

            admin = administratorsBD.SingleOrDefault(a => a.Username == "Upserted");

            Assert.NotNull(admin);
            Assert.Equal("Upserty", admin.FirstName);
            Assert.Equal("McUpserton", admin.LastName);
            Assert.Equal("Upserted@hotmail.com", admin.Email);
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Exactly(2));
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Test_UpsertAdministrator_ThrowUpdateDeletedAccount()
        {
            List<Administrator_SQLDTO> administratorsBD = new List<Administrator_SQLDTO>
            {
                new Administrator_SQLDTO
                {
                    Id= 1,
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Token = "token",
                    Organisations = {
                    new Organisation_SQLDTO()
                            {
                                Name = "CEGEP Ste-Foy",
                                Active = true
                            }},
                    Active = true
                },
                new Administrator_SQLDTO
                {
                    Id= 2,
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = true
                },
                new Administrator_SQLDTO
                {
                    Id= 3,
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = false
                }
            };
            List<Student_SQLDTO> students = new List<Student_SQLDTO>()
            {
                new Student_SQLDTO()
                {
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Matricule = "1141200",
                    Classes =
                    {
                        new Classroom_SQLDTO()
                        {
                            Name = "ProjetSynthese",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = true
                        },
                        new Classroom_SQLDTO()
                        {
                            Name = "RPLP",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = true
                        },
                        new Classroom_SQLDTO()
                        {
                            Name = "OOP",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = false
                        }
                    },
                    Active = true
                },
                new Student_SQLDTO()
                {
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Matricule = "1122334",
                    Active = true
                },
                new Student_SQLDTO()
                {
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Matricule = "1324354",
                    Active = false
                }
            };
            List<Teacher_SQLDTO> teachers = new List<Teacher_SQLDTO>()
            {
                new Teacher_SQLDTO()
                {
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Classes =
                    {
                        new Classroom_SQLDTO()
                        {
                            Name = "ProjetSynthese",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = true
                        },
                        new Classroom_SQLDTO()
                        {
                            Name = "RPLP",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = true
                        },
                        new Classroom_SQLDTO()
                        {
                            Name = "OOP",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = false
                        }
                    },
                    Active = true
                },
                new Teacher_SQLDTO()
                {
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Active = true
                },
                new Teacher_SQLDTO()
                {
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Active = false
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Students).ReturnsDbSet(students);
            context.Setup(x => x.Teachers).ReturnsDbSet(teachers);
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsBD);
            DepotAdministrator depot = new DepotAdministrator(context.Object);

            Administrator_SQLDTO? admin = administratorsBD.SingleOrDefault(a => a.Username == "BACenComm");

            Assert.Throws<ArgumentException>(
                () =>
                {
                    depot.UpsertAdministrator(admin.ToEntityWithoutList());
                });
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Never);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Test_UpsertAdministrator_ThrowUsernameTaken_UsernameTakenNotActive()
        {
            List<Administrator_SQLDTO> administratorsBD = new List<Administrator_SQLDTO>
            {
                new Administrator_SQLDTO
                {
                    Id= 1,
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Token = "token",
                    Organisations = {
                    new Organisation_SQLDTO()
                            {
                                Name = "CEGEP Ste-Foy",
                                Active = true
                            }},
                    Active = true
                },
                new Administrator_SQLDTO
                {
                    Id= 2,
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = true
                },
                new Administrator_SQLDTO
                {
                    Id= 3,
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = false
                }
            };
            List<Student_SQLDTO> students = new List<Student_SQLDTO>()
            {
                new Student_SQLDTO()
                {
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Matricule = "1141200",
                    Classes =
                    {
                        new Classroom_SQLDTO()
                        {
                            Name = "ProjetSynthese",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = true
                        },
                        new Classroom_SQLDTO()
                        {
                            Name = "RPLP",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = true
                        },
                        new Classroom_SQLDTO()
                        {
                            Name = "OOP",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = false
                        }
                    },
                    Active = true
                },
                new Student_SQLDTO()
                {
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Matricule = "1122334",
                    Active = true
                },
                new Student_SQLDTO()
                {
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Matricule = "1324354",
                    Active = false
                }
            };
            List<Teacher_SQLDTO> teachers = new List<Teacher_SQLDTO>()
            {
                new Teacher_SQLDTO()
                {
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Classes =
                    {
                        new Classroom_SQLDTO()
                        {
                            Name = "ProjetSynthese",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = true
                        },
                        new Classroom_SQLDTO()
                        {
                            Name = "RPLP",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = true
                        },
                        new Classroom_SQLDTO()
                        {
                            Name = "OOP",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = false
                        }
                    },
                    Active = true
                },
                new Teacher_SQLDTO()
                {
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Active = true
                },
                new Teacher_SQLDTO()
                {
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Active = false
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Students).ReturnsDbSet(students);
            context.Setup(x => x.Teachers).ReturnsDbSet(teachers);
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsBD);
            DepotAdministrator depot = new DepotAdministrator(context.Object);

            Administrator_SQLDTO? admin = new Administrator_SQLDTO
            {
                Id = 2,
                Username = "BACenComm",
                FirstName = "Jonathan",
                LastName = "Blouin",
                Email = "ikeameatbol@hotmail.com",
                Token = "token",
                Organisations = { },
                Active = true
            };

            Assert.Throws<ArgumentException>(
                () =>
                {
                    depot.UpsertAdministrator(admin.ToEntityWithoutList());
                });
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Test_UpsertAdministrator_ThrowUsernameTaken_NewAdmin()
        {
            List<Administrator_SQLDTO> administratorsBD = new List<Administrator_SQLDTO>
            {
                new Administrator_SQLDTO
                {
                    Id= 1,
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Token = "token",
                    Organisations = {
                    new Organisation_SQLDTO()
                            {
                                Name = "CEGEP Ste-Foy",
                                Active = true
                            }},
                    Active = true
                },
                new Administrator_SQLDTO
                {
                    Id= 2,
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = true
                },
                new Administrator_SQLDTO
                {
                    Id= 3,
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = false
                }
            };
            List<Student_SQLDTO> students = new List<Student_SQLDTO>()
            {
                new Student_SQLDTO()
                {
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Matricule = "1141200",
                    Classes =
                    {
                        new Classroom_SQLDTO()
                        {
                            Name = "ProjetSynthese",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = true
                        },
                        new Classroom_SQLDTO()
                        {
                            Name = "RPLP",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = true
                        },
                        new Classroom_SQLDTO()
                        {
                            Name = "OOP",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = false
                        }
                    },
                    Active = true
                },
                new Student_SQLDTO()
                {
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Matricule = "1122334",
                    Active = true
                },
                new Student_SQLDTO()
                {
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Matricule = "1324354",
                    Active = false
                }
            };
            List<Teacher_SQLDTO> teachers = new List<Teacher_SQLDTO>()
            {
                new Teacher_SQLDTO()
                {
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Classes =
                    {
                        new Classroom_SQLDTO()
                        {
                            Name = "ProjetSynthese",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = true
                        },
                        new Classroom_SQLDTO()
                        {
                            Name = "RPLP",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = true
                        },
                        new Classroom_SQLDTO()
                        {
                            Name = "OOP",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = false
                        }
                    },
                    Active = true
                },
                new Teacher_SQLDTO()
                {
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Active = true
                },
                new Teacher_SQLDTO()
                {
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Active = false
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Students).ReturnsDbSet(students);
            context.Setup(x => x.Teachers).ReturnsDbSet(teachers);
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsBD);
            DepotAdministrator depot = new DepotAdministrator(context.Object);

            Administrator_SQLDTO admin = new Administrator_SQLDTO()
            {
                Username = "ThPaquet",
                FirstName = "Thierry",
                LastName = "Paquet",
                Token = "token",
                Organisations = new List<Organisation_SQLDTO>(),
                Email = "swerve@hotmail.com",
                Active = true
            };

            Assert.Throws<ArgumentException>(
                () =>
                {
                    depot.UpsertAdministrator(admin.ToEntityWithoutList());
                });
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Exactly(2));
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Test_UpsertAdministrator_ThrowEmailTaken_EmailTakenNotActive()
        {
            List<Administrator_SQLDTO> administratorsBD = new List<Administrator_SQLDTO>
            {
                new Administrator_SQLDTO
                {
                    Id= 1,
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Token = "token",
                    Organisations = {
                    new Organisation_SQLDTO()
                            {
                                Name = "CEGEP Ste-Foy",
                                Active = true
                            }},
                    Active = true
                },
                new Administrator_SQLDTO
                {
                    Id= 2,
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = true
                },
                new Administrator_SQLDTO
                {
                    Id= 3,
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = false
                }
            };
            List<Student_SQLDTO> students = new List<Student_SQLDTO>()
            {
                new Student_SQLDTO()
                {
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Matricule = "1141200",
                    Classes =
                    {
                        new Classroom_SQLDTO()
                        {
                            Name = "ProjetSynthese",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = true
                        },
                        new Classroom_SQLDTO()
                        {
                            Name = "RPLP",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = true
                        },
                        new Classroom_SQLDTO()
                        {
                            Name = "OOP",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = false
                        }
                    },
                    Active = true
                },
                new Student_SQLDTO()
                {
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Matricule = "1122334",
                    Active = true
                },
                new Student_SQLDTO()
                {
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Matricule = "1324354",
                    Active = false
                }
            };
            List<Teacher_SQLDTO> teachers = new List<Teacher_SQLDTO>()
            {
                new Teacher_SQLDTO()
                {
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Classes =
                    {
                        new Classroom_SQLDTO()
                        {
                            Name = "ProjetSynthese",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = true
                        },
                        new Classroom_SQLDTO()
                        {
                            Name = "RPLP",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = true
                        },
                        new Classroom_SQLDTO()
                        {
                            Name = "OOP",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = false
                        }
                    },
                    Active = true
                },
                new Teacher_SQLDTO()
                {
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Active = true
                },
                new Teacher_SQLDTO()
                {
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Active = false
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Students).ReturnsDbSet(students);
            context.Setup(x => x.Teachers).ReturnsDbSet(teachers);
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsBD);
            DepotAdministrator depot = new DepotAdministrator(context.Object);

            Administrator_SQLDTO? admin = new Administrator_SQLDTO
            {
                Id = 2,
                Username = "ikeameatbol",
                FirstName = "Jonathan",
                LastName = "Blouin",
                Email = "BACenComm@hotmail.com",
                Token = "token",
                Organisations = { },
                Active = true
            };

            Assert.Throws<ArgumentException>(
                () =>
                {
                    depot.UpsertAdministrator(admin.ToEntityWithoutList());
                });
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Test_UpsertAdministrator_ThrowEmailTaken_NewAdmin()
        {
            List<Administrator_SQLDTO> administratorsBD = new List<Administrator_SQLDTO>
            {
                new Administrator_SQLDTO
                {
                    Id= 1,
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Token = "token",
                    Organisations = {
                    new Organisation_SQLDTO()
                            {
                                Name = "CEGEP Ste-Foy",
                                Active = true
                            }},
                    Active = true
                },
                new Administrator_SQLDTO
                {
                    Id= 2,
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = true
                },
                new Administrator_SQLDTO
                {
                    Id= 3,
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = false
                }
            };
            List<Student_SQLDTO> students = new List<Student_SQLDTO>()
            {
                new Student_SQLDTO()
                {
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Matricule = "1141200",
                    Classes =
                    {
                        new Classroom_SQLDTO()
                        {
                            Name = "ProjetSynthese",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = true
                        },
                        new Classroom_SQLDTO()
                        {
                            Name = "RPLP",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = true
                        },
                        new Classroom_SQLDTO()
                        {
                            Name = "OOP",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = false
                        }
                    },
                    Active = true
                },
                new Student_SQLDTO()
                {
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Matricule = "1122334",
                    Active = true
                },
                new Student_SQLDTO()
                {
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Matricule = "1324354",
                    Active = false
                }
            };
            List<Teacher_SQLDTO> teachers = new List<Teacher_SQLDTO>()
            {
                new Teacher_SQLDTO()
                {
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Classes =
                    {
                        new Classroom_SQLDTO()
                        {
                            Name = "ProjetSynthese",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = true
                        },
                        new Classroom_SQLDTO()
                        {
                            Name = "RPLP",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = true
                        },
                        new Classroom_SQLDTO()
                        {
                            Name = "OOP",
                            OrganisationName = "CEGEP Ste-Foy",
                            Active = false
                        }
                    },
                    Active = true
                },
                new Teacher_SQLDTO()
                {
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Active = true
                },
                new Teacher_SQLDTO()
                {
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Active = false
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Students).ReturnsDbSet(students);
            context.Setup(x => x.Teachers).ReturnsDbSet(teachers);
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsBD);
            DepotAdministrator depot = new DepotAdministrator(context.Object);

            Administrator_SQLDTO admin = new Administrator_SQLDTO()
            {
                Username = "Swerve",
                FirstName = "Thierry",
                LastName = "Paquet",
                Token = "token",
                Organisations = new List<Organisation_SQLDTO>(),
                Email = "ThPaquet@hotmail.com",
                Active = true
            };

            Assert.Throws<ArgumentException>(
                () =>
                {
                    depot.UpsertAdministrator(admin.ToEntityWithoutList());
                });
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Exactly(3));
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Test_GetAdminOrganisations()
        {
            List<Administrator_SQLDTO> administratorsBD = new List<Administrator_SQLDTO>
            {
                new Administrator_SQLDTO
                {
                    Id= 1,
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Token = "token",
                    Organisations = {
                    new Organisation_SQLDTO()
                            {
                                Name = "CEGEP Ste-Foy",
                                Active = true
                            }},
                    Active = true
                },
                new Administrator_SQLDTO
                {
                    Id= 2,
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = true
                },
                new Administrator_SQLDTO
                {
                    Id= 3,
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = false
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsBD);
            DepotAdministrator depot = new DepotAdministrator(context.Object);

            List<Organisation> organisations = depot.GetAdminOrganisations("ThPaquet");

            Assert.Equal(1, organisations.Count);
            Assert.Contains(organisations, o => o.Name == "CEGEP Ste-Foy");
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Test_JoinOrganisation()
        {
            List<Administrator_SQLDTO> administratorsBD = new List<Administrator_SQLDTO>
            {
                new Administrator_SQLDTO
                {
                    Id= 1,
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Token = "token",
                    Organisations = {
                    new Organisation_SQLDTO()
                            {
                                Name = "CEGEP Ste-Foy",
                                Active = true
                            }},
                    Active = true
                },
                new Administrator_SQLDTO
                {
                    Id= 2,
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = true
                },
                new Administrator_SQLDTO
                {
                    Id= 3,
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = false
                }
            };
            List<Organisation_SQLDTO> organisations = new List<Organisation_SQLDTO>()
            {
                new Organisation_SQLDTO()
                {
                    Name = "CEGEP Ste-Foy",
                    Administrators = administratorsBD,
                    Active = true
                },
                new Organisation_SQLDTO()
                {
                    Name = "College Edouard-Montpetit",
                    Administrators = administratorsBD,
                    Active = true
                },
                new Organisation_SQLDTO()
                {
                    Name = "Universite Laval",
                    Administrators = administratorsBD,
                    Active = false
                },
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsBD);
            context.Setup(x => x.Organisations).ReturnsDbSet(organisations);
            DepotAdministrator depot = new DepotAdministrator(context.Object);

            organisations.Add(new Organisation_SQLDTO
            {
                Name = "RPLP",
                Active = true
            });

            Assert.Contains(administratorsBD, a => a.Username == "ikeameatbol" && a.Active);
            depot.JoinOrganisation("ikeameatbol", "RPLP");

            Administrator_SQLDTO? admin = administratorsBD.FirstOrDefault(a => a.Username == "ikeameatbol");

            Assert.NotNull(admin);
            Assert.Equal(1, admin.Organisations.Count);
            Assert.Contains(admin.Organisations, o => o.Name == "RPLP");
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Test_LeaveOrganisation()
        {
            List<Administrator_SQLDTO> administratorsBD = new List<Administrator_SQLDTO>
            {
                new Administrator_SQLDTO
                {
                    Id= 1,
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = true
                },
                new Administrator_SQLDTO
                {
                    Id= 2,
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = true
                },
                new Administrator_SQLDTO
                {
                    Id= 3,
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = false
                }
            };
            List<Organisation_SQLDTO> organisations = new List<Organisation_SQLDTO>()
            {
                new Organisation_SQLDTO()
                {
                    Name = "CEGEP Ste-Foy",
                    Administrators = administratorsBD,
                    Active = true
                },
                new Organisation_SQLDTO()
                {
                    Name = "College Edouard-Montpetit",
                    Administrators = administratorsBD,
                    Active = true
                },
                new Organisation_SQLDTO()
                {
                    Name = "Universite Laval",
                    Administrators = administratorsBD,
                    Active = false
                },
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsBD);
            context.Setup(x => x.Organisations).ReturnsDbSet(organisations);
            DepotAdministrator depot = new DepotAdministrator(context.Object);

            Administrator_SQLDTO? admin = null;
            Organisation_SQLDTO? organisation = null;

            admin = administratorsBD.FirstOrDefault(a => a.Username == "ThPaquet");
            organisation = organisations.FirstOrDefault(o => o.Name == "CEGEP Ste-Foy");

            Assert.NotNull(admin);
            Assert.NotNull(organisation);

            depot.JoinOrganisation("ThPaquet", "CEGEP Ste-Foy");
            admin = administratorsBD.FirstOrDefault(a => a.Username == "ThPaquet");
            organisation = admin.Organisations.FirstOrDefault(o => o.Name == "CEGEP Ste-Foy");
            Assert.NotNull(admin);
            Assert.NotNull(organisation);

            depot.LeaveOrganisation("ThPaquet", "CEGEP Ste-Foy");

            admin = administratorsBD.FirstOrDefault(a => a.Username == "ThPaquet");
            organisation = admin.Organisations.FirstOrDefault(o => o.Name == "CEGEP Ste-Foy");

            Assert.Null(organisation);
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Exactly(2));
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Test_DeleteAdministrator()
        {
            List<Administrator_SQLDTO> administratorsBD = new List<Administrator_SQLDTO>
            {
                new Administrator_SQLDTO
                {
                    Id= 1,
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = true
                },
                new Administrator_SQLDTO
                {
                    Id= 2,
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = true
                },
                new Administrator_SQLDTO
                {
                    Id= 3,
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = false
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsBD);
            DepotAdministrator depot = new DepotAdministrator(context.Object);

            Assert.True(administratorsBD.Any(a => a.Username == "ikeameatbol" && a.Active));

            depot.DeleteAdministrator("ikeameatbol");

            Assert.True(administratorsBD.Any(a => a.Username == "ikeameatbol" && !a.Active));
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Test_ReactivateAdministrator()
        {
            List<Administrator_SQLDTO> administratorsBD = new List<Administrator_SQLDTO>
            {
                new Administrator_SQLDTO
                {
                    Id= 1,
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = true
                },
                new Administrator_SQLDTO
                {
                    Id= 2,
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = true
                },
                new Administrator_SQLDTO
                {
                    Id= 3,
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Token = "token",
                    Organisations = {},
                    Active = false
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsBD);
            DepotAdministrator depot = new DepotAdministrator(context.Object);

            Assert.True(administratorsBD.Any(a => a.Username == "BACenComm" && !a.Active));

            depot.ReactivateAdministrator("BACenComm");

            Assert.True(administratorsBD.Any(a => a.Username == "BACenComm" && a.Active));
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }
    }
}