﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RPLP.ENTITES;
using RPLP.JOURNALISATION;
using RPLP.SERVICES.InterfacesDepots;
using System.Diagnostics;

namespace RPLP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassroomController : ControllerBase
    {
        private readonly IDepotClassroom _depot;

        public ClassroomController(IDepotClassroom p_depotClassroom)
        {
            if (p_depotClassroom == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "ClassroomController - Constructeur - p_depotClassroom passé en paramêtre null"));
            }

            this._depot = p_depotClassroom;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Classroom>> Get()
        {
            try
            {
                return Ok(this._depot.GetClassrooms());
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Id/{id}")]
        public ActionResult<Classroom> GetClassroomById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString(),
                    "ClassroomController - GetClassroomById - id passé en paramêtre est hors limites"));
                }

                return Ok(this._depot.GetClassroomById(id));
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet("Name/{classroomName}")]
        public ActionResult<Classroom> GetClassroomByName(string classroomName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "ClassroomController - GetClassroomByName - classroomName passé en paramêtre est vide"));
                }


                return Ok(this._depot.GetClassroomByName(classroomName));
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet("Organisation/{organisationName}/Classroom")]
        public ActionResult<Classroom> GetClassroomsByOrganisationName(string organisationName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(organisationName))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "ClassroomController - GetClassroomsByOrganisationName - organisationName passé en paramêtre est vide"));
                }

                return Ok(this._depot.GetClassroomsByOrganisationName(organisationName));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Assignments/{classroomName}")]
        public ActionResult<List<Assignment>> GetAssignmentsByClassroomName(string classroomName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "ClassroomController - GetAssignmentsByClassroomName - classroomName passé en paramêtre est vide"));
                }

                return Ok(this._depot.GetAssignmentsByClassroomName(classroomName));
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet("Name/{classroomName}/Teachers")]
        public ActionResult<List<Teacher>> GetTeachers(string classroomName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "ClassroomController - GetTeachers - classroomName passé en paramêtre est vide"));
                }

                return Ok(this._depot.GetTeachersByClassroomName(classroomName));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Name/{classroomName}/Students")]
        public ActionResult<List<Student>> GetStudents(string classroomName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "ClassroomController - GetStudents - classroomName passé en paramêtre est vide"));
                }

                return Ok(this._depot.GetStudentsByClassroomName(classroomName));
            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpGet("Name/{classroomName}/Assignments")]
        public ActionResult<List<Assignment>> GetAssignments(string classroomName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "ClassroomController - GetAssignments - classroomName passé en paramêtre est vide"));
                }

                return Ok(this._depot.GetAssignmentsByClassroomName(classroomName));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("Name/{classroomName}/Teachers/Add/{teacherUsername}")]
        public ActionResult AddTeacherToClassroom(string classroomName, string teacherUsername)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                        "ClassroomController - AddTeacherToClassroom - classroomName passé en paramêtre est vide"));

                    return BadRequest();
                }

                if (string.IsNullOrWhiteSpace(teacherUsername))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                        "ClassroomController - AddTeacherToClassroom - teacherUsername passé en paramêtre est vide"));

                    return BadRequest();
                }

                this._depot.AddTeacherToClassroom(classroomName, teacherUsername);

                return Created(nameof(this.AddTeacherToClassroom), classroomName);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("Name/{classroomName}/Students/Add/{studentUsername}")]
        public ActionResult AddStudentToClassroom(string classroomName, string studentUsername)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                           "ClassroomController - AddStudentToClassroom - classroomName passé en paramêtre est vide"));

                    return BadRequest();
                }

                if (string.IsNullOrWhiteSpace(studentUsername))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                           "ClassroomController - AddStudentToClassroom - studentUsername passé en paramêtre est vide"));

                    return BadRequest();
                }

                this._depot.AddStudentToClassroom(classroomName, studentUsername);

                return Created(nameof(this.AddStudentToClassroom), classroomName);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("Name/{classroomName}/Students/Add/Matricule/{studentMatricule}")]
        public ActionResult AddStudentToClassroomMatricule(string classroomName, string studentMatricule)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                              "ClassroomController - AddStudentToClassroomMatricule - classroomName passé en paramêtre est vide"));

                    return BadRequest();
                }

                if (string.IsNullOrWhiteSpace(studentMatricule))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                              "ClassroomController - AddStudentToClassroomMatricule - classroomName passé en paramêtre est vide"));

                    return BadRequest();
                }

                this._depot.AddStudentToClassroomMatricule(classroomName, studentMatricule);

                return Created(nameof(this.AddStudentToClassroom), classroomName);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("Name/{classroomName}/Assignments/Add/{assignmentName}")]
        public ActionResult AddAssignmentToClassroom(string classroomName, string assignmentName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                                  "ClassroomController - AddAssignmentToClassroom - classroomName passé en paramêtre est vide"));

                    return BadRequest();
                }

                if (string.IsNullOrWhiteSpace(assignmentName))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                                  "ClassroomController - AddAssignmentToClassroom - assignmentName passé en paramêtre est vide"));

                    return BadRequest();
                }

                this._depot.AddAssignmentToClassroom(classroomName, assignmentName);

                return Created(nameof(this.AddAssignmentToClassroom), classroomName);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("Name/{classroomName}/Teachers/Remove/{teacherUsername}")]
        public ActionResult RemoveTeacherFromClassroom(string classroomName, string teacherUsername)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                                  "ClassroomController - RemoveTeacherFromClassroom - classroomName passé en paramêtre est vide"));

                    return BadRequest();
                }

                if (string.IsNullOrWhiteSpace(teacherUsername))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                                  "ClassroomController - RemoveTeacherFromClassroom - teacherUsername passé en paramêtre est vide"));

                    return BadRequest();
                }

                this._depot.RemoveTeacherFromClassroom(classroomName, teacherUsername);

                return NoContent();
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost("Name/{classroomName}/Students/Remove/{studentUsername}")]
        public ActionResult RemoveStudentFromClassroom(string classroomName, string studentUsername)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                                      "ClassroomController - RemoveStudentFromClassroom - classroomName passé en paramêtre est vide"));

                    return BadRequest();
                }

                if (string.IsNullOrWhiteSpace(studentUsername))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                                      "ClassroomController - RemoveStudentFromClassroom - studentUsername passé en paramêtre est vide"));

                    return BadRequest();
                }

                this._depot.RemoveStudentFromClassroom(classroomName, studentUsername);

                return NoContent();
            }
            catch (Exception)
            {
                throw;
            }
          
        }

        [HttpPost("Name/{classroomName}/Assignments/Remove/{assignmentName}")]
        public ActionResult RemoveAssignmentFromClassroom(string classroomName, string assignmentName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                                          "ClassroomController - RemoveAssignmentFromClassroom - classroomName passé en paramêtre est vide"));

                    return BadRequest();
                }

                if (string.IsNullOrWhiteSpace(assignmentName))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                                          "ClassroomController - RemoveAssignmentFromClassroom - assignmentName passé en paramêtre est vide"));

                    return BadRequest();
                }

                this._depot.RemoveAssignmentFromClassroom(classroomName, assignmentName);

                return NoContent();
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public ActionResult UpsertClassroom([FromBody] Classroom p_classroom)
        {
            try
            {
                if (p_classroom == null)
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                                             "ClassroomController - UpsertClassroom - p_classroom passé en paramêtre est null"));

                    return BadRequest();
                }

                if (!ModelState.IsValid)
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentException().ToString(), new StackTrace().ToString(),
                                             "ClassroomController - UpsertClassroom - p_classroom passé en paramêtre n'est pas valide"));

                    return BadRequest();
                }

                this._depot.UpsertClassroom(p_classroom);

                return Created(nameof(this.UpsertClassroom), p_classroom);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpDelete("Name/{classroomName}")]
        public ActionResult DeleteClassroom(string classroomName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(classroomName))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                                          "ClassroomController - DeleteClassroom - classroomName passé en paramêtre est vide"));

                    return BadRequest();
                }

                this._depot.DeleteClassroom(classroomName);
                return NoContent();
            }
            catch (Exception)
            {
                throw;
            }
           
        }
    }
}
