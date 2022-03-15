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

        public Classroom GetClassroomById(int p_id)
        {
            return this._context.Classrooms.Where(classroom => classroom.Id == p_id).Select(classroom => classroom.ToEntity()).FirstOrDefault();
        }

        public Classroom GetClassroomByName(string p_name)
        {
            return this._context.Classrooms.Where(classroom => classroom.Name == p_name).Select(classroom => classroom.ToEntity()).FirstOrDefault();
        }

        public List<Classroom> GetClassrooms()
        {
            return this._context.Classrooms.Select(classroom => classroom.ToEntity()).ToList();
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

            if (p_classroom.Assignment.Count >= 1)
            {
                foreach (Assignment assignment in p_classroom.Assignment)
                {
                    assignments.Add(new Assignment_SQLDTO(assignment));
                }
            }

            Classroom_SQLDTO classroomResult = this._context.Classrooms.Where(classroom => classroom.Id == p_classroom.Id).FirstOrDefault();

            if (classroomResult != null)
            {
                classroomResult.Name = p_classroom.Name;
                classroomResult.OrganisationName = p_classroom.OrganisationName;
                classroomResult.Students = students;
                classroomResult.Teachers = teachers;
                classroomResult.Assignment = assignments;

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
                classDTO.Assignment = assignments;

                this._context.Classrooms.Add(classDTO);
                this._context.SaveChanges();
            }
        }
    }
}
