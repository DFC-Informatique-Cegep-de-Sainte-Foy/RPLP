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
        public int OrganisationId { get; set; }
        public string FullName { get; set; }

        public Repository()
        {

        }

        public Repository(int p_id, string p_name, int p_organisationId, string p_fullName)
        {
            this.Id = p_id;
            this.Name = p_name;
            this.OrganisationId = p_organisationId;
            this.FullName = p_fullName;
        }

    }
}
