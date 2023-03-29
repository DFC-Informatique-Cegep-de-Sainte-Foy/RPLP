using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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

namespace RPLP.UnitTesting.DepotTests
{
    public class TestsDepotAdministrateur
    {

        [Fact]
        public void Test_GetAdministrators_CountIsGood()
        {
            var administrators1 = new List<Administrator_SQLDTO>
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
            }.AsQueryable();

            Mock<DbSet<Administrator_SQLDTO>> mock = new Mock<DbSet<Administrator_SQLDTO>>();
            mock.As<IQueryable<Administrator_SQLDTO>>().Setup(m => m.GetEnumerator()).Returns(() => administrators1.GetEnumerator());

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(c => c.Administrators).Returns(mock.Object);

            DepotAdministrator depot = new DepotAdministrator(context.Object);
            List<Administrator> administrators = depot.GetAdministrators();

            Assert.NotNull(administrators);
            Assert.Equal(2, administrators.Count);
            Assert.Contains(administrators, a => a.Username == "ThPaquet");
            Assert.DoesNotContain(administrators, a => a.Username == "BACenComm");
            Assert.NotEmpty(administrators.FirstOrDefault(a => a.Username == "ThPaquet").Organisations);
        }

        //    [Fact]
        //    public void Test_GetDeactivatedAdministrators()
        //    {



        //        Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();

        //        {
        //            DepotAdministrator depot = new DepotAdministrator(context);
        //            List<Administrator> administrators = depot.GetDeactivatedAdministrators();

        //            Assert.NotNull(administrators);
        //            Assert.Equal(1, administrators.Count);
        //            Assert.Contains(administrators, a => a.Username == "BACenComm");
        //        }


        //    }

        //    [Fact]
        //    public void Test_GetByName()
        //    {

        //        Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();


        //        {
        //            DepotAdministrator depot = new DepotAdministrator(context);
        //            Administrator administrator = depot.GetAdministratorByUsername("ThPaquet");

        //            Assert.NotNull(administrator);
        //            Assert.Equal("Thierry", administrator.FirstName);
        //            Assert.Equal("Paquet", administrator.LastName);
        //            Assert.Equal("token", administrator.Token);
        //            Assert.NotEmpty(administrator.Organisations);
        //        }


        //    }

        //    [Fact]
        //    public void Test_GetByName_NotActive()
        //    {


        //        Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();

        //        {
        //            Administrator_SQLDTO adminInContext = context.Administrators.FirstOrDefault(a => a.Username == "BACenComm" && a.Active == false);
        //            Assert.NotNull(adminInContext);

        //            DepotAdministrator depot = new DepotAdministrator(context);
        //            Administrator administrator = depot.GetAdministratorByUsername("BACenComm");
        //            Assert.Null(administrator);

        //        }


        //    }

        //    [Fact]
        //    public void Test_GetById()
        //    {


        //        Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();

        //        {
        //            Administrator_SQLDTO? admin = context.Administrators
        //                .SingleOrDefaultAsync(a => a.Username == "ThPaquet")
        //                .Result;

        //            int id = admin.Id;

        //            DepotAdministrator depot = new DepotAdministrator(context);
        //            Administrator administrator = depot.GetAdministratorById(id);

        //            Assert.NotNull(administrator);
        //            Assert.Equal("Thierry", administrator.FirstName);
        //            Assert.Equal("Paquet", administrator.LastName);
        //            Assert.Equal("token", administrator.Token);
        //            Assert.NotEmpty(administrator.Organisations);
        //        }


        //    }

        //    [Fact]
        //    public void Test_GetById_NotActive()
        //    {



        //        Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
        //        {
        //            Administrator_SQLDTO? adminInContext = context.Administrators
        //                .SingleOrDefault(a => a.Username == "BACenComm" && a.Active == false);
        //            Assert.NotNull(adminInContext);

        //            int id = adminInContext.Id;

        //            DepotAdministrator depot = new DepotAdministrator(context);
        //            Administrator administrator = depot.GetAdministratorById(id);

        //            Assert.Null(administrator);
        //        }


        //    }

        //    [Fact]
        //    public void Test_GetByEmail()
        //    {

        //        Mock<RPLPDbContext> context = new Mock<RPLPDbContext>(); Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();


        //        {
        //            Administrator_SQLDTO? admin = context.Administrators
        //                .SingleOrDefault(a => a.Email == "ThPaquet@hotmail.com");

        //            DepotAdministrator depot = new DepotAdministrator(context);
        //            Administrator administrator = depot.GetAdministratorByEmail("ThPaquet@hotmail.com");

        //            Assert.NotNull(administrator);
        //            Assert.Equal("Thierry", administrator.FirstName);
        //            Assert.Equal("Paquet", administrator.LastName);
        //            Assert.Equal("token", administrator.Token);
        //            Assert.NotEmpty(administrator.Organisations);
        //        }


