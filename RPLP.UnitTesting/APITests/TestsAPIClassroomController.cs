﻿using Microsoft.AspNetCore.Mvc;
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
    public class TestsAPIClassroomController
    {
        [Fact]
        public void Test_Get()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotClassroom> depot = new Mock<IDepotClassroom>();
            ClassroomController controller = new ClassroomController(depot.Object);
            List<Classroom> classroomsInMockDepot = new List<Classroom>()
            {
                new Classroom()
                {
                    Id = 1,
                    Name = "ProjetSynthese"
                },
                new Classroom()
                {
                    Id = 2,
                    Name = "Cybersecurity"
                }
            };

            depot.Setup(d => d.GetClassrooms()).Returns(classroomsInMockDepot);

            var response = controller.Get();
            var result = Assert.IsType<OkObjectResult>(response.Result);
            List<Classroom> classrooms = result.Value as List<Classroom>;

            depot.Verify(d => d.GetClassrooms(), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(2, classrooms.Count);
            Assert.Contains(classrooms, c => c.Name == "ProjetSynthese");
        }

        [Fact]
        public void Test_GetClassroomById()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotClassroom> depot = new Mock<IDepotClassroom>();
            ClassroomController controller = new ClassroomController(depot.Object);
            Classroom classroomInMockDepot = new Classroom()
            {
                Id = 1,
                Name = "RPLP"
            };

            depot.Setup(d => d.GetClassroomById(1)).Returns(classroomInMockDepot);

            var response = controller.GetClassroomById(1);
            var result = Assert.IsType<OkObjectResult>(response.Result);
            Classroom classroom = result.Value as Classroom;

            depot.Verify(d => d.GetClassroomById(1), Times.Once);
            Assert.NotNull(result);
            Assert.Equal("RPLP", classroom.Name);
        }

        [Fact]
        public void Test_GetClassroomByName()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotClassroom> depot = new Mock<IDepotClassroom>();
            ClassroomController controller = new ClassroomController(depot.Object);
            Classroom classroomInMockDepot = new Classroom()
            {
                Id = 1,
                Name = "RPLP"
            };

            depot.Setup(d => d.GetClassroomByName("RPLP")).Returns(classroomInMockDepot);

            var response = controller.GetClassroomByName("RPLP");
            var result = Assert.IsType<OkObjectResult>(response.Result);
            Classroom classroom = result.Value as Classroom;

            depot.Verify(d => d.GetClassroomByName("RPLP"), Times.Once);
            Assert.NotNull(result);
            Assert.Equal("RPLP", classroom.Name);
        }

        [Fact]
        public void Test_GetClassroomByClassroomName()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotClassroom> depot = new Mock<IDepotClassroom>();
            ClassroomController controller = new ClassroomController(depot.Object);
            List<Classroom> classroomsInMockDepot = new List<Classroom>()
            {
                new Classroom()
                {
                    Id = 1,
                    Name = "RPLP",
                    OrganisationName = "CEGEP Ste-Foy"
                },
                new Classroom()
                {
                    Id = 2,
                    Name = "OOP",
                    OrganisationName = "CEGEP Ste-Foy"
                }
            };

            depot.Setup(d => d.GetClassroomsByOrganisationName("CEGEP Ste-Foy")).Returns(classroomsInMockDepot);

            var response = controller.GetClassroomsByOrganisationName("CEGEP Ste-Foy");
            var result = Assert.IsType<OkObjectResult>(response.Result);
            List<Classroom> classrooms = result.Value as List<Classroom>;

            depot.Verify(d => d.GetClassroomsByOrganisationName("CEGEP Ste-Foy"), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(2, classrooms.Count);
            Assert.All(classrooms, c => c.OrganisationName.Equals("CEGEP Ste-Foy"));
        }

        [Fact]
        public void Test_GetTeachers()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotClassroom> depot = new Mock<IDepotClassroom>();
            ClassroomController controller = new ClassroomController(depot.Object);
            List<Teacher> teachersInMockDepot = new List<Teacher>()
            {
                new Teacher()
                {
                    Id = 1,
                    Username = "ThPaquet"
                },
                new Teacher()
                {
                    Id = 2,
                    Username = "BACenComm"
                }
            };

            depot.Setup(d => d.GetTeachersByClassroomName("ProjetSynthese")).Returns(teachersInMockDepot);

            var response = controller.GetTeachers("ProjetSynthese");
            var result = Assert.IsType<OkObjectResult>(response.Result);
            List<Teacher> teachers = result.Value as List<Teacher>;

            depot.Verify(d => d.GetTeachersByClassroomName("ProjetSynthese"), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(2, teachers.Count);
            Assert.Contains(teachers, t => t.Username == "ThPaquet");
        }

        [Fact]
        public void Test_GetStudents()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotClassroom> depot = new Mock<IDepotClassroom>();
            ClassroomController controller = new ClassroomController(depot.Object);
            List<Student> studentsInMockDepot = new List<Student>()
            {
                new Student()
                {
                    Id = 1,
                    Username = "ThPaquet"
                },
                new Student()
                {
                    Id = 2,
                    Username = "BACenComm"
                }
            };

            depot.Setup(d => d.GetStudentsByClassroomName("ProjetSynthese")).Returns(studentsInMockDepot);

            var response = controller.GetStudents("ProjetSynthese");
            var result = Assert.IsType<OkObjectResult>(response.Result);
            List<Student> student = result.Value as List<Student>;

            depot.Verify(d => d.GetStudentsByClassroomName("ProjetSynthese"), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(2, student.Count);
            Assert.Contains(student, s => s.Username == "ThPaquet");
        }

        [Fact]
        public void Test_GetAssignments()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotClassroom> depot = new Mock<IDepotClassroom>();
            ClassroomController controller = new ClassroomController(depot.Object);
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
                    Name = "Dar Si Hmad"
                }
            };

            depot.Setup(d => d.GetAssignmentsByClassroomName("ProjetSynthese")).Returns(assignmentsInMockDepot);

            var response = controller.GetAssignments("ProjetSynthese");
            var result = Assert.IsType<OkObjectResult>(response.Result);
            List<Assignment> assignments = result.Value as List<Assignment>;

            depot.Verify(d => d.GetAssignmentsByClassroomName("ProjetSynthese"), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(2, assignments.Count);
            Assert.Contains(assignments, a => a.Name == "RPLP");
        }

        [Fact]
        public void Test_GetAssignmentsByClassroomName()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotClassroom> depot = new Mock<IDepotClassroom>();
            ClassroomController controller = new ClassroomController(depot.Object);
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
                    Name = "Dar Si Hmad"
                }
            };

            depot.Setup(d => d.GetAssignmentsByClassroomName("ProjetSynthese")).Returns(assignmentsInMockDepot);

            var response = controller.GetAssignmentsByClassroomName("ProjetSynthese");
            var result = Assert.IsType<OkObjectResult>(response.Result);
            List<Assignment> assignments = result.Value as List<Assignment>;

            depot.Verify(d => d.GetAssignmentsByClassroomName("ProjetSynthese"), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(2, assignments.Count);
            Assert.Contains(assignments, a => a.Name == "RPLP");
        }

        [Fact]
        public void Test_AddTeacherToClassroom_NullOrWhitespaceClassroom()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotClassroom> depot = new Mock<IDepotClassroom>();
            ClassroomController controller = new ClassroomController(depot.Object);

            var response = controller.AddTeacherToClassroom("", "ThPaquet");
            var result = Assert.IsType<BadRequestResult>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_AddTeacherToClassroom_NullOrWhitespaceTeacher()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotClassroom> depot = new Mock<IDepotClassroom>();
            ClassroomController controller = new ClassroomController(depot.Object);

            var response = controller.AddTeacherToClassroom("RPLP", "");
            var result = Assert.IsType<BadRequestResult>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_AddTeacherToClassroom_Created()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotClassroom> depot = new Mock<IDepotClassroom>();
            ClassroomController controller = new ClassroomController(depot.Object);

            var response = controller.AddTeacherToClassroom("RPLP", "ThPaquet");
            var result = Assert.IsType<CreatedResult>(response);

            depot.Verify(d => d.AddTeacherToClassroom("RPLP", "ThPaquet"), Times.Once);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_AddStudentToClassroom_NullOrWhitespaceClassroom()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotClassroom> depot = new Mock<IDepotClassroom>();
            ClassroomController controller = new ClassroomController(depot.Object);

            var response = controller.AddStudentToClassroom("", "ThPaquet");
            var result = Assert.IsType<BadRequestResult>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_AddStudentToClassroom_NullOrWhitespaceTeacher()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotClassroom> depot = new Mock<IDepotClassroom>();
            ClassroomController controller = new ClassroomController(depot.Object);

            var response = controller.AddStudentToClassroom("RPLP", "");
            var result = Assert.IsType<BadRequestResult>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_AddStudentToClassroom_Created()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotClassroom> depot = new Mock<IDepotClassroom>();
            ClassroomController controller = new ClassroomController(depot.Object);

            var response = controller.AddStudentToClassroom("RPLP", "ThPaquet");
            var result = Assert.IsType<CreatedResult>(response);

            depot.Verify(d => d.AddStudentToClassroom("RPLP", "ThPaquet"), Times.Once);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_AddAssignmentToClassroom_NullOrWhitespaceClassroom()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotClassroom> depot = new Mock<IDepotClassroom>();
            ClassroomController controller = new ClassroomController(depot.Object);

            var response = controller.AddAssignmentToClassroom("", "RPLP");
            var result = Assert.IsType<BadRequestResult>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_AddAssignmentToClassroom_NullOrWhitespaceAssignment()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotClassroom> depot = new Mock<IDepotClassroom>();
            ClassroomController controller = new ClassroomController(depot.Object);

            var response = controller.AddAssignmentToClassroom("", "ProjetSynthese");
            var result = Assert.IsType<BadRequestResult>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_AddAssignmentToClassroom_Created()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotClassroom> depot = new Mock<IDepotClassroom>();
            ClassroomController controller = new ClassroomController(depot.Object);

            var response = controller.AddAssignmentToClassroom("ProjetSynthese", "RPLP");
            var result = Assert.IsType<CreatedResult>(response);

            depot.Verify(d => d.AddAssignmentToClassroom("ProjetSynthese", "RPLP"), Times.Once);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_RemoveTeacherFromClassroom_NullOrWhitespaceClassroom()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotClassroom> depot = new Mock<IDepotClassroom>();
            ClassroomController controller = new ClassroomController(depot.Object);

            var response = controller.RemoveTeacherFromClassroom("", "ThPaquet");
            var result = Assert.IsType<BadRequestResult>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_RemoveTeacherFromClassroom_NullOrWhitespaceTeacher()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotClassroom> depot = new Mock<IDepotClassroom>();
            ClassroomController controller = new ClassroomController(depot.Object);

            var response = controller.RemoveTeacherFromClassroom("RPLP", "");
            var result = Assert.IsType<BadRequestResult>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_RemoveTeacherFromClassroom_NoContent()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotClassroom> depot = new Mock<IDepotClassroom>();
            ClassroomController controller = new ClassroomController(depot.Object);

            var response = controller.RemoveTeacherFromClassroom("RPLP", "ThPaquet");
            var result = Assert.IsType<NoContentResult>(response);

            depot.Verify(d => d.RemoveTeacherFromClassroom("RPLP", "ThPaquet"), Times.Once);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_RemoveStudentFromClassroom_NullOrWhitespaceClassroom()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotClassroom> depot = new Mock<IDepotClassroom>();
            ClassroomController controller = new ClassroomController(depot.Object);

            var response = controller.RemoveStudentFromClassroom("", "ThPaquet");
            var result = Assert.IsType<BadRequestResult>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_RemoveStudentFromClassroom_NullOrWhitespaceTeacher()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotClassroom> depot = new Mock<IDepotClassroom>();
            ClassroomController controller = new ClassroomController(depot.Object);

            var response = controller.RemoveStudentFromClassroom("RPLP", "");
            var result = Assert.IsType<BadRequestResult>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_RemoveStudentFromClassroom_NoContent()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotClassroom> depot = new Mock<IDepotClassroom>();
            ClassroomController controller = new ClassroomController(depot.Object);

            var response = controller.RemoveStudentFromClassroom("RPLP", "ThPaquet");
            var result = Assert.IsType<NoContentResult>(response);

            depot.Verify(d => d.RemoveStudentFromClassroom("RPLP", "ThPaquet"), Times.Once);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_RemoveAssignmentFromClassroom_NullOrWhitespaceClassroom()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotClassroom> depot = new Mock<IDepotClassroom>();
            ClassroomController controller = new ClassroomController(depot.Object);

            var response = controller.RemoveAssignmentFromClassroom("", "RPLP");
            var result = Assert.IsType<BadRequestResult>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_RemoveAssignmentFromClassroom_NullOrWhitespaceAssignment()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotClassroom> depot = new Mock<IDepotClassroom>();
            ClassroomController controller = new ClassroomController(depot.Object);

            var response = controller.RemoveAssignmentFromClassroom("", "ProjetSynthese");
            var result = Assert.IsType<BadRequestResult>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_RemoveAssignmentFromClassroom_NoContent()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotClassroom> depot = new Mock<IDepotClassroom>();
            ClassroomController controller = new ClassroomController(depot.Object);

            var response = controller.RemoveAssignmentFromClassroom("ProjetSynthese", "RPLP");
            var result = Assert.IsType<NoContentResult>(response);

            depot.Verify(d => d.RemoveAssignmentFromClassroom("ProjetSynthese", "RPLP"), Times.Once);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_UpsertClassroom_NullClassroom()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotClassroom> depot = new Mock<IDepotClassroom>();
            ClassroomController controller = new ClassroomController(depot.Object);

            var response = controller.UpsertClassroom(null);
            var result = Assert.IsType<BadRequestResult>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_UpsertClassroom_ModelStateNotValid()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotClassroom> depot = new Mock<IDepotClassroom>();
            ClassroomController controller = new ClassroomController(depot.Object);
            Classroom classroom = new Classroom();
            controller.ModelState.AddModelError("", "Mock ModelState not valid");

            var response = controller.UpsertClassroom(classroom);
            var result = Assert.IsType<BadRequestResult>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_UpsertClassroom_Created()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotClassroom> depot = new Mock<IDepotClassroom>();
            ClassroomController controller = new ClassroomController(depot.Object);
            Classroom classroom = new Classroom();

            var response = controller.UpsertClassroom(classroom);
            var result = Assert.IsType<CreatedResult>(response);

            depot.Verify(d => d.UpsertClassroom(classroom), Times.Once);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_DeleteClassroom_NoContent()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotClassroom> depot = new Mock<IDepotClassroom>();
            ClassroomController controller = new ClassroomController(depot.Object);

            var response = controller.DeleteClassroom("ProjetSynthese");
            var result = Assert.IsType<NoContentResult>(response);

            depot.Verify(d => d.DeleteClassroom("ProjetSynthese"), Times.Once);
            Assert.NotNull(result);
        }
    }
}
