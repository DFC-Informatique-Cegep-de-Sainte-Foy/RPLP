using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.ENTITES
{
    public class Administrator
    {
        public int id { get; set; }
        public string username { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public List<Organisation> organisations { get; set; }

        public Administrator()
        {
            this.organisations = new List<Organisation>();
        }

        public Administrator(int p_id, string p_username, string p_firstName, string p_lastName, List<Organisation> p_organisations)
        {
            this.id = p_id;
            this.username = p_username;
            this.firstName = p_firstName;
            this.lastName = p_lastName;
            this.organisations = p_organisations;
        }
    }
}
