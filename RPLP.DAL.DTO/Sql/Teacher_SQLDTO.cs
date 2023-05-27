using RPLP.ENTITES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RPLP.DAL.DTO.Sql
{
    [Index("Username","Email", IsUnique = true)]
    public class Teacher_SQLDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<Classroom_SQLDTO> Classes { get; set; }
        public bool Active { get; set; }

        public Teacher_SQLDTO()
        {
            this.Classes = new List<Classroom_SQLDTO>();
        }

        public Teacher_SQLDTO(Teacher p_teacher)
        {
            List<Classroom_SQLDTO> classes = new List<Classroom_SQLDTO>();

            this.Id = p_teacher.Id;
            this.Username = p_teacher.Username;
            this.FirstName = p_teacher.FirstName;
            this.LastName = p_teacher.LastName;
            this.Email = p_teacher.Email;

            if (p_teacher.Classes.Count >= 1)
            {
                foreach (Classroom classroom in p_teacher.Classes)
                {
                    classes.Add(new Classroom_SQLDTO(classroom));
                }
            }

            this.Classes = classes;
            this.Active = true;
        }

        public Teacher ToEntity()
        {
            return new Teacher(this.Id, this.Username, this.FirstName, this.LastName, this.Email, this.Classes.Select(classroom => classroom.ToEntity()).ToList());
        }

        public Teacher ToEntityWithoutList()
        {
            return new Teacher(this.Id, this.Username, this.FirstName, this.LastName, this.Email, new List<Classroom>());
        }
    }
}
