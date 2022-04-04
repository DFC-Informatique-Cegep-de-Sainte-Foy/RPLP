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
        public string Token { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<Organisation> Organisations { get; set; }

        public Administrator()
        {
            this.Organisations = new List<Organisation>();
        }

        public Administrator(int p_id, string p_username,string p_token ,string p_firstName, string p_lastName, string p_email, List<Organisation> p_organisations)
        {
            this.Id = p_id;
            this.Username = p_username;
            this.Token = p_token;
            this.FirstName = p_firstName;
            this.LastName = p_lastName;
            this.Email = p_email;
            this.Organisations = p_organisations;
        }
    }
}
