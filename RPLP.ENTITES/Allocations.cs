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
        }

        public Allocations(List<Repository> p_repositories, Classroom p_classroom)
        {
            this.Pairs = new List<Allocation>();
            this._classroom = p_classroom;
            this._repositories = p_repositories;
        }

        public void CreateRandomReviewsAllocation(int p_numberOfReviews)
        {
            if (p_numberOfReviews > 0 && p_numberOfReviews < this._repositories.Count)
            {
                List<string> usernamesFromCurrentRepos = ExtractUsernameFromRepoName();

                for (int i = 0; i < _repositories.Count; i++)
                {
                    for (int j = 0; j < p_numberOfReviews; j++)
                    {
                        int repoId = _repositories[i].Id;
                        //flag: Ensure that in no case the reviewer is the owner of the repos
                        int k = 0;
                        int indexReviewer = 0;
                        do
                        {
                            k++;
                            indexReviewer = (i + j + k) % usernamesFromCurrentRepos.Count;
                        } while (_repositories[i].Name.ToLower()
                                 .Contains(usernamesFromCurrentRepos[indexReviewer].ToLower()));
                        //flag: Ensure that in no case the reviewer is the owner of the repos

                        int reviewerId = GetReviewerIdParUsername(usernamesFromCurrentRepos[indexReviewer]);
                        string thisAllocationUniqueId = $"r{repoId}s{reviewerId}t0";

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

        public void CreateReviewsAllocationsForStudentsWithoutRepository(List<Student> p_students,
            int p_numberOfReviews)
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

        public void CreateTeacherReviewsAllocation(string p_teacherUsername)
        {
            if (p_teacherUsername != string.Empty)
            {
                int teacherId = this._classroom.Teachers
                    .Where(teacher => teacher.Username.ToLower() == p_teacherUsername.ToLower()).FirstOrDefault().Id;

                List<Allocation> TeacherAllocationToBeAdded = new List<Allocation>();

                if (this.Pairs.Count > 0)
                {
                    foreach (Allocation allocation in this.Pairs)
                    {
                        int repoId = allocation.RepositoryId;
                        string thisAllocationUniqueId = $"r{repoId}s0t{teacherId}";

                        try
                        {
                            if (this.Pairs.FirstOrDefault(all => all.Id == thisAllocationUniqueId) is null)
                                if (TeacherAllocationToBeAdded.FirstOrDefault(all => all.Id == thisAllocationUniqueId)
                                    is
                                    null)
                                    TeacherAllocationToBeAdded.Add(new Allocation(thisAllocationUniqueId, repoId, null,
                                        teacherId, 31));
                        }
                        catch (Exception e)
                        {
                            Logging.Instance.Journal(new Log(e.Message,
                                e.StackTrace.Replace(System.Environment.NewLine, "."),
                                "CreateTeacherReviewsAllocation(string p_teacherUsername) - catch", 0));
                        }
                    }
                }
                else
                {
                    foreach (Repository repo in this._repositories)
                    {
                        int repoId = repo.Id;
                        string thisAllocationUniqueId = $"r{repoId}s0t{teacherId}";

                        try
                        {
                            if (this.Pairs.FirstOrDefault(all => all.Id == thisAllocationUniqueId) is null)
                                if (TeacherAllocationToBeAdded.FirstOrDefault(all => all.Id == thisAllocationUniqueId)
                                    is
                                    null)
                                    TeacherAllocationToBeAdded.Add(new Allocation(thisAllocationUniqueId, repoId, null,
                                        teacherId, 31));
                        }
                        catch (Exception e)
                        {
                            Logging.Instance.Journal(new Log(e.Message,
                                e.StackTrace.Replace(System.Environment.NewLine, "."),
                                "CreateTeacherReviewsAllocation(string p_teacherUsername) - catch", 0));
                        }
                    }
                }

                this.Pairs.AddRange(TeacherAllocationToBeAdded);
            }
            else
            {
                throw new ArgumentException("Teacher name cannot be missing", nameof(p_teacherUsername));
            }
        }

        public void CreateTutorReviewsAllocation(string p_tutorUsername, int p_tutorId)
        {
            if (p_tutorUsername != string.Empty)
            {
                List<Allocation> tutorAllocationToBeAdded = new List<Allocation>();

                if (this.Pairs.Count > 0)
                {
                    foreach (Allocation allocation in this.Pairs)
                    {
                        int repoId = allocation.RepositoryId;
                        string thisAllocationUniqueId = $"r{repoId}s{p_tutorId}t0";

                        try
                        {
                            if (this.Pairs.FirstOrDefault(all => all.Id == thisAllocationUniqueId) is null)
                                if (tutorAllocationToBeAdded.FirstOrDefault(all => all.Id == thisAllocationUniqueId) is
                                    null)
                                    tutorAllocationToBeAdded.Add(new Allocation(thisAllocationUniqueId, repoId,
                                        p_tutorId,
                                        null, 31));
                        }
                        catch (Exception e)
                        {
                            Logging.Instance.Journal(new Log(e.Message,
                                e.StackTrace.Replace(System.Environment.NewLine, "."),
                                "CreateTutorReviewsAllocation(string p_tutorUsername, int p_tutorId) - catch", 0));
                        }
                    }
                }
                else
                {
                    foreach (Repository repo in this._repositories)
                    {
                        int repoId = repo.Id;
                        string thisAllocationUniqueId = $"r{repoId}s{p_tutorId}t0";

                        try
                        {
                            if (this.Pairs.FirstOrDefault(all => all.Id == thisAllocationUniqueId) is null)
                                if (tutorAllocationToBeAdded.FirstOrDefault(all => all.Id == thisAllocationUniqueId) is
                                    null)
                                    tutorAllocationToBeAdded.Add(new Allocation(thisAllocationUniqueId, repoId,
                                        p_tutorId,
                                        null, 31));
                        }
                        catch (Exception e)
                        {
                            Logging.Instance.Journal(new Log(e.Message,
                                e.StackTrace.Replace(System.Environment.NewLine, "."),
                                "CreateTutorReviewsAllocation(string p_tutorUsername, int p_tutorId) - catch", 0));
                        }
                    }
                }

                this.Pairs.AddRange(tutorAllocationToBeAdded);
            }
            else
            {
                throw new ArgumentException("Tutor username cannot be missing", nameof(p_tutorUsername));
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
                .Where(reviewer => reviewer.Username.ToLower() == reviewerUsername.ToLower()).FirstOrDefault().Id;

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