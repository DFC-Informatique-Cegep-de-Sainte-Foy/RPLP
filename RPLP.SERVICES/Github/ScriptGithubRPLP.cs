using Newtonsoft.Json;
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

namespace RPLP.SERVICES.Github
{
    public class ScriptGithubRPLP
    {
        private readonly IDepotClassroom _depotClassroom;
        private readonly IDepotRepository _depotRepository;
        private readonly IDepotOrganisation _depotOrganisation;
        private readonly GithubApiAction _githubApiAction;
        private Classroom _activeClassroom;

        public ScriptGithubRPLP(IDepotClassroom p_depotClassroom, IDepotRepository p_depotRepository,
            IDepotOrganisation p_depotOrganisation, string p_token)
        {
            if (p_depotClassroom == null)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - Constructeur - p_depotClassroom passé en paramètre est null", 0));
            }

            if (p_depotRepository == null)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - Constructeur - p_depotRepository passé en paramètre est null", 0));
            }

            if (p_depotOrganisation == null)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - Constructeur - p_depotOrganisation passé en paramètre est null", 0));
            }

            if (string.IsNullOrWhiteSpace(p_token))
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - Constructeur - p_token passé en paramètre est vide", 0));
            }

            this._depotClassroom = p_depotClassroom;
            this._depotRepository = p_depotRepository;
            this._depotOrganisation = p_depotOrganisation;
            this._githubApiAction = new GithubApiAction(p_token);
        }

        private void CreateOrUpdateActiveClassroom(string p_organisationName, string p_classRoomName,
            string p_assignmentName)
        {
            if (this._activeClassroom is null)
                this._activeClassroom = new Classroom();

            // Mettre a jour le classroom active
            this._activeClassroom.Name = p_classRoomName;
            this._activeClassroom.OrganisationName = p_organisationName;
            this._activeClassroom.Teachers = _depotClassroom.GetTeachersByClassroomName(this._activeClassroom.Name);
            this._activeClassroom.Assignments = _depotClassroom.GetAssignmentsByClassroomName(this._activeClassroom.Name);
            this._activeClassroom.Students = _depotClassroom.GetStudentsByClassroomName(this._activeClassroom.Name);
            this._activeClassroom.UpdateActiveAssignment(p_assignmentName);
        }

        public void ScriptAssignStudentToAssignmentReview(string p_organisationName, string p_classRoomName,
            string p_assignmentName, int p_reviewsPerRepository)
        {
            if (p_reviewsPerRepository <= 0)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentOutOfRangeException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptAssignStudentToAssignmentReview - p_reviewsPerRepository passé en paramètre est hors des limites",
                    0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptAssignStudentToAssignmentReview - p_organisationName passé en paramètre est vide",
                    0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            if (string.IsNullOrWhiteSpace(p_classRoomName))
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptAssignStudentToAssignmentReview - p_classRoomName passé en paramètre est vide",
                    0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            if (string.IsNullOrWhiteSpace(p_assignmentName))
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptAssignStudentToAssignmentReview - p_token passé en paramètre est vide",
                    0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            CreateOrUpdateActiveClassroom(p_organisationName, p_classRoomName, p_assignmentName);

            RPLP.JOURNALISATION.Logging.Journal(new Log(
                $"ScriptGithubRPLP - List<Student> students : {this._activeClassroom.Students.Count} : {p_reviewsPerRepository + 1})"));
            if (this._activeClassroom.Students.Count < p_reviewsPerRepository + 1)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptAssignStudentToAssignmentReview - La liste students assignée à partir de la méthode _depotClassroom.GetStudentsByClassroomName(p_classRoomName) n'est pas conforme selon la demande",
                    0));

                throw new ArgumentException("Number of students inferior to number of reviews");
            }

            RPLP.JOURNALISATION.Logging.Journal(
                new Log($"ScriptGithubRPLP - Avant List<Repository> repositoriesToAssign)"));

            List<Repository> repositoriesToAssign = getRepositoriesToAssign(p_assignmentName);
            RPLP.JOURNALISATION.Logging.Journal(
                new Log($"ScriptGithubRPLP - Après List<Repository> repositoriesToAssign"));

            if (repositoriesToAssign == null)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptAssignStudentToAssignmentReview - La liste repositoriesToAssign assignée à partir de la méthode getRepositoriesToAssign(p_organisationName, p_classRoomName, p_assignmentName, students) n'est pas conforme selon la demande",
                    0));

                throw new ArgumentNullException($"No repositories to assign in {p_classRoomName}");
            }

            Dictionary<string, int> studentDictionary = GetStudentDictionary(repositoriesToAssign, p_assignmentName);
            RPLP.JOURNALISATION.Logging.Journal(new Log(
                $"ScriptGithubRPLP - ScriptAssignStudentToAssignmentReview(string p_organisationName:{p_organisationName}, string p_classRoomName:{p_classRoomName}, string p_assignmentName:{p_assignmentName}, int p_reviewsPerRepository:{p_reviewsPerRepository} - studentDictionary:{studentDictionary.Count})"));

            foreach (Repository repository in repositoriesToAssign)
            {
                Thread.Sleep(20000); // arbitrary sleep to avoid forbiden 403 from GH - a revenir
                prepareRepositoryAndCreatePullRequest(p_organisationName, repository.Name, studentDictionary, p_reviewsPerRepository);
            }
            // Passer la liste de Repos a la classe Allocations 
            // Envoyer la liste de allocations a bd
            // Prepare Repository and Create pull request
            // -    pour l'instant c'est fait par repo
            // -    c'est a changer par une liste d'allocation
            // AssignStudentReviewersToPullRequests
            // -    c'est aussi a changer pour une liste d'allocation
        }

        public void ScriptRemoveStudentCollaboratorsFromAssignment(string p_organisationName, string p_classRoomName,
            string p_assignmentName)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptRemoveStudentCollaboratorsFromAssignment - p_organisationName passé en paramètre est vide",
                    0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            if (string.IsNullOrWhiteSpace(p_classRoomName))
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptRemoveStudentCollaboratorsFromAssignment - p_classRoomName passé en paramètre est vide",
                    0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            if (string.IsNullOrWhiteSpace(p_assignmentName))
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptRemoveStudentCollaboratorsFromAssignment - p_assignmentName passé en paramètre est vide",
                    0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            CreateOrUpdateActiveClassroom(p_organisationName, p_classRoomName, p_assignmentName);

            List<Repository> repositoriesToAssign = getRepositoriesToAssign(p_assignmentName);

            if (repositoriesToAssign == null)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
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
                    RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
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

        private void prepareRepositoryAndCreatePullRequest(string p_organisationName, string p_repositoryName,
            Dictionary<string, int> p_studentDictionary, int p_reviewsPerRepository)
        {
            Branch_JSONDTO branchDTO = new Branch_JSONDTO();
            RPLP.JOURNALISATION.Logging.Journal(new Log(
                $"ScriptGithubRPLP - prepareRepositoryAndCreatePullRequest(string p_organisationName: {p_organisationName}," +
                $" string p_repositoryName:{p_repositoryName}, Dictionary<string, int> p_studentDictionary: {p_studentDictionary.Count}, " +
                $"int p_reviewsPerRepository: {p_reviewsPerRepository})"));

            List<Branch_JSONDTO> branchesResult = this._githubApiAction.GetRepositoryBranchesGithub(p_organisationName, p_repositoryName);
            RPLP.JOURNALISATION.Logging.Journal(new Log($"ScriptGithubRPLP - branchesResult: {branchesResult.Count})"));

            if (branchesResult == null)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - prepareRepositoryAndCreatePullRequest - la liste branchesResult assignée à partir de la méthode this._githubApiAction.GetRepositoryBranchesGithub(p_organisationName, p_repositoryName)  est null",
                    0));

                throw new ArgumentNullException($"Branch does not exist or wrong name was entered");
            }

            branchDTO = GetFeedbackBranchFromBranchList(branchesResult);
            RPLP.JOURNALISATION.Logging.Journal(new Log($"ScriptGithubRPLP - branchDTO: {branchDTO.reference})"));

            AssignStudentReviewersToPullRequests(p_studentDictionary, p_organisationName, p_repositoryName,
                p_reviewsPerRepository, branchDTO);
        }

        private void createPullRequestAndAssignUser(string p_organisationName, string p_repositoryName, string p_sha,
            string p_username)
        {
            string newBranchName = $"Feedback-{p_username}";

            string resultCreateBranch =
                this._githubApiAction.CreateNewBranchForFeedbackGitHub(p_organisationName, p_repositoryName, p_sha,
                    newBranchName);
            RPLP.JOURNALISATION.Logging.Journal(new Log(
                $"ScriptGithubRPLP - createPullRequestAndAssignUser - resultCreateBranch: {resultCreateBranch} - p_organisationName:{p_organisationName} - p_repositoryName:{p_repositoryName} - p_username: {p_username}"));
            if (resultCreateBranch != "Created")
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - createPullRequestAndAssignUser - la variable resultCreateBranch retourne que la branche n'as pas été créée à partir de la méthode this._githubApiAction.CreateNewBranchForFeedbackGitHub(p_organisationName, p_repositoryName, p_sha, newBranchName); ",
                    0));

                throw new ArgumentException($"Branch not created in {p_repositoryName}");
            }

            string resultCreatePR = this._githubApiAction.CreateNewPullRequestFeedbackGitHub(p_organisationName,
                p_repositoryName, newBranchName, newBranchName.ToLower(),
                "Voici où vous devez mettre vos commentaires");
            RPLP.JOURNALISATION.Logging.Journal(
                new Log($"ScriptGithubRPLP - createPullRequestAndAssignUser - resultCreatePR: {resultCreatePR}"));

            if (resultCreatePR != "Created")
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - createPullRequestAndAssignUser - la variable resultCreatePR retourne que la requête de tirage n'as pas été créée à partir de la méthode this._githubApiAction.CreateNewPullRequestFeedbackGitHub(p_organisationName, p_repositoryName, newBranchName, newBranchName.ToLower(), \"Voici où vous devez mettre vos commentaires\"); ",
                    0));

                throw new ArgumentException($"PullRequest not created in {p_repositoryName}");
            }

            string resultAddStudent =
                this._githubApiAction.AddStudentAsCollaboratorToPeerRepositoryGithub(p_organisationName,
                    p_repositoryName, p_username);
            RPLP.JOURNALISATION.Logging.Journal(
                new Log($"ScriptGithubRPLP - createPullRequestAndAssignUser - resultAddStudent: {resultAddStudent}"));

            if (resultAddStudent != "Created")
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - createPullRequestAndAssignUser - la variable resultAddStudent retourne que l'utilisateur n'as pas été créée à partir de la méthode his._githubApiAction.CreateNewPullRequestFeedbackGitHub(p_organisationName, p_repositoryName, newBranchName, newBranchName.ToLower(), \"Voici où vous devez mettre vos commentaires\"); ",
                    0));

                throw new ArgumentException($"Student not added in {p_repositoryName}");
            }
        }

        private List<Repository> getRepositoriesToAssign(string p_assignmentName)
        {
            RPLP.JOURNALISATION.Logging.Journal(new Log(
                $"ScriptGithubRPLP - List<Assignment> assignmentsResult:{this._activeClassroom.Assignments.Count})"));
            if (this._activeClassroom.Assignments.Count < 1)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - getRepositoriesToAssign - la liste assignmentsResult retourne qu'il n'y pas d'assignement à partir de la méthode _depotClassroom.GetAssignmentsByClassroomName(p_classRoomName); ",
                    0));

                throw new ArgumentException($"No assignment in {this._activeClassroom.Name}");
            }
            

            RPLP.JOURNALISATION.Logging.Journal(
                new Log($"ScriptGithubRPLP - Assignment assignment:{this._activeClassroom.ActiveAssignment})"));
            if (this._activeClassroom.ActiveAssignment == null)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - getRepositoriesToAssign - la variable assignment assignée à partir de la méthode assignmentsResult.SingleOrDefault(assignment => assignment.Name == p_assignmentName); est null ",
                    0));
                throw new ArgumentException($"no assignment with name {p_assignmentName}");
            }

            List<Repository> repositoriesToThisAssignment = new List<Repository>();

            List<Repository> repositoriesFromDBForActiveClassroom =
                this._depotRepository.GetRepositoriesFromOrganisationName(this._activeClassroom.OrganisationName);

            RPLP.JOURNALISATION.Logging.Journal(new Log(
                $"ScriptGithubRPLP - List<Repository> repositoriesResult:{repositoriesFromDBForActiveClassroom.Count})"));
            repositoriesToThisAssignment =
                GetStudentsRepositoriesForAssignment(repositoriesFromDBForActiveClassroom, this._activeClassroom.ActiveAssignment);

            RPLP.JOURNALISATION.Logging.Journal(new Log(
                $"ScriptGithubRPLP - getRepositoriesToAssign(string p_organisationName:" +
                $"{this._activeClassroom.OrganisationName}, string p_classRoomName:{this._activeClassroom.Name}, string p_assignmentName:" +
                $"{p_assignmentName}, List<Student> p_students:{this._activeClassroom.Students.Count} - repositoriesResult:" +
                $"{repositoriesFromDBForActiveClassroom.Count} - repositories:{repositoriesToThisAssignment.Count})"));

            return repositoriesToThisAssignment;
        }

        public void ScriptAssignTeacherToAssignmentReview(string p_organisationName, string p_classRoomName,
            string p_assignmentName, string teacherUsername)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptAssignTeacherToAssignmentReview - p_organisationName passé en paramètre est vide",
                    0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            if (string.IsNullOrWhiteSpace(p_classRoomName))
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptAssignTeacherToAssignmentReview - p_classRoomName passé en paramètre est vide",
                    0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            if (string.IsNullOrWhiteSpace(p_assignmentName))
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptAssignTeacherToAssignmentReview - p_assignmentName passé en paramètre est vide",
                    0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            if (string.IsNullOrWhiteSpace(teacherUsername))
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptAssignTeacherToAssignmentReview - teacherUsername passé en paramètre est vide",
                    0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            CreateOrUpdateActiveClassroom(p_organisationName, p_classRoomName, p_assignmentName);

            if (this._activeClassroom.Students.Count < 1)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptAssignTeacherToAssignmentReview - la liste students assignée à partir de la méthode _depotClassroom.GetStudentsByClassroomName(p_classRoomName); est vide.",
                    0));

                throw new ArgumentException("Number of students cannot be less than one");
            }

            if (this._activeClassroom.Teachers.Count < 1)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptAssignTeacherToAssignmentReview -  la liste teachers assignée à partir de la méthode _depotClassroom.GetTeachersByClassroomName(p_classRoomName); est vide.",
                    0));

                throw new ArgumentException("Number of teachers cannot be less than one");
            }

            List<Repository> repositoriesToAssign = getRepositoriesToAssign(p_assignmentName);

            if (repositoriesToAssign == null)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptAssignTeacherToAssignmentReview - la liste repositoriesToAssign assignée à partir de getRepositoriesToAssign(p_organisationName, p_classRoomName, p_assignmentName, students); est null",
                    0));

                throw new ArgumentNullException($"No repositories to assign in {p_classRoomName}");
            }

            foreach (Repository repository in repositoriesToAssign)
            {
                createPullRequestForTeacher(p_organisationName, repository.Name, "FichierTexte.txt", "FeedbackTeacher",
                    "RmljaGllciB0ZXh0ZSBwb3VyIGNyw6nDqSBQUg==", teacherUsername);
            }
        }

        private void createPullRequestForTeacher(string p_organisationName, string p_repositoryName,
            string p_newFileName, string p_message, string p_content, string teacherUsername)
        {
            Branch_JSONDTO branchDTO = new Branch_JSONDTO();

            List<Branch_JSONDTO> branchesResult =
                this._githubApiAction.GetRepositoryBranchesGithub(p_organisationName, p_repositoryName);

            if (branchesResult.Count <= 0)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - createPullRequestForTeacher - la liste branchesResult assignée à partir de this._githubApiAction.GetRepositoryBranchesGithub(p_organisationName, p_repositoryName); est vide",
                    0));

                throw new ArgumentNullException($"Branch does not exist or wrong name was entered");
            }

            branchDTO = GetMainBranchFromBranchList(branchesResult);

            CreatePullRequestAndAssignTeacher(p_organisationName, p_repositoryName, branchDTO.gitObject.sha,
                p_newFileName, p_message, p_content, teacherUsername);
        }

        private void CreatePullRequestAndAssignTeacher(string p_organisationName, string p_repositoryName, string p_sha,
            string p_newFileName, string p_message, string p_content, string teacherUsername)
        {
            string newBranchName = $"feedback-{teacherUsername}";

            string resultCreateBranch =
                this._githubApiAction.CreateNewBranchForFeedbackGitHub(p_organisationName, p_repositoryName, p_sha,
                    newBranchName);

            if (resultCreateBranch != "Created")
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - CreatePullRequestAndAssignTeacher - la variable resultCreateBranch indique que la branche n'as pas été créée à partir de la méthode  this._githubApiAction.CreateNewBranchForFeedbackGitHub(p_organisationName, p_repositoryName, p_sha, newBranchName);",
                    0));

                throw new ArgumentException($"Branch not created in {p_repositoryName}");
            }

            this._githubApiAction.AddFileToContentsGitHub(p_organisationName, p_repositoryName, newBranchName,
                p_newFileName, p_message, p_content);

            string resultCreatePR = this._githubApiAction.CreateNewPullRequestFeedbackGitHub(p_organisationName,
                p_repositoryName, newBranchName, "Feedback", "Voici où vous devez mettre vos commentaires");

            if (resultCreatePR != "Created")
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - CreatePullRequestAndAssignTeacher - la variable resultCreatePR indique que la requête de tirage n'as pas été créée à partir de la méthode  this._githubApiAction.CreateNewPullRequestFeedbackGitHub(p_organisationName, p_repositoryName, newBranchName, \"Feedback\", \"Voici où vous devez mettre vos commentaires\");",
                    0));

                throw new ArgumentException($"PullRequest not created in {p_repositoryName}");
            }
        }

        public string ScriptDownloadAllRepositoriesForAssignment(string p_organisationName, string p_classRoomName,
            string p_assignmentName)
        {
            string directoryToZipName = "ZippedRepos";

            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptDownloadAllRepositoriesForAssignment - p_organisationName passé en paramètre est vide",
                    0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            if (string.IsNullOrWhiteSpace(p_classRoomName))
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptDownloadAllRepositoriesForAssignment - p_classRoomName passé en paramètre est vide",
                    0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            if (string.IsNullOrWhiteSpace(p_assignmentName))
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptDownloadAllRepositoriesForAssignment - p_assignmentName passé en paramètre est vide",
                    0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            CreateOrUpdateActiveClassroom(p_organisationName, p_classRoomName, p_assignmentName);

            if (this._activeClassroom.Students.Count < 1)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptDownloadAllRepositoriesForAssignment - la liste students assignée à partir de _depotClassroom.GetStudentsByClassroomName(p_classRoomName); est vide",
                    0));

                throw new ArgumentException("Number of students cannot be less than one");
            }

            if (this._activeClassroom.Assignments.Count < 1)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptDownloadAllRepositoriesForAssignment - la liste assignmentsResult assignée à partir de _depotClassroom.GetAssignmentsByClassroomName(p_classRoomName); est vide",
                    0));


                throw new ArgumentException($"No assignment in {p_classRoomName}");
            }

            Assignment assignmentToReview =
                this._activeClassroom.Assignments.SingleOrDefault(assignment => assignment.Name == p_assignmentName);

            if (assignmentToReview == null)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
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
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptDownloadOneRepositoryForAssignment - p_organisationName passé en paramètre est vide",
                    0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            if (string.IsNullOrWhiteSpace(p_classRoomName))
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptDownloadOneRepositoryForAssignment - p_classRoomName passé en paramètre est vide",
                    0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            if (string.IsNullOrWhiteSpace(p_assignmentName))
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptDownloadOneRepositoryForAssignment - p_assignmentName passé en paramètre est vide",
                    0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            CreateOrUpdateActiveClassroom(p_organisationName, p_classRoomName, p_assignmentName);

            if (this._activeClassroom.Students.Count < 1)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptDownloadOneRepositoryForAssignment - la liste students assignée à partir de _depotClassroom.GetStudentsByClassroomName(p_classRoomName); est vide",
                    0));


                throw new ArgumentException("Number of students cannot be less than one");
            }

            if (this._activeClassroom.Assignments.Count < 1)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - ScriptDownloadOneRepositoryForAssignment - la liste assignmentsResult assignée à partir de _depotClassroom.GetAssignmentsByClassroomName(p_classRoomName);est vide",
                    0));


                throw new ArgumentException($"No assignment in {p_classRoomName}");
            }

            Assignment assignmentToReview =
                this._activeClassroom.Assignments.SingleOrDefault(assignment => assignment.Name == p_assignmentName);

            if (assignmentToReview == null)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
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
            List<Repository> repositories = this.ReturnMissingRepositories();

            if (repositories.Count == null)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "ScriptGithubRPLP - EnsureOrganisationRepositoriesAreInDB - la liste repositories assignée à partir de this.ReturnMissingRepositories(); est null",
                    0));
            }

            repositories.ForEach(r => this._depotRepository.UpsertRepository(r));
        }

        private List<Repository> ReturnMissingRepositories()
        {
            List<Repository> repositoriesToAdd = new List<Repository>();
            List<Organisation> organisations = this._depotOrganisation.GetOrganisations();

            if (organisations == null)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(),
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

                repositoriesToAdd.AddRange(repositoriesOnGithub
                    .Where(ghRepo =>
                        repositoriesInDB.FirstOrDefault(dbRepo => dbRepo.FullName == ghRepo.full_name) == null)
                    .Select(r => new Repository()
                    {
                        FullName = r.full_name,
                        Name = r.name,
                        OrganisationName = r.full_name.Split('/')[0]
                    }));
            }

            return repositoriesToAdd;
        }

        #endregion

        #region Private Submethods

        private Dictionary<string, int> GetStudentDictionary(List<Repository> p_repositories, string p_assignment)
        {
            Dictionary<string, int> studentDictionary = new Dictionary<string, int>();
            RPLP.JOURNALISATION.Logging.Journal(new Log(
                $"ScriptGithubRPLP - GetStudentDictionary(List<Repository> p_repositories: {p_repositories.Count}) studentDictionary: {studentDictionary.Count}"));

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

        private Branch_JSONDTO GetFeedbackBranchFromBranchList(List<Branch_JSONDTO> p_branches)
        {
            Branch_JSONDTO feedbackBranch = new Branch_JSONDTO();

            foreach (Branch_JSONDTO branch in p_branches)
            {
                string[] branchName = branch.reference.Split("/");

                if (branchName[2] == "feedback")
                {
                    feedbackBranch = branch;
                    break;
                }
            }

            return feedbackBranch;
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

        private void AssignStudentReviewersToPullRequests(Dictionary<string, int> p_studentDictionary,
            string p_organisationName, string p_repositoryName, int p_reviewsPerRepository, Branch_JSONDTO branchDTO)
        {
            int numberStudentAdded = 0;
            string substringContainingTheAssingnmentName = this._activeClassroom.ActiveAssignment.Name + '-';
            string repositoryNameLessAssignmentName = p_repositoryName.ToLower()
                .Replace(substringContainingTheAssingnmentName.ToLower(), "");

            RPLP.JOURNALISATION.Logging.Journal(new Log(
                $"ScriptGitHubRPLP - AssignStudentReviewersToPullRequests(Dictionary<string, int> p_studentDictionary: {p_studentDictionary.Count}, string p_organisationName: {p_organisationName},string p_repositoryName: {p_repositoryName}, int p_reviewsPerRepository: {p_reviewsPerRepository}, Branch_JSONDTO branchDTO: {branchDTO.reference})"));

            List<string> usernamesLessCurrentUsernameRepo = p_studentDictionary.Keys
                .Where(key => key.ToLower() != repositoryNameLessAssignmentName.ToLower()
                              && p_studentDictionary[key] < p_reviewsPerRepository).ToList();

            List<string> reviewersAddedToThisRepo = new List<string>();

            do
            {
                ShuffleListInPlace(usernamesLessCurrentUsernameRepo);
                string username = usernamesLessCurrentUsernameRepo[0];

                if (!reviewersAddedToThisRepo.Contains(username)
                    && p_studentDictionary[username] < p_reviewsPerRepository)
                {
                    reviewersAddedToThisRepo.Add(username);
                    usernamesLessCurrentUsernameRepo.Remove(username);
                    RPLP.JOURNALISATION.Logging.Journal(new Log(
                        $"ScriptGitHubRPLP - AssignStudentReviewersToPullRequests:: -repository name: {p_repositoryName} - reviewer: {username}"));

                    p_studentDictionary[username]++;
                    numberStudentAdded++;

                    createPullRequestAndAssignUser(p_organisationName, p_repositoryName, branchDTO.gitObject.sha, username);
                }
            } while (numberStudentAdded < p_reviewsPerRepository);
        }

        private List<Repository> GetStudentsRepositoriesForAssignment(List<Repository> p_repositories,
            Assignment p_assignment)
        {
            List<Repository> repositoriesToBeAddedToPeerReview = new List<Repository>();
            string substringContainingTheAssingnmentName = p_assignment.Name + '-';

            for (int i = 0; i < p_repositories.Count; i++)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(
                    $"ScriptGitHubRPLP - GetStudentsRepositoriesForAssignment - repository:{p_repositories[i].Name}:{p_repositories.Count} - splitRepository[0]:{substringContainingTheAssingnmentName} assignmentName:{p_assignment.Name} Student:{this._activeClassroom.Students.Count}"));
                if (p_repositories[i].Name.ToLower().Contains(substringContainingTheAssingnmentName.ToLower()))
                {
                    string repositoryNameLessAssignmentName =
                        p_repositories[i].Name.ToLower().Replace(substringContainingTheAssingnmentName.ToLower(), "");

                    for (int j = 0; j < this._activeClassroom.Students.Count; j++)
                    {
                        RPLP.JOURNALISATION.Logging.Journal(new Log(
                            $"ScriptGitHubRPLP - GetStudentsRepositoriesForAssignment - {repositoryNameLessAssignmentName}:{this._activeClassroom.Students[j].Username}"));

                        if (repositoryNameLessAssignmentName.ToLower() ==
                            this._activeClassroom.Students[j].Username.ToLower())
                        {
                            RPLP.JOURNALISATION.Logging.Journal(new Log(
                                $"ScriptGitHubRPLP - GetStudentsRepositoriesForAssignment - repository:{p_repositories[i].Name} - splitRepository[1]:{repositoryNameLessAssignmentName} student:{this._activeClassroom.Students[j].Username}"));
                            repositoriesToBeAddedToPeerReview.Add(p_repositories[i]);
                            break;
                        }
                    }
                }
            }

            RPLP.JOURNALISATION.Logging.Journal(new Log(
                $"ScriptGitHubRPLP - GetStudentsRepositoriesForAssignment(List<Repository> p_repositories, List<Student> p_students, string assignmentName) repositories:{repositoriesToBeAddedToPeerReview.Count}"));

            return repositoriesToBeAddedToPeerReview;
        }

        private Branch_JSONDTO GetMainBranchFromBranchList(List<Branch_JSONDTO> branchesResult)
        {
            Branch_JSONDTO branchDTO = new Branch_JSONDTO();

            foreach (Branch_JSONDTO branch in branchesResult)
            {
                string[] branchName = branch.reference.Split("/");

                if (branchName[2] == "main")
                {
                    branchDTO = branch;
                    break;
                }
            }

            return branchDTO;
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
                var download = _githubApiAction.DownloadRepository(repository.OrganisationName, repository.Name);
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
            if (File.Exists("repo.zip"))
            {
                File.Delete("repo.zip");
            }

            var download = _githubApiAction.DownloadRepository(p_repository.OrganisationName, p_repository.Name);
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

        #endregion
    }
}