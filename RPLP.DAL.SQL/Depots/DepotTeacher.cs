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
    public class DepotTeacher : IDepotTeacher
    {
        private readonly RPLPDbContext _context;

        public DepotTeacher()
        {
            this._context = new RPLPDbContext();
        }

        public Teacher GetTeacherById(int p_id)
        {
            Teacher teacher = this._context.Teachers.Where(teacher => teacher.Id == p_id).Select(teacher => teacher.ToEntity()).FirstOrDefault();

            if (teacher == null)
                return new Teacher();

            return teacher;
        }

        public List<Teacher> GetTeachers()
        {
            return this._context.Teachers.Select(teacher => teacher.ToEntity()).ToList();
        }

        public void UpsertTeacher(Teacher p_teacher)
        {
            List<Classroom_SQLDTO> classes = new List<Classroom_SQLDTO>();

            if (p_teacher.Classes.Count >= 1)
            {
                foreach (Classroom classroom in p_teacher.Classes)
                {
                    classes.Add(new Classroom_SQLDTO(classroom));
                }
            }

            Teacher_SQLDTO teacherResult = this._context.Teachers.Where(teacher => teacher.Id == p_teacher.Id).SingleOrDefault();

            if (teacherResult != null)
            {
                teacherResult.Username = p_teacher.Username;
                teacherResult.FirstName = p_teacher.FirstName;
                teacherResult.LastName = p_teacher.LastName;
                teacherResult.Classes = classes;

                this._context.Update(teacherResult);
                this._context.SaveChanges();
            }
            else
            {
                Teacher_SQLDTO teacher = new Teacher_SQLDTO();
                teacher.Username = p_teacher.Username;
                teacher.FirstName = p_teacher.FirstName;
                teacher.LastName = p_teacher.LastName;
                teacher.Classes = classes;

                this._context.Teachers.Add(teacher);
                this._context.SaveChanges();
            }
        }
    }
}
