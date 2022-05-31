using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPLP.ENTITES;
using Xunit;

namespace RPLP.UnitTesting.EntityTests
{
    public class TestsEntityConstructors
    {
        [Fact]
        public void Test_AdministratorConstructor_NoArgument()
        {
            Administrator administrator = new Administrator();

            Assert.NotNull(administrator);
            Assert.NotNull(administrator.Organisations);
            Assert.Empty(administrator.Organisations);
        }


        [Fact]
        public void Test_AdministratorConstructor_WithArguments()
        {
            int id = 1;
            string username = "ThPaquet";
            string token = "Token";
            string firstName = "Thierry";
            string lastName = "Paquet";
            string email = "Th_Paquet@hotmail.com";
            List<Organisation> organisations = new List<Organisation>();
            Administrator administrator = new Administrator(id, username, token, firstName, lastName, email, organisations);

            Assert.Equal(id, administrator.Id);
            Assert.Equal(username, administrator.Username);
            Assert.Equal(token, administrator.Token);
            Assert.Equal(lastName, administrator.LastName);
            Assert.Equal(email, administrator.Email);
            Assert.Equal(organisations, administrator.Organisations);
        }

        [Fact]
        public void Test_AssignmentConstructor_NoArgument()
        {
            Assignment assignment = new Assignment();

            Assert.NotNull(assignment);
            Assert.NotNull(assignment.DistributionDate);
            Assert.Null(assignment.DeliveryDeadline);
        }

        [Fact]
        public void Test_AssignmentConstructor_Arguments_NoDeadline()
        {
            int id = 2;
            string name = "NewAssignment";
            string classroomName = "ProjetSynthese";
            string repositoryName = "PiFou86";
            string description = "RPLP";
            DateTime distributionDate = DateTime.Now;

            Assignment assignment = new Assignment(id, name, classroomName, description, distributionDate);

            Assert.Equal(id, assignment.Id);
            Assert.Equal(name, assignment.Name);
            Assert.Equal(classroomName, assignment.ClassroomName);
            Assert.Equal(description, assignment.Description);
            Assert.Equal(distributionDate, assignment.DistributionDate);
        }

        [Fact]
        public void Test_AssignmentConstructor_Arguments_WithDeadline()
        {
            int id = 2;
            string name = "NewAssignment";
            string classroomName = "ProjetSynthese";
            string repositoryName = "PiFou86";
            string description = "RPLP";
            DateTime distributionDate = DateTime.Now;
            DateTime deadline = DateTime.Now.AddDays(1);

            Assignment assignment = new Assignment(id, name, classroomName, description, deadline, distributionDate);

            Assert.Equal(id, assignment.Id);
            Assert.Equal(name, assignment.Name);
            Assert.Equal(classroomName, assignment.ClassroomName);
            Assert.Equal(description, assignment.Description);
            Assert.Equal(distributionDate, assignment.DistributionDate);
            Assert.Equal(deadline, assignment.DeliveryDeadline);
        }

        [Fact]
        public void Test_ClassroomConstructor_NoArgument()
        {
            Classroom classroom = new Classroom();

            Assert.NotNull(classroom);
            Assert.NotNull(classroom.Teachers);
            Assert.Empty(classroom.Teachers);
            Assert.NotNull(classroom.Students);
            Assert.Empty(classroom.Students);
            Assert.NotNull(classroom.Assignments);
            Assert.Empty(classroom.Assignments);
        }

        [Fact]
        public void Test_ClassroomConstructor_WithArguments()
        {
            int id = 3;
            string name = "ProjetSynthese";
            string organisationName = "CEGEP Ste-Foy";
            List<Student> students = new List<Student>();
            List<Teacher> teachers = new List<Teacher>();
            List<Assignment> assignments = new List<Assignment>();

            Classroom classroom = new Classroom(id, name, organisationName, students, teachers, assignments);

            Assert.Equal(id, classroom.Id);
            Assert.Equal(name, classroom.Name);
            Assert.Equal(organisationName, classroom.OrganisationName);
            Assert.Equal(students, classroom.Students);
            Assert.Equal(teachers, classroom.Teachers);
            Assert.Equal(assignments, classroom.Assignments);
        }

        [Fact]
        public void Test_CommentConstructor_WithoutArguments()
        {
            Comment comment = new Comment();

            Assert.NotNull(comment);
            Assert.NotNull(comment.Created_at);
            Assert.NotNull(comment.Updated_at);
        }

        [Fact]
        public void Test_CommentConstructor_WithPartialArguments()
        {
            int id = 4;
            string writtenBy = "ThierryPaquet";
            string repositoryName = "ThPaquet";
            string body = "Some";
            DateTime createdAt = DateTime.Now;
            DateTime updatedAt = DateTime.Now.AddDays(1);

            Comment comment = new Comment(id, writtenBy, repositoryName, body, createdAt, updatedAt);

            Assert.NotNull(comment);
            Assert.Equal(id, comment.Id);
            Assert.Equal(writtenBy, comment.WrittenBy);
            Assert.Equal(repositoryName, comment.RepositoryName);
            Assert.Equal(body, comment.Body);
            Assert.Equal(createdAt, comment.Created_at);
            Assert.Equal(updatedAt, comment.Updated_at);
        }

