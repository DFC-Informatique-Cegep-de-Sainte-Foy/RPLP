using Microsoft.AspNetCore.Mvc;
using Moq;
using RPLP.API.Controllers;
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
    public class TestsAPITeacherController
    {
        [Fact]
        public void Test_Get()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
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
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
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
        public void Test_GetTeacherByUsername()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
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
        public void Test_GetTeacherByEmail()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotTeacher> depot = new Mock<IDepotTeacher>();
            TeacherController controller = new TeacherController(depot.Object);

            Teacher teacherInMockDepot = new Teacher()
            {
                Id = 1,
                Username = "ThPaquet",
                Email = "ThPaquet@hotmail.com"
            };

            depot.Setup(d => d.GetTeacherByEmail("ThPaquet@hotmail.com")).Returns(teacherInMockDepot);

            var response = controller.GetTeacherByEmail("ThPaquet@hotmail.com");

            var result = Assert.IsType<OkObjectResult>(response.Result);
            Teacher teacher = result.Value as Teacher;

            depot.Verify(d => d.GetTeacherByEmail("ThPaquet@hotmail.com"), Times.Once);
            Assert.NotNull(result);
            Assert.Equal("ThPaquet", teacher.Username);
        }

        [Fact]
        public void Test_GetTeacherClasses()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
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
        public void Test_GetTeacherOrganisationsByUsername()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotTeacher> depot = new Mock<IDepotTeacher>();
            TeacherController controller = new TeacherController(depot.Object);

            List<Organisation> mockOrganisations = new List<Organisation>()
            {
                new Organisation()
                {
                    Name = "CEGEP Ste-Foy"
                },
                new Organisation()
                {
                    Name = "RPLP"
                }
            };

            depot.Setup(d => d.GetTeacherOrganisations("ThPaquet")).Returns(mockOrganisations);

            var response = controller.GetTeacherOrganisationsByUsername("ThPaquet");

            var result = Assert.IsType<OkObjectResult>(response.Result);
            List<Organisation> organisations = result.Value as List<Organisation>;

            depot.Verify(d => d.GetTeacherOrganisations("ThPaquet"), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(2, organisations.Count);
            Assert.Contains(organisations, c => c.Name == "CEGEP Ste-Foy");
        }

        [Fact]
        public void Test_GetTeacherOrganisationsByEmail()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotTeacher> depot = new Mock<IDepotTeacher>();
            TeacherController controller = new TeacherController(depot.Object);

            List<Organisation> mockOrganisations = new List<Organisation>()
            {
                new Organisation()
                {
                    Name = "CEGEP Ste-Foy"
                },
                new Organisation()
                {
                    Name = "RPLP"
                }
            };

            Teacher teacher = new Teacher()
            {
                Username = "ThPaquet",
                Email = "ThPaquet@hotmail.com"
            };

            depot.Setup(d => d.GetTeacherOrganisations("ThPaquet")).Returns(mockOrganisations);
            depot.Setup(d => d.GetTeacherByEmail("ThPaquet@hotmail.com")).Returns(teacher);

            var response = controller.GetTeacherOrganisationsByEmail("ThPaquet@hotmail.com");

            var result = Assert.IsType<OkObjectResult>(response.Result);
            List<Organisation> organisations = result.Value as List<Organisation>;

            depot.Verify(d => d.GetTeacherOrganisations("ThPaquet"), Times.Once);
            depot.Verify(d => d.GetTeacherByEmail("ThPaquet@hotmail.com"), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(2, organisations.Count);
            Assert.Contains(organisations, c => c.Name == "CEGEP Ste-Foy");
        }

        [Fact]
        public void Test_GetClassroomsOfTeacherInOrganisationByEmail()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotTeacher> depot = new Mock<IDepotTeacher>();
            TeacherController controller = new TeacherController(depot.Object);

            string username = "ThPaquet";
            string email = "ThPaquet@hotmail.com";
            string organisation = "CEGEP Ste-Foy";

            List<Classroom> classroomsInMockDepot = new List<Classroom>()
            {
                new Classroom()
                {
                    Id = 1,
                    Name = "ProjetSynthese",
                    OrganisationName = "CEGEP Ste-Foy"
                },
                new Classroom()
                {
                    Id = 2,
                    Name = "OOP",
                    OrganisationName = "CEGEP Ste-Foy"
                }
            };

            depot.Setup(d => d.GetTeacherClassesInOrganisationByEmail(email, "CEGEP Ste-Foy")).Returns(classroomsInMockDepot);

            var response = controller.GetClassroomsOfTeacherInOrganisationByEmail(email, organisation);

            var result = Assert.IsType<OkObjectResult>(response.Result);
            List<Classroom> classrooms = result.Value as List<Classroom>;

            depot.Verify(d => d.GetTeacherClassesInOrganisationByEmail(email, organisation), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(2, classrooms.Count);
            Assert.Contains(classrooms, c => c.Name == "ProjetSynthese");
        }

        [Fact]
        public void Test_UpsertTeacher_NullTeacher()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotTeacher> depot = new Mock<IDepotTeacher>();
            TeacherController controller = new TeacherController(depot.Object);

            var response = controller.UpsertTeacher(null);

            var result = Assert.IsType<BadRequestResult>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_UpsertTeacher_ModelStateNotValid()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
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
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotTeacher> depot = new Mock<IDepotTeacher>();
            TeacherController controller = new TeacherController(depot.Object);
            Teacher? teacher = null;

            depot.Setup(d => d.UpsertTeacher(teacher)).Throws<ArgumentException>();

            Assert.IsType<BadRequestResult>(controller.UpsertTeacher(teacher));
        }

        [Fact]
        public void Test_UpsertTeacher_Created()
        {
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
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
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;
            Mock<IDepotTeacher> depot = new Mock<IDepotTeacher>();
            TeacherController controller = new TeacherController(depot.Object);

            var response = controller.DeleteTeacher("ThPaquet");

            var result = Assert.IsType<NoContentResult>(response);
            depot.Verify(d => d.DeleteTeacher("ThPaquet"), Times.Once());
            Assert.NotNull(result);
        }
    }
}
