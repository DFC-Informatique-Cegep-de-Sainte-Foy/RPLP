using RPLP.ENTITES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.SERVICES.InterfacesDepots
{
    internal interface IDepotAssignation
    {
        public List<Assignation> GetAssignations();
        public List<Assignation> GetAssignationsByStudentId(int p_studentId);
        public List<Assignation> GetAssignationsByRepositoryID(int p_repositoryId);
        public Assignation GetAssignationByStudentAndRepositoryIDs(int p_studentId, int p_repositoryId);
        public List<Assignation> GetAssignationsByAssignmentID(int p_assignmentId);
        public List<Assignation> GetAssignationsByStudentUsername(string p_studentUsername);
        public List<Assignation> GetAssignationsByRepositoryName(string p_repositoryName);
        public Assignation GetAssignationByStudentAndRepositoryNames(string p_studentUsername, string p_repositoryName);
        public List<Assignation> GetAssignationsByAssignmentName(string p_assignmentName);
        public void UpsertAssignation(Assignation p_assignation);
        public void DeleteAssignation(Assignation p_assignation);
    }
}
