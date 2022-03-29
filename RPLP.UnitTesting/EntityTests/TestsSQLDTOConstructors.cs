using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPLP.DAL.DTO.Sql;
using RPLP.ENTITES;
using Xunit;

namespace RPLP.UnitTesting.EntityTests
{
    public class TestsSQLDTOConstructors
    {
        [Fact]
        public void Test_AdministratorSQLDTOConstructor_NoArgument()
        {
            Administrator_SQLDTO administrator = new Administrator_SQLDTO();

            Assert.NotNull(administrator);
            Assert.NotNull(administrator.Organisations);
            Assert.Empty(administrator.Organisations);
            Assert.False(administrator.Active);
        }

        [Fact]
        public void Test_AdministratorSQLDTOConstructor_FromEntity()
        {
            Administrator administrator = new Administrator()
            {
                Id = 1,
                FirstName = "Thierry",
                LastName = "Paquet",
                Username = "ThPaquet",
                Token = "Token",
                Organisations = new List<Organisation>()
                {
                    new Organisation()
                    {
                        Id = 1,
                        Administrators = new List<Administrator>(),
                        Name = "CEGEP Ste-Foy"
                    }
                }
            };

            Administrator_SQLDTO administrator_SQLDTO = new Administrator_SQLDTO(administrator);

            Assert.NotNull(administrator_SQLDTO);
            Assert.Equal(administrator.Id, administrator_SQLDTO.Id);
            Assert.Equal(administrator.Username, administrator_SQLDTO.Username);
            Assert.Equal(administrator.FirstName, administrator_SQLDTO.FirstName);
            Assert.Equal(administrator.LastName, administrator_SQLDTO.LastName);
            Assert.Equal(administrator.Token, administrator_SQLDTO.Token);
            Assert.Equal(administrator.Organisations.First().Name, administrator_SQLDTO.Organisations.First().Name);
            Assert.True(administrator_SQLDTO.Active);
        }

        [Fact]
        public void Test_AssignmentConstructor_WithoutArgument()
        {
            Assignment_SQLDTO assignment = new Assignment_SQLDTO();

            Assert.NotNull(assignment);
            Assert.NotNull(assignment.DistributionDate);
            Assert.Null(assignment.DeliveryDeadline);
            Assert.False(assignment.Active);
        }

        [Fact]
        public void Test_AssignmentSQLDTOConstructor_FromEntity()
        {
            Assignment assignment = new Assignment()
            {
                Id = 2,
                Name = "RPLP",
                ClassroomName = "ProjetSynthese",
                Description = "Do it",
                DistributionDate = DateTime.Now,
                DeliveryDeadline = DateTime.Now.AddDays(1)
            };

            Assignment_SQLDTO assignment_SQLDTO = new Assignment_SQLDTO(assignment);

            Assert.NotNull(assignment_SQLDTO);
            Assert.Equal(assignment.Id, assignment_SQLDTO.Id);
            Assert.Equal(assignment.Name, assignment_SQLDTO.Name);
            Assert.Equal(assignment.ClassroomName, assignment_SQLDTO.ClassroomName);
            Assert.Equal(assignment.Description, assignment_SQLDTO.Description);
            Assert.Equal(assignment.DeliveryDeadline, assignment_SQLDTO.DeliveryDeadline);
            Assert.Equal(assignment.DistributionDate, assignment_SQLDTO.DistributionDate);
            Assert.True(assignment_SQLDTO.Active);
        }

        [Fact]
        public void Test_ClassroomSQLDTOConstructor_WithoutArgument()
        {
            Classroom_SQLDTO classroom = new Classroom_SQLDTO();

            Assert.NotNull(classroom);
            Assert.NotNull(classroom.Teachers);
            Assert.Empty(classroom.Teachers);
            Assert.NotNull(classroom.Students);
            Assert.Empty(classroom.Students);
            Assert.NotNull(classroom.Assignments);
            Assert.Empty(classroom.Assignments);
            Assert.False(classroom.Active); 
        }

