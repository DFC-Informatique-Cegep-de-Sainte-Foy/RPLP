using Microsoft.EntityFrameworkCore;
using RPLP.DAL.DTO.Sql;
using RPLP.DAL.SQL;
using RPLP.DAL.SQL.Depots;
using RPLP.ENTITES;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RPLP.UnitTesting.DepotTests
{
    [Collection("DatabaseTests")]
    public class TestsDepotRepository
    {
        private static readonly DbContextOptions<RPLPDbContext> options = new DbContextOptionsBuilder<RPLPDbContext>()
                .UseSqlServer("Server=localhost,1434; Database=RPLP; User Id=sa; password=Cad3pend86!")
                .Options;

        private void DeleteRepositoryTableContent()
        {
            using (var context = new RPLPDbContext(options))
            {
                context.Database.ExecuteSqlRaw("DELETE from Repositories;");
            }
        }

        private void InsertRepository(Repository_SQLDTO p_repository)
        {
            using (var context = new RPLPDbContext(options))
            {
                context.Repositories.Add(p_repository);
                context.SaveChanges();
            }
        }

        private void InsertPremadeRepositories()
        {
            List<Repository_SQLDTO> repositories = new List<Repository_SQLDTO>()
            {
                new Repository_SQLDTO()
                {
                    Name = "ThPaquet",
                    FullName = "Thierry Paquet",
                    OrganisationName = "RPLP",
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Name = "ikeameatbol",
                    FullName = "Jonathan Blouin",
                    OrganisationName = "RPLP",
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Name = "BACenComm",
                    FullName = "Melissa Lachapelle",
                    OrganisationName = "RPLP",
                    Active = false
                },
            };

            using (var context = new RPLPDbContext(options))
            {
                context.Repositories.AddRange(repositories);
                context.SaveChanges();
            }
        }

        [Fact]
        public void Test_GetRepositoryById()
        {
            this.DeleteRepositoryTableContent();
            this.InsertPremadeRepositories();

            using (var context = new RPLPDbContext(options))
            {
                Repository_SQLDTO? repositoryThPaquet = context.Repositories.FirstOrDefault(r => r.Name == "ThPaquet");
                Assert.NotNull(repositoryThPaquet);

                int repositoryId = repositoryThPaquet.Id;

                DepotRepository depotRepository = new DepotRepository(context);

                Repository repository = depotRepository.GetRepositoryById(repositoryId);

                Assert.NotNull(repository);
            }

            this.DeleteRepositoryTableContent();
        }

        [Fact]
        public void Test_GetRepositoryById_NotActive()
        {
            this.DeleteRepositoryTableContent();
            this.InsertPremadeRepositories();

            using (var context = new RPLPDbContext(options))
            {
                Repository_SQLDTO? repositoryBACenComm = context.Repositories.FirstOrDefault(r => r.Name == "BACenComm");
                Assert.NotNull(repositoryBACenComm);

                int repositoryId = repositoryBACenComm.Id;

                DepotRepository depotRepository = new DepotRepository(context);

                Repository repository = depotRepository.GetRepositoryById(repositoryId);

                Assert.Null(repository);
            }

            this.DeleteRepositoryTableContent();
        }

        [Fact]
        public void Test_GetRepositoryByName()
        {
            this.DeleteRepositoryTableContent();
            this.InsertPremadeRepositories();

            using (var context = new RPLPDbContext(options))
            {
                DepotRepository depotRepository = new DepotRepository(context);

                Repository repository = depotRepository.GetRepositoryByName("ThPaquet");

                Assert.NotNull(repository);
            }

            this.DeleteRepositoryTableContent();
        }

        [Fact]
        public void Test_GetRepositoryByName_NotActive()
        {
            this.DeleteRepositoryTableContent();
            this.InsertPremadeRepositories();

            using (var context = new RPLPDbContext(options))
            {
                DepotRepository depotRepository = new DepotRepository(context);

                Repository repository = depotRepository.GetRepositoryByName("BACenComm");

                Assert.Null(repository);
            }

            this.DeleteRepositoryTableContent();
        }

        [Fact]
        public void Test_UpsertRepository_Inserts()
        {
            this.DeleteRepositoryTableContent();

            using (var context = new RPLPDbContext(options))
            {
                Repository repository = new Repository()
                {
                    Name = "testrepo",
                    FullName = "Test Repository",
                    OrganisationName = "test organisation"
                };

                DepotRepository depot = new DepotRepository(context);

                depot.UpsertRepository(repository);
            };

            using (var context = new RPLPDbContext(options))
            {
                Repository_SQLDTO? repository = context.Repositories.FirstOrDefault(r => r.Name == "testrepo");

                Assert.NotNull(repository);
                Assert.Equal("testrepo", repository.Name);
                Assert.Equal("Test Repository", repository.FullName);
                Assert.Equal("test organisation", repository.OrganisationName);
                Assert.True(repository.Active);
            }

            this.DeleteRepositoryTableContent();
        }

        [Fact]
        public void Test_UpsertRepository_Updates()
        {
            this.DeleteRepositoryTableContent();
            this.InsertPremadeRepositories();

            using (var context = new RPLPDbContext(options))
            {
                Repository_SQLDTO repository = context.Repositories.FirstOrDefault(r => r.Name == "ThPaquet");
                Assert.NotNull(repository);

                repository.Name = "testrepo";
                repository.FullName = "Test Repository";
                repository.OrganisationName = "test organisation";


                DepotRepository depot = new DepotRepository(context);

                depot.UpsertRepository(repository.ToEntity());
            };

            using (var context = new RPLPDbContext(options))
            {
                Repository_SQLDTO? repository = context.Repositories.FirstOrDefault(r => r.Name == "testrepo");

                Assert.NotNull(repository);
                Assert.Equal("testrepo", repository.Name);
                Assert.Equal("Test Repository", repository.FullName);
                Assert.Equal("test organisation", repository.OrganisationName);
                Assert.True(repository.Active);
            }

            this.DeleteRepositoryTableContent();
        }

        [Fact]
        public void Test_DeleteRepository()
        {
            this.DeleteRepositoryTableContent();
            this.InsertPremadeRepositories();

            using (var context = new RPLPDbContext(options))
            {
                DepotRepository depot = new DepotRepository(context);
                depot.DeleteRepository("ThPaquet");
            };

            using (var context = new RPLPDbContext(options))
            {
                Repository_SQLDTO? repository = context.Repositories.FirstOrDefault(r => r.Name == "ThPaquet" && r.Active);
                Assert.Null(repository);
            };

            this.DeleteRepositoryTableContent();
        }
    }
}
