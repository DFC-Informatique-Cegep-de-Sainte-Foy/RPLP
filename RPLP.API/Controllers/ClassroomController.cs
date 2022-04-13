using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RPLP.ENTITES;
using RPLP.SERVICES.InterfacesDepots;

namespace RPLP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassroomController : ControllerBase
    {
        private readonly IDepotClassroom _depot;

        public ClassroomController(IDepotClassroom p_depotClassroom)
        {
            this._depot = p_depotClassroom;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Classroom>> Get()
        {
            return Ok(this._depot.GetClassrooms());
        }

        [HttpGet("Id/{id}")]
        public ActionResult<Classroom> GetClassroomById(int id)
        {
            return Ok(this._depot.GetClassroomById(id));
        }

        [HttpGet("Name/{classroomName}")]
        public ActionResult<Classroom> GetClassroomByName(string classroomName)
        {
            return Ok(this._depot.GetClassroomByName(classroomName));
        }

        [HttpGet("Organisation/{organisationName}")]
        public ActionResult<Classroom> GetClassroomsByOrganisationName(string organisationName)
        {
            return Ok(this._depot.GetClassroomsByOrganisationName(organisationName));
        }

        [HttpGet("Assignments/{classroomName}")]
        public ActionResult<List<Assignment>> GetAssignmentsByClassroomName(string classroomName)
        {
            return Ok(this._depot.GetAssignmentsByClassroomName(classroomName));
        }

        [HttpGet("Name/{classroomName}/Teachers")]
        public ActionResult<List<Teacher>> GetTeachers(string classroomName)
        {
            return Ok(this._depot.GetTeachersByClassroomName(classroomName));
        }

        [HttpGet("Name/{classroomName}/Students")]
        public ActionResult<List<Student>> GetStudents(string classroomName)
        {
            return Ok(this._depot.GetStudentsByClassroomName(classroomName));
        }

        [HttpGet("Name/{classroomName}/Assignments")]
        public ActionResult<List<Assignment>> GetAssignments(string classroomName)
        {
            return Ok(this._depot.GetAssignmentsByClassroomName(classroomName));
        }

        [HttpPost("Name/{classroomName}/Teachers/Add/{teacherUsername}")]
        public ActionResult AddTeacherToClassroom(string classroomName, string teacherUsername)
        {
            if (string.IsNullOrWhiteSpace(classroomName) || string.IsNullOrWhiteSpace(teacherUsername))
            {
                return BadRequest();
            }

            this._depot.AddTeacherToClassroom(classroomName, teacherUsername);

            return Created(nameof(this.AddTeacherToClassroom), classroomName);
        }

        [HttpPost("Name/{classroomName}/Students/Add/{studentUsername}")]
        public ActionResult AddStudentToClassroom(string classroomName, string studentUsername)
        {
            if (string.IsNullOrWhiteSpace(classroomName) || string.IsNullOrWhiteSpace(studentUsername))
            {
                return BadRequest();
            }

            this._depot.AddStudentToClassroom(classroomName, studentUsername);

            return Created(nameof(this.AddStudentToClassroom), classroomName);
        }

        [HttpPost("Name/{classroomName}/Assignments/Add/{assignmentName}")]
        public ActionResult AddAssignmentToClassroom(string classroomName, string assignmentName)
        {
            if (string.IsNullOrWhiteSpace(classroomName) || string.IsNullOrWhiteSpace(assignmentName))
            {
                return BadRequest();
            }

            this._depot.AddAssignmentToClassroom(classroomName, assignmentName);

            return Created(nameof(this.AddAssignmentToClassroom), classroomName);
        }

        [HttpPost("Name/{classroomName}/Teachers/Remove/{teacherUsername}")]
        public ActionResult RemoveTeacherFromClassroom(string classroomName, string teacherUsername)
        {
            if (string.IsNullOrWhiteSpace(classroomName) || string.IsNullOrWhiteSpace(teacherUsername))
            {
                return BadRequest();
            }

            this._depot.RemoveTeacherFromClassroom(classroomName, teacherUsername);

            return  NoContent(); 
        }

        [HttpPost("Name/{classroomName}/Students/Remove/{studentUsername}")]
        public ActionResult RemoveStudentFromClassroom(string classroomName, string studentUsername)
        {
            if (string.IsNullOrWhiteSpace(classroomName) || string.IsNullOrWhiteSpace(studentUsername))
            {
                return BadRequest();
            }

            this._depot.RemoveStudentFromClassroom(classroomName, studentUsername);

            return  NoContent(); 
        }

        [HttpPost("Name/{classroomName}/Assignments/Remove/{assignmentName}")]
        public ActionResult RemoveAssignmentFromClassroom(string classroomName, string assignmentName)
        {
            if (string.IsNullOrWhiteSpace(classroomName) || string.IsNullOrWhiteSpace(assignmentName))
            {
                return BadRequest();
            }

            this._depot.RemoveAssignmentFromClassroom(classroomName, assignmentName);

            return  NoContent(); 
        }

        [HttpPost]
        public ActionResult UpsertClassroom([FromBody] Classroom p_classroom)
        {
            if (p_classroom == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            this._depot.UpsertClassroom(p_classroom);

            return Created(nameof(this.UpsertClassroom), p_classroom);
        }

        [HttpDelete("Name/{classroomName}")]
        public ActionResult DeleteClassroom(string classroomName)
        {
            this._depot.DeleteClassroom(classroomName);
            return NoContent();
        }
    }
}
