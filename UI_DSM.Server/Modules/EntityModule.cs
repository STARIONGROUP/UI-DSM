// --------------------------------------------------------------------------------------------------------
// <copyright file="EntityModule.cs" company="RHEA System S.A.">
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

    using Carter.ModelBinding;
    using Carter.Response;

    using Microsoft.AspNetCore.Mvc;

    using UI_DSM.Serializer.Json;
    using UI_DSM.Server.Managers;
    using UI_DSM.Server.Services.SearchService;
    using UI_DSM.Server.Types;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Extensions;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Base class for all <see cref="ModuleBase" /> handling <see cref="Entity" />
    /// </summary>
    /// <typeparam name="TEntity">An <see cref="Entity" /></typeparam>
    /// <typeparam name="TEntityDto">An <see cref="EntityDto" /></typeparam>
    public abstract class EntityModule<TEntity, TEntityDto> : ModuleBase where TEntity : Entity where TEntityDto : EntityDto
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EntityModule{TEntity,TEntityDto}" /> class.
        /// </summary>
        protected EntityModule()
        {
            this.EntityName = typeof(TEntity).Name;
        }

        /// <summary>
        ///     The name of the type of the <see cref="TEntity" />
        /// </summary>
        protected string EntityName { get; private set; }

        /// <summary>
        ///     Adds routes to the <see cref="IEndpointRouteBuilder" />
        /// </summary>
        /// <param name="app">The <see cref="IEndpointRouteBuilder" /></param>
        [ExcludeFromCodeCoverage]
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet(this.MainRoute, this.GetEntities)
                .Produces<IEnumerable<EntityDto>>()
                .WithTags(this.EntityName)
                .WithName($"{this.EntityName}/GetEntities");

            app.MapGet(this.MainRoute + "/{entityId:guid}", this.GetEntity)
                .Produces<IEnumerable<EntityDto>>()
                .Produces<IEnumerable<EntityDto>>(404)
                .WithTags(this.EntityName)
                .WithName($"{this.EntityName}/GetEntity");

            app.MapPost($"{this.MainRoute}/Create", this.ConvertEntityAndCreate)
                .Accepts<TEntityDto>("application/json")
                .Produces<EntityRequestResponseDto>(201)
                .Produces<EntityRequestResponseDto>(422)
                .Produces<EntityRequestResponseDto>(500)
                .WithTags(this.EntityName)
                .WithName($"{this.EntityName}/CreateEntity");

            app.MapDelete(this.MainRoute + "/{entityId:guid}", this.DeleteEntity)
                .Produces<RequestResponseDto>()
                .Produces<RequestResponseDto>(404)
                .Produces<RequestResponseDto>(500)
                .WithTags(this.EntityName)
                .WithName($"{this.EntityName}/DeleteEntity");

            app.MapPut(this.MainRoute + "/{entityId:guid}", this.ConvertEntityAndUpdate)
                .Produces<EntityRequestResponseDto>()
                .Produces<EntityRequestResponseDto>(404)
                .Produces<EntityRequestResponseDto>(422)
                .Produces<EntityRequestResponseDto>(500)
                .WithTags(this.EntityName)
                .WithName($"{this.EntityName}/UpdateEntity");
        }

        /// <summary>
        ///     Gets a collection of all <see cref="Entity" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        public virtual async Task GetEntities(IEntityManager<TEntity> manager, HttpContext context, [FromQuery] int deepLevel = 0)
        {
            var entities = await manager.GetEntities(deepLevel);
            var entitiesDto = entities.ToDtos();
            await context.Response.Negotiate(entitiesDto);
        }

        /// <summary>
        ///     Get a <see cref="TEntityDto" /> based on its <see cref="Guid" /> with all associated entities
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        public virtual async Task GetEntity(IEntityManager<TEntity> manager, Guid entityId, HttpContext context, [FromQuery] int deepLevel = 0)
        {
            var entities = (await manager.GetEntity(entityId, deepLevel)).ToList();

            if (entities.Count == 0)
            {
                context.Response.StatusCode = 404;
                return;
            }

            await context.Response.Negotiate(entities.ToDtos());
        }

        /// <summary>
        ///     Tries to create a new <see cref="TEntity" /> based on its <see cref="TEntityDto" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="dto">The <see cref="TEntityDto" /></param>
        /// <param name="searchService">The <see cref="ISearchService"/></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        public virtual async Task CreateEntity(IEntityManager<TEntity> manager, TEntityDto dto, ISearchService searchService, HttpContext context, [FromQuery] int deepLevel = 0)
        {
            var requestReponse = new EntityRequestResponseDto();
            var entity = this.ValidateEntityDtoAndCreateEntity(dto, context, requestReponse);

            if (entity == null)
            {
                await context.Response.Negotiate(requestReponse);
                return;
            }

            await manager.ResolveProperties(entity, dto);
            var identityResult = await manager.CreateEntity(entity);
            this.HandleOperationResult(requestReponse, context.Response, identityResult, 201, deepLevel);
            await context.Response.Negotiate(requestReponse);

            if (identityResult.Succeeded)
            {
                await searchService.IndexData((TEntityDto)identityResult.Entity.ToDto());
            }
        }

        /// <summary>
        ///     Tries to delete an <see cref="TEntity" /> defined by the given <see cref="Guid" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="TEntity" /> to delete</param>
        /// <param name="searchService">The <see cref="ISearchService"/></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="RequestResponseDto" /> as result</returns>
        public virtual async Task<RequestResponseDto> DeleteEntity(IEntityManager<TEntity> manager, Guid entityId, ISearchService searchService, HttpContext context)
        {
            var entity = await manager.FindEntity(entityId);
            var requestResponse = new RequestResponseDto();

            if (entity == null)
            {
                requestResponse.Errors = new List<string>
                {
                    $"{this.EntityName} with the id {entityId} does not exist"
                };

                context.Response.StatusCode = 404;
                return requestResponse;
            }

            var identityResult = await manager.DeleteEntity(entity);
            requestResponse.IsRequestSuccessful = identityResult.Succeeded;

            if (!identityResult.Succeeded)
            {
                requestResponse.Errors = identityResult.Errors;
                context.Response.StatusCode = 500;
            }

            await searchService.DeleteIndexedData(entity.ToDto());
            return requestResponse;
        }

        /// <summary>
        ///     Tries to update an existing <see cref="TEntity" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="TEntity" /></param>
        /// <param name="dto">The <see cref="TEntityDto" /></param>
        /// <param name="searchService">The <see cref="ISearchService"/></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" />as result</returns>
        public virtual async Task UpdateEntity(IEntityManager<TEntity> manager, Guid entityId, TEntityDto dto, ISearchService searchService, 
            HttpContext context, [FromQuery] int deepLevel = 0)
        {
            var entity = await manager.FindEntity(entityId);
            var requestResponse = new EntityRequestResponseDto();

            if (entity == null)
            {
                requestResponse.Errors = new List<string>
                {
                    $"{this.EntityName} with the id {entityId} does not exist"
                };

                context.Response.StatusCode = 404;
                await context.Response.Negotiate(requestResponse);
                return;
            }

            await this.ValidateAndUpdateEntity(manager, entity, dto, searchService, context, requestResponse, deepLevel);
        }

        /// <summary>
        ///     Deserialize the body of the request as an <see cref="TEntityDto" />
        /// </summary>
        /// <param name="deserializer">The <see cref="IJsonDeserializer" /></param>
        /// <param name="context">The <see cref="HttpRequest" /></param>
        /// <returns>The deserialized <see cref="TEntityDto" /></returns>
        protected TEntityDto DeserializeBodyRequest(IJsonDeserializer deserializer, HttpContext context)
        {
            if (!context.Request.HasJsonContentType())
            {
                return null;
            }

            var dto = deserializer.Deserialize(context.Request.BodyReader.AsStream()).FirstOrDefault() as TEntityDto;
            return dto;
        }

        /// <summary>
        ///     Validates the found <see cref="TEntityDto" /> and tries to update the <see cref="TEntity" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entity">The <see cref="TEntity" /></param>
        /// <param name="dto">The <see cref="TEntityDto" /></param>
        /// <param name="searchService">The <see cref="ISearchService"/></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="requestResponse">The <see cref="EntityRequestResponseDto" /></param>
        /// <param name="deepLevel">The deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        protected async Task ValidateAndUpdateEntity(IEntityManager<TEntity> manager, TEntity entity, TEntityDto dto, ISearchService searchService,
            HttpContext context, EntityRequestResponseDto requestResponse, int deepLevel)
        {
            var validationResult = context.Request.Validate(dto);

            if (!validationResult.IsValid)
            {
                requestResponse.Errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();

                context.Response.StatusCode = 422;
                await context.Response.Negotiate(requestResponse);
                return;
            }

            await manager.ResolveProperties(entity, dto);
            var identityResult = await manager.UpdateEntity(entity);
            this.HandleOperationResult(requestResponse, context.Response, identityResult, deepLevel: deepLevel);
            await context.Response.Negotiate(requestResponse);

            if (identityResult.Succeeded)
            {
                await searchService.IndexData((TEntityDto)identityResult.Entity.ToDto());
            }
        }

        /// <summary>
        ///     Handles the result of the <see cref="EntityOperationResult{TEntity}" />
        /// </summary>
        /// <param name="requestReponse">The <see cref="EntityRequestResponseDto" /> to reply</param>
        /// <param name="httpResponse">The <see cref="HttpResponse" /></param>
        /// <param name="identityResult">The <see cref="EntityRequestResponseDto" /></param>
        /// <param name="successStatusCode">The <see cref="HttpResponse.StatusCode" /> in case of success</param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        protected void HandleOperationResult(EntityRequestResponseDto requestReponse,
            HttpResponse httpResponse, EntityOperationResult<TEntity> identityResult, int successStatusCode = 200, int deepLevel = 0)
        {
            requestReponse.IsRequestSuccessful = identityResult.Succeeded;

            if (!identityResult.Succeeded)
            {
                requestReponse.Errors = identityResult.Errors;
                httpResponse.StatusCode = 500;
                return;
            }

            if (identityResult.Entity != null)
            {
                requestReponse.Entities = identityResult.Entity.GetAssociatedEntities(deepLevel).ToDtos();
            }
            else
            {
                requestReponse.Entities = identityResult.Entities.SelectMany(x => x.GetAssociatedEntities(deepLevel)).ToDtos();
            }

            httpResponse.StatusCode = successStatusCode;
        }

        /// <summary>
        ///     Validates the given <see cref="TEntityDto" /> and creates the corresponding <see cref="TEntity" />
        /// </summary>
        /// <param name="dto">The <see cref="TEntityDto" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="requestResponse">The <see cref="requestResponse" /></param>
        /// <returns>The created <see cref="TEntity" /></returns>
        protected TEntity ValidateEntityDtoAndCreateEntity(TEntityDto dto, HttpContext context, EntityRequestResponseDto requestResponse)
        {
            var validationResult = context.Request.Validate(dto);

            if (!validationResult.IsValid)
            {
                requestResponse.Errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                context.Response.StatusCode = 422;
                return null;
            }

            if (dto.InstantiatePoco() is not TEntity entity || entity.Id != Guid.Empty)
            {
                requestResponse.Errors = new List<string>
                {
                    "Invalid DTO or the Id has to be empty"
                };

                context.Response.StatusCode = 422;
                return null;
            }

            return entity;
        }

        /// <summary>
        ///     Validates the <see cref="Entity" /> and its <see cref="Entity.EntityContainer" />
        /// </summary>
        /// <param name="entity">The <see cref="Entity" /></param>
        /// <param name="keyId">The key identifier for the container</param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="requestResponse">The <see cref="RequestResponseDto" /></param>
        /// <returns>A value indicating if the <see cref="Entity" /> and its container are valid</returns>
        protected bool ValidateEntityAndContainer(Entity entity, string keyId, HttpContext context, RequestResponseDto requestResponse)
        {
            if (entity == null)
            {
                requestResponse.Errors = new List<string>
                {
                    $"{this.EntityName} with the id {this.GetAdditionalRouteId(context.Request, "entityId")} does not exist"
                };

                context.Response.StatusCode = 404;
                return false;
            }

            if (entity.EntityContainer == null || entity.EntityContainer.Id != this.GetAdditionalRouteId(context.Request, keyId))
            {
                context.Response.StatusCode = 400;

                requestResponse.Errors = new List<string>
                {
                    "Invalid container ID"
                };

                return false;
            }

            return true;
        }

        /// <summary>
        ///     Verifies that a <see cref="Participant" /> is allowed to do an action
        /// </summary>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="participant">The current <see cref="Participant" /></param>
        /// <param name="requestedAccess">The requested <see cref="AccessRight" /></param>
        /// <returns>A <see cref="Task" /> with the assert</returns>
        protected static async Task<bool> IsAllowedTo(HttpContext context, Participant participant, AccessRight requestedAccess)
        {
            if (!participant.IsAllowedTo(requestedAccess))
            {
                var requestResponse = new EntityRequestResponseDto
                {
                    Errors = new List<string> { "You don't have requested access right" }
                };

                context.Response.StatusCode = 403;
                await context.Response.Negotiate(requestResponse);
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Deserialize the <see cref="TEntityDto" /> and tries to create the corresponding POCO
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="deserializer">The <see cref="IJsonDeserializer" /></param>
        /// <param name="searchService">The <see cref="ISearchService"/></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">The deepLevel</param>
        /// <returns>A <see cref="Task" /></returns>
        private Task ConvertEntityAndCreate(IEntityManager<TEntity> manager, IJsonDeserializer deserializer, ISearchService searchService, HttpContext context, [FromQuery] int deepLevel = 0)
        {
            var dto = this.DeserializeBodyRequest(deserializer, context);

            if (dto == null)
            {
                context.Response.StatusCode = 400;
                return Task.CompletedTask;
            }

            return this.CreateEntity(manager, dto, searchService, context, deepLevel);
        }

        /// <summary>
        ///     Deserialize the <see cref="TEntityDto" /> and tries to update the corresponding POCO
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="deserializer">The <see cref="IJsonDeserializer" /></param>
        /// <param name="searchService">The <see cref="ISearchService"/></param>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="TEntity" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">The deepLevel</param>
        /// <returns>A <see cref="Task" /></returns>
        private Task ConvertEntityAndUpdate(IEntityManager<TEntity> manager, IJsonDeserializer deserializer, ISearchService searchService,
            Guid entityId, HttpContext context, [FromQuery] int deepLevel = 0)
        {
            var dto = this.DeserializeBodyRequest(deserializer, context);

            if (dto == null)
            {
                context.Response.StatusCode = 400;
                return Task.CompletedTask;
            }

            return this.UpdateEntity(manager, entityId, dto, searchService, context, deepLevel);
        }
    }
}
