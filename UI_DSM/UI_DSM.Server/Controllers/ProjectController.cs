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
    public class ProjectController : EntityController<Project, ProjectDto>
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
        ///     Gets a collection of all <see cref="ProjectDto" />
        /// </summary>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="ProjectDto" />as result</returns>
        [Authorize(Roles = "Administrator")]
        public override async Task<IActionResult> GetEntities()
        {
            var roles = await this.projectManager.GetEntities();
            return this.Ok(roles.Select(x => (ProjectDto)x.ToDto()));
        }

        /// <summary>
        ///     Get a <see cref="ProjectDto" /> based on its <see cref="Guid" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="ProjectDto" /> if found</returns>
        [Authorize]
        public override async Task<IActionResult> GetEntity(Guid entityId)
        {
            var project = await this.projectManager.GetEntity(entityId);
            return project == null ? this.NotFound() : this.Ok((ProjectDto)project.ToDto());
        }

        /// <summary>
        ///     Tries to create a new <see cref="Entity" /> based on its <see cref="ProjectDto" />
        /// </summary>
        /// <param name="dto">The <see cref="ProjectDto" /></param>
        /// <returns>A <see cref="Task" /> with the result of the creation</returns>
        [Authorize(Roles = "Administrator")]
        public override async Task<IActionResult> CreateEntity(ProjectDto dto)
        {
            var response = new EntityRequestResponseDto<ProjectDto>();

            if (dto.InstantiatePoco() is not Project project || project.Id != Guid.Empty)
            {
                response.Errors = new List<string>
                {
                    "Invalid DTO or the Id has to be empty"
                };

                return this.BadRequest(response);
            }

            project.ResolveProperties(dto);
            var identityResult = await this.projectManager.CreateEntity(project);
            return this.HandleOperationResult(response, identityResult);
        }

        /// <summary>
        ///     Tries to delete an <see cref="Entity" /> defined by the given <see cref="Guid" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="Entity" /> to delete</param>
        /// <returns>A <see cref="Task" /> with the result of the operation</returns>
        [Authorize(Roles = "Administrator")]
        public override async Task<IActionResult> DeleteEntity(Guid entityId)
        {
            var project = await this.projectManager.GetEntity(entityId);
            var response = new RequestResponseDto();

            if (project == null)
            {
                response.Errors = new List<string>
                {
                    $"Project with the id {entityId} does not exist"
                };

                return this.NotFound(response);
            }

            var identityResult = await this.projectManager.DeleteEntity(project);
            response.IsRequestSuccessful = identityResult.Succeeded;

            if (identityResult.Succeeded)
            {
                return this.Ok(response);
            }

            response.Errors = identityResult.Errors;
            return this.StatusCode(500, response);
        }

        /// <summary>
        ///     Tries to update an existing <see cref="Entity" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="Entity" /> to update</param>
        /// <param name="dto">The <see cref="ProjectDto" /> to update the <see cref="Entity" /></param>
        /// <returns>A <see cref="Task" /> with the result of the operation</returns>
        [Authorize(Roles = "Administrator")]
        public override async Task<IActionResult> UpdateEntity(Guid entityId, ProjectDto dto)
        {
            var existingProject = await this.projectManager.GetEntity(entityId);
            var response = new EntityRequestResponseDto<ProjectDto>();

            if (existingProject == null)
            {
                response.Errors = new List<string>
                {
                    $"Project with the id {entityId} does not exist"
                };

                return this.NotFound(response);
            }

            existingProject.ResolveProperties(dto);
            var identityResult = await this.projectManager.UpdateEntity(existingProject);
            return this.HandleOperationResult(response, identityResult);
        }
    }
}
