using RPLP.DAL.DTO.Json;
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
        public Allocations_JSONDTO Allocations { get; set; }
        public MessageGitHubAPI()
        {

        }

        public MessageGitHubAPI(Guid p_identifiant, Allocations_JSONDTO p_allocations)
        {
            MessageID = p_identifiant;
            Allocations = p_allocations;
        }
    }
}
