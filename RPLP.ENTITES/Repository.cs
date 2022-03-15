using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.ENTITES
{
    public class Repository
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string OrganisationName { get; set; }
        public string FullName { get; set; }

        public Repository()
        {

        }

        public Repository(int p_id, string p_name, string p_organisationName, string p_fullName)
        {
            this.Id = p_id;
            this.Name = p_name;
            this.OrganisationName = p_organisationName;
            this.FullName = p_fullName;
        }

    }
}
