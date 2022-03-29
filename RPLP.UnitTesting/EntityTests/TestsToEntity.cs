using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using RPLP.DAL.DTO.Sql;
using RPLP.ENTITES;

namespace RPLP.UnitTesting.EntityTests
{
    public class TestsToEntity
    {
        [Fact]
        public void Test_Administrator_ToEntity()
        {
            Administrator_SQLDTO administratorSQLDTO = new Administrator_SQLDTO()
            {
                Id = 1,
                FirstName = "Thierry",
                LastName = "Paquet",
                Username = "ThPaquet",
                Token = "Token",
                Organisations = new List<Organisation_SQLDTO>()
                {
                    new Organisation_SQLDTO()
                    {
                        Id = 1,
                        Administrators = new List<Administrator_SQLDTO>(),
                        Name = "CEGEP Ste-Foy"
                    }
                }
            };

            Administrator administrator = administratorSQLDTO.ToEntity();

            Assert.NotNull(administrator);
            Assert.Equal(administratorSQLDTO.Id, administrator.Id);
            Assert.Equal(administratorSQLDTO.Username, administrator.Username);
            Assert.Equal(administratorSQLDTO.FirstName, administrator.FirstName);
            Assert.Equal(administratorSQLDTO.LastName, administrator.LastName);
            Assert.Equal(administratorSQLDTO.Token, administrator.Token);
            Assert.Equal(administratorSQLDTO.Organisations.First().Name, administrator.Organisations.First().Name);
        }

        [Fact]
        public void Test_Administrator_ToEntity_WithoutList()
        {
            Administrator_SQLDTO administratorSQLDTO = new Administrator_SQLDTO()
            {
                Id = 1,
                FirstName = "Thierry",
                LastName = "Paquet",
                Username = "ThPaquet",
                Token = "Token",
                Organisations = new List<Organisation_SQLDTO>()
                {
                    new Organisation_SQLDTO()
                    {
                        Id = 1,
                        Administrators = new List<Administrator_SQLDTO>(),
                        Name = "CEGEP Ste-Foy"
                    }
                }
            };

            Administrator administrator = administratorSQLDTO.ToEntityWithoutList();

            Assert.NotNull(administrator);
            Assert.Equal(administratorSQLDTO.Id, administrator.Id);
            Assert.Equal(administratorSQLDTO.Username, administrator.Username);
            Assert.Equal(administratorSQLDTO.FirstName, administrator.FirstName);
            Assert.Equal(administratorSQLDTO.LastName, administrator.LastName);
            Assert.Equal(administratorSQLDTO.Token, administrator.Token);
            Assert.NotNull(administrator.Organisations);
            Assert.Empty(administrator.Organisations);
        }

        [Fact]
        public void Test_Assignment_ToEntity()
        {
            Assignment_SQLDTO assignmentSQLDTO = new Assignment_SQLDTO()
            {
                Id = 2,
                Name = "RPLP",
                ClassroomName = "ProjetSynthese",
                Description = "Do it",
                DistributionDate = DateTime.Now,
                DeliveryDeadline = DateTime.Now.AddDays(1)
            };

            Assignment assignment = assignmentSQLDTO.ToEntity();

            Assert.NotNull(assignment);
            Assert.Equal(assignmentSQLDTO.Id, assignment.Id);
            Assert.Equal(assignmentSQLDTO.Name, assignment.Name);
            Assert.Equal(assignmentSQLDTO.ClassroomName, assignment.ClassroomName);
            Assert.Equal(assignmentSQLDTO.Description, assignment.Description);
            Assert.Equal(assignmentSQLDTO.DeliveryDeadline, assignment.DeliveryDeadline);
            Assert.Equal(assignmentSQLDTO.DistributionDate, assignment.DistributionDate);
        }

        [Fact]
        public void Test_Classroom_ToEntity()
        {
            Classroom_SQLDTO classroomSQLDTO = new Classroom_SQLDTO()
            {
                Id = 1,
                Name = "ProjetSynthese",
                Assignments = new List<Assignment_SQLDTO>()
                {
                    new Assignment_SQLDTO()
                    {
                        Id = 1,
                        Name = "RPLP"
                    }
                },
                Students = new List<Student_SQLDTO>()
                {
                    new Student_SQLDTO()
                    {
                        Id = 1,
                        Username = "ThPaquet"
                    }
                },
                Teachers = new List<Teacher_SQLDTO>()
                {
                    new Teacher_SQLDTO()
                    {
                        Id = 1,
                        Username = "PiFou86"
                    }
                }
            };

            Classroom classroom = classroomSQLDTO.ToEntity();

            Assert.NotNull(classroom);
            Assert.Equal(classroomSQLDTO.Id, classroom.Id);
            Assert.Equal(classroomSQLDTO.Name, classroom.Name);
            Assert.Equal(classroomSQLDTO.OrganisationName, classroom.OrganisationName);
            Assert.Equal(classroomSQLDTO.Assignments.First().Name, classroom.Assignments.First().Name);
            Assert.Equal(classroomSQLDTO.Students.First().Username, classroom.Students.First().Username);
            Assert.Equal(classroomSQLDTO.Teachers.First().Username, classroom.Teachers.First().Username);
        }

