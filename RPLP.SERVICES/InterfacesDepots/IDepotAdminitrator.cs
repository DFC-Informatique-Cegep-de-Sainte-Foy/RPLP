using RPLP.ENTITES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.SERVICES.InterfacesDepots
{
    public interface IDepotAdminitrator
    {
        public List<Administrator> GetAdministrators();
        public Administrator GetAdministratorById(int id);
        public void UpsertAdministrator(Administrator administrator);
    }
}
