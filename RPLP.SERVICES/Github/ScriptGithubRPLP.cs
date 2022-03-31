﻿using RPLP.DAL.DTO.Json;
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

        public string ScriptAssignStudentToAssignmentReview(string p_organisationName, string p_classRoomName, string p_assignmentName, int p_reviewsPerRepository)
        {
            if (p_reviewsPerRepository <= 0 || string.IsNullOrWhiteSpace(p_organisationName) || string.IsNullOrWhiteSpace(p_classRoomName) || string.IsNullOrWhiteSpace(p_assignmentName))
                throw new ArgumentException("One of the provided value is incorrect or null");

            string result = "";
            List<Student> students = _depotClassroom.GetStudentsByClassroomName(p_classRoomName);

            if (students.Count < p_reviewsPerRepository + 1)
            {
                return "Number of studentss inferior to number of reviews";
            }

            List<Repository> repositoriesToAssign = getRepositoriesToAssign(p_organisationName, p_classRoomName, p_assignmentName, students);
            Dictionary<string, int> studentDictionary = new Dictionary<string, int>();

            //Populer le dictionnaire d'assignation
            foreach (Repository repository in repositoriesToAssign)
            {
                string[] splitRepository = repository.Name.Split('-');
                string studentUsername = splitRepository[1];

                studentDictionary[studentUsername] = 0;
            }

            //Faire l'action
            foreach (Repository repository in repositoriesToAssign)
            {
                result = prepareRepositoryAndCreatePullRequest(p_organisationName, repository.Name, studentDictionary, p_reviewsPerRepository);
            }

            return result;
        }

        private string prepareRepositoryAndCreatePullRequest(string p_organisationName, string p_repositoryName, Dictionary<string, int> p_studentDictionary, int p_reviewsPerRepository)
        {
            string result = "";
            string[] splitRepository = p_repositoryName.Split('-');
            Branch_JSONDTO branchDTO = new Branch_JSONDTO();

            List<Branch_JSONDTO> branchesResult = this._githubApiAction.GetRepositoryBranchesGithub(p_organisationName, p_repositoryName);

            if (branchesResult == null)
            {
                throw new ArgumentNullException("Branch is null");
            }

            foreach (Branch_JSONDTO branch in branchesResult)
            {
                string[] branchName = branch.reference.Split("/");

                if (branchName[2] == "feedback")
                {
                    branchDTO = branch;
                    break;
                }
            }

            int numberStudentAdded = 0;

            do
            {
                string username = p_studentDictionary.Where(dictionary => dictionary.Key.ToLower() != splitRepository[1].ToLower())
                                                     .FirstOrDefault(dictionary => dictionary.Value == p_studentDictionary.Values.Min()).Key;

                result = createPullRequestAndAssignUser(p_organisationName, p_repositoryName, branchDTO.gitObject.sha, username);
                if (result != "Created")
                    return result;

                p_studentDictionary[username] = p_studentDictionary[username]++;
                numberStudentAdded++;

            } while (numberStudentAdded < p_reviewsPerRepository);


            return result;
        }

        private string createPullRequestAndAssignUser(string p_organisationName, string p_repositoryName, string p_sha, string p_username)
        {
            string newBranchName = $"Feedback-{p_username}";

            //un return a ete mis a chaque pour que si un erreur arrive entre les actions sa stop
            string resultCreateBranch = this._githubApiAction.CreateNewBranchForFeedbackGitHub(p_organisationName, p_repositoryName, p_sha, newBranchName);
            if (resultCreateBranch != "Created")
                return "Branch not created";

            string resultCreatePR = this._githubApiAction.CreateNewPullRequestFeedbackGitHub(p_organisationName, p_repositoryName, newBranchName, newBranchName.ToLower(), "Voici ou vous devez mettre vos commentaires");
            if (resultCreatePR != "Created")
                return "PullRequest not created";

            string resultAddStudent = this._githubApiAction.AddStudentAsCollaboratorToPeerRepositoryGithub(p_organisationName, p_repositoryName, p_username);
            if (resultAddStudent != "Created")
                return "Student not added";

            return "Created";
        }


        private List<Repository> getRepositoriesToAssign(string p_organisationName, string p_classRoomName, string p_assignmentName, List<Student> p_students)
        {
            List<Assignment> assignmentsResult = _depotClassroom.GetAssignmentsByClassroomName(p_classRoomName);

            if (assignmentsResult.Count < 1)
            {
                throw new ArgumentOutOfRangeException("number of repository is insufficient");
            }

            Assignment assignment = assignmentsResult.SingleOrDefault(assignment => assignment.Name == p_assignmentName);

            if (assignment == null)
            {
                throw new ArgumentNullException("assignment is null");
            }

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
                            break;
                        }
                    }

                }
            }

            return repositories;
        }
    }
}
