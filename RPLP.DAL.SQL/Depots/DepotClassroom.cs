using Microsoft.EntityFrameworkCore;
using RPLP.DAL.DTO.Sql;
using RPLP.ENTITES;
using RPLP.JOURNALISATION;
using RPLP.SERVICES.InterfacesDepots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.DAL.SQL.Depots
{
    public class DepotClassroom : IDepotClassroom
    {
        private readonly RPLPDbContext _context;

        public DepotClassroom()
        {
            this._context = new RPLPDbContext(new DbContextOptionsBuilder<RPLPDbContext>().UseSqlServer("Server=rplp.db; Database=RPLP; User Id=sa; password=Cad3pend86!").Options);
            //this._context = new RPLPDbContext(new DbContextOptionsBuilder<RPLPDbContext>().UseSqlServer("Server=localhost,1433; Database=RPLP; User Id=sa; password=Cad3pend86!").Options);
        }

        public DepotClassroom(RPLPDbContext context)
        {
            this._context = context;
        }

        public List<Classroom> GetClassrooms()
        {
            List<Classroom_SQLDTO> classesResult = this._context.Classrooms.Where(classroom => classroom.Active)
                                                                       .Include(classroom => classroom.Teachers.Where(teacher => teacher.Active))
                                                                       .Include(classroom => classroom.Students.Where(student => student.Active))
                                                                       .Include(classroom => classroom.Assignments.Where(assignment => assignment.Active))
                                                                       .ToList();

            RPLP.JOURNALISATION.Journalisation.Journaliser(new Log("Classroom - Student - Teacher - Assignment", $"DepotClassroom - Method - GetClassrooms() - Return List<Classroom>"));

            if (classesResult.Count <= 0)
                return new List<Classroom>();
            else
            {
                List<Classroom> classes = classesResult.Select(classroom => classroom.ToEntityWithoutList()).ToList();

                for (int i = 0; i < classesResult.Count; i++)
                {
                    if (classesResult[i].Id == classes[i].Id)
                    {
                        if (classesResult[i].Students.Count >= 1)
                        {
                            List<Student> students = classesResult[i].Students.Select(student => student.ToEntityWithoutList()).ToList();
                            classes[i].Students = students;
                        }

                        if (classesResult[i].Teachers.Count >= 1)
                        {
                            List<Teacher> teachers = classesResult[i].Teachers.Select(teacher => teacher.ToEntityWithoutList()).ToList();
                            classes[i].Teachers = teachers;
                        }

                        if (classesResult[i].Assignments.Count >= 1)
                        {
                            List<Assignment> assignments = classesResult[i].Assignments.Select(assignments => assignments.ToEntity()).ToList();
                            classes[i].Assignments = assignments;
                        }
                    }
                }

                return classes;
            }
        }

        public Classroom GetClassroomById(int p_id)
        {
            Classroom_SQLDTO classroomResult = this._context.Classrooms.Where(classroom => classroom.Id == p_id && classroom.Active)
                                                                       .Include(classroom => classroom.Teachers.Where(teacher => teacher.Active))
                                                                       .Include(classroom => classroom.Students.Where(student => student.Active))
                                                                       .Include(classroom => classroom.Assignments.Where(assignment => assignment.Active))
                                                                       .FirstOrDefault();

            RPLP.JOURNALISATION.Journalisation.Journaliser(new Log("Classroom - Student - Teacher - Assignment", $"DepotClassroom - Method - GetClassroomById(int p_id) - Return Classroom"));

            if (classroomResult == null)
                return new Classroom();

            Classroom classroom = classroomResult.ToEntityWithoutList();

            if (classroomResult.Students.Count >= 1)
            {
                List<Student> students = classroomResult.Students.Select(student => student.ToEntityWithoutList()).ToList();
                classroom.Students = students;
            }

            if (classroomResult.Teachers.Count >= 1)
            {
                List<Teacher> teachers = classroomResult.Teachers.Select(teacher => teacher.ToEntityWithoutList()).ToList();
                classroom.Teachers = teachers;
            }

            if (classroomResult.Assignments.Count >= 1)
            {
                List<Assignment> assignments = classroomResult.Assignments.Select(assignment => assignment.ToEntity()).ToList();
                classroom.Assignments = assignments;
            }

            return classroom;
        }

        public Classroom GetClassroomByName(string p_name)
        {
            Classroom_SQLDTO classroomResult = this._context.Classrooms.Where(classroom => classroom.Name == p_name && classroom.Active)
                                                                       .Include(classroom => classroom.Teachers.Where(teacher => teacher.Active))
                                                                       .Include(classroom => classroom.Students.Where(student => student.Active))
                                                                       .Include(classroom => classroom.Assignments.Where(assignment => assignment.Active))
                                                                       .FirstOrDefault();

            RPLP.JOURNALISATION.Journalisation.Journaliser(new Log("Classroom - Student - Teacher - Assignment", $"DepotClassroom - Method - GetClassroomByName(string p_name) - Return Classroom"));

            if (classroomResult == null)
                return new Classroom();

            Classroom classroom = classroomResult.ToEntityWithoutList();

            if (classroomResult.Students.Count >= 1)
            {
                List<Student> students = classroomResult.Students.Select(student => student.ToEntityWithoutList()).ToList();
                classroom.Students = students;
            }

            if (classroomResult.Teachers.Count >= 1)
            {
                List<Teacher> teachers = classroomResult.Teachers.Select(teacher => teacher.ToEntityWithoutList()).ToList();
                classroom.Teachers = teachers;
            }

            if (classroomResult.Assignments.Count >= 1)
            {
                List<Assignment> assignments = classroomResult.Assignments.Select(assignment => assignment.ToEntity()).ToList();
                classroom.Assignments = assignments;
            }

            return classroom;
        }

        public List<Assignment> GetAssignmentsByClassroomName(string p_classroomName)
        {
            Classroom_SQLDTO classroomResult = this._context.Classrooms.Where(classroom => classroom.Active)
                                                                       .Include(classroom => classroom.Assignments.Where(assignment => assignment.Active))
                                                                       .FirstOrDefault(classroom => classroom.Name == p_classroomName);

            RPLP.JOURNALISATION.Journalisation.Journaliser(new Log("Classroom - Assignment", $"DepotClassroom - Method - GetAssignmentsByClassroomName(string p_classroomName) - Return List<Assignment>"));

            if (classroomResult != null && classroomResult.Assignments.Count >= 1)
                return classroomResult.Assignments.Select(assignment => assignment.ToEntity()).ToList();

            return new List<Assignment>();
        }

        public List<Student> GetStudentsByClassroomName(string p_classroomName)
        {
            Classroom_SQLDTO classroomResult = this._context.Classrooms.Where(classroom => classroom.Active)
                                                                       .Include(classroom => classroom.Students.Where(student => student.Active))
                                                                       .FirstOrDefault(classroom => classroom.Name == p_classroomName);

            RPLP.JOURNALISATION.Journalisation.Journaliser(new Log("Classroom - Student", $"DepotClassroom - Method - GetStudentsByClassroomName(string p_classroomName) - Return List<Student>"));

            if (classroomResult != null && classroomResult.Students.Count >= 1)
                return classroomResult.Students.Select(student => student.ToEntityWithoutList()).ToList();

            return new List<Student>();
        }

        public List<Teacher> GetTeachersByClassroomName(string p_classroomName)
        {
            Classroom_SQLDTO classroomResult = this._context.Classrooms.Where(classroom => classroom.Active)
                                                                       .Include(classroom => classroom.Teachers.Where(teachers => teachers.Active))
                                                                       .FirstOrDefault(classroom => classroom.Name == p_classroomName);

            RPLP.JOURNALISATION.Journalisation.Journaliser(new Log("Classroom - Teacher", $"DepotClassroom - Method - GetTeachersByClassroomName(string p_classroomName) - Return List<Teacher>"));

            if (classroomResult != null && classroomResult.Teachers.Count >= 1)
                return classroomResult.Teachers.Select(teacher => teacher.ToEntityWithoutList()).ToList();

            return new List<Teacher>();
        }

        public void AddAssignmentToClassroom(string p_classroomName, string p_assignmentName)
        {
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
                }
            }

            RPLP.JOURNALISATION.Journalisation.Journaliser(new Log("Classroom - Assignments", $"DepotClassroom - Method - AddAssignmentToClassroom(string p_classroomName, string p_assignmentName) - Void"));
        }

        public void AddStudentToClassroom(string p_classroomName, string p_studentUsername)
        {
            Classroom_SQLDTO classroomResult = this._context.Classrooms.Include(classroom => classroom.Students.Where(student => student.Active))
                                                                       .FirstOrDefault(classroom => classroom.Name == p_classroomName && classroom.Active);
            if (classroomResult != null)
            {
                Student_SQLDTO studentResult = this._context.Students.Where(student => student.Active)
                                                                     .SingleOrDefault(student => student.Username == p_studentUsername);

                if (studentResult != null && !classroomResult.Students.Contains(studentResult))
                {
                    classroomResult.Students.Add(studentResult);

                    this._context.Update(classroomResult);
                    this._context.SaveChanges();
                }
            }

            RPLP.JOURNALISATION.Journalisation.Journaliser(new Log("Classroom - Student", $"DepotClassroom - Method - AddStudentToClassroom(string p_classroomName, string p_studentUsername) - Void"));
        }

        public void AddStudentToClassroomMatricule(string p_classroomName, string p_studentMatricule)
        {
            Classroom_SQLDTO classroomResult = this._context.Classrooms.Include(classroom => classroom.Students.Where(student => student.Active))
                                                                       .FirstOrDefault(classroom => classroom.Name == p_classroomName && classroom.Active);
            if (classroomResult != null)
            {
                Student_SQLDTO studentResult = this._context.Students.Where(student => student.Active)
                                                                     .SingleOrDefault(student => student.Matricule == p_studentMatricule);

                if (studentResult != null && !classroomResult.Students.Contains(studentResult))
                {
                    classroomResult.Students.Add(studentResult);

                    this._context.Update(classroomResult);
                    this._context.SaveChanges();
                }
            }

            RPLP.JOURNALISATION.Journalisation.Journaliser(new Log("Classroom - Student", $"DepotClassroom - Method - AddStudentToClassroomMatricule(string p_classroomName, string p_studentMatricule) - Void"));
        }

        public void AddTeacherToClassroom(string p_classroomName, string p_teacherUsername)
        {
            Classroom_SQLDTO classroomResult = this._context.Classrooms.Where(classroom => classroom.Active)
                                                                       .Include(classroom => classroom.Teachers.Where(teacher => teacher.Active))
                                                                       .FirstOrDefault(classroom => classroom.Name == p_classroomName);
            if (classroomResult != null)
            {
                Teacher_SQLDTO teacherResult = this._context.Teachers.Where(teacher => teacher.Active)
                                                                     .SingleOrDefault(teacher => teacher.Username == p_teacherUsername);

                if (teacherResult != null && !classroomResult.Teachers.Contains(teacherResult))
                {
                    classroomResult.Teachers.Add(teacherResult);

                    this._context.Update(classroomResult);
                    this._context.SaveChanges();
                }
            }

            RPLP.JOURNALISATION.Journalisation.Journaliser(new Log("Classroom - Teacher", $"DepotClassroom - Method - AddTeacherToClassroom(string p_classroomName, string p_teacherUsername) - Void"));
        }

        public void RemoveAssignmentFromClassroom(string p_classroomName, string p_assignmentName)
        {
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
                }
            }

            RPLP.JOURNALISATION.Journalisation.Journaliser(new Log("Classroom - Assignment", $"DepotClassroom - Method - RemoveAssignmentFromClassroom(string p_classroomName, string p_assignmentName) - Void"));
        }

        public void RemoveStudentFromClassroom(string p_classroomName, string p_username)
        {
            Classroom_SQLDTO classroomResult = this._context.Classrooms.Include(classroom => classroom.Students.Where(student => student.Active))
                                                                       .FirstOrDefault(classroom => classroom.Name == p_classroomName && classroom.Active);

            Student_SQLDTO studentResult = this._context.Students
                .SingleOrDefault(student => student.Username == p_username && student.Active);

            if (classroomResult.Students.SingleOrDefault(s => s.Id == studentResult.Id) != null)
            {
                classroomResult.Students.Remove(studentResult);

                this._context.Update(classroomResult);
                this._context.SaveChanges();
            }

            RPLP.JOURNALISATION.Journalisation.Journaliser(new Log("Classroom - Student", $"DepotClassroom - Method - RemoveStudentFromClassroom(string p_classroomName, string p_username) - Void"));
        }

        public void RemoveTeacherFromClassroom(string p_classroomName, string p_username)
        {
            Classroom_SQLDTO classroomResult = this._context.Classrooms.Include(classroom => classroom.Teachers.Where(teacher => teacher.Active))
                                                                       .FirstOrDefault(classroom => classroom.Name == p_classroomName && classroom.Active);

            Teacher_SQLDTO teacherResult = this._context.Teachers
                .SingleOrDefault(teacher => teacher.Username == p_username && teacher.Active);

            if (classroomResult.Teachers.SingleOrDefault(t => t.Id == teacherResult.Id) != null)
            {
                classroomResult.Teachers.Remove(teacherResult);

                this._context.Update(classroomResult);
                this._context.SaveChanges();
            }

            RPLP.JOURNALISATION.Journalisation.Journaliser(new Log("Classroom - Teacher", $"DepotClassroom - Method - RemoveTeacherFromClassroom(string p_classroomName, string p_username) - Void"));
        }

        public void UpsertClassroom(Classroom p_classroom)
        {
            List<Student_SQLDTO> students = new List<Student_SQLDTO>();
            List<Teacher_SQLDTO> teachers = new List<Teacher_SQLDTO>();
            List<Assignment_SQLDTO> assignments = new List<Assignment_SQLDTO>();

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
            }

            RPLP.JOURNALISATION.Journalisation.Journaliser(new Log("Classroom - Student - Teacher - Assignment", $"DepotClassroom - Method - UpsertClassroom(Classroom p_classroom) - Void"));
        }

        public void DeleteClassroom(string p_classroomName)
        {
            Classroom_SQLDTO classroomResult = this._context.Classrooms.Where(classroom => classroom.Active)
                                                                       .SingleOrDefault(classroom => classroom.Name == p_classroomName);
            if (classroomResult != null)
            {
                classroomResult.Active = false;

                this._context.Update(classroomResult);
                this._context.SaveChanges();
            }

            RPLP.JOURNALISATION.Journalisation.Journaliser(new Log("Classroom", $"DepotClassroom - Method - DeleteClassroom(string p_classroomName) - Void"));
        }

        public List<Classroom> GetClassroomsByOrganisationName(string p_organisationName)
        {
            List<Classroom_SQLDTO> classesResult = this._context.Classrooms.Where(classroom => classroom.Active && classroom.OrganisationName == p_organisationName).ToList();

            RPLP.JOURNALISATION.Journalisation.Journaliser(new Log("Classroom", $"DepotClassroom - Method - GetClassroomsByOrganisationName(string p_organisationName) - Return List<Classroom>"));

            if (classesResult.Count <= 0)
                return new List<Classroom>();
            else
            {
                List<Classroom> classes = classesResult.Select(classroom => classroom.ToEntityWithoutList()).ToList();

                return classes;
            }
        }
    }
}
