using Microsoft.EntityFrameworkCore;
using RPLP.DAL.DTO.Sql;
using RPLP.DAL.SQL;
using RPLP.DAL.SQL.Depots;
using RPLP.ENTITES;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RPLP.UnitTesting.DepotTests
{
    [Collection("DatabaseTests")]
    public class TestsDepotAdministrateur
    {
        private static readonly DbContextOptions<RPLPDbContext> options = new DbContextOptionsBuilder<RPLPDbContext>()
                .UseSqlServer("Server=localhost,1434; Database=RPLP; User Id=sa; password=Cad3pend86!")
                .Options;

        private void DeleteAdministratorAndRelatedTablesContent()
        {
            using (var context = new RPLPDbContext(options))
            {
                context.Database.ExecuteSqlRaw("DELETE from Administrators;");
                context.Database.ExecuteSqlRaw("DELETE from Organisations;");
            }
        }

        private void InsertPremadeAdmins()
        {
            using (var context = new RPLPDbContext(options))
            {
                context.Administrators.AddRange(
                    new Administrator_SQLDTO
                    {
                        Username = "ThPaquet",
                        FirstName = "Thierry",
                        LastName = "Paquet",
                        Token = "token",
                        Organisations = new List<Organisation_SQLDTO>()
                        {
                            new Organisation_SQLDTO()
                            {
                                Name = "CEGEP Ste-Foy",
                                Active = true
                            }
                        },
                        Email = "ThPaquet@hotmail.com",
                        Active = true
                    },
                    new Administrator_SQLDTO
                    {
                        Username = "ikeameatbol",
                        FirstName = "Jonathan",
                        LastName = "Blouin",
                        Token = "token",
                        Email = "ikeameatbol@hotmail.com",
                        Active = true
                    },
                    new Administrator_SQLDTO
                    {
                        Username = "BACenComm",
                        FirstName = "Melissa",
                        LastName = "Lachapelle",
                        Token = "token",
                        Email = "BACenComm@hotmail.com",
                        Active = false
                    }
                );

                context.SaveChanges();
            }
        }


        [Fact]
        public void Test_GetAdministrators_CountIsGood()
        {
            this.DeleteAdministratorAndRelatedTablesContent();
            this.InsertPremadeAdmins();


            using (var context = new RPLPDbContext(options))
            {
                DepotAdministrator depot = new DepotAdministrator(context);
                List<Administrator> administrators = depot.GetAdministrators();

                Assert.NotNull(administrators);
                Assert.Equal(2, administrators.Count);
                Assert.Contains(administrators, a => a.Username == "ThPaquet");
                Assert.DoesNotContain(administrators, a => a.Username == "BACenComm");
                Assert.NotEmpty(administrators.FirstOrDefault(a => a.Username == "ThPaquet").Organisations);
            }

            this.DeleteAdministratorAndRelatedTablesContent();
        }

        [Fact]
        public void Test_GetByName()
        {
            this.DeleteAdministratorAndRelatedTablesContent();
            this.InsertPremadeAdmins();

            using (var context = new RPLPDbContext(options))
            {
                DepotAdministrator depot = new DepotAdministrator(context);
                Administrator administrator = depot.GetAdministratorByUsername("ThPaquet");

                Assert.NotNull(administrator);
                Assert.Equal("Thierry", administrator.FirstName);
                Assert.Equal("Paquet", administrator.LastName);
                Assert.Equal("token", administrator.Token);
                Assert.NotEmpty(administrator.Organisations);
            }

            this.DeleteAdministratorAndRelatedTablesContent();
        }

        [Fact]
        public void Test_GetByName_NotActive()
        {
            this.DeleteAdministratorAndRelatedTablesContent();
            this.InsertPremadeAdmins();

            using (var context = new RPLPDbContext(options))
            {
                Administrator_SQLDTO adminInContext = context.Administrators.FirstOrDefault(a => a.Username == "BACenComm" && a.Active == false);
                Assert.NotNull(adminInContext);

                DepotAdministrator depot = new DepotAdministrator(context);
                Administrator administrator = depot.GetAdministratorByUsername("BACenComm");
                Assert.Null(administrator);

            }

            this.DeleteAdministratorAndRelatedTablesContent();
        }

        [Fact]
        public void Test_GetById()
        {
            this.DeleteAdministratorAndRelatedTablesContent();
            this.InsertPremadeAdmins();

            using (var context = new RPLPDbContext(options))
            {
                Administrator_SQLDTO? admin = context.Administrators
                    .SingleOrDefaultAsync(a => a.Username == "ThPaquet")
                    .Result;

                int id = admin.Id;

                DepotAdministrator depot = new DepotAdministrator(context);
                Administrator administrator = depot.GetAdministratorById(id);

                Assert.NotNull(administrator);
                Assert.Equal("Thierry", administrator.FirstName);
                Assert.Equal("Paquet", administrator.LastName);
                Assert.Equal("token", administrator.Token);
                Assert.NotEmpty(administrator.Organisations);
            }

            this.DeleteAdministratorAndRelatedTablesContent();
        }

        [Fact]
        public void Test_GetById_NotActive()
        {
            this.DeleteAdministratorAndRelatedTablesContent();
            this.InsertPremadeAdmins();

            using (var context = new RPLPDbContext(options))
            {
                Administrator_SQLDTO? adminInContext = context.Administrators
                    .SingleOrDefault(a => a.Username == "BACenComm" && a.Active == false);
                Assert.NotNull(adminInContext);

                int id = adminInContext.Id;

                DepotAdministrator depot = new DepotAdministrator(context);
                Administrator administrator = depot.GetAdministratorById(id);

                Assert.Null(administrator);
            }

            this.DeleteAdministratorAndRelatedTablesContent();
        }

        [Fact]
        public void Test_GetByEmail()
        {
            this.DeleteAdministratorAndRelatedTablesContent();
            this.InsertPremadeAdmins();

            using (var context = new RPLPDbContext(options))
            {
                Administrator_SQLDTO? admin = context.Administrators
                    .SingleOrDefault(a => a.Email == "ThPaquet@hotmail.com");

                DepotAdministrator depot = new DepotAdministrator(context);
                Administrator administrator = depot.GetAdministratorByEmail("ThPaquet@hotmail.com");

                Assert.NotNull(administrator);
                Assert.Equal("Thierry", administrator.FirstName);
                Assert.Equal("Paquet", administrator.LastName);
                Assert.Equal("token", administrator.Token);
                Assert.NotEmpty(administrator.Organisations);
            }

            this.DeleteAdministratorAndRelatedTablesContent();
        }

        [Fact]
        public void Test_GetByEmail_NotActive()
        {
            this.DeleteAdministratorAndRelatedTablesContent();
            this.InsertPremadeAdmins();

            using (var context = new RPLPDbContext(options))
            {
                Administrator_SQLDTO? adminInContext = context.Administrators
                    .SingleOrDefault(a => a.Email == "BACenComm@hotmail.com" && a.Active == false);
                Assert.NotNull(adminInContext);

                DepotAdministrator depot = new DepotAdministrator(context);
                Administrator administrator = depot.GetAdministratorByEmail("BACenComm@hotmail.com");

                Assert.Null(administrator);
            }

            this.DeleteAdministratorAndRelatedTablesContent();
        }

        [Fact]
        public void Test_UpsertAdministrator_Inserts()
        {
            this.DeleteAdministratorAndRelatedTablesContent();

            using (var context = new RPLPDbContext(options))
            {
                DepotAdministrator depot = new DepotAdministrator(context);

                depot.UpsertAdministrator(new Administrator
                {
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Token = "token",
                    Email = "ThPaquet@hotmail.com"
                });
            }

            using (var context = new RPLPDbContext(options))
            {
                Administrator_SQLDTO admin = context.Administrators
                    .Include(a => a.Organisations)
                    .SingleOrDefault(a => a.Username == "ThPaquet");
                Assert.NotNull(admin);
                Assert.Equal("Thierry", admin.FirstName);
                Assert.Equal("Paquet", admin.LastName);
                Assert.Equal("token", admin.Token);
                Assert.Equal("ThPaquet@hotmail.com", admin.Email);
            }

            this.DeleteAdministratorAndRelatedTablesContent();
        }

        [Fact]
        public void Test_UpsertAdministrator_Updates()
        {
            this.DeleteAdministratorAndRelatedTablesContent();
            this.InsertPremadeAdmins();

            using (var context = new RPLPDbContext(options))
            {
                DepotAdministrator depot = new DepotAdministrator(context);

                Administrator_SQLDTO? admin = context.Administrators
                    .AsNoTracking()
                    .SingleOrDefault(a => a.Username == "ThPaquet");

                admin.Username = "Upserted";
                admin.FirstName = "Upserty";
                admin.LastName = "McUpserton";
                admin.Email = "Upserted@hotmail.com";
                depot.UpsertAdministrator(admin.ToEntityWithoutList());

                admin = context.Administrators
                    .AsNoTracking()
                    .SingleOrDefault(a => a.Username == "ThPaquet");
                Assert.Null(admin);

                admin = context.Administrators
                    .AsNoTracking()
                    .SingleOrDefault(a => a.Username == "Upserted");

                Assert.NotNull(admin);
                Assert.Equal("Upserty", admin.FirstName);
                Assert.Equal("McUpserton", admin.LastName);
                Assert.Equal("Upserted@hotmail.com", admin.Email);
            }

            this.DeleteAdministratorAndRelatedTablesContent();
        }

        [Fact]
        public void Test_UpsertAdministrator_ThrowUpdateDeletedAccount()
        {
            this.DeleteAdministratorAndRelatedTablesContent();
            this.InsertPremadeAdmins();

            using (var context = new RPLPDbContext(options))
            {
                DepotAdministrator depot = new DepotAdministrator(context);

                Administrator_SQLDTO? admin = context.Administrators.SingleOrDefault(a => a.Username == "BACenComm");

                Assert.Throws<ArgumentException>(
                    () =>
                    {
                        depot.UpsertAdministrator(admin.ToEntityWithoutList());
                    });
            }

            this.DeleteAdministratorAndRelatedTablesContent();
        }

        [Fact]
        public void Test_UpsertAdministrator_ThrowUsernameTaken_UsernameTakenNotActive()
        {
            this.DeleteAdministratorAndRelatedTablesContent();
            this.InsertPremadeAdmins();

            using (var context = new RPLPDbContext(options))
            {
                DepotAdministrator depot = new DepotAdministrator(context);

                Administrator_SQLDTO? admin = context.Administrators.SingleOrDefault(a => a.Username == "ikeameatbol");
                Assert.NotNull(admin);

                admin.Username = "BACenComm";

                Assert.Throws<ArgumentException>(
                    () =>
                    {
                        depot.UpsertAdministrator(admin.ToEntityWithoutList());
                    });
            }

            this.DeleteAdministratorAndRelatedTablesContent();
        }

        [Fact]
        public void Test_UpsertAdministrator_ThrowUsernameTaken_NewAdmin()
        {
            this.DeleteAdministratorAndRelatedTablesContent();
            this.InsertPremadeAdmins();

            using (var context = new RPLPDbContext(options))
            {
                DepotAdministrator depot = new DepotAdministrator(context);

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
            }

            this.DeleteAdministratorAndRelatedTablesContent();
        }

        [Fact]
        public void Test_UpsertAdministrator_ThrowEmailTaken_EmailTakenNotActive()
        {
            this.DeleteAdministratorAndRelatedTablesContent();
            this.InsertPremadeAdmins();

            using (var context = new RPLPDbContext(options))
            {

                DepotAdministrator depot = new DepotAdministrator(context);

                Administrator_SQLDTO? admin = context.Administrators.SingleOrDefault(a => a.Email == "ikeameatbol@hotmail.com");
                Assert.NotNull(admin);


                admin.Email = "BACenComm@hotmail.com";

                Assert.Throws<ArgumentException>(
                    () =>
                    {
                        depot.UpsertAdministrator(admin.ToEntityWithoutList());
                    });
            }

            this.DeleteAdministratorAndRelatedTablesContent();
        }

        [Fact]
        public void Test_UpsertAdministrator_ThrowEmailTaken_NewAdmin()
        {
            this.DeleteAdministratorAndRelatedTablesContent();
            this.InsertPremadeAdmins();

            using (var context = new RPLPDbContext(options))
            {
                DepotAdministrator depot = new DepotAdministrator(context);

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
            }

            this.DeleteAdministratorAndRelatedTablesContent();
        }

        [Fact]

        public void Test_GetAdminOrganisations()
        {

            this.DeleteAdministratorAndRelatedTablesContent();
            this.InsertPremadeAdmins();

            using (var context = new RPLPDbContext(options))
            {
                DepotAdministrator depot = new DepotAdministrator(context);

                List<Organisation> organisations = depot
                    .GetAdminOrganisations("ThPaquet");

                Assert.Equal(1, organisations.Count);
                Assert.Contains(organisations, o => o.Name == "CEGEP Ste-Foy");
            }

            this.DeleteAdministratorAndRelatedTablesContent();
        }

        [Fact]
        public void Test_JoinOrganisation()
        {
            this.DeleteAdministratorAndRelatedTablesContent();
            this.InsertPremadeAdmins();

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
                Assert.Contains(context.Administrators, a => a.Username == "ikeameatbol" && a.Active);
                depot.JoinOrganisation("ikeameatbol", "RPLP");
            }

            using (var context = new RPLPDbContext(options))
            {

                Administrator_SQLDTO? admin = context.Administrators
                    .Include(a => a.Organisations)
                    .FirstOrDefault(a => a.Username == "ikeameatbol");

                Assert.NotNull(admin);
                Assert.Equal(1, admin.Organisations.Count);
                Assert.Contains(admin.Organisations, o => o.Name == "RPLP");
            }

            this.DeleteAdministratorAndRelatedTablesContent();
        }

        [Fact]
        public void Test_LeaveOrganisation()
        {
            this.DeleteAdministratorAndRelatedTablesContent();

            Administrator_SQLDTO? admin = null;
            Organisation_SQLDTO? organisation = null;

            this.InsertPremadeAdmins();

            using (var context = new RPLPDbContext(options))
            {
                admin = context.Administrators.Include(a => a.Organisations)
                    .FirstOrDefault(a => a.Username == "ThPaquet");
                organisation = admin.Organisations.FirstOrDefault(o => o.Name == "CEGEP Ste-Foy");

                Assert.NotNull(admin);
                Assert.NotNull(organisation);

                DepotAdministrator depot = new DepotAdministrator(context);

                depot.LeaveOrganisation("ThPaquet", "CEGEP Ste-Foy");

                admin = context.Administrators.FirstOrDefault(a => a.Username == "ThPaquet");
                organisation = admin.Organisations.FirstOrDefault(o => o.Name == "CEGEP Ste-Foy");

                Assert.Null(organisation);
            }

            this.DeleteAdministratorAndRelatedTablesContent();
        }

        
    }
}