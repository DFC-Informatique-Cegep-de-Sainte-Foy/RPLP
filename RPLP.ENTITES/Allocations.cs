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
        public Classroom _classroom { get; }
        public List<Repository> _repositories { get; }

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

        public Allocations()
        {
        }

        public Allocations(List<Repository> p_repositories, Classroom p_classroom,
            List<Allocation> p_existingAllocation)
        {
            Pairs = p_existingAllocation;
            this._classroom = p_classroom;
            this._repositories = p_repositories;
            // A revoir dans la prochaine iteration
            //ShuffleListInPlace(this._repositories);
        }

        public Allocations(List<Repository> p_repositories, Classroom p_classroom)
        {
            this.Pairs = new List<Allocation>();
            this._classroom = p_classroom;
            this._repositories = p_repositories;
            // A revoir dans la prochaine iteration
            //ShuffleListInPlace(this._repositories);
        }

        public void CreateRandomReviewsAllocation(int p_numberOfReviews)
        {
            // RPLP.JOURNALISATION.Logging.Instance.Journal(
            //     new Log($"Allocations.cs - CreateRandomReviewsAllocation(int p_numberOfReviews)" +
            //             $"p_numberOfReviews={p_numberOfReviews}" +
            //             $"this._repositories.Count={this._repositories.Count}" +
            //             $"this.Pairs={this.Pairs.Count}"));

            if (p_numberOfReviews > 0 && p_numberOfReviews < this._repositories.Count - 1)
            {
                List<string> usernamesFromCurrentRepos = ExtractUsernameFromRepoName();

                for (int i = 0; i < _repositories.Count; i++)
                {
                    for (int j = 0; j < p_numberOfReviews; j++)
                    {
                        int repoId = _repositories[i].Id;
                        int reviewerId =
                            GetReviewerIdParUsername(
                                usernamesFromCurrentRepos[(i + j + 1) % usernamesFromCurrentRepos.Count]);
                        string thisAllocationUniqueId = $"r{repoId}s{reviewerId}t0";
                        // RPLP.JOURNALISATION.Logging.Instance.Journal(
                        //     new Log($"Allocations.cs - CreateRandomReviewsAllocation(int p_numberOfReviews)" +
                        //             $"i={i} repoId={repoId}" +
                        //             $"j={j} reviewerId={reviewerId}"));
                        if (this.Pairs.FirstOrDefault(all => all.Id == thisAllocationUniqueId) is null)
                            this.Pairs.Add(new Allocation(thisAllocationUniqueId, repoId, reviewerId, null, 31));
                    }
                }
            }
            else
            {
                throw new ArgumentException("Parameter out of bounds", nameof(p_numberOfReviews));
            }
        }

        public void CreateReviewsAllocationsForStudentsWithoutRepository(List<Student> p_students, int p_numberOfReviews)
        {
            int indexAssignation = 0;

            if (p_numberOfReviews > _repositories.Count)
            {
                p_numberOfReviews = _repositories.Count;
            }

            foreach (Student student in p_students)
            {
                for (int i = 0; i < p_numberOfReviews; i++)
                {
                    indexAssignation = indexAssignation % _repositories.Count;
                    int repoId = _repositories[indexAssignation].Id;
                    string thisAllocationUniqueId = $"r{repoId}s{student.Id}t0";
                    if (this.Pairs.FirstOrDefault(all => all.Id == thisAllocationUniqueId) is null)
                        this.Pairs.Add(new Allocation(thisAllocationUniqueId, repoId, student.Id, null, 31));
                    indexAssignation++;
                }
            }
        }

        private void ShuffleListInPlace<T>(List<T> p_listToShuffle)
        {
            Random rnd = new Random();
            int n = p_listToShuffle.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                (p_listToShuffle[k], p_listToShuffle[n]) = (p_listToShuffle[n], p_listToShuffle[k]);
            }
        }

        public void CreateTeacherReviewsAllocation(string p_teacherUsername)
        {
            RPLP.JOURNALISATION.Logging.Instance.Journal(
                new Log($"Allocations.cs - CreateTeacherReviewsAllocation(string p_teacherUsername)" +
                        $"p_teacherUsername={p_teacherUsername}"));

            if (p_teacherUsername != string.Empty)
            {
                int teacherId = this._classroom.Teachers
                    .Where(teacher => teacher.Username.ToLower() == p_teacherUsername.ToLower()).FirstOrDefault().Id;


                RPLP.JOURNALISATION.Logging.Instance.Journal(
                    new Log($"Allocations.cs - CreateTeacherReviewsAllocation(string p_teacherUsername)" +
                            $"teacherId={teacherId}"));
                List<Allocation> TeacherAllocationToBeAdded = new List<Allocation>();

                foreach (Allocation allocation in this.Pairs)
                {
                    int repoId = allocation.RepositoryId;
                    string thisAllocationUniqueId = $"r{repoId}s0t{teacherId}";
                    RPLP.JOURNALISATION.Logging.Instance.Journal(
                        new Log($"Allocations.cs - CreateTeacherReviewsAllocation(string p_teacherUsername)" +
                                $"repoId={repoId}" +
                                $"teacherId={teacherId}" +
                                $"thisAllocationUniqueId={thisAllocationUniqueId}"));
                    try
                    {
                        if (this.Pairs.FirstOrDefault(all => all.Id == thisAllocationUniqueId) is null)
                            if (TeacherAllocationToBeAdded.FirstOrDefault(all => all.Id == thisAllocationUniqueId) is
                                null)
                                TeacherAllocationToBeAdded.Add(new Allocation(thisAllocationUniqueId, repoId, null,
                                    teacherId, 31));
                    }
                    catch (Exception e)
                    {
                        RPLP.JOURNALISATION.Logging.Instance.Journal(
                            new Log($"error ! ={e.Message}"));
                    }
                }

                this.Pairs.AddRange(TeacherAllocationToBeAdded);
            }
            else
            {
                throw new ArgumentException("Teacher name cannot be missing", nameof(p_teacherUsername));
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
            int reviewerId = this._classroom.Students
                .Where(reviewer => reviewer.Username.ToLower() == reviewerUsername.ToLower()).FirstOrDefault()
                .Id;

            // RPLP.JOURNALISATION.Logging.Instance.Journal(
            //     new Log($"Allocations.cs - GetReviewerIdParUsername(string reviewerUsername)" +
            //             $"reviewerUsername={reviewerUsername}" +
            //             $"reviewerId={reviewerId}"));
            return reviewerId;
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