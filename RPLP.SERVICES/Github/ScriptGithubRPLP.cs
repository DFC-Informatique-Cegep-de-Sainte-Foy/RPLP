using RPLP.DAL.DTO.Json;
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
        private readonly IDepotRepository _depotRepository;
        private readonly GithubApiAction _githubApiAction;

        public ScriptGithubRPLP(IDepotClassroom p_depotClassroom, IDepotRepository p_depotRepository, string p_token)
        {
            this._depotClassroom = p_depotClassroom;
            this._depotRepository = p_depotRepository;
            this._githubApiAction = new GithubApiAction(p_token);
        }

        public string ScriptAssignStudentToAssignmentReview(string p_organisationName, string p_classRoomName, string p_assignmentName, int p_ReviewsPerRepository)
        {
            if (p_ReviewsPerRepository <= 0 || string.IsNullOrWhiteSpace(p_organisationName) || string.IsNullOrWhiteSpace(p_classRoomName) || string.IsNullOrWhiteSpace(p_assignmentName))
                throw new ArgumentException("One of the provided value is incorrect or null");

            List<Student> students = _depotClassroom.GetStudentsByClassroomName(p_classRoomName);

            if (students.Count >= p_ReviewsPerRepository + 1)
            {
                List<Repository> repositoriesToAssign = getRepositoriesToAssign(p_organisationName, p_classRoomName, p_assignmentName, students);

                foreach (Repository repository in repositoriesToAssign)
                {
                    string[] splitRepository = repository.Name.Split('-');
                    Branch_JSONDTO branchDTO = new Branch_JSONDTO();
                    List<Branch_JSONDTO> branchesResult = this._githubApiAction.GetRepositoryBranchesGithub(p_organisationName, repository.Name);

                    foreach (Branch_JSONDTO branch in branchesResult)
                    {
                        string[] branchName = branch.reference.Split("/");

                        if (branchName[2] == "feedback")
                        {
                            branchDTO = branch;
                            break;
                        }
                    }

                    if (branchDTO != null)
                    {
                        int numberStudentAdded = 0;

                        for (int i = 0; i < students.Count; i++)
                        {
                            if (students[i].Username.ToLower() != splitRepository[1].ToLower() && numberStudentAdded <= p_ReviewsPerRepository)
                            {
                                string newBranchName = $"Feedback-{students[i].Username}";

                                string resultCreateBRanch = this._githubApiAction.CreateNewBranchForFeedbackGitHub(p_organisationName, repository.Name, branchDTO.gitObject.sha, newBranchName);

                                string resultCreatePR = this._githubApiAction.CreateNewPullRequestFeedbackGitHub(p_organisationName, repository.Name, newBranchName, newBranchName.ToLower(), "Voici ou vous devez mettre vos commentaires");

                                this._githubApiAction.AddStudentAsCollaboratorToPeerRepositoryGithub(p_organisationName, repository.Name, students[i].Username);
                                numberStudentAdded++;
                            }
                        }

                    }
                }

                return "ok";
            }

            return "not ok";
        }

        public List<Repository> getRepositoriesToAssign(string p_organisationName, string p_classRoomName, string p_assignmentName, List<Student> p_students)
        {
            List<Assignment> assignmentsResult = _depotClassroom.GetAssignmentsByClassroomName(p_classRoomName);

            if (assignmentsResult.Count >= 1)
            {
                Assignment assignment = assignmentsResult.SingleOrDefault(assignment => assignment.Name == p_assignmentName);

                if (assignment != null)
                {
                    List<Repository> repositories = new List<Repository>();
                    List<Repository> repositoriesResult = this._depotRepository.GetRepositoriesFromOrganisationName(p_organisationName);

                    foreach (Repository repository in repositoriesResult)
                    {
                        string[] splitRepository = repository.Name.Split('-');

                        if (splitRepository[0] == assignment.Name)
                        {
                            foreach (Student student in p_students)
                            {
                                if (splitRepository[1] == student.Username)
                                {
                                    repositories.Add(repository);
                                }
                            }

                        }
                    }

                    return repositories;
                }
            }

            throw new ArgumentOutOfRangeException("Not enough students");
        }
    }
}
