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

        public Student GetStudentById(int p_id)
        {
            Student student = this._context.Students.Where(student => student.Id == p_id).Select(student => student.ToEntity()).FirstOrDefault();

            if (student == null)
                return new Student();

            return student;
        }

        public List<Student> GetStudents()
        {
            return this._context.Students.Select(student => student.ToEntity()).ToList();
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

            Student_SQLDTO studentResult = this._context.Students.Where(student => student.Id == p_student.Id).SingleOrDefault();

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
    }
}