        [Fact]
        public void Test_Classroom_ToEntityWithoutList()
        {
            Classroom_SQLDTO classroomSQLDTO = new Classroom_SQLDTO()
            {
                Id = 1,
                Name = "ProjetSynthese",
                Assignments = new List<Assignment_SQLDTO>()
                {
                    new Assignment_SQLDTO()
                    {
                        Id = 1,
                        Name = "RPLP"
                    }
                },
                Students = new List<Student_SQLDTO>()
                {
                    new Student_SQLDTO()
                    {
                        Id = 1,
                        Username = "ThPaquet"
                    }
                },
                Teachers = new List<Teacher_SQLDTO>()
                {
                    new Teacher_SQLDTO()
                    {
                        Id = 1,
                        Username = "PiFou86"
                    }
                }
            };

            Classroom classroom = classroomSQLDTO.ToEntityWithoutList();

            Assert.NotNull(classroom);
            Assert.Equal(classroomSQLDTO.Id, classroom.Id);
            Assert.Equal(classroomSQLDTO.Name, classroom.Name);
            Assert.Equal(classroomSQLDTO.OrganisationName, classroom.OrganisationName);
            Assert.NotNull(classroom.Assignments);
            Assert.Empty(classroom.Assignments);
            Assert.NotNull(classroom.Students);
            Assert.NotNull(classroom.Students);
            Assert.Empty(classroom.Teachers);
            Assert.Empty(classroom.Teachers);
        }

        [Fact]
        public void Test_Comment_ToEntity()
        {
            Comment_SQLDTO comment_SQLDTO = new Comment_SQLDTO()
            {
                Id = 3,
                WrittenBy = "ThierryPaquet",
                RepositoryName = "ThPaquet",
                Diff_Hunk = "Hunk",
                Path = "Path",
                Original_Position = 4,
                Body = "Ready",
                In_Reply_To_Id = 5,
                Created_at = DateTime.MinValue,
                Updated_at = DateTime.MaxValue
            };

            Comment comment = comment_SQLDTO.ToEntity();

            Assert.NotNull(comment);
            Assert.Equal(comment_SQLDTO.Id, comment.Id);
            Assert.Equal(comment_SQLDTO.WrittenBy, comment.WrittenBy);
            Assert.Equal(comment_SQLDTO.RepositoryName, comment.RepositoryName);
            Assert.Equal(comment_SQLDTO.Diff_Hunk, comment.Diff_Hunk);
            Assert.Equal(comment_SQLDTO.Path, comment.Path);
            Assert.Equal(comment_SQLDTO.Original_Position, comment.Original_Position);
            Assert.Equal(comment_SQLDTO.Body, comment.Body);
            Assert.Equal(comment_SQLDTO.In_Reply_To_Id, comment.In_Reply_To_Id);
            Assert.Equal(comment_SQLDTO.Created_at, comment.Created_at);
            Assert.Equal(comment_SQLDTO.Updated_at, comment.Updated_at);
        }

        [Fact]
        public void Test_Organisation_ToEntity()
        {
            Organisation_SQLDTO organisation_SQLDTO = new Organisation_SQLDTO()
            {
                Id = 2,
                Name = "CEGEP Ste-Foy",
                Administrators = new List<Administrator_SQLDTO>()
                {
                    new Administrator_SQLDTO()
                    {
                        Username = "ThPaquet"
                    }
                }
            };

            Organisation organisation = organisation_SQLDTO.ToEntity();

            Assert.NotNull(organisation);
            Assert.Equal(organisation_SQLDTO.Id, organisation.Id);
            Assert.Equal(organisation_SQLDTO.Name, organisation.Name);
            Assert.Equal(organisation_SQLDTO.Administrators.First().Username, organisation.Administrators.First().Username);
        }

        [Fact]
        public void Test_Organisation_ToEntityWithoutList()
        {
            Organisation_SQLDTO organisation_SQLDTO = new Organisation_SQLDTO()
            {
                Id = 2,
                Name = "CEGEP Ste-Foy",
                Administrators = new List<Administrator_SQLDTO>()
                {
                    new Administrator_SQLDTO()
                    {
                        Username = "ThPaquet"
                    }
                }
            };

            Organisation organisation = organisation_SQLDTO.ToEntityWithoutList();

            Assert.NotNull(organisation);
            Assert.Equal(organisation_SQLDTO.Id, organisation.Id);
            Assert.Equal(organisation_SQLDTO.Name, organisation.Name);
            Assert.NotNull(organisation.Administrators);
            Assert.Empty(organisation.Administrators);
        }

