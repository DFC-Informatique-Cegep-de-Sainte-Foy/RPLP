using RPLP.ENTITES;
using RPLP.SERVICES.Github;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.SERVICES
{
    public class MessageGitHubAPI
    {
        public Guid MessageID { get; set; }
        public Allocations Allocations { get; set; }
        public string OrganisationName { get; set; }
        public string RepositoryName { get; set; }
        public string SHA { get; set; }
        public string Username { get; set; }
        public MessageGitHubAPI()
        {

        }

        public MessageGitHubAPI(Guid p_identifiant, Allocations p_allocations, string p_organisationName, string p_repositoryName, string p_sha,
           string p_username)
        {
            MessageID = p_identifiant;
            Allocations = p_allocations;
            OrganisationName = p_organisationName;
            RepositoryName = p_repositoryName;
            SHA = p_sha;
            Username = p_username;
        }
    }
}
