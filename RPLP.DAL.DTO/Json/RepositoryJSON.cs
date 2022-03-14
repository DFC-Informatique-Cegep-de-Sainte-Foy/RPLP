using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.DAL.DTO.Json
{
    public class RepositoryJSON
    {
        public int id { get; set; }
        public string name { get; set; }
        public string full_name { get; set; }
        public OrganisationJSON owner { get; set; }
    }
}
