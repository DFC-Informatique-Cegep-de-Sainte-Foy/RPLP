using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RPLP.DAL.SQL.Depots;
using RPLP.ENTITES;
using RPLP.MVC.Models;
using RPLP.SERVICES.Github;
using RPLP.SERVICES.InterfacesDepots;

namespace RPLP.MVC.Controllers
{
    public class RPLPController : Controller
    {
        private readonly IDepotOrganisation _depotOrganisation = new DepotOrganisation();
        private readonly IDepotClassroom _depotClassroom = new DepotClassroom();
        private readonly ScriptGithubRPLP _scriptGithub;

        public RPLPController()
        {
            string token = "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";
            GithubApiAction _githubAction = new GithubApiAction(token);
            _scriptGithub = new ScriptGithubRPLP(new DepotClassroom(), new DepotRepository(), token);
        }

        public IActionResult Index()
        {
            PeerReviewViewModel model = new PeerReviewViewModel();
            model.Organisations = GetOrganisations();

            return View(model);
        }

        private List<OrganisationViewModel> GetOrganisations()
        {
            List<OrganisationViewModel> organisations = new List<OrganisationViewModel>();
            List<Organisation> databaseOrganisations = _depotOrganisation.GetOrganisations();

            if (databaseOrganisations.Count >= 1)
            {
                foreach (Organisation org in databaseOrganisations)
                {
                    organisations.Add(new OrganisationViewModel(org.Name));
                }
            }

            return organisations;
        }

        [HttpGet]
        public ActionResult<List<ClassroomViewModel>> GetClassroomsOfOrganisationByName(string orgName)
        {
            List<ClassroomViewModel> classes = new List<ClassroomViewModel>();
            List<Classroom> databaseClasses = _depotClassroom.GetClassroomsByOrganisationName(orgName);

            if (databaseClasses.Count >= 1)
            {
                foreach (Classroom classroom in databaseClasses)
                {
                    classes.Add(new ClassroomViewModel(classroom.Name));
                }
            }

            return classes;
        }

        [HttpGet]
        public ActionResult<List<AssignmentViewModel>> GetAssignmentsOfClassroomByName(string classroomName)
        {
            List<AssignmentViewModel> assignments = new List<AssignmentViewModel>();
            List<Assignment> databaseAssignments = _depotClassroom.GetAssignmentsByClassroomName(classroomName);

            if (databaseAssignments.Count >= 1)
            {
                foreach (Assignment assignment in databaseAssignments)
                {
                    assignments.Add(new AssignmentViewModel(assignment.Name));
                }
            }

            return assignments;
        }

        [HttpGet]
        public ActionResult StartStudentAssignationScript(string organisationName, string classroomName, string assignmentName, int numberOfReviews)
        {
            try
            {
                _scriptGithub.ScriptAssignStudentToAssignmentReview(organisationName, classroomName, assignmentName, numberOfReviews);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }

}
