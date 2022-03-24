using RPLP.ENTITES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.SERVICES.InterfacesDepots
{
    public interface IDepotRepository
    {
        public Repository GetRepositoryById(int p_id);
        public Repository GetRepositoryByName(string p_repositoryName);
        public void UpsertRepository(Repository repository);
        public void DeleteRepository(string p_repositoryName);
    }
}
