﻿using Microsoft.EntityFrameworkCore;
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
    public class TestsDepotClassroom
    {
        private static readonly DbContextOptions<RPLPDbContext> options = new DbContextOptionsBuilder<RPLPDbContext>()
                .UseSqlServer("Server=localhost,1434; Database=RPLP; User Id=sa; password=Cad3pend86!")
                .Options;

        private void DeleteClassroomAndRelatedTablesContent()
        {
            using (var context = new RPLPDbContext(options))
            {
                context.Database.ExecuteSqlRaw("DELETE from Assignments;");
                context.Database.ExecuteSqlRaw("DELETE from Students;");
                context.Database.ExecuteSqlRaw("DELETE from Teachers;");
                context.Database.ExecuteSqlRaw("DELETE from Classrooms;");
                context.Database.ExecuteSqlRaw("DELETE from Organisations;");
            }
        }

        private void InsertClassroom(Classroom_SQLDTO p_classroom)
        {
            using (var context = new RPLPDbContext(options))
            {
                context.Classrooms.Add(p_classroom);
                context.SaveChanges();
            }
        }

        private void InsertClassrooms(IEnumerable<Classroom_SQLDTO> p_classrooms)
        {
            using (var context = new RPLPDbContext(options))
            {
                context.Classrooms.AddRange(p_classrooms);
                context.SaveChanges();
            }
        }

        private void InsertClassroomProjetSynthese()
        {
            Classroom_SQLDTO classroom = new Classroom_SQLDTO()
            {
                Name = "ProjetSynthese",
                OrganisationName = "CEGEP Ste-Foy",
                Assignments = new List<Assignment_SQLDTO>(),
                Students = new List<Student_SQLDTO>(),
                Teachers = new List<Teacher_SQLDTO>(),
                Active = true
            };

            classroom.Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "Review",
                ClassroomName = "ProjetSynthese",
                DistributionDate = System.DateTime.Now,
                Description = "Review a partner\'s code",
                DeliveryDeadline = System.DateTime.Now.AddDays(1),
                Active = true
            });

            classroom.Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "Scrum",
                ClassroomName = "ProjetSynthese",
                DistributionDate = System.DateTime.Now,
                Description = "Daily briefing",
                DeliveryDeadline = System.DateTime.Now.AddDays(1),
                Active = true
            });

            classroom.Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "Recap",
                ClassroomName = "ProjetSynthese",
                DistributionDate = System.DateTime.Now,
                Description = "Recapituclation of the iteration",
                DeliveryDeadline = System.DateTime.Now.AddDays(1),
                Active = false
            });

            classroom.Students.Add(new Student_SQLDTO()
            {
                Username = "ThPaquet",
                FirstName = "Thierry",
                LastName = "Paquet",
                Active = true
            });

            classroom.Students.Add(new Student_SQLDTO()
            {
                Username = "ikeameatbol",
                FirstName = "Jonathan",
                LastName = "Blouin",
                Active = true
            });

            classroom.Students.Add(new Student_SQLDTO()
            {
                Username = "BACenComm",
                FirstName = "Melissa",
                LastName = "Lachapelle",
                Active = false
            });

            classroom.Teachers.Add(new Teacher_SQLDTO()
            {
                Username = "PiFou86",
                FirstName = "Pierre-Francois",
                LastName = "Leon",
                Active = true
            });

            classroom.Teachers.Add(new Teacher_SQLDTO()
            {
                Username = "JPDuch",
                FirstName = "Jean-Pierre",
                LastName = "Duchesneau",
                Active = true
            });

            classroom.Teachers.Add(new Teacher_SQLDTO()
            {
                Username = "BoumBoum",
                FirstName = "Andre",
                LastName = "Boumso",
                Active = false
            });

            InsertClassroom(classroom);
        }

        private void InsertMultiplePremadeClassrooms()
        {
            Classroom_SQLDTO classroom = new Classroom_SQLDTO()
            {
                Name = "ProjetSynthese",
                OrganisationName = "CEGEP Ste-Foy",
                Assignments = new List<Assignment_SQLDTO>(),
                Students = new List<Student_SQLDTO>(),
                Teachers = new List<Teacher_SQLDTO>(),
                Active = true
            };

            classroom.Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "Review",
                ClassroomName = "ProjetSynthese",
                DistributionDate = System.DateTime.Now,
                Description = "Review a partner\'s code",
                DeliveryDeadline = System.DateTime.Now.AddDays(1),
                Active = true
            });

            classroom.Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "Scrum",
                ClassroomName = "ProjetSynthese",
                DistributionDate = System.DateTime.Now,
                Description = "Daily briefing",
                DeliveryDeadline = System.DateTime.Now.AddDays(1),
                Active = true
            });

            classroom.Students.Add(new Student_SQLDTO()
            {
                Username = "ThPaquet",
                FirstName = "Thierry",
                LastName = "Paquet",
                Active = true
            });

            classroom.Teachers.Add(new Teacher_SQLDTO()
            {
                Username = "PiFou86",
                FirstName = "Pierre-Francois",
                LastName = "Leon",
                Active = true
            });

            Classroom_SQLDTO classroomTwo = new Classroom_SQLDTO()
            {
                Name = "OOP",
                OrganisationName = "CEGEP Ste-Foy",
                Assignments = new List<Assignment_SQLDTO>(),
                Students = new List<Student_SQLDTO>(),
                Teachers = new List<Teacher_SQLDTO>(),
                Active = true
            };

            classroom.Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "UnitTests",
                ClassroomName = "OOP",
                DistributionDate = System.DateTime.Now,
                Description = "Review a partner\'s code",
                DeliveryDeadline = System.DateTime.Now.AddDays(1)
            });

            classroom.Students.Add(new Student_SQLDTO()
            {
                Username = "ThPaquet",
                FirstName = "Thierry",
                LastName = "Paquet",
                Active = true
            });

            classroom.Teachers.Add(new Teacher_SQLDTO()
            {
                Username = "PiFou86",
                FirstName = "Pierre-Francois",
                LastName = "Leon",
                Active = true
            });

            InsertClassroom(classroom);
            InsertClassroom(classroomTwo);
        }

        [Fact]
        public void Test_GetClassrooms()
        {
            this.DeleteClassroomAndRelatedTablesContent();
            this.InsertMultiplePremadeClassrooms();

            using (var context = new RPLPDbContext(options))
            {
                DepotClassroom depot = new DepotClassroom(context);
                List<Classroom> classrooms = depot.GetClassrooms();

                Assert.Equal(2, classrooms.Count);
                Assert.Contains(classrooms, c => c.Name == "ProjetSynthese");
                Assert.Contains(classrooms, c => c.Name == "OOP");

                Classroom? projetSyntheseClassroom = classrooms.FirstOrDefault(c => c.Name == "ProjetSynthese");
                Assert.NotNull(projetSyntheseClassroom);

                Assert.Equal(2, projetSyntheseClassroom.Assignments.Count);

                Assert.Contains(projetSyntheseClassroom.Assignments, a => a.Name == "Review");
                Assert.Contains(projetSyntheseClassroom.Students, s => s.Username == "ThPaquet");
                Assert.Contains(projetSyntheseClassroom.Teachers, t => t.Username == "PiFou86");
            }

            this.DeleteClassroomAndRelatedTablesContent();
        }

        [Fact]
        public void Test_GetClassroomById()
        {
            this.DeleteClassroomAndRelatedTablesContent();
            this.InsertClassroomProjetSynthese();

            using (var context = new RPLPDbContext(options))
            {
                DepotClassroom depot = new DepotClassroom(context);
                int classroomId = context.Classrooms.FirstOrDefault(c => c.Name == "ProjetSynthese").Id;

                Classroom classroom = depot.GetClassroomById(classroomId);

                Assert.NotNull(classroom);
                Assert.Contains(classroom.Assignments, a => a.Name == "Review");
                Assert.Contains(classroom.Students, s => s.Username == "ThPaquet");
                Assert.Contains(classroom.Teachers, t => t.Username == "PiFou86");
            }

            this.DeleteClassroomAndRelatedTablesContent();
        }

        [Fact]
        public void Test_GetClassroomByName()
        {
            this.DeleteClassroomAndRelatedTablesContent();
            this.InsertClassroomProjetSynthese();

            using (var context = new RPLPDbContext(options))
            {
                DepotClassroom depot = new DepotClassroom(context);

                Classroom classroom = depot.GetClassroomByName("ProjetSynthese");

                Assert.NotNull(classroom);
                Assert.Contains(classroom.Assignments, a => a.Name == "Review");
                Assert.Contains(classroom.Students, s => s.Username == "ThPaquet");
                Assert.Contains(classroom.Teachers, t => t.Username == "PiFou86");
            }

            this.DeleteClassroomAndRelatedTablesContent();
        }

        [Fact]
        public void Test_GetAssignmentsByClassroomName()
        {
            this.DeleteClassroomAndRelatedTablesContent();
            this.InsertClassroomProjetSynthese();

            using (var context = new RPLPDbContext(options))
            {
                DepotClassroom depot = new DepotClassroom(context);

                List<Assignment> assignments = depot.GetAssignmentsByClassroomName("ProjetSynthese");

                Assert.NotNull(assignments);
                Assert.NotNull(assignments.FirstOrDefault(a => a.Name == "Review"));
                Assert.Equal(2, assignments.Count);
            }

            this.DeleteClassroomAndRelatedTablesContent();
        }

        [Fact]
        public void Test_GetStudentsByClassroomName()
        {
            this.DeleteClassroomAndRelatedTablesContent();
            this.InsertClassroomProjetSynthese();

            using (var context = new RPLPDbContext(options))
            {
                DepotClassroom depot = new DepotClassroom(context);

                List<Student> students = depot.GetStudentsByClassroomName("ProjetSynthese");

                Assert.NotNull(students);
                Assert.NotNull(students.FirstOrDefault(a => a.Username == "ThPaquet"));
                Assert.Null(students.FirstOrDefault(a => a.Username == "BACenComm"));
                Assert.Equal(2, students.Count);
            }

            this.DeleteClassroomAndRelatedTablesContent();
        }

        [Fact]
        public void Test_GetTeachersByClassroomName()
        {
            this.DeleteClassroomAndRelatedTablesContent();
            this.InsertClassroomProjetSynthese();

            using (var context = new RPLPDbContext(options))
            {
                DepotClassroom depot = new DepotClassroom(context);

                List<Teacher> teachers = depot.GetTeachersByClassroomName("ProjetSynthese");

                Assert.NotNull(teachers);
                Assert.NotNull(teachers.FirstOrDefault(a => a.Username == "PiFou86"));
                Assert.Null(teachers.FirstOrDefault(a => a.Username == "BoumBoum"));
                Assert.Equal(2, teachers.Count);
            }

            this.DeleteClassroomAndRelatedTablesContent();
        }

        [Fact]
        public void Test_AddAssignmentToClassroom()
        {
            this.DeleteClassroomAndRelatedTablesContent();
            this.InsertClassroomProjetSynthese();

            using (var context = new RPLPDbContext(options))
            {
                Classroom_SQLDTO? classroom = context.Classrooms
                                                    .Include(c => c.Assignments)
                                                    .FirstOrDefault(c => c.Name == "ProjetSynthese");
                Assignment_SQLDTO? assignment = classroom.Assignments.FirstOrDefault(a => a.Name == "Test");

                Assert.Null(assignment);

                context.Assignments.Add(new Assignment_SQLDTO()
                {
                    Name = "Test",
                    ClassroomName = "RPLP",
                    DistributionDate = System.DateTime.Now,
                    Description = "Test",
                    DeliveryDeadline = System.DateTime.Now.AddDays(1),
                    Active = true
                });

                context.SaveChanges();

            }

            using (var context = new RPLPDbContext(options))
            {
                DepotClassroom depot = new DepotClassroom(context);
                depot.AddAssignmentToClassroom("ProjetSynthese", "Test");
            }

            using (var context = new RPLPDbContext(options))
            {
                Classroom_SQLDTO? classroom = context.Classrooms
                                                    .Include(c => c.Assignments)
                                                    .FirstOrDefault(c => c.Name == "ProjetSynthese");
                Assignment_SQLDTO? assignment = classroom.Assignments.FirstOrDefault(a => a.Name == "Test");

                Assert.NotNull(assignment);
            }

            this.DeleteClassroomAndRelatedTablesContent();
        }

        [Fact]
        public void Test_AddStudentToClassroom()
        {
            this.DeleteClassroomAndRelatedTablesContent();
            this.InsertClassroomProjetSynthese();

            using (var context = new RPLPDbContext(options))
            {
                Classroom_SQLDTO? classroom = context.Classrooms
                                                    .Include(c => c.Assignments)
                                                    .FirstOrDefault(c => c.Name == "ProjetSynthese");
                Student_SQLDTO? student = classroom.Students.FirstOrDefault(a => a.Username == "Test");

                Assert.Null(student);

                context.Students.Add(new Student_SQLDTO()
                {
                    Username = "Test",
                    FirstName = "Tester",
                    LastName = "McTesty",
                    Active = true
                });

                context.SaveChanges();

            }

            using (var context = new RPLPDbContext(options))
            {
                DepotClassroom depot = new DepotClassroom(context);
                depot.AddStudentToClassroom("ProjetSynthese", "Test");
            }

            using (var context = new RPLPDbContext(options))
            {
                Classroom_SQLDTO? classroom = context.Classrooms
                                                    .Include(c => c.Students)
                                                    .FirstOrDefault(c => c.Name == "ProjetSynthese");
                Student_SQLDTO? student = classroom.Students.FirstOrDefault(a => a.Username == "Test");

                Assert.NotNull(student);
            }

            this.DeleteClassroomAndRelatedTablesContent();
        }

        [Fact]
        public void Test_AddTeacherToClassroom()
        {
            this.DeleteClassroomAndRelatedTablesContent();
            this.InsertClassroomProjetSynthese();

            using (var context = new RPLPDbContext(options))
            {
                Classroom_SQLDTO? classroom = context.Classrooms
                                                    .Include(c => c.Teachers)
                                                    .FirstOrDefault(c => c.Name == "ProjetSynthese");
                Teacher_SQLDTO? teacher = classroom.Teachers.FirstOrDefault(a => a.Username == "Test");

                Assert.Null(teacher);

                context.Teachers.Add(new Teacher_SQLDTO()
                {
                    Username = "Test",
                    FirstName = "Tester",
                    LastName = "McTesty",
                    Active = true
                });

                context.SaveChanges();

            }

            using (var context = new RPLPDbContext(options))
            {
                DepotClassroom depot = new DepotClassroom(context);
                depot.AddTeacherToClassroom("ProjetSynthese", "Test");
            }

            using (var context = new RPLPDbContext(options))
            {
                Classroom_SQLDTO? classroom = context.Classrooms
                                                    .Include(c => c.Teachers)
                                                    .FirstOrDefault(c => c.Name == "ProjetSynthese");
                Teacher_SQLDTO? teacher = classroom.Teachers.FirstOrDefault(a => a.Username == "Test");

                Assert.NotNull(teacher);
            }

            this.DeleteClassroomAndRelatedTablesContent();
        }

        [Fact]
        public void Test_RemoveAssignmentFromClassroom()
        {
            this.DeleteClassroomAndRelatedTablesContent();
            this.InsertClassroomProjetSynthese();

            using (var context = new RPLPDbContext(options))
            {
                Classroom_SQLDTO? classroom = context
                    .Classrooms
                    .Include(c => c.Assignments)
                    .FirstOrDefault(c => c.Name == "ProjetSynthese");
                Assignment_SQLDTO assignment = classroom.Assignments.FirstOrDefault(a => a.Name == "Review");


                Assert.NotNull(assignment);
            }

            using (var context = new RPLPDbContext(options))
            {
                DepotClassroom depot = new DepotClassroom(context);
                depot.RemoveAssignmentFromClassroom("ProjetSynthese", "Review");
            }

            using (var context = new RPLPDbContext(options))
            {
                Classroom_SQLDTO? classroom = context
                    .Classrooms
                    .Include(c => c.Assignments)
                    .FirstOrDefault(c => c.Name == "ProjetSynthese");
                Assignment_SQLDTO assignment = classroom.Assignments.FirstOrDefault(a => a.Name == "Review");

                Assert.Null(assignment);
            }

            this.DeleteClassroomAndRelatedTablesContent();
        }

        [Fact]
        public void Test_RemoveStudentFromClassroom()
        {
            this.DeleteClassroomAndRelatedTablesContent();
            this.InsertClassroomProjetSynthese();

            using (var context = new RPLPDbContext(options))
            {
                Classroom_SQLDTO? classroom = context
                    .Classrooms
                    .Include(c => c.Students)
                    .FirstOrDefault(c => c.Name == "ProjetSynthese");
                Student_SQLDTO student = classroom.Students.FirstOrDefault(a => a.Username == "ThPaquet");


                Assert.NotNull(student);
            }

            using (var context = new RPLPDbContext(options))
            {
                DepotClassroom depot = new DepotClassroom(context);
                depot.RemoveStudentFromClassroom("ProjetSynthese", "ThPaquet");
            }

            using (var context = new RPLPDbContext(options))
            {
                Classroom_SQLDTO? classroom = context
                    .Classrooms
                    .Include(c => c.Students)
                    .FirstOrDefault(c => c.Name == "ProjetSynthese");
                Student_SQLDTO student = classroom.Students.FirstOrDefault(a => a.Username == "ThPaquet");

                Assert.Null(student);
            }

            this.DeleteClassroomAndRelatedTablesContent();
        }

        [Fact]
        public void Test_RemoveTeacherFromClassroom()
        {
            this.DeleteClassroomAndRelatedTablesContent();
            this.InsertClassroomProjetSynthese();

            using (var context = new RPLPDbContext(options))
            {
                Classroom_SQLDTO? classroom = context
                    .Classrooms
                    .Include(c => c.Teachers)
                    .FirstOrDefault(c => c.Name == "ProjetSynthese");
                Teacher_SQLDTO teacher = classroom.Teachers.FirstOrDefault(a => a.Username == "PiFou86");


                Assert.NotNull(teacher);
            }

            using (var context = new RPLPDbContext(options))
            {
                DepotClassroom depot = new DepotClassroom(context);
                depot.RemoveTeacherFromClassroom("ProjetSynthese", "PiFou86");
            }

            using (var context = new RPLPDbContext(options))
            {
                Classroom_SQLDTO? classroom = context
                    .Classrooms
                    .Include(c => c.Teachers)
                    .FirstOrDefault(c => c.Name == "ProjetSynthese");
                Teacher_SQLDTO teacher = classroom.Teachers.FirstOrDefault(a => a.Username == "PiFou86");

                Assert.Null(teacher);
            }

            this.DeleteClassroomAndRelatedTablesContent();
        }

        [Fact]
        public void Test_UpsertClassroom_Inserts()
        {
            this.DeleteClassroomAndRelatedTablesContent();

            using (var context = new RPLPDbContext(options))
            {
                Classroom_SQLDTO? classroom = context.Classrooms.FirstOrDefault(c => c.Name == "RPLP");
                Assert.Null(classroom);

                DepotClassroom depot = new DepotClassroom(context);

                Classroom newClassroom = new Classroom()
                {
                    Name = "RPLP",
                    OrganisationName = "CEGEP Ste-Foy",
                    Assignments = new List<Assignment>(),
                    Students = new List<Student>(),
                    Teachers = new List<Teacher>()
                };

                depot.UpsertClassroom(newClassroom);
            }
            using (var context = new RPLPDbContext(options))
            {
                Classroom_SQLDTO? classroom = context.Classrooms.FirstOrDefault(c => c.Name == "RPLP");
                Assert.NotNull(classroom);
            }

            this.DeleteClassroomAndRelatedTablesContent();
        }

        [Fact]
        public void Test_UpsertClassroom_Updates()
        {
            this.DeleteClassroomAndRelatedTablesContent();
            this.InsertClassroomProjetSynthese();

            using (var context = new RPLPDbContext(options))
            {
                Classroom_SQLDTO? classroom = context.Classrooms.FirstOrDefault(c => c.Name == "ProjetSynthese");
                Assert.NotNull(classroom);

                DepotClassroom depot = new DepotClassroom(context);

                classroom.OrganisationName = "Test";
                classroom.Assignments.Add(new Assignment_SQLDTO()
                {
                    Name = "TestAssignment",
                    ClassroomName = "ProjetSynthese",
                    DistributionDate = System.DateTime.Now,
                    Description = "AssignmentUpsertTest",
                    DeliveryDeadline = System.DateTime.Now.AddDays(1),
                    Active = true
                });

                classroom.Students.Add(new Student_SQLDTO()
                {
                    Username = "TestStudent",
                    FirstName = "Testy",
                    LastName = "McTestson",
                    Active = true
                });

                classroom.Teachers.Add(new Teacher_SQLDTO()
                {
                    Username = "TestTeacher",
                    FirstName = "Test-Francois",
                    LastName = "Testeon",
                    Active = true
                });

                depot.UpsertClassroom(classroom.ToEntity());
            }

            using (var context = new RPLPDbContext(options))
            {
                Classroom_SQLDTO? classroom = context.Classrooms
                    .Include(c => c.Assignments)
                    .Include(c => c.Students)
                    .Include(c => c.Teachers)
                    .FirstOrDefault(c => c.Name == "ProjetSynthese");

                Assert.NotNull(classroom);
                Assert.NotNull(classroom.Assignments.SingleOrDefault(a => a.Name == "TestAssignment"));
                Assert.NotNull(classroom.Students.SingleOrDefault(a => a.Username == "TestStudent"));
                Assert.NotNull(classroom.Teachers.SingleOrDefault(a => a.Username == "TestTeacher"));
            }

            this.DeleteClassroomAndRelatedTablesContent();
        }

        [Fact]
        public void Test_DeleteClassroom()
        {
            this.DeleteClassroomAndRelatedTablesContent();
            this.InsertClassroomProjetSynthese();

            using (var context = new RPLPDbContext(options))
            {
                Classroom_SQLDTO? classroom = context.Classrooms.FirstOrDefault(c => c.Name == "ProjetSynthese");
                Assert.NotNull(classroom);

                DepotClassroom depot = new DepotClassroom(context);
                depot.DeleteClassroom("ProjetSynthese");
            }

            using (var context = new RPLPDbContext(options))
            {
                Classroom_SQLDTO? classroom = context.Classrooms.FirstOrDefault(c => c.Name == "ProjetSynthese" && c.Active == true);
                Assert.Null(classroom);
            }


            this.DeleteClassroomAndRelatedTablesContent();
        }
    }
}
