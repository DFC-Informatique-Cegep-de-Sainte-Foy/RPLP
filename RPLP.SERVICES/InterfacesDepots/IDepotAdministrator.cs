using RPLP.ENTITES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.SERVICES.InterfacesDepots
{
    public interface IDepotAdministrator
    {
        public List<Administrator> GetAdministrators();
        public Administrator GetAdministratorById(int p_id);
        public Administrator GetAdministratorByUsername(string p_adminUsername);
        public List<Organisation> GetAdminOrganisations(string p_adminUsername);
        public void UpsertAdministrator(Administrator p_administrator);
        public void JoinOrganisation(string p_adminUsername, string p_organisationName);
        public void LeaveOrganisation(string p_adminUsername, string p_organisationName);
        public void DeleteAdministrator(string p_adminUsername);
    }
}
