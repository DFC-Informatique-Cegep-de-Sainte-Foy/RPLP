using RPLP.DAL.DTO.Sql;
using RPLP.ENTITES;
using RPLP.SERVICES.InterfacesDepots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.SERVICES.Github
{
    public class ScriptGithubRPLP
    {
        private readonly IDepotClassroom _depotClassroom;

        public ScriptGithubRPLP(IDepotClassroom p_depotClassroom)
        {
            this._depotClassroom = p_depotClassroom;
        }

        public List<Classroom> ScriptAssignStudentToAssignmentReview()
        {
            return _depotClassroom.GetClassrooms();
        }
    }
}
