using RPLP.ENTITES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.ENTITES.InterfacesDepots
{
    public interface IDepotAssignment
    {
        public List<Assignment> GetAssignments();
        public Assignment GetAssignmentById(int p_id);
        public Assignment GetAssignmentByName(string p_assignmentName);
        public List<Assignment> GetAssignmentsByClassroomName(string p_classroomName);
        public void UpsertAssignment(Assignment p_assignment);
        public void DeleteAssignment(string p_assignmentName);
    }
}
