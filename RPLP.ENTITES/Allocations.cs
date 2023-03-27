using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.ENTITES
{
    public class Allocations
    {
        public List<Allocation> Pairs { get; private set; }
        public int Status { 
            get
            {
                int status = int.MaxValue;
                foreach (Allocation allocation in this.Pairs) 
                {
                    if (allocation.Status < status)
                    {
                        status = allocation.Status;
                    }
                }
                return status;
            } 
        }

        public Allocations()
        {
            Pairs = new List<Allocation>();
        }

        public Allocations(List<Repository> p_repositories) 
        {
            ;
        }

        public List<Allocation> GetAllocationsByStudentId(int p_studentId)
        {
            return Pairs.Where(pair => pair.StudentId == p_studentId).ToList();
        }

        public List<Allocation> GetAllocationsByRepositoryId(int p_repositoryId)
        {
            return Pairs.Where(pair => pair.RepositoryId == p_repositoryId).ToList();
        }

        public int NumberOfAllocationsByStudentId(int p_studentId)
        {
            return Pairs.Where(pair => pair.StudentId == p_studentId).Count();
        }

        public int NumberOfAllocationsByRepositoryId(int p_repositoryId)
        {
            return Pairs.Where(pair => pair.StudentId == p_repositoryId).Count();
        }

        public void DeactivateAllocations()
        {
            foreach (Allocation allocation in this.Pairs)
            {
                allocation.Status = 0;
            }
        }
    }
}
