using Microsoft.EntityFrameworkCore;
using RPLP.DAL.DTO.Sql;
using RPLP.DAL.SQL;
using RPLP.DAL.SQL.Depots;
using RPLP.ENTITES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RPLP.UnitTesting.DepotTests
{
    [Collection("DatabaseTests")]
    public class TestsVerificatorForDepot
    {
        private static readonly DbContextOptions<RPLPDbContext> options = new DbContextOptionsBuilder<RPLPDbContext>()
            .UseSqlServer("Server=localhost,1434; Database=RPLP; User Id=sa; password=Cad3pend86!")
            //.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .Options;

        private void InsertPremades()
        {
            using (var context = new RPLPDbContext(options))
            {
                context.Administrators.Add(
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
                    });
                context.Students.Add(
                    new Student_SQLDTO
                    {
                        Username = "ikeameatbol",
                        FirstName = "Jonathan",
                        LastName = "Blouin",
                        Email = "ikeameatbol@hotmail.com",
                        Active = true
                    });
                context.Teachers.Add(
                    new Teacher_SQLDTO
                    {
                        Username = "BACenComm",
                        FirstName = "Melissa",
                        LastName = "Lachapelle",
                        Email = "BACenComm@hotmail.com",
                        Active = true
                    });

                context.SaveChanges();
            }
        }

        private void DeleteRelatedTablesContent()
        {
            using (var context = new RPLPDbContext(options))
            {
                context.Database.ExecuteSqlRaw("DELETE from Administrators;");
                context.Database.ExecuteSqlRaw("DELETE from Students;");
                context.Database.ExecuteSqlRaw("DELETE from Teachers;");
            }
        }

        [Fact]
        public void Test_CheckUsernameTaken_Taken()
        {
            this.DeleteRelatedTablesContent();
            this.InsertPremades();

            using (var context = new RPLPDbContext(options))
            {
                VerificatorForDepot verificator = new VerificatorForDepot(context);
                Assert.True(verificator.CheckUsernameTaken("ThPaquet"));
                Assert.True(verificator.CheckUsernameTaken("ikeameatbol"));
                Assert.True(verificator.CheckUsernameTaken("BACenComm"));
            }

            this.DeleteRelatedTablesContent();
        }

        [Fact]
        public void Test_CheckUsernameTaken_NotTaken()
        {
            this.DeleteRelatedTablesContent();
            this.InsertPremades();

            using (var context = new RPLPDbContext(options))
            {
                VerificatorForDepot verificator = new VerificatorForDepot(context);
                Assert.False(verificator.CheckUsernameTaken("PiFou86"));
            }

            this.DeleteRelatedTablesContent();
        }

        [Fact]
        public void Test_CheckEmailTaken_Taken()
        {
            this.DeleteRelatedTablesContent();
            this.InsertPremades();

            using (var context = new RPLPDbContext(options))
            {
                VerificatorForDepot verificator = new VerificatorForDepot(context);
                Assert.True(verificator.CheckEmailTaken("ThPaquet@hotmail.com"));
                Assert.True(verificator.CheckEmailTaken("ikeameatbol@hotmail.com"));
                Assert.True(verificator.CheckEmailTaken("BACenComm@hotmail.com"));
            }

            this.DeleteRelatedTablesContent();
        }

        [Fact]
        public void Test_CheckEmailTaken_NotTaken()
        {
            this.DeleteRelatedTablesContent();
            this.InsertPremades();

            using (var context = new RPLPDbContext(options))
            {
                VerificatorForDepot verificator = new VerificatorForDepot(context);
                Assert.False(verificator.CheckEmailTaken("PiFou86@hotmail.com"));
            }

            this.DeleteRelatedTablesContent();
        }

        [Fact]
        public void Test_GetUserType_Administrator()
        {
            this.DeleteRelatedTablesContent();
            this.InsertPremades();

            using (var context = new RPLPDbContext(options))
            {
                VerificatorForDepot verificator = new VerificatorForDepot(context);
                Administrator? administrator = context.Administrators.FirstOrDefault(a => a.Email == "ThPaquet@hotmail.com")?.ToEntityWithoutList();
                Assert.NotNull(administrator);


                Assert.Equal(typeof(Administrator), verificator.GetUserTypeByEmail("ThPaquet@hotmail.com"));
            }

            this.DeleteRelatedTablesContent();
        }

        [Fact]
        public void Test_GetUserType_Student()
        {
            this.DeleteRelatedTablesContent();
            this.InsertPremades();

            using (var context = new RPLPDbContext(options))
            {
                VerificatorForDepot verificator = new VerificatorForDepot(context);
                Student? student = context.Students.FirstOrDefault(a => a.Email == "ikeameatbol@hotmail.com")?.ToEntityWithoutList();
                Assert.NotNull(student);


                Assert.Equal(typeof(Student), verificator.GetUserTypeByEmail("ikeameatbol@hotmail.com"));
            }

            this.DeleteRelatedTablesContent();
        }

        [Fact]
        public void Test_GetUserType_Teacher()
        {
            this.DeleteRelatedTablesContent();
            this.InsertPremades();

            using (var context = new RPLPDbContext(options))
            {
                VerificatorForDepot verificator = new VerificatorForDepot(context);
                Teacher? teacher = context.Teachers.FirstOrDefault(a => a.Email == "BACenComm@hotmail.com")?.ToEntityWithoutList();
                Assert.NotNull(teacher);


                Assert.Equal(typeof(Teacher), verificator.GetUserTypeByEmail("BACenComm@hotmail.com"));
            }

            this.DeleteRelatedTablesContent();
        }

        [Fact]
        public void Test_GetUserType_Null()
        {
            this.DeleteRelatedTablesContent();

            using (var context = new RPLPDbContext(options))
            {
                VerificatorForDepot verificator = new VerificatorForDepot(context);
                Assert.Null(verificator.GetUserTypeByEmail("whatever@hotmail.com"));
            }
        }
    }
}
