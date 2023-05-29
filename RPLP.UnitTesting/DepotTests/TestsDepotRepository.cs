using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using RPLP.DAL.DTO.Json;
using RPLP.DAL.DTO.Sql;
using RPLP.DAL.SQL;
using RPLP.DAL.SQL.Depots;
using RPLP.ENTITES;
using RPLP.JOURNALISATION;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RPLP.UnitTesting.DepotTests
{
    public class TestsDepotRepository
    {
        [Fact]
        public void Test_GetRepositoryById()
        {
            List<Repository_SQLDTO> repositoriesDB = new List<Repository_SQLDTO>()
                {
                    new Repository_SQLDTO()
                    {
                        Id= 1,
                        Name = "ThPaquet",
                        FullName = "Thierry Paquet",
                        OrganisationId = 1,
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id= 2,
                        Name = "ikeameatbol",
                        FullName = "Jonathan Blouin",
                        OrganisationId = 1,
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id= 3,
                        Name = "BACenComm",
                        FullName = "Melissa Lachapelle",
                        OrganisationId = 1,
                        Active = false
                    },
                };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDB);
            DepotRepository depot = new DepotRepository(context.Object);

            Repository_SQLDTO? repositoryThPaquet = repositoriesDB.FirstOrDefault(r => r.Name == "ThPaquet");
            Assert.NotNull(repositoryThPaquet);

            int repositoryId = repositoryThPaquet.Id;

            Repository repository = depot.GetRepositoryById(repositoryId);

            Assert.NotNull(repository);
           
            
        }

        [Fact]
        public void Test_GetRepositoryById_NotActive()
        {
            List<Repository_SQLDTO> repositoriesDB = new List<Repository_SQLDTO>()
                {
                    new Repository_SQLDTO()
                    {
                        Id = 1,
                        Name = "ThPaquet",
                        FullName = "Thierry Paquet",
                        OrganisationId = 1,
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id = 2,
                        Name = "ikeameatbol",
                        FullName = "Jonathan Blouin",
                        OrganisationId = 1,
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id = 3,
                        Name = "BACenComm",
                        FullName = "Melissa Lachapelle",
                        OrganisationId = 1,
                        Active = false
                    },
                };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDB);
            DepotRepository depot = new DepotRepository(context.Object);

            Repository_SQLDTO? repositoryBACenComm = repositoriesDB.FirstOrDefault(r => r.Name == "BACenComm");
            Assert.NotNull(repositoryBACenComm);

            int repositoryId = repositoryBACenComm.Id;

            Repository repository = depot.GetRepositoryById(repositoryId);

            Assert.Null(repository);
           
            
        }

        [Fact]
        public void Test_GetRepositoryByName()
        {
            List<Organisation_SQLDTO> organisationsDB = new List<Organisation_SQLDTO>()
            {
                new Organisation_SQLDTO()
                {
                    Active = true,
                    Id = 1,
                    Name = "organisation"
                }
            };
            
            List<Repository_SQLDTO> repositoriesDB = new List<Repository_SQLDTO>()
                {
                    new Repository_SQLDTO()
                    {
                        Id= 1,
                        Name = "ThPaquet",
                        FullName = "Thierry Paquet",
                        OrganisationId = 1,
                        Organisation = new Organisation_SQLDTO()
                        {
                            Active = true,
                            Id = 1,
                            Name = "organisation"
                        },
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id= 2,
                        Name = "ikeameatbol",
                        FullName = "Jonathan Blouin",
                        OrganisationId = 1,
                        Organisation = new Organisation_SQLDTO()
                        {
                            Active = true,
                            Id = 1,
                            Name = "organisation"
                        },
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id= 3,
                        Name = "BACenComm",
                        FullName = "Melissa Lachapelle",
                        OrganisationId = 1,
                        Organisation = new Organisation_SQLDTO()
                        {
                            Active = true,
                            Id = 1,
                            Name = "organisation"
                        },
                        Active = false
                    },
                };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Organisations).ReturnsDbSet(organisationsDB);
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDB);
            DepotRepository depot = new DepotRepository(context.Object);

            Repository repository = depot.GetRepositoryByName("ThPaquet");

            Assert.NotNull(repository);
           
            
        }

        [Fact]
        public void Test_GetRepositoryByName_NotActive()
        {
            List<Repository_SQLDTO> repositoriesDB = new List<Repository_SQLDTO>()
                {
                    new Repository_SQLDTO()
                    {
                        Id=1,
                        Name = "ThPaquet",
                        FullName = "Thierry Paquet",
                        OrganisationId = 1,
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=2,
                        Name = "ikeameatbol",
                        FullName = "Jonathan Blouin",
                        OrganisationId = 1,
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=3,
                        Name = "BACenComm",
                        FullName = "Melissa Lachapelle",
                        OrganisationId = 1,
                        Active = false
                    },
                };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDB);
            DepotRepository depot = new DepotRepository(context.Object);

            Repository repository = depot.GetRepositoryByName("BACenComm");

            Assert.Null(repository);
           
            
        }

        [Fact]
        public void Test_UpsertRepository_Inserts()
        {
            List<Repository_SQLDTO> repositoriesDB = new List<Repository_SQLDTO>()
                {
                    new Repository_SQLDTO()
                    {
                        Id=1,
                        Name = "ThPaquet",
                        FullName = "Thierry Paquet",
                        OrganisationId = 1,
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=2,
                        Name = "ikeameatbol",
                        FullName = "Jonathan Blouin",
                        OrganisationId = 1,
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=3,
                        Name = "BACenComm",
                        FullName = "Melissa Lachapelle",
                        OrganisationId = 1,
                        Active = false
                    },
                };
            List<Organisation_SQLDTO> organisationsDB = new List<Organisation_SQLDTO>()
            {
                new Organisation_SQLDTO()
                {
                    Id=1,
                    Name = "CEGEP Ste-Foy",
                    Administrators = new List<Administrator_SQLDTO>(),
                    Active = true
                },
                new Organisation_SQLDTO()
                {
                    Id=2,
                    Name = "College Edouard-Montpetit",
                    Administrators = new List<Administrator_SQLDTO>(),
                    Active = true
                },
                new Organisation_SQLDTO()
                {
                    Id=3,
                    Name = "Universite Laval",
                    Administrators = new List<Administrator_SQLDTO>(),
                    Active = false
                },
            };

            foreach (Repository_SQLDTO repository_ in repositoriesDB)
            {
                repository_.Organisation = organisationsDB.Where(x => x.Id == repository_.OrganisationId).FirstOrDefault();
            }

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDB);
            context.Setup(x => x.Organisations).ReturnsDbSet(organisationsDB);
            context.Setup(m => m.Repositories.Add(It.IsAny<Repository_SQLDTO>())).Callback<Repository_SQLDTO>(repositoriesDB.Add);
            DepotRepository depot = new DepotRepository(context.Object);
            
            Organisation mockOrganisation = new Organisation()
            {
                Id = 4,
                Administrators = new List<Administrator>(),      
                Name = "Mock Organisation"
            };

            Repository repository = new Repository()
            {
                Id= 4,
                Name = "testrepo",
                FullName = "Test Repository",
                Organisation = mockOrganisation
            };

            depot.UpsertRepository(repository);

            Repository_SQLDTO? repositorySQL = repositoriesDB.FirstOrDefault(r => r.Name == "testrepo");

            Assert.NotNull(repositorySQL);
            Assert.Equal("testrepo", repositorySQL.Name);
            Assert.Equal("Test Repository", repositorySQL.FullName);
            Assert.True(repositorySQL.Active);
           
            
        }

        [Fact]
        public void Test_UpsertRepository_Updates()
        {
            List<Repository_SQLDTO> repositoriesDB = new List<Repository_SQLDTO>()
                {
                    new Repository_SQLDTO()
                    {
                        Id=1,
                        Name = "ThPaquet",
                        FullName = "Thierry Paquet",
                        OrganisationId = 1,
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=2,
                        Name = "ikeameatbol",
                        FullName = "Jonathan Blouin",
                        OrganisationId = 1,
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=3,
                        Name = "BACenComm",
                        FullName = "Melissa Lachapelle",
                        OrganisationId = 1,
                        Active = false
                    },
                };
            List<Organisation_SQLDTO> organisationsDB = new List<Organisation_SQLDTO>()
            {
                new Organisation_SQLDTO()
                {
                    Id=1,
                    Name = "CEGEP Ste-Foy",
                    Administrators = new List<Administrator_SQLDTO>(),
                    Active = true
                },
                new Organisation_SQLDTO()
                {
                    Id=2,
                    Name = "College Edouard-Montpetit",
                    Administrators = new List<Administrator_SQLDTO>(),
                    Active = true
                },
                new Organisation_SQLDTO()
                {
                    Id=3,
                    Name = "Universite Laval",
                    Administrators = new List<Administrator_SQLDTO>(),
                    Active = false
                },
            };

            foreach (Repository_SQLDTO repository_ in repositoriesDB)
            {
                repository_.Organisation = organisationsDB.Where(x => x.Id == repository_.OrganisationId).FirstOrDefault();
            }

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Organisations).ReturnsDbSet(organisationsDB);
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDB);
            DepotRepository depot = new DepotRepository(context.Object);
            
            Organisation mockOrganisation = new Organisation()
            {
                Administrators = new List<Administrator>(),
                Id = 4,
                Name = "Mock Organisation"
            };

            Repository_SQLDTO? repository = repositoriesDB.FirstOrDefault(r => r.Name == "ThPaquet");
            Assert.NotNull(repository);

            Repository _repository = new Repository()
            {
                Id = repository.Id,
                Name = repository.Name,
                FullName = "Test Repository",
                Organisation = mockOrganisation
            };

            depot.UpsertRepository(_repository);

            Repository_SQLDTO? repositorySQL = repositoriesDB.FirstOrDefault(r => r.Name == "ThPaquet");

            Assert.NotNull(repositorySQL);
            Assert.Equal("ThPaquet", repositorySQL.Name);
            Assert.Equal("Test Repository", repositorySQL.FullName);
            Assert.Equal(4, repositorySQL.OrganisationId);
            Assert.True(repositorySQL.Active);
           
            
        }

        [Fact]
        public void Test_DeleteRepository()
        {
            List<Repository_SQLDTO> repositoriesDB = new List<Repository_SQLDTO>()
                {
                    new Repository_SQLDTO()
                    {
                        Id=1,
                        Name = "ThPaquet",
                        FullName = "Thierry Paquet",
                        OrganisationId = 1,
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=2,
                        Name = "ikeameatbol",
                        FullName = "Jonathan Blouin",
                        OrganisationId = 1,
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=3,
                        Name = "BACenComm",
                        FullName = "Melissa Lachapelle",
                        OrganisationId = 1,
                        Active = false
                    },
                };
            List<Organisation_SQLDTO> organisationsDB = new List<Organisation_SQLDTO>()
            {
                new Organisation_SQLDTO()
                {
                    Id=1,
                    Name = "CEGEP Ste-Foy",
                    Administrators = new List<Administrator_SQLDTO>(),
                    Active = true
                },
                new Organisation_SQLDTO()
                {
                    Id=2,
                    Name = "College Edouard-Montpetit",
                    Administrators = new List<Administrator_SQLDTO>(),
                    Active = true
                },
                new Organisation_SQLDTO()
                {
                    Id=3,
                    Name = "Universite Laval",
                    Administrators = new List<Administrator_SQLDTO>(),
                    Active = false
                },
            };

            foreach (Repository_SQLDTO repository_ in repositoriesDB)
            {
                repository_.Organisation = organisationsDB.Where(x => x.Id == repository_.OrganisationId).FirstOrDefault();
            }

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDB);
            context.Setup(x => x.Organisations).ReturnsDbSet(organisationsDB);
            DepotRepository depot = new DepotRepository(context.Object);
            
            Repository mockRepository = new Repository()
            {
                FullName = "ThPaquet",
                Id = 1,
                Name = "ThPaquet",
                Organisation = new Organisation()
            };

            depot.DeleteRepository(mockRepository);

            Repository_SQLDTO? repository = repositoriesDB.FirstOrDefault(r => r.Name == "ThPaquet" && r.Active);
            Assert.Null(repository);
           
            
        }

        [Fact]
        public void Test_GetRepositories()
        {
            List<Repository_SQLDTO> repositoriesDB = new List<Repository_SQLDTO>()
                {
                    new Repository_SQLDTO()
                    {
                        Id=1,
                        Name = "ThPaquet",
                        FullName = "Thierry Paquet",
                        OrganisationId = 1,
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=2,
                        Name = "ikeameatbol",
                        FullName = "Jonathan Blouin",
                        OrganisationId = 1,
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=3,
                        Name = "BACenComm",
                        FullName = "Melissa Lachapelle",
                        OrganisationId = 1,
                        Active = false
                    },
                };
            List<Organisation_SQLDTO> organisationsDB = new List<Organisation_SQLDTO>()
            {
                new Organisation_SQLDTO()
                {
                    Id=1,
                    Name = "CEGEP Ste-Foy",
                    Administrators = new List<Administrator_SQLDTO>(),
                    Active = true
                },
                new Organisation_SQLDTO()
                {
                    Id=2,
                    Name = "College Edouard-Montpetit",
                    Administrators = new List<Administrator_SQLDTO>(),
                    Active = true
                },
                new Organisation_SQLDTO()
                {
                    Id=3,
                    Name = "Universite Laval",
                    Administrators = new List<Administrator_SQLDTO>(),
                    Active = false
                },
            };

            foreach (Repository_SQLDTO repository_ in repositoriesDB)
            {
                repository_.Organisation = organisationsDB.Where(x => x.Id == repository_.OrganisationId).FirstOrDefault();
            }

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDB);
            context.Setup(x => x.Organisations).ReturnsDbSet(organisationsDB);
            DepotRepository depot = new DepotRepository(context.Object);

            List<Repository> repositories = depot.GetRepositories();

            Assert.Equal(2, repositories.Count);
           
            
        }

        [Fact]
        public void Test_GetRepositoriesByOrganisationName()
        {
            List<Repository_SQLDTO> repositoriesDB = new List<Repository_SQLDTO>()
                {
                    new Repository_SQLDTO()
                    {
                        Id=1,
                        Name = "ThPaquet",
                        FullName = "Thierry Paquet",
                        OrganisationId = 1,
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=2,
                        Name = "ikeameatbol",
                        FullName = "Jonathan Blouin",
                        OrganisationId = 1,
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=3,
                        Name = "BACenComm",
                        FullName = "Melissa Lachapelle",
                        OrganisationId = 1,
                        Active = false
                    },
                };
            List<Organisation_SQLDTO> organisationsDB = new List<Organisation_SQLDTO>()
            {
                new Organisation_SQLDTO()
                {
                    Id=1,
                    Name = "CEGEP Ste-Foy",
                    Administrators = new List<Administrator_SQLDTO>(),
                    Active = true
                },
                new Organisation_SQLDTO()
                {
                    Id=2,
                    Name = "College Edouard-Montpetit",
                    Administrators = new List<Administrator_SQLDTO>(),
                    Active = true
                },
                new Organisation_SQLDTO()
                {
                    Id=3,
                    Name = "Universite Laval",
                    Administrators = new List<Administrator_SQLDTO>(),
                    Active = false
                },
            };

            foreach (Repository_SQLDTO repository_ in repositoriesDB)
            {
                repository_.Organisation = organisationsDB.Where(x => x.Id == repository_.OrganisationId).FirstOrDefault();
            }

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDB);
            context.Setup(x => x.Organisations).ReturnsDbSet(organisationsDB);
            DepotRepository depot = new DepotRepository(context.Object);

            List<Repository> repositories = depot.GetRepositoriesFromOrganisationName("CEGEP Ste-Foy");

            Assert.Equal(3, repositories.Count);
           
            
        }

        [Fact]
        public void Test_ReactivateRepository_InactiveRepository_RepositoryActive()
        {
            List<Repository_SQLDTO> repositoriesDB = new List<Repository_SQLDTO>()
                {
                    new Repository_SQLDTO()
                    {
                        Id=1,
                        Name = "ThPaquet",
                        FullName = "Thierry Paquet",
                        OrganisationId = 1,
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=2,
                        Name = "ikeameatbol",
                        FullName = "Jonathan Blouin",
                        OrganisationId = 1,
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=3,
                        Name = "BACenComm",
                        FullName = "Melissa Lachapelle",
                        OrganisationId = 1,
                        Active = false
                    },
                };
            List<Organisation_SQLDTO> organisationsDB = new List<Organisation_SQLDTO>()
            {
                new Organisation_SQLDTO()
                {
                    Id=1,
                    Name = "CEGEP Ste-Foy",
                    Administrators = new List<Administrator_SQLDTO>(),
                    Active = true
                },
                new Organisation_SQLDTO()
                {
                    Id=2,
                    Name = "College Edouard-Montpetit",
                    Administrators = new List<Administrator_SQLDTO>(),
                    Active = true
                },
                new Organisation_SQLDTO()
                {
                    Id=3,
                    Name = "Universite Laval",
                    Administrators = new List<Administrator_SQLDTO>(),
                    Active = false
                },
            };

            foreach (Repository_SQLDTO repository_ in repositoriesDB)
            {
                repository_.Organisation = organisationsDB.Where(x => x.Id == repository_.OrganisationId).FirstOrDefault();
            }

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDB);
            context.Setup(x => x.Organisations).ReturnsDbSet(organisationsDB);
            DepotRepository depot = new DepotRepository(context.Object);
            
            Repository mockRepository = new Repository()
            {
                Id = 3,
                Name = "BACenComm",
                FullName = "Melissa Lachapelle",
                Organisation = new Organisation()
            };

            depot.ReactivateRepository(mockRepository);

            Repository_SQLDTO? repository = repositoriesDB.FirstOrDefault(r => r.Name == "BACenComm");
            Assert.True(repository.Active);
        }

        [Fact]
        public void Test_ReactivateRepository_ActiveRepository_RepositoryActive()
        {
            List<Repository_SQLDTO> repositoriesDB = new List<Repository_SQLDTO>()
                {
                    new Repository_SQLDTO()
                    {
                        Id=1,
                        Name = "ThPaquet",
                        FullName = "Thierry Paquet",
                        OrganisationId = 1,
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=2,
                        Name = "ikeameatbol",
                        FullName = "Jonathan Blouin",
                        OrganisationId = 1,
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=3,
                        Name = "BACenComm",
                        FullName = "Melissa Lachapelle",
                        OrganisationId = 1,
                        Active = true
                    },
                };
            List<Organisation_SQLDTO> organisationsDB = new List<Organisation_SQLDTO>()
            {
                new Organisation_SQLDTO()
                {
                    Id=1,
                    Name = "CEGEP Ste-Foy",
                    Administrators = new List<Administrator_SQLDTO>(),
                    Active = true
                },
                new Organisation_SQLDTO()
                {
                    Id=2,
                    Name = "College Edouard-Montpetit",
                    Administrators = new List<Administrator_SQLDTO>(),
                    Active = true
                },
                new Organisation_SQLDTO()
                {
                    Id=3,
                    Name = "Universite Laval",
                    Administrators = new List<Administrator_SQLDTO>(),
                    Active = false
                },
            };

            foreach (Repository_SQLDTO repository_ in repositoriesDB)
            {
                repository_.Organisation = organisationsDB.Where(x => x.Id == repository_.OrganisationId).FirstOrDefault();
            }
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDB);
            context.Setup(x => x.Organisations).ReturnsDbSet(organisationsDB);
            DepotRepository depot = new DepotRepository(context.Object);
            
            Repository mockRepository = new Repository()
            {
                Id = 3,
                Name = "BACenComm",
                FullName = "Melissa Lachapelle",
            };

            depot.ReactivateRepository(mockRepository);

            Repository_SQLDTO? repository = repositoriesDB.FirstOrDefault(r => r.Name == "BACenComm");
            Assert.True(repository.Active);
        }

        [Fact]
        public void Test_ReactivateRepository_InexistantRepository_DoesNothing()
        {
            List<Repository_SQLDTO> repositoriesDB = new List<Repository_SQLDTO>()
                {
                    new Repository_SQLDTO()
                    {
                        Id=1,
                        Name = "ThPaquet",
                        FullName = "Thierry Paquet",
                        OrganisationId = 1,
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=2,
                        Name = "ikeameatbol",
                        FullName = "Jonathan Blouin",
                        OrganisationId = 1,
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=3,
                        Name = "BACenComm",
                        FullName = "Melissa Lachapelle",
                        OrganisationId = 1,
                        Active = false
                    },
                };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDB);
            DepotRepository depot = new DepotRepository(context.Object);

            depot.ReactivateRepository(new Repository());

            Repository_SQLDTO? repository = repositoriesDB.FirstOrDefault(r => r.Name == "InexistantRepo");
            Assert.Null(repository);
        }

        [Fact]
        public void Test_ReactivateRepository_ParamNull_ExceptionThrown()
        {
            List<Repository_SQLDTO> repositoriesDB = new List<Repository_SQLDTO>()
                {
                    new Repository_SQLDTO()
                    {
                        Id=1,
                        Name = "ThPaquet",
                        FullName = "Thierry Paquet",
                        OrganisationId = 1,
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=2,
                        Name = "ikeameatbol",
                        FullName = "Jonathan Blouin",
                        OrganisationId = 1,
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=3,
                        Name = "BACenComm",
                        FullName = "Melissa Lachapelle",
                        OrganisationId = 1,
                        Active = false
                    },
                };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDB);
            DepotRepository depot = new DepotRepository(context.Object);

            Repository repositoryName = null;

            Assert.Throws<ArgumentNullException>(() => { depot.ReactivateRepository(repositoryName); });
        }

        [Fact]
        public void Test_ReactivateRepository_ParamWhitespace_ExceptionThrown()
        {
            List<Repository_SQLDTO> repositoriesDB = new List<Repository_SQLDTO>()
                {
                    new Repository_SQLDTO()
                    {
                        Id=1,
                        Name = "ThPaquet",
                        FullName = "Thierry Paquet",
                        OrganisationId = 1,
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=2,
                        Name = "ikeameatbol",
                        FullName = "Jonathan Blouin",
                        OrganisationId = 1,
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=3,
                        Name = "BACenComm",
                        FullName = "Melissa Lachapelle",
                        OrganisationId = 1,
                        Active = false
                    },
                };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDB);
            DepotRepository depot = new DepotRepository(context.Object);

            Repository repositoryName = null;

            Assert.Throws<ArgumentNullException>(() => { depot.ReactivateRepository(repositoryName); });
        }
    }
}

