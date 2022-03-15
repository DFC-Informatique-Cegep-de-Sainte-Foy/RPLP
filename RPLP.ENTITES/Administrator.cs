using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.ENTITES
{
    public class Administrator
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<Organisation> Organisations { get; set; }

        public Administrator()
        {
            this.Organisations = new List<Organisation>();
        }

        public Administrator(int p_id, string p_username, string p_firstName, string p_lastName, List<Organisation> p_organisations)
        {
            this.Id = p_id;
            this.Username = p_username;
            this.FirstName = p_firstName;
            this.LastName = p_lastName;
            this.Organisations = p_organisations;
        }
    }
}
