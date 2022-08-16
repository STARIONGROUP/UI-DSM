// --------------------------------------------------------------------------------------------------------
// <copyright file="ProjectController.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Server.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using UI_DSM.Server.Managers.ProjectManager;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     The <see cref="ProjectController" /> is a <see cref="Controller" /> to manage <see cref="Project" />s
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        /// <summary>
        ///     The <see cref="IProjectManager" />
        /// </summary>
        private readonly IProjectManager projectManager;

        /// <summary>
        ///     Initialize a new <see cref="ProjectController" />
        /// </summary>
        /// <param name="projectManager">The <see cref="IProjectManager" /></param>
        public ProjectController(IProjectManager projectManager)
        {
            this.projectManager = projectManager;
        }

        /// <summary>
        ///     Gets a collections of all <see cref="Project" />
        /// </summary>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="ProjectDto" /> as result</returns>
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetProjects()
        {
            var projects = await this.projectManager.GetProjects();
            var projectsDto = projects.Select(x => (ProjectDto)x.ToDto());
            return this.Ok(projectsDto);
        }

        /// <summary>
        ///     Tries to create a new <see cref="Project" />
        /// </summary>
        /// <param name="newProject">The <see cref="ProjectDto" /></param>
        /// <returns>A <see cref="Task" /> with the result of the creation</returns>
        [HttpPost("Create")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateProject([FromBody] ProjectDto newProject)
        {
            var response = new EntityRequestResponseDto<ProjectDto>();

            if (newProject.InstantiatePoco() is not Project project || project.Id != Guid.Empty)
            {
                response.Errors = new List<string>
                {
                    "Invalid DTO or the Id has to be empty"
                };

                return this.BadRequest(response);
            }

            project.ProjectName = newProject.ProjectName;
            var identityResult = await this.projectManager.CreateProject(project);
            response.IsRequestSuccessful = identityResult.Succeeded;

            if (!identityResult.Succeeded)
            {
                response.Errors = identityResult.Errors;
                return this.BadRequest(response);
            }

            response.Entity = identityResult.Entity.ToDto() as ProjectDto;
            return this.Ok(response);
        }

        /// <summary>
        ///     Tries to delete a <see cref="Project" /> defined by the given <see cref="projectId" />
        /// </summary>
        /// <param name="projectId">The <see cref="Guid" /> of the project to delete</param>
        /// <returns>A <see cref="Task" /> with the result of the delete action</returns>
        [HttpDelete("{projectId}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteProject(Guid projectId)
        {
            var project = await this.projectManager.GetProject(projectId);
            var response = new RequestResponseDto();

            if (project == null)
            {
                response.Errors = new List<string>
                {
                    $"Project with the id {projectId} does not exist"
                };

                return this.NotFound(response);
            }

            var identityResult = await this.projectManager.DeleteProject(project);
            response.IsRequestSuccessful = identityResult.Succeeded;

            if (identityResult.Succeeded)
            {
                return this.Ok(response);
            }

            response.Errors = identityResult.Errors;
            return this.StatusCode(500, response);
        }

        /// <summary>
        ///     Gets a <see cref="Project" /> based on its <see cref="Guid" />
        /// </summary>
        /// <param name="projectId">The <see cref="Guid" /> of the <see cref="Project" /> to get</param>
        /// <returns>A <see cref="Task" /> with the <see cref="Project" /> if found </returns>
        [HttpGet("{projectId:guid}")]
        [Authorize]
        public async Task<IActionResult> GetProject(Guid projectId)
        {
            var project = await this.projectManager.GetProject(projectId);
            return project == null ? this.NotFound() : this.Ok((ProjectDto)project.ToDto());
        }
    }
}
