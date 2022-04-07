using Microsoft.EntityFrameworkCore;
using RPLP.DAL.DTO.Sql;
using RPLP.DAL.SQL;
using RPLP.DAL.SQL.Depots;
using RPLP.ENTITES;
using System;
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
                //.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .Options;

        private void DeleteStudentsAndRelatedTablesContent()
        {
            using (var context = new RPLPDbContext(options))
            {
                context.Database.ExecuteSqlRaw("DELETE from Assignments;");
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
                new Student_SQLDTO()
                {
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
                    Active = true
                },
                new Student_SQLDTO()
                {
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Email = "BACenComm@hotmail.com",
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
                Assert.Equal(2, students.FirstOrDefault(s => s.Username == "ThPaquet").Classes.Count);
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
                Assert.Equal(2, student.Classes.Count);
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

        [Fact]
        public void Test_GetStudentClasses()
        {
            this.DeleteStudentsAndRelatedTablesContent();
            this.InsertPremadeStudents();

            using (var context = new RPLPDbContext(options))
            {
                DepotStudent depot = new DepotStudent(context);
                List<Classroom> classes = depot.GetStudentClasses("ThPaquet");

                Assert.NotNull(classes);
                Assert.Equal(2, classes.Count);
                Assert.Contains(classes, c => c.Name == "RPLP");
            }

            this.DeleteStudentsAndRelatedTablesContent();
        }

        [Fact]
        public void Test_UpsertStudent_Inserts()
        {
            this.DeleteStudentsAndRelatedTablesContent();

            using (var context = new RPLPDbContext(options))
            {
                DepotStudent depot = new DepotStudent(context);
                Student_SQLDTO? studentInContext = context.Students.FirstOrDefault(s => s.Username == "ThPaquet");
                Assert.Null(studentInContext);

                Student student = new Student()
                {
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com"
                };

                depot.UpsertStudent(student);
            }

            using (var context = new RPLPDbContext(options))
            {
                Student_SQLDTO? studentInContext = context.Students
                                                .Include(s => s.Classes)
                                                .SingleOrDefault(s => s.Username == "ThPaquet");

                Assert.NotNull(studentInContext);
                Assert.Equal("ThPaquet", studentInContext.Username);
                Assert.Equal("Thierry", studentInContext.FirstName);
                Assert.Equal("Paquet", studentInContext.LastName);
                Assert.Equal("ThPaquet@hotmail.com", studentInContext.Email);
            }

            this.DeleteStudentsAndRelatedTablesContent();
        }

        [Fact]
        public void Test_UpsertStudent_Updates()
        {
            this.DeleteStudentsAndRelatedTablesContent();
            this.InsertPremadeStudents();

            using (var context = new RPLPDbContext(options))
            {
                DepotStudent depot = new DepotStudent(context);

                Student_SQLDTO? studentInContext = context.Students
                    .AsNoTracking()
                    .FirstOrDefault(s => s.Username == "ThPaquet");
                Assert.NotNull(studentInContext);

                studentInContext.Username = "Upserted";
                studentInContext.FirstName = "Upserty";
                studentInContext.LastName = "McUpserton";

                depot.UpsertStudent(studentInContext.ToEntity());
            }

            using (var context = new RPLPDbContext(options))
            {
                Student_SQLDTO? studentInContext = context.Students
                                                .AsNoTracking()
                                                .Include(s => s.Classes)
                                                .SingleOrDefault(s => s.Username == "Upserted");

                Assert.NotNull(studentInContext);
                Assert.Equal(3, studentInContext.Classes.Count);
                Assert.Equal("Upserted", studentInContext.Username);
                Assert.Equal("Upserty", studentInContext.FirstName);
                Assert.Equal("McUpserton", studentInContext.LastName);
            }

            this.DeleteStudentsAndRelatedTablesContent();
        }

        [Fact]
        public void Test_DeleteStudent()
        {
            this.DeleteStudentsAndRelatedTablesContent();
            this.InsertPremadeStudents();

            using (var context = new RPLPDbContext(options))
            {
                DepotStudent depot = new DepotStudent(context);

                Student_SQLDTO? studentInContext = context.Students.SingleOrDefault(s => s.Username == "ThPaquet");
                Assert.NotNull(studentInContext);

                depot.DeleteStudent("ThPaquet");
            }

            using (var context = new RPLPDbContext(options))
            {
                Student_SQLDTO? studentInContext = context.Students.SingleOrDefault(s => s.Username == "ThPaquet" && s.Active);
                Assert.Null(studentInContext);
            }

            this.DeleteStudentsAndRelatedTablesContent();
        }

        [Fact]
        public void Test_UpsertStudent_ThrowUpdateDeletedAccount()
        {
            this.DeleteStudentsAndRelatedTablesContent();
            this.InsertPremadeStudents();

            using (var context = new RPLPDbContext(options))
            {
                DepotStudent depot = new DepotStudent(context);

                Student_SQLDTO? student = context.Students.SingleOrDefault(a => a.Username == "BACenComm");

                Assert.Throws<ArgumentException>(
                    () =>
                    {
                        depot.UpsertStudent(student.ToEntityWithoutList());
                    });
            }

            this.DeleteStudentsAndRelatedTablesContent();
        }

        [Fact]
        public void Test_UpsertStudent_ThrowUsernameTaken_UsernameTakenNotActive()
        {
            this.DeleteStudentsAndRelatedTablesContent();
            this.InsertPremadeStudents();

            using (var context = new RPLPDbContext(options))
            {
                DepotStudent depot = new DepotStudent(context);

                Student_SQLDTO? student = context.Students.SingleOrDefault(a => a.Username == "ikeameatbol");
                Assert.NotNull(student);

                student.Username = "BACenComm";

                Assert.Throws<ArgumentException>(
                    () =>
                    {
                        depot.UpsertStudent(student.ToEntityWithoutList());
                    });
            }

            this.DeleteStudentsAndRelatedTablesContent();
        }

        [Fact]
        public void Test_UpsertStudent_ThrowUsernameTaken_NewAdmin()
        {
            this.DeleteStudentsAndRelatedTablesContent();
            this.InsertPremadeStudents();

            using (var context = new RPLPDbContext(options))
            {
                DepotStudent depot = new DepotStudent(context);

                Student_SQLDTO student = new Student_SQLDTO()
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
                        depot.UpsertStudent(student.ToEntityWithoutList());
                    });
            }

            this.DeleteStudentsAndRelatedTablesContent();
        }

        [Fact]
        public void Test_UpsertStudent_ThrowEmailTaken_EmailTakenNotActive()
        {
            this.DeleteStudentsAndRelatedTablesContent();
            this.InsertPremadeStudents();

            using (var context = new RPLPDbContext(options))
            {

                DepotStudent depot = new DepotStudent(context);

                Student_SQLDTO? student = context.Students.SingleOrDefault(a => a.Email == "ikeameatbol@hotmail.com");
                Assert.NotNull(student);


                student.Email = "BACenComm@hotmail.com";

                Assert.Throws<ArgumentException>(
                    () =>
                    {
                        depot.UpsertStudent(student.ToEntityWithoutList());
                    });
            }

            this.DeleteStudentsAndRelatedTablesContent();
        }

        [Fact]
        public void Test_UpsertStudent_ThrowEmailTaken_NewAdmin()
        {
            this.DeleteStudentsAndRelatedTablesContent();
            this.InsertPremadeStudents();

            using (var context = new RPLPDbContext(options))
            {
                DepotStudent depot = new DepotStudent(context);

                Student_SQLDTO student = new Student_SQLDTO()
                {
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
                        depot.UpsertStudent(student.ToEntityWithoutList());
                    });
            }

            this.DeleteStudentsAndRelatedTablesContent();
        }
    }
}