        [Fact]
        public void Test_Repository_ToEntity()
        {
            Repository_SQLDTO repository_SQLDTO = new Repository_SQLDTO()
            {
                Id = 3,
                Name = "ThPaquet",
                FullName = "Thierry Paquet",
                OrganisationName = "CEGEP Ste-Foy"
            };

            Repository repository = repository_SQLDTO.ToEntity();

            Assert.NotNull(repository);
            Assert.Equal(repository_SQLDTO.Id, repository.Id);
            Assert.Equal(repository_SQLDTO.Name, repository.Name);
            Assert.Equal(repository_SQLDTO.FullName, repository.FullName);
            Assert.Equal(repository_SQLDTO.OrganisationName, repository.OrganisationName);
        }

        [Fact]
        public void Test_Student_ToEntity()
        {
            Student_SQLDTO student_SQLDTO = new Student_SQLDTO()
            {
                Id = 9,
                Username = "ThPaquet",
                FirstName = "Thierry",
                LastName = "Paquet",
                Classes = new List<Classroom_SQLDTO>()
                {
                    new Classroom_SQLDTO()
                    {
                        Id = 2,
                        Name = "ProjetSynthese"
                    }
                }
            };

            Student student = student_SQLDTO.ToEntity();

            Assert.NotNull(student);
            Assert.Equal(student_SQLDTO.Id, student.Id);
            Assert.Equal(student_SQLDTO.Username, student.Username);
            Assert.Equal(student_SQLDTO.FirstName, student.FirstName);
            Assert.Equal(student_SQLDTO.LastName, student.LastName);
            Assert.Equal(student_SQLDTO.Classes.First().Name, student.Classes.First().Name);
        }


        [Fact]
        public void Test_Student_ToEntityWithoutList()
        {
            Student_SQLDTO student_SQLDTO = new Student_SQLDTO()
            {
                Id = 9,
                Username = "ThPaquet",
                FirstName = "Thierry",
                LastName = "Paquet",
                Classes = new List<Classroom_SQLDTO>()
                {
                    new Classroom_SQLDTO()
                    {
                        Id = 2,
                        Name = "ProjetSynthese"
                    }
                }
            };

            Student student = student_SQLDTO.ToEntityWithoutList();

            Assert.NotNull(student);
            Assert.Equal(student_SQLDTO.Id, student.Id);
            Assert.Equal(student_SQLDTO.Username, student.Username);
            Assert.Equal(student_SQLDTO.FirstName, student.FirstName);
            Assert.Equal(student_SQLDTO.LastName, student.LastName);
            Assert.NotNull(student.Classes);
            Assert.Empty(student.Classes);
        }


        [Fact]
        public void Test_Teacher_ToEntity()
        {
            Teacher_SQLDTO teacher_SQLDTO = new Teacher_SQLDTO()
            {
                Id = 9,
                Username = "ThPaquet",
                FirstName = "Thierry",
                LastName = "Paquet",
                Classes = new List<Classroom_SQLDTO>()
                {
                    new Classroom_SQLDTO()
                    {
                        Id = 2,
                        Name = "ProjetSynthese"
                    }
                }
            };

            Teacher teacher = teacher_SQLDTO.ToEntity();

            Assert.NotNull(teacher);
            Assert.Equal(teacher_SQLDTO.Id, teacher.Id);
            Assert.Equal(teacher_SQLDTO.Username, teacher.Username);
            Assert.Equal(teacher_SQLDTO.FirstName, teacher.FirstName);
            Assert.Equal(teacher_SQLDTO.LastName, teacher.LastName);
            Assert.Equal(teacher_SQLDTO.Classes.First().Name, teacher.Classes.First().Name);
        }

        [Fact]
        public void Test_Teacher_ToEntityWithoutList()
        {
            Teacher_SQLDTO teacher_SQLDTO = new Teacher_SQLDTO()
            {
                Id = 9,
                Username = "ThPaquet",
                FirstName = "Thierry",
                LastName = "Paquet",
                Classes = new List<Classroom_SQLDTO>()
                {
                    new Classroom_SQLDTO()
                    {
                        Id = 2,
                        Name = "ProjetSynthese"
                    }
                }
            };

            Teacher teacher = teacher_SQLDTO.ToEntityWithoutList();

            Assert.NotNull(teacher);
            Assert.Equal(teacher_SQLDTO.Id, teacher.Id);
            Assert.Equal(teacher_SQLDTO.Username, teacher.Username);
            Assert.Equal(teacher_SQLDTO.FirstName, teacher.FirstName);
            Assert.Equal(teacher_SQLDTO.LastName, teacher.LastName);
            Assert.NotNull(teacher.Classes);
            Assert.Empty(teacher.Classes);
        }
    }
}
