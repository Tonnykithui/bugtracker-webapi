using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core2.Interfaces;
using Core2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TicketSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IProject _project;

        public ProjectsController(IProject project)
        {
            _project = project;
        }

        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            return Ok(await _project.GetProjectsAsync());
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject(Project project)
        {
            var cproject = await _project.CreateProjectAsync(project);
            return Ok("Project created");
            //return CreatedAtAction(
            //    nameof(GetProjectById),
            //    new { id = cproject.ProjectId },
            //    cproject
            //    );
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectById(int? id)
        {

            return Ok(await _project.GetSingleProjectAsync(id));
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProject(int id, Project project)
        {
            _project.UpdateProjectAsync(id, project);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProject(int id)
        {
            _project.DeleteProjectAsync(id);
            return NoContent();
        }

        [HttpPost]
        [Route("/api/projects/{id}/assignusers")]
        public IActionResult AssignUsers(int id, ProjUser projUser)
        {
            if (id == 0 || projUser.ApplicationUserIds == null)
            {
                return BadRequest("Provide correct credentials");
            }

            _project.AssignUsersProject(id, projUser);

            return NoContent();
        }

        [HttpDelete]
        [Route("/api/projects/{id}/deleteusers/{userId}")]
        public IActionResult DeleteAssignUsers(int id, string userId)
        {
            if (id == 0 || userId == null)
            {
                return BadRequest("Provide correct credentials");
            }

            _project.DeleteUserFromProject(id, userId);

            return NoContent();
        }
    }
}