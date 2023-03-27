using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPLP.JOURNALISATION;


namespace RPLP.ENTITES
{
    public class Allocations
    {
        public List<Allocation> Pairs { get; }

        private Classroom _classroom;

        private List<Repository> _repositories;

        public int Status
        {
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

        public Allocations(List<Allocation> p_existingAllocation)
        {
            Pairs = p_existingAllocation;
        }

        public Allocations(List<Repository> p_repositories, Classroom p_classroom)
        {
            this.Pairs = new List<Allocation>();
            this._classroom = p_classroom;
            this._repositories = p_repositories;
        }

        public void CreateRandomReviewsAllocation(int p_numberOfReviews)
        {
            if (p_numberOfReviews > 0 && p_numberOfReviews < this._repositories.Count - 1)
            {
                List<string> usernamesFromCurrentRepos = ExtractUsernameFromRepoName();

                for (int i = 0; i < _repositories.Count; i++)
                {
                    for (int j = 0; j < p_numberOfReviews; j++)
                    {
                        this.Pairs.Add(new Allocation(_repositories[i].Id,
                            GetReviewerIdParUsername(
                                usernamesFromCurrentRepos[(i + j + 1) % usernamesFromCurrentRepos.Count]),
                            1));
                    }
                }
            }
            else
            {
                throw new ArgumentException("Parameter out of bounds", nameof(p_numberOfReviews));
            }
        }

        private List<string> ExtractUsernameFromRepoName()
        {
            List<string> usernamesFromRepos = new List<string>();
            string substringContainingTheAssingnmentName = this._classroom.ActiveAssignment.Name + '-';
            foreach (Repository repo in this._repositories)
            {
                usernamesFromRepos.Add(repo.Name.ToLower()
                    .Replace(substringContainingTheAssingnmentName.ToLower(), ""));
            }

            return usernamesFromRepos;
        }

        private int GetReviewerIdParUsername(string reviewerUsername)
        {
            return this._classroom.Students.Where(reviewer => reviewer.Username == reviewerUsername).FirstOrDefault()
                .Id;
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