        [Fact]
        public void Test_ClassroomSQLDTOConstructor_FromEntity()
        {
            Classroom classroom = new Classroom()
            {
                Id = 1,
                Name = "ProjetSynthese",
                Assignments = new List<Assignment>()
                {
                    new Assignment()
                    {
                        Id = 1,
                        Name = "RPLP"
                    }
                },
                Students = new List<Student>()
                {
                    new Student()
                    {
                        Id = 1,
                        Username = "ThPaquet"
                    }
                },
                Teachers = new List<Teacher>()
                {
                    new Teacher()
                    {
                        Id = 1,
                        Username = "PiFou86"
                    }
                }
            };


            Classroom_SQLDTO classroom_SQLDTO = new Classroom_SQLDTO(classroom);

            Assert.NotNull(classroom_SQLDTO);
            Assert.Equal(classroom.Id, classroom_SQLDTO.Id);
            Assert.Equal(classroom.OrganisationName, classroom_SQLDTO.OrganisationName);
            Assert.Equal(classroom.Name, classroom_SQLDTO.Name);
            Assert.Equal(classroom.Students.First().Username, classroom_SQLDTO.Students.First().Username);
            Assert.Equal(classroom.Teachers.First().Username, classroom_SQLDTO.Teachers.First().Username);
            Assert.Equal(classroom.Assignments.First().Name, classroom_SQLDTO.Assignments.First().Name);
            Assert.True(classroom_SQLDTO.Active);
        }

        [Fact]
        public void Test_CommentSQLDTOConstructor_WithoutArguments()
        {
            Comment_SQLDTO comment = new Comment_SQLDTO();

            Assert.NotNull(comment);
            Assert.NotNull(comment.Created_at);
            Assert.NotNull(comment.Updated_at);
            Assert.False(comment.Active);
        }

        [Fact]
        public void Test_CommentSQLDTOConstructor_FromEntity()
        {


            Comment comment = new Comment()
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

            Comment_SQLDTO comment_SQLDTO = new Comment_SQLDTO(comment);

            Assert.NotNull(comment_SQLDTO);
            Assert.Equal(comment.Id, comment_SQLDTO.Id);
            Assert.Equal(comment.WrittenBy, comment_SQLDTO.WrittenBy);
            Assert.Equal(comment.RepositoryName, comment_SQLDTO.RepositoryName);
            Assert.Equal(comment.Diff_Hunk, comment_SQLDTO.Diff_Hunk);
            Assert.Equal(comment.Path, comment_SQLDTO.Path);
            Assert.Equal(comment.Original_Position, comment_SQLDTO.Original_Position);
            Assert.Equal(comment.Body, comment_SQLDTO.Body);
            Assert.Equal(comment.In_Reply_To_Id, comment_SQLDTO.In_Reply_To_Id);
            Assert.Equal(comment.Created_at, comment_SQLDTO.Created_at);
            Assert.Equal(comment.Updated_at, comment_SQLDTO.Updated_at);
            Assert.True(comment_SQLDTO.Active);
        }

        [Fact]
        public void Test_OrganisationSQLDDTOContructor_WithoutArguments()
        {
            Organisation_SQLDTO organisation = new Organisation_SQLDTO();

            Assert.NotNull(organisation);
            Assert.NotNull(organisation.Administrators);
            Assert.Empty(organisation.Administrators);
            Assert.False(organisation.Active);
        }

        [Fact]
        public void Test_OrganisationSQLDTOConstructor_FromEntity()
        {
            Organisation organisation = new Organisation()
            {
                Id = 2,
                Name = "CEGEP Ste-Foy",
                Administrators = new List<Administrator>()
                {
                    new Administrator()
                    {
                        Username = "ThPaquet"
                    }
                }
            };

            Organisation_SQLDTO organisation_SQLDTO = new Organisation_SQLDTO(organisation);

            Assert.NotNull(organisation_SQLDTO);
            Assert.Equal(organisation.Id, organisation_SQLDTO.Id);
            Assert.Equal(organisation.Name, organisation_SQLDTO.Name);
            Assert.Equal(organisation.Administrators.First().Username, organisation_SQLDTO.Administrators.First().Username);
            Assert.True(organisation_SQLDTO.Active);
        }

