using Microsoft.EntityFrameworkCore;
using RPLP.DAL.DTO.Sql;
using RPLP.ENTITES;
using RPLP.JOURNALISATION;
using RPLP.ENTITES.InterfacesDepots;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.DAL.SQL.Depots
{
    public class DepotStudent : IDepotStudent
    {
        private readonly RPLPDbContext _context;

        public DepotStudent(RPLPDbContext p_context)
        {
            if (p_context == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                               "DepotStudent - DepotStudent(RPLPDbContext p_context) - p_context de type RPLPDbContext passé en paramètre est null", 0));
            }

            this._context = p_context;
        }

        public List<Student> GetStudents()
        {
            List<Student_SQLDTO> studentsResult = this._context.Students.Where(student => student.Active)
                                                                        .Include(student => student.Classes.Where(classroom => classroom.Active)).ToList();

            List<Student> students = studentsResult.Select(admin => admin.ToEntityWithoutList()).ToList();

            if (students == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                               "DepotStudent - GetStudents - la liste students de type Student assignée à partir de la méthode studentsResult.Select(admin => admin.ToEntityWithoutList()).ToList(); est null", 0));
            }

            for (int i = 0; i < studentsResult.Count; i++)
            {
                if (studentsResult[i].Id == students[i].Id && studentsResult[i].Classes.Count >= 1)
                    students[i].Classes = studentsResult[i].Classes.Select(classroom => classroom.ToEntityWithoutList()).ToList();
            }

            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Student", $"DepotStudent - Method - GetStudents() - Return List<Student>"));

            return students;
        }

        public List<Student> GetDeactivatedStudents()
        {
            List<Student_SQLDTO> studentsResult = this._context.Students.Where(student => !student.Active)
                                                                        .Include(student => student.Classes.Where(classroom => classroom.Active)).ToList();

            List<Student> students = studentsResult.Select(admin => admin.ToEntityWithoutList()).ToList();

            if (students == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                               "DepotStudent - GetDeactivatedStudents - la liste students de type Student assignée à partir de la méthode studentsResult.Select(admin => admin.ToEntityWithoutList()).ToList(); est null", 0));
            }

            for (int i = 0; i < studentsResult.Count; i++)
            {
                if (studentsResult[i].Id == students[i].Id && studentsResult[i].Classes.Count >= 1)
                    students[i].Classes = studentsResult[i].Classes.Select(classroom => classroom.ToEntityWithoutList()).ToList();
            }

            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Student", $"DepotStudent - Method - GetDeactivatedStudents() - Return List<Student>"));

            return students;
        }

        public Student GetStudentById(int p_id)
        {
            if (p_id < 0)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "DepotStudent - GetStudentById - p_id passé en paramêtre est hors des limites", 0));
            }

            Student_SQLDTO studentResult = this._context.Students
                .Include(student => student.Classes.Where(classroom => classroom.Active))
                .FirstOrDefault(student => student.Id == p_id && student.Active);

            
            if (studentResult == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Student", $"DepotStudent - Method - GetStudentById(int p_id) - Return Student - studentResult est null",0));

                return null;
            }
               
            Student student = studentResult.ToEntityWithoutList();

            if (studentResult.Classes.Count >= 1)
            {
                List<Classroom> classes = studentResult.Classes.Select(classroom => classroom.ToEntityWithoutList()).ToList();
                student.Classes = classes;

                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Student", $"DepotStudent - Method - GetStudentById(int p_id) - Return Student"));
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Student", $"DepotStudent - Method - GetStudentById(int p_id) - Return Student - studentResult.Classes.Count est vide",0));
            }

            return student;
        }

        public Student GetStudentByUsername(string p_studentUsername)
        {
            if (string.IsNullOrWhiteSpace(p_studentUsername))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotStudent - GetStudentByUsername - p_studentUsername passé en paramètre est vide", 0));
            }

            Student_SQLDTO studentResult = this._context.Students
                .Include(student => student.Classes.Where(classroom => classroom.Active))
                .FirstOrDefault(student => student.Username == p_studentUsername && student.Active);


            if (studentResult == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Student", $"DepotStudent - Method - GetStudentByUsername(string p_studentUsername) - Return Student - studentResult est null",0));
                return null;
            }
             
            Student student = studentResult.ToEntityWithoutList();

            if (studentResult.Classes.Count >= 1)
            {
                List<Classroom> classes = studentResult.Classes.Select(classroom => classroom.ToEntityWithoutList()).ToList();
                student.Classes = classes;

                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Student", $"DepotStudent - Method - GetStudentByUsername(string p_studentUsername) - Return Student"));
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Student", $"DepotStudent - Method - GetStudentByUsername(string p_studentUsername) - Return Student - studentResult.Classes.Count est vide",0));
            }
            return student;
        }

        public List<Classroom> GetStudentClasses(string p_studentUsername)
        {
            if (string.IsNullOrWhiteSpace(p_studentUsername))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotStudent - GetStudentClasses - p_studentUsername passé en paramètre est vide", 0));
            }

            List<Classroom> classes = new List<Classroom>();

            Student_SQLDTO student = this._context.Students.Where(student => student.Active)
                                                           .Include(student => student.Classes.Where(classroom => classroom.Active))
                                                           .FirstOrDefault(student => student.Username == p_studentUsername);

            if (student != null)
            {
                if (student.Classes.Count >= 1)
                {
                    foreach (Classroom_SQLDTO classroom in student.Classes)
                    {
                        classes.Add(classroom.ToEntityWithoutList());
                    }
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Student - Classroom", $"DepotStudent - Method - GetStudentClasses(string p_studentUsername) - Return List<Classroom>"));

                    return classes;
                }
                else
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Student - Classroom", $"DepotStudent - Method - GetStudentClasses(string p_studentUsername) - Return List<Classroom> - student.Classes.Count est vide",0));
                }
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Student - Classroom", $"DepotStudent - Method - GetStudentClasses(string p_studentUsername) - Return List<Classroom> - student est null", 0));
            }

            return new List<Classroom>();
        }

        public void UpsertStudent(Student p_student)
        {
            if (p_student == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotStudent - UpsertStudent - p_student passé en paramètre est null", 0));
            }

            List<Classroom_SQLDTO> classes = new List<Classroom_SQLDTO>();
            VerificatorForDepot verificator = new VerificatorForDepot(this._context);

            if (p_student.Classes.Count >= 1)
            {
                foreach (Classroom classroom in p_student.Classes)
                {
                    classes.Add(new Classroom_SQLDTO(classroom));
                }
            }

            Student_SQLDTO? studentResult = this._context.Students
                .AsNoTracking()
                .SingleOrDefault(student => student.Id == p_student.Id);



            if (studentResult != null)
            {
                if (!studentResult.Active)
                {
                    throw new ArgumentException("Deleted accounts cannot be updated.");
                }

                studentResult.Username = p_student.Username;
                studentResult.FirstName = p_student.FirstName;
                studentResult.LastName = p_student.LastName;
                studentResult.Email = p_student.Email;
                studentResult.Matricule = p_student.Matricule;

                this._context.Update(studentResult);
                this._context.SaveChanges();

                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Student - Classroom", $"DepotStudent - Method - UpsertStudent(Student p_student) - Void - Update Student"));
            }
            else
            {
                if ((studentResult != null && studentResult.Username != p_student.Username &&
                verificator.CheckUsernameTaken(p_student.Username)) ||
                studentResult == null && verificator.CheckUsernameTaken(p_student.Username))
                {
                    throw new ArgumentException("Username already taken.");
                }

                if ((studentResult != null && studentResult.Email != p_student.Email && verificator.CheckEmailTaken(p_student.Email)) ||
                    studentResult == null && verificator.CheckEmailTaken(p_student.Email))
                {
                    throw new ArgumentException("Email already in use.");
                }

                Student_SQLDTO student = new Student_SQLDTO();
                student.Username = p_student.Username;
                student.FirstName = p_student.FirstName;
                student.LastName = p_student.LastName;
                student.Email = p_student.Email;
                student.Classes = classes;
                student.Matricule = p_student.Matricule;
                student.Active = true;

                this._context.Students.Add(student);
                this._context.SaveChanges();

                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Student - Classroom", $"DepotStudent - Method - UpsertStudent(Student p_student) - Void - Add Student"));
            }
        }

        public void DeleteStudent(string p_studentUsername)
        {
            if (string.IsNullOrWhiteSpace(p_studentUsername))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotStudent - DeleteStudent - p_studentUsername passé en paramètre est vide", 0));
            }

            Student_SQLDTO studentResult = this._context.Students.Where(student => student.Active)
                                                                 .FirstOrDefault(student => student.Username == p_studentUsername);
            if (studentResult != null)
            {
                studentResult.Active = false;

                this._context.Update(studentResult);
                this._context.SaveChanges();

                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Student", $"DepotStudent - Method - DeleteStudent(string p_studentUsername) - Void - delete student"));
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Student", $"DepotStudent - Method - DeleteStudent(string p_studentUsername) - Void - studentResult est null",0));
            }
        }

        public void ReactivateStudent(string p_studentUsername)
        {
            if (string.IsNullOrWhiteSpace(p_studentUsername))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotStudent - ReactivateStudent - p_studentUsername passé en paramètre est vide", 0));
            }

            Student_SQLDTO studentResult = this._context.Students.Where(student => !student.Active)
                                                                 .FirstOrDefault(student => student.Username == p_studentUsername);
            if (studentResult != null)
            {
                studentResult.Active = true;

                this._context.Update(studentResult);
                this._context.SaveChanges();

                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Student", $"DepotStudent - Method - ReactivateStudent(string p_studentUsername) - Void - reactive student"));
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Student", $"DepotStudent - Method - ReactivateStudent(string p_studentUsername) - Void - studentResult est null",0));
            }
            
        }
    }
}
