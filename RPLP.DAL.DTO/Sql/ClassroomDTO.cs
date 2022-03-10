using RPLP.ENTITES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.DAL.DTO.Sql
{
    public class ClassroomDTO
    {
        public int id { get; set; }
        public string name { get; set; }

        public ClassroomDTO()
        {

        }

        public ClassroomDTO(Classroom classroom)
        {
            this.id = classroom.id;
            this.name = classroom.name;
        }

        public Classroom ToEntity()
        {
            return new Classroom(this.id, this.name, new List<Student>(), new List<Teacher>(), new List<Assignment>());
        }
    }
}