        [Fact]
        public void Test_CommentConstructor_WithAllArguments()
        {
            int id = 4;
            string writtenBy = "ThierryPaquet";
            string repositoryName = "ThPaquet";
            string diffHunk = "diffHunk";
            string path = "forward";
            int originalPosition = 42;
            string body = "Some";
            int inReplyToId = 5;
            DateTime createdAt = DateTime.Now;
            DateTime updatedAt = DateTime.Now.AddDays(1);

            Comment comment = new Comment(id, writtenBy, repositoryName, diffHunk, path, originalPosition, inReplyToId, body, createdAt, updatedAt);

            Assert.NotNull(comment);
            Assert.Equal(id, comment.Id);
            Assert.Equal(writtenBy, comment.WrittenBy);
            Assert.Equal(diffHunk, comment.Diff_Hunk);
            Assert.Equal(path, comment.Path);
            Assert.Equal(originalPosition, comment.Original_Position);
            Assert.Equal(repositoryName, comment.RepositoryName);
            Assert.Equal(inReplyToId, comment.In_Reply_To_Id);
            Assert.Equal(body, comment.Body);
            Assert.Equal(createdAt, comment.Created_at);
            Assert.Equal(updatedAt, comment.Updated_at);
        }

        [Fact]
        public void Test_OrganisationContructor_WithoutArguments()
        {
            Organisation organisation = new Organisation();

            Assert.NotNull(organisation);
            Assert.NotNull(organisation.Administrators);
            Assert.Empty(organisation.Administrators);
        }

        [Fact]
        public void Test_OrganisationContructor_WithArguments()
        {
            int id = 5;
            string name = "CEGEP Ste-Foy";
            List<Administrator> administrators = new List<Administrator>();

            Organisation organisation = new Organisation(id, name, administrators);

            Assert.NotNull(organisation);
            Assert.Equal(id, organisation.Id);
            Assert.Equal(name, organisation.Name);
            Assert.Equal(administrators, organisation.Administrators);
        }

        [Fact]
        public void Test_RepositoryConstructor_WithoutArguments()
        {
            Repository repository = new Repository();

            Assert.NotNull(repository);
        }

        [Fact]
        public void Test_RepositoryConstructor_WithArguments()
        {
            int id = 6;
            string name = "ThPaquet";
            string organisationName = "CEGEP Ste-Foy";
            string fullName = "Thierry Paquet";

            Repository repository = new Repository(id, name, organisationName, fullName);

            Assert.NotNull(repository);
            Assert.Equal(id, repository.Id);
            Assert.Equal(name, repository.Name);
            Assert.Equal(organisationName, repository.OrganisationName);
            Assert.Equal(fullName, repository.FullName);
        }

        [Fact]
        public void Test_StudentConstructor_WithoutArguments()
        {
            Student student = new Student();

            Assert.NotNull(student);
            Assert.NotNull(student.Classes);
            Assert.Empty(student.Classes);
        }

        [Fact]
        public void Test_StudentConstructor_WithArguments()
        {
            int id = 7;
            string username = "ThPaquet";
            string firstName = "Thierry";
            string lastName = "Paquet";
            string email = "Th_Paquet@hotmail.com";
            string matricule = "1234567";
            List<Classroom> classes = new List<Classroom>();

            Student student = new Student(id, username, firstName, lastName, email, matricule, classes);

            Assert.NotNull(student);
            Assert.Equal(id, student.Id);
            Assert.Equal(username, student.Username);
            Assert.Equal(firstName, student.FirstName);
            Assert.Equal(lastName, student.LastName);
            Assert.Equal(email, student.Email);
            Assert.Equal(matricule, student.Matricule);
            Assert.Equal(classes, student.Classes);
        }

        [Fact]
        public void Test_TeacherConstructor_WithoutArguments()
        {
            Teacher teacher = new Teacher();

            Assert.NotNull(teacher);
            Assert.NotNull(teacher.Classes);
            Assert.Empty(teacher.Classes);
        }

        [Fact]
        public void Test_TeacherConstructor_WithArguments()
        {
            int id = 7;
            string username = "ThPaquet";
            string firstName = "Thierry";
            string lastName = "Paquet";
            string email = "Th_Paquet@hotmail.com";
            List<Classroom> classes = new List<Classroom>();

            Teacher teacher = new Teacher(id, username, firstName, lastName, email, classes);

            Assert.NotNull(teacher);
            Assert.Equal(id, teacher.Id);
            Assert.Equal(username, teacher.Username);
            Assert.Equal(firstName, teacher.FirstName);
            Assert.Equal(lastName, teacher.LastName);
            Assert.Equal(email, teacher.Email);
            Assert.Equal(classes, teacher.Classes);
        }
    }
}
