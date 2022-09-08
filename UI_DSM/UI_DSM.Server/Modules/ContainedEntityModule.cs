// --------------------------------------------------------------------------------------------------------
// <copyright file="ContainedEntityModule.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw
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

    using Microsoft.AspNetCore.Components;

    using UI_DSM.Server.Managers;
    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Extensions;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     The <see cref="ContainedEntityModule{TEntity,TEntityDto, TEntityContainer}" /> is an
    ///     <see cref="EntityModule{TEntity,TEntityDto}" />
    ///     for <see cref="Entity" /> that have a container
    /// </summary>
    /// <typeparam name="TEntity">An <see cref="Entity" /></typeparam>
    /// <typeparam name="TEntityDto">An <see cref="EntityDto" /></typeparam>
    /// <typeparam name="TEntityContainer">The container of <see cref="TEntity" /></typeparam>
    public abstract class ContainedEntityModule<TEntity, TEntityDto, TEntityContainer> : EntityModule<TEntity, TEntityDto> where TEntity : Entity
        where TEntityDto : EntityDto
        where TEntityContainer : Entity
    {
        /// <summary>
        ///     An array of <see cref="string" /> that contains the key inside the route for accessing to the request parameter
        /// </summary>
        protected readonly string ContainerRouteKey;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContainedEntityModule{TEntity,TEntityDto, TEntityContainer}" /> class.
        /// </summary>
        protected ContainedEntityModule()
        {
            this.ContainerRouteKey = this.RetrieveLastRouteParameter();
        }

        /// <summary>
        ///     Gets a collection of all <see cref="Entity" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        public override async Task GetEntities(IEntityManager<TEntity> manager, HttpContext context, int deepLevel = 0)
        {
            if (!await this.AdditionalRouteValidation(context))
            {
                return;
            }

            var containedManager = (IContainedEntityManager<TEntity>)manager;
            var containerId = this.GetAdditionalRouteId(context.Request, this.ContainerRouteKey);
            var entities = await containedManager.GetContainedEntities(containerId, deepLevel);
            await context.Response.Negotiate(entities.ToDtos());
        }

        /// <summary>
        ///     Get a <see cref="TEntityDto" /> based on its <see cref="Guid" /> with all associated entities
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        public override async Task GetEntity(IEntityManager<TEntity> manager, Guid entityId, HttpContext context, int deepLevel = 0)
        {
            if (!await this.AdditionalRouteValidation(context))
            {
                return;
            }

            var entity = await manager.FindEntity(entityId);

            if (entity == null)
            {
                context.Response.StatusCode = 404;
                return;
            }

            var containerId = this.GetAdditionalRouteId(context.Request, this.ContainerRouteKey);

            if (entity.EntityContainer == null || entity.EntityContainer.Id != containerId)
            {
                context.Response.StatusCode = 400;
                return;
            }

            await context.Response.Negotiate(entity.GetAssociatedEntities(deepLevel).ToDtos());
        }

        /// <summary>
        ///     Tries to create a new <see cref="TEntity" /> based on its <see cref="TEntityDto" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="dto">The <see cref="TEntityDto" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        public override async Task CreateEntity(IEntityManager<TEntity> manager, TEntityDto dto, HttpContext context, int deepLevel = 0)
        {
            var requestResponse = new EntityRequestResponseDto();

            if (!await this.AdditionalRouteValidation(context))
            {
                requestResponse.Errors = new List<string>
                {
                    "Route validation failure"
                };

                await context.Response.Negotiate(requestResponse);
                return;
            }

            var entity = this.ValidateEntityDtoAndCreateEntity(dto, context, requestResponse);

            if (entity == null)
            {
                await context.Response.Negotiate(requestResponse);
                return;
            }

            await manager.ResolveProperties(entity, dto);

            var containerManager = context.RequestServices.GetService<IEntityManager<TEntityContainer>>();

            if (containerManager == null)
            {
                context.Response.StatusCode = 500;
                await context.Response.Negotiate(requestResponse);
                return;
            }

            var container = await containerManager.FindEntity(this.GetAdditionalRouteId(context.Request, this.ContainerRouteKey));

            if (container == null)
            {
                context.Response.StatusCode = 400;

                requestResponse.Errors = new List<string>
                {
                    "Unable to find the given container"
                };

                await context.Response.Negotiate(requestResponse);
                return;
            }

            this.AddEntityIntoContainerCollection(entity, container);
            var identityResult = await manager.CreateEntity(entity);
            this.HandleOperationResult(requestResponse, context.Response, identityResult, 201, deepLevel);
            await context.Response.Negotiate(requestResponse);
        }

        /// <summary>
        ///     Tries to delete an <see cref="TEntity" /> defined by the given <see cref="Guid" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="TEntity" /> to delete</param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="RequestResponseDto" /> as result</returns>
        public override async Task<RequestResponseDto> DeleteEntity(IEntityManager<TEntity> manager, Guid entityId, HttpContext context)
        {
            if (!await this.AdditionalRouteValidation(context))
            {
                return new RequestResponseDto
                {
                    Errors = new List<string>
                    {
                        "Route validation failure"
                    }
                };
            }

            if (manager is not IContainedEntityManager<TEntity> containedManager)
            {
                context.Response.StatusCode = 500;

                return new RequestResponseDto
                {
                    Errors = new List<string>
                    {
                        "Internal error"
                    }
                };
            }

            var entity = await containedManager.FindEntityWithContainer(entityId);
            var requestResponse = new RequestResponseDto();

            if (!this.ValidateEntityAndContainer(entity, this.ContainerRouteKey, context, requestResponse))
            {
                return requestResponse;
            }

            var identityResult = await manager.DeleteEntity(entity);
            requestResponse.IsRequestSuccessful = identityResult.Succeeded;

            if (!identityResult.Succeeded)
            {
                requestResponse.Errors = identityResult.Errors;
                context.Response.StatusCode = 500;
            }

            return requestResponse;
        }

        /// <summary>
        ///     Tries to update an existing <see cref="TEntity" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="TEntity" /></param>
        /// <param name="dto">The <see cref="TEntityDto" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" />as result</returns>
        public override async Task UpdateEntity(IEntityManager<TEntity> manager, Guid entityId, TEntityDto dto, HttpContext context, int deepLevel = 0)
        {
            var requestResponse = new EntityRequestResponseDto();

            if (!await this.AdditionalRouteValidation(context))
            {
                requestResponse.Errors = new List<string>
                {
                    "Route validation failure"
                };

                await context.Response.Negotiate(requestResponse);
                return;
            }

            if (manager is not IContainedEntityManager<TEntity> containedManager)
            {
                context.Response.StatusCode = 500;

                requestResponse.Errors = new List<string>
                {
                    "Internal error"
                };

                await context.Response.Negotiate(requestResponse);
                return;
            }

            var entity = await containedManager.FindEntityWithContainer(entityId);

            if (!this.ValidateEntityAndContainer(entity, this.ContainerRouteKey, context, requestResponse))
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

            await manager.ResolveProperties(entity, dto);
            var idendityResult = await manager.UpdateEntity(entity);
            this.HandleOperationResult(requestResponse, context.Response, idendityResult, deepLevel: deepLevel);
            await context.Response.Negotiate(requestResponse);
        }

        /// <summary>
        ///     Adds the <see cref="TEntity" /> into the corresponding collection of the <see cref="TEntityContainer" />
        /// </summary>
        /// <param name="entity">The <see cref="TEntity" /></param>
        /// <param name="container">The <see cref="TEntityContainer" /></param>
        protected abstract void AddEntityIntoContainerCollection(TEntity entity, TEntityContainer container);

        /// <summary>
        ///     Verifies if the provided routes is correctly formatted with all containment
        /// </summary>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /> with the result of the check</returns>
        protected abstract Task<bool> AdditionalRouteValidation(HttpContext context);

        /// <summary>
        ///     Gets a <see cref="Participant" /> based on the identity of the current User for a <see cref="Project" />
        /// </summary>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="projectKey">The key to get the <see cref="Project" /> id</param>
        /// <returns>A <see cref="Task" /> with a <see cref="Participant" /> if found</returns>
        protected async Task<Participant> GetParticipantBasedOnRequest(HttpContext context, string projectKey)
        {
            var participantManager = context.RequestServices.GetService<IParticipantManager>();

            if (participantManager == null)
            {
                context.Response.StatusCode = 500;
                return null;
            }

            var projectId = this.GetAdditionalRouteId(context.Request, projectKey);
            var participant = await participantManager.GetParticipantForProject(projectId, context.User.Identity?.Name);

            if (participant == null)
            {
                context.Response.StatusCode = 401;
                return null;
            }

            return participant;
        }

        /// <summary>
        ///     Gets the last parameter contained into the <see cref="RouteAttribute" /> of a
        ///     <see cref="ContainedEntityModule{TEntity,TEntityDto,TEntityContainer}" />
        /// </summary>
        /// <returns>The last parameter</returns>
        private string RetrieveLastRouteParameter()
        {
            var splittedRoute = this.MainRoute.Split("/");
            var lastParameter = splittedRoute.LastOrDefault(x => x.StartsWith("{") && x.EndsWith("}"));

            if (lastParameter == null)
            {
                throw new ArgumentException("The main route is incorrect for a contained Entity");
            }

            return lastParameter.Substring(1, lastParameter.Length - 2).Split(":")[0];
        }
    }
}
