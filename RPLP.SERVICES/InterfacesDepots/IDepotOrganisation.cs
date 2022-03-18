using RPLP.ENTITES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.SERVICES.InterfacesDepots
{
    public interface IDepotOrganisation
    {
        public List<Organisation> GetOrganisations();
        public Organisation GetOrganisationById(int id);
        public void UpsertOrganisation(Organisation organisation);
        public void AddAdministrator(string p_adminUsername, string p_organisationName);
    }
}
