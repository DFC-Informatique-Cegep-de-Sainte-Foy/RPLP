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
    public class DepotTeacher : IDepotTeacher
    {
        private readonly RPLPDbContext _context;

        public DepotTeacher()
        {
            this._context = new RPLPDbContext();
        }

        public DepotTeacher(RPLPDbContext p_context)
        {
            this._context = p_context;
        }

        public List<Teacher> GetTeachers()
        {
            List<Teacher_SQLDTO> teachersResult = this._context.Teachers.Where(teacher => teacher.Active)
                                                                        .Include(teacher => teacher.Classes.Where(classroom => classroom.Active)).ToList();

            List<Teacher> teachers = teachersResult.Select(teacher => teacher.ToEntityWithoutList()).ToList();

            for (int i = 0; i < teachersResult.Count; i++)
            {
                if (teachersResult[i].Id == teachers[i].Id && teachersResult[i].Classes.Count >= 1)
                    teachers[i].Classes = teachersResult[i].Classes.Select(classroom => classroom.ToEntityWithoutList()).ToList();
            }

            return teachers;
        }

        public Teacher GetTeacherById(int p_id)
        {
            Teacher_SQLDTO teacherResult = this._context.Teachers
                .Include(teacher => teacher.Classes.Where(classroom => classroom.Active))
                .FirstOrDefault(teacher => teacher.Id == p_id && teacher.Active);
                                                                 
                                                                 
            if (teacherResult == null)
                return null;

            Teacher teacher = teacherResult.ToEntityWithoutList();

            if (teacherResult.Classes.Count >= 1)
            {
                List<Classroom> classes = teacherResult.Classes.Select(classroom => classroom.ToEntityWithoutList()).ToList();
                teacher.Classes = classes;
            }

            return teacher;
        }

        public Teacher GetTeacherByUsername(string p_teacherUsername)
        {
            Teacher_SQLDTO teacherResult = this._context.Teachers.Include(teacher => teacher.Classes.Where(classroom => classroom.Active))
                                                                 .FirstOrDefault(teacher => teacher.Username == p_teacherUsername && teacher.Active);
            if (teacherResult == null)
                return null;

            Teacher teacher = teacherResult.ToEntityWithoutList();

            if (teacherResult.Classes.Count >= 1)
            {
                List<Classroom> classes = teacherResult.Classes.Select(classroom => classroom.ToEntityWithoutList()).ToList();
                teacher.Classes = classes;
            }

            return teacher;
        }

        public List<Classroom> GetTeacherClasses(string p_teacherUsername)
        {
            List<Classroom> classes = new List<Classroom>();
            Teacher_SQLDTO teacher = this._context.Teachers.Include(teacher => teacher.Classes.Where(classroom => classroom.Active))
                                                           .FirstOrDefault(teacher => teacher.Username == p_teacherUsername && teacher.Active);
            if (teacher != null)
            {
                if (teacher.Classes.Count >= 1)
                {
                    foreach (Classroom_SQLDTO classroom in teacher.Classes)
                    {
                        classes.Add(classroom.ToEntityWithoutList());
                    }

                    return classes;
                }
            }

            return new List<Classroom>();
        }

        public void AddClassroomToTeacher(string p_teacherUsername, string p_classroomName)
        {
            Classroom_SQLDTO classroomResult = this._context.Classrooms
                .FirstOrDefault(classroom => classroom.Name == p_classroomName && classroom.Active);

            if (classroomResult != null)
            {
                Teacher_SQLDTO teacherResult = this._context.Teachers
                    .SingleOrDefault(teacher => teacher.Username == p_teacherUsername && teacher.Active);
                if (teacherResult != null)
                {
                    teacherResult.Classes.Add(classroomResult);

                    this._context.Update(teacherResult);
                    this._context.SaveChanges();
                }
            }
        }

        public void RemoveClassroomFromTeacher(string p_teacherUsername, string p_classroomName)
        {
            Classroom_SQLDTO classroomResult = this._context.Classrooms
                .FirstOrDefault(classroom => classroom.Name == p_classroomName && classroom.Active);
            if (classroomResult != null)
            {
                Teacher_SQLDTO teacherResult = this._context.Teachers
                    .Include(t => t.Classes)
                    .SingleOrDefault(teacher => teacher.Username == p_teacherUsername && teacher.Active);

                if (teacherResult != null)
                {
                    teacherResult.Classes.Remove(classroomResult);
                    this._context.Update(teacherResult);
                    this._context.SaveChanges();
                }
            }
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

            Teacher_SQLDTO teacherResult = this._context.Teachers.SingleOrDefault(teacher => teacher.Id == p_teacher.Id && teacher.Active);
            if (teacherResult != null)
            {
                teacherResult.Username = p_teacher.Username;
                teacherResult.FirstName = p_teacher.FirstName;
                teacherResult.LastName = p_teacher.LastName;
                teacherResult.Email = p_teacher.Email;
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
                teacher.Email = p_teacher.Email;
                teacher.Classes = classes;
                teacher.Active = true;

                this._context.Teachers.Add(teacher);
                this._context.SaveChanges();
            }
        }

        public void DeleteTeacher(string p_teacherUsername)
        {
            Teacher_SQLDTO teacherResult = this._context.Teachers
                .FirstOrDefault(teacher => teacher.Username == p_teacherUsername && teacher.Active);

            if (teacherResult != null)
            {
                teacherResult.Active = false;

                this._context.Update(teacherResult);
                this._context.SaveChanges();
            }
        }
    }
}
