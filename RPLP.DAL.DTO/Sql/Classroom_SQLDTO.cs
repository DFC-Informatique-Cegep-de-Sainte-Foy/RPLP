using RPLP.ENTITES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.DAL.DTO.Sql
{
    public class Classroom_SQLDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string OrganisationName { get; set; }
        public List<Student_SQLDTO> Students { get; set; }
        public List<Teacher_SQLDTO> Teachers { get; set; }
        public List<Assignment_SQLDTO> Assignment { get; set; }
        public bool Active { get; set; }

        public Classroom_SQLDTO()
        {
            this.Students = new List<Student_SQLDTO>();
            this.Teachers = new List<Teacher_SQLDTO>();
            this.Assignment = new List<Assignment_SQLDTO>();
        }

        public Classroom_SQLDTO(Classroom classroom)
        {
            List<Student_SQLDTO> students = new List<Student_SQLDTO>();
            List<Teacher_SQLDTO> teachers = new List<Teacher_SQLDTO>();
            List<Assignment_SQLDTO> assignments = new List<Assignment_SQLDTO>();

            this.Id = classroom.Id;
            this.Name = classroom.Name;
            this.OrganisationName = classroom.OrganisationName;

            if (classroom.Students.Count >= 1)
            {
                foreach (Student student in classroom.Students)
                {
                    students.Add(new Student_SQLDTO(student));
                }
            }
            if (classroom.Teachers.Count >= 1)
            {
                foreach (Teacher teacher in classroom.Teachers)
                {
                    teachers.Add(new Teacher_SQLDTO(teacher));
                }
            }
            if (classroom.Assignment.Count >= 1)
            {
                foreach (Assignment assignment in classroom.Assignment)
                {
                    assignments.Add(new Assignment_SQLDTO(assignment));
                }
            }

            this.Students = students;
            this.Teachers = teachers;
            this.Assignment = assignments;
            this.Active = true;
        }

        public Classroom ToEntity()
        {
            return new Classroom(this.Id, this.Name, this.OrganisationName, this.Students.Select(student => student.ToEntity()).ToList(), this.Teachers.Select(teacher => teacher.ToEntity()).ToList(), this.Assignment.Select(assignment => assignment.ToEntity()).ToList());
        }
    }
}
