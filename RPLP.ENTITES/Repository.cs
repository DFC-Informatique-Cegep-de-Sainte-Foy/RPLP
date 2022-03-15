using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.ENTITES
{
    public class Repository
    {
        public int id { get; set; }
        public string name { get; set; }
        public string organisationName { get; set; }
        public string fullName { get; set; }

        public Repository()
        {

        }

        public Repository(int p_id, string p_name, string p_organisationName, string p_fullName)
        {
            this.id = p_id;
            this.name = p_name;
            this.organisationName = p_organisationName;
            this.fullName = p_fullName;
        }

    }
}
