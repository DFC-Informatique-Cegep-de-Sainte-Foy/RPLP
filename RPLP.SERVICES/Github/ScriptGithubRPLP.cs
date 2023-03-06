using Newtonsoft.Json;
using RPLP.DAL.DTO.Json;
using RPLP.DAL.DTO.Sql;
using RPLP.ENTITES;
using RPLP.JOURNALISATION;
using RPLP.SERVICES.InterfacesDepots;
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

        public ScriptGithubRPLP(IDepotClassroom p_depotClassroom, IDepotRepository p_depotRepository, IDepotOrganisation p_depotOrganisation, string p_token)
        {
            if (p_depotClassroom == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                            "ScriptGithubRPLP - Constructeur - p_depotClassroom passé en paramètre est null", 0));
            }

            if (p_depotRepository == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                            "ScriptGithubRPLP - Constructeur - p_depotRepository passé en paramètre est null", 0));
            }

            if (p_depotOrganisation == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                            "ScriptGithubRPLP - Constructeur - p_depotOrganisation passé en paramètre est null", 0));
            }

            if (string.IsNullOrWhiteSpace(p_token))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                            "ScriptGithubRPLP - Constructeur - p_token passé en paramètre est vide", 0));
            }

            this._depotClassroom = p_depotClassroom;
            this._depotRepository = p_depotRepository;
            this._depotOrganisation = p_depotOrganisation;
            this._githubApiAction = new GithubApiAction(p_token);
        }

        public void ScriptAssignStudentToAssignmentReview(string p_organisationName, string p_classRoomName, string p_assignmentName, int p_reviewsPerRepository)
        {
            if (p_reviewsPerRepository <= 0)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                            "ScriptGithubRPLP - ScriptAssignStudentToAssignmentReview - p_reviewsPerRepository passé en paramètre est hors des limites", 0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            if(string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                            "ScriptGithubRPLP - ScriptAssignStudentToAssignmentReview - p_organisationName passé en paramètre est vide", 0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            if(string.IsNullOrWhiteSpace(p_classRoomName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                            "ScriptGithubRPLP - ScriptAssignStudentToAssignmentReview - p_classRoomName passé en paramètre est vide", 0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            if (string.IsNullOrWhiteSpace(p_assignmentName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                            "ScriptGithubRPLP - ScriptAssignStudentToAssignmentReview - p_token passé en paramètre est vide", 0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            List<Student> students = _depotClassroom.GetStudentsByClassroomName(p_classRoomName);

            if (students.Count < p_reviewsPerRepository + 1)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                            "ScriptGithubRPLP - ScriptAssignStudentToAssignmentReview - La liste students assignée à partir de la méthode _depotClassroom.GetStudentsByClassroomName(p_classRoomName) n'est pas conforme selon la demande", 0));

                throw new ArgumentException("Number of students inferior to number of reviews");
            }


            List<Repository> repositoriesToAssign = getRepositoriesToAssign(p_organisationName, p_classRoomName, p_assignmentName, students);

            Dictionary<string, int> studentDictionary = new Dictionary<string, int>();

            if (repositoriesToAssign == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                            "ScriptGithubRPLP - ScriptAssignStudentToAssignmentReview - La liste repositoriesToAssign assignée à partir de la méthode getRepositoriesToAssign(p_organisationName, p_classRoomName, p_assignmentName, students) n'est pas conforme selon la demande", 0));

                throw new ArgumentNullException($"No repositories to assign in {p_classRoomName}");
            }
                
            studentDictionary = GetStudentDictionary(repositoriesToAssign);

            foreach (Repository repository in repositoriesToAssign)
            {
                prepareRepositoryAndCreatePullRequest(p_organisationName, repository.Name, studentDictionary, p_reviewsPerRepository);
            }
        }

        public void ScriptRemoveStudentCollaboratorsFromAssignment(string p_organisationName, string p_classRoomName, string p_assignmentName)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                           "ScriptGithubRPLP - ScriptRemoveStudentCollaboratorsFromAssignment - p_organisationName passé en paramètre est vide", 0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            if (string.IsNullOrWhiteSpace(p_classRoomName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                           "ScriptGithubRPLP - ScriptRemoveStudentCollaboratorsFromAssignment - p_classRoomName passé en paramètre est vide", 0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            if(string.IsNullOrWhiteSpace(p_assignmentName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                           "ScriptGithubRPLP - ScriptRemoveStudentCollaboratorsFromAssignment - p_assignmentName passé en paramètre est vide", 0));

                throw new ArgumentException("the provided value is incorrect or null");
            }
                

            List<Student> students = _depotClassroom.GetStudentsByClassroomName(p_classRoomName);
            List<Repository> repositoriesToAssign = getRepositoriesToAssign(p_organisationName, p_classRoomName, p_assignmentName, students);

            if(repositoriesToAssign == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "ScriptGithubRPLP - ScriptRemoveStudentCollaboratorsFromAssignment - la liste repositoriesToAssign assignée à partir de la méthode getRepositoriesToAssign(p_organisationName, p_classRoomName, p_assignmentName, students);  est null", 0));
            }

            foreach (Repository repository in repositoriesToAssign)
            {
                List<Collaborator_JSONDTO> collaborator = _githubApiAction.GetCollaboratorFromStudentRepositoryGithub(p_organisationName, repository.Name);

                if(collaborator == null)
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "ScriptGithubRPLP - ScriptRemoveStudentCollaboratorsFromAssignment - la liste collaborator assignée à partir de la méthode _githubApiAction.GetCollaboratorFromStudentRepositoryGithub(p_organisationName, repository.Name);  est null", 0));
                }

                collaborator.ForEach(collaborator =>
                {
                    if (collaborator.role_name == "triage")
                        _githubApiAction.RemoveStudentAsCollaboratorFromPeerRepositoryGithub(p_organisationName, repository.Name, collaborator.login);
                });
            }
        }

        private void prepareRepositoryAndCreatePullRequest(string p_organisationName, string p_repositoryName, Dictionary<string, int> p_studentDictionary, int p_reviewsPerRepository)
        {
            Branch_JSONDTO branchDTO = new Branch_JSONDTO();

            List<Branch_JSONDTO> branchesResult = this._githubApiAction.GetRepositoryBranchesGithub(p_organisationName, p_repositoryName);

            if (branchesResult == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "ScriptGithubRPLP - prepareRepositoryAndCreatePullRequest - la liste branchesResult assignée à partir de la méthode this._githubApiAction.GetRepositoryBranchesGithub(p_organisationName, p_repositoryName)  est null", 0));

                throw new ArgumentNullException($"Branch does not exist or wrong name was entered");
            }

            branchDTO = GetFeedbackBranchFromBranchList(branchesResult);
            AssignStudentReviewersToPullRequests(p_studentDictionary, p_organisationName, p_repositoryName, p_reviewsPerRepository, branchDTO);
        }

        private void createPullRequestAndAssignUser(string p_organisationName, string p_repositoryName, string p_sha, string p_username)
        {
            string newBranchName = $"Feedback-{p_username}";

            string resultCreateBranch = this._githubApiAction.CreateNewBranchForFeedbackGitHub(p_organisationName, p_repositoryName, p_sha, newBranchName);

            if (resultCreateBranch != "Created")
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "ScriptGithubRPLP - createPullRequestAndAssignUser - la variable resultCreateBranch retourne que la branche n'as pas été créée à partir de la méthode this._githubApiAction.CreateNewBranchForFeedbackGitHub(p_organisationName, p_repositoryName, p_sha, newBranchName); ", 0));

                throw new ArgumentException($"Branch not created in {p_repositoryName}");
            }

            string resultCreatePR = this._githubApiAction.CreateNewPullRequestFeedbackGitHub(p_organisationName, p_repositoryName, newBranchName, newBranchName.ToLower(), "Voici où vous devez mettre vos commentaires");
            
            if (resultCreatePR != "Created")
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "ScriptGithubRPLP - createPullRequestAndAssignUser - la variable resultCreatePR retourne que la requête de tirage n'as pas été créée à partir de la méthode his._githubApiAction.CreateNewPullRequestFeedbackGitHub(p_organisationName, p_repositoryName, newBranchName, newBranchName.ToLower(), \"Voici où vous devez mettre vos commentaires\"); ", 0));

                throw new ArgumentException($"PullRequest not created in {p_repositoryName}");
            }
                

            string resultAddStudent = this._githubApiAction.AddStudentAsCollaboratorToPeerRepositoryGithub(p_organisationName, p_repositoryName, p_username);

            if (resultAddStudent != "Created")
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                      "ScriptGithubRPLP - createPullRequestAndAssignUser - la variable resultAddStudent retourne que l'utilisateur n'as pas été créée à partir de la méthode his._githubApiAction.CreateNewPullRequestFeedbackGitHub(p_organisationName, p_repositoryName, newBranchName, newBranchName.ToLower(), \"Voici où vous devez mettre vos commentaires\"); ", 0));

                throw new ArgumentException($"Student not added in {p_repositoryName}");
            }
        }

        private List<Repository> getRepositoriesToAssign(string p_organisationName, string p_classRoomName, string p_assignmentName, List<Student> p_students)
        {
            List<Assignment> assignmentsResult = _depotClassroom.GetAssignmentsByClassroomName(p_classRoomName);

            if (assignmentsResult.Count < 1)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                      "ScriptGithubRPLP - getRepositoriesToAssign - la liste assignmentsResult retourne qu'il n'y pas d'assignement à partir de la méthode _depotClassroom.GetAssignmentsByClassroomName(p_classRoomName); ", 0));

                throw new ArgumentException($"No assignment in {p_classRoomName}");
            }
                
            Assignment assignment = assignmentsResult.SingleOrDefault(assignment => assignment.Name == p_assignmentName);

            if (assignment == null)
            {

                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                      "ScriptGithubRPLP - getRepositoriesToAssign - la variable assignment assignée à partir de la méthode assignmentsResult.SingleOrDefault(assignment => assignment.Name == p_assignmentName); est null ", 0));

                throw new ArgumentException($"no assignment with name {p_assignmentName}");
            }
                
            List<Repository> repositories = new List<Repository>();

            List<Repository> repositoriesResult = this._depotRepository.GetRepositoriesFromOrganisationName(p_organisationName);
            repositories = GetStudentsRepositoriesForAssignment(repositoriesResult, p_students, p_assignmentName);

            return repositories;
        }

        public void ScriptAssignTeacherToAssignmentReview(string p_organisationName, string p_classRoomName, string p_assignmentName, string teacherUsername)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                           "ScriptGithubRPLP - ScriptAssignTeacherToAssignmentReview - p_organisationName passé en paramètre est vide", 0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            if (string.IsNullOrWhiteSpace(p_classRoomName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                           "ScriptGithubRPLP - ScriptAssignTeacherToAssignmentReview - p_classRoomName passé en paramètre est vide", 0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            if (string.IsNullOrWhiteSpace(p_assignmentName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                           "ScriptGithubRPLP - ScriptAssignTeacherToAssignmentReview - p_assignmentName passé en paramètre est vide", 0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            if (string.IsNullOrWhiteSpace(teacherUsername))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                           "ScriptGithubRPLP - ScriptAssignTeacherToAssignmentReview - teacherUsername passé en paramètre est vide", 0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            List<Student> students = _depotClassroom.GetStudentsByClassroomName(p_classRoomName);
            List<Teacher> teachers = _depotClassroom.GetTeachersByClassroomName(p_classRoomName);

            if (students.Count < 1)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                           "ScriptGithubRPLP - ScriptAssignTeacherToAssignmentReview - la liste students assignée à partir de la méthode _depotClassroom.GetStudentsByClassroomName(p_classRoomName); est vide.", 0));

                throw new ArgumentException("Number of students cannot be less than one");
            }
                
            if (teachers.Count < 1)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                           "ScriptGithubRPLP - ScriptAssignTeacherToAssignmentReview -  la liste teachers assignée à partir de la méthode _depotClassroom.GetTeachersByClassroomName(p_classRoomName); est vide.", 0));

                throw new ArgumentException("Number of teachers cannot be less than one");
            }

            List<Repository> repositoriesToAssign = getRepositoriesToAssign(p_organisationName, p_classRoomName, p_assignmentName, students);

            if (repositoriesToAssign == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                           "ScriptGithubRPLP - ScriptAssignTeacherToAssignmentReview - la liste repositoriesToAssign assignée à partir de getRepositoriesToAssign(p_organisationName, p_classRoomName, p_assignmentName, students); est null", 0));

                throw new ArgumentNullException($"No repositories to assign in {p_classRoomName}");
            }

            foreach (Repository repository in repositoriesToAssign)
            {
                createPullRequestForTeacher(p_organisationName, repository.Name, "FichierTexte.txt", "FeedbackTeacher", "RmljaGllciB0ZXh0ZSBwb3VyIGNyw6nDqSBQUg==", teacherUsername);
            }
        }

        private void createPullRequestForTeacher(string p_organisationName, string p_repositoryName, string p_newFileName, string p_message, string p_content, string teacherUsername)
        {
            Branch_JSONDTO branchDTO = new Branch_JSONDTO();

            List<Branch_JSONDTO> branchesResult = this._githubApiAction.GetRepositoryBranchesGithub(p_organisationName, p_repositoryName);

            if (branchesResult.Count <= 0)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                          "ScriptGithubRPLP - createPullRequestForTeacher - la liste branchesResult assignée à partir de this._githubApiAction.GetRepositoryBranchesGithub(p_organisationName, p_repositoryName); est vide", 0));

                throw new ArgumentNullException($"Branch does not exist or wrong name was entered");
            }

            branchDTO = GetMainBranchFromBranchList(branchesResult);

            CreatePullRequestAndAssignTeacher(p_organisationName, p_repositoryName, branchDTO.gitObject.sha, p_newFileName, p_message, p_content, teacherUsername);
        }

        private void CreatePullRequestAndAssignTeacher(string p_organisationName, string p_repositoryName, string p_sha, string p_newFileName, string p_message, string p_content, string teacherUsername)
        {
            string newBranchName = $"feedback-{teacherUsername}";

            string resultCreateBranch = this._githubApiAction.CreateNewBranchForFeedbackGitHub(p_organisationName, p_repositoryName, p_sha, newBranchName);

            if (resultCreateBranch != "Created")
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                         "ScriptGithubRPLP - CreatePullRequestAndAssignTeacher - la variable resultCreateBranch indique que la branche n'as pas été créée à partir de la méthode  this._githubApiAction.CreateNewBranchForFeedbackGitHub(p_organisationName, p_repositoryName, p_sha, newBranchName);", 0));

                throw new ArgumentException($"Branch not created in {p_repositoryName}");
            }
                
            this._githubApiAction.AddFileToContentsGitHub(p_organisationName, p_repositoryName, newBranchName, p_newFileName, p_message, p_content);

            string resultCreatePR = this._githubApiAction.CreateNewPullRequestFeedbackGitHub(p_organisationName, p_repositoryName, newBranchName, "Feedback", "Voici où vous devez mettre vos commentaires");
            
            if (resultCreatePR != "Created")
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                         "ScriptGithubRPLP - CreatePullRequestAndAssignTeacher - la variable resultCreatePR indique que la requête de tirage n'as pas été créée à partir de la méthode  this._githubApiAction.CreateNewPullRequestFeedbackGitHub(p_organisationName, p_repositoryName, newBranchName, \"Feedback\", \"Voici où vous devez mettre vos commentaires\");", 0));

                throw new ArgumentException($"PullRequest not created in {p_repositoryName}");
            }
        }

        public string ScriptDownloadAllRepositoriesForAssignment(string p_organisationName, string p_classRoomName, string p_assignmentName)
        {
            Console.Out.WriteLine($"API - ScriptDownloadAllRepositoriesForAssignment({p_organisationName}, {p_classRoomName}, {p_assignmentName})");

            string directoryToZipName = "ZippedRepos";

            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                           "ScriptGithubRPLP - ScriptDownloadAllRepositoriesForAssignment - p_organisationName passé en paramètre est vide", 0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            if (string.IsNullOrWhiteSpace(p_classRoomName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                           "ScriptGithubRPLP - ScriptDownloadAllRepositoriesForAssignment - p_classRoomName passé en paramètre est vide", 0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            if (string.IsNullOrWhiteSpace(p_assignmentName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                           "ScriptGithubRPLP - ScriptDownloadAllRepositoriesForAssignment - p_assignmentName passé en paramètre est vide", 0));

                throw new ArgumentException("the provided value is incorrect or null");
            }


            List<Student> students = _depotClassroom.GetStudentsByClassroomName(p_classRoomName);

            Console.Out.WriteLine($"students.Count == {students.Count}");

            if (students.Count < 1)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                          "ScriptGithubRPLP - ScriptDownloadAllRepositoriesForAssignment - la liste students assignée à partir de _depotClassroom.GetStudentsByClassroomName(p_classRoomName); est vide", 0));

                throw new ArgumentException("Number of students cannot be less than one");
            }

            List<Assignment> assignmentsResult = _depotClassroom.GetAssignmentsByClassroomName(p_classRoomName);

            Console.Out.WriteLine($"assignmentsResult.Count == {assignmentsResult.Count}");

            if (assignmentsResult.Count < 1)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                          "ScriptGithubRPLP - ScriptDownloadAllRepositoriesForAssignment - la liste assignmentsResult assignée à partir de _depotClassroom.GetAssignmentsByClassroomName(p_classRoomName); est vide", 0));


                throw new ArgumentException($"No assignment in {p_classRoomName}");
            }

            Assignment assignment = assignmentsResult.SingleOrDefault(assignment => assignment.Name == p_assignmentName);

            Console.Out.WriteLine($"assignment is null == {assignment == null}");

            if (assignment == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                         "ScriptGithubRPLP - ScriptDownloadAllRepositoriesForAssignment - la liste assignment assignée à partir de assignmentsResult.SingleOrDefault(assignment => assignment.Name == p_assignmentName); est null", 0));


                throw new ArgumentException($"No assignment with name {p_assignmentName}");
            }

            List<Repository> repositoriesResult = this._depotRepository.GetRepositoriesFromOrganisationName(p_organisationName);
            List<Repository> repositories = GetStudentsRepositoriesForAssignment(repositoriesResult, students, p_assignmentName);

            Console.Out.WriteLine($"repositoriesResult.Count == {repositoriesResult}");
            Console.Out.WriteLine($"repositories.Count == {repositoriesResult}");

            DeleteFilesAndDirectoriesForDownloads();
            Directory.CreateDirectory(directoryToZipName);

            DownloadRepositoriesToDirectory(repositories);
            string path = GetZipFromReposDirectory();

            return path;
        }

        public string ScriptDownloadOneRepositoryForAssignment(string p_organisationName, string p_classRoomName, string p_assignmentName, string p_repositoryName)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                           "ScriptGithubRPLP - ScriptDownloadOneRepositoryForAssignment - p_organisationName passé en paramètre est vide", 0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            if (string.IsNullOrWhiteSpace(p_classRoomName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                           "ScriptGithubRPLP - ScriptDownloadOneRepositoryForAssignment - p_classRoomName passé en paramètre est vide", 0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            if (string.IsNullOrWhiteSpace(p_assignmentName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                           "ScriptGithubRPLP - ScriptDownloadOneRepositoryForAssignment - p_assignmentName passé en paramètre est vide", 0));

                throw new ArgumentException("the provided value is incorrect or null");
            }

            List<Student> students = _depotClassroom.GetStudentsByClassroomName(p_classRoomName);

            if (students.Count < 1)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                         "ScriptGithubRPLP - ScriptDownloadOneRepositoryForAssignment - la liste students assignée à partir de _depotClassroom.GetStudentsByClassroomName(p_classRoomName); est vide", 0));


                throw new ArgumentException("Number of students cannot be less than one");
            }

            List<Assignment> assignmentsResult = _depotClassroom.GetAssignmentsByClassroomName(p_classRoomName);

            if (assignmentsResult.Count < 1)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                         "ScriptGithubRPLP - ScriptDownloadOneRepositoryForAssignment - la liste assignmentsResult assignée à partir de _depotClassroom.GetAssignmentsByClassroomName(p_classRoomName);est vide", 0));


                throw new ArgumentException($"No assignment in {p_classRoomName}");
            }

            Assignment assignment = assignmentsResult.SingleOrDefault(assignment => assignment.Name == p_assignmentName);

            if (assignment == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                         "ScriptGithubRPLP - ScriptDownloadOneRepositoryForAssignment - la liste assignment assignée à partir de assignmentsResult.SingleOrDefault(assignment => assignment.Name == p_assignmentName); est null", 0));

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

            if(repositories.Count == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                         "ScriptGithubRPLP - EnsureOrganisationRepositoriesAreInDB - la liste repositories assignée à partir de this.ReturnMissingRepositories(); est null", 0));

            }

            repositories.ForEach(r => this._depotRepository.UpsertRepository(r));
        }

        private List<Repository> ReturnMissingRepositories()
        {
            List<Repository> repositoriesToAdd = new List<Repository>();
            List<Organisation> organisations = this._depotOrganisation.GetOrganisations();

            if(organisations == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "ScriptGithubRPLP - ReturnMissingRepositories - la liste organisations assignée à partir de this._depotOrganisation.GetOrganisations(); est null", 0));

            }

            foreach (Organisation organisation in organisations)
            {
                List<Repository> repositoriesInDB = this._depotRepository.GetRepositoriesFromOrganisationName(organisation.Name);
                List<Repository_JSONDTO> repositoriesOnGithub = this._githubApiAction.GetOrganisationRepositoriesGithub(organisation.Name);

                repositoriesToAdd.AddRange(repositoriesOnGithub
                    .Where(ghRepo => repositoriesInDB.FirstOrDefault(dbRepo => dbRepo.FullName == ghRepo.full_name) == null)
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

        private Dictionary<string, int> GetStudentDictionary(List<Repository> p_repositories)
        {
            Dictionary<string, int> studentDictionary = new Dictionary<string, int>();

            foreach (Repository repository in p_repositories)
            {
                string[] splitRepository = repository.Name.Split('-');
                string studentUsername = splitRepository[1];

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

        private void AssignStudentReviewersToPullRequests(Dictionary<string, int> p_studentDictionary, string p_organisationName,
            string p_repositoryName, int p_reviewsPerRepository, Branch_JSONDTO branchDTO)
        {
            int numberStudentAdded = 0;
            string[] splitRepository = p_repositoryName.Split('-');

            do
            {
                string username = p_studentDictionary.Where(dictionary => dictionary.Key.ToLower() != splitRepository[1].ToLower())
                                                     .FirstOrDefault(dictionary => dictionary.Value == p_studentDictionary.Values.Min()).Key;

                createPullRequestAndAssignUser(p_organisationName, p_repositoryName, branchDTO.gitObject.sha, username);

                p_studentDictionary[username] = p_studentDictionary[username]++;
                numberStudentAdded++;

            } while (numberStudentAdded < p_reviewsPerRepository);
        }

        private List<Repository> GetStudentsRepositoriesForAssignment(List<Repository> p_repositories, List<Student> p_students, string assignmentName)
        {
            List<Repository> repositories = new List<Repository>();

            foreach (Repository repository in p_repositories)
            {
                string[] splitRepository = repository.Name.Split('-');

                if (splitRepository[0] == assignmentName)
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
