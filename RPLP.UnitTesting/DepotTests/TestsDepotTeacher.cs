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
    public class TestsDepotTeacher
    {
        private static readonly DbContextOptions<RPLPDbContext> options = new DbContextOptionsBuilder<RPLPDbContext>()
                .UseSqlServer("Server=localhost,1434; Database=RPLP; User Id=sa; password=Cad3pend86!")
                //.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .Options;

        private void DeleteTeachersAndRelatedTablesContent()
        {
            using (var context = new RPLPDbContext(options))
            {
                context.Database.ExecuteSqlRaw("DELETE from Teachers;");
                context.Database.ExecuteSqlRaw("DELETE from Classrooms;");
            }
        }

        private void InsertPremadeTeachers()
        {
            List<Teacher_SQLDTO> teachers = new List<Teacher_SQLDTO>()
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
                new Teacher_SQLDTO()
                {
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Email = "ikeameatbol@hotmail.com",
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

            using (var context = new RPLPDbContext(options))
            {
                context.Teachers.AddRange(teachers);
                context.SaveChanges();
            }
        }

        [Fact]
        private void Test_GetTeachers()
        {
            this.DeleteTeachersAndRelatedTablesContent();
            this.InsertPremadeTeachers();

            using (var context = new RPLPDbContext(options))
            {
                DepotTeacher depot = new DepotTeacher(context);
                List<Teacher> teachers = depot.GetTeachers();

                Assert.NotNull(teachers);
                Assert.Equal(2, teachers.Count);
                Assert.Contains(teachers, t => t.Username == "ThPaquet");
                Assert.DoesNotContain(teachers, t => t.Username == "BACenComm");
                Assert.Equal(2, teachers.FirstOrDefault(t => t.Username == "ThPaquet").Classes.Count);
            }

            this.DeleteTeachersAndRelatedTablesContent();
        }

        [Fact]
        private void Test_GetTeacherById()
        {
            this.DeleteTeachersAndRelatedTablesContent();
            this.InsertPremadeTeachers();

            using (var context = new RPLPDbContext(options))
            {
                Teacher_SQLDTO thPaquetInContext = context.Teachers.FirstOrDefault(t => t.Username == "ThPaquet");
                Assert.NotNull(thPaquetInContext);

                int teacherId = thPaquetInContext.Id;

                DepotTeacher depot = new DepotTeacher(context);
                Teacher teacher = depot.GetTeacherById(teacherId);

                Assert.NotNull(teacher);
                Assert.Equal("ThPaquet", teacher.Username);
                Assert.Equal("Thierry", teacher.FirstName);
                Assert.Equal("Paquet", teacher.LastName);
                Assert.Equal(2, teacher.Classes.Count);
            }

            this.DeleteTeachersAndRelatedTablesContent();
        }

        [Fact]
        private void Test_GetTeacherById_NotActive()
        {
            this.DeleteTeachersAndRelatedTablesContent();
            this.InsertPremadeTeachers();

            using (var context = new RPLPDbContext(options))
            {
                Teacher_SQLDTO teacherInContext = context.Teachers.FirstOrDefault(t => t.Username == "BACenComm");
                Assert.NotNull(teacherInContext);
                Assert.False(teacherInContext.Active);

                int teacherId = teacherInContext.Id;

                DepotTeacher depot = new DepotTeacher(context);
                Teacher teacher = depot.GetTeacherById(teacherId);

                Assert.Null(teacher);
            }

            this.DeleteTeachersAndRelatedTablesContent();
        }

        [Fact]
        private void Test_GetTeacherByUsername()
        {
            this.DeleteTeachersAndRelatedTablesContent();
            this.InsertPremadeTeachers();

            using (var context = new RPLPDbContext(options))
            {
                Teacher_SQLDTO thPaquetInContext = context.Teachers.FirstOrDefault(t => t.Username == "ThPaquet");
                Assert.NotNull(thPaquetInContext);

                DepotTeacher depot = new DepotTeacher(context);
                Teacher teacher = depot.GetTeacherByUsername("ThPaquet");

                Assert.NotNull(teacher);
                Assert.Equal("ThPaquet", teacher.Username);
                Assert.Equal("Thierry", teacher.FirstName);
                Assert.Equal("Paquet", teacher.LastName);
                Assert.Equal(2, teacher.Classes.Count);
            }

            this.DeleteTeachersAndRelatedTablesContent();
        }

        [Fact]
        private void Test_GetTeacherByUsername_NotActive()
        {
            this.DeleteTeachersAndRelatedTablesContent();
            this.InsertPremadeTeachers();

            using (var context = new RPLPDbContext(options))
            {
                Teacher_SQLDTO teacherInContext = context.Teachers.FirstOrDefault(t => t.Username == "BACenComm");
                Assert.NotNull(teacherInContext);
                Assert.False(teacherInContext.Active);

                DepotTeacher depot = new DepotTeacher(context);
                Teacher teacher = depot.GetTeacherByUsername("BACenComm");

                Assert.Null(teacher);
            }

            this.DeleteTeachersAndRelatedTablesContent();
        }

        [Fact]
        private void Test_GetTeacherClasses()
        {
            this.DeleteTeachersAndRelatedTablesContent();
            this.InsertPremadeTeachers();

            using (var context = new RPLPDbContext(options))
            {
                DepotTeacher depot = new DepotTeacher(context);
                List<Classroom> classes = depot.GetTeacherClasses("ThPaquet");

                Assert.NotNull(classes);
                Assert.Equal(2, classes.Count);
                Assert.Contains(classes, c => c.Name == "ProjetSynthese");
                Assert.DoesNotContain(classes, c => c.Name == "OOP");
            }

            this.DeleteTeachersAndRelatedTablesContent();
        }

        [Fact]
        private void Test_AddClassroomToTeacher()
        {
            this.DeleteTeachersAndRelatedTablesContent();
            this.InsertPremadeTeachers();

            using (var context = new RPLPDbContext(options))
            {
                DepotTeacher depot = new DepotTeacher(context);

                Teacher_SQLDTO teacherInContext = context.Teachers
                    .Include(t => t.Classes)
                    .FirstOrDefault(t => t.Username == "ikeameatbol");

                Assert.NotNull(teacherInContext);
                Assert.DoesNotContain(teacherInContext.Classes, c => c.Name == "RPLP");

                Classroom_SQLDTO classroomInContext = context.Classrooms.FirstOrDefault(c => c.Name == "RPLP");
                Assert.NotNull(classroomInContext);

                depot.AddClassroomToTeacher("ikeameatbol", "RPLP");
            }

            using (var context = new RPLPDbContext(options))
            {
                DepotTeacher depot = new DepotTeacher(context);

                Teacher_SQLDTO teacherInContext = context.Teachers
                    .Include(t => t.Classes)
                    .FirstOrDefault(t => t.Username == "ikeameatbol");
                Assert.NotNull(teacherInContext);

                Assert.Contains(teacherInContext.Classes, c => c.Name == "RPLP");
            }

            this.DeleteTeachersAndRelatedTablesContent();
        }

        [Fact]
        private void Test_RemoveClassroomFromTeacher()
        {
            this.DeleteTeachersAndRelatedTablesContent();
            this.InsertPremadeTeachers();

            using (var context = new RPLPDbContext(options))
            {
                DepotTeacher depot = new DepotTeacher(context);

                Teacher_SQLDTO teacherInContext = context.Teachers
                    .Include(t => t.Classes)
                    .FirstOrDefault(t => t.Username == "ThPaquet");

                Assert.NotNull(teacherInContext);
                Assert.Contains(teacherInContext.Classes, c => c.Name == "RPLP");

                depot.RemoveClassroomFromTeacher("ThPaquet", "RPLP");
            }

            using (var context = new RPLPDbContext(options))
            {
                DepotTeacher depot = new DepotTeacher(context);

                Teacher_SQLDTO teacherInContext = context.Teachers
                    .Include(t => t.Classes)
                    .FirstOrDefault(t => t.Username == "ThPaquet");
                Assert.NotNull(teacherInContext);

                Assert.DoesNotContain(teacherInContext.Classes, c => c.Name == "RPLP");
            }

            this.DeleteTeachersAndRelatedTablesContent();
        }

        [Fact]
        public void Test_UpsertTeacher_Inserts()
        {
            this.DeleteTeachersAndRelatedTablesContent();

            using (var context = new RPLPDbContext(options))
            {
                DepotTeacher depot = new DepotTeacher(context);
                Teacher teacher = new Teacher()
                {
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Email = "ThPaquet@hotmail.com"
                };

                Teacher_SQLDTO teacherInContext = context.Teachers.FirstOrDefault(t => t.Username == "ThPaquet");
                Assert.Null(teacherInContext);

                depot.UpsertTeacher(teacher);
            }

            using (var context = new RPLPDbContext(options))
            {
                Teacher_SQLDTO teacherInContext = context.Teachers.FirstOrDefault(t => t.Username == "ThPaquet");

                Assert.NotNull(teacherInContext);
                Assert.Equal("Thierry", teacherInContext.FirstName);
                Assert.Equal("Paquet", teacherInContext.LastName);
            }

            this.DeleteTeachersAndRelatedTablesContent();
        }

        [Fact]
        public void Test_UpsertTeacher_Updates()
        {
            this.DeleteTeachersAndRelatedTablesContent();
            this.InsertPremadeTeachers();

            using (var context = new RPLPDbContext(options))
            {
                DepotTeacher depot = new DepotTeacher(context);

                Teacher_SQLDTO teacherInContext = context.Teachers
                    .AsNoTracking()
                    .FirstOrDefault(t => t.Username == "ThPaquet");
                Assert.NotNull(teacherInContext);

                teacherInContext.Username = "Upserted";
                teacherInContext.FirstName = "Upserty";
                teacherInContext.LastName = "McUpserton";

                Teacher_SQLDTO updatedTeacherBeforeUpsert = context.Teachers
                    .AsNoTracking()
                    .FirstOrDefault(t => t.Username == "Upserted");

                Assert.Null(updatedTeacherBeforeUpsert);

                depot.UpsertTeacher(teacherInContext.ToEntityWithoutList());
            }

            using (var context = new RPLPDbContext(options))
            {
                Teacher_SQLDTO teacherInContext = context.Teachers
                    .AsNoTracking()
                    .FirstOrDefault(t => t.Username == "Upserted");

                Assert.NotNull(teacherInContext);
                Assert.Equal("Upserty", teacherInContext.FirstName);
                Assert.Equal("McUpserton", teacherInContext.LastName);

                Teacher_SQLDTO teacherBeforeUpsert = context.Teachers.FirstOrDefault(t => t.Username == "ThPaquet");

                Assert.Null(teacherBeforeUpsert);
            }

            this.DeleteTeachersAndRelatedTablesContent();
        }

        [Fact]
        public void Test_UpsertTeacher_ThrowUpdateDeletedAccount()
        {
            this.DeleteTeachersAndRelatedTablesContent();
            this.InsertPremadeTeachers();

            using (var context = new RPLPDbContext(options))
            {
                DepotTeacher depot = new DepotTeacher(context);

                Teacher_SQLDTO? teacher = context.Teachers.SingleOrDefault(a => a.Username == "BACenComm");

                Assert.Throws<ArgumentException>(
                    () =>
                    {
                        depot.UpsertTeacher(teacher.ToEntityWithoutList());
                    });
            }

            this.DeleteTeachersAndRelatedTablesContent();
        }

        [Fact]
        public void Test_UpsertTeacher_ThrowUsernameTaken_UsernameTakenNotActive()
        {
            this.DeleteTeachersAndRelatedTablesContent();
            this.InsertPremadeTeachers();

            using (var context = new RPLPDbContext(options))
            {
                DepotTeacher depot = new DepotTeacher(context);

                Teacher_SQLDTO? teacher = context.Teachers.SingleOrDefault(a => a.Username == "ikeameatbol");
                Assert.NotNull(teacher);

                teacher.Username = "BACenComm";

                Assert.Throws<ArgumentException>(
                    () =>
                    {
                        depot.UpsertTeacher(teacher.ToEntityWithoutList());
                    });
            }

            this.DeleteTeachersAndRelatedTablesContent();
        }

        [Fact]
        public void Test_UpsertTeacher_ThrowUsernameTaken_NewAdmin()
        {
            this.DeleteTeachersAndRelatedTablesContent();
            this.InsertPremadeTeachers();

            using (var context = new RPLPDbContext(options))
            {
                DepotTeacher depot = new DepotTeacher(context);

                Teacher_SQLDTO teacher = new Teacher_SQLDTO()
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
                        depot.UpsertTeacher(teacher.ToEntityWithoutList());
                    });
            }

            this.DeleteTeachersAndRelatedTablesContent();
        }

        [Fact]
        public void Test_UpsertTeacher_ThrowEmailTaken_EmailTakenNotActive()
        {
            this.DeleteTeachersAndRelatedTablesContent();
            this.InsertPremadeTeachers();

            using (var context = new RPLPDbContext(options))
            {

                DepotTeacher depot = new DepotTeacher(context);

                Teacher_SQLDTO? teacher = context.Teachers.SingleOrDefault(a => a.Email == "ikeameatbol@hotmail.com");
                Assert.NotNull(teacher);


                teacher.Email = "BACenComm@hotmail.com";

                Assert.Throws<ArgumentException>(
                    () =>
                    {
                        depot.UpsertTeacher(teacher.ToEntityWithoutList());
                    });
            }

            this.DeleteTeachersAndRelatedTablesContent();
        }

        [Fact]
        public void Test_UpsertTeacher_ThrowEmailTaken_NewAdmin()
        {
            this.DeleteTeachersAndRelatedTablesContent();
            this.InsertPremadeTeachers();

            using (var context = new RPLPDbContext(options))
            {
                DepotTeacher depot = new DepotTeacher(context);

                Teacher_SQLDTO teacher = new Teacher_SQLDTO()
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
                        depot.UpsertTeacher(teacher.ToEntityWithoutList());
                    });
            }

            this.DeleteTeachersAndRelatedTablesContent();
        }
    }
}
