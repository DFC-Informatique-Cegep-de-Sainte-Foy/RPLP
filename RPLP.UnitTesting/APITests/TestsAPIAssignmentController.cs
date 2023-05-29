using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using RPLP.API.Controllers;
using RPLP.DAL.DTO.Sql;
using RPLP.DAL.SQL;
using RPLP.DAL.SQL.Depots;
using RPLP.ENTITES;
using RPLP.ENTITES.InterfacesDepots;
using RPLP.JOURNALISATION;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RPLP.UnitTesting.APITests
{
    public class TestsAPIAssignmentController
    {
        [Fact]
        public void Test_GetAssignments()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotAssignment> depot = new Mock<IDepotAssignment>();
            List<Assignment> assignmentsInMockDepot = new List<Assignment>()
            {
                new Assignment()
                {
                    Id = 1,
                    Name = "RPLP"
                },
                new Assignment()
                {
                    Id = 2,
                    Name = "Scrum"
                }
            };

            depot.Setup(d => d.GetAssignments()).Returns(assignmentsInMockDepot);

            AssignmentController controller = new AssignmentController(depot.Object);

            var response = controller.Get();
            var result = Assert.IsType<OkObjectResult>(response.Result);

            depot.Verify(d => d.GetAssignments(), Times.Once);
            Assert.NotNull(result);

            List<Assignment> assignments = result.Value as List<Assignment>;

            Assert.Equal(2, assignments.Count);
        }

        [Fact]
        public void Test_GetAssignmentById()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotAssignment> depot = new Mock<IDepotAssignment>();
            Assignment assignment = new Assignment()
            {
                Id = 1,
                Name = "RPLP"
            };


            depot.Setup(d => d.GetAssignmentById(1)).Returns(assignment);

            AssignmentController controller = new AssignmentController(depot.Object);

            var response = controller.GetAssignmentById(1);
            var result = Assert.IsType<OkObjectResult>(response.Result);

            depot.Verify(d => d.GetAssignmentById(1), Times.Once);
            Assert.NotNull(result);

            Assignment assignments = result.Value as Assignment;

            Assert.Equal("RPLP", assignment.Name);
        }

        [Fact]
        public void Test_GetAssignmentByName()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotAssignment> depot = new Mock<IDepotAssignment>();
            AssignmentController controller = new AssignmentController(depot.Object);

            Assignment assignment = new Assignment()
            {
                Id = 1,
                Name = "RPLP"
            };


            depot.Setup(d => d.GetAssignmentByName("RPLP")).Returns(assignment);

           

            var response = controller.GetAssignmentByName("RPLP");
            var result = Assert.IsType<OkObjectResult>(response.Result);

            depot.Verify(d => d.GetAssignmentByName("RPLP"), Times.Once);
            Assert.NotNull(result);

            assignment = result.Value as Assignment;

            Assert.Equal("RPLP", assignment.Name);
        }

        [Fact]
        public void Test_GetAssignmentByClassroomName()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotAssignment> depot = new Mock<IDepotAssignment>();
            AssignmentController controller = new AssignmentController(depot.Object);

            Classroom mockClassroom = new Classroom()
            {
                Id = 1,
                Name = "ProjetSynthese"
            };

            List<Assignment> assignments = new List<Assignment>()
            {
                new Assignment()
                {
                    Id = 1,
                    Name = "RPLP",
                    Classroom = mockClassroom
                },
                new Assignment()
                {
                    Id = 2,
                    Name = "Scrum",
                    Classroom = mockClassroom
                }
            };


            depot.Setup(d => d.GetAssignmentsByClassroomName("RPLP")).Returns(assignments);

            var response = controller.GetAssignmentsByClassroomName("RPLP");
            var result = Assert.IsType<OkObjectResult>(response.Result);

            depot.Verify(d => d.GetAssignmentsByClassroomName("RPLP"), Times.Once);
            Assert.NotNull(result);

            assignments = result.Value as List<Assignment>;

            Assert.Contains(assignments, a => a.Classroom.Id == 1);
        }

        [Fact]
        public void Test_Post_AssignmentNull()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotAssignment> depot = new Mock<IDepotAssignment>();
            AssignmentController controller = new AssignmentController(depot.Object);

            var response = controller.Post(null);
            var result = Assert.IsType<BadRequestResult>;
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_Post_NotValidModelState()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotAssignment> depot = new Mock<IDepotAssignment>();
            AssignmentController controller = new AssignmentController(depot.Object);
            Assignment assignment = new Assignment();
            controller.ModelState.AddModelError("", "ModelState not valid Mock");

            var response = controller.Post(assignment);
            var result = Assert.IsType<BadRequestResult>;
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_Post_Created()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotAssignment> depot = new Mock<IDepotAssignment>();
            AssignmentController controller = new AssignmentController(depot.Object);
            Assignment assignment = new Assignment();

            var response = controller.Post(assignment);
            var result = Assert.IsType<CreatedResult>;

            depot.Verify(d => d.UpsertAssignment(assignment), Times.Once);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_DeleteAssignment()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotAssignment> depot = new Mock<IDepotAssignment>();
            AssignmentController controller = new AssignmentController(depot.Object);

            var response = controller.DeleteAssignment("RPLP");
            var result = Assert.IsType<NoContentResult>;

            depot.Verify(d => d.DeleteAssignment("RPLP"), Times.Once);
            Assert.NotNull(result);
        }
    }
}
