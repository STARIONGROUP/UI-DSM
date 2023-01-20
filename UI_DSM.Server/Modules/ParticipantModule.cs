// --------------------------------------------------------------------------------------------------------
// <copyright file="ParticipantModule.cs" company="RHEA System S.A.">
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
    using System.Diagnostics.CodeAnalysis;

    using Carter.Response;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using UI_DSM.Server.Managers;
    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Server.Services.SearchService;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Extensions;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     The <see cref="ParticipantModule" /> is a <see cref="EntityModule{TEntity,TEntityDto}" /> to manage
    ///     <see cref="Participant" /> involved into a <see cref="Project" />
    /// </summary>
    [Microsoft.AspNetCore.Components.Route("api/Project/{projectId:guid}/Participant")]
    public class ParticipantModule : ContainedEntityModule<Participant, ParticipantDto, Project>
    {
        /// <summary>
        ///     Adds routes to the <see cref="IEndpointRouteBuilder" />
        /// </summary>
        /// <param name="app">The <see cref="IEndpointRouteBuilder" /></param>
        [ExcludeFromCodeCoverage]
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            base.AddRoutes(app);

            app.MapGet($"{this.MainRoute}/AvailableUsers", this.GetAvailableUsers)
                .Produces<IEnumerable<EntityDto>>()
                .Produces<IEnumerable<EntityDto>>(404)
                .WithTags(this.EntityName)
                .WithName($"{this.EntityName}/AvailableUsers");

            app.MapGet($"{this.MainRoute}/LoggedUser", this.GetLoggedParticipant)
                .Produces<IEnumerable<EntityDto>>()
                .Produces<IEnumerable<EntityDto>>(404)
                .WithTags(this.EntityName)
                .WithName($"{this.EntityName}/LoggedUser");
        }

        /// <summary>
        ///     Gets the current logged <see cref="Participant" />
        /// </summary>
        /// <param name="participantManager">The <see cref="IParticipantManager" /></param>
        /// <param name="projectManager">The <see cref="IEntityManager{Project}" /></param>
        /// <param name="projectId">The <see cref="Project" /> id</param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public async Task GetLoggedParticipant(IParticipantManager participantManager, IEntityManager<Project> projectManager, Guid projectId, HttpContext context)
        {
            var project = await projectManager.FindEntity(projectId);

            if (project == null)
            {
                context.Response.StatusCode = 404;
                return;
            }

            var participant = await participantManager.GetParticipantForProject(projectId, context.User!.Identity!.Name);

            if (participant == null)
            {
                context.Response.StatusCode = 401;
                return;
            }

            await context.Response.Negotiate(participant.GetAssociatedEntities().ToDtos());
        }

        /// <summary>
        ///     Get a collection of <see cref="UserEntity" /> that are not involved into the given project
        /// </summary>
        /// <param name="participantManager">The <see cref="IParticipantManager" /></param>
        /// <param name="projectManager">The <see cref="IEntityManager{Project}" /></param>
        /// <param name="projectId">The <see cref="Project" /> id</param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public async Task GetAvailableUsers(IParticipantManager participantManager, IEntityManager<Project> projectManager, Guid projectId, HttpContext context)
        {
            if (!await IsAuthorizedParticipant(participantManager, context, projectId))
            {
                return;
            }

            var existingProject = await projectManager.FindEntity(projectId);

            if (existingProject == null)
            {
                context.Response.StatusCode = 404;
                return;
            }

            var availableUsers = await participantManager.GetAvailableUsersForParticipantCreation(projectId);
            await context.Response.Negotiate(availableUsers.ToDtos());
        }

        /// <summary>
        ///     Gets a collection of all <see cref="Entity" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public override Task GetEntities(IEntityManager<Participant> manager, HttpContext context, [FromQuery] int deepLevel = 0)
        {
            return base.GetEntities(manager, context, deepLevel);
        }

        /// <summary>
        ///     Get a <see cref="Participant" /> based on its <see cref="Guid" /> with all associated entities
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public override Task GetEntity(IEntityManager<Participant> manager, Guid entityId, HttpContext context, [FromQuery] int deepLevel = 0)
        {
            return base.GetEntity(manager, entityId, context, deepLevel);
        }

        /// <summary>
        ///     Tries to create a new <see cref="Participant" /> based on its <see cref="ParticipantDto" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="dto">The <see cref="ParticipantDto" /></param>
        /// <param name="searchService">The <see cref="ISearchService"/></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public override async Task CreateEntity(IEntityManager<Participant> manager, ParticipantDto dto, ISearchService searchService, HttpContext context, 
            [FromQuery] int deepLevel = 0)
        {
            if (!await IsAuthorizedParticipant((IParticipantManager)manager, context, this.GetAdditionalRouteId(context.Request, this.ContainerRouteKey)))
            {
                return;
            }

            await base.CreateEntity(manager, dto, searchService, context, deepLevel);
        }

        /// <summary>
        ///     Tries to update an existing <see cref="Participant" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="Participant" /></param>
        /// <param name="dto">The <see cref="ParticipantDto" /></param>
        /// <param name="searchService">The <see cref="ISearchService"/></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public override async Task UpdateEntity(IEntityManager<Participant> manager, Guid entityId, ParticipantDto dto, ISearchService searchService, HttpContext context, [FromQuery] int deepLevel = 0)
        {
            if (!await IsAuthorizedParticipant((IParticipantManager)manager, context, this.GetAdditionalRouteId(context.Request, this.ContainerRouteKey)))
            {
                return;
            }

            await base.UpdateEntity(manager, entityId, dto, searchService, context, deepLevel);
        }

        /// <summary>
        ///     Tries to delete an <see cref="Participant" /> defined by the given <see cref="Guid" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="Participant" /> to delete</param>
        /// <param name="searchService">The <see cref="ISearchService"/></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="RequestResponseDto" /> as result</returns>
        [Authorize]
        public override async Task<RequestResponseDto> DeleteEntity(IEntityManager<Participant> manager, Guid entityId,ISearchService searchService, HttpContext context)
        {
            if (!await IsAuthorizedParticipant((IParticipantManager)manager, context, this.GetAdditionalRouteId(context.Request, this.ContainerRouteKey)))
            {
                return new RequestResponseDto()
                {
                    Errors = {"Unauthorized user"}
                };
            }

            return await base.DeleteEntity(manager, entityId, searchService, context);
        }

        /// <summary>
        ///     Adds the <see cref="Participant" /> into the corresponding collection of the <see cref="Project" />
        /// </summary>
        /// <param name="entity">The <see cref="Participant" /></param>
        /// <param name="container">The <see cref="Project" /></param>
        protected override void AddEntityIntoContainerCollection(Participant entity, Project container)
        {
            container.Participants.Add(entity);
        }

        /// <summary>
        ///     Verifies if the provided routes is correctly formatted with all containment
        /// </summary>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /> with the result of the check</returns>
        protected override async Task<bool> AdditionalRouteValidation(HttpContext context)
        {
            await Task.CompletedTask;
            return true;
        }

        /// <summary>
        ///     Verifies that a <see cref="Participant" /> has sufficient access right for management
        /// </summary>
        /// <param name="participantManager">The <see cref="IParticipantManager" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="projectId">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <returns>A <see cref="Task" /> with the resut </returns>
        private static async Task<bool> IsAuthorizedParticipant(IParticipantManager participantManager, HttpContext context, Guid projectId)
        {
            if (!context.User.IsInRole("Administrator"))
            {
                var participant = await participantManager.GetParticipantForProject(projectId, context.User.Identity?.Name);

                if (participant == null || !participant.IsAllowedTo(AccessRight.ProjectManagement))
                {
                    context.Response.StatusCode = 403;
                    return false;
                }
            }

            return true;
        }
    }
}
