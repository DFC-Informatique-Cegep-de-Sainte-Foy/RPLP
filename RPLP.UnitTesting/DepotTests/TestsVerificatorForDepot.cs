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
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RPLP.UnitTesting.DepotTests
{
    public class TestsVerificatorForDepot
    {
        [Fact]
        public void Test_CheckUsernameTaken_Taken()
        {
            List<Student_SQLDTO> studentsBD = new List<Student_SQLDTO>()
            {
                new Student_SQLDTO()
                {
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Matricule = "1122334",
                    Active = true
                }
            };
            List<Teacher_SQLDTO> teachersDB = new List<Teacher_SQLDTO>()
            {
                new Teacher_SQLDTO()
                {
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Active = false
                }
            };
            List<Administrator_SQLDTO> administratorsBD = new List<Administrator_SQLDTO>
            {
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
                    }
            };
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsBD);
            context.Setup(x => x.Students).ReturnsDbSet(studentsBD);
            context.Setup(x => x.Teachers).ReturnsDbSet(teachersDB);
            VerificatorForDepot verificator = new VerificatorForDepot(context.Object);

            Assert.True(verificator.CheckUsernameTaken("ThPaquet"));
            Assert.True(verificator.CheckUsernameTaken("ikeameatbol"));
            Assert.True(verificator.CheckUsernameTaken("BACenComm"));
        }

        [Fact]
        public void Test_CheckUsernameTaken_NotTaken()
        {
            List<Student_SQLDTO> studentsBD = new List<Student_SQLDTO>()
            {
                new Student_SQLDTO()
                {
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Matricule = "1122334",
                    Active = true
                }
            };
            List<Teacher_SQLDTO> teachersDB = new List<Teacher_SQLDTO>()
            {
                new Teacher_SQLDTO()
                {
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Active = false
                }
            };
            List<Administrator_SQLDTO> administratorsBD = new List<Administrator_SQLDTO>
            {
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
                    }
            };
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsBD);
            context.Setup(x => x.Students).ReturnsDbSet(studentsBD);
            context.Setup(x => x.Teachers).ReturnsDbSet(teachersDB);
            VerificatorForDepot verificator = new VerificatorForDepot(context.Object);
            Assert.False(verificator.CheckUsernameTaken("PiFou86"));
        }

        [Fact]
        public void Test_CheckEmailTaken_Taken()
        {
            List<Student_SQLDTO> studentsBD = new List<Student_SQLDTO>()
            {
                new Student_SQLDTO()
                {
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Matricule = "1122334",
                    Active = true
                }
            };
            List<Teacher_SQLDTO> teachersDB = new List<Teacher_SQLDTO>()
            {
                new Teacher_SQLDTO()
                {
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Active = false
                }
            };
            List<Administrator_SQLDTO> administratorsBD = new List<Administrator_SQLDTO>
            {
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
                    }
            };
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsBD);
            context.Setup(x => x.Students).ReturnsDbSet(studentsBD);
            context.Setup(x => x.Teachers).ReturnsDbSet(teachersDB);
            VerificatorForDepot verificator = new VerificatorForDepot(context.Object);
            Assert.True(verificator.CheckEmailTaken("ThPaquet@hotmail.com"));
            Assert.True(verificator.CheckEmailTaken("ikeameatbol@hotmail.com"));
            Assert.True(verificator.CheckEmailTaken("BACenComm@hotmail.com"));
        }

        [Fact]
        public void Test_CheckEmailTaken_NotTaken()
        {
            List<Student_SQLDTO> studentsBD = new List<Student_SQLDTO>()
            {
                new Student_SQLDTO()
                {
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Matricule = "1122334",
                    Active = true
                }
            };
            List<Teacher_SQLDTO> teachersDB = new List<Teacher_SQLDTO>()
            {
                new Teacher_SQLDTO()
                {
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Active = false
                }
            };
            List<Administrator_SQLDTO> administratorsBD = new List<Administrator_SQLDTO>
            {
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
                    }
            };
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsBD);
            context.Setup(x => x.Students).ReturnsDbSet(studentsBD);
            context.Setup(x => x.Teachers).ReturnsDbSet(teachersDB);
            VerificatorForDepot verificator = new VerificatorForDepot(context.Object);
            Assert.False(verificator.CheckEmailTaken("PiFou86@hotmail.com"));
        }

        [Fact]
        public void Test_GetUserType_Administrator()
        {
            List<Student_SQLDTO> studentsBD = new List<Student_SQLDTO>()
            {
                new Student_SQLDTO()
                {
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Matricule = "1122334",
                    Active = true
                }
            };
            List<Teacher_SQLDTO> teachersDB = new List<Teacher_SQLDTO>()
            {
                new Teacher_SQLDTO()
                {
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Active = false
                }
            };
            List<Administrator_SQLDTO> administratorsBD = new List<Administrator_SQLDTO>
            {
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
                    }
            };
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsBD);
            context.Setup(x => x.Students).ReturnsDbSet(studentsBD);
            context.Setup(x => x.Teachers).ReturnsDbSet(teachersDB);
            VerificatorForDepot verificator = new VerificatorForDepot(context.Object);

            Administrator? administrator = administratorsBD.FirstOrDefault(a => a.Email == "ThPaquet@hotmail.com")?.ToEntityWithoutList();
            Assert.NotNull(administrator);
            Assert.Equal(typeof(Administrator), verificator.GetUserTypeByEmail("ThPaquet@hotmail.com"));
        }

        [Fact]
        public void Test_GetUserType_Student()
        {
            List<Student_SQLDTO> studentsBD = new List<Student_SQLDTO>()
            {
                new Student_SQLDTO()
                {
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Matricule = "1122334",
                    Active = true
                }
            };
            List<Teacher_SQLDTO> teachersDB = new List<Teacher_SQLDTO>()
            {
                new Teacher_SQLDTO()
                {
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Active = false
                }
            };
            List<Administrator_SQLDTO> administratorsBD = new List<Administrator_SQLDTO>
            {
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
                    }
            };
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsBD);
            context.Setup(x => x.Students).ReturnsDbSet(studentsBD);
            context.Setup(x => x.Teachers).ReturnsDbSet(teachersDB);
            VerificatorForDepot verificator = new VerificatorForDepot(context.Object);

            Student? student = studentsBD.FirstOrDefault(a => a.Email == "ikeameatbol@hotmail.com")?.ToEntityWithoutList();
            Assert.NotNull(student);

            Assert.Equal(typeof(Student), verificator.GetUserTypeByEmail("ikeameatbol@hotmail.com"));
        }

        //[Fact]
        //public void Test_GetUserType_Teacher()
        //{
        //    List<Student_SQLDTO> studentsBD = new List<Student_SQLDTO>()
        //    {
        //        new Student_SQLDTO()
        //        {
        //            Username = "ikeameatbol",
        //            FirstName = "Jonathan",
        //            LastName = "Blouin",
        //            Email = "ikeameatbol@hotmail.com",
        //            Matricule = "1122334",
        //            Active = true
        //        }
        //    };
        //    List<Teacher_SQLDTO> teachersDB = new List<Teacher_SQLDTO>()
        //    {
        //        new Teacher_SQLDTO()
        //        {
        //            Username = "BACenComm",
        //            FirstName = "Melissa",
        //            LastName = "Lachapelle",
        //            Email = "BACenComm@hotmail.com",
        //            Active = false
        //        }
        //    };
        //    List<Administrator_SQLDTO> administratorsBD = new List<Administrator_SQLDTO>
        //    {
        //        new Administrator_SQLDTO
        //            {
        //                Username = "ThPaquet",
        //                FirstName = "Thierry",
        //                LastName = "Paquet",
        //                Token = "token",
        //                Organisations = new List<Organisation_SQLDTO>()
        //                {
        //                    new Organisation_SQLDTO()
        //                    {
        //                        Name = "CEGEP Ste-Foy",
        //                        Active = true
        //                    }
        //                },
        //                Email = "ThPaquet@hotmail.com",
        //                Active = true
        //            }
        //    };
        //    var logMock = new Mock<IManipulationLogs>();
        //    Logging.Instance.ManipulationLog = logMock.Object;

        //    Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
        //    context.Setup(x => x.Administrators).ReturnsDbSet(administratorsBD);
        //    context.Setup(x => x.Students).ReturnsDbSet(studentsBD);
        //    context.Setup(x => x.Teachers).ReturnsDbSet(teachersDB);
        //    VerificatorForDepot verificator = new VerificatorForDepot(context.Object);

        //    Teacher? teacher = teachersDB.FirstOrDefault(a => a.Email == "BACenComm@hotmail.com")?.ToEntityWithoutList();
        //    Assert.NotNull(teacher);

        //    Assert.Equal(typeof(Teacher), verificator.GetUserTypeByEmail("BACenComm@hotmail.com"));
        //}

        [Fact]
        public void Test_GetUserType_Null()
        {
            List<Student_SQLDTO> studentsBD = new List<Student_SQLDTO>()
            {
                new Student_SQLDTO()
                {
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Matricule = "1122334",
                    Active = true
                }
            };
            List<Teacher_SQLDTO> teachersDB = new List<Teacher_SQLDTO>()
            {
                new Teacher_SQLDTO()
                {
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Active = false
                }
            };
            List<Administrator_SQLDTO> administratorsBD = new List<Administrator_SQLDTO>
            {
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
                    }
            };
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsBD);
            context.Setup(x => x.Students).ReturnsDbSet(studentsBD);
            context.Setup(x => x.Teachers).ReturnsDbSet(teachersDB);
            VerificatorForDepot verificator = new VerificatorForDepot(context.Object);

            Assert.Null(verificator.GetUserTypeByEmail("whatever@hotmail.com"));

        }
    }
}
