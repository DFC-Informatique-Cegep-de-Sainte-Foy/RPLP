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
    public class TestsDepotAdministrateur
    {
        private static readonly DbContextOptions<RPLPDbContext> options = new DbContextOptionsBuilder<RPLPDbContext>()
                .UseSqlServer("Server=localhost,1434; Database=RPLP; User Id=sa; password=Cad3pend86!")
                .Options;

        private void DeleteAdministratorTableContent()
        {
            using (var context = new RPLPDbContext(options))
            {
                context.Database.ExecuteSqlRaw("DELETE from Administrators;");
            }
        }

        private void DeleteOrganisationTableContent()
        {
            using (var context = new RPLPDbContext(options))
            {
                context.Database.ExecuteSqlRaw("DELETE from Organisations;");
            }
        }

        private void InsertAdminBACenComm()
        {
            using (var context = new RPLPDbContext(options))
            {
                context.Administrators.Add(
                    new Administrator_SQLDTO
                    {
                        Username = "BACenComm",
                        FirstName = "Melissa",
                        LastName = "Lachapelle",
                        Token = "token",
                        Active = true
                    }
                );

                context.SaveChanges();
            }
        }

        private void InsertAdmin(Administrator_SQLDTO p_admin)
        {
            using (var context = new RPLPDbContext(options))
            {
                context.Administrators.Add(p_admin);
                context.SaveChanges();
            }
        }


        [Fact]
        public void Test_GetAll_CountIsGood()
        {
            int expected = 3;
            int result = 0;

            this.DeleteAdministratorTableContent();

            using (var context = new RPLPDbContext(options))
            {
                context.Administrators.AddRange(
                    new Administrator_SQLDTO
                    {
                        Username = "ThPaquet",
                        FirstName = "Thierry",
                        LastName = "Paquet",
                        Token = "token",
                        Active = true
                    },
                    new Administrator_SQLDTO
                    {
                        Username = "ikeameatbol",
                        FirstName = "Jonathan",
                        LastName = "Blouin",
                        Token = "token",
                        Active = true
                    },
                    new Administrator_SQLDTO
                    {
                        Username = "BACenComm",
                        FirstName = "Melissa",
                        LastName = "Lachapelle",
                        Token = "token",
                        Active = true
                    }
                );

                context.SaveChanges();
            }

            using (var context = new RPLPDbContext(options))
            {
                DepotAdministrator depot = new DepotAdministrator(context);
                List<Administrator> administrators = depot.GetAdministrators();
                result = administrators.Count;

                Assert.Equal(expected, result);
            }

            this.DeleteAdministratorTableContent();
        }

        [Fact]
        public void Test_GetByName()
        {
            this.DeleteAdministratorTableContent();

            string expectedFirstname = "Melissa";
            string result = "";

            this.InsertAdminBACenComm();

            using (var context = new RPLPDbContext(options))
            {
                DepotAdministrator depot = new DepotAdministrator(context);
                Administrator administrator = depot.GetAdministratorByName("BACenComm");
                result = administrator.FirstName;

                Assert.Equal(expectedFirstname, result);
            }

            this.DeleteAdministratorTableContent();
        }

        [Fact]
        public void Test_GetById()
        {
            this.DeleteAdministratorTableContent();

            int id = 0;
            string expectedUsername = "BACenComm";
            string result = "";

            this.InsertAdminBACenComm();

            using (var context = new RPLPDbContext(options))
            {
                Administrator_SQLDTO? admin = context.Administrators
                    .SingleOrDefaultAsync(a => a.Username == "BACenComm")
                    .Result;

                id = admin.Id;
            }

            using (var context = new RPLPDbContext(options))
            {
                DepotAdministrator depot = new DepotAdministrator(context);
                Administrator administrator = depot.GetAdministratorById(id);
                result = administrator.Username;

                Assert.Equal(expectedUsername, result);
            }

            this.DeleteAdministratorTableContent();
        }

        [Fact]
        public void Test_UpsertAdministrator_Inserts()
        {
            this.DeleteAdministratorTableContent();

            bool expected = true;
            bool result = false;

            using (var context = new RPLPDbContext(options))
            {
                DepotAdministrator depot = new DepotAdministrator(context);

                depot.UpsertAdministrator(new Administrator
                {
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Token = "token"
                });

                Administrator_SQLDTO? admin = context.Administrators
                    .SingleOrDefaultAsync(a => a.Username == "BACenComm")
                    .Result;

                result = admin != null;

                Assert.Equal(expected, result);
            }

            this.DeleteAdministratorTableContent();
        }

        [Fact]
        public void Test_UpsertAdministrator_Updates()
        {
            this.DeleteAdministratorTableContent();

            bool expected = true;
            bool result = false;

            this.InsertAdminBACenComm();

            using (var context = new RPLPDbContext(options))
            {
                DepotAdministrator depot = new DepotAdministrator(context);

                Administrator_SQLDTO? admin = context.Administrators
                    .SingleOrDefaultAsync(a => a.Username == "BACenComm")
                    .Result;

                admin.Username = "LaMachine";
                depot.UpsertAdministrator(admin.ToEntityWitouthOrganisations());

                Administrator_SQLDTO? adminModifie = context.Administrators
                    .SingleOrDefaultAsync(a => a.Username == "LaMachine")
                    .Result;

                admin = context.Administrators
                    .SingleOrDefaultAsync(a => a.Username == "BACenComm")
                    .Result;


                result = adminModifie != null && admin == null;

                Assert.Equal(expected, result);
            }

            this.DeleteAdministratorTableContent();
        }

        [Fact]

        public void Test_GetAdminOrganisations()
        {

            this.DeleteAdministratorTableContent();
            this.DeleteOrganisationTableContent();
            int expected = 2;
            int result = 0;


            this.InsertAdmin(new Administrator_SQLDTO
            {
                Username = "BACenComm",
                FirstName = "Melissa",
                LastName = "Lachapelle",
                Token = "token",
                Organisations = new List<Organisation_SQLDTO>
                {
                    new Organisation_SQLDTO
                    {
                        Name = "RPLP"
                    },

                    new Organisation_SQLDTO
                    {
                        Name = "Test"
                    }
                },
                Active = true
            });

            using (var context = new RPLPDbContext(options))
            {
                DepotAdministrator depot = new DepotAdministrator(context);

                List<Organisation> organisations = depot
                    .GetAdminOrganisations("BACenComm")
                    .ToList();

                result = organisations.Count;

                Assert.Equal(expected, result);
                Assert.NotNull(organisations.FirstOrDefault(o => o.Name == "RPLP"));
                Assert.NotNull(organisations.FirstOrDefault(o => o.Name == "Test"));
            }

            this.DeleteAdministratorTableContent();
            this.DeleteOrganisationTableContent();
        }

        [Fact]
        public void Test_JoinOrganisation()
        {
            this.DeleteAdministratorTableContent();
            this.DeleteOrganisationTableContent();
            string expected = "RPLP";
            string result = "";

            this.InsertAdminBACenComm();

            using (var context = new RPLPDbContext(options))
            {
                context.Organisations.Add(new Organisation_SQLDTO
                {
                    Name = "RPLP",
                    Active = true
                });

                context.SaveChanges();
            }

            using (var context = new RPLPDbContext(options))
            {
                DepotAdministrator depot = new DepotAdministrator(context);

                depot.JoinOrganisation("BACenComm", "RPLP");

                Administrator_SQLDTO? admin = context.Administrators.FirstOrDefault(a => a.Username == "BACenComm");

                Assert.Equal(1, admin.Organisations.Count);
                Organisation? orgAdmin = admin.Organisations.FirstOrDefault().ToEntityWithoutAdministrators();


                result = orgAdmin.Name;
            }

            Assert.Equal(expected, result);
            this.DeleteOrganisationTableContent();
            this.DeleteAdministratorTableContent();
        }

        [Fact]
        public void Test_LeaveOrganisation()
        {
            this.DeleteAdministratorTableContent();
            this.DeleteOrganisationTableContent();

            Administrator_SQLDTO? admin = null;
            Organisation_SQLDTO? organisation = null;

            this.InsertAdmin(
            new Administrator_SQLDTO
            {
                Username = "BACenComm",
                FirstName = "Melissa",
                LastName = "Lachapelle",
                Token = "token",
                Active = true,
                Organisations =
                {
                    new Organisation_SQLDTO
                    {
                        Name = "RPLP",
                        Active = true
                    }
                }
            });

            using (var context = new RPLPDbContext(options))
            {
                admin = context.Administrators.Include(a => a.Organisations)
                    .FirstOrDefault(a => a.Username == "BACenComm");
                organisation = admin.Organisations.FirstOrDefault(o => o.Name == "RPLP");

                Assert.NotNull(admin);
                Assert.NotNull(organisation);

                DepotAdministrator depot = new DepotAdministrator(context);

                depot.LeaveOrganisation("BACenComm", "RPLP");

                admin = context.Administrators.FirstOrDefault(a => a.Username == "BACenComm");
                organisation = admin.Organisations.FirstOrDefault(o => o.Name == "RPLP");

                Assert.Null(organisation);
            }

            this.DeleteAdministratorTableContent();
            this.DeleteOrganisationTableContent();
        }
    }
}