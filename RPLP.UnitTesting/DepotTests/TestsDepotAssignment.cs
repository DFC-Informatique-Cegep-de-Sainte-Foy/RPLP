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

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Assignments).ReturnsDbSet(assignmentsBD);
            DepotAssignment depot = new DepotAssignment(context.Object);

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

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Assignments).ReturnsDbSet(assignmentsBD);
            DepotAssignment depot = new DepotAssignment(context.Object);

            int assignmentId = assignmentsBD.FirstOrDefault(a => a.Name == "Review").Id;
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

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Assignments).ReturnsDbSet(assignmentsBD);
            DepotAssignment depot = new DepotAssignment(context.Object);

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

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Assignments).ReturnsDbSet(assignmentsBD);
            DepotAssignment depot = new DepotAssignment(context.Object);

            List<Assignment> assignments = depot.GetAssignmentsByClassroomName("RPLP");

            Assert.True(assignments.Count == 2);
           
            
        }

        [Fact]
        public void Test_UpsertAssignment_Inserts()
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

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Assignments).ReturnsDbSet(assignmentsBD);
            DepotAssignment depot = new DepotAssignment(context.Object);

            Assignment assignment = new Assignment()
            {
                Name = "Review",
                ClassroomId = 1,
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
                    Name = "AnotherOne",
                    ClassroomId = 1,
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

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Assignments).ReturnsDbSet(assignmentsBD);
            DepotAssignment depot = new DepotAssignment(context.Object);

            Assert.NotNull(assignmentsBD.FirstOrDefault(a => a.Name == "Review" && a.Active == true));

            depot.DeleteAssignment("Review");

            Assert.Null(assignmentsBD.FirstOrDefault(a => a.Name == "Review" && a.Active == true));
           
            
        }
    }
}