using RPLP.ENTITES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.SERVICES.InterfacesDepots
{
    public interface IDepotAllocation
    {
        public List<Allocation> GetAllocations();
        public List<Allocation> GetAllocationsByStudentId(int p_studentId);
        public List<Allocation> GetAllocationsByRepositoryID(int p_repositoryId);
        public Allocation GetAllocationByStudentAndRepositoryIDs(int p_studentId, int p_repositoryId);
        public List<Allocation> GetAllocationsByAssignmentID(int p_assignmentId);
        public List<Allocation> GetAllocationsByStudentUsername(string p_studentUsername);
        public List<Allocation> GetAllocationsByRepositoryName(string p_repositoryName);
        public Allocation GetAllocationByStudentAndRepositoryNames(string p_studentUsername, string p_repositoryName);
        public List<Allocation> GetAllocationsByAssignmentName(string p_assignmentName);
        public void UpsertAllocation(Allocation p_allocation);
        public void UpsertAllocationsBatch(List<Allocation> p_allocations);
        public void DeleteAllocation(Allocation p_allocation);
        public void DeleteAllocationsBatch(List<Allocation> p_allocations);
    }
}