        //    }

        //    [Fact]
        //    public void Test_GetByEmail_NotActive()
        //    {


        //        Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();

        //        {
        //            Administrator_SQLDTO? adminInContext = context.Administrators
        //                .SingleOrDefault(a => a.Email == "BACenComm@hotmail.com" && a.Active == false);
        //            Assert.NotNull(adminInContext);

        //            DepotAdministrator depot = new DepotAdministrator(context);
        //            Administrator administrator = depot.GetAdministratorByEmail("BACenComm@hotmail.com");

        //            Assert.Null(administrator);
        //        }


        //    }

        //    [Fact]
        //    public void Test_UpsertAdministrator_Inserts()
        //    {
        //        Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();

        //        {
        //            DepotAdministrator depot = new DepotAdministrator(context);

        //            depot.UpsertAdministrator(new Administrator
        //            {
        //                Username = "ThPaquet",
        //                FirstName = "Thierry",
        //                LastName = "Paquet",
        //                Token = "token",
        //                Email = "ThPaquet@hotmail.com"
        //            });
        //        }


        //        {
        //            Administrator_SQLDTO admin = context.Administrators
        //                .Include(a => a.Organisations)
        //                .SingleOrDefault(a => a.Username == "ThPaquet");
        //            Assert.NotNull(admin);
        //            Assert.Equal("Thierry", admin.FirstName);
        //            Assert.Equal("Paquet", admin.LastName);
        //            Assert.Equal("token", admin.Token);
        //            Assert.Equal("ThPaquet@hotmail.com", admin.Email);
        //        }


        //    }

        //    [Fact]
        //    public void Test_UpsertAdministrator_Updates()
        //    {


        //        Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();

        //        {
        //            DepotAdministrator depot = new DepotAdministrator(context);

        //            Administrator_SQLDTO? admin = context.Administrators
        //                .AsNoTracking()
        //                .SingleOrDefault(a => a.Username == "ThPaquet");

        //            admin.Username = "Upserted";
        //            admin.FirstName = "Upserty";
        //            admin.LastName = "McUpserton";
        //            admin.Email = "Upserted@hotmail.com";
        //            depot.UpsertAdministrator(admin.ToEntityWithoutList());

        //            admin = context.Administrators
        //                .AsNoTracking()
        //                .SingleOrDefault(a => a.Username == "ThPaquet");
        //            Assert.Null(admin);

        //            admin = context.Administrators
        //                .AsNoTracking()
        //                .SingleOrDefault(a => a.Username == "Upserted");

        //            Assert.NotNull(admin);
        //            Assert.Equal("Upserty", admin.FirstName);
        //            Assert.Equal("McUpserton", admin.LastName);
        //            Assert.Equal("Upserted@hotmail.com", admin.Email);
        //        }


        //    }

        //    [Fact]
        //    public void Test_UpsertAdministrator_ThrowUpdateDeletedAccount()
        //    {


        //        Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();

        //        {
        //            DepotAdministrator depot = new DepotAdministrator(context);

        //            Administrator_SQLDTO? admin = context.Administrators.SingleOrDefault(a => a.Username == "BACenComm");

        //            Assert.Throws<ArgumentException>(
        //                () =>
        //                {
        //                    depot.UpsertAdministrator(admin.ToEntityWithoutList());
        //                });
        //        }


        //    }

        //    [Fact]
        //    public void Test_UpsertAdministrator_ThrowUsernameTaken_UsernameTakenNotActive()
        //    {


        //        Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();

        //        {
        //            DepotAdministrator depot = new DepotAdministrator(context);

        //            Administrator_SQLDTO? admin = context.Administrators.SingleOrDefault(a => a.Username == "ikeameatbol");
        //            Assert.NotNull(admin);

        //            admin.Username = "BACenComm";

        //            Assert.Throws<ArgumentException>(
        //                () =>
        //                {
        //                    depot.UpsertAdministrator(admin.ToEntityWithoutList());
        //                });
        //        }


        //    }

        //    [Fact]
        //    public void Test_UpsertAdministrator_ThrowUsernameTaken_NewAdmin()
        //    {


        //        Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();

        //        {
        //            DepotAdministrator depot = new DepotAdministrator(context);

        //            Administrator_SQLDTO admin = new Administrator_SQLDTO()
        //            {
        //                Username = "ThPaquet",
        //                FirstName = "Thierry",
        //                LastName = "Paquet",
        //                Token = "token",
        //                Organisations = new List<Organisation_SQLDTO>(),
        //                Email = "swerve@hotmail.com",
        //                Active = true
        //            };

        //            Assert.Throws<ArgumentException>(
        //                () =>
        //                {
        //                    depot.UpsertAdministrator(admin.ToEntityWithoutList());
        //                });
        //        }


        //    }

