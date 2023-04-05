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
    public class TestsDepotStudent
    {
        [Fact]
        public void Test_GetStudents()
        {
            List<Student_SQLDTO> studentsBD = new List<Student_SQLDTO>()
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

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Students).ReturnsDbSet(studentsBD);
            DepotStudent depot = new DepotStudent(context.Object);

            List<Student> students = depot.GetStudents();

            Assert.DoesNotContain(students, s => s.Username == "BACenComm");
            Assert.Contains(students, s => s.Username == "ThPaquet");
            Assert.Contains(students, s => s.Username == "ikeameatbol");
            Assert.Equal(3, students.FirstOrDefault(s => s.Username == "ThPaquet").Classes.Count);
           
            
        }

        [Fact]
        public void Test_GetDeactivatedStudents()
        {
            List<Student_SQLDTO> studentsBD = new List<Student_SQLDTO>()
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

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Students).ReturnsDbSet(studentsBD);
            DepotStudent depot = new DepotStudent(context.Object);

            List<Student> students = depot.GetDeactivatedStudents();

            Assert.Contains(students, s => s.Username == "BACenComm");
            Assert.DoesNotContain(students, s => s.Username == "ThPaquet");
            Assert.DoesNotContain(students, s => s.Username == "ikeameatbol");
           
            
        }

        [Fact]
        public void Test_GetStudentById()
        {
            List<Student_SQLDTO> studentsBD = new List<Student_SQLDTO>()
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

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Students).ReturnsDbSet(studentsBD);
            DepotStudent depot = new DepotStudent(context.Object);

            Student_SQLDTO? studentThPaquet = studentsBD.FirstOrDefault(s => s.Username == "ThPaquet");
            Assert.NotNull(studentThPaquet);

            int thPaquetId = studentThPaquet.Id;
    
            Student student = depot.GetStudentById(thPaquetId);

            Assert.NotNull(student);
            Assert.Equal("ThPaquet", student.Username);
            Assert.Equal("Thierry", student.FirstName);
            Assert.Equal("Paquet", student.LastName);
            Assert.Equal(3, student.Classes.Count);
           
            
        }

        [Fact]
        public void Test_GetStudentById_NotActive()
        {
            List<Student_SQLDTO> studentsBD = new List<Student_SQLDTO>()
            {
                new Student_SQLDTO()
                {
                    Id= 1,
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
                    Id = 2,
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Matricule = "1122334",
                    Active = true
                },
                new Student_SQLDTO()
                {
                    Id= 3,
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Matricule = "1324354",
                    Active = false
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Students).ReturnsDbSet(studentsBD);
            DepotStudent depot = new DepotStudent(context.Object);

            Student_SQLDTO? studentBACenComm = studentsBD.FirstOrDefault(s => s.Username == "BACenComm");
            Assert.NotNull(studentBACenComm);

            int BACenCommId = studentBACenComm.Id;

            Student student = depot.GetStudentById(BACenCommId);

            Assert.Null(student);
           
            
        }

        [Fact]
        public void Test_GetStudentByUsername()
        {
            List<Student_SQLDTO> studentsBD = new List<Student_SQLDTO>()
            {
                new Student_SQLDTO()
                {Id = 1,
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
                {Id = 2,
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Matricule = "1122334",
                    Active = true
                },
                new Student_SQLDTO()
                {Id = 3,
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Matricule = "1324354",
                    Active = false
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Students).ReturnsDbSet(studentsBD);
            DepotStudent depot = new DepotStudent(context.Object);

            Student student = depot.GetStudentByUsername("ThPaquet");

            Assert.NotNull(student);
            Assert.Equal("ThPaquet", student.Username);
            Assert.Equal("Thierry", student.FirstName);
            Assert.Equal("Paquet", student.LastName);
           
            
        }

        [Fact]
        public void Test_GetStudentByName_NotActive()
        {
            List<Student_SQLDTO> studentsBD = new List<Student_SQLDTO>()
            {
                new Student_SQLDTO()
                {Id = 1,
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
                {Id = 2,
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Matricule = "1122334",
                    Active = true
                },
                new Student_SQLDTO()
                {Id = 3,
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Matricule = "1324354",
                    Active = false
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Students).ReturnsDbSet(studentsBD);
            DepotStudent depot = new DepotStudent(context.Object);

            Student student = depot.GetStudentByUsername("BACenComm");

            Assert.Null(student);
           
            
        }

        [Fact]
        public void Test_GetStudentClasses()
        {
            List<Student_SQLDTO> studentsBD = new List<Student_SQLDTO>()
            {
                new Student_SQLDTO()
                {Id = 1,
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
                {Id = 2,
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Matricule = "1122334",
                    Active = true
                },
                new Student_SQLDTO()
                {Id = 3,
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Matricule = "1324354",
                    Active = false
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Students).ReturnsDbSet(studentsBD);
            DepotStudent depot = new DepotStudent(context.Object);

            List<Classroom> classes = depot.GetStudentClasses("ThPaquet");

            Assert.NotNull(classes);
            Assert.Equal(3, classes.Count);
            Assert.Contains(classes, c => c.Name == "RPLP");
           
            
        }

        [Fact]
        public void Test_UpsertStudent_Inserts()
        {
            List<Student_SQLDTO> studentsBD = new List<Student_SQLDTO>()
            {
                new Student_SQLDTO()
                {Id = 1,
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
                {Id = 2,
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Matricule = "1122334",
                    Active = true
                },
                new Student_SQLDTO()
                {Id=3,
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
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Teachers).ReturnsDbSet(teachers);
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsBD);
            context.Setup(x => x.Students).ReturnsDbSet(studentsBD);
            context.Setup(m => m.Students.Add(It.IsAny<Student_SQLDTO>())).Callback<Student_SQLDTO>(studentsBD.Add);
            DepotStudent depot = new DepotStudent(context.Object);

            Student_SQLDTO? studentInContext = studentsBD.FirstOrDefault(s => s.Username == "Node");
            Assert.Null(studentInContext);

            Student student = new Student()
            {
                Username = "Node",
                FirstName = "Dylan",
                LastName = "Veilleux",
                Email = "Node@hotmail.com",
                Matricule = "2196149"
            };

            depot.UpsertStudent(student);

            studentInContext = studentsBD.SingleOrDefault(s => s.Username == "Node");

            Assert.NotNull(studentInContext);
            Assert.Equal("Node", studentInContext.Username);
            Assert.Equal("Dylan", studentInContext.FirstName);
            Assert.Equal("Veilleux", studentInContext.LastName);
            Assert.Equal("Node@hotmail.com", studentInContext.Email);
           
            
        }

        [Fact]
        public void Test_UpsertStudent_Updates()
        {
            List<Student_SQLDTO> studentsBD = new List<Student_SQLDTO>()
            {
                new Student_SQLDTO()
                {Id= 1,
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
                {Id = 2,
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Matricule = "1122334",
                    Active = true
                },
                new Student_SQLDTO()
                {Id = 3,
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Matricule = "1324354",
                    Active = false
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Students).ReturnsDbSet(studentsBD);
            DepotStudent depot = new DepotStudent(context.Object);

            Student_SQLDTO? studentInContext = studentsBD.FirstOrDefault(s => s.Username == "ThPaquet");
            Assert.NotNull(studentInContext);

            studentInContext.Username = "Upserted";
            studentInContext.FirstName = "Upserty";
            studentInContext.LastName = "McUpserton";

            depot.UpsertStudent(studentInContext.ToEntity());

            studentInContext = studentsBD.SingleOrDefault(s => s.Username == "Upserted");

            Assert.NotNull(studentInContext);
            Assert.Equal(3, studentInContext.Classes.Count);
            Assert.Equal("Upserted", studentInContext.Username);
            Assert.Equal("Upserty", studentInContext.FirstName);
            Assert.Equal("McUpserton", studentInContext.LastName);
           
            
        }

        [Fact]
        public void Test_DeleteStudent()
        {
            List<Student_SQLDTO> studentsBD = new List<Student_SQLDTO>()
            {
                new Student_SQLDTO()
                {Id=1,
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
                {Id = 2,
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Matricule = "1122334",
                    Active = true
                },
                new Student_SQLDTO()
                {Id = 3,
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Matricule = "1324354",
                    Active = false
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Students).ReturnsDbSet(studentsBD);
            DepotStudent depot = new DepotStudent(context.Object);

            Student_SQLDTO? studentInContext = studentsBD.SingleOrDefault(s => s.Username == "ThPaquet");
            Assert.NotNull(studentInContext);

            depot.DeleteStudent("ThPaquet");

            studentInContext = studentsBD.SingleOrDefault(s => s.Username == "ThPaquet" && s.Active);

            Assert.Null(studentInContext);
           
            
        }

        [Fact]
        public void Test_ReactivateStudent()
        {
            List<Student_SQLDTO> studentsBD = new List<Student_SQLDTO>()
            {
                new Student_SQLDTO()
                {Id = 1,
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
                {Id = 2,
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Matricule = "1122334",
                    Active = true
                },
                new Student_SQLDTO()
                {Id = 3,
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Matricule = "1324354",
                    Active = false
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Students).ReturnsDbSet(studentsBD);
            DepotStudent depot = new DepotStudent(context.Object);

            Student_SQLDTO? studentInContext = studentsBD.SingleOrDefault(s => s.Username == "BACenComm" && !s.Active);
            Assert.NotNull(studentInContext);

            depot.ReactivateStudent("BACenComm");

            studentInContext = studentsBD.SingleOrDefault(s => s.Username == "BACenComm" && s.Active);

            Assert.NotNull(studentInContext);
           
            
        }

        [Fact]
        public void Test_UpsertStudent_ThrowUpdateDeletedAccount()
        {
            List<Student_SQLDTO> studentsBD = new List<Student_SQLDTO>()
            {
                new Student_SQLDTO()
                {Id = 1,
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
                {Id=2,
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Matricule = "1122334",
                    Active = true
                },
                new Student_SQLDTO()
                {Id = 3,
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Matricule = "1324354",
                    Active = false
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Students).ReturnsDbSet(studentsBD);
            DepotStudent depot = new DepotStudent(context.Object);

            Student_SQLDTO? student = studentsBD.SingleOrDefault(a => a.Username == "BACenComm");

            Assert.Throws<ArgumentException>(
                () =>
                {
                    depot.UpsertStudent(student.ToEntityWithoutList());
                });

            //logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Never);
            
        }

        [Fact]
        public void Test_UpsertStudent_ThrowUsernameTaken_UsernameTakenNotActive()
        {
            List<Student_SQLDTO> studentsBD = new List<Student_SQLDTO>()
            {
                new Student_SQLDTO()
                {Id = 1,
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
                {Id = 2,
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Matricule = "1122334",
                    Active = true
                },
                new Student_SQLDTO()
                {Id = 3,
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
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Teachers).ReturnsDbSet(teachers);
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsBD);
            context.Setup(x => x.Students).ReturnsDbSet(studentsBD);
            DepotStudent depot = new DepotStudent(context.Object);

            string username = "BACenComm";


            Student_SQLDTO? studentInDB = studentsBD.FirstOrDefault(s => s.Username == username);
            Assert.NotNull(studentInDB);
            Assert.False(studentInDB.Active);

            Student student = new Student()
            {
                Id = 244,
                Email = "Testeron@hotmail.com",
                FirstName = "Testy",
                LastName = "McTesterton",
                Username = username
            };

            Assert.Throws<ArgumentException>(
                () =>
                {
                    depot.UpsertStudent(student);
                });
            //logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Never);
            
        }

        [Fact]
        public void Test_UpsertStudent_ThrowUsernameTaken_NewAdmin()
        {
            List<Student_SQLDTO> studentsBD = new List<Student_SQLDTO>()
            {
                new Student_SQLDTO()
                {Id= 1,
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
                {Id = 2,
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Matricule = "1122334",
                    Active = true
                },
                new Student_SQLDTO()
                {Id = 3,
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
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Teachers).ReturnsDbSet(teachers);
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsBD);
            context.Setup(x => x.Students).ReturnsDbSet(studentsBD);
            DepotStudent depot = new DepotStudent(context.Object);

            Student_SQLDTO student = new Student_SQLDTO()
            {
                Username = "ThPaquet",
                FirstName = "Thierry",
                LastName = "Paquet",
                Classes = new List<Classroom_SQLDTO>(),
                Email = "swerve@hotmail.com",
                Active = true
            };

            Assert.Throws<ArgumentException>(
                () =>
                {
                    depot.UpsertStudent(student.ToEntityWithoutList());
                });
            //logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Never);
            
        }

        [Fact]
        public void Test_UpsertStudent_ThrowEmailTaken_EmailTakenNotActive()
        {
            List<Student_SQLDTO> studentsBD = new List<Student_SQLDTO>()
            {
                new Student_SQLDTO()
                {Id= 1,
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
                {Id = 2,
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Matricule = "1122334",
                    Active = true
                },
                new Student_SQLDTO()
                {Id = 3,
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
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Teachers).ReturnsDbSet(teachers);
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsBD);
            context.Setup(x => x.Students).ReturnsDbSet(studentsBD);
            DepotStudent depot = new DepotStudent(context.Object);

            string email = "BACenComm@hotmail.com";

            Student_SQLDTO? studentInDB = studentsBD.FirstOrDefault(s => s.Email == email);

            Assert.NotNull(studentInDB);
            Assert.False(studentInDB.Active);

            Student student = new Student()
            {
                Id = 244,
                Email = email,
                FirstName = "Testy",
                LastName = "McTesterton",
                Username = "Tester"
            };

            Assert.Throws<ArgumentException>(
                () =>
                {
                    depot.UpsertStudent(student);
                });
            //logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Never);
            
        }

        [Fact]
        public void Test_UpsertStudent_ThrowEmailTaken_NewAdmin()
        {
            List<Student_SQLDTO> studentsBD = new List<Student_SQLDTO>()
            {
                new Student_SQLDTO()
                {Id = 1,
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
                {Id = 2,
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Matricule = "1122334",
                    Active = true
                },
                new Student_SQLDTO()
                {Id=3,
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
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Teachers).ReturnsDbSet(teachers);
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsBD);
            context.Setup(x => x.Students).ReturnsDbSet(studentsBD);
            DepotStudent depot = new DepotStudent(context.Object);

            Student_SQLDTO student = new Student_SQLDTO()
            {Id=4,
                Username = "Swerve",
                FirstName = "Thierry",
                LastName = "Paquet",
                Classes = new List<Classroom_SQLDTO>(),
                Email = "ThPaquet@hotmail.com",
                Active = true
            };

            Assert.Throws<ArgumentException>(
                () =>
                {
                    depot.UpsertStudent(student.ToEntityWithoutList());
                });
            //logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Never);
            
        }
    }
}
