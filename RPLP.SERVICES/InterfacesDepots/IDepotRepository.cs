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
        public List<Repository> GetRepositories();
        public Repository GetRepositoryById(int p_id);
        public Repository GetRepositoryByName(string p_repositoryName);
        public void UpsertRepository(Repository repository);
        public List<Repository> GetRepositoryByClassroomName(string p_classroomName);
    }
}
