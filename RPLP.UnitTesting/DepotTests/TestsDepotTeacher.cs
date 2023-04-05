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
    public class TestsDepotTeacher
    {
        [Fact]
        private void Test_GetTeachers()
        {
            List<Teacher_SQLDTO> teachersDB = new List<Teacher_SQLDTO>()
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
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Teachers).ReturnsDbSet(teachersDB);
            DepotTeacher depot = new DepotTeacher(context.Object);

            List<Teacher> teachers = depot.GetTeachers();

            Assert.NotNull(teachers);
            Assert.Equal(2, teachers.Count);
            Assert.Contains(teachers, t => t.Username == "ThPaquet");
            Assert.DoesNotContain(teachers, t => t.Username == "BACenComm");
            Assert.Equal(3, teachers.FirstOrDefault(t => t.Username == "ThPaquet").Classes.Count);
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        private void Test_GetDeactivatedTeachers()
        {
            List<Teacher_SQLDTO> teachersDB = new List<Teacher_SQLDTO>()
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
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Teachers).ReturnsDbSet(teachersDB);
            DepotTeacher depot = new DepotTeacher(context.Object);

            List<Teacher> teachers = depot.GetDeactivatedTeachers();

            Assert.NotNull(teachers);
            Assert.Equal(1, teachers.Count);
            Assert.DoesNotContain(teachers, t => t.Username == "ThPaquet");
            Assert.Contains(teachers, t => t.Username == "BACenComm");
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        private void Test_GetTeacherById()
        {
            List<Teacher_SQLDTO> teachersDB = new List<Teacher_SQLDTO>()
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
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Teachers).ReturnsDbSet(teachersDB);
            DepotTeacher depot = new DepotTeacher(context.Object);

            Teacher_SQLDTO thPaquetInContext = teachersDB.FirstOrDefault(t => t.Username == "ThPaquet");
            Assert.NotNull(thPaquetInContext);

            int teacherId = thPaquetInContext.Id;

            Teacher teacher = depot.GetTeacherById(teacherId);

            Assert.NotNull(teacher);
            Assert.Equal("ThPaquet", teacher.Username);
            Assert.Equal("Thierry", teacher.FirstName);
            Assert.Equal("Paquet", teacher.LastName);
            Assert.Equal(3, teacher.Classes.Count);
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        private void Test_GetTeacherById_NotActive()
        {
            List<Teacher_SQLDTO> teachersDB = new List<Teacher_SQLDTO>()
            {
                new Teacher_SQLDTO()
                {
                    Id=1,
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
                    Id=2,
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Active = true
                },
                new Teacher_SQLDTO()
                {
                    Id=3,
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Active = false
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Teachers).ReturnsDbSet(teachersDB);
            DepotTeacher depot = new DepotTeacher(context.Object);

            Teacher_SQLDTO teacherInContext = teachersDB.FirstOrDefault(t => t.Username == "BACenComm");
            Assert.NotNull(teacherInContext);
            Assert.False(teacherInContext.Active);

            int teacherId = teacherInContext.Id;

            Teacher teacher = depot.GetTeacherById(teacherId);

            Assert.Null(teacher);
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        private void Test_GetTeacherByEmail()
        {
            List<Teacher_SQLDTO> teachersDB = new List<Teacher_SQLDTO>()
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
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Teachers).ReturnsDbSet(teachersDB);
            DepotTeacher depot = new DepotTeacher(context.Object);

            Teacher_SQLDTO thPaquetInContext = teachersDB.FirstOrDefault(t => t.Email == "ThPaquet@hotmail.com");
            Assert.NotNull(thPaquetInContext);


            Teacher teacher = depot.GetTeacherByEmail("ThPaquet@hotmail.com");

            Assert.NotNull(teacher);
            Assert.Equal("ThPaquet", teacher.Username);
            Assert.Equal("Thierry", teacher.FirstName);
            Assert.Equal("Paquet", teacher.LastName);
            Assert.Equal(3, teacher.Classes.Count);
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        private void Test_GetTeacherByEmail_NotActive()
        {
            List<Teacher_SQLDTO> teachersDB = new List<Teacher_SQLDTO>()
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
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Teachers).ReturnsDbSet(teachersDB);
            DepotTeacher depot = new DepotTeacher(context.Object);

            Teacher_SQLDTO teacherInContext = teachersDB.FirstOrDefault(t => t.Email == "BACenComm@hotmail.com");
            Assert.NotNull(teacherInContext);
            Assert.False(teacherInContext.Active);


            Teacher teacher = depot.GetTeacherByEmail("BACenComm@hotmail.com");

            Assert.Null(teacher);
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        private void Test_GetTeacherByUsername()
        {
            List<Teacher_SQLDTO> teachersDB = new List<Teacher_SQLDTO>()
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
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Teachers).ReturnsDbSet(teachersDB);
            DepotTeacher depot = new DepotTeacher(context.Object);

            Teacher_SQLDTO thPaquetInContext = teachersDB.FirstOrDefault(t => t.Username == "ThPaquet");
            Assert.NotNull(thPaquetInContext);


            Teacher teacher = depot.GetTeacherByUsername("ThPaquet");

            Assert.NotNull(teacher);
            Assert.Equal("ThPaquet", teacher.Username);
            Assert.Equal("Thierry", teacher.FirstName);
            Assert.Equal("Paquet", teacher.LastName);
            Assert.Equal(3, teacher.Classes.Count);
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        private void Test_GetTeacherByUsername_NotActive()
        {
            List<Teacher_SQLDTO> teachersDB = new List<Teacher_SQLDTO>()
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
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Teachers).ReturnsDbSet(teachersDB);
            DepotTeacher depot = new DepotTeacher(context.Object);

            Teacher_SQLDTO teacherInContext = teachersDB.FirstOrDefault(t => t.Username == "BACenComm");
            Assert.NotNull(teacherInContext);
            Assert.False(teacherInContext.Active);


            Teacher teacher = depot.GetTeacherByUsername("BACenComm");

            Assert.Null(teacher);
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        private void Test_GetTeacherClassesInOrganisationByEmail()
        {
            List<Teacher_SQLDTO> teachersDB = new List<Teacher_SQLDTO>()
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
            List<Classroom_SQLDTO> classroomBD = new List<Classroom_SQLDTO>
            {
                new Classroom_SQLDTO
                {
                    Name = "ProjetSynthese",
                    OrganisationName = "CEGEP Ste-Foy",
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = teachersDB,
                    Active = true
                },
                new Classroom_SQLDTO()
                {
                    Name = "RPLP",
                    OrganisationName = "CEGEP Ste-Foy",
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = teachersDB,
                    Active = true
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Classrooms).ReturnsDbSet(classroomBD);
            context.Setup(x => x.Teachers).ReturnsDbSet(teachersDB);
            DepotTeacher depot = new DepotTeacher(context.Object);

            Teacher_SQLDTO thPaquetInContext = teachersDB.FirstOrDefault(t => t.Email == "ThPaquet@hotmail.com");
            Assert.NotNull(thPaquetInContext);
            Assert.True(thPaquetInContext.Active);
            Assert.Contains(thPaquetInContext.Classes, c => c.Name == "ProjetSynthese" && c.OrganisationName == "CEGEP Ste-Foy");
            Assert.Contains(thPaquetInContext.Classes, c => c.Name == "RPLP" && c.OrganisationName == "CEGEP Ste-Foy");

            List<Classroom> classes = depot.GetTeacherClassesInOrganisationByEmail("ThPaquet@hotmail.com", "CEGEP Ste-Foy");

            Assert.Equal(2, classes.Count);
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        private void Test_GetTeacherOrganisations()
        {
            List<Teacher_SQLDTO> teachersDB = new List<Teacher_SQLDTO>()
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
            List<Organisation_SQLDTO> organisationsDB = new List<Organisation_SQLDTO>()
            {
                new Organisation_SQLDTO()
                {
                    Name = "CEGEP Ste-Foy",
                    Administrators = new List<Administrator_SQLDTO>(),
                    Active = true
                },
                new Organisation_SQLDTO()
                {
                    Name = "College Edouard-Montpetit",
                    Administrators = new List<Administrator_SQLDTO>(),
                    Active = true
                },
                new Organisation_SQLDTO()
                {
                    Name = "Universite Laval",
                    Administrators = new List<Administrator_SQLDTO>(),
                    Active = false
                },
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Teachers).ReturnsDbSet(teachersDB);
            context.Setup(x => x.Organisations).ReturnsDbSet(organisationsDB);
            DepotTeacher depot = new DepotTeacher(context.Object);

            organisationsDB.Add(new Organisation_SQLDTO()
            {
                Active = true,
                Name = "CEGEP Ste-Foy"
            });

            List<Organisation> organisations = depot.GetTeacherOrganisations("ThPaquet");

            Assert.NotNull(organisations);
            Assert.Equal(1, organisations.Count);
            Assert.Contains(organisations, o => o.Name == "CEGEP Ste-Foy");
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }


        [Fact]
        private void Test_GetTeacherClasses()
        {
            List<Teacher_SQLDTO> teachersDB = new List<Teacher_SQLDTO>()
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
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Teachers).ReturnsDbSet(teachersDB);
            DepotTeacher depot = new DepotTeacher(context.Object);

            List<Classroom> classes = depot.GetTeacherClasses("ThPaquet");

            Assert.NotNull(classes);
            Assert.Equal(2, classes.Count);
            Assert.Contains(classes, c => c.Name == "ProjetSynthese");
            Assert.DoesNotContain(classes, c => c.Name == "OOP");
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        private void Test_GetTeacherClassesInOrganisation()
        {
            List<Teacher_SQLDTO> teachersDB = new List<Teacher_SQLDTO>()
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
            List<Classroom_SQLDTO> classroomBD = new List<Classroom_SQLDTO>
            {
                new Classroom_SQLDTO
                {
                    Name = "ProjetSynthese",
                    OrganisationName = "CEGEP Ste-Foy",
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = teachersDB,
                    Active = true
                },
                new Classroom_SQLDTO()
                {
                    Name = "RPLP",
                    OrganisationName = "CEGEP Ste-Foy",
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = teachersDB,
                    Active = true
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Classrooms).ReturnsDbSet(classroomBD);
            context.Setup(x => x.Teachers).ReturnsDbSet(teachersDB);
            DepotTeacher depot = new DepotTeacher(context.Object);

            List<Classroom> classes = depot.GetTeacherClassesInOrganisation("ThPaquet", "CEGEP Ste-Foy");

            Assert.Equal(2, classes.Count);
            Assert.Contains(classes, c => c.Name == "RPLP");
            Assert.DoesNotContain(classes, c => c.Name == "OOP");
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        private void Test_AddClassroomToTeacher()
        {
            List<Teacher_SQLDTO> teachersDB = new List<Teacher_SQLDTO>()
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
            List<Classroom_SQLDTO> classroomBD = new List<Classroom_SQLDTO>
            {
                new Classroom_SQLDTO
                {
                    Name = "ProjetSynthese",
                    OrganisationName = "CEGEP Ste-Foy",
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                },
                new Classroom_SQLDTO()
                {
                    Name = "RPLP",
                    OrganisationName = "CEGEP Ste-Foy",
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Teachers).ReturnsDbSet(teachersDB);
            context.Setup(x => x.Classrooms).ReturnsDbSet(classroomBD);
            DepotTeacher depot = new DepotTeacher(context.Object);

            Teacher_SQLDTO teacherInContext = teachersDB.FirstOrDefault(t => t.Username == "ikeameatbol");

            Assert.NotNull(teacherInContext);
            Assert.DoesNotContain(teacherInContext.Classes, c => c.Name == "RPLP");

            Classroom_SQLDTO classroomInContext = classroomBD.FirstOrDefault(c => c.Name == "RPLP");
            Assert.NotNull(classroomInContext);

            depot.AddClassroomToTeacher("ikeameatbol", "RPLP");


            teacherInContext = teachersDB.FirstOrDefault(t => t.Username == "ikeameatbol");
            Assert.NotNull(teacherInContext);

            Assert.Contains(teacherInContext.Classes, c => c.Name == "RPLP");
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        private void Test_RemoveClassroomFromTeacher()
        {
            List<Teacher_SQLDTO> teachersDB = new List<Teacher_SQLDTO>()
            {
                new Teacher_SQLDTO()
                {
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com",
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
            List<Classroom_SQLDTO> classroomBD = new List<Classroom_SQLDTO>
            {
                new Classroom_SQLDTO
                {
                    Id = 1,
                    Name = "ProjetSynthese",
                    OrganisationName = "CEGEP Ste-Foy",
                    Active = true
                },
                new Classroom_SQLDTO()
                {
                    Id=2,
                    Name = "RPLP",
                    OrganisationName = "CEGEP Ste-Foy",
                    Active = true
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Classrooms).ReturnsDbSet(classroomBD);
            context.Setup(x => x.Teachers).ReturnsDbSet(teachersDB);
            DepotTeacher depot = new DepotTeacher(context.Object);

            Teacher_SQLDTO teacherInContext = teachersDB.FirstOrDefault(t => t.Username == "ThPaquet");
            teacherInContext.Classes.Add(classroomBD[0]);
            teacherInContext.Classes.Add(classroomBD[1]);

            Assert.NotNull(teacherInContext);
            Assert.Contains(teacherInContext.Classes, c => c.Name == "RPLP");

            depot.RemoveClassroomFromTeacher("ThPaquet", "RPLP");

            teacherInContext = teachersDB.FirstOrDefault(t => t.Username == "ThPaquet");
            Assert.NotNull(teacherInContext);

            Assert.DoesNotContain(teacherInContext.Classes, c => c.Name == "RPLP");
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Test_UpsertTeacher_Inserts()
        {
            List<Teacher_SQLDTO> teachersDB = new List<Teacher_SQLDTO>()
            {
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
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Teachers).ReturnsDbSet(teachersDB);
            context.Setup(m => m.Teachers.Add(It.IsAny<Teacher_SQLDTO>())).Callback<Teacher_SQLDTO>(teachersDB.Add);
            DepotTeacher depot = new DepotTeacher(context.Object);

            Teacher teacher = new Teacher()
            {
                Username = "ThPaquet",
                FirstName = "Thierry",
                LastName = "Paquet",
                Email = "ThPaquet@hotmail.com"
            };

            Teacher_SQLDTO teacherInContext = teachersDB.FirstOrDefault(t => t.Username == "ThPaquet");
            Assert.Null(teacherInContext);

            depot.UpsertTeacher(teacher);

            teacherInContext = teachersDB.FirstOrDefault(t => t.Username == "ThPaquet");

            Assert.NotNull(teacherInContext);
            Assert.Equal("Thierry", teacherInContext.FirstName);
            Assert.Equal("Paquet", teacherInContext.LastName);
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Test_UpsertTeacher_Updates()
        {
            List<Teacher_SQLDTO> teachersDB = new List<Teacher_SQLDTO>()
            {
                new Teacher_SQLDTO()
                {
                    Id = 1,
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
                    Id = 2,
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Active = true
                },
                new Teacher_SQLDTO()
                {
                    Id= 3,
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
                    Active = false
                }
            };
             
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            
            context.Setup(x => x.Teachers).ReturnsDbSet(teachersDB);
            DepotTeacher depot = new DepotTeacher(context.Object);

            Teacher? teacherInContext = teachersDB.FirstOrDefault(t => t.Username == "ThPaquet").ToEntity();
            Assert.NotNull(teacherInContext);

            teacherInContext.Username = "Upserted";
            teacherInContext.FirstName = "Upserty";
            teacherInContext.LastName = "McUpserton";

            Teacher_SQLDTO updatedTeacherBeforeUpsert = teachersDB.FirstOrDefault(t => t.Username == "Upserted");

            Assert.Null(updatedTeacherBeforeUpsert);

            depot.UpsertTeacher(teacherInContext);

            Teacher? teacherEntity = teachersDB.FirstOrDefault(t => t.Username == "Upserted").ToEntity();

            Assert.NotNull(teacherEntity);
            Assert.Equal("Upserty", teacherEntity.FirstName);
            Assert.Equal("McUpserton", teacherEntity.LastName);

            Teacher_SQLDTO teacherBeforeUpsert = teachersDB.FirstOrDefault(t => t.Username == "ThPaquet");

            Assert.Null(teacherBeforeUpsert);
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Test_UpsertTeacher_ThrowUpdateDeletedAccount()
        {
            List<Teacher_SQLDTO> teachersDB = new List<Teacher_SQLDTO>()
            {
                new Teacher_SQLDTO()
                {
                    Id = 1,
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
                    Id = 2,
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Active = true
                },
                new Teacher_SQLDTO()
                {
                    Id= 3,
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

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Students).ReturnsDbSet(students);
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsBD);
            context.Setup(x => x.Teachers).ReturnsDbSet(teachersDB);
            DepotTeacher depot = new DepotTeacher(context.Object);
            Teacher_SQLDTO? teacher = teachersDB.SingleOrDefault(a => a.Username == "BACenComm");

            Assert.Throws<ArgumentException>(
                () =>
                {
                    depot.UpsertTeacher(teacher.ToEntityWithoutList());
                });
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Never);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Test_UpsertTeacher_ThrowUsernameTaken_UsernameTakenNotActive()
        {
            List<Teacher_SQLDTO> teachersDB = new List<Teacher_SQLDTO>()
            {
                new Teacher_SQLDTO()
                {
                    Id = 1,
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
                    Id = 2,
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Active = true
                },
                new Teacher_SQLDTO()
                {
                    Id= 3,
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

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Students).ReturnsDbSet(students);
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsBD);
            context.Setup(x => x.Teachers).ReturnsDbSet(teachersDB);
            DepotTeacher depot = new DepotTeacher(context.Object);

            string username = "BACenComm";

            Teacher_SQLDTO? teacherInDB = teachersDB.FirstOrDefault(t => t.Username == username);
            Assert.NotNull(teacherInDB);
            Assert.False(teacherInDB.Active);

            Teacher teacher = new Teacher()
            {
                Id = 244,
                Email = "Tester@hotmail.com",
                FirstName = "Testy",
                LastName = "McTesterton",
                Username = username
            };

            Assert.Throws<ArgumentException>(
                () =>
                {
                    depot.UpsertTeacher(teacher);
                });
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Never);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Test_UpsertTeacher_ThrowUsernameTaken_NewAdmin()
        {
            List<Teacher_SQLDTO> teachersDB = new List<Teacher_SQLDTO>()
            {
                new Teacher_SQLDTO()
                {
                    Id = 1,
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
                    Id = 2,
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Active = true
                },
                new Teacher_SQLDTO()
                {
                    Id= 3,
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

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Students).ReturnsDbSet(students);
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsBD);
            context.Setup(x => x.Teachers).ReturnsDbSet(teachersDB);
            DepotTeacher depot = new DepotTeacher(context.Object);

            Teacher_SQLDTO teacher = new Teacher_SQLDTO()
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
                    depot.UpsertTeacher(teacher.ToEntityWithoutList());
                });
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Never);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Test_UpsertTeacher_ThrowEmailTaken_EmailTakenNotActive()
        {
            List<Teacher_SQLDTO> teachersDB = new List<Teacher_SQLDTO>()
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

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Students).ReturnsDbSet(students);
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsBD);
            context.Setup(x => x.Teachers).ReturnsDbSet(teachersDB);
            DepotTeacher depot = new DepotTeacher(context.Object);

            string email = "BACenComm@hotmail.com";

            Teacher_SQLDTO? teacherInDB = teachersDB.FirstOrDefault(t => t.Email == email);
            Assert.NotNull(teacherInDB);
            Assert.False(teacherInDB.Active);

            Teacher teacher = new Teacher()
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
                    depot.UpsertTeacher(teacher);
                });
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Never);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Test_UpsertTeacher_ThrowEmailTaken_NewAdmin()
        {
            List<Teacher_SQLDTO> teachersDB = new List<Teacher_SQLDTO>()
            {
                new Teacher_SQLDTO()
                {
                    Id = 1,
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
                    Id = 2,
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Active = true
                },
                new Teacher_SQLDTO()
                {
                    Id= 3,
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
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Students).ReturnsDbSet(students);
            context.Setup(x => x.Administrators).ReturnsDbSet(administratorsBD);
            context.Setup(x => x.Teachers).ReturnsDbSet(teachersDB);
            DepotTeacher depot = new DepotTeacher(context.Object);

            Teacher_SQLDTO teacher = new Teacher_SQLDTO()
            {
                Id= 4,
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
                    depot.UpsertTeacher(teacher.ToEntityWithoutList());
                });
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Never);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Test_DeleteTeacher()
        {
            List<Teacher_SQLDTO> teachersDB = new List<Teacher_SQLDTO>()
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
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Teachers).ReturnsDbSet(teachersDB);
            DepotTeacher depot = new DepotTeacher(context.Object);

            Teacher_SQLDTO teacherInDB = teachersDB.FirstOrDefault(t => t.Username == "ThPaquet" && t.Active);
            Assert.NotNull(teacherInDB);

            depot.DeleteTeacher(teacherInDB.Username);

            teacherInDB = teachersDB.FirstOrDefault(t => t.Username == "ThPaquet" && t.Active);
            Assert.Null(teacherInDB);
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Test_ReactivateTeacher()
        {
            List<Teacher_SQLDTO> teachersDB = new List<Teacher_SQLDTO>()
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
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Teachers).ReturnsDbSet(teachersDB);
            DepotTeacher depot = new DepotTeacher(context.Object);

            Teacher_SQLDTO teacherInDB = teachersDB.FirstOrDefault(t => t.Username == "BACenComm" && !t.Active);
            Assert.NotNull(teacherInDB);

            depot.ReactivateTeacher(teacherInDB.Username);

            teacherInDB = teachersDB.FirstOrDefault(t => t.Username == "BACenComm" && t.Active);
            Assert.NotNull(teacherInDB);
            logMock.Verify(log => log.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }
    }
}
