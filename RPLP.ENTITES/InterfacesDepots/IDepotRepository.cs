using RPLP.ENTITES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.ENTITES.InterfacesDepots
{
    public interface IDepotRepository
    {
        public List<Repository> GetRepositories();
        public List<Repository> GetRepositoriesFromOrganisationName(string p_organisationName);
        public Repository GetRepositoryById(int p_id);
        public Repository GetRepositoryByName(string p_repositoryName);
        public void UpsertRepository(Repository repository);
        public void DeleteRepository(string p_repositoryName);
        public List<Repository> GetAllRepositoriesFromOrganisationName(string p_organisationName);
    }
}
