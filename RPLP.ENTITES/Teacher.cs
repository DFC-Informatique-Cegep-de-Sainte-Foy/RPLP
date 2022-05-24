using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.ENTITES
{
    public class Teacher
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<Classroom> Classes { get; set; }

        public Teacher()
        {
            this.Classes = new List<Classroom>();
        }

        public Teacher(int p_id, string p_username, string p_firstName, string p_lastName, string p_email, List<Classroom> p_classes)
        {
            this.Id = p_id;
            this.Username = p_username;
            this.FirstName = p_firstName;
            this.LastName = p_lastName;
            this.Email = p_email;
            this.Classes = p_classes;
        }
    }
}
