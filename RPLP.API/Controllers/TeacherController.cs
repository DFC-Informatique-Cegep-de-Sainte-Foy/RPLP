﻿using Microsoft.AspNetCore.Mvc;
using RPLP.ENTITES;
using RPLP.JOURNALISATION;
using RPLP.SERVICES.InterfacesDepots;
using System.Diagnostics;

namespace RPLP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private readonly IDepotTeacher _depot;

        public TeacherController(IDepotTeacher p_depot)
        {
            if (p_depot == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                "TeacherController - Constructeur - p_depot passé en paramêtre est null"));
            }

            this._depot = p_depot;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Teacher>> Get()
        {
            try
            {
                return Ok(this._depot.GetTeachers());
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Deactivated")]
        public ActionResult<IEnumerable<Teacher>> GetDeactivated()
        {
            try
            {
                return Ok(this._depot.GetDeactivatedTeachers());
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Id/{id}")]
        public ActionResult<Teacher> GetTeacherById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString(),
                    "TeacherController - GetTeacherById - id passé en paramêtre hors limites"));
                }

                return Ok(this._depot.GetTeacherById(id));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Username/{teacherUsername}")]
        public ActionResult<Teacher> GetTeacherByUsername(string teacherUsername)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(teacherUsername))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "TeacherController - GetTeacherByUsername - teacherUsername passé en paramêtre est vide"));
                }

                return Ok(this._depot.GetTeacherByUsername(teacherUsername));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Email/{email}")]
        public ActionResult<Teacher> GetTeacherByEmail(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "TeacherController - GetTeacherByEmail - email passé en paramêtre est vide"));
                }

                return Ok(this._depot.GetTeacherByEmail(email));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Username/{teacherUsername}/Classrooms")]
        public ActionResult<List<Classroom>> GetTeacherClasses(string teacherUsername)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(teacherUsername))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "TeacherController - GetTeacherClasses - teacherUsername passé en paramêtre est vide"));
                }

                return Ok(this._depot.GetTeacherClasses(teacherUsername));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Email/{email}/Organisations")]
        public ActionResult<List<Organisation>> GetTeacherOrganisationsByEmail(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "TeacherController - GetTeacherOrganisationsByEmail - email passé en paramêtre est vide"));
                }

                string? username = this._depot.GetTeacherByEmail(email)?.Username;

                if (string.IsNullOrWhiteSpace(username))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "TeacherController - GetTeacherOrganisationsByEmail - username assigné à partir de la méthode GetTeacherByEmail est vide"));

                    return BadRequest();
                }

                return Ok(this._depot.GetTeacherOrganisations(username));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Username/{username}/Organisations")]
        public ActionResult<List<Organisation>> GetTeacherOrganisationsByUsername(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "TeacherController - GetTeacherOrganisationsByUsername - username passé en paramêtre est vide"));

                    return BadRequest();
                }

                return Ok(this._depot.GetTeacherOrganisations(username));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Email/{email}/Organisation/{organisationName}/Classrooms")]
        public ActionResult<List<Classroom>> GetClassroomsOfTeacherInOrganisationByEmail(string email, string organisationName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "TeacherController - GetClassroomsOfTeacherInOrganisationByEmail - email passé en paramêtre est vide"));
                }

                if (string.IsNullOrWhiteSpace(organisationName))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "TeacherController - GetClassroomsOfTeacherInOrganisationByEmail - organisationName passé en paramêtre est vide"));
                }

                return Ok(this._depot.GetTeacherClassesInOrganisationByEmail(email, organisationName));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult UpsertTeacher([FromBody] Teacher p_teacher)
        {
            try
            {
                if (p_teacher == null)
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                   "TeacherController - GetClassroomsOfTeacherInOrganisationByEmail - p_teacher passé en paramêtre est null"));

                    return BadRequest();
                }

                if(p_teacher.Id <= 0)
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString(),
                   "TeacherController - GetClassroomsOfTeacherInOrganisationByEmail - le ID du paramêtre p_teacher est hors limites"));

                    return BadRequest();
                }

                if(!ModelState.IsValid)
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentException().ToString(), new StackTrace().ToString(),
                   "TeacherController - GetClassroomsOfTeacherInOrganisationByEmail - p_teacher passé en paramêtre n'est pas un modèle valide"));

                    return BadRequest();
                }

                this._depot.UpsertTeacher(p_teacher);
                
                return Created(nameof(this.UpsertTeacher), p_teacher);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpDelete("Username/{teacherUsername}")]
        public ActionResult DeleteTeacher(string teacherUsername)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(teacherUsername))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "TeacherController - DeleteTeacher - teacherUsername passé en paramêtre est vide"));
                }

                this._depot.DeleteTeacher(teacherUsername);
                return NoContent();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("Reactivate/{username}")]
        public ActionResult ReactivateTeacher(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString(),
                    "TeacherController - ReactivateTeacher - username passé en paramêtre est vide"));
                }

                this._depot.ReactivateTeacher(username);
                return NoContent();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
