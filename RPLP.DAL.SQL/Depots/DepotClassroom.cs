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
using System.Xml.Linq;

namespace RPLP.DAL.SQL.Depots
{
    public class DepotClassroom : IDepotClassroom
    {
        private readonly RPLPDbContext _context;

        public DepotClassroom(RPLPDbContext context)
        {
            if (context == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotClassroom - DepotClassroom(RPLPDbContext context) - context de type RPLPDbContext passé en paramètre est null",
                    0));
            }

            this._context = context;
        }

        public List<Classroom> GetClassrooms()
        {
            List<Classroom_SQLDTO> classesResult = this._context.Classrooms.Where(classroom => classroom.Active)
                .Include(classroom => classroom.Teachers.Where(teacher => teacher.Active))
                .Include(classroom => classroom.Students.Where(student => student.Active))
                .Include(classroom => classroom.Assignments.Where(assignment => assignment.Active))
                .ToList();

            if (classesResult.Count <= 0)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotClassroom - GetClassrooms() - classesResult.Count <= 0", 0));

                return new List<Classroom>();
            }
            else
            {
                List<Classroom> classes = classesResult.Select(classroom => classroom.ToEntityWithoutList()).ToList();

                if (classes == null)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "DepotClassroom - GetClassrooms - la liste classes assignée à partir de classesResult.Select(classroom => classroom.ToEntityWithoutList()).ToList(); est null",
                        0));
                }

                for (int i = 0; i < classesResult.Count; i++)
                {
                    if (classesResult[i].Id == classes[i].Id)
                    {
                        if (classesResult[i].Students.Count >= 1)
                        {
                            List<Student> students = classesResult[i].Students
                                .Select(student => student.ToEntityWithoutList()).ToList();
                            classes[i].Students = students;
                        }

                        if (classesResult[i].Teachers.Count >= 1)
                        {
                            List<Teacher> teachers = classesResult[i].Teachers
                                .Select(teacher => teacher.ToEntityWithoutList()).ToList();
                            classes[i].Teachers = teachers;
                        }

                        if (classesResult[i].Assignments.Count >= 1)
                        {
                            List<Assignment> assignments = classesResult[i].Assignments
                                .Select(assignments => assignments.ToEntity()).ToList();
                            classes[i].Assignments = assignments;
                        }
                    }
                }

                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Classroom - Student - Teacher - Assignment",
                    $"DepotClassroom - Method - GetClassrooms() - Return List<Classroom>"));

                return classes;
            }
        }

        public Classroom GetClassroomById(int p_id)
        {
            if (p_id < 0)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentOutOfRangeException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotClassroom - GetClassroomById - p_id passé en paramêtre est hors des limites", 0));
            }

            Classroom_SQLDTO classroomResult = this._context.Classrooms
                .Where(classroom => classroom.Id == p_id && classroom.Active)
                .Include(classroom => classroom.Teachers.Where(teacher => teacher.Active))
                .Include(classroom => classroom.Students.Where(student => student.Active))
                .Include(classroom => classroom.Assignments.Where(assignment => assignment.Active))
                .FirstOrDefault();

            if (classroomResult == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Classroom - Student - Teacher - Assignment",
                    $"DepotClassroom - Method - GetClassroomById(int p_id) - Return Classroom - classroomResult est null",
                    0));

                return new Classroom();
            }

            Classroom classroom = classroomResult.ToEntityWithoutList();

            if (classroomResult.Students.Count >= 1)
            {
                List<Student> students =
                    classroomResult.Students.Select(student => student.ToEntityWithoutList()).ToList();
                classroom.Students = students;
            }

            if (classroomResult.Teachers.Count >= 1)
            {
                List<Teacher> teachers =
                    classroomResult.Teachers.Select(teacher => teacher.ToEntityWithoutList()).ToList();
                classroom.Teachers = teachers;
            }

            if (classroomResult.Assignments.Count >= 1)
            {
                List<Assignment> assignments =
                    classroomResult.Assignments.Select(assignment => assignment.ToEntity()).ToList();
                classroom.Assignments = assignments;
            }

            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Classroom - Student - Teacher - Assignment",
                $"DepotClassroom - Method - GetClassroomById(int p_id) - Return Classroom"));

            return classroom;
        }

        public Classroom GetClassroomByName(string p_name)
        {
            if (string.IsNullOrWhiteSpace(p_name))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotClassroom - GetClassroomByName - p_name passé en paramètre est vide", 0));
            }

            Classroom_SQLDTO classroomResult = this._context.Classrooms
                .Where(classroom => classroom.Name == p_name && classroom.Active)
                .Include(classroom => classroom.Teachers.Where(teacher => teacher.Active))
                .Include(classroom => classroom.Students.Where(student => student.Active))
                .Include(classroom => classroom.Assignments.Where(assignment => assignment.Active))
                .FirstOrDefault();

            if (classroomResult == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Classroom - Student - Teacher - Assignment",
                    $"DepotClassroom - Method - GetClassroomByName(string p_name) - Return Classroom - classroomResult est null",
                    0));

                return new Classroom();
            }

            Classroom classroom = classroomResult.ToEntityWithoutList();

            if (classroomResult.Students.Count >= 1)
            {
                List<Student> students =
                    classroomResult.Students.Select(student => student.ToEntityWithoutList()).ToList();
                classroom.Students = students;
            }

            if (classroomResult.Teachers.Count >= 1)
            {
                List<Teacher> teachers =
                    classroomResult.Teachers.Select(teacher => teacher.ToEntityWithoutList()).ToList();
                classroom.Teachers = teachers;
            }

            if (classroomResult.Assignments.Count >= 1)
            {
                List<Assignment> assignments =
                    classroomResult.Assignments.Select(assignment => assignment.ToEntity()).ToList();
                classroom.Assignments = assignments;
            }

            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Classroom - Student - Teacher - Assignment",
                $"DepotClassroom - Method - GetClassroomByName(string p_name) - Return Classroom"));

            return classroom;
        }

        public List<Assignment> GetAssignmentsByClassroomName(string p_classroomName)
        {
            if (string.IsNullOrWhiteSpace(p_classroomName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotClassroom - GetAssignmentsByClassroomName - p_classroomName passé en paramètre est vide", 0));
            }

            Classroom_SQLDTO classroomResult = this._context.Classrooms.Where(classroom => classroom.Active)
                .Include(classroom => classroom.Assignments.Where(assignment => assignment.Active))
                .FirstOrDefault(classroom => classroom.Name == p_classroomName);

            if (classroomResult != null && classroomResult.Assignments.Count >= 1)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Classroom - Assignment",
                    $"DepotClassroom - Method - GetAssignmentsByClassroomName(string p_classroomName) - Return List<Assignment> Count : {classroomResult.Assignments.Count}"));
                return classroomResult.Assignments.Select(assignment => assignment.ToEntity()).ToList();
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Classroom - Assignment",
                    $"DepotClassroom - Method - GetAssignmentsByClassroomName(string p_classroomName) - Return List<Assignment> - classroomResult == null || classroomResult.Assignments.Count < 0",
                    0));
            }

            return new List<Assignment>();
        }

        public List<Student> GetStudentsByClassroomName(string p_classroomName)
        {
            if (string.IsNullOrWhiteSpace(p_classroomName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotClassroom - GetStudentsByClassroomName - p_classroomName passé en paramètre est vide", 0));
            }

            Classroom_SQLDTO classroomResult = this._context.Classrooms.Where(classroom => classroom.Active)
                .Include(classroom => classroom.Students.Where(student => student.Active))
                .FirstOrDefault(classroom => classroom.Name == p_classroomName);

            if (classroomResult != null && classroomResult.Students.Count >= 1)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Classroom - Student",
                    $"DepotClassroom - Method - GetStudentsByClassroomName(string p_classroomName) - Return List<Student> Count : {classroomResult.Students.Count}"));
                return classroomResult.Students.Select(student => student.ToEntityWithoutList()).ToList();
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Classroom - Student",
                    $"DepotClassroom - Method - GetStudentsByClassroomName(string p_classroomName) - Return List<Student> - classroomResult == null || classroomResult.Students.Count < 0",
                    0));
            }

            return new List<Student>();
        }

        public List<Teacher> GetTeachersByClassroomName(string p_classroomName)
        {
            if (string.IsNullOrWhiteSpace(p_classroomName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotClassroom - GetTeachersByClassroomName - p_classroomName passé en paramètre est vide", 0));
            }

            Classroom_SQLDTO classroomResult = this._context.Classrooms.Where(classroom => classroom.Active)
                .Include(classroom => classroom.Teachers.Where(teachers => teachers.Active))
                .FirstOrDefault(classroom => classroom.Name == p_classroomName);

            if (classroomResult != null && classroomResult.Teachers.Count >= 1)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Classroom - Teacher",
                    $"DepotClassroom - Method - GetTeachersByClassroomName(string p_classroomName) - Return List<Teacher> - classroomResult != null && classroomResult.Teachers.Count >= 1"));

                return classroomResult.Teachers.Select(teacher => teacher.ToEntityWithoutList()).ToList();
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Classroom - Teacher",
                    $"DepotClassroom - Method - GetTeachersByClassroomName(string p_classroomName) - Return List<Teacher> - classroomResult == null && classroomResult.Teachers.Count < 0",
                    0));
            }

            return new List<Teacher>();
        }

        public void AddAssignmentToClassroom(string p_classroomName, string p_assignmentName)
        {
            if (string.IsNullOrWhiteSpace(p_classroomName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotClassroom - AddAssignmentToClassroom - p_classroomName passé en paramètre est vide", 0));
            }

            if (string.IsNullOrWhiteSpace(p_assignmentName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotClassroom - AddAssignmentToClassroom - p_assignmentName passé en paramètre est vide", 0));
            }

            Classroom_SQLDTO classroomResult = this._context.Classrooms.Where(classroom => classroom.Active)
                .Include(classroom => classroom.Assignments.Where(assignment => assignment.Active))
                .FirstOrDefault(classroom => classroom.Name == p_classroomName);
            if (classroomResult != null)
            {
                Assignment_SQLDTO assignmentResult = this._context.Assignments.Where(assignment => assignment.Active)
                    .SingleOrDefault(assignment => assignment.Name == p_assignmentName &&
                                                   assignment.ClassroomName == p_classroomName);

                if (assignmentResult != null && !classroomResult.Assignments.Contains(assignmentResult))
                {
                    classroomResult.Assignments.Add(assignmentResult);

                    this._context.Update(classroomResult);
                    this._context.SaveChanges();

                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Classroom - Assignments",
                        $"DepotClassroom - Method - AddAssignmentToClassroom(string p_classroomName, string p_assignmentName) - Void - add Assignment to classroom"));
                }
                else
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "DepotClassroom - AddAssignmentToClassroom(string p_classroomName, string p_assignmentName) - assignmentResult != null && !classroomResult.Assignments.Contains(assignmentResult) pas entree dans ce if",
                        0));
                }
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Classroom - Assignments",
                    $"DepotClassroom - Method - AddAssignmentToClassroom(string p_classroomName, string p_assignmentName) - Void - classroomResult est null",
                    0));
            }
        }

        public void AddStudentToClassroom(string p_classroomName, string p_studentUsername)
        {
            if (string.IsNullOrWhiteSpace(p_classroomName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotClassroom - AddStudentToClassroom - p_classroomName passé en paramètre est vide", 0));
            }

            if (string.IsNullOrWhiteSpace(p_studentUsername))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotClassroom - AddStudentToClassroom - p_studentUsername passé en paramètre est vide", 0));
            }

            Classroom_SQLDTO classroomResult = this._context.Classrooms
                .Include(classroom => classroom.Students.Where(student => student.Active))
                .SingleOrDefault(classroom => classroom.Name == p_classroomName && classroom.Active);
            if (classroomResult != null)
            {
                Student_SQLDTO studentResult = this._context.Students.Where(student => student.Active)
                    .SingleOrDefault(student => student.Username == p_studentUsername);

                if (studentResult != null && !classroomResult.Students.Contains(studentResult))
                {
                    this._context.ChangeTracker.Clear();
                    this._context.Attach(classroomResult);
                    this._context.Entry(classroomResult).Collection(x => x.Students).Load();

                    classroomResult.Students.Add(studentResult);
                    this._context.SaveChanges();

                    //this._context.Database.ExecuteSqlRaw($"INSERT INTO Classroom_SQLDTOStudent_SQLDTO (ClassesId, StudentsId) VALUES({classroomResult.Id},{studentResult.Id});");

                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Classroom - Student",
                        $"DepotClassroom - Method - AddStudentToClassroom(string p_classroomName, string p_studentUsername) - Void - add student classroom"));
                }
                else
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "DepotClassroom - AddStudentToClassroom(string p_classroomName, string p_studentUsername) - studentResult != null && !classroomResult.Students.Contains(studentResult) pas entree dans ce if",
                        0));
                }
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Classroom - Student",
                    $"DepotClassroom - Method - AddStudentToClassroom(string p_classroomName, string p_studentUsername) - Void - classroomResult est null",
                    0));
            }
        }

        public void AddStudentToClassroomMatricule(string p_classroomName, string p_studentMatricule)
        {
            if (string.IsNullOrWhiteSpace(p_classroomName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotClassroom - AddStudentToClassroomMatricule - p_classroomName passé en paramètre est vide",
                    0));
            }

            if (string.IsNullOrWhiteSpace(p_studentMatricule))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotClassroom - AddStudentToClassroomMatricule - p_studentUsername passé en paramètre est vide",
                    0));
            }

            Classroom_SQLDTO classroomResult = this._context.Classrooms
                .Include(classroom => classroom.Students.Where(student => student.Active))
                .FirstOrDefault(classroom => classroom.Name == p_classroomName && classroom.Active);
            if (classroomResult != null)
            {
                Student_SQLDTO studentResult = this._context.Students.Where(student => student.Active)
                    .SingleOrDefault(student => student.Matricule == p_studentMatricule);

                if (studentResult != null && !classroomResult.Students.Contains(studentResult))
                {
                    this._context.ChangeTracker.Clear();
                    this._context.Attach(classroomResult);
                    this._context.Entry(classroomResult).Collection(x => x.Students).Load();

                    classroomResult.Students.Add(studentResult);
                    this._context.SaveChanges();

                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Classroom - Student",
                        $"DepotClassroom - Method - AddStudentToClassroomMatricule(string p_classroomName, string p_studentMatricule) - Void - add student classroom avec matricule"));
                }
                else
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "DepotClassroom - AddStudentToClassroomMatricule(string p_classroomName, string p_studentMatricule) - studentResult != null && !classroomResult.Students.Contains(studentResult) pas entree dans ce if",
                        0));
                }
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Classroom - Student",
                    $"DepotClassroom - Method - AddStudentToClassroomMatricule(string p_classroomName, string p_studentMatricule) - Void - classroomResult est null",
                    0));
            }
        }

        public void AddTeacherToClassroom(string p_classroomName, string p_teacherUsername)
        {
            if (this._context.ChangeTracker != null)
            {
                this._context.ChangeTracker.Clear();
            }

            if (string.IsNullOrWhiteSpace(p_classroomName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotClassroom - AddTeacherToClassroom - p_classroomName passé en paramètre est vide", 0));
            }

            if (string.IsNullOrWhiteSpace(p_teacherUsername))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotClassroom - AddTeacherToClassroom - p_teacherUsername passé en paramètre est vide", 0));
            }

            Classroom_SQLDTO classroomResult = this._context.Classrooms.Where(classroom => classroom.Active)
                .Include(classroom => classroom.Teachers.Where(teacher => teacher.Active))
                .SingleOrDefault(classroom => classroom.Name == p_classroomName);
            if (classroomResult != null)
            {
                Teacher_SQLDTO teacherResult = this._context.Teachers.Where(teacher => teacher.Active)
                    .SingleOrDefault(teacher => teacher.Username == p_teacherUsername);

                if (teacherResult != null && !classroomResult.Teachers.Contains(teacherResult))
                {
                    this._context.ChangeTracker.Clear();
                    this._context.Attach(classroomResult);
                    this._context.Entry(classroomResult).Collection(x => x.Teachers).Load();

                    classroomResult.Teachers.Add(teacherResult);
                    this._context.SaveChanges();

                    //this._context.Database.ExecuteSqlRaw($"INSERT INTO Classroom_SQLDTOTeacher_SQLDTO (ClassesId, TeachersId) VALUES({classroomResult.Id},{teacherResult.Id});");

                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Classroom - Teacher",
                        $"DepotClassroom - Method - AddTeacherToClassroom(string p_classroomName, string p_teacherUsername) - Void - add teacher classroom"));
                }
                else
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "DepotClassroom - AddTeacherToClassroom(string p_classroomName, string p_teacherUsername) - teacherResult != null && !classroomResult.Teachers.Contains(teacherResult) pas entree dans ce if",
                        0));
                }
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Classroom - Teacher",
                    $"DepotClassroom - Method - AddTeacherToClassroom(string p_classroomName, string p_teacherUsername) - Void - classroomResult est null",
                    0));
            }
        }

        public void RemoveAssignmentFromClassroom(string p_classroomName, string p_assignmentName)
        {
            if (string.IsNullOrWhiteSpace(p_classroomName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotClassroom - RemoveAssignmentFromClassroom - p_classroomName passé en paramètre est vide", 0));
            }

            if (string.IsNullOrWhiteSpace(p_assignmentName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotClassroom - RemoveAssignmentFromClassroom - p_assignmentName passé en paramètre est vide",
                    0));
            }

            Classroom_SQLDTO classroomResult = this._context.Classrooms.Where(classroom => classroom.Active)
                .Include(classroom => classroom.Assignments.Where(assignment => assignment.Active))
                .FirstOrDefault(classroom => classroom.Name == p_classroomName);
            if (classroomResult != null)
            {
                Assignment_SQLDTO assignmentResult = this._context.Assignments.Where(assignment => assignment.Active)
                    .SingleOrDefault(assignment => assignment.Name == p_assignmentName &&
                                                   assignment.ClassroomName == p_classroomName);

                if (assignmentResult != null && classroomResult.Assignments.Contains(assignmentResult))
                {
                    classroomResult.Assignments.Remove(assignmentResult);

                    this._context.Update(classroomResult);
                    this._context.SaveChanges();

                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Classroom - Assignment",
                        $"DepotClassroom - Method - RemoveAssignmentFromClassroom(string p_classroomName, string p_assignmentName) - Void - remove assigment classroom"));
                }
                else
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "DepotClassroom - RemoveAssignmentFromClassroom(string p_classroomName, string p_assignmentName) - assignmentResult != null && classroomResult.Assignments.Contains(assignmentResult) pas entree dans ce if",
                        0));
                }
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Classroom - Assignment",
                    $"DepotClassroom - Method - RemoveAssignmentFromClassroom(string p_classroomName, string p_assignmentName) - Void - classroomResult est null",
                    0));
            }
        }

        public void RemoveStudentFromClassroom(string p_classroomName, string p_username)
        {
            if (string.IsNullOrWhiteSpace(p_classroomName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotClassroom - RemoveStudentFromClassroom - p_classroomName passé en paramètre est vide", 0));
            }

            if (string.IsNullOrWhiteSpace(p_username))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotClassroom - RemoveStudentFromClassroom - p_username passé en paramètre est vide", 0));
            }

            Classroom_SQLDTO classroomResult = this._context.Classrooms
                .Include(classroom => classroom.Students.Where(student => student.Active))
                .SingleOrDefault(classroom => classroom.Name == p_classroomName && classroom.Active);
            if (classroomResult != null)
            {
                Student_SQLDTO studentResult = this._context.Students
                    .SingleOrDefault(student => student.Username == p_username && student.Active);

                if (studentResult is not null &&
                    classroomResult.Students.SingleOrDefault(s => s.Id == studentResult.Id) != null)
                {
                    int index = classroomResult.Students.IndexOf(
                        classroomResult.Students.FirstOrDefault(t => t.Id == studentResult.Id));
                    if (this._context.ChangeTracker != null)
                    {
                        this._context.ChangeTracker.Clear();
                        this._context.Attach(classroomResult);
                        this._context.Entry(classroomResult).Collection(x => x.Students).Load();
                    }

                    classroomResult.Students.RemoveAt(index);
                    this._context.SaveChanges();

                    //this._context.Database.ExecuteSqlRaw($"DELETE FROM Classroom_SQLDTOStudent_SQLDTO WHERE ClassesId={classroomResult.Id} AND StudentsId={studentResult.Id};");

                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Classroom - Student",
                        $"DepotClassroom - Method - RemoveStudentFromClassroom(string p_classroomName, string p_username) - Void - remove student classroom"));
                }
                else
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "DepotClassroom - RemoveStudentFromClassroom(string p_classroomName, string p_username) - classroomResult.Students.SingleOrDefault(s => s.Id == studentResult.Id) != null pas entree dans le if",
                        0));
                }
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Classroom - Teacher",
                    $"DepotClassroom - Method - RemoveStudentFromClassroom(string p_classroomName, string p_username) - Void - classroomResult est null",
                    0));
            }
        }

        public void RemoveTeacherFromClassroom(string p_classroomName, string p_username)
        {
            if (this._context.ChangeTracker != null)
            {
                this._context.ChangeTracker.Clear();
            }

            if (string.IsNullOrWhiteSpace(p_classroomName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotClassroom - RemoveTeacherFromClassroom - p_classroomName passé en paramètre est vide", 0));
            }

            if (string.IsNullOrWhiteSpace(p_username))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotClassroom - RemoveTeacherFromClassroom - p_username passé en paramètre est vide", 0));
            }

            Classroom_SQLDTO classroomResult = this._context.Classrooms
                .Include(classroom => classroom.Teachers.Where(teacher => teacher.Active))
                .SingleOrDefault(classroom => classroom.Name == p_classroomName && classroom.Active);
            if (classroomResult != null)
            {
                Teacher_SQLDTO teacherResult = this._context.Teachers
                    .SingleOrDefault(teacher => teacher.Username == p_username && teacher.Active);

                if (teacherResult is not null &&
                    classroomResult.Teachers.SingleOrDefault(t => t.Id == teacherResult.Id) != null)
                {
                    int index = classroomResult.Teachers.IndexOf(
                        classroomResult.Teachers.FirstOrDefault(t => t.Id == teacherResult.Id));
                    if (this._context.ChangeTracker != null)
                    {
                        this._context.ChangeTracker.Clear();
                        this._context.Attach(classroomResult);
                        this._context.Entry(classroomResult).Collection(x => x.Teachers).Load();
                    }

                    classroomResult.Teachers.RemoveAt(index);
                    this._context.SaveChanges();

                    //this._context.Database.ExecuteSqlRaw($"DELETE FROM Classroom_SQLDTOTeacher_SQLDTO WHERE ClassesId={classroomResult.Id} AND TeachersId={teacherResult.Id};");

                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Classroom - Teacher",
                        $"DepotClassroom - Method - RemoveTeacherFromClassroom(string p_classroomName, string p_username) - Void - remove teacher classroom"));
                }
                else
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "DepotClassroom - RemoveTeacherFromClassroom(string p_classroomName, string p_username) - teacherResult is not null && classroomResult.Teachers.SingleOrDefault(t => t.Id == teacherResult.Id) != null pas entree dans le if",
                        0));
                }
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Classroom - Teacher",
                    $"DepotClassroom - Method - RemoveTeacherFromClassroom(string p_classroomName, string p_teacherUsername) - Void - classroomResult est null",
                    0));
            }
        }

        public void UpsertClassroom(Classroom p_classroom)
        {
            if (this._context.ChangeTracker != null)
            {
                this._context.ChangeTracker.Clear();
            }

            if (p_classroom == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotClassroom - UpsertClassroom - p_classroom passé en paramètre est null", 0));
            }

            List<Student_SQLDTO> students = new List<Student_SQLDTO>();
            List<Teacher_SQLDTO> teachers = new List<Teacher_SQLDTO>();
            List<Assignment_SQLDTO> assignments = new List<Assignment_SQLDTO>();

            if (p_classroom.Students == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotClassroom - UpsertClassroom - p_classroom.Students passé en paramètre est null", 0));
            }

            if (p_classroom.Teachers == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotClassroom - UpsertClassroom - p_classroom.Teachers passé en paramètre est null", 0));
            }

            if (p_classroom.Assignments == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotClassroom - UpsertClassroom - p_classroom.Assignments passé en paramètre est null", 0));
            }

            if (p_classroom.Students.Count >= 1)
            {
                foreach (Student student in p_classroom.Students)
                {
                    students.Add(new Student_SQLDTO(student));
                }
            }

            if (p_classroom.Teachers.Count >= 1)
            {
                foreach (Teacher teacher in p_classroom.Teachers)
                {
                    teachers.Add(new Teacher_SQLDTO(teacher));
                }
            }

            if (p_classroom.Assignments.Count >= 1)
            {
                foreach (Assignment assignment in p_classroom.Assignments)
                {
                    assignments.Add(new Assignment_SQLDTO(assignment));
                }
            }

            Classroom_SQLDTO classroomResult = this._context.Classrooms.Where(classroom => classroom.Active)
                .FirstOrDefault(classroom => classroom.Id == p_classroom.Id);

            if (classroomResult != null)
            {
                classroomResult.Name = p_classroom.Name;
                classroomResult.OrganisationName = p_classroom.OrganisationName;

                this._context.Update(classroomResult);
                this._context.SaveChanges();

                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Classroom - Student - Teacher - Assignment",
                    $"DepotClassroom - Method - UpsertClassroom(Classroom p_classroom) - Void - Update Classroom"));
            }
            else
            {
                Classroom_SQLDTO classDTO = new Classroom_SQLDTO();
                classDTO.Name = p_classroom.Name;
                classDTO.OrganisationName = p_classroom.OrganisationName;
                classDTO.Students = students;
                classDTO.Teachers = teachers;
                classDTO.Assignments = assignments;
                classDTO.Active = true;

                this._context.Classrooms.Add(classDTO);
                this._context.SaveChanges();

                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Classroom - Student - Teacher - Assignment",
                    $"DepotClassroom - Method - UpsertClassroom(Classroom p_classroom) - Void - Add Classroom"));
            }
        }

        public void DeleteClassroom(string p_classroomName)
        {
            if (string.IsNullOrWhiteSpace(p_classroomName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotClassroom - DeleteClassroom - p_classroomName passé en paramètre est vide", 0));
            }

            Classroom_SQLDTO classroomResult = this._context.Classrooms.Where(classroom => classroom.Active)
                .SingleOrDefault(classroom => classroom.Name == p_classroomName);
            if (classroomResult != null)
            {
                classroomResult.Active = false;

                this._context.Update(classroomResult);
                this._context.SaveChanges();

                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Classroom",
                    $"DepotClassroom - Method - DeleteClassroom(string p_classroomName) - Void - delete classroom"));
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotClassroom - DeleteClassroom(string p_classroomName) - classroomResult est null", 0));
            }
        }

        public List<Classroom> GetClassroomsByOrganisationName(string p_organisationName)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotClassroom - GetClassroomsByOrganisationName - p_organisationName passé en paramètre est vide",
                    0));
            }

            List<Classroom_SQLDTO> classesResult = this._context.Classrooms
                .Where(classroom => classroom.Active && classroom.OrganisationName == p_organisationName).ToList();

            if (classesResult.Count <= 0)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotClassroom - DeleteClassroom(string p_classroomName) - classesResult.Count <= 0", 0));
                return new List<Classroom>();
            }
            else
            {
                List<Classroom> classes =
                    classesResult.Select(classroom => classroom.ToEntityWithoutList()).ToList();
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Classroom",
                    $"DepotClassroom - Method - GetClassroomsByOrganisationName(string p_organisationName) - Return List<Classroom>"));

                return classes;
            }
        }
    }
}