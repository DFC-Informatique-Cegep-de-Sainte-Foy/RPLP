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
    public class TestsDepotAssignment
    {
        [Fact]
        public void Test_GetAssignments()
        {
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
                    ClassroomId = 1,
                    DistributionDate = System.DateTime.Now,
                    Description = "Review another partner\'s code",
                    DeliveryDeadline = System.DateTime.Now.AddDays(1),
                    Active = true
                }
            };
            List<Classroom_SQLDTO> classroomBD = new List<Classroom_SQLDTO>
            {
                new Classroom_SQLDTO
                {
                    Id= 1,
                    Name = "ProjetSynthese",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                },
                new Classroom_SQLDTO()
                {
                    Id= 2,
                    Name = "OOP",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Assignments).ReturnsDbSet(assignmentsBD);
            context.Setup(x => x.Classrooms).ReturnsDbSet(classroomBD);
            DepotAssignment depot = new DepotAssignment(context.Object);

            foreach (Assignment_SQLDTO assignment_SQLDTO in assignmentsBD)
            {
                assignment_SQLDTO.Classroom = classroomBD.Where(x => x.Id == assignment_SQLDTO.ClassroomId).FirstOrDefault();
            }

            List<Assignment> fetchedAssignments = depot.GetAssignments();

            Assert.Contains(fetchedAssignments, f => f.Name == "Review");
            Assert.Contains(fetchedAssignments, f => f.Name == "AnotherOne");
            Assert.Equal(2, fetchedAssignments.Count);
           
            
        }

        [Fact]
        public void Test_GetAssignmentById()
        {
            List<Assignment_SQLDTO> assignmentsBD = new List<Assignment_SQLDTO>()
            {
                new Assignment_SQLDTO()
                {
                    Id= 1,
                    Name = "Review",
                    ClassroomId = 1,
                    DistributionDate = System.DateTime.Now,
                    Description = "Review a partner\'s code",
                    DeliveryDeadline = System.DateTime.Now.AddDays(1),
                    Active = true
                },
                new Assignment_SQLDTO()
                {
                    Id= 2,
                    Name = "AnotherOne",
                    ClassroomId = 1,
                    DistributionDate = System.DateTime.Now,
                    Description = "Review another partner\'s code",
                    DeliveryDeadline = System.DateTime.Now.AddDays(1),
                    Active = true
                }
            };
            List<Classroom_SQLDTO> classroomBD = new List<Classroom_SQLDTO>
            {
                new Classroom_SQLDTO
                {
                    Id= 1,
                    Name = "ProjetSynthese",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                },
                new Classroom_SQLDTO()
                {
                    Id= 2,
                    Name = "OOP",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Assignments).ReturnsDbSet(assignmentsBD);
            DepotAssignment depot = new DepotAssignment(context.Object);

            int assignmentId = assignmentsBD.FirstOrDefault(a => a.Name == "Review").Id;

            foreach (Assignment_SQLDTO assignment_SQLDTO in assignmentsBD)
            {
                assignment_SQLDTO.Classroom = classroomBD.Where(x => x.Id == assignment_SQLDTO.ClassroomId).FirstOrDefault();
            }

            Assignment assignment = depot.GetAssignmentById(assignmentId);

            Assert.NotNull(assignment);
            Assert.Equal("Review", assignment.Name);
           
            
        }

        [Fact]
        public void Test_GetAssignmentByName()
        {
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
                    ClassroomId = 1,
                    DistributionDate = System.DateTime.Now,
                    Description = "Review another partner\'s code",
                    DeliveryDeadline = System.DateTime.Now.AddDays(1),
                    Active = true
                }
            };
            List<Classroom_SQLDTO> classroomBD = new List<Classroom_SQLDTO>
            {
                new Classroom_SQLDTO
                {
                    Id= 1,
                    Name = "ProjetSynthese",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                },
                new Classroom_SQLDTO()
                {
                    Id= 2,
                    Name = "OOP",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Assignments).ReturnsDbSet(assignmentsBD);
            DepotAssignment depot = new DepotAssignment(context.Object);

            foreach (Assignment_SQLDTO assignment_SQLDTO in assignmentsBD)
            {
                assignment_SQLDTO.Classroom = classroomBD.Where(x => x.Id == assignment_SQLDTO.ClassroomId).FirstOrDefault();
            }

            Assignment assignment = depot.GetAssignmentByName("Review");

            Assert.NotNull(assignment);
           
            
        }

        [Fact]
        public void Test_GetAssignmentsByClassroomName()
        {
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
                    ClassroomId = 1,
                    DistributionDate = System.DateTime.Now,
                    Description = "Review another partner\'s code",
                    DeliveryDeadline = System.DateTime.Now.AddDays(1),
                    Active = true
                }
            };
            List<Classroom_SQLDTO> classroomBD = new List<Classroom_SQLDTO>
            {
                new Classroom_SQLDTO
                {
                    Id= 1,
                    Name = "ProjetSynthese",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                },
                new Classroom_SQLDTO()
                {
                    Id= 2,
                    Name = "OOP",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Assignments).ReturnsDbSet(assignmentsBD);
            context.Setup(x => x.Classrooms).ReturnsDbSet(classroomBD);
            DepotAssignment depot = new DepotAssignment(context.Object);

            foreach (Assignment_SQLDTO assignment_SQLDTO in assignmentsBD)
            {
                assignment_SQLDTO.Classroom = classroomBD.Where(x => x.Id == assignment_SQLDTO.ClassroomId).FirstOrDefault();
            }

            List<Assignment> assignments = depot.GetAssignmentsByClassroomName("ProjetSynthese");

            Assert.True(assignments.Count == 2);
           
            
        }

        [Fact]
        public void Test_UpsertAssignment_Inserts()
        {
            List<Assignment_SQLDTO> assignmentsBD = new List<Assignment_SQLDTO>()
            {
                new Assignment_SQLDTO()
                {
                    Id = 1,
                    Name = "AnotherOne",
                    ClassroomId = 1,
                    DistributionDate = System.DateTime.Now,
                    Description = "Review another partner\'s code",
                    DeliveryDeadline = System.DateTime.Now.AddDays(1),
                    Active = true
                }
            };

            List<Classroom_SQLDTO> classroomBD = new List<Classroom_SQLDTO>
            {
                new Classroom_SQLDTO
                {
                    Id= 1,
                    Name = "ProjetSynthese",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                },
                new Classroom_SQLDTO()
                {
                    Id= 2,
                    Name = "OOP",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                }
            };
            Organisation organisation = new Organisation()
            {               
                    Id = 1,
                    Name = "CEGEP Ste-Foy",
                    Administrators = new List<Administrator>(),              
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Assignments).ReturnsDbSet(assignmentsBD);
            context.Setup(x => x.Classrooms).ReturnsDbSet(classroomBD);
            context.Setup(m => m.Assignments.Add(It.IsAny<Assignment_SQLDTO>())).Callback<Assignment_SQLDTO>(assignmentsBD.Add);
            DepotAssignment depot = new DepotAssignment(context.Object);

            foreach (Assignment_SQLDTO assignment_SQLDTO in assignmentsBD)
            {
                assignment_SQLDTO.Classroom = classroomBD.Where(x => x.Id == assignment_SQLDTO.ClassroomId).FirstOrDefault();
            }

            Classroom mockClassroom = new Classroom()
            {
                Id = 3,
                Name = "RPLP",
                Organisation = organisation,
            }; 

            Assignment assignment = new Assignment()
            {
                Id = 2,
                Name = "Review",
                Classroom = mockClassroom,
                DistributionDate = System.DateTime.Now,
                Description = "Review a partner\'s code",
                DeliveryDeadline = System.DateTime.Now.AddDays(1)
            };

            depot.UpsertAssignment(assignment);

            Assignment_SQLDTO? assignmentSQL = assignmentsBD.FirstOrDefault(a => a.Name == "Review");

            Assert.NotNull(assignmentSQL);
           
            

        }

        [Fact]
        public void Test_UpsertAssignment_Updates()
        {
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
                    Id = 1,
                    Name = "AnotherOne",
                    ClassroomId = 1,
                    DistributionDate = System.DateTime.Now,
                    Description = "Review another partner\'s code",
                    DeliveryDeadline = System.DateTime.Now.AddDays(1),
                    Active = true
                }
            };

            List<Classroom_SQLDTO> classroomBD = new List<Classroom_SQLDTO>
            {
                new Classroom_SQLDTO
                {
                    Id= 1,
                    Name = "ProjetSynthese",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                },
                new Classroom_SQLDTO()
                {
                    Id= 2,
                    Name = "OOP",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                }
            };
            Organisation_SQLDTO organisation = new Organisation_SQLDTO()
            {
                Id = 1,
                Name = "CEGEP Ste-Foy",
                Administrators = new List<Administrator_SQLDTO>(),
            };

            classroomBD[0].Organisation = organisation;
            assignmentsBD[0].Classroom = classroomBD[0];

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Assignments).ReturnsDbSet(assignmentsBD);
            DepotAssignment depot = new DepotAssignment(context.Object);

            Assignment? assignment = assignmentsBD.FirstOrDefault(a => a.Name == "Review").ToEntity();

            assignment.Name = "Modified";

            depot.UpsertAssignment(assignment);

            Assignment_SQLDTO? nonModifiedAssignment = assignmentsBD.FirstOrDefault(a => a.Name == "Review");
            Assignment_SQLDTO? modifiedAssignment = assignmentsBD.FirstOrDefault(a => a.Name == "Modified");

            Assert.Null(nonModifiedAssignment);
            Assert.NotNull(modifiedAssignment);         
        }

        [Fact]
        public void Test_DeleteAssignment()
        {
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
                    ClassroomId = 1,
                    DistributionDate = System.DateTime.Now,
                    Description = "Review another partner\'s code",
                    DeliveryDeadline = System.DateTime.Now.AddDays(1),
                    Active = true
                }
            };
            List<Classroom_SQLDTO> classroomsDTO = new List<Classroom_SQLDTO>()
            {
                new Classroom_SQLDTO()
                {
                    Id = 1,
                    Name = "Classroom1",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                },

                new Classroom_SQLDTO()
                {
                    Id = 2,
                    Name = "Classroom2",
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                }
           };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Assignments).ReturnsDbSet(assignmentsBD);
            context.Setup(x => x.Classrooms).ReturnsDbSet(classroomsDTO);
            DepotAssignment depot = new DepotAssignment(context.Object);

            string assignmentName = "Review";

            Assert.NotNull(assignmentsBD.FirstOrDefault(a => a.Name == assignmentName && a.Active == true));

            Assignment_SQLDTO assignment = assignmentsBD.Where(x => x.Name == assignmentName).FirstOrDefault();
            assignment.Classroom = classroomsDTO.Where(x => x.Id == assignment.ClassroomId).FirstOrDefault();

            depot.DeleteAssignment("Review");

            Assert.Null(assignmentsBD.FirstOrDefault(a => a.Name == "Review" && a.Active == true));
           
            
        }
    }
}