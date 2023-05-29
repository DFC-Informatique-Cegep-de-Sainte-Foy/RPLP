using RPLP.ENTITES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.ENTITES.InterfacesDepots
{
    public interface IDepotOrganisation
    {
        public List<Organisation> GetOrganisations();
        public List<Organisation> GetOrganisationsInactives();
        public Organisation GetOrganisationById(int p_id);
        public Organisation GetOrganisationByName(string p_organisationName);
        public List<Administrator> GetAdministratorsByOrganisation(string p_organisationName);
        public void AddAdministratorToOrganisation(string p_organisationName, string p_adminUsername);
        public void RemoveAdministratorFromOrganisation(string p_organisationName, string p_adminUsername);
        public void UpsertOrganisation(Organisation p_organisation);
        public void DeleteOrganisation(string p_organisationName);
        //public void ReactivateOrganisation(string orgName);
        public void ReactivateOrganisation(Organisation p_organisation);
    }
}