        [Fact]
        public void Test_RepositorySQLDTOConstructor_WithoutArguments()
        {
            Repository_SQLDTO repository = new Repository_SQLDTO();

            Assert.NotNull(repository);
            Assert.False(repository.Active);
        }

        [Fact]
        public void Test_RepositorySQLDTOConstructor_FromEntity()
        {
            Repository repository = new Repository()
            {
                Id = 3,
                Name = "ThPaquet",
                FullName = "Thierry Paquet",
                OrganisationName = "CEGEP Ste-Foy"
            };

            Repository_SQLDTO repository_SQLDTO = new Repository_SQLDTO(repository);

            Assert.NotNull(repository);
            Assert.Equal(repository.Id, repository_SQLDTO.Id);
            Assert.Equal(repository.Name, repository_SQLDTO.Name);
            Assert.Equal(repository.FullName, repository_SQLDTO.FullName);
            Assert.Equal(repository.OrganisationName, repository_SQLDTO.OrganisationName);
            Assert.True(repository_SQLDTO.Active);
        }

        [Fact]
        public void Test_StudentSQLDTOConstructor_WithoutArguments()
        {
            Student_SQLDTO student = new Student_SQLDTO();

            Assert.NotNull(student);
            Assert.NotNull(student.Classes);
            Assert.Empty(student.Classes);
            Assert.False(student.Active);
        }

        [Fact]
        public void Test_StudentSQLDTOConstructor_FromEntity()
        {
            Student student = new Student()
            {
                Id = 9,
                Username = "ThPaquet",
                FirstName = "Thierry",
                LastName = "Paquet",
                Classes = new List<Classroom>()
                {
                    new Classroom()
                    {
                        Id = 2,
                        Name = "ProjetSynthese"
                    }
                }
            };

            Student_SQLDTO student_SQLDTO = new Student_SQLDTO(student);

            Assert.NotNull(student_SQLDTO);
            Assert.Equal(student.Id, student_SQLDTO.Id);
            Assert.Equal(student.Username, student_SQLDTO.Username);
            Assert.Equal(student.FirstName, student_SQLDTO.FirstName);
            Assert.Equal(student.LastName, student_SQLDTO.LastName);
            Assert.Equal(student.Classes.First().Name, student_SQLDTO.Classes.First().Name);
            Assert.True(student_SQLDTO.Active);
        }

        [Fact]
        public void Test_TeacherConstructor_WithoutArguments()
        {
            Teacher_SQLDTO teacher = new Teacher_SQLDTO();

            Assert.NotNull(teacher);
            Assert.NotNull(teacher.Classes);
            Assert.Empty(teacher.Classes);
            Assert.False(teacher.Active);
        }

        [Fact]
        public void Test_TeacherSQLDTOConstructer_FromEntity()
        {
            Teacher teacher = new Teacher()
            {
                Id = 9,
                Username = "ThPaquet",
                FirstName = "Thierry",
                LastName = "Paquet",
                Classes = new List<Classroom>()
                {
                    new Classroom()
                    {
                        Id = 2,
                        Name = "ProjetSynthese"
                    }
                }
            };

            Teacher_SQLDTO teacher_SQLDTO = new Teacher_SQLDTO(teacher);

            Assert.NotNull(teacher_SQLDTO);
            Assert.Equal(teacher.Id, teacher_SQLDTO.Id);
            Assert.Equal(teacher.Username, teacher_SQLDTO.Username);
            Assert.Equal(teacher.FirstName, teacher_SQLDTO.FirstName);
            Assert.Equal(teacher.LastName, teacher_SQLDTO.LastName);
            Assert.Equal(teacher.Classes.First().Name, teacher_SQLDTO.Classes.First().Name);
            Assert.True(teacher_SQLDTO.Active);
        }
    }
}
