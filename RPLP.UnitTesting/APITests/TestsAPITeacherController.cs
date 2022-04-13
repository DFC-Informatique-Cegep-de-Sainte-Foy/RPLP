using Microsoft.AspNetCore.Mvc;
using Moq;
using RPLP.API.Controllers;
using RPLP.ENTITES;
using RPLP.SERVICES.InterfacesDepots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RPLP.UnitTesting.APITests
{
    public class TestsAPITeacherController
    {
        [Fact]
        public void Test_Get()
        {
            Mock<IDepotTeacher> depot = new Mock<IDepotTeacher>();
            TeacherController controller = new TeacherController(depot.Object);

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

            depot.Setup(d => d.GetTeachers()).Returns(teachersInMockDepot);

            var response = controller.Get();

            var result = Assert.IsType<OkObjectResult>(response.Result);
            List<Teacher> teachers = result.Value as List<Teacher>;

            depot.Verify(d => d.GetTeachers(), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(2, teachers.Count);
            Assert.Contains(teachers, t => t.Username == "ThPaquet");
        }

        [Fact]
        public void Test_GetStudentById()
        {
            Mock<IDepotTeacher> depot = new Mock<IDepotTeacher>();
            TeacherController controller = new TeacherController(depot.Object);

            Teacher teacherInMockDepot = new Teacher()
            {
                Id = 1,
                Username = "ThPaquet"
            };

            depot.Setup(d => d.GetTeacherById(1)).Returns(teacherInMockDepot);

            var response = controller.GetTeacherById(1);

            var result = Assert.IsType<OkObjectResult>(response.Result);
            Teacher teacher = result.Value as Teacher;

            depot.Verify(d => d.GetTeacherById(1), Times.Once);
            Assert.NotNull(result);
            Assert.Equal("ThPaquet", teacher.Username);
        }

        [Fact]
        public void Test_GetStudentByUsername()
        {
            Mock<IDepotTeacher> depot = new Mock<IDepotTeacher>();
            TeacherController controller = new TeacherController(depot.Object);

            Teacher teacherInMockDepot = new Teacher()
            {
                Id = 1,
                Username = "ThPaquet"
            };

            depot.Setup(d => d.GetTeacherByUsername("ThPaquet")).Returns(teacherInMockDepot);

            var response = controller.GetTeacherByUsername("ThPaquet");

            var result = Assert.IsType<OkObjectResult>(response.Result);
            Teacher teacher = result.Value as Teacher;

            depot.Verify(d => d.GetTeacherByUsername("ThPaquet"), Times.Once);
            Assert.NotNull(result);
            Assert.Equal("ThPaquet", teacher.Username);
        }

        [Fact]
        public void Test_GetTeacherClasses()
        {
            Mock<IDepotTeacher> depot = new Mock<IDepotTeacher>();
            TeacherController controller = new TeacherController(depot.Object);

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
                    Name = "OOP"
                }
            };

            depot.Setup(d => d.GetTeacherClasses("ThPaquet")).Returns(classroomsInMockDepot);

            var response = controller.GetTeacherClasses("ThPaquet");

            var result = Assert.IsType<OkObjectResult>(response.Result);
            List<Classroom> classrooms = result.Value as List<Classroom>;

            depot.Verify(d => d.GetTeacherClasses("ThPaquet"), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(2, classrooms.Count);
            Assert.Contains(classrooms, c => c.Name == "ProjetSynthese");
        }

        [Fact]
        public void Test_UpsertTeacher_NullTeacher()
        {
            Mock<IDepotTeacher> depot = new Mock<IDepotTeacher>();
            TeacherController controller = new TeacherController(depot.Object);

            var response = controller.UpsertTeacher(null);

            var result = Assert.IsType<BadRequestResult>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_UpsertTeacher_ModelStateNotValid()
        {
            Mock<IDepotTeacher> depot = new Mock<IDepotTeacher>();
            TeacherController controller = new TeacherController(depot.Object);
            controller.ModelState.AddModelError("", "Mock ModelState Not Valid");

            var response = controller.UpsertTeacher(new Teacher());

            var result = Assert.IsType<BadRequestResult>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_Post_CatchesException_ReturnBadRequest()
        {
            Mock<IDepotTeacher> depot = new Mock<IDepotTeacher>();
            TeacherController controller = new TeacherController(depot.Object);
            Teacher teacher = new Teacher();

            depot.Setup(d => d.UpsertTeacher(teacher)).Throws<ArgumentException>();

            Assert.IsType<BadRequestObjectResult>(controller.UpsertTeacher(teacher));
        }

        [Fact]
        public void Test_UpsertTeacher_Created()
        {
            Mock<IDepotTeacher> depot = new Mock<IDepotTeacher>();
            TeacherController controller = new TeacherController(depot.Object);
            Teacher teacher = new Teacher();

            var response = controller.UpsertTeacher(teacher);

            var result = Assert.IsType<CreatedResult>(response);
            depot.Verify(d => d.UpsertTeacher(teacher), Times.Once());
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_DeleteTeacher()
        {
            Mock<IDepotTeacher> depot = new Mock<IDepotTeacher>();
            TeacherController controller = new TeacherController(depot.Object);

            var response = controller.DeleteTeacher("ThPaquet");

            var result = Assert.IsType<NoContentResult>(response);
            depot.Verify(d => d.DeleteTeacher("ThPaquet"), Times.Once());
            Assert.NotNull(result);
        }
    }
}
