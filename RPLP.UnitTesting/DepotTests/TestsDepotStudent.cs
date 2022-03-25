using Microsoft.EntityFrameworkCore;
using RPLP.DAL.DTO.Sql;
using RPLP.DAL.SQL;
using RPLP.DAL.SQL.Depots;
using RPLP.ENTITES;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RPLP.UnitTesting.DepotTests
{
    [Collection("DatabaseTests")]
    public class TestsDepotStudent
    {
        private static readonly DbContextOptions<RPLPDbContext> options = new DbContextOptionsBuilder<RPLPDbContext>()
                .UseSqlServer("Server=localhost,1434; Database=RPLP; User Id=sa; password=Cad3pend86!")
                .Options;

        private void DeleteStudentsAndRelatedTablesContent()
        {
            using (var context = new RPLPDbContext(options))
            {
                context.Database.ExecuteSqlRaw("DELETE from Students;");
                context.Database.ExecuteSqlRaw("DELETE from Classrooms;");
            }
        }

        private void InsertPremadeStudents()
        {
            List<Student_SQLDTO> students = new List<Student_SQLDTO>()
            {
                new Student_SQLDTO()
                {
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Active = true
                },
                new Student_SQLDTO()
                {
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Active = true
                },
                new Student_SQLDTO()
                {
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Active = false
                }
            };

            using (var context = new RPLPDbContext(options))
            {
                context.Students.AddRange(students);
                context.SaveChanges();
            }
        }

        [Fact]
        public void Test_GetStudents()
        {
            this.DeleteStudentsAndRelatedTablesContent();
            this.InsertPremadeStudents();

            using (var context = new RPLPDbContext(options))
            {
                DepotStudent depot = new DepotStudent(context);
                List<Student> students = depot.GetStudents();

                Assert.DoesNotContain(students, s => s.Username == "BACenComm");
                Assert.Contains(students, s => s.Username == "ThPaquet");
                Assert.Contains(students, s => s.Username == "ikeameatbol");
                Assert.Equal(2, students.Count);
            }

            this.DeleteStudentsAndRelatedTablesContent();
        }

        [Fact]
        public void Test_GetStudentById()
        {
            this.DeleteStudentsAndRelatedTablesContent();
            this.InsertPremadeStudents();

            using (var context = new RPLPDbContext(options))
            {
                Student_SQLDTO? studentThPaquet = context.Students.FirstOrDefault(s => s.Username == "ThPaquet");
                Assert.NotNull(studentThPaquet);

                int thPaquetId = studentThPaquet.Id;

                DepotStudent depot = new DepotStudent(context);
                Student student = depot.GetStudentById(thPaquetId);

                Assert.NotNull(student);
                Assert.Equal("ThPaquet", student.Username);
                Assert.Equal("Thierry", student.FirstName);
                Assert.Equal("Paquet", student.LastName);
            }

            this.DeleteStudentsAndRelatedTablesContent();
        }

        [Fact]
        public void Test_GetStudentById_NotActive()
        {
            this.DeleteStudentsAndRelatedTablesContent();
            this.InsertPremadeStudents();

            using (var context = new RPLPDbContext(options))
            {
                Student_SQLDTO? studentBACenComm = context.Students.FirstOrDefault(s => s.Username == "BACenComm");
                Assert.NotNull(studentBACenComm);

                int BACenCommId = studentBACenComm.Id;

                DepotStudent depot = new DepotStudent(context);
                Student student = depot.GetStudentById(BACenCommId);

                Assert.Null(student);
            }

            this.DeleteStudentsAndRelatedTablesContent();
        }

        [Fact]
        public void Test_GetStudentByUsername()
        {
            this.DeleteStudentsAndRelatedTablesContent();
            this.InsertPremadeStudents();

            using (var context = new RPLPDbContext(options))
            {
                DepotStudent depot = new DepotStudent(context);
                Student student = depot.GetStudentByUsername("ThPaquet");

                Assert.NotNull(student);
                Assert.Equal("ThPaquet", student.Username);
                Assert.Equal("Thierry", student.FirstName);
                Assert.Equal("Paquet", student.LastName);
            }

            this.DeleteStudentsAndRelatedTablesContent();
        }

        [Fact]
        public void Test_GetStudentByName_NotActive()
        {
            this.DeleteStudentsAndRelatedTablesContent();
            this.InsertPremadeStudents();

            using (var context = new RPLPDbContext(options))
            {
                DepotStudent depot = new DepotStudent(context);
                Student student = depot.GetStudentByUsername("BACenComm");

                Assert.Null(student);
            }

            this.DeleteStudentsAndRelatedTablesContent();
        }
    }
}
