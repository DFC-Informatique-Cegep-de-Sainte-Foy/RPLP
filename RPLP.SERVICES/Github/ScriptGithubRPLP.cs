﻿using Newtonsoft.Json;
using RPLP.DAL.DTO.Json;
using RPLP.DAL.DTO.Sql;
using RPLP.ENTITES;
using RPLP.JOURNALISATION;
using RPLP.ENTITES.InterfacesDepots;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPLP.SERVICES.InterfacesDepots;
using System.Globalization;
using System.Data;
using System.Xml.Linq;

namespace RPLP.SERVICES.Github
{
    public class ScriptGithubRPLP
    {
        private readonly IDepotClassroom _depotClassroom;
        private readonly IDepotRepository _depotRepository;
        private readonly IDepotOrganisation _depotOrganisation;
        private readonly IDepotAllocation _depotAllocation;
        private readonly IDepotStudent _depotStudent;
        private readonly GithubApiAction _githubApiAction;
        private Classroom _activeClassroom;
        private Allocations _allocations;

        public ScriptGithubRPLP(IDepotClassroom p_depotClassroom, IDepotRepository p_depotRepository,
            IDepotOrganisation p_depotOrganisation, IDepotAllocation p_depotAllocation, IDepotStudent p_depotStudent, string p_token)
        {
            if (p_depotClassroom == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - Constructeur - p_depotClassroom passé en paramètre est null", 0));
            }

            if (p_depotRepository == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - Constructeur - p_depotRepository passé en paramètre est null", 0));
            }

            if (p_depotOrganisation == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - Constructeur - p_depotOrganisation passé en paramètre est null", 0));
            }

            if (string.IsNullOrWhiteSpace(p_token))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - Constructeur - p_token passé en paramètre est vide", 0));
            }

            this._depotClassroom = p_depotClassroom;
            this._depotRepository = p_depotRepository;
            this._depotOrganisation = p_depotOrganisation;
            this._depotAllocation = p_depotAllocation;
            this._depotStudent = p_depotStudent;
            this._githubApiAction = new GithubApiAction(p_token);
        }

        private void CreateOrUpdateActiveClassroom(string p_organisationName, string p_classRoomName,
            string p_assignmentName)
        {
            if (this._activeClassroom is null)
                this._activeClassroom = new Classroom();


            this._activeClassroom.Name = p_classRoomName;
            this._activeClassroom.OrganisationName = p_organisationName;
            this._activeClassroom.Teachers = _depotClassroom.GetTeachersByClassroomName(this._activeClassroom.Name);
            this._activeClassroom.Assignments =
                _depotClassroom.GetAssignmentsByClassroomName(this._activeClassroom.Name);
            this._activeClassroom.Students = _depotClassroom.GetStudentsByClassroomName(this._activeClassroom.Name);
            this._activeClassroom.UpdateActiveAssignment(p_assignmentName);
        }

        public void CreateOrUpdateAllocations(List<Repository> p_repositories)
        {
            if (this._allocations is null)
            {
                List<Allocation> allocationsExistingInDb =
                    this._depotAllocation.GetAllocationsByAssignmentName(this._activeClassroom.ActiveAssignment.Name);
                if (allocationsExistingInDb.Count > 0)
                {
                    List<Repository> repositoriesDansBd = new List<Repository>();
                    allocationsExistingInDb.ForEach(alloc =>
                        repositoriesDansBd.Add(_depotRepository.GetRepositoryById(alloc.RepositoryId)));
                    this._allocations =
                        new Allocations(repositoriesDansBd, this._activeClassroom, allocationsExistingInDb);
                }
                else
                {
                    this._allocations = new Allocations(p_repositories, this._activeClassroom);
                }
            }
        }

        public void ScriptAssignStudentToAssignmentReview(string p_organisationName, string p_classRoomName,
            string p_assignmentName, int p_reviewsPerRepository)
        {
            if (p_reviewsPerRepository <= 0)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentOutOfRangeException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptAssignStudentToAssignmentReview - p_reviewsPerRepository passé en paramètre est hors des limites",
                    0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptAssignStudentToAssignmentReview - p_organisationName passé en paramètre est vide",
                    0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            if (string.IsNullOrWhiteSpace(p_classRoomName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptAssignStudentToAssignmentReview - p_classRoomName passé en paramètre est vide",
                    0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            if (string.IsNullOrWhiteSpace(p_assignmentName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptAssignStudentToAssignmentReview - p_token passé en paramètre est vide",
                    0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            ValidateAllRepositoriesByAssignementHasBranch(p_organisationName, p_assignmentName);

            CreateOrUpdateActiveClassroom(p_organisationName, p_classRoomName, p_assignmentName);

            List<Repository> repositoriesToAssign = GetRepositoriesToAssign();

            if (repositoriesToAssign.Count <= p_reviewsPerRepository)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new InvalidOperationException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptAssignStudentToAssignmentReview - La liste students ajustée sans les étudiants sans dépôt n'est pas conforme selon la demande",
                    0));

                throw new ArgumentException("Number of students inferior to number of reviews");
            }

            // flag : shuffle the repositories list
            if (repositoriesToAssign is not null)
            {
                ShuffleListInPlace(repositoriesToAssign);
            }
            // flag : shuffle the repositories list

            if (repositoriesToAssign == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptAssignStudentToAssignmentReview - La liste repositoriesToAssign assignée à partir de la méthode getRepositoriesToAssign(p_organisationName, p_classRoomName, p_assignmentName, students) n'est pas conforme selon la demande",
                    0));

                throw new ArgumentNullException($"No repositories to assign in {p_classRoomName}");
            }

