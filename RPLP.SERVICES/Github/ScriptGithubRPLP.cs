using Newtonsoft.Json;
using RPLP.DAL.DTO.Json;
using RPLP.DAL.DTO.Sql;
using RPLP.ENTITES;
using RPLP.SERVICES.InterfacesDepots;
using System;
using System.Collections.Generic;
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

        public ScriptGithubRPLP(IDepotClassroom p_depotClassroom, IDepotRepository p_depotRepository,
            IDepotOrganisation p_depotOrganisation, string p_token)
        {
            this._depotClassroom = p_depotClassroom;
            this._depotRepository = p_depotRepository;
            this._depotOrganisation = p_depotOrganisation;
            this._githubApiAction = new GithubApiAction(p_token);
        }

        public void ScriptAssignStudentToAssignmentReview(string p_organisationName, string p_classRoomName, string p_assignmentName, int p_reviewsPerRepository)
        {
            if (p_reviewsPerRepository <= 0 || string.IsNullOrWhiteSpace(p_organisationName) || string.IsNullOrWhiteSpace(p_classRoomName) || string.IsNullOrWhiteSpace(p_assignmentName))
                throw new ArgumentException("One of the provided value is incorrect or null");

            List<Student> students = _depotClassroom.GetStudentsByClassroomName(p_classRoomName);

            if (students.Count < p_reviewsPerRepository + 1)
                throw new ArgumentException("Number of students inferior to number of reviews");


            List<Repository> repositoriesToAssign = getRepositoriesToAssign(p_organisationName, p_classRoomName, p_assignmentName, students);
            Dictionary<string, int> studentDictionary = new Dictionary<string, int>();

            if (repositoriesToAssign == null)
                throw new ArgumentNullException($"No repositories to assign in {p_classRoomName}");

            studentDictionary = GetStudentDictionary(repositoriesToAssign);

            foreach (Repository repository in repositoriesToAssign)
            {
                prepareRepositoryAndCreatePullRequest(p_organisationName, repository.Name, studentDictionary, p_reviewsPerRepository);
            }
        }

        public void ScriptRemoveStudentCollaboratorsFromAssignment(string p_organisationName, string p_classRoomName, string p_assignmentName)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName) || string.IsNullOrWhiteSpace(p_classRoomName) || string.IsNullOrWhiteSpace(p_assignmentName))
                throw new ArgumentException("One of the provided value is incorrect or null");

            List<Student> students = _depotClassroom.GetStudentsByClassroomName(p_classRoomName);
            List<Repository> repositoriesToAssign = getRepositoriesToAssign(p_organisationName, p_classRoomName, p_assignmentName, students);

            foreach (Repository repository in repositoriesToAssign)
            {
                List<Collaborator_JSONDTO> collaborator = _githubApiAction.GetCollaboratorFromStudentRepositoryGithub(p_organisationName, repository.Name);
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
                throw new ArgumentNullException($"Branch does not exist or wrong name was entered");

            branchDTO = GetFeedbackBranchFromBranchList(branchesResult);
            AssignStudentReviewersToPullRequests(p_studentDictionary, p_organisationName, p_repositoryName, p_reviewsPerRepository, branchDTO);
        }

        private void createPullRequestAndAssignUser(string p_organisationName, string p_repositoryName, string p_sha, string p_username)
        {
            string newBranchName = $"Feedback-{p_username}";

            string resultCreateBranch = this._githubApiAction.CreateNewBranchForFeedbackGitHub(p_organisationName, p_repositoryName, p_sha, newBranchName);
            if (resultCreateBranch != "Created")
                throw new ArgumentException($"Branch not created in {p_repositoryName}");

            string resultCreatePR = this._githubApiAction.CreateNewPullRequestFeedbackGitHub(p_organisationName, p_repositoryName, newBranchName, newBranchName.ToLower(), "Voici où vous devez mettre vos commentaires");
            if (resultCreatePR != "Created")
                throw new ArgumentException($"PullRequest not created in {p_repositoryName}");

            string resultAddStudent = this._githubApiAction.AddStudentAsCollaboratorToPeerRepositoryGithub(p_organisationName, p_repositoryName, p_username);
            if (resultAddStudent != "Created")
                throw new ArgumentException($"Student not added in {p_repositoryName}");
        }

        private List<Repository> getRepositoriesToAssign(string p_organisationName, string p_classRoomName, string p_assignmentName, List<Student> p_students)
        {
            List<Assignment> assignmentsResult = _depotClassroom.GetAssignmentsByClassroomName(p_classRoomName);

            if (assignmentsResult.Count < 1)
                throw new ArgumentException($"No assignment in {p_classRoomName}");

            Assignment assignment = assignmentsResult.SingleOrDefault(assignment => assignment.Name == p_assignmentName);

            if (assignment == null)
                throw new ArgumentException($"no assignment with name {p_assignmentName}");

            List<Repository> repositories = new List<Repository>();

            List<Repository> repositoriesResult = this._depotRepository.GetRepositoriesFromOrganisationName(p_organisationName);
            repositories = GetStudentsRepositoriesForAssignment(repositoriesResult, p_students, p_assignmentName);

            return repositories;
        }

        public void ScriptAssignTeacherToAssignmentReview(string p_organisationName, string p_classRoomName, string p_assignmentName, string teacherUsername)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName) || string.IsNullOrWhiteSpace(p_classRoomName) || string.IsNullOrWhiteSpace(p_assignmentName))
                throw new ArgumentException("One of the provided value is incorrect or null");

            List<Student> students = _depotClassroom.GetStudentsByClassroomName(p_classRoomName);
            List<Teacher> teachers = _depotClassroom.GetTeachersByClassroomName(p_classRoomName);

            if (students.Count < 1)
                throw new ArgumentException("Number of students cannot be less than one");

            if (teachers.Count < 1)
                throw new ArgumentException("Number of teachers cannot be less than one");

            List<Repository> repositoriesToAssign = getRepositoriesToAssign(p_organisationName, p_classRoomName, p_assignmentName, students);

            if (repositoriesToAssign == null)
                throw new ArgumentNullException($"No repositories to assign in {p_classRoomName}");

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
                throw new ArgumentNullException($"Branch does not exist or wrong name was entered");

            branchDTO = GetMainBranchFromBranchList(branchesResult);

            CreatePullRequestAndAssignTeacher(p_organisationName, p_repositoryName, branchDTO.gitObject.sha, p_newFileName, p_message, p_content, teacherUsername);
        }

        private void CreatePullRequestAndAssignTeacher(string p_organisationName, string p_repositoryName, string p_sha, string p_newFileName, string p_message, string p_content, string teacherUsername)
        {
            string newBranchName = $"feedback-{teacherUsername}";

            string resultCreateBranch = this._githubApiAction.CreateNewBranchForFeedbackGitHub(p_organisationName, p_repositoryName, p_sha, newBranchName);
            if (resultCreateBranch != "Created")
                throw new ArgumentException($"Branch not created in {p_repositoryName}");

            this._githubApiAction.AddFileToContentsGitHub(p_organisationName, p_repositoryName, newBranchName, p_newFileName, p_message, p_content);

            string resultCreatePR = this._githubApiAction.CreateNewPullRequestFeedbackGitHub(p_organisationName, p_repositoryName, newBranchName, "Feedback", "Voici où vous devez mettre vos commentaires");
            if (resultCreatePR != "Created")
                throw new ArgumentException($"PullRequest not created in {p_repositoryName}");
        }

        public string ScriptDownloadAllRepositoriesForAssignment(string p_organisationName, string p_classRoomName, string p_assignmentName)
        {
            Console.Out.WriteLine($"API - ScriptDownloadAllRepositoriesForAssignment({p_organisationName}, {p_classRoomName}, {p_assignmentName})");
            string directoryToZipName = "ZippedRepos";

            if (string.IsNullOrWhiteSpace(p_organisationName) || string.IsNullOrWhiteSpace(p_classRoomName) || string.IsNullOrWhiteSpace(p_assignmentName))
                throw new ArgumentException("One of the provided value is incorrect or null");


            List<Student> students = _depotClassroom.GetStudentsByClassroomName(p_classRoomName);
            Console.Out.WriteLine($"students.Count == {students.Count}");
            if (students.Count < 1)
                throw new ArgumentException("Number of students cannot be less than one");

            List<Assignment> assignmentsResult = _depotClassroom.GetAssignmentsByClassroomName(p_classRoomName);

            Console.Out.WriteLine($"assignmentsResult.Count == {assignmentsResult.Count}");
            if (assignmentsResult.Count < 1)
                throw new ArgumentException($"No assignment in {p_classRoomName}");

            Assignment assignment = assignmentsResult.SingleOrDefault(assignment => assignment.Name == p_assignmentName);

            Console.Out.WriteLine($"assignment is null == {assignment == null}");
            if (assignment == null)
                throw new ArgumentException($"No assignment with name {p_assignmentName}");


            List<Repository> repositoriesResult = this._depotRepository.GetRepositoriesFromOrganisationName(p_organisationName);
            List<Repository> repositories = GetStudentsRepositoriesForAssignment(repositoriesResult, students, p_assignmentName);

            Console.Out.WriteLine($"repositoriesResult.Count == {repositoriesResult.Count}");
            Console.Out.WriteLine($"repositories.Count == {repositories.Count}");

            DeleteFilesAndDirectoriesForDownloads();
            Directory.CreateDirectory(directoryToZipName);

            DownloadRepositoriesToDirectory(repositories);
            string path = GetZipFromReposDirectory();

            return path;
        }

        public string ScriptDownloadOneRepositoryForAssignment(string p_organisationName, string p_classRoomName, string p_assignmentName, string p_repositoryName)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName) || string.IsNullOrWhiteSpace(p_classRoomName) || string.IsNullOrWhiteSpace(p_assignmentName))
                throw new ArgumentException("One of the provided value is incorrect or null");


            List<Student> students = _depotClassroom.GetStudentsByClassroomName(p_classRoomName);

            if (students.Count < 1)
                throw new ArgumentException("Number of students cannot be less than one");


            List<Assignment> assignmentsResult = _depotClassroom.GetAssignmentsByClassroomName(p_classRoomName);

            if (assignmentsResult.Count < 1)
                throw new ArgumentException($"No assignment in {p_classRoomName}");

            Assignment assignment = assignmentsResult.SingleOrDefault(assignment => assignment.Name == p_assignmentName);

            if (assignment == null)
                throw new ArgumentException($"No assignment with name {p_assignmentName}");

            Repository repository = _depotRepository.GetRepositoryByName(p_repositoryName);

            string path = DownloadRepositoryToFile(repository);

            return path;
        }

        #region coherence

        public void EnsureOrganisationRepositoriesAreInDB()
        {
            List<Repository> repositories = this.ReturnMissingRepositories();
            repositories.ForEach(r => this._depotRepository.UpsertRepository(r));
        }

        private List<Repository> ReturnMissingRepositories()
        {
            List<Repository> repositoriesToAdd = new List<Repository>();
            List<Organisation> organisations = this._depotOrganisation.GetOrganisations();

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
            Console.Out.WriteLine($"GetStudentsRepositoriesForAssignment");
            List<Repository> repositories = new List<Repository>();

            foreach (Repository repository in p_repositories)
            {
                Console.Out.WriteLine($"Trying to parse repository.Name {repository.Name}");
                string[] splitRepository = repository.Name.Split('-');

                //if (splitRepository[0] == assignmentName)
                Console.Out.WriteLine($"repository.Name.StartsWith(assignmentName) == {repository.Name.StartsWith(assignmentName)}");
                if (repository.Name.StartsWith(assignmentName))
                {
                    string username = repository.Name.Substring(assignmentName.Length + 1);
                    Console.Out.WriteLine($"username == {username}");
                    foreach (Student student in p_students)
                    {
                        if (username == student.Username)
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

            Console.Out.WriteLine($"Trying to DownloadRepositoriesToDirectory - p_repositories.Count == {p_repositories.Count}");
            foreach (Repository repository in p_repositories)
            {
                Console.Out.WriteLine($"Processing repository {repository.OrganisationName} / {repository.Name}");

                try {
                    var download = _githubApiAction.DownloadRepository(repository.OrganisationName, repository.Name);
                    Stream stream = download.Content.ReadAsStream();

                    using (var fileStream = File.Create("repo.zip"))
                    {
                        stream.CopyTo(fileStream);
                    }

                    ZipFile.ExtractToDirectory("repo.zip", $"ZippedRepos/{repository.Name}");
                } catch (Exception ex) {
                    Console.Out.WriteLine($"DownloadRepositoriesToDirectory - {ex.Message}");
                }

                try {
                    if (File.Exists("repo.zip")) {
                        File.Delete("repo.zip");
                    }
                } catch (Exception ex) {
                    Console.Out.WriteLine($"DownloadRepositoriesToDirectory - {ex.Message}");
                }
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
