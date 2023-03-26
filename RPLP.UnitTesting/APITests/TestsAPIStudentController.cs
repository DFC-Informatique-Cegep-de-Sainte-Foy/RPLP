using Microsoft.AspNetCore.Mvc;
using Moq;
using RPLP.API.Controllers;
using RPLP.ENTITES;
using RPLP.ENTITES.InterfacesDepots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RPLP.UnitTesting.APITests
{
    public class TestsAPIStudentController
    {
        [Fact]
        public void Test_Get()
        {
            Mock<IDepotStudent> depot = new Mock<IDepotStudent>();
            StudentController controller = new StudentController(depot.Object);

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

            depot.Setup(d => d.GetStudents()).Returns(studentsInMockDepot);

            var response = controller.Get();
            
            var result = Assert.IsType<OkObjectResult>(response.Result);
            List<Student> students = result.Value as List<Student>;

            depot.Verify(d => d.GetStudents(), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(2, students.Count);
            Assert.Contains(students, s => s.Username == "ThPaquet");
        }

        [Fact]
        public void Test_GetStudentById()
        {
            Mock<IDepotStudent> depot = new Mock<IDepotStudent>();
            StudentController controller = new StudentController(depot.Object);

            Student studentInMockDepot = new Student()
            {
                Id = 1,
                Username = "ThPaquet"
            };

            depot.Setup(d => d.GetStudentById(1)).Returns(studentInMockDepot);

            var response = controller.GetStudentById(1);

            var result = Assert.IsType<OkObjectResult>(response.Result);
            Student student = result.Value as Student;

            depot.Verify(d => d.GetStudentById(1), Times.Once);
            Assert.NotNull(result);
            Assert.Equal("ThPaquet", student.Username);
        }

        [Fact]
        public void Test_GetStudentByUserName()
        {
            Mock<IDepotStudent> depot = new Mock<IDepotStudent>();
            StudentController controller = new StudentController(depot.Object);

            Student studentInMockDepot = new Student()
            {
                Id = 1,
                Username = "ThPaquet"
            };

            depot.Setup(d => d.GetStudentByUsername("ThPaquet")).Returns(studentInMockDepot);

            var response = controller.GetStudentByUsername("ThPaquet");

            var result = Assert.IsType<OkObjectResult>(response.Result);
            Student student = result.Value as Student;

            depot.Verify(d => d.GetStudentByUsername("ThPaquet"), Times.Once);
            Assert.NotNull(result);
            Assert.Equal("ThPaquet", student.Username);
        }

        [Fact]
        public void Test_GetStudentClasses()
        {
            Mock<IDepotStudent> depot = new Mock<IDepotStudent>();
            StudentController controller = new StudentController(depot.Object);

            List<Classroom> classroomsInMockDepot = new List<Classroom>()
            {
                new Classroom()
                {
                    Id = 1,
                    Name = "RPLP"
                },
                new Classroom()
                {
                    Id = 2,
                    Name = "ProjetSynthese"
                }
            };

            depot.Setup(d => d.GetStudentClasses("ThPaquet")).Returns(classroomsInMockDepot);

            var response = controller.GetStudentClasses("ThPaquet");

            var result = Assert.IsType<OkObjectResult>(response.Result);
            List<Classroom> classrooms = result.Value as List<Classroom>;

            depot.Verify(d => d.GetStudentClasses("ThPaquet"), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(2, classrooms.Count);
            Assert.Contains(classrooms, c => c.Name == "RPLP");
        }

        [Fact]
        public void Test_Post_NullStudent()
        {
            Mock<IDepotStudent> depot = new Mock<IDepotStudent>();
            StudentController controller = new StudentController(depot.Object);

            var response = controller.Post(null);

            var result = Assert.IsType<BadRequestResult>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_Post_ModelStateNotValid()
        {
            Mock<IDepotStudent> depot = new Mock<IDepotStudent>();
            StudentController controller = new StudentController(depot.Object);
            controller.ModelState.AddModelError("", "Mock ModelState Not Valid");

            var response = controller.Post(new Student());

            var result = Assert.IsType<BadRequestResult>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_Post_CatchesException_ReturnBadRequest()
        {
            Mock<IDepotStudent> depot = new Mock<IDepotStudent>();
            StudentController controller = new StudentController(depot.Object);
            Student student = new Student();

            depot.Setup(d => d.UpsertStudent(student)).Throws<ArgumentException>();

            Assert.IsType<BadRequestObjectResult>(controller.Post(student));
        }

        [Fact]
        public void Test_Post_Created()
        {
            Mock<IDepotStudent> depot = new Mock<IDepotStudent>();
            StudentController controller = new StudentController(depot.Object);
            Student student = new Student();

            var response = controller.Post(student);

            depot.Verify(d => d.UpsertStudent(student), Times.Once);
            var result = Assert.IsType<CreatedResult>(response);
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_DeleteStudent()
        {
            Mock<IDepotStudent> depot = new Mock<IDepotStudent>();
            StudentController controller = new StudentController(depot.Object);

            var response = controller.DeleteStudent("ThPaquet");

            depot.Verify(d => d.DeleteStudent("ThPaquet"), Times.Once);
            var result = Assert.IsType<NoContentResult>(response);
            Assert.NotNull(result);
        }
    }
}