            List<Student> studentswithoutRepository = GetStudentsWithoutRepositoryFromAssignment(repositoriesToAssign);

            CreateOrUpdateAllocations(repositoriesToAssign);
            this._allocations.CreateRandomReviewsAllocation(p_reviewsPerRepository);
            this._allocations.CreateReviewsAllocationsForStudentsWithoutRepository(studentswithoutRepository, p_reviewsPerRepository);
            this._depotAllocation.UpsertAllocationsBatch(this._allocations.Pairs);
            PrepareRepositoryAndCreatePullRequest();
        }

        public void ScriptRemoveStudentCollaboratorsFromAssignment(string p_organisationName, string p_classRoomName,
            string p_assignmentName)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptRemoveStudentCollaboratorsFromAssignment - p_organisationName passé en paramètre est vide",
                    0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            if (string.IsNullOrWhiteSpace(p_classRoomName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptRemoveStudentCollaboratorsFromAssignment - p_classRoomName passé en paramètre est vide",
                    0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            if (string.IsNullOrWhiteSpace(p_assignmentName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptRemoveStudentCollaboratorsFromAssignment - p_assignmentName passé en paramètre est vide",
                    0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            CreateOrUpdateActiveClassroom(p_organisationName, p_classRoomName, p_assignmentName);

            List<Repository> repositoriesToAssign = GetRepositoriesToAssign();

            if (repositoriesToAssign == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptRemoveStudentCollaboratorsFromAssignment - la liste repositoriesToAssign assignée à partir de la méthode getRepositoriesToAssign(p_organisationName, p_classRoomName, p_assignmentName, students);  est null",
                    0));
            }

            foreach (Repository repository in repositoriesToAssign)
            {
                List<Collaborator_JSONDTO> collaborator =
                    _githubApiAction.GetCollaboratorFromStudentRepositoryGithub(p_organisationName, repository.Name);

                if (collaborator == null)
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "ScriptGithubRPLP - ScriptRemoveStudentCollaboratorsFromAssignment - la liste collaborator assignée à partir de la méthode _githubApiAction.GetCollaboratorFromStudentRepositoryGithub(p_organisationName, repository.Name);  est null",
                        0));
                }

                collaborator.ForEach(collaborator =>
                {
                    if (collaborator.role_name == "triage")
                        _githubApiAction.RemoveStudentAsCollaboratorFromPeerRepositoryGithub(p_organisationName,
                            repository.Name, collaborator.login);
                });
            }
        }

        private void PrepareRepositoryAndCreatePullRequest()
        {
            Producer.CallGitHubAPI(this._allocations);
        }

        public string GetNameOfRepository(int p_id)
        {
            if (p_id < 0)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentOutOfRangeException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - getNameOfRepository - p_id est hors des limites",
                    0));
            }

            return this._depotRepository.GetRepositoryById(p_id).Name;
        }

        public void SetAllocationAfterAssignation(Allocation p_allocation)
        {
            if (p_allocation == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - SetAllocationAfterAssignation - p_allocation == null",
                    0));
            }

            this._depotAllocation.SetAllocationAfterCreation(p_allocation);
        }

        public List<Allocation> GetAllocationBySelectedAllocationID(List<Allocation> p_allocations)
        {
            if (p_allocations == null || p_allocations.Count == 0)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - GetAllocationBySelectedAllocationID - p_allocations == null ou est vide",
                    0));
            }

            return this._depotAllocation.GetSelectedAllocationsByAllocationID(p_allocations);
        }

        public string CreatePullRequestAndAssignUser(string p_organisationName, string p_repositoryName,
            string p_username)
        {
            ManageConfigurationForFeedback(p_organisationName, p_repositoryName);

            Branch_JSONDTO feedbackBranch =
                GetBranchFromBranchesPerBranchType(p_organisationName, p_repositoryName, "feedback");

            string result = "";

            if (feedbackBranch != null)
            {
                string newBranchName = $"Feedback-{p_username}";

                string resultCreateBranch =
                    this._githubApiAction.CreateNewBranchGitHub(p_organisationName, p_repositoryName,
                        feedbackBranch.gitObject.sha, newBranchName.ToLower());

                result = resultCreateBranch;

                if (result == "Created")
                {
                    result = ManagePullRequestCreationForReviewer(p_organisationName, p_repositoryName, newBranchName);

                    if (result == "Created")
                    {
                         
                        string resultAddStudent =
                            this._githubApiAction.AddStudentAsCollaboratorToPeerRepositoryGithub(p_organisationName, p_repositoryName, p_username);
                        
                        // FLAG : here we accept 'NoContent' as 'Created', as both work for our purposes
                        result = resultAddStudent;
                        if (result == "NoContent")
                            result = "Created";
                        // FLAG : here we accept 'NoContent' as 'Created', as both work for our purposes

                        if (result != "Created")
                        {
                            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                                new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                                $"ScriptGithubRPLP - createPullRequestAndAssignUser - la variable resultAddStudent retourne que l'utilisateur n'as pas été créée à partir de la méthode this._githubApiAction.AddStudentAsCollaboratorToPeerRepositoryGithub({p_organisationName}, {p_repositoryName}, {p_username});",
                                0));
                        }
                    }
                    else
                    {
                        RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                            new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                            $"ScriptGithubRPLP - createPullRequestAndAssignUser - la variable resultCreatePR retourne que la requête de tirage n'as pas été créée à partir de la méthode this._githubApiAction.AddStudentAsCollaboratorToPeerRepositoryGithub({p_organisationName}, {p_repositoryName}, {p_username});",
                            0));
                    }
                }
                else
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "ScriptGithubRPLP - createPullRequestAndAssignUser - la variable resultCreateBranch retourne que la branche n'as pas été créée à partir de la méthode this._githubApiAction.AddStudentAsCollaboratorToPeerRepositoryGithub({p_organisationName}, {p_repositoryName}, {p_username});",
                        0));
                }
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - createPullRequestAndAssignUser - Il y a eu un problème dans la méthode ManageConfigurationForFeedback); ",
                    0));
            }

            return result;
        }

        private void ManageConfigurationForFeedback(string p_organisationName, string p_repositoryName)
        {
            Branch_JSONDTO mainBranch =
                GetBranchFromBranchesPerBranchType(p_organisationName, p_repositoryName, "main");

            if (mainBranch == null)
            {
                //flag
                //Cette section devrai être gérée grâce à la US de Simon
            }

            Branch_JSONDTO feedbackBranch =
                GetBranchFromBranchesPerBranchType(p_organisationName, p_repositoryName, "feedback");

            string resultFeedback = "";

            if (feedbackBranch == null)
            {
                resultFeedback = this._githubApiAction.CreateNewBranchGitHub(p_organisationName, p_repositoryName,
                    mainBranch.gitObject.sha, "feedback");

                feedbackBranch = GetBranchFromBranchesPerBranchType(p_organisationName, p_repositoryName, "feedback");
            }

            if (feedbackBranch != null || resultFeedback == "Created")
            {
                string resultPR = ManagePullRequestForFeedbackBranch(p_organisationName, p_repositoryName);
            }
        }

        private PullRequest_JSONDTO GetPullRequestByName(string p_name, List<PullRequest_JSONDTO> p_pullRequests)
        {
            if (string.IsNullOrWhiteSpace(p_name))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - GetPullRequestByName - p_name passé en paramètre est vide",
                    0));
            }

            if (p_pullRequests.Count > 0 && p_name != "")
            {
                PullRequest_JSONDTO prByName = p_pullRequests.Select(p => p)
                    .Where(m => m.title.ToLower() == p_name.ToLower()).FirstOrDefault();

                return prByName;
            }

            return null;
        }

        private string ManagePullRequestForFeedbackBranch(string p_organisationName, string p_repositoryName)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ManagePullRequestforFeedbackBranch - p_organisationName passé en paramètre est vide",
                    0));
            }

            if (string.IsNullOrWhiteSpace(p_repositoryName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ManagePullRequestforFeedbackBranch - p_repositoryName passé en paramètre est vide",
                    0));
            }

            List<PullRequest_JSONDTO> pullResquests =
                this._githubApiAction.GetRepositoryPullRequestsGithub(p_organisationName, p_repositoryName);

            PullRequest_JSONDTO pullRequestFeedback = GetPullRequestByName("Feedback", pullResquests);

            if (pullRequestFeedback == null)
            {
                this._githubApiAction.AddFileToContentsGitHub(
                    p_organisationName,
                    p_repositoryName,
                    "feedback",
                    "CommitFeedback.txt",
                    "UHJlbWllciBjb21taXQgc3VyIEZlZWRiYWNr",
                    "FeedbackBranch");

                string resultCreatePR = this._githubApiAction.CreateNewPullRequestGitHub(p_organisationName,
                    p_repositoryName, "main", "Feedback", "Welcome on the feedback branch, the branch to rule them all",
                    "feedback");

                return resultCreatePR;
            }
            else
            {
                return "Already Created";
            }
        }

        private string ManagePullRequestCreationForReviewer(string p_organisationName, string p_repositoryName,
            string newBranchName)
        {
            this._githubApiAction.AddFileToContentsGitHub(
                p_organisationName,
                p_repositoryName,
                newBranchName.ToLower(),
                $"Commit{newBranchName}.txt",
                "RmljaGllciBwb3VyIGxhIHJlcXXDqnRlIGRlIHRpcmFnZSBjcsOpw6llIHBhciByw6l2aXNldXIg",
                $"{newBranchName}Branch");

            string resultCreatePR = this._githubApiAction.CreateNewPullRequestGitHub(p_organisationName,
                p_repositoryName, "feedback", newBranchName,
                "Voici où vous devez mettre vos commentaires", newBranchName.ToLower());

            return resultCreatePR;
        }

        private List<Repository> GetRepositoriesToAssign()
        {
            if (this._activeClassroom.Assignments.Count < 1)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - getRepositoriesToAssign - la liste assignmentsResult retourne qu'il n'y pas d'assignement à partir de la méthode _depotClassroom.GetAssignmentsByClassroomName(p_classRoomName); ",
                    0));

                throw new ArgumentException($"No assignment in {this._activeClassroom.Name}");
            }

            if (this._activeClassroom.ActiveAssignment == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - getRepositoriesToAssign - la variable assignment assignée à partir de la méthode assignmentsResult.SingleOrDefault(assignment => assignment.Name == p_assignmentName); est null ",
                    0));
                throw new ArgumentException($"no assignment with name {this._activeClassroom.ActiveAssignment}");
            }

            List<Repository> repositoriesToThisAssignment = new List<Repository>();

            List<Repository> repositoriesFromDBForActiveClassroom =
                this._depotRepository.GetRepositoriesFromOrganisationName(this._activeClassroom.OrganisationName);

            repositoriesToThisAssignment =
                GetStudentsRepositoriesForAssignment(repositoriesFromDBForActiveClassroom,
                    this._activeClassroom.ActiveAssignment);

            return repositoriesToThisAssignment;
        }

        public void ScriptAssignTeacherToAssignmentReview(string p_organisationName, string p_classRoomName,
            string p_assignmentName, string teacherUsername)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptAssignTeacherToAssignmentReview - p_organisationName passé en paramètre est vide",
                    0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            if (string.IsNullOrWhiteSpace(p_classRoomName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptAssignTeacherToAssignmentReview - p_classRoomName passé en paramètre est vide",
                    0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            if (string.IsNullOrWhiteSpace(p_assignmentName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptAssignTeacherToAssignmentReview - p_assignmentName passé en paramètre est vide",
                    0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            if (string.IsNullOrWhiteSpace(teacherUsername))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptAssignTeacherToAssignmentReview - teacherUsername passé en paramètre est vide",
                    0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            CreateOrUpdateActiveClassroom(p_organisationName, p_classRoomName, p_assignmentName);

            if (this._activeClassroom.Students.Count < 1)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptAssignTeacherToAssignmentReview - la liste students assignée à partir de la méthode _depotClassroom.GetStudentsByClassroomName(p_classRoomName); est vide.",
                    0));

                throw new ArgumentException("Number of students cannot be less than one");
            }

            if (this._activeClassroom.Teachers.Count < 1)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptAssignTeacherToAssignmentReview -  la liste teachers assignée à partir de la méthode _depotClassroom.GetTeachersByClassroomName(p_classRoomName); est vide.",
                    0));

                throw new ArgumentException("Number of teachers cannot be less than one");
            }

            List<Repository> repositoriesToAssign = GetRepositoriesToAssign();

            if (repositoriesToAssign == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptAssignTeacherToAssignmentReview - la liste repositoriesToAssign assignée à partir de getRepositoriesToAssign(p_organisationName, p_classRoomName, p_assignmentName, students); est null",
                    0));

                throw new ArgumentNullException($"No repositories to assign in {p_classRoomName}");
            }

            CreateOrUpdateAllocations(repositoriesToAssign);
            this._allocations.CreateTeacherReviewsAllocation(teacherUsername);
            this._depotAllocation.UpsertAllocationsBatch(this._allocations.Pairs);
            Producer.CallGitHubAPI(this._allocations, "professor");
        }

        public void ScriptAssignTutorToAssignmentReview(string p_organisationName, string p_classRoomName,
            string p_assignmentName, string tutorUsername)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptAssignTutorToAssignmentReview - p_organisationName passé en paramètre est vide",
                    0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            if (string.IsNullOrWhiteSpace(p_classRoomName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptAssignTutorToAssignmentReview - p_classRoomName passé en paramètre est vide",
                    0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            if (string.IsNullOrWhiteSpace(p_assignmentName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptAssignTutorToAssignmentReview - p_assignmentName passé en paramètre est vide",
                    0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            if (string.IsNullOrWhiteSpace(tutorUsername))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptAssignTutorToAssignmentReview - tutorUsername passé en paramètre est vide",
                    0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            CreateOrUpdateActiveClassroom(p_organisationName, p_classRoomName, p_assignmentName);

            if (this._activeClassroom.Students.Count < 1)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptAssignTutorToAssignmentReview - la liste students assignée à partir de la méthode _depotClassroom.GetStudentsByClassroomName(p_classRoomName); est vide.",
                    0));

                throw new ArgumentException("Number of students cannot be less than one");
            }

            if (this._activeClassroom.Teachers.Count < 1)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptAssignTutorToAssignmentReview -  la liste teachers assignée à partir de la méthode _depotClassroom.GetTeachersByClassroomName(p_classRoomName); est vide.",
                    0));

                throw new ArgumentException("Number of teachers cannot be less than one");
            }

            List<Repository> repositoriesToAssign = GetRepositoriesToAssign();

            if (repositoriesToAssign == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptAssignTutorToAssignmentReview - la liste repositoriesToAssign assignée à partir de getRepositoriesToAssign(p_organisationName, p_classRoomName, p_assignmentName, students); est null",
                    0));

                throw new ArgumentNullException($"No repositories to assign in {p_classRoomName}");
            }

            Student tutor = this._depotStudent.GetStudentByUsername(tutorUsername);

            if (tutor is not null)
            {
                CreateOrUpdateAllocations(repositoriesToAssign);
                this._allocations._classroom.Students.Add(tutor);
                this._allocations.CreateTutorReviewsAllocation(tutorUsername, tutor.Id);
                this._depotAllocation.UpsertAllocationsBatch(this._allocations.Pairs);
                Producer.CallGitHubAPI(this._allocations, "students");
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptAssignTutorToAssignmentReview - tutorUsername passé en paramètre est vide ou inexistant",
                    0));

                throw new ArgumentException("the tutor username is incorrect or null");
            }
        }

        public string createPullRequestForTeacher(string p_organisationName, string p_repositoryName, string p_username,
            string p_newFileName, string p_message, string p_content)
        {
            Branch_JSONDTO mainBranch =
                GetBranchFromBranchesPerBranchType(p_organisationName, p_repositoryName, "main");

            string result = "";

            if (mainBranch != null)
            {
                string newBranchName = $"Feedback-{p_username}";

                string resultCreateBranch =
                    this._githubApiAction.CreateNewBranchGitHub(p_organisationName, p_repositoryName,
                        mainBranch.gitObject.sha, newBranchName.ToLower());

                result = resultCreateBranch;

                if (result == "Created")
                {
                    result = CreatePullRequestAndAssignTeacher(p_organisationName, p_repositoryName,
                        newBranchName.ToLower(), p_newFileName, p_message, p_content, p_username);

                    if (result != "Created")
                    {
                        RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                            new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                            "ScriptGithubRPLP - createPullRequestAndAssignUser - la variable resultCreatePR retourne que la requête de tirage n'as pas été créée à partir de la méthode this._githubApiAction.CreateNewPullRequestFeedbackGitHub(p_organisationName, p_repositoryName, newBranchName, newBranchName.ToLower(), \"Voici où vous devez mettre vos commentaires\"); ",
                            0));
                    }
                }
                else
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                        new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "ScriptGithubRPLP - createPullRequestAndAssignUser - la variable resultCreateBranch retourne que la branche n'as pas été créée à partir de la méthode this._githubApiAction.CreateNewBranchForFeedbackGitHub(p_organisationName, p_repositoryName, p_sha, newBranchName); ",
                        0));
                }
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - createPullRequestAndAssignUser - Il y a eu un problème dans la méthode ManageConfigurationForFeedback); ",
                    0));
            }

            return result;
        }

        private string CreatePullRequestAndAssignTeacher(string p_organisationName, string p_repositoryName,
            string p_newBranchName,
            string p_newFileName, string p_message, string p_content, string teacherUsername)
        {
            this._githubApiAction.AddFileToContentsGitHub(
                p_organisationName,
                p_repositoryName,
                p_newBranchName,
                p_newFileName,
                p_content,
                p_message);

            string resultCreatePR = this._githubApiAction.CreateNewPullRequestGitHub(
                p_organisationName,
                p_repositoryName,
                "main",
                p_newBranchName.ToLower(),
                "Voici où vous devez mettre vos commentaires",
                p_newBranchName);

            if (resultCreatePR != "Created")
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - CreatePullRequestAndAssignTeacher - la variable resultCreatePR indique que la requête de tirage n'as pas été créée à partir de la méthode  " +
                    "this._githubApiAction.CreateNewPullRequestFeedbackGitHub(p_organisationName, p_repositoryName, newBranchName, \"Feedback\", \"Voici où vous devez mettre vos commentaires\");",
                    0));

                throw new ArgumentException($"PullRequest not created in {p_repositoryName}");
            }

            return resultCreatePR;
        }

        public string ScriptDownloadAllRepositoriesForAssignment(string p_organisationName, string p_classRoomName,
            string p_assignmentName)
        {
            string directoryToZipName = "ZippedRepos";

            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptDownloadAllRepositoriesForAssignment - p_organisationName passé en paramètre est vide",
                    0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            if (string.IsNullOrWhiteSpace(p_classRoomName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptDownloadAllRepositoriesForAssignment - p_classRoomName passé en paramètre est vide",
                    0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            if (string.IsNullOrWhiteSpace(p_assignmentName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptDownloadAllRepositoriesForAssignment - p_assignmentName passé en paramètre est vide",
                    0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            CreateOrUpdateActiveClassroom(p_organisationName, p_classRoomName, p_assignmentName);

            if (this._activeClassroom.Students.Count < 1)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptDownloadAllRepositoriesForAssignment - la liste students assignée à partir de _depotClassroom.GetStudentsByClassroomName(p_classRoomName); est vide",
                    0));

                throw new ArgumentException("Number of students cannot be less than one");
            }

            if (this._activeClassroom.Assignments.Count < 1)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptDownloadAllRepositoriesForAssignment - la liste assignmentsResult assignée à partir de _depotClassroom.GetAssignmentsByClassroomName(p_classRoomName); est vide",
                    0));


                throw new ArgumentException($"No assignment in {p_classRoomName}");
            }

            Assignment assignmentToReview =
                this._activeClassroom.Assignments.SingleOrDefault(assignment => assignment.Name == p_assignmentName);

            if (assignmentToReview == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptDownloadAllRepositoriesForAssignment - la liste assignment assignée à partir de assignmentsResult.SingleOrDefault(assignment => assignment.Name == p_assignmentName); est null",
                    0));


                throw new ArgumentException($"No assignment with name {p_assignmentName}");
            }

            List<Repository> repositoriesFromDBForActiveClassroom =
                this._depotRepository.GetRepositoriesFromOrganisationName(this._activeClassroom.OrganisationName);

            List<Repository> repositories =
                GetStudentsRepositoriesForAssignment(repositoriesFromDBForActiveClassroom, assignmentToReview);

            DeleteFilesAndDirectoriesForDownloads();
            Directory.CreateDirectory(directoryToZipName);

            DownloadRepositoriesToDirectory(repositories);
            string path = GetZipFromReposDirectory();

            return path;
        }

        public string ScriptDownloadOneRepositoryForAssignment(string p_organisationName, string p_classRoomName,
            string p_assignmentName, string p_repositoryName)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptDownloadOneRepositoryForAssignment - p_organisationName passé en paramètre est vide",
                    0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            if (string.IsNullOrWhiteSpace(p_classRoomName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptDownloadOneRepositoryForAssignment - p_classRoomName passé en paramètre est vide",
                    0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            if (string.IsNullOrWhiteSpace(p_assignmentName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptDownloadOneRepositoryForAssignment - p_assignmentName passé en paramètre est vide",
                    0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            CreateOrUpdateActiveClassroom(p_organisationName, p_classRoomName, p_assignmentName);

            if (this._activeClassroom.Students.Count < 1)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptDownloadOneRepositoryForAssignment - la liste students assignée à partir de _depotClassroom.GetStudentsByClassroomName(p_classRoomName); est vide",
                    0));


                throw new ArgumentException("Number of students cannot be less than one");
            }

            if (this._activeClassroom.Assignments.Count < 1)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptDownloadOneRepositoryForAssignment - la liste assignmentsResult assignée à partir de _depotClassroom.GetAssignmentsByClassroomName(p_classRoomName);est vide",
                    0));


                throw new ArgumentException($"No assignment in {p_classRoomName}");
            }

            Assignment assignmentToReview =
                this._activeClassroom.Assignments.SingleOrDefault(assignment => assignment.Name == p_assignmentName);

            if (assignmentToReview == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptDownloadOneRepositoryForAssignment - la liste assignment assignée à partir de assignmentsResult.SingleOrDefault(assignment => assignment.Name == p_assignmentName); est null",
                    0));

                throw new ArgumentException($"No assignment with name {p_assignmentName}");
            }

            Repository repository = _depotRepository.GetRepositoryByName(p_repositoryName);

            string path = DownloadRepositoryToFile(repository);

            return path;
        }

        #region coherence

        public void EnsureOrganisationRepositoriesAreInDB()
        {
            List<Organisation> organisations = this._depotOrganisation.GetOrganisations();

            if (organisations == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ReturnMissingRepositories - la liste organisations assignée à partir de this._depotOrganisation.GetOrganisations(); est null",
                    0));
            }

            foreach (Organisation organisation in organisations)
            {
                List<Repository> repositoriesInDB =
                    this._depotRepository.GetRepositoriesFromOrganisationName(organisation.Name);

                List<Repository_JSONDTO> repositoriesOnGithub =
                    this._githubApiAction.GetOrganisationRepositoriesGithub(organisation.Name);

                ManageMissingRepositoryInDBFromGitHub(repositoriesInDB, repositoriesOnGithub);
                ManageMissingRepositoryInGitHubFromDB(repositoriesInDB, repositoriesOnGithub);
            }
        }

        private void ManageMissingRepositoryInDBFromGitHub(List<Repository> repositoriesInDB,
            List<Repository_JSONDTO> repositoriesOnGithub)
        {
            List<Repository> repositoriesToAdd = new List<Repository>();

            repositoriesToAdd.AddRange(repositoriesOnGithub
                .Where(ghRepo =>
                    repositoriesInDB.FirstOrDefault(dbRepo => dbRepo.FullName == ghRepo.full_name) == null)
                .Select(r => new Repository()
                {
                    FullName = r.full_name,
                    Name = r.name,
                    Organisation = this._depotOrganisation.GetOrganisationByName(r.full_name.Split('/')[0])
                }));

            repositoriesToAdd.ForEach(r => this._depotRepository.UpsertRepository(r));
        }


        private void ManageMissingRepositoryInGitHubFromDB(List<Repository> repositoriesInDB,
            List<Repository_JSONDTO> repositoriesOnGithub)
        {
            List<Repository> repositoriesDesactivate = new List<Repository>();

            foreach (Repository repo in repositoriesInDB)
            {
                bool estInclus = false;

                foreach (Repository_JSONDTO repoGitHub in repositoriesOnGithub)
                {
                    if (repo.FullName == repoGitHub.full_name)
                    {
                        estInclus = true;
                    }
                }

                if (!estInclus)
                {
                    repositoriesDesactivate.Add(repo);
                }
            }

            repositoriesDesactivate.ForEach(r => this._depotRepository.DeleteRepository(r));
        }

        public void ValidateAllRepositoriesHasBranch()
        {
            List<Organisation> organisations = this._depotOrganisation.GetOrganisations();

            if (organisations == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ValidateAllRepositories - la liste organisations assignée à partir de this._depotOrganisation.GetOrganisations(); est null",
                    0));
            }

            foreach (Organisation o in organisations)
            {
                List<Repository> repositoriesInDB = this._depotRepository.GetRepositoriesFromOrganisationName(o.Name);

                foreach (Repository r in repositoriesInDB)
                {
                    this.ValidateOneRepositoryHasBranch(o.Name, r);
                }
            }
        }

        private void ValidateOneRepositoryHasBranch(string p_organisationName, Repository p_repository)
        {
            List<Branch_JSONDTO>? branches =
                _githubApiAction.GetRepositoryBranchesGithub(p_organisationName, p_repository.Name);
            
            if (branches is null || branches.Count == 0)
            {
                _depotRepository.DeleteRepository(p_repository);
            }
            else if (ValidateMainBranchExistsFromBranchList(branches))
            {
                _depotRepository.ReactivateRepository(p_repository);
            }
            else
            {
                // flag Revalider si on fait autre chose s'il manque la branche main
                _depotRepository.DeleteRepository(p_repository);
            }
        }

        public void ValidateAllRepositoriesByAssignementHasBranch(string p_organisationName, string p_assignementName)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ValidateAllRepositoriesByAssignementHasBranch - p_organisationName est vide",
                    0));
            }
            if (string.IsNullOrWhiteSpace(p_assignementName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ValidateAllRepositoriesByAssignementHasBranch - p_assignementName assignée est vide",
                    0));
            }

           
            List<Repository> repositoriesInDB = this._depotRepository.GetRepositoriesFromAssignementName(p_organisationName, p_assignementName);

           
            foreach (Repository r in repositoriesInDB)
            {
                this.ValidateOneRepositoryHasBranch(p_organisationName, r);
            }
            
        }

        #endregion

        #region Private Submethods

        private Dictionary<string, int> GetStudentDictionary(List<Repository> p_repositories, string p_assignment)
        {
            Dictionary<string, int> studentDictionary = new Dictionary<string, int>();

            string substringContainingTheAssingnmentName = p_assignment + '-';

            foreach (Repository repository in p_repositories)
            {
                string repositoryNameLessAssignmentName = repository.Name.ToLower()
                    .Replace(substringContainingTheAssingnmentName.ToLower(), "");

                string studentUsername = repositoryNameLessAssignmentName;
                studentDictionary[studentUsername] = 0;
            }

            return studentDictionary;
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

        private List<Repository> GetStudentsRepositoriesForAssignment(List<Repository> p_repositories,
            Assignment p_assignment)
        {
            List<Repository> repositoriesToBeAddedToPeerReview = new List<Repository>();
            string substringContainingTheAssingnmentName = p_assignment.Name + '-';

            for (int i = 0; i < p_repositories.Count; i++)
            {
                if (p_repositories[i].Name.ToLower().Contains(substringContainingTheAssingnmentName.ToLower()))
                {
                    string repositoryNameLessAssignmentName =
                        p_repositories[i].Name.ToLower().Replace(substringContainingTheAssingnmentName.ToLower(), "");

                    for (int j = 0; j < this._activeClassroom.Students.Count; j++)
                    {
                        if (repositoryNameLessAssignmentName.ToLower() ==
                            this._activeClassroom.Students[j].Username.ToLower())
                        {
                            repositoriesToBeAddedToPeerReview.Add(p_repositories[i]);
                            break;
                        }
                    }
                }
            }

            return repositoriesToBeAddedToPeerReview;
        }

        private Branch_JSONDTO GetBranchFromBranchListByType(List<Branch_JSONDTO> p_branches, string p_type)
        {
            foreach (Branch_JSONDTO branch in p_branches)
            {
                string[] branchName = branch.reference.Split("/");

                if (branchName[2] == p_type)
                {
                    return branch;
                }
            }

            return null;
        }


        private bool ValidateMainBranchExistsFromBranchList(List<Branch_JSONDTO> branchesResult)
        {
            bool mainExists = false;

            foreach (Branch_JSONDTO branch in branchesResult)
            {
                string[] branchName = branch.reference.Split("/");
                if (branchName[2] == "main")
                {
                    mainExists = true;
                }
            }

            return mainExists;
        }

        private Branch_JSONDTO GetBranchFromBranchesPerBranchType(string p_organisationName, string p_repositoryName,
            string p_branchType)
        {
            List<Branch_JSONDTO> getAllAvailableBranchesInRepository =
                this._githubApiAction.GetRepositoryBranchesGithub(p_organisationName, p_repositoryName);

            if (getAllAvailableBranchesInRepository.Count <= 0)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - GetBranchFromBranchesPerRepository - la liste branchesResult assignée à partir de this._githubApiAction.GetRepositoryBranchesGithub(p_organisationName, p_repositoryName); est vide",
                    0));

                throw new ArgumentNullException($"Branch does not exist or wrong name was entered");
            }

            return GetBranchFromBranchListByType(getAllAvailableBranchesInRepository, p_branchType);
        }

        private void DeleteFilesAndDirectoriesForDownloads()
        {
            if (File.Exists("ZippedRepos.zip"))
                File.Delete("ZippedRepos.zip");

            if (File.Exists("repo.zip"))
                File.Delete("repo.zip");

            if (Directory.Exists("ZippedRepos"))
                Directory.Delete("ZippedRepos", true);
        }

        private void DownloadRepositoriesToDirectory(List<Repository> p_repositories)
        {
            foreach (Repository repository in p_repositories)
            {
                var download = _githubApiAction.DownloadRepository(repository.Organisation.Name, repository.Name);
                Stream stream = download.Content.ReadAsStream();

                using (var fileStream = File.Create("repo.zip"))
                {
                    stream.CopyTo(fileStream);
                }

                ZipFile.ExtractToDirectory("repo.zip", $"ZippedRepos/{repository.Name}");

                if (File.Exists("repo.zip"))
                    File.Delete("repo.zip");
            }
        }

        private string DownloadRepositoryToFile(Repository p_repository)
        {
            string activeOrganisationName = this._activeClassroom.OrganisationName;
            if (File.Exists("repo.zip"))
            {
                File.Delete("repo.zip");
            }

            var download = _githubApiAction.DownloadRepository(p_repository.Organisation.Name, p_repository.Name);
            Stream stream = download.Content.ReadAsStream();

            using (var fileStream = File.Create("repo.zip"))
            {
                stream.CopyTo(fileStream);
                string path = Path.GetFullPath("repo.zip");

                return path;
            }
        }

        private string GetZipFromReposDirectory()
        {
            File.Delete("ZippedRepos.zip");
            ZipFile.CreateFromDirectory("ZippedRepos", "ZippedRepos.zip");
            Directory.Delete("ZippedRepos", true);

            return Path.GetFullPath("ZippedRepos.zip");
        }

        private List<Student> GetStudentsWithoutRepositoryFromAssignment(List<Repository> p_repositories)
        {
            List<Student> students = new List<Student>();

            foreach (Student student in this._activeClassroom.Students)
            {
                bool hasRepository = false;

                foreach (Repository reopsitory in p_repositories)
                {
                    if (reopsitory.Name.ToLower().Contains(student.Username.ToLower()))
                    {
                        hasRepository = true;
                        break;
                    }
                }

                if (!hasRepository)
                {
                    students.Add(student);
                }
            }

            return students;
        }

        #endregion
    }
}