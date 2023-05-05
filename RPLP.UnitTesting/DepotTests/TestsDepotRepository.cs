using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
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
                        OrganisationName = "RPLP",
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id= 2,
                        Name = "ikeameatbol",
                        FullName = "Jonathan Blouin",
                        OrganisationName = "RPLP",
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id= 3,
                        Name = "BACenComm",
                        FullName = "Melissa Lachapelle",
                        OrganisationName = "RPLP",
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
                        OrganisationName = "RPLP",
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id = 2,
                        Name = "ikeameatbol",
                        FullName = "Jonathan Blouin",
                        OrganisationName = "RPLP",
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id = 3,
                        Name = "BACenComm",
                        FullName = "Melissa Lachapelle",
                        OrganisationName = "RPLP",
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
            List<Repository_SQLDTO> repositoriesDB = new List<Repository_SQLDTO>()
                {
                    new Repository_SQLDTO()
                    {
                        Id= 1,
                        Name = "ThPaquet",
                        FullName = "Thierry Paquet",
                        OrganisationName = "RPLP",
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id= 2,
                        Name = "ikeameatbol",
                        FullName = "Jonathan Blouin",
                        OrganisationName = "RPLP",
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id= 3,
                        Name = "BACenComm",
                        FullName = "Melissa Lachapelle",
                        OrganisationName = "RPLP",
                        Active = false
                    },
                };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
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
                        OrganisationName = "RPLP",
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=2,
                        Name = "ikeameatbol",
                        FullName = "Jonathan Blouin",
                        OrganisationName = "RPLP",
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=3,
                        Name = "BACenComm",
                        FullName = "Melissa Lachapelle",
                        OrganisationName = "RPLP",
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
                        OrganisationName = "RPLP",
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=2,
                        Name = "ikeameatbol",
                        FullName = "Jonathan Blouin",
                        OrganisationName = "RPLP",
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=3,
                        Name = "BACenComm",
                        FullName = "Melissa Lachapelle",
                        OrganisationName = "RPLP",
                        Active = false
                    },
                };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDB);
            context.Setup(m => m.Repositories.Add(It.IsAny<Repository_SQLDTO>())).Callback<Repository_SQLDTO>(repositoriesDB.Add);
            DepotRepository depot = new DepotRepository(context.Object);

            Repository repository = new Repository()
            {
                Name = "testrepo",
                FullName = "Test Repository",
                OrganisationName = "test organisation"
            };

            depot.UpsertRepository(repository);

            Repository_SQLDTO? repositorySQL = repositoriesDB.FirstOrDefault(r => r.Name == "testrepo");

            Assert.NotNull(repositorySQL);
            Assert.Equal("testrepo", repositorySQL.Name);
            Assert.Equal("Test Repository", repositorySQL.FullName);
            Assert.Equal("test organisation", repositorySQL.OrganisationName);
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
                        OrganisationName = "RPLP",
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=2,
                        Name = "ikeameatbol",
                        FullName = "Jonathan Blouin",
                        OrganisationName = "RPLP",
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=3,
                        Name = "BACenComm",
                        FullName = "Melissa Lachapelle",
                        OrganisationName = "RPLP",
                        Active = false
                    },
                };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDB);
            DepotRepository depot = new DepotRepository(context.Object);

            Repository? repository = repositoriesDB.FirstOrDefault(r => r.Name == "ThPaquet").ToEntity();
            Assert.NotNull(repository);

            repository.Name = "testrepo";
            repository.FullName = "Test Repository";
            repository.OrganisationName = "test organisation";

            depot.UpsertRepository(repository);

            Repository_SQLDTO? repositorySQL = repositoriesDB.FirstOrDefault(r => r.Name == "testrepo");

            Assert.NotNull(repositorySQL);
            Assert.Equal("testrepo", repositorySQL.Name);
            Assert.Equal("Test Repository", repositorySQL.FullName);
            Assert.Equal("test organisation", repositorySQL.OrganisationName);
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
                        OrganisationName = "RPLP",
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=2,
                        Name = "ikeameatbol",
                        FullName = "Jonathan Blouin",
                        OrganisationName = "RPLP",
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=3,
                        Name = "BACenComm",
                        FullName = "Melissa Lachapelle",
                        OrganisationName = "RPLP",
                        Active = false
                    },
                };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDB);
            DepotRepository depot = new DepotRepository(context.Object);

            depot.DeleteRepository("ThPaquet");

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
                        OrganisationName = "RPLP",
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=2,
                        Name = "ikeameatbol",
                        FullName = "Jonathan Blouin",
                        OrganisationName = "RPLP",
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=3,
                        Name = "BACenComm",
                        FullName = "Melissa Lachapelle",
                        OrganisationName = "RPLP",
                        Active = false
                    },
                };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDB);
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
                        OrganisationName = "RPLP",
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=2,
                        Name = "ikeameatbol",
                        FullName = "Jonathan Blouin",
                        OrganisationName = "RPLP",
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=3,
                        Name = "BACenComm",
                        FullName = "Melissa Lachapelle",
                        OrganisationName = "RPLP",
                        Active = false
                    },
                };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDB);
            DepotRepository depot = new DepotRepository(context.Object);

            List<Repository> repositories = depot.GetRepositoriesFromOrganisationName("RPLP");

            Assert.Equal(2, repositories.Count);
           
            
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
                        OrganisationName = "RPLP",
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=2,
                        Name = "ikeameatbol",
                        FullName = "Jonathan Blouin",
                        OrganisationName = "RPLP",
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=3,
                        Name = "BACenComm",
                        FullName = "Melissa Lachapelle",
                        OrganisationName = "RPLP",
                        Active = false
                    },
                };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDB);
            DepotRepository depot = new DepotRepository(context.Object);

            depot.ReactivateRepository("BACenComm");

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
                        OrganisationName = "RPLP",
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=2,
                        Name = "ikeameatbol",
                        FullName = "Jonathan Blouin",
                        OrganisationName = "RPLP",
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=3,
                        Name = "BACenComm",
                        FullName = "Melissa Lachapelle",
                        OrganisationName = "RPLP",
                        Active = false
                    },
                };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDB);
            DepotRepository depot = new DepotRepository(context.Object);

            depot.ReactivateRepository("BACenComm");

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
                        OrganisationName = "RPLP",
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=2,
                        Name = "ikeameatbol",
                        FullName = "Jonathan Blouin",
                        OrganisationName = "RPLP",
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=3,
                        Name = "BACenComm",
                        FullName = "Melissa Lachapelle",
                        OrganisationName = "RPLP",
                        Active = false
                    },
                };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDB);
            DepotRepository depot = new DepotRepository(context.Object);

            depot.ReactivateRepository("InexistantRepo");

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
                        OrganisationName = "RPLP",
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=2,
                        Name = "ikeameatbol",
                        FullName = "Jonathan Blouin",
                        OrganisationName = "RPLP",
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=3,
                        Name = "BACenComm",
                        FullName = "Melissa Lachapelle",
                        OrganisationName = "RPLP",
                        Active = false
                    },
                };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDB);
            DepotRepository depot = new DepotRepository(context.Object);

            string repositoryName = null;

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
                        OrganisationName = "RPLP",
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=2,
                        Name = "ikeameatbol",
                        FullName = "Jonathan Blouin",
                        OrganisationName = "RPLP",
                        Active = true
                    },
                    new Repository_SQLDTO()
                    {
                        Id=3,
                        Name = "BACenComm",
                        FullName = "Melissa Lachapelle",
                        OrganisationName = "RPLP",
                        Active = false
                    },
                };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDB);
            DepotRepository depot = new DepotRepository(context.Object);

            string repositoryName = " ";

            Assert.Throws<ArgumentNullException>(() => { depot.ReactivateRepository(repositoryName); });
        }
    }
}

