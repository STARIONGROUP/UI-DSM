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

    using UI_DSM.Server.Managers;
    using UI_DSM.Server.Types;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
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
                .Produces<IEnumerable<TEntityDto>>()
                .WithTags(this.EntityName)
                .WithName($"{this.EntityName}/GetEntities");

            app.MapGet(this.MainRoute + "/{entityId:guid}", this.GetEntity)
                .Produces<TEntityDto>()
                .Produces<TEntityDto>(404)
                .WithTags(this.EntityName)
                .WithName($"{this.EntityName}/GetEntity");

            app.MapPost($"{this.MainRoute}/Create", this.CreateEntity)
                .Accepts<TEntityDto>("application/json")
                .Produces<EntityRequestResponseDto<TEntityDto>>(201)
                .Produces<EntityRequestResponseDto<TEntityDto>>(422)
                .Produces<EntityRequestResponseDto<TEntityDto>>(500)
                .WithTags(this.EntityName)
                .WithName($"{this.EntityName}/CreateEntity");

            app.MapDelete(this.MainRoute + "/{entityId:guid}", this.DeleteEntity)
                .Produces<RequestResponseDto>()
                .Produces<RequestResponseDto>(404)
                .Produces<RequestResponseDto>(500)
                .WithTags(this.EntityName)
                .WithName($"{this.EntityName}/DeleteEntity");

            app.MapPut(this.MainRoute + "/{entityId:guid}", this.UpdateEntity)
                .Produces<EntityRequestResponseDto<TEntityDto>>()
                .Produces<EntityRequestResponseDto<TEntityDto>>(404)
                .Produces<EntityRequestResponseDto<TEntityDto>>(422)
                .Produces<EntityRequestResponseDto<TEntityDto>>(500)
                .WithTags(this.EntityName)
                .WithName($"{this.EntityName}/UpdateEntity");
        }

        /// <summary>
        ///     Gets a collection of all <see cref="TEntityDto" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="TEntityDto" /> as result</returns>
        public virtual async Task<IEnumerable<TEntityDto>> GetEntities(IEntityManager<TEntity> manager)
        {
            var entities = await manager.GetEntities();
            return entities.Select(x => (TEntityDto)x.ToDto());
        }

        /// <summary>
        ///     Get a <see cref="TEntityDto" /> based on its <see cref="Guid" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <param name="response">The <see cref="HttpResponse" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="TEntityDto" /> if found</returns>
        public virtual async Task<TEntityDto> GetEntity(IEntityManager<TEntity> manager, Guid entityId, HttpResponse response)
        {
            var entity = await manager.GetEntity(entityId);

            if (entity == null)
            {
                response.StatusCode = 404;
                return null;
            }

            return (TEntityDto)entity.ToDto();
        }

        /// <summary>
        ///     Tries to create a new <see cref="TEntity" /> based on its <see cref="TEntityDto" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="dto">The <see cref="TEntityDto" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityRequestResponseDto{TEntityDto}" /> as result</returns>
        public virtual async Task<EntityRequestResponseDto<TEntityDto>> CreateEntity(IEntityManager<TEntity> manager, TEntityDto dto, HttpContext context)
        {
            var requestReponse = new EntityRequestResponseDto<TEntityDto>();
            var entity = this.ValidateEntityDtoAndCreateEntity(dto, context, requestReponse);

            if (entity == null)
            {
                return requestReponse;
            }

            var identityResult = await manager.CreateEntity(entity);
            this.HandleOperationResult(requestReponse, context.Response, identityResult, 201);
            return requestReponse;
        }

        /// <summary>
        ///     Tries to delete an <see cref="TEntity" /> defined by the given <see cref="Guid" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="TEntity" /> to delete</param>
        /// <param name="response">The <see cref="HttpResponse" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="RequestResponseDto" /> as result</returns>
        public virtual async Task<RequestResponseDto> DeleteEntity(IEntityManager<TEntity> manager, Guid entityId, HttpResponse response)
        {
            var entity = await manager.GetEntity(entityId);
            var requestResponse = new RequestResponseDto();

            if (entity == null)
            {
                requestResponse.Errors = new List<string>
                {
                    $"{this.EntityName} with the id {entityId} does not exist"
                };

                response.StatusCode = 404;
                return requestResponse;
            }

            var identityResult = await manager.DeleteEntity(entity);
            requestResponse.IsRequestSuccessful = identityResult.Succeeded;

            if (!identityResult.Succeeded)
            {
                requestResponse.Errors = identityResult.Errors;
                response.StatusCode = 500;
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
        /// <returns>A <see cref="Task" /> with <see cref="EntityRequestResponseDto{TEntityDto}" /> as result</returns>
        public virtual async Task<EntityRequestResponseDto<TEntityDto>> UpdateEntity(IEntityManager<TEntity> manager, Guid entityId, TEntityDto dto, HttpContext context)
        {
            var entity = await manager.GetEntity(entityId);
            var requestResponse = new EntityRequestResponseDto<TEntityDto>();

            if (entity == null)
            {
                requestResponse.Errors = new List<string>
                {
                    $"{this.EntityName} with the id {entityId} does not exist"
                };

                context.Response.StatusCode = 404;
                return requestResponse;
            }

            var validationResult = context.Request.Validate(dto);

            if (!validationResult.IsValid)
            {
                requestResponse.Errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                context.Response.StatusCode = 422;
                return requestResponse;
            }

            entity.ResolveProperties(dto);
            var idendityResult = await manager.UpdateEntity(entity);
            this.HandleOperationResult(requestResponse, context.Response, idendityResult);
            return requestResponse;
        }

        /// <summary>
        ///     Handles the result of the <see cref="EntityOperationResult{TEntity}" />
        /// </summary>
        /// <param name="requestReponse">The <see cref="EntityRequestResponseDto{TEntityDto}" /> to reply</param>
        /// <param name="httpResponse">The <see cref="HttpResponse" /></param>
        /// <param name="identityResult">The <see cref="EntityRequestResponseDto{TEntityDto}" /></param>
        /// <param name="successStatusCode">The <see cref="HttpResponse.StatusCode" /> in case of success</param>
        protected void HandleOperationResult(EntityRequestResponseDto<TEntityDto> requestReponse,
            HttpResponse httpResponse, EntityOperationResult<TEntity> identityResult, int successStatusCode = 200)
        {
            requestReponse.IsRequestSuccessful = identityResult.Succeeded;

            if (!identityResult.Succeeded)
            {
                requestReponse.Errors = identityResult.Errors;
                httpResponse.StatusCode = 500;
                return;
            }

            requestReponse.Entity = identityResult.Entity.ToDto() as TEntityDto;
            httpResponse.StatusCode = successStatusCode;
        }

        /// <summary>
        ///     Validates the given <see cref="TEntityDto" /> and creates the corresponding <see cref="TEntity" />
        /// </summary>
        /// <param name="dto">The <see cref="TEntityDto" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="requestResponse">The <see cref="requestResponse" /></param>
        /// <returns>The created <see cref="TEntity" /></returns>
        protected TEntity ValidateEntityDtoAndCreateEntity(TEntityDto dto, HttpContext context, EntityRequestResponseDto<TEntityDto> requestResponse)
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

            entity.ResolveProperties(dto);
            return entity;
        }
    }
}
