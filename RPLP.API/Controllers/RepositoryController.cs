﻿using Microsoft.AspNetCore.Mvc;
using RPLP.ENTITES;
using RPLP.SERVICES.InterfacesDepots;

namespace RPLP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepositoryController : ControllerBase
    {
        private readonly IDepotRepository _depot;

        public RepositoryController(IDepotRepository p_depot)
        {
            this._depot = p_depot;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Repository>> Get()
        {
            return Ok(this._depot.GetRepositories());
        }

        [HttpGet("Id/{id}")]
        public ActionResult<Repository> GetRepositoryById(int id)
        {
            return Ok(this._depot.GetRepositoryById(id));
        }

        [HttpGet("Name/{repositoryName}")]
        public ActionResult<Repository> GetRepositoryByName(string repositoryName)
        {
            return Ok(this._depot.GetRepositoryByName(repositoryName));
        }

        [HttpGet("Classroom/Name/{classroomName}")]
        public ActionResult<IEnumerable<Repository>> GetRepositoryByClassroomName(string classroomName)
        {
            return Ok(this._depot.GetRepositoryByClassroomName(classroomName));
        }

        [HttpPost]
        public ActionResult Post([FromBody] Repository p_repository)
        {
            if (p_repository == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            this._depot.UpsertRepository(p_repository);

            return Created(nameof(this.Post), p_repository);
        }
    }
}
