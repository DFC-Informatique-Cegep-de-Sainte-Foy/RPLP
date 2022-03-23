using Microsoft.EntityFrameworkCore;
using RPLP.DAL.DTO.Sql;
using RPLP.ENTITES;
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
            this._context = new RPLPDbContext();
        }

        public List<Classroom> GetClassrooms()
        {
            List<Classroom_SQLDTO> classesResult = this._context.Classrooms.Where(classroom => classroom.Active)
                                                                       .Include(classroom => classroom.Teachers)
                                                                       .Include(classroom => classroom.Students)
                                                                       .Include(classroom => classroom.Assignments)
                                                                       .ToList();
            if (classesResult.Count <= 0)

                return new List<Classroom>();
            else
            {
                List<Classroom> classes = classesResult.Where(classroom => classroom.Active).Select(classroom => classroom.ToEntityWithoutList()).ToList();

                for (int i = 0; i < classesResult.Count; i++)
                {
                    if (classesResult[i].Id == classes[i].Id)
                    {
                        if (classesResult[i].Students.Count >= 1)
                        {
                            List<Student> students = classesResult[i].Students.Where(student => student.Active).Select(student => student.ToEntityWithoutList()).ToList();
                            classes[i].Students = students;
                        }

                        if (classesResult[i].Teachers.Count >= 1)
                        {
                            List<Teacher> teachers = classesResult[i].Teachers.Where(teacher => teacher.Active).Select(teacher => teacher.ToEntityWithoutList()).ToList();
                            classes[i].Teachers = teachers;
                        }

                        if (classesResult[i].Assignments.Count >= 1)
                        {
                            List<Assignment> assignments = classesResult[i].Assignments.Where(assignment => assignment.Active).Select(student => student.ToEntity()).ToList();
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
                                                                       .Include(classroom => classroom.Teachers)
                                                                       .Include(classroom => classroom.Students)
                                                                       .Include(classroom => classroom.Assignments)
                                                                       .FirstOrDefault();
            if (classroomResult == null)
                return new Classroom();

            Classroom classroom = classroomResult.ToEntityWithoutList();

            if (classroomResult.Students.Count >= 1)
            {
                List<Student> students = classroomResult.Students.Where(student => student.Active).Select(student => student.ToEntityWithoutList()).ToList();
                classroom.Students = students;
            }

            if (classroomResult.Teachers.Count >= 1)
            {
                List<Teacher> teachers = classroomResult.Teachers.Where(teacher => teacher.Active).Select(teacher => teacher.ToEntityWithoutList()).ToList();
                classroom.Teachers = teachers;
            }

            if (classroomResult.Assignments.Count >= 1)
            {
                List<Assignment> assignments = classroomResult.Assignments.Where(assignment => assignment.Active).Select(assignment => assignment.ToEntity()).ToList();
                classroom.Assignments = assignments;
            }

            return classroom;
        }

        public Classroom GetClassroomByName(string p_name)
        {
            Classroom_SQLDTO classroomResult = this._context.Classrooms.Where(classroom => classroom.Name == p_name && classroom.Active)
                                                                       .Include(classroom => classroom.Teachers)
                                                                       .Include(classroom => classroom.Students)
                                                                       .Include(classroom => classroom.Assignments)
                                                                       .FirstOrDefault();
            if (classroomResult == null)
                return new Classroom();

            Classroom classroom = classroomResult.ToEntityWithoutList();

            if (classroomResult.Students.Count >= 1)
            {
                List<Student> students = classroomResult.Students.Where(student => student.Active).Select(student => student.ToEntityWithoutList()).ToList();
                classroom.Students = students;
            }

            if (classroomResult.Teachers.Count >= 1)
            {
                List<Teacher> teachers = classroomResult.Teachers.Where(teacher => teacher.Active).Select(teacher => teacher.ToEntityWithoutList()).ToList();
                classroom.Teachers = teachers;
            }

            if (classroomResult.Assignments.Count >= 1)
            {
                List<Assignment> assignments = classroomResult.Assignments.Where(assignment => assignment.Active).Select(assignment => assignment.ToEntity()).ToList();
                classroom.Assignments = assignments;
            }

            return classroom;
        }

        public List<Assignment> GetAssignmentsByClassroomName(string p_classroomName)
        {
            Classroom_SQLDTO classroomResult = this._context.Classrooms.Where(classroom => classroom.Name == p_classroomName && classroom.Active)
                                                                       .Include(classroom => classroom.Assignments)
                                                                       .FirstOrDefault();

            if (classroomResult != null && classroomResult.Assignments.Count >= 1)
                return classroomResult.Assignments.Select(assignment => assignment.ToEntity()).ToList();

            return new List<Assignment>();
        }

        public List<Student> GetStudentsByClassroomName(string p_classroomName)
        {
            Classroom_SQLDTO classroomResult = this._context.Classrooms.Where(classroom => classroom.Name == p_classroomName && classroom.Active)
                                                                       .Include(classroom => classroom.Students)
                                                                       .FirstOrDefault();

            if (classroomResult != null && classroomResult.Students.Count >= 1)
                return classroomResult.Students.Select(student => student.ToEntityWithoutList()).ToList();

            return new List<Student>();
        }

        public List<Teacher> GetTeachersByClassroomName(string p_classroomName)
        {
            Classroom_SQLDTO classroomResult = this._context.Classrooms.Where(classroom => classroom.Name == p_classroomName && classroom.Active)
                                                                       .Include(classroom => classroom.Teachers)
                                                                       .FirstOrDefault();

            if (classroomResult != null && classroomResult.Teachers.Count >= 1)
                return classroomResult.Teachers.Select(teacher => teacher.ToEntityWithoutList()).ToList();

            return new List<Teacher>();
        }

        public void AddAssignmentToClassroom(string p_classroomName, string p_assignmentName)
        {
            Classroom_SQLDTO classroomResult = this._context.Classrooms.Where(classroom => classroom.Name == p_classroomName && classroom.Active)
                                                                       .Include(classroom => classroom.Assignments)
                                                                       .FirstOrDefault();
            if (classroomResult != null)
            {
                Assignment_SQLDTO assignmentResult = this._context.Assignments.Where(assignment => assignment.Active)
                                                                              .SingleOrDefault(assignment => assignment.Name == p_assignmentName);

                if (assignmentResult != null && !classroomResult.Assignments.Contains(assignmentResult))
                {
                    classroomResult.Assignments.Add(assignmentResult);

                    this._context.Update(classroomResult);
                    this._context.SaveChanges();
                }
            }
        }

        public void AddStudentToClassroom(string p_classroomName, string p_studentUsername)
        {
            Classroom_SQLDTO classroomResult = this._context.Classrooms.Where(classroom => classroom.Name == p_classroomName && classroom.Active)
                                                           .Include(classroom => classroom.Students)
                                                           .FirstOrDefault();
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
        }

        public void AddTeacherToClassroom(string p_classroomName, string p_teacherUsername)
        {
            Classroom_SQLDTO classroomResult = this._context.Classrooms.Where(classroom => classroom.Name == p_classroomName && classroom.Active)
                                                                       .Include(classroom => classroom.Teachers)
                                                                       .FirstOrDefault();
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
        }

        public void RemoveAssignmentFromClassroom(string p_classroomName, string p_assignment)
        {
            Classroom_SQLDTO classroomResult = this._context.Classrooms.Where(classroom => classroom.Name == p_classroomName && classroom.Active)
                                                                       .Include(classroom => classroom.Assignments)
                                                                       .FirstOrDefault();

            Assignment_SQLDTO assignmentResult = this._context.Assignments.Where(assignment => assignment.Active)
                                                                          .SingleOrDefault(assignment => assignment.Name == p_assignment);

            if (classroomResult.Assignments.Contains(assignmentResult))
            {
                classroomResult.Assignments.Remove(assignmentResult);

                this._context.Update(classroomResult);
                this._context.SaveChanges();
            }
        }

        public void RemoveStudentFromClassroom(string p_classroomName, string p_username)
        {
            Classroom_SQLDTO classroomResult = this._context.Classrooms.Where(classroom => classroom.Name == p_classroomName && classroom.Active)
                                                                       .Include(classroom => classroom.Students)
                                                                       .FirstOrDefault();

            Student_SQLDTO studentResult = this._context.Students.Where(student => student.Active)
                                                                 .SingleOrDefault(student => student.Username == p_username);

            if (classroomResult.Students.Contains(studentResult))
            {
                classroomResult.Students.Remove(studentResult);

                this._context.Update(classroomResult);
                this._context.SaveChanges();
            }
        }

        public void RemoveTeacherFromClassroom(string p_classroomName, string p_username)
        {
            Classroom_SQLDTO classroomResult = this._context.Classrooms.Where(classroom => classroom.Name == p_classroomName && classroom.Active)
                                                                       .Include(classroom => classroom.Teachers)
                                                                       .FirstOrDefault();

            Teacher_SQLDTO teacherResult = this._context.Teachers.Where(teacher => teacher.Active).SingleOrDefault(teacher => teacher.Username == p_username);

            if (classroomResult.Teachers.Contains(teacherResult))
            {
                classroomResult.Teachers.Remove(teacherResult);

                this._context.Update(classroomResult);
                this._context.SaveChanges();
            }
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

            Classroom_SQLDTO classroomResult = this._context.Classrooms.Where(classroom => classroom.Active).FirstOrDefault(classroom => classroom.Id == p_classroom.Id);

            if (classroomResult != null)
            {
                classroomResult.Name = p_classroom.Name;
                classroomResult.OrganisationName = p_classroom.OrganisationName;
                classroomResult.Students = students;
                classroomResult.Teachers = teachers;
                classroomResult.Assignments = assignments;

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
        }

        public void DeleteClassroom(string p_classroomName)
        {
            Classroom_SQLDTO classroomResult = this._context.Classrooms.Where(classroom => classroom.Active).SingleOrDefault(classroom => classroom.Name == p_classroomName);

            if (classroomResult != null)
            {
                classroomResult.Active = false;

                this._context.Update(classroomResult);
                this._context.SaveChanges();
            }
        }
    }
}
