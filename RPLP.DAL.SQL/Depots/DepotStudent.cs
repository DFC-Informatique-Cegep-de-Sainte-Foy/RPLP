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
    public class DepotStudent : IDepotStudent
    {
        private readonly RPLPDbContext _context;

        public DepotStudent()
        {
            this._context = new RPLPDbContext();
        }

        public DepotStudent(RPLPDbContext p_context)
        {
            this._context = p_context;
        }

        public List<Student> GetStudents()
        {
            List<Student_SQLDTO> studentsResult = this._context.Students.Where(student => student.Active)
                                                                        .Include(student => student.Classes.Where(classroom => classroom.Active)).ToList();

            List<Student> students = studentsResult.Select(admin => admin.ToEntityWithoutList()).ToList();

            for (int i = 0; i < studentsResult.Count; i++)
            {
                if (studentsResult[i].Id == students[i].Id && studentsResult[i].Classes.Count >= 1)
                    students[i].Classes = studentsResult[i].Classes.Select(classroom => classroom.ToEntityWithoutList()).ToList();
            }

            return students;
        }

        public Student GetStudentById(int p_id)
        {
            Student_SQLDTO studentResult = this._context.Students
                .Include(student => student.Classes.Where(classroom => classroom.Active))
                .FirstOrDefault(student => student.Id == p_id && student.Active);
                                                                 
                                                                 
            if (studentResult == null)
                return null;

            Student student = studentResult.ToEntityWithoutList();

            if (studentResult.Classes.Count >= 1)
            {
                List<Classroom> classes = studentResult.Classes.Select(classroom => classroom.ToEntityWithoutList()).ToList();
                student.Classes = classes;
            }

            return student;
        }

        public Student GetStudentByUsername(string p_studentUsername)
        {
            Student_SQLDTO studentResult = this._context.Students
                .Include(student => student.Classes.Where(classroom => classroom.Active))
                .FirstOrDefault(student => student.Username == p_studentUsername && student.Active);
                                                                 
            if (studentResult == null)
                return null;

            Student student = studentResult.ToEntityWithoutList();

            if (studentResult.Classes.Count >= 1)
            {
                List<Classroom> classes = studentResult.Classes.Select(classroom => classroom.ToEntityWithoutList()).ToList();
                student.Classes = classes;
            }

            return student;
        }

        public List<Classroom> GetStudentClasses(string p_studentUsername)
        {
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

                    return classes;
                }
            }

            return new List<Classroom>();
        }

        public void UpsertStudent(Student p_student)
        {
            List<Classroom_SQLDTO> classes = new List<Classroom_SQLDTO>();

            if (p_student.Classes.Count >= 1)
            {
                foreach (Classroom classroom in p_student.Classes)
                {
                    classes.Add(new Classroom_SQLDTO(classroom));
                }
            }

            Student_SQLDTO studentResult = this._context.Students.Where(student => student.Active)
                                                                 .SingleOrDefault(student => student.Id == p_student.Id);
            if (studentResult != null)
            {
                studentResult.Username = p_student.Username;
                studentResult.FirstName = p_student.FirstName;
                studentResult.LastName = p_student.LastName;
                studentResult.Classes = classes;

                this._context.Update(studentResult);
                this._context.SaveChanges();
            }
            else
            {
                Student_SQLDTO student = new Student_SQLDTO();
                student.Username = p_student.Username;
                student.FirstName = p_student.FirstName;
                student.LastName = p_student.LastName;
                student.Classes = classes;

                this._context.Students.Add(student);
                this._context.SaveChanges();
            }
        }

        public void DeleteStudent(string p_studentUsername)
        {
            Student_SQLDTO studentResult = this._context.Students.Where(student => student.Active)
                                                                 .FirstOrDefault(student => student.Username == p_studentUsername);
            if (studentResult != null)
            {
                studentResult.Active = false;

                this._context.Update(studentResult);
                this._context.SaveChanges();
            }
        }
    }
}
