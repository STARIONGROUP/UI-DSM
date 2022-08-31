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
    using Carter.ModelBinding;
    using Carter.Response;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using UI_DSM.Server.Managers;
    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Server.Managers.ProjectManager;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     The <see cref="ParticipantModule" /> is a <see cref="EntityModule{TEntity,TEntityDto}" /> to manage
    ///     <see cref="Participant" /> involved into a <see cref="Project" />
    /// </summary>
    [Microsoft.AspNetCore.Components.Route("api/Project/{projectId:guid}/Participant")]
    public class ParticipantModule : EntityModule<Participant, ParticipantDto>
    {
        /// <summary>
        ///     Gets a collection of all <see cref="Entity" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public override async Task GetEntities(IEntityManager<Participant> manager, HttpContext context, [FromQuery] int deepLevel = 0)
        {
            var participantManager = (IParticipantManager)manager;
            var projectId = this.GetAdditionalRouteId(context.Request, "projectId");
            var entities = await participantManager.GetParticipantsOfProject(projectId, deepLevel);
            var entitiesDto = entities.Select(x => x.ToDto()).ToList();
            await context.Response.Negotiate(entitiesDto);
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
        public override async Task GetEntity(IEntityManager<Participant> manager, Guid entityId, HttpContext context, [FromQuery] int deepLevel = 0)
        {
            var participant = await manager.FindEntity(entityId);

            if (participant == null)
            {
                context.Response.StatusCode = 404;
                return;
            }

            var projectId = this.GetAdditionalRouteId(context.Request, "projectId");

            if (participant.EntityContainer == null || participant.EntityContainer.Id != projectId)
            {
                context.Response.StatusCode = 400;
                return;
            }

            var entities = await manager.GetEntity(entityId, deepLevel);
            await context.Response.Negotiate(entities.Select(x => x.ToDto()).ToList());
        }

        /// <summary>
        ///     Tries to create a new <see cref="Participant" /> based on its <see cref="ParticipantDto" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="dto">The <see cref="ParticipantDto" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize(Roles = "Administrator")]
        public override async Task CreateEntity(IEntityManager<Participant> manager, ParticipantDto dto, HttpContext context, [FromQuery] int deepLevel = 0)
        {
            var requestResponse = new EntityRequestResponseDto();
            var entity = this.ValidateEntityDtoAndCreateEntity(dto, context, requestResponse);

            if (entity == null)
            {
                await context.Response.Negotiate(requestResponse);
                return;
            }

            await manager.ResolveProperties(entity, dto);
            var projectManager = context.RequestServices.GetService<IProjectManager>();

            if (projectManager == null)
            {
                context.Response.StatusCode = 500;
                await context.Response.Negotiate(requestResponse);
                return;
            }

            var project = await projectManager.FindEntity(this.GetAdditionalRouteId(context.Request, "projectId"));

            if (project == null)
            {
                context.Response.StatusCode = 400;

                requestResponse.Errors = new List<string>
                {
                    "Unable to find the given project"
                };

                await context.Response.Negotiate(requestResponse);
                return;
            }

            project.Participants.Add(entity);
            var identityResult = await manager.CreateEntity(entity);
            this.HandleOperationResult(requestResponse, context.Response, identityResult, 201, deepLevel);
            await context.Response.Negotiate(requestResponse);
        }

        /// <summary>
        ///     Tries to update an existing <see cref="Participant" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="Participant" /></param>
        /// <param name="dto">The <see cref="ParticipantDto" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize(Roles = "Administrator")]
        public override async Task UpdateEntity(IEntityManager<Participant> manager, Guid entityId, ParticipantDto dto, HttpContext context, [FromQuery] int deepLevel = 0)
        {
            var participant = await manager.FindEntity(entityId);
            var requestResponse = new EntityRequestResponseDto();

            if (!this.ValidateEntityAndContainer(participant, "projectId", context, requestResponse))
            {
                await context.Response.Negotiate(requestResponse);
                return;
            }

            var validationResult = context.Request.Validate(dto);

            if (!validationResult.IsValid)
            {
                requestResponse.Errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                context.Response.StatusCode = 422;
                await context.Response.Negotiate(requestResponse);
                return;
            }

            await manager.ResolveProperties(participant, dto);
            var idendityResult = await manager.UpdateEntity(participant);
            this.HandleOperationResult(requestResponse, context.Response, idendityResult, deepLevel: deepLevel);
            await context.Response.Negotiate(requestResponse);
        }

        /// <summary>
        ///     Tries to delete an <see cref="Participant" /> defined by the given <see cref="Guid" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="Participant" /> to delete</param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="RequestResponseDto" /> as result</returns>
        [Authorize(Roles = "Administrator")]
        public override async Task<RequestResponseDto> DeleteEntity(IEntityManager<Participant> manager, Guid entityId, HttpContext context)
        {
            var participant = await manager.FindEntity(entityId);
            var requestResponse = new RequestResponseDto();

            if (!this.ValidateEntityAndContainer(participant, "projectId", context, requestResponse))
            {
                return requestResponse;
            }

            var identityResult = await manager.DeleteEntity(participant);
            requestResponse.IsRequestSuccessful = identityResult.Succeeded;

            if (!identityResult.Succeeded)
            {
                requestResponse.Errors = identityResult.Errors;
                context.Response.StatusCode = 500;
            }

            return requestResponse;
        }
    }
}
