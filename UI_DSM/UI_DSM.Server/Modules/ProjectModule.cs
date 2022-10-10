// --------------------------------------------------------------------------------------------------------
// <copyright file="ProjectModule.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Modules
{
    using Carter.Response;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using UI_DSM.Server.Managers;
    using UI_DSM.Server.Managers.ProjectManager;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Extensions;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     The <see cref="ProjectModule" /> is a <see cref="EntityModule{TEntity,TEntityDto}" /> to manage
    ///     <see cref="Project" />
    /// </summary>
    [Microsoft.AspNetCore.Components.Route("api/Project")]
    public class ProjectModule : EntityModule<Project, ProjectDto>
    {
        /// <summary>
        ///     Adds routes to the <see cref="IEndpointRouteBuilder" />
        /// </summary>
        /// <param name="app">The <see cref="IEndpointRouteBuilder" /></param>
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            base.AddRoutes(app);

            app.MapGet(this.MainRoute + "/UserParticipation", this.GetProjectsForUser)
                .Produces<IEnumerable<EntityDto>>()
                .WithTags(this.EntityName)
                .WithName($"{this.EntityName}/GetProjectsForUser");

            app.MapGet($"{this.MainRoute}/OpenTasksAndComments", this.GetOpenTasksAndComments)
                .Produces<Dictionary<Guid, ComputedProjectProperties>>()
                .WithTags(this.EntityName)
                .WithName($"{this.EntityName}/GetOpenTasksAndComments");
        }

        /// <summary>
        ///     Gets a collection of <see cref="Project" /> where a user is participating
        /// </summary>
        /// <param name="projectManager">The <see cref="IProjectManager" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns></returns>
        [Authorize]
        public async Task GetProjectsForUser(IProjectManager projectManager, HttpContext context)
        {
            var userName = context.User.Identity?.Name;
            var projects = await projectManager.GetAvailableProjectsForUser(userName);
            await context.Response.Negotiate(projects.ToDtos());
        }

        /// <summary>
        ///     Gets a collection of all <see cref="EntityDto" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize(Roles = "Administrator")]
        public override Task GetEntities(IEntityManager<Project> manager, HttpContext context, [FromQuery] int deepLevel = 0)
        {
            return base.GetEntities(manager, context, deepLevel);
        }

        /// <summary>
        ///     Get a <see cref="ProjectDto" /> based on its <see cref="Guid" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /> with the <see cref="ProjectDto" /> if found</returns>
        [Authorize]
        public override Task GetEntity(IEntityManager<Project> manager, Guid entityId, HttpContext context, [FromQuery] int deepLevel = 0)
        {
            return base.GetEntity(manager, entityId, context, deepLevel);
        }

        /// <summary>
        ///     Tries to create a new <see cref="Project" /> based on its <see cref="ProjectDto" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="dto">The <see cref="ProjectDto" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize(Roles = "Administrator")]
        public override Task CreateEntity(IEntityManager<Project> manager, ProjectDto dto, HttpContext context, [FromQuery] int deepLevel = 0)
        {
            return base.CreateEntity(manager, dto, context, deepLevel);
        }

        /// <summary>
        ///     Tries to delete an <see cref="Project" /> defined by the given <see cref="Guid" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="Project" /> to delete</param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="RequestResponseDto" /> as result</returns>
        [Authorize(Roles = "Administrator")]
        public override Task<RequestResponseDto> DeleteEntity(IEntityManager<Project> manager, Guid entityId, HttpContext context)
        {
            return base.DeleteEntity(manager, entityId, context);
        }

        /// <summary>
        ///     Tries to update an existing <see cref="Project" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <param name="dto">The <see cref="ProjectDto" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize(Roles = "Administrator")]
        public override Task UpdateEntity(IEntityManager<Project> manager, Guid entityId, ProjectDto dto, HttpContext context, [FromQuery] int deepLevel = 0)
        {
            return base.UpdateEntity(manager, entityId, dto, context, deepLevel);
        }

        /// <summary>
        ///     Gets, for all <see cref="Project" />, the number of open <see cref="ReviewTask" /> and <see cref="Comment" />
        ///     related to the <see cref="Project" />
        /// </summary>
        /// <param name="projectManager">The <see cref="IProjectManager"/></param>
        /// <param name="context">The <see cref="HttpContext"/></param>
        /// <returns>A <see cref="Task"/></returns>
        [Authorize]
        public async Task GetOpenTasksAndComments(IProjectManager projectManager, HttpContext context)
        {
            var projectsId = (await projectManager.GetAvailableProjectsForUser(context.User.Identity?.Name)).Select(x => x.Id) ;

            var computedProperties = await projectManager.GetOpenTasksAndComments(projectsId, context.User.Identity?.Name);
            await context.Response.Negotiate(computedProperties);
        }
    }
}
