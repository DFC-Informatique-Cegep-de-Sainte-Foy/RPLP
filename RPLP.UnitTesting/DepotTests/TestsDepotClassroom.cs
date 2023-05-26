using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using RPLP.DAL.DTO.Sql;
using RPLP.DAL.SQL;
using RPLP.DAL.SQL.Depots;
using RPLP.ENTITES;
using RPLP.JOURNALISATION;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RPLP.UnitTesting.DepotTests
{
    public class TestsDepotClassroom
    {

        [Fact]
        public void Test_GetClassrooms()
        {
            List<Classroom_SQLDTO> classroomBD = new List<Classroom_SQLDTO>
            {
                new Classroom_SQLDTO
                {
                    Name = "ProjetSynthese",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                },
                new Classroom_SQLDTO()
                {
                    Name = "OOP",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                }
            };


            classroomBD[0].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "Review",
                ClassroomId = 1,
                DistributionDate = System.DateTime.Now,
                Description = "Review a partner\'s code",
                DeliveryDeadline = System.DateTime.Now.AddDays(1),
                Active = true
            });

            classroomBD[0].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "Scrum",
                ClassroomId = 1,
                DistributionDate = System.DateTime.Now,
                Description = "Daily briefing",
                DeliveryDeadline = System.DateTime.Now.AddDays(1),
                Active = true
            });

            classroomBD[0].Students.Add(new Student_SQLDTO()
            {
                Username = "ThPaquet",
                FirstName = "Thierry",
                LastName = "Paquet",
                Email = "ThPaquet@hotmail.com",
                Matricule = "1141200",
                Active = true
            });

            classroomBD[0].Teachers.Add(new Teacher_SQLDTO()
            {
                Username = "PiFou86",
                FirstName = "Pierre-Francois",
                LastName = "Leon",
                Email = "PiFou86@hotmail.com",
                Active = true
            });

            classroomBD[1].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "UnitTests",
                ClassroomId = 2,
                DistributionDate = System.DateTime.Now,
                Description = "Review a partner\'s code",
                DeliveryDeadline = System.DateTime.Now.AddDays(1)
            });
            classroomBD[1].Students.Add(new Student_SQLDTO()
            {
                Username = "ikeameatbol",
                FirstName = "Jonathan",
                LastName = "Blouin",
                Email = "ikeameatbol@hotmail.com",
                Matricule = "1122334",
                Active = true
            });

            classroomBD[1].Teachers.Add(new Teacher_SQLDTO()
            {
                Username = "BACenComm",
                FirstName = "Melissa",
                LastName = "Lachapelle",
                Email = "BACenComm@hotmail.com",
                Active = true
            });

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Classrooms).ReturnsDbSet(classroomBD);
            DepotClassroom depot = new DepotClassroom(context.Object);

            List<Classroom> classrooms = depot.GetClassrooms();

            Assert.Equal(2, classrooms.Count);
            Assert.Contains(classrooms, c => c.Name == "ProjetSynthese");
            Assert.Contains(classrooms, c => c.Name == "OOP");
            Classroom? projetSyntheseClassroom = classrooms.FirstOrDefault(c => c.Name == "ProjetSynthese");
            Assert.NotNull(projetSyntheseClassroom);
            Assert.Equal(2, projetSyntheseClassroom.Assignments.Count);
            Assert.Contains(projetSyntheseClassroom.Assignments, a => a.Name == "Review");
            Assert.Contains(projetSyntheseClassroom.Students, s => s.Username == "ThPaquet");
            Assert.Contains(projetSyntheseClassroom.Teachers, t => t.Username == "PiFou86");
           
            
        }

        [Fact]
        public void Test_GetClassroomById()
        {
            List<Classroom_SQLDTO> classroomBD = new List<Classroom_SQLDTO>
            {
                new Classroom_SQLDTO
                {
                    Name = "ProjetSynthese",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                },
                new Classroom_SQLDTO()
                {
                    Name = "OOP",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                }
            };


            classroomBD[0].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "Review",
                ClassroomId = 1,
                DistributionDate = System.DateTime.Now,
                Description = "Review a partner\'s code",
                DeliveryDeadline = System.DateTime.Now.AddDays(1),
                Active = true
            });

            classroomBD[0].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "Scrum",
                ClassroomId = 1,
                DistributionDate = System.DateTime.Now,
                Description = "Daily briefing",
                DeliveryDeadline = System.DateTime.Now.AddDays(1),
                Active = true
            });

            classroomBD[0].Students.Add(new Student_SQLDTO()
            {
                Username = "ThPaquet",
                FirstName = "Thierry",
                LastName = "Paquet",
                Email = "ThPaquet@hotmail.com",
                Matricule = "1141200",
                Active = true
            });

            classroomBD[0].Teachers.Add(new Teacher_SQLDTO()
            {
                Username = "PiFou86",
                FirstName = "Pierre-Francois",
                LastName = "Leon",
                Email = "PiFou86@hotmail.com",
                Active = true
            });
            classroomBD[1].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "UnitTests",
                ClassroomId = 2,
                DistributionDate = System.DateTime.Now,
                Description = "Review a partner\'s code",
                DeliveryDeadline = System.DateTime.Now.AddDays(1)
            });
            classroomBD[1].Students.Add(new Student_SQLDTO()
            {
                Username = "ikeameatbol",
                FirstName = "Jonathan",
                LastName = "Blouin",
                Email = "ikeameatbol@hotmail.com",
                Matricule = "1122334",
                Active = true
            });

            classroomBD[1].Teachers.Add(new Teacher_SQLDTO()
            {
                Username = "BACenComm",
                FirstName = "Melissa",
                LastName = "Lachapelle",
                Email = "BACenComm@hotmail.com",
                Active = true
            });

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Classrooms).ReturnsDbSet(classroomBD);
            DepotClassroom depot = new DepotClassroom(context.Object);

            int classroomId = classroomBD.FirstOrDefault(c => c.Name == "ProjetSynthese").Id;

            Classroom classroom = depot.GetClassroomById(classroomId);

            Assert.NotNull(classroom);
            Assert.Contains(classroom.Assignments, a => a.Name == "Review");
            Assert.Contains(classroom.Students, s => s.Username == "ThPaquet");
            Assert.Contains(classroom.Teachers, t => t.Username == "PiFou86");
           
            
        }

        [Fact]
        public void Test_GetClassroomByName()
        {
            List<Classroom_SQLDTO> classroomBD = new List<Classroom_SQLDTO>
            {
                new Classroom_SQLDTO
                {
                    Name = "ProjetSynthese",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                },
                new Classroom_SQLDTO()
                {
                    Name = "OOP",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                }
            };


            classroomBD[0].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "Review",
                ClassroomId = 1,
                DistributionDate = System.DateTime.Now,
                Description = "Review a partner\'s code",
                DeliveryDeadline = System.DateTime.Now.AddDays(1),
                Active = true
            });

            classroomBD[0].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "Scrum",
                ClassroomId = 1,
                DistributionDate = System.DateTime.Now,
                Description = "Daily briefing",
                DeliveryDeadline = System.DateTime.Now.AddDays(1),
                Active = true
            });

            classroomBD[0].Students.Add(new Student_SQLDTO()
            {
                Username = "ThPaquet",
                FirstName = "Thierry",
                LastName = "Paquet",
                Email = "ThPaquet@hotmail.com",
                Matricule = "1141200",
                Active = true
            });

            classroomBD[0].Teachers.Add(new Teacher_SQLDTO()
            {
                Username = "PiFou86",
                FirstName = "Pierre-Francois",
                LastName = "Leon",
                Email = "PiFou86@hotmail.com",
                Active = true
            });

            classroomBD[1].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "UnitTests",
                ClassroomId = 2,
                DistributionDate = System.DateTime.Now,
                Description = "Review a partner\'s code",
                DeliveryDeadline = System.DateTime.Now.AddDays(1)
            });
            classroomBD[1].Students.Add(new Student_SQLDTO()
            {
                Username = "ikeameatbol",
                FirstName = "Jonathan",
                LastName = "Blouin",
                Email = "ikeameatbol@hotmail.com",
                Matricule = "1122334",
                Active = true
            });

            classroomBD[1].Teachers.Add(new Teacher_SQLDTO()
            {
                Username = "BACenComm",
                FirstName = "Melissa",
                LastName = "Lachapelle",
                Email = "BACenComm@hotmail.com",
                Active = true
            });

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Classrooms).ReturnsDbSet(classroomBD);
            DepotClassroom depot = new DepotClassroom(context.Object);

            Classroom classroom = depot.GetClassroomByName("ProjetSynthese");

            Assert.NotNull(classroom);
            Assert.Contains(classroom.Assignments, a => a.Name == "Review");
            Assert.Contains(classroom.Students, s => s.Username == "ThPaquet");
            Assert.Contains(classroom.Teachers, t => t.Username == "PiFou86");
           
            
        }

        [Fact]
        public void Test_GetClassroomsByOrganisationName()
        {
            List<Classroom_SQLDTO> classroomBD = new List<Classroom_SQLDTO>
            {
                new Classroom_SQLDTO
                {
                    Name = "ProjetSynthese",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                },
                new Classroom_SQLDTO()
                {
                    Name = "OOP",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                }
            };


            classroomBD[0].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "Review",
                ClassroomId = 1,
                DistributionDate = System.DateTime.Now,
                Description = "Review a partner\'s code",
                DeliveryDeadline = System.DateTime.Now.AddDays(1),
                Active = true
            });

            classroomBD[0].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "Scrum",
                ClassroomId = 1,
                DistributionDate = System.DateTime.Now,
                Description = "Daily briefing",
                DeliveryDeadline = System.DateTime.Now.AddDays(1),
                Active = true
            });

            classroomBD[0].Students.Add(new Student_SQLDTO()
            {
                Username = "ThPaquet",
                FirstName = "Thierry",
                LastName = "Paquet",
                Email = "ThPaquet@hotmail.com",
                Matricule = "1141200",
                Active = true
            });

            classroomBD[0].Teachers.Add(new Teacher_SQLDTO()
            {
                Username = "PiFou86",
                FirstName = "Pierre-Francois",
                LastName = "Leon",
                Email = "PiFou86@hotmail.com",
                Active = true
            });
            classroomBD[1].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "UnitTests",
                ClassroomId = 2,
                DistributionDate = System.DateTime.Now,
                Description = "Review a partner\'s code",
                DeliveryDeadline = System.DateTime.Now.AddDays(1)
            });

            classroomBD[1].Students.Add(new Student_SQLDTO()
            {
                Username = "ikeameatbol",
                FirstName = "Jonathan",
                LastName = "Blouin",
                Email = "ikeameatbol@hotmail.com",
                Matricule = "1122334",
                Active = true
            });

            classroomBD[1].Teachers.Add(new Teacher_SQLDTO()
            {
                Username = "BACenComm",
                FirstName = "Melissa",
                LastName = "Lachapelle",
                Email = "BACenComm@hotmail.com",
                Active = true
            });

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Classrooms).ReturnsDbSet(classroomBD);
            DepotClassroom depot = new DepotClassroom(context.Object);

            classroomBD.Add(new Classroom_SQLDTO()
            {
                Name = "false",
                OrganisationId = 1,
                Active = false,
                Assignments = new List<Assignment_SQLDTO>(),
                Students = new List<Student_SQLDTO>(),
                Teachers = new List<Teacher_SQLDTO>()
            });

            Assert.True(classroomBD.Count() == 3);
            Assert.True(classroomBD.Any(c => c.Name == "false" && !c.Active));
            Assert.True(classroomBD.Where(c => c.Active).Count() == 2);

            List<Classroom> classrooms = depot.GetClassroomsByOrganisationName("CEGEP Ste-Foy");

            Assert.True(classrooms.Count == 2);
           
            
        }

        [Fact]
        public void Test_GetAssignmentsByClassroomName()
        {
            List<Classroom_SQLDTO> classroomBD = new List<Classroom_SQLDTO>
            {
                new Classroom_SQLDTO
                {
                    Name = "ProjetSynthese",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                },
                new Classroom_SQLDTO()
                {
                    Name = "OOP",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                }
            };


            classroomBD[0].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "Review",
                ClassroomId = 1,
                DistributionDate = System.DateTime.Now,
                Description = "Review a partner\'s code",
                DeliveryDeadline = System.DateTime.Now.AddDays(1),
                Active = true
            });

            classroomBD[0].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "Scrum",
                ClassroomId = 1,
                DistributionDate = System.DateTime.Now,
                Description = "Daily briefing",
                DeliveryDeadline = System.DateTime.Now.AddDays(1),
                Active = true
            });

            classroomBD[0].Students.Add(new Student_SQLDTO()
            {
                Username = "ThPaquet",
                FirstName = "Thierry",
                LastName = "Paquet",
                Email = "ThPaquet@hotmail.com",
                Matricule = "1141200",
                Active = true
            });

            classroomBD[0].Teachers.Add(new Teacher_SQLDTO()
            {
                Username = "PiFou86",
                FirstName = "Pierre-Francois",
                LastName = "Leon",
                Email = "PiFou86@hotmail.com",
                Active = true
            });
            classroomBD[1].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "UnitTests",
                ClassroomId = 2,
                DistributionDate = System.DateTime.Now,
                Description = "Review a partner\'s code",
                DeliveryDeadline = System.DateTime.Now.AddDays(1)
            });

            classroomBD[1].Students.Add(new Student_SQLDTO()
            {
                Username = "ikeameatbol",
                FirstName = "Jonathan",
                LastName = "Blouin",
                Email = "ikeameatbol@hotmail.com",
                Matricule = "1122334",
                Active = true
            });

            classroomBD[1].Teachers.Add(new Teacher_SQLDTO()
            {
                Username = "BACenComm",
                FirstName = "Melissa",
                LastName = "Lachapelle",
                Email = "BACenComm@hotmail.com",
                Active = true
            });

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Classrooms).ReturnsDbSet(classroomBD);
            DepotClassroom depot = new DepotClassroom(context.Object);

            List<Assignment> assignments = depot.GetAssignmentsByClassroomName("ProjetSynthese");

            Assert.NotNull(assignments);
            Assert.NotNull(assignments.FirstOrDefault(a => a.Name == "Review"));
            Assert.Equal(2, assignments.Count);
           
            
        }

        [Fact]
        public void Test_GetStudentsByClassroomName()
        {
            List<Classroom_SQLDTO> classroomBD = new List<Classroom_SQLDTO>
            {
                new Classroom_SQLDTO
                {
                    Name = "ProjetSynthese",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                },
                new Classroom_SQLDTO()
                {
                    Name = "OOP",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                }
            };


            classroomBD[0].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "Review",
                ClassroomId = 1,
                DistributionDate = System.DateTime.Now,
                Description = "Review a partner\'s code",
                DeliveryDeadline = System.DateTime.Now.AddDays(1),
                Active = true
            });

            classroomBD[0].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "Scrum",
                ClassroomId = 1,
                DistributionDate = System.DateTime.Now,
                Description = "Daily briefing",
                DeliveryDeadline = System.DateTime.Now.AddDays(1),
                Active = true
            });

            classroomBD[0].Students.Add(new Student_SQLDTO()
            {
                Username = "ThPaquet",
                FirstName = "Thierry",
                LastName = "Paquet",
                Email = "ThPaquet@hotmail.com",
                Matricule = "1141200",
                Active = true
            });

            classroomBD[0].Teachers.Add(new Teacher_SQLDTO()
            {
                Username = "PiFou86",
                FirstName = "Pierre-Francois",
                LastName = "Leon",
                Email = "PiFou86@hotmail.com",
                Active = true
            });
            classroomBD[1].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "UnitTests",
                ClassroomId = 2,
                DistributionDate = System.DateTime.Now,
                Description = "Review a partner\'s code",
                DeliveryDeadline = System.DateTime.Now.AddDays(1)
            });

            classroomBD[1].Students.Add(new Student_SQLDTO()
            {
                Username = "ikeameatbol",
                FirstName = "Jonathan",
                LastName = "Blouin",
                Email = "ikeameatbol@hotmail.com",
                Matricule = "1122334",
                Active = true
            });

            classroomBD[1].Teachers.Add(new Teacher_SQLDTO()
            {
                Username = "BACenComm",
                FirstName = "Melissa",
                LastName = "Lachapelle",
                Email = "BACenComm@hotmail.com",
                Active = true
            });

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Classrooms).ReturnsDbSet(classroomBD);
            DepotClassroom depot = new DepotClassroom(context.Object);

            List<Student> students = depot.GetStudentsByClassroomName("ProjetSynthese");

            Assert.NotNull(students);
            Assert.NotNull(students.FirstOrDefault(a => a.Username == "ThPaquet"));
            Assert.Null(students.FirstOrDefault(a => a.Username == "BACenComm"));
            Assert.Equal(1, students.Count);
           
            
        }

        [Fact]
        public void Test_GetTeachersByClassroomName()
        {
            List<Classroom_SQLDTO> classroomBD = new List<Classroom_SQLDTO>
            {
                new Classroom_SQLDTO
                {
                    Name = "ProjetSynthese",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                },
                new Classroom_SQLDTO()
                {
                    Name = "OOP",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                }
            };


            classroomBD[0].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "Review",
                ClassroomId = 1,
                DistributionDate = System.DateTime.Now,
                Description = "Review a partner\'s code",
                DeliveryDeadline = System.DateTime.Now.AddDays(1),
                Active = true
            });

            classroomBD[0].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "Scrum",
                ClassroomId = 1,
                DistributionDate = System.DateTime.Now,
                Description = "Daily briefing",
                DeliveryDeadline = System.DateTime.Now.AddDays(1),
                Active = true
            });

            classroomBD[0].Students.Add(new Student_SQLDTO()
            {
                Username = "ThPaquet",
                FirstName = "Thierry",
                LastName = "Paquet",
                Email = "ThPaquet@hotmail.com",
                Matricule = "1141200",
                Active = true
            });

            classroomBD[0].Teachers.Add(new Teacher_SQLDTO()
            {
                Username = "PiFou86",
                FirstName = "Pierre-Francois",
                LastName = "Leon",
                Email = "PiFou86@hotmail.com",
                Active = true
            });
            classroomBD[1].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "UnitTests",
                ClassroomId = 2,
                DistributionDate = System.DateTime.Now,
                Description = "Review a partner\'s code",
                DeliveryDeadline = System.DateTime.Now.AddDays(1)
            });

            classroomBD[1].Students.Add(new Student_SQLDTO()
            {
                Username = "ikeameatbol",
                FirstName = "Jonathan",
                LastName = "Blouin",
                Email = "ikeameatbol@hotmail.com",
                Matricule = "1122334",
                Active = true
            });

            classroomBD[1].Teachers.Add(new Teacher_SQLDTO()
            {
                Username = "BACenComm",
                FirstName = "Melissa",
                LastName = "Lachapelle",
                Email = "BACenComm@hotmail.com",
                Active = true
            });

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Classrooms).ReturnsDbSet(classroomBD);
            DepotClassroom depot = new DepotClassroom(context.Object);

            List<Teacher> teachers = depot.GetTeachersByClassroomName("ProjetSynthese");

            Assert.NotNull(teachers);
            Assert.NotNull(teachers.FirstOrDefault(a => a.Username == "PiFou86"));
            Assert.Null(teachers.FirstOrDefault(a => a.Username == "BoumBoum"));
            Assert.Equal(1, teachers.Count);
           
            
        }

        [Fact]
        public void Test_AddAssignmentToClassroom()
        {
            List<Classroom_SQLDTO> classroomBD = new List<Classroom_SQLDTO>
            {
                new Classroom_SQLDTO
                {
                    Name = "ProjetSynthese",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                },
                new Classroom_SQLDTO()
                {
                    Name = "OOP",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                }
            };


            classroomBD[0].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "Review",
                ClassroomId = 1,
                DistributionDate = System.DateTime.Now,
                Description = "Review a partner\'s code",
                DeliveryDeadline = System.DateTime.Now.AddDays(1),
                Active = true
            });

            classroomBD[0].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "Scrum",
                ClassroomId = 1,
                DistributionDate = System.DateTime.Now,
                Description = "Daily briefing",
                DeliveryDeadline = System.DateTime.Now.AddDays(1),
                Active = true
            });

            classroomBD[0].Students.Add(new Student_SQLDTO()
            {
                Username = "ThPaquet",
                FirstName = "Thierry",
                LastName = "Paquet",
                Email = "ThPaquet@hotmail.com",
                Matricule = "1141200",
                Active = true
            });

            classroomBD[0].Teachers.Add(new Teacher_SQLDTO()
            {
                Username = "PiFou86",
                FirstName = "Pierre-Francois",
                LastName = "Leon",
                Email = "PiFou86@hotmail.com",
                Active = true
            });
            classroomBD[1].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "UnitTests",
                ClassroomId = 2,
                DistributionDate = System.DateTime.Now,
                Description = "Review a partner\'s code",
                DeliveryDeadline = System.DateTime.Now.AddDays(1)
            });
            classroomBD[1].Students.Add(new Student_SQLDTO()
            {
                Username = "ikeameatbol",
                FirstName = "Jonathan",
                LastName = "Blouin",
                Email = "ikeameatbol@hotmail.com",
                Matricule = "1122334",
                Active = true
            });

            classroomBD[1].Teachers.Add(new Teacher_SQLDTO()
            {
                Username = "BACenComm",
                FirstName = "Melissa",
                LastName = "Lachapelle",
                Email = "BACenComm@hotmail.com",
                Active = true
            });
            List<Assignment_SQLDTO> assignmentsBD = new List<Assignment_SQLDTO>()
            {
                new Assignment_SQLDTO()
                {
                    Name = "UnitTests",
                    ClassroomId = 1,
                    DistributionDate = System.DateTime.Now,
                    Description = "Review a partner\'s code",
                    DeliveryDeadline = System.DateTime.Now.AddDays(1),
                    Active = true
                },
                new Assignment_SQLDTO()
                {
                    Name = "AnotherOne",
                    ClassroomId = 3,
                    DistributionDate = System.DateTime.Now,
                    Description = "Review another partner\'s code",
                    DeliveryDeadline = System.DateTime.Now.AddDays(1),
                    Active = true
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Assignments).ReturnsDbSet(assignmentsBD);
            context.Setup(x => x.Classrooms).ReturnsDbSet(classroomBD);
            DepotClassroom depot = new DepotClassroom(context.Object);

            Assignment_SQLDTO? assignment = classroomBD[1].Assignments.FirstOrDefault(a => a.Name == "UnitTests");
            Assert.NotNull(assignment);

            Classroom_SQLDTO? classroom = classroomBD.FirstOrDefault(c => c.Name == "ProjetSynthese");
            Assignment_SQLDTO? assignmentInClassroom = classroom.Assignments.FirstOrDefault(a => a.Name == "UnitTests");

            Assert.Null(assignmentInClassroom);

            depot.AddAssignmentToClassroom("ProjetSynthese", "UnitTests");

            classroom = classroomBD.FirstOrDefault(c => c.Name == "ProjetSynthese");
            assignment = classroom.Assignments.FirstOrDefault(a => a.Name == "UnitTests");

            Assert.NotNull(assignment);
           
            
        }

        [Fact]
        public void Test_AddStudentToClassroom()
        {
            List<Classroom_SQLDTO> classroomBD = new List<Classroom_SQLDTO>
            {
                new Classroom_SQLDTO
                {
                    Name = "ProjetSynthese",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                },
                new Classroom_SQLDTO()
                {
                    Name = "OOP",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                }
            };


            classroomBD[0].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "Review",
                ClassroomId = 1,
                DistributionDate = System.DateTime.Now,
                Description = "Review a partner\'s code",
                DeliveryDeadline = System.DateTime.Now.AddDays(1),
                Active = true
            });

            classroomBD[0].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "Scrum",
                ClassroomId = 1,
                DistributionDate = System.DateTime.Now,
                Description = "Daily briefing",
                DeliveryDeadline = System.DateTime.Now.AddDays(1),
                Active = true
            });

            classroomBD[0].Students.Add(new Student_SQLDTO()
            {
                Username = "ThPaquet",
                FirstName = "Thierry",
                LastName = "Paquet",
                Email = "ThPaquet@hotmail.com",
                Matricule = "1141200",
                Active = true
            });

            classroomBD[0].Teachers.Add(new Teacher_SQLDTO()
            {
                Username = "PiFou86",
                FirstName = "Pierre-Francois",
                LastName = "Leon",
                Email = "PiFou86@hotmail.com",
                Active = true
            });
            classroomBD[1].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "UnitTests",
                ClassroomId = 2,
                DistributionDate = System.DateTime.Now,
                Description = "Review a partner\'s code",
                DeliveryDeadline = System.DateTime.Now.AddDays(1)
            });

            classroomBD[1].Teachers.Add(new Teacher_SQLDTO()
            {
                Username = "BACenComm",
                FirstName = "Melissa",
                LastName = "Lachapelle",
                Email = "BACenComm@hotmail.com",
                Active = true
            });

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
                            OrganisationId = 1,
                            Active = true
                        },
                        new Classroom_SQLDTO()
                        {
                            Name = "RPLP",
                            OrganisationId = 1,
                            Active = true
                        },
                        new Classroom_SQLDTO()
                        {
                            Name = "OOP",
                            OrganisationId = 1,
                            Active = false
                        }
                    },
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
            context.Setup(x => x.Classrooms).ReturnsDbSet(classroomBD);
            DepotClassroom depot = new DepotClassroom(context.Object);

            Classroom_SQLDTO? classroom = classroomBD.FirstOrDefault(c => c.Name == "ProjetSynthese");

            Student_SQLDTO? student = classroom.Students.FirstOrDefault(a => a.Username == "ikeameatbol");

            Assert.Null(student);

            classroomBD[0].Students.Add(new Student_SQLDTO()
            {
                Username = "ikeameatbol",
                FirstName = "Jonathan",
                LastName = "Blouin",
                Email = "ikeameatbol@hotmail.com",
                Matricule = "1122334",
                Active = true
            });

            depot.AddStudentToClassroom("ProjetSynthese", "ikeameatbol");

            classroom = classroomBD.FirstOrDefault(c => c.Name == "ProjetSynthese");
            student = classroom.Students.FirstOrDefault(a => a.Username == "ikeameatbol");

            Assert.NotNull(student);
           
            
        }

        [Fact]
        public void Test_AddTeacherToClassroom()
        {
            List<Classroom_SQLDTO> classroomBD = new List<Classroom_SQLDTO>
            {
                new Classroom_SQLDTO
                {
                    Name = "ProjetSynthese",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                },
                new Classroom_SQLDTO()
                {
                    Name = "OOP",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                }
            };


            classroomBD[0].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "Review",
                ClassroomId = 1,
                DistributionDate = System.DateTime.Now,
                Description = "Review a partner\'s code",
                DeliveryDeadline = System.DateTime.Now.AddDays(1),
                Active = true
            });

            classroomBD[0].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "Scrum",
                ClassroomId = 1,
                DistributionDate = System.DateTime.Now,
                Description = "Daily briefing",
                DeliveryDeadline = System.DateTime.Now.AddDays(1),
                Active = true
            });

            classroomBD[0].Students.Add(new Student_SQLDTO()
            {
                Username = "ThPaquet",
                FirstName = "Thierry",
                LastName = "Paquet",
                Email = "ThPaquet@hotmail.com",
                Matricule = "1141200",
                Active = true
            });

            classroomBD[0].Teachers.Add(new Teacher_SQLDTO()
            {
                Username = "PiFou86",
                FirstName = "Pierre-Francois",
                LastName = "Leon",
                Email = "PiFou86@hotmail.com",
                Active = true
            });
            classroomBD[1].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "UnitTests",
                ClassroomId = 2,
                DistributionDate = System.DateTime.Now,
                Description = "Review a partner\'s code",
                DeliveryDeadline = System.DateTime.Now.AddDays(1)
            });

            classroomBD[1].Students.Add(new Student_SQLDTO()
            {
                Username = "ikeameatbol",
                FirstName = "Jonathan",
                LastName = "Blouin",
                Email = "ikeameatbol@hotmail.com",
                Matricule = "1122334",
                Active = true
            });

            classroomBD[1].Teachers.Add(new Teacher_SQLDTO()
            {
                Username = "BACenComm",
                FirstName = "Melissa",
                LastName = "Lachapelle",
                Email = "BACenComm@hotmail.com",
                Active = true
            });

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
                            OrganisationId = 1,
                            Active = true
                        },
                        new Classroom_SQLDTO()
                        {
                            Name = "RPLP",
                            OrganisationId = 1,
                            Active = true
                        },
                        new Classroom_SQLDTO()
                        {
                            Name = "OOP",
                            OrganisationId = 1,
                            Active = false
                        }
                    },
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
            context.Setup(x => x.Classrooms).ReturnsDbSet(classroomBD);
            DepotClassroom depot = new DepotClassroom(context.Object);

            Classroom_SQLDTO? classroom = classroomBD.FirstOrDefault(c => c.Name == "ProjetSynthese");
            Teacher_SQLDTO? teacher = classroom.Teachers.FirstOrDefault(a => a.Username == "ikeameatbol");

            Assert.Null(teacher);

            classroomBD[0].Teachers.Add(new Teacher_SQLDTO()
            {
                Username = "ikeameatbol",
                FirstName = "Jonathan",
                LastName = "Blouin",
                Email = "ikeameatbol@hotmail.com",
                Active = true
            });

            depot.AddTeacherToClassroom("ProjetSynthese", "ikeameatbol");

            classroom = classroomBD.FirstOrDefault(c => c.Name == "ProjetSynthese");
            teacher = classroom.Teachers.FirstOrDefault(a => a.Username == "ikeameatbol");

            Assert.NotNull(teacher);
           
            
        }

        [Fact]
        public void Test_RemoveAssignmentFromClassroom()
        {
            List<Classroom_SQLDTO> classroomBD = new List<Classroom_SQLDTO>
            {
                new Classroom_SQLDTO
                {
                    Name = "ProjetSynthese",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                },
                new Classroom_SQLDTO()
                {
                    Name = "OOP",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                }
            };

            classroomBD[0].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "Scrum",
                ClassroomId = 1,
                DistributionDate = System.DateTime.Now,
                Description = "Daily briefing",
                DeliveryDeadline = System.DateTime.Now.AddDays(1),
                Active = true
            });

            classroomBD[0].Students.Add(new Student_SQLDTO()
            {
                Username = "ThPaquet",
                FirstName = "Thierry",
                LastName = "Paquet",
                Email = "ThPaquet@hotmail.com",
                Matricule = "1141200",
                Active = true
            });

            classroomBD[0].Teachers.Add(new Teacher_SQLDTO()
            {
                Username = "PiFou86",
                FirstName = "Pierre-Francois",
                LastName = "Leon",
                Email = "PiFou86@hotmail.com",
                Active = true
            });
            classroomBD[1].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "UnitTests",
                ClassroomId = 2,
                DistributionDate = System.DateTime.Now,
                Description = "Review a partner\'s code",
                DeliveryDeadline = System.DateTime.Now.AddDays(1)
            });

            classroomBD[1].Students.Add(new Student_SQLDTO()
            {
                Username = "ikeameatbol",
                FirstName = "Jonathan",
                LastName = "Blouin",
                Email = "ikeameatbol@hotmail.com",
                Matricule = "1122334",
                Active = true
            });

            classroomBD[1].Teachers.Add(new Teacher_SQLDTO()
            {
                Username = "BACenComm",
                FirstName = "Melissa",
                LastName = "Lachapelle",
                Email = "BACenComm@hotmail.com",
                Active = true
            });

            List<Assignment_SQLDTO> assignmentsBD = new List<Assignment_SQLDTO>()
            {
                new Assignment_SQLDTO()
                {
                    Name = "Review",
                    ClassroomId = 1,
                    DistributionDate = System.DateTime.Now,
                    Description = "Review a partner\'s code",
                    DeliveryDeadline = System.DateTime.Now.AddDays(1),
                    Active = true
                },
                new Assignment_SQLDTO()
                {
                    Name = "AnotherOne",
                    ClassroomId = 3,
                    DistributionDate = System.DateTime.Now,
                    Description = "Review another partner\'s code",
                    DeliveryDeadline = System.DateTime.Now.AddDays(1),
                    Active = true
                }
            };

            classroomBD[0].Assignments.Add(assignmentsBD[0]);

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Assignments).ReturnsDbSet(assignmentsBD);
            context.Setup(x => x.Classrooms).ReturnsDbSet(classroomBD);
            DepotClassroom depot = new DepotClassroom(context.Object);

            Classroom_SQLDTO? classroom = classroomBD.FirstOrDefault(c => c.Name == "ProjetSynthese");
            Assignment_SQLDTO assignment = classroom.Assignments.FirstOrDefault(a => a.Name == "Review");


            Assert.NotNull(classroom);
            Assert.NotNull(assignment);
            Assert.Contains(classroom.Assignments, a => a.Id == assignment.Id);

            depot.RemoveAssignmentFromClassroom("ProjetSynthese", "Review");

            classroom = classroomBD.FirstOrDefault(c => c.Name == "ProjetSynthese");
            assignment = classroom.Assignments.FirstOrDefault(a => a.Name == "Review");

            Assert.Null(assignment);
           
            
        }

        [Fact]
        public void Test_RemoveStudentFromClassroom()
        {
            List<Classroom_SQLDTO> classroomBD = new List<Classroom_SQLDTO>
            {
                new Classroom_SQLDTO
                {
                    Name = "ProjetSynthese",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                },
                new Classroom_SQLDTO()
                {
                    Name = "OOP",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                }
            };


            classroomBD[0].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "Review",
                ClassroomId = 1,
                DistributionDate = System.DateTime.Now,
                Description = "Review a partner\'s code",
                DeliveryDeadline = System.DateTime.Now.AddDays(1),
                Active = true
            });

            classroomBD[0].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "Scrum",
                ClassroomId = 1,
                DistributionDate = System.DateTime.Now,
                Description = "Daily briefing",
                DeliveryDeadline = System.DateTime.Now.AddDays(1),
                Active = true
            });

            classroomBD[0].Teachers.Add(new Teacher_SQLDTO()
            {
                Username = "PiFou86",
                FirstName = "Pierre-Francois",
                LastName = "Leon",
                Email = "PiFou86@hotmail.com",
                Active = true
            });

            classroomBD[1].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "UnitTests",
                ClassroomId = 2,
                DistributionDate = System.DateTime.Now,
                Description = "Review a partner\'s code",
                DeliveryDeadline = System.DateTime.Now.AddDays(1)
            });
            classroomBD[1].Students.Add(new Student_SQLDTO()
            {
                Username = "ikeameatbol",
                FirstName = "Jonathan",
                LastName = "Blouin",
                Email = "ikeameatbol@hotmail.com",
                Matricule = "1122334",
                Active = true
            });

            classroomBD[1].Teachers.Add(new Teacher_SQLDTO()
            {
                Username = "BACenComm",
                FirstName = "Melissa",
                LastName = "Lachapelle",
                Email = "BACenComm@hotmail.com",
                Active = true
            });
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
                            OrganisationId = 1,
                            Active = true
                        },
                        new Classroom_SQLDTO()
                        {
                            Name = "RPLP",
                            OrganisationId = 1,
                            Active = true
                        },
                        new Classroom_SQLDTO()
                        {
                            Name = "OOP",
                            OrganisationId = 1,
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

            classroomBD[0].Students.Add(studentsBD[0]);

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Students).ReturnsDbSet(studentsBD);
            context.Setup(x => x.Classrooms).ReturnsDbSet(classroomBD);
            DepotClassroom depot = new DepotClassroom(context.Object);

            Classroom_SQLDTO? classroom = classroomBD.FirstOrDefault(c => c.Name == "ProjetSynthese");

            Student_SQLDTO student = classroom.Students.FirstOrDefault(a => a.Username == "ThPaquet");

            Assert.NotNull(classroom);
            Assert.NotNull(student);
            Assert.Contains<Student_SQLDTO>(student, classroom.Students);

            depot.RemoveStudentFromClassroom("ProjetSynthese", "ThPaquet");

            classroom = classroomBD.FirstOrDefault(c => c.Name == "ProjetSynthese");
            student = classroom.Students.FirstOrDefault(a => a.Username == "ThPaquet");

            Assert.Null(student);
           
            
        }

        [Fact]
        public void Test_RemoveTeacherFromClassroom()
        {
            List<Classroom_SQLDTO> classroomBD = new List<Classroom_SQLDTO>
            {
                new Classroom_SQLDTO
                {
                    Name = "ProjetSynthese",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                },
                new Classroom_SQLDTO()
                {
                    Name = "OOP",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                }
            };


            classroomBD[0].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "Review",
                ClassroomId = 1,
                DistributionDate = System.DateTime.Now,
                Description = "Review a partner\'s code",
                DeliveryDeadline = System.DateTime.Now.AddDays(1),
                Active = true
            });

            classroomBD[0].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "Scrum",
                ClassroomId = 1,
                DistributionDate = System.DateTime.Now,
                Description = "Daily briefing",
                DeliveryDeadline = System.DateTime.Now.AddDays(1),
                Active = true
            });

            classroomBD[0].Students.Add(new Student_SQLDTO()
            {
                Username = "ThPaquet",
                FirstName = "Thierry",
                LastName = "Paquet",
                Email = "ThPaquet@hotmail.com",
                Matricule = "1141200",
                Active = true
            });

            classroomBD[1].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "UnitTests",
                ClassroomId = 2,
                DistributionDate = System.DateTime.Now,
                Description = "Review a partner\'s code",
                DeliveryDeadline = System.DateTime.Now.AddDays(1)
            });
            classroomBD[1].Students.Add(new Student_SQLDTO()
            {
                Username = "ikeameatbol",
                FirstName = "Jonathan",
                LastName = "Blouin",
                Email = "ikeameatbol@hotmail.com",
                Matricule = "1122334",
                Active = true
            });

            classroomBD[1].Teachers.Add(new Teacher_SQLDTO()
            {
                Username = "BACenComm",
                FirstName = "Melissa",
                LastName = "Lachapelle",
                Email = "BACenComm@hotmail.com",
                Active = true
            });

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
                            OrganisationId = 1,
                            Active = true
                        },
                        new Classroom_SQLDTO()
                        {
                            Name = "RPLP",
                            OrganisationId = 1,
                            Active = true
                        },
                        new Classroom_SQLDTO()
                        {
                            Name = "OOP",
                            OrganisationId = 1,
                            Active = false
                        }
                    },
                    Active = true
                },
                new Teacher_SQLDTO()
                {
                    Username = "PiFou86",
                    FirstName = "Pierre-Francois",
                    LastName = "Leon",
                    Email = "PiFou86@hotmail.com",
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

            classroomBD[0].Teachers.Add(teachersDB[1]);

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Teachers).ReturnsDbSet(teachersDB);
            context.Setup(x => x.Classrooms).ReturnsDbSet(classroomBD);
            DepotClassroom depot = new DepotClassroom(context.Object);

            Classroom_SQLDTO? classroom = classroomBD.FirstOrDefault(c => c.Name == "ProjetSynthese");
            Teacher_SQLDTO teacher = classroom.Teachers.FirstOrDefault(a => a.Username == "PiFou86");


            Assert.NotNull(teacher);

            depot.RemoveTeacherFromClassroom("ProjetSynthese", "PiFou86");

            classroom = classroomBD.FirstOrDefault(c => c.Name == "ProjetSynthese");
            teacher = classroom.Teachers.FirstOrDefault(a => a.Username == "PiFou86");

            Assert.Null(teacher);
           
            
        }

        [Fact]
        public void Test_UpsertClassroom_Inserts()
        {
            List<Classroom_SQLDTO> classroomBD = new List<Classroom_SQLDTO>
            {
                new Classroom_SQLDTO
                {
                    Name = "ProjetSynthese",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                },
                new Classroom_SQLDTO()
                {
                    Name = "OOP",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                }
            };

            Organisation mockOrganisation = new Organisation()
            {
                Administrators = new List<Administrator>(),
                Id = 1,
                Name = "Mock Organisation"
            };


            classroomBD[0].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "Review",
                ClassroomId = 1,
                DistributionDate = System.DateTime.Now,
                Description = "Review a partner\'s code",
                DeliveryDeadline = System.DateTime.Now.AddDays(1),
                Active = true
            });

            classroomBD[0].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "Scrum",
                ClassroomId = 1,
                DistributionDate = System.DateTime.Now,
                Description = "Daily briefing",
                DeliveryDeadline = System.DateTime.Now.AddDays(1),
                Active = true
            });

            classroomBD[0].Students.Add(new Student_SQLDTO()
            {
                Username = "ThPaquet",
                FirstName = "Thierry",
                LastName = "Paquet",
                Email = "ThPaquet@hotmail.com",
                Matricule = "1141200",
                Active = true
            });

            classroomBD[0].Teachers.Add(new Teacher_SQLDTO()
            {
                Username = "PiFou86",
                FirstName = "Pierre-Francois",
                LastName = "Leon",
                Email = "PiFou86@hotmail.com",
                Active = true
            });

            classroomBD[1].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "UnitTests",
                ClassroomId = 2,
                DistributionDate = System.DateTime.Now,
                Description = "Review a partner\'s code",
                DeliveryDeadline = System.DateTime.Now.AddDays(1)
            });
            classroomBD[1].Students.Add(new Student_SQLDTO()
            {
                Username = "ikeameatbol",
                FirstName = "Jonathan",
                LastName = "Blouin",
                Email = "ikeameatbol@hotmail.com",
                Matricule = "1122334",
                Active = true
            });

            classroomBD[1].Teachers.Add(new Teacher_SQLDTO()
            {
                Username = "BACenComm",
                FirstName = "Melissa",
                LastName = "Lachapelle",
                Email = "BACenComm@hotmail.com",
                Active = true
            });

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Classrooms).ReturnsDbSet(classroomBD);
            DepotClassroom depot = new DepotClassroom(context.Object);

            Classroom_SQLDTO? classroom = classroomBD.FirstOrDefault(c => c.Name == "RPLP");
            Assert.Null(classroom);

            Classroom newClassroom = new Classroom()
            {
                Name = "RPLP",
                Organisation = mockOrganisation,
                Assignments = new List<Assignment>(),
                Students = new List<Student>(),
                Teachers = new List<Teacher>()
            };

            depot.UpsertClassroom(newClassroom);

            classroom = classroomBD.FirstOrDefault(c => c.Name == "RPLP");
            Assert.NotNull(classroom);
           
            
        }

        [Fact]
        public void Test_UpsertClassroom_Updates()
        {
            List<Classroom_SQLDTO> classroomBD = new List<Classroom_SQLDTO>
            {
                new Classroom_SQLDTO
                {
                    Name = "ProjetSynthese",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                },
                new Classroom_SQLDTO()
                {
                    Name = "OOP",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                }
            };


            classroomBD[0].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "Review",
                ClassroomId = 1,
                DistributionDate = System.DateTime.Now,
                Description = "Review a partner\'s code",
                DeliveryDeadline = System.DateTime.Now.AddDays(1),
                Active = true
            });

            classroomBD[0].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "Scrum",
                ClassroomId = 1,
                DistributionDate = System.DateTime.Now,
                Description = "Daily briefing",
                DeliveryDeadline = System.DateTime.Now.AddDays(1),
                Active = true
            });

            classroomBD[0].Students.Add(new Student_SQLDTO()
            {
                Username = "ThPaquet",
                FirstName = "Thierry",
                LastName = "Paquet",
                Email = "ThPaquet@hotmail.com",
                Matricule = "1141200",
                Active = true
            });

            classroomBD[0].Teachers.Add(new Teacher_SQLDTO()
            {
                Username = "PiFou86",
                FirstName = "Pierre-Francois",
                LastName = "Leon",
                Email = "PiFou86@hotmail.com",
                Active = true
            });
            classroomBD[1].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "UnitTests",
                ClassroomId = 2,
                DistributionDate = System.DateTime.Now,
                Description = "Review a partner\'s code",
                DeliveryDeadline = System.DateTime.Now.AddDays(1)
            });

            classroomBD[1].Students.Add(new Student_SQLDTO()
            {
                Username = "ikeameatbol",
                FirstName = "Jonathan",
                LastName = "Blouin",
                Email = "ikeameatbol@hotmail.com",
                Matricule = "1122334",
                Active = true
            });

            classroomBD[1].Teachers.Add(new Teacher_SQLDTO()
            {
                Username = "BACenComm",
                FirstName = "Melissa",
                LastName = "Lachapelle",
                Email = "BACenComm@hotmail.com",
                Active = true
            });

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Classrooms).ReturnsDbSet(classroomBD);
            DepotClassroom depot = new DepotClassroom(context.Object);

            Classroom_SQLDTO? classroom = classroomBD.FirstOrDefault(c => c.Name == "ProjetSynthese");
            Assert.NotNull(classroom);

            classroom.OrganisationId = 2;
            classroom.Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "TestAssignment",
                ClassroomId = 1,
                DistributionDate = System.DateTime.Now,
                Description = "AssignmentUpsertTest",
                DeliveryDeadline = System.DateTime.Now.AddDays(1),
                Active = true
            });

            classroom.Students.Add(new Student_SQLDTO()
            {
                Username = "TestStudent",
                FirstName = "Testy",
                LastName = "McTestson",
                Email = "Test@hotmail.com",
                Matricule = "1133221",
                Active = true
            });

            classroom.Teachers.Add(new Teacher_SQLDTO()
            {
                Username = "TestTeacher",
                FirstName = "Test-Francois",
                LastName = "Testeon",
                Email = "Testeon@hotmail.com",
                Active = true
            });

            depot.UpsertClassroom(classroom.ToEntity());

            classroom = classroomBD.FirstOrDefault(c => c.Name == "ProjetSynthese");

            Assert.NotNull(classroom);
            Assert.NotNull(classroom.Assignments.SingleOrDefault(a => a.Name == "TestAssignment"));
            Assert.NotNull(classroom.Students.SingleOrDefault(a => a.Username == "TestStudent"));
            Assert.NotNull(classroom.Teachers.SingleOrDefault(a => a.Username == "TestTeacher"));
           
            
        }

        [Fact]
        public void Test_DeleteClassroom()
        {
            List<Classroom_SQLDTO> classroomBD = new List<Classroom_SQLDTO>
            {
                new Classroom_SQLDTO
                {
                    Name = "ProjetSynthese",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                },
                new Classroom_SQLDTO()
                {
                    Name = "OOP",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                }
            };


            classroomBD[0].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "Review",
                ClassroomId = 1,
                DistributionDate = System.DateTime.Now,
                Description = "Review a partner\'s code",
                DeliveryDeadline = System.DateTime.Now.AddDays(1),
                Active = true
            });

            classroomBD[0].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "Scrum",
                ClassroomId = 1,
                DistributionDate = System.DateTime.Now,
                Description = "Daily briefing",
                DeliveryDeadline = System.DateTime.Now.AddDays(1),
                Active = true
            });

            classroomBD[0].Students.Add(new Student_SQLDTO()
            {
                Username = "ThPaquet",
                FirstName = "Thierry",
                LastName = "Paquet",
                Email = "ThPaquet@hotmail.com",
                Matricule = "1141200",
                Active = true
            });

            classroomBD[0].Teachers.Add(new Teacher_SQLDTO()
            {
                Username = "PiFou86",
                FirstName = "Pierre-Francois",
                LastName = "Leon",
                Email = "PiFou86@hotmail.com",
                Active = true
            });
            classroomBD[1].Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "UnitTests",
                ClassroomId = 2,
                DistributionDate = System.DateTime.Now,
                Description = "Review a partner\'s code",
                DeliveryDeadline = System.DateTime.Now.AddDays(1)
            });

            classroomBD[1].Students.Add(new Student_SQLDTO()
            {
                Username = "ikeameatbol",
                FirstName = "Jonathan",
                LastName = "Blouin",
                Email = "ikeameatbol@hotmail.com",
                Matricule = "1122334",
                Active = true
            });

            classroomBD[1].Teachers.Add(new Teacher_SQLDTO()
            {
                Username = "BACenComm",
                FirstName = "Melissa",
                LastName = "Lachapelle",
                Email = "BACenComm@hotmail.com",
                Active = true
            });

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Classrooms).ReturnsDbSet(classroomBD);
            DepotClassroom depot = new DepotClassroom(context.Object);

            Classroom_SQLDTO? classroom = classroomBD.FirstOrDefault(c => c.Name == "ProjetSynthese");
            Assert.NotNull(classroom);

            depot.DeleteClassroom("ProjetSynthese");

            classroom = classroomBD.FirstOrDefault(c => c.Name == "ProjetSynthese" && c.Active == true);

            Assert.Null(classroom);
           
            
        }
    }
}
