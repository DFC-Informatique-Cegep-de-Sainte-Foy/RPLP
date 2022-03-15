using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.ENTITES
{
    public class Student
    {
        public int id { get; set; }
        public string username { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public List<Classroom> classes { get; set; }

        public Student()
        {
            this.classes = new List<Classroom>();
        }

        public Student(int p_id, string p_username, string p_firstName, string p_lastName, List<Classroom> p_classes)
        {
            this.id = p_id;
            this.username = p_username;
            this.firstName = p_firstName;
            this.lastName = p_lastName;
            this.classes = p_classes;
        }
    }
}
