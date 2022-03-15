using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.ENTITES
{
    public class Organisation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Administrator> Administrators { get; set; }

        public Organisation()
        {
            this.Administrators = new List<Administrator>();
        }

        public Organisation(int p_id, string p_name, List<Administrator> p_administrators)
        {
            this.Id = p_id;
            this.Name = p_name;
            this.Administrators = p_administrators;
        }
    }
}
