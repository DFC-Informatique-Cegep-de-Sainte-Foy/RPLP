using Microsoft.EntityFrameworkCore;
using RPLP.DAL.DTO.Sql;
using RPLP.DAL.SQL;
using RPLP.DAL.SQL.Depots;
using RPLP.ENTITES;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RPLP.UnitTesting
{
    [Collection("DatabaseTests")]
    public class TestsDepotClassroom
    {
        private static readonly DbContextOptions<RPLPDbContext> options = new DbContextOptionsBuilder<RPLPDbContext>()
                .UseSqlServer("Server=localhost,1434; Database=RPLP; User Id=sa; password=Cad3pend86!")
                .Options;

        private void DeleteAssignmentAndRelatedTablesContent()
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
                ClassroomName = "RPLP",
                DistributionDate = System.DateTime.Now,
                Description = "Review a partner\'s code",
                DeliveryDeadline = System.DateTime.Now.AddDays(1)
            });

            classroom.Assignments.Add(new Assignment_SQLDTO()
            {
                Name = "Scrum",
                ClassroomName = "RPLP",
                DistributionDate = System.DateTime.Now,
                Description = "Daily briefing of what is done, what will be done and what is blocking progress.",
                DeliveryDeadline = System.DateTime.Now.AddDays(1)
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
            this.DeleteAssignmentAndRelatedTablesContent();
            this.InsertMultiplePremadeClassrooms();

            using (var context = new RPLPDbContext(options))
            {
                DepotClassroom depot = new DepotClassroom(context);
                List<Classroom> classrooms = depot.GetClassrooms();

                Assert.Equal(2, classrooms.Count);
                Assert.Contains(classrooms, c => c.Name == "ProjetSynthese");
                Assert.Contains(classrooms, c => c.Name == "OOP");

                Classroom? projetSyntheseClassroom = classrooms.FirstOrDefault(c => c.Name == "ProjetSynthese");

                Assert.Contains(projetSyntheseClassroom.Assignments, a => a.Name == "Review");
                Assert.Contains(projetSyntheseClassroom.Students, s => s.Username == "ThPaquet");
                Assert.Contains(projetSyntheseClassroom.Teachers, t => t.Username == "PiFou86");
            }

            this.DeleteAssignmentAndRelatedTablesContent();
        }

        [Fact]
        public void Test_GetClassroomById()
        {
            this.DeleteAssignmentAndRelatedTablesContent();
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

            this.DeleteAssignmentAndRelatedTablesContent();
        }

        [Fact]
        public void Test_GetClassroomByName()
        {
            this.DeleteAssignmentAndRelatedTablesContent();
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

            this.DeleteAssignmentAndRelatedTablesContent();
        }

        [Fact]
        public void Test_GetAssignmentsByClassroomName()
        {

        }
    }
}