        //    [Fact]
        //    public void Test_UpsertAdministrator_ThrowEmailTaken_EmailTakenNotActive()
        //    {

        //        Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();


        //        {

        //            DepotAdministrator depot = new DepotAdministrator(context);

        //            Administrator_SQLDTO? admin = context.Administrators.SingleOrDefault(a => a.Email == "ikeameatbol@hotmail.com");
        //            Assert.NotNull(admin);


        //            admin.Email = "BACenComm@hotmail.com";

        //            Assert.Throws<ArgumentException>(
        //                () =>
        //                {
        //                    depot.UpsertAdministrator(admin.ToEntityWithoutList());
        //                });
        //        }


        //    }

        //    [Fact]
        //    public void Test_UpsertAdministrator_ThrowEmailTaken_NewAdmin()
        //    {


        //        Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();

        //        {
        //            DepotAdministrator depot = new DepotAdministrator(context);

        //            Administrator_SQLDTO admin = new Administrator_SQLDTO()
        //            {
        //                Username = "Swerve",
        //                FirstName = "Thierry",
        //                LastName = "Paquet",
        //                Token = "token",
        //                Organisations = new List<Organisation_SQLDTO>(),
        //                Email = "ThPaquet@hotmail.com",
        //                Active = true
        //            };

        //            Assert.Throws<ArgumentException>(
        //                () =>
        //                {
        //                    depot.UpsertAdministrator(admin.ToEntityWithoutList());
        //                });
        //        }


        //    }

        //    [Fact]

        //    public void Test_GetAdminOrganisations()
        //    {


        //        Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();


        //        {
        //            DepotAdministrator depot = new DepotAdministrator(context);

        //            List<Organisation> organisations = depot
        //                .GetAdminOrganisations("ThPaquet");

        //            Assert.Equal(1, organisations.Count);
        //            Assert.Contains(organisations, o => o.Name == "CEGEP Ste-Foy");
        //        }


        //    }

        //    [Fact]
        //    public void Test_JoinOrganisation()
        //    {


        //        Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();

        //        {
        //            context.Organisations.Add(new Organisation_SQLDTO
        //            {
        //                Name = "RPLP",
        //                Active = true
        //            });

        //            context.SaveChanges();
        //        }


        //        {
        //            DepotAdministrator depot = new DepotAdministrator(context);
        //            Assert.Contains(context.Administrators, a => a.Username == "ikeameatbol" && a.Active);
        //            depot.JoinOrganisation("ikeameatbol", "RPLP");
        //        }


        //        {

        //            Administrator_SQLDTO? admin = context.Administrators
        //                .Include(a => a.Organisations)
        //                .FirstOrDefault(a => a.Username == "ikeameatbol");

        //            Assert.NotNull(admin);
        //            Assert.Equal(1, admin.Organisations.Count);
        //            Assert.Contains(admin.Organisations, o => o.Name == "RPLP");
        //        }


        //    }

        //    [Fact]
        //    public void Test_LeaveOrganisation()
        //    {
        //        Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();

        //        Administrator_SQLDTO? admin = null;
        //        Organisation_SQLDTO? organisation = null;




        //        {
        //            admin = context.Administrators.Include(a => a.Organisations)
        //                .FirstOrDefault(a => a.Username == "ThPaquet");
        //            organisation = admin.Organisations.FirstOrDefault(o => o.Name == "CEGEP Ste-Foy");

        //            Assert.NotNull(admin);
        //            Assert.NotNull(organisation);

        //            DepotAdministrator depot = new DepotAdministrator(context);

        //            depot.LeaveOrganisation("ThPaquet", "CEGEP Ste-Foy");

        //            admin = context.Administrators.FirstOrDefault(a => a.Username == "ThPaquet");
        //            organisation = admin.Organisations.FirstOrDefault(o => o.Name == "CEGEP Ste-Foy");

        //            Assert.Null(organisation);
        //        }


        //    }

        //    [Fact]
        //    public void Test_DeleteAdministrator()
        //    {

        //        Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();


        //        {
        //            Assert.True(context.Administrators.Any(a => a.Username == "ikeameatbol" && a.Active));

        //            DepotAdministrator depot = new DepotAdministrator(context);

        //            depot.DeleteAdministrator("ikeameatbol");
        //        }


        //        {
        //            Assert.True(context.Administrators.Any(a => a.Username == "ikeameatbol" && !a.Active));
        //        }


        //    }

        //    [Fact]
        //    public void Test_ReactivateAdministrator()
        //    {


        //        Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();

        //        {
        //            Assert.True(context.Administrators.Any(a => a.Username == "BACenComm" && !a.Active));

        //            DepotAdministrator depot = new DepotAdministrator(context);

        //            depot.ReactivateAdministrator("BACenComm");
        //        }


        //        {
        //            Assert.True(context.Administrators.Any(a => a.Username == "BACenComm" && a.Active));
        //        }


        //    }
    }
}