using Microsoft.AspNetCore.Mvc;
using RPLP.ENTITES;
using RPLP.JOURNALISATION;
using RPLP.SERVICES.InterfacesDepots;
using System.Diagnostics;

namespace RPLP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentController : ControllerBase
    {
        private readonly IDepotAssignment _depot;

        public AssignmentController(IDepotAssignment p_depot)
        {
            if (p_depot == null)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "AssignmentController - Constructeur - Dépot passé en paramêtre null", 0));
            }

            this._depot = p_depot;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Assignment>> Get()
        {
            try
            {
                Logging.Journal(new Log("api/Assignment", 200, "AssignmentController - GET méthode Get"));

                return Ok(this._depot.GetAssignments());
            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpGet("Id/{id}")]
        public ActionResult<Assignment> GetAssignmentById(int id)
        {
            try
            {
                if (id < 0)
                {
                    RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "AssignmentController - GetAssignmentById - id passé en paramêtre est hors limites", 0));
                }

                Logging.Journal(new Log($"api/Assignment/Id/{id}", 200, "AssignmentController - GET méthode GetAssignmentById"));

                return Ok(this._depot.GetAssignmentById(id));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Name/{assignmentName}")]
        public ActionResult<Assignment> GetAssignmentByName(string assignmentName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(assignmentName))
                {
                    RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "AssignmentController - GetAssignmentByName - assignmentName passé en paramêtre est vide", 0));
                }

                Logging.Journal(new Log($"api/Assignment/Name/{assignmentName}", 200, "AssignmentController - GET méthode GetAssignmentByName"));

                return Ok(this._depot.GetAssignmentByName(assignmentName));
            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpGet("Classroom/{classroomName}/Assignments")]
        public ActionResult<List<Assignment>> GetAssignmentsByClassroomName(string classroomName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "AssignmentController - GetAssignmentsByClassroomName - classroomName passé en paramêtre est vide", 0));
                }

                Logging.Journal(new Log($"api/Assignment/Classroom/{classroomName}/Assignments", 200, "AssignmentController - GET méthode GetAssignmentsByClassroomName"));

                return Ok(this._depot.GetAssignmentsByClassroomName(classroomName));
            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpPost]
        public ActionResult Post([FromBody] Assignment p_assignment)
        {
            try
            {
                if (p_assignment == null)
                {
                    RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "AssignmentController - Post - p_assignment passé en paramêtre est null", 0));

                    return BadRequest();
                }

                if (!ModelState.IsValid)
                {
                    RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "AssignmentController - Post - classroomName passé en paramêtre n'est pas valide", 0));

                    return BadRequest();
                }

                Logging.Journal(new Log($"api/Assignment/", 201, "AssignmentController - POST méthode Post"));

                this._depot.UpsertAssignment(p_assignment);

                return Created(nameof(this.Post), p_assignment);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpDelete("Name/{assignmentName}")]
        public ActionResult DeleteAssignment(string assignmentName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(assignmentName))
                {
                    RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "AssignmentController - DeleteAssignment - assignmentName passé en paramêtre est vide", 0));
                }

                Logging.Journal(new Log($"api/Assignment/Name/{assignmentName}", 204, "AssignmentController - DELETE méthode DeleteAssignment"));

                this._depot.DeleteAssignment(assignmentName);
                return NoContent();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
