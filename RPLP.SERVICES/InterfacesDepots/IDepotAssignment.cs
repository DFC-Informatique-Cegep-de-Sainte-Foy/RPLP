using RPLP.ENTITES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.SERVICES.InterfacesDepots
{
    public interface IDepotAssignment
    {
        public List<Assignment> GetAssignments();
        public Assignment GetAssignmentById(int id);
        public void UpsertAssignment(Assignment assignment);
    }
}
