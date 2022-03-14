using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.ENTITES
{
    public class Organisation
    {
        public int id { get; set; }
        public string name { get; set; }
        public List<Teacher> teachers { get; set; }
        public List<Student> students { get; set; }
        public List<Administrator> administrators { get; set; }

        public Organisation()
        {
            this.teachers = new List<Teacher>();
            this.students = new List<Student>();
            this.administrators = new List<Administrator>();
        }
    }
}
