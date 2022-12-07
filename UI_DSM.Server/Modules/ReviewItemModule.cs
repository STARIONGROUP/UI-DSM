// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewItemModule.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw, Nabil Abbar
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

    using CDP4Common.CommonData;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using UI_DSM.Server.Managers;
    using UI_DSM.Server.Managers.ReviewItemManager;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Extensions;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     The <see cref="ReviewObjectiveModule" /> is a
    ///     <see cref="ContainedEntityModule{TEntity,TEntityDto, TEntityContainer}" /> to manage
    ///     <see cref="ReviewItem" /> related to a <see cref="Review" />
    /// </summary>
    [Microsoft.AspNetCore.Components.Route("api/Project/{projectId:guid}/Review/{reviewId:guid}/ReviewItem")]
    public class ReviewItemModule : ContainedEntityModule<ReviewItem, ReviewItemDto, Review>
    {
        /// <summary>
        ///     The route key for the ProjectId
        /// </summary>
        private const string ProjectKey = "projectId";

        /// <summary>
        ///     Adds routes to the <see cref="IEndpointRouteBuilder" />
        /// </summary>
        /// <param name="app">The <see cref="IEndpointRouteBuilder" /></param>
        [ExcludeFromCodeCoverage]
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            base.AddRoutes(app);

            app.MapGet(this.MainRoute + "/ForThing/{thingId:guid}", this.GetEntityForThing)
                .Produces<IEnumerable<Entity>>()
                .WithTags(this.EntityName)
                .WithName($"{this.EntityName}/GetEntityForThing");

            app.MapPost(this.MainRoute + "/ForThings", this.GetEntitiesForThings)
                .Accepts<IEnumerable<Guid>>("application/json")
                .Produces<IEnumerable<Entity>>()
                .WithTags(this.EntityName)
                .WithName($"{this.EntityName}/GetEntitiesForThings");
        }

        /// <summary>
        ///     Gets a <see cref="ReviewItem" /> that is linked to the provided <see cref="Thing" />
        /// </summary>
        /// <param name="manager">The <see cref="IReviewItemManager" /></param>
        /// <param name="thingId">The <see cref="Guid" /> of the related <see cref="Thing" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">The optionnal deeplevel</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public async Task GetEntityForThing(IReviewItemManager manager, Guid thingId, HttpContext context, [FromQuery] int deepLevel = 0)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, ProjectKey);

            if (participant == null)
            {
                return;
            }

            if (!await this.AdditionalRouteValidation(context))
            {
                return;
            }

            var reviewId = this.GetAdditionalRouteId(context.Request, this.ContainerRouteKey);

            var entities = await manager.GetReviewItemForThing(reviewId, thingId, deepLevel);
            await context.Response.Negotiate(entities.ToDtos());
        }

        /// <summary>
        ///     Gets all <see cref="ReviewItem" /> that are linked to a provided <see cref="Thing" />
        /// </summary>
        /// <param name="manager">The <see cref="IReviewItemManager" /></param>
        /// <param name="thingIds">The collection <see cref="Guid" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">The optionnal deeplevel</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public async Task GetEntitiesForThings(IReviewItemManager manager, [FromBody] IEnumerable<Guid> thingIds, HttpContext context, [FromQuery] int deepLevel = 0)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, ProjectKey);

            if (participant == null)
            {
                return;
            }

            if (!await this.AdditionalRouteValidation(context))
            {
                return;
            }

            var reviewId = this.GetAdditionalRouteId(context.Request, this.ContainerRouteKey);

            var entities = await manager.GetReviewItemsForThings(reviewId, thingIds, deepLevel);
            await context.Response.Negotiate(entities.ToDtos());
        }

        /// <summary>
        ///     Gets a collection of all <see cref="Entity" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public override async Task GetEntities(IEntityManager<ReviewItem> manager, HttpContext context, int deepLevel = 0)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, ProjectKey);

            if (participant == null)
            {
                return;
            }

            await base.GetEntities(manager, context, deepLevel);
        }

        /// <summary>
        ///     Get a <see cref="ReviewItemDto" /> based on its <see cref="Guid" /> with all associated entities
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public override async Task GetEntity(IEntityManager<ReviewItem> manager, Guid entityId, HttpContext context, int deepLevel = 0)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, ProjectKey);

            if (participant == null)
            {
                return;
            }

            await base.GetEntity(manager, entityId, context, deepLevel);
        }

        /// <summary>
        ///     Tries to create a new <see cref="ReviewItem" /> based on its <see cref="ReviewItemDto" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="dto">The <see cref="ReviewItemDto" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public override async Task CreateEntity(IEntityManager<ReviewItem> manager, ReviewItemDto dto, HttpContext context, int deepLevel = 0)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, ProjectKey);

            if (participant == null)
            {
                return;
            }

            var reviewId = this.GetAdditionalRouteId(context.Request, this.ContainerRouteKey);

            if (manager is not IReviewItemManager reviewItemManager)
            {
                context.Response.StatusCode = 500;
                return;
            }

            var existingItem = await reviewItemManager.GetReviewItemForThing(reviewId, dto.ThingId);

            if (existingItem.Any())
            {
                context.Response.StatusCode = 409;
                return;
            }

            await base.CreateEntity(manager, dto, context, deepLevel);
        }

        /// <summary>
        ///     Tries to delete an <see cref="ReviewItem" /> defined by the given <see cref="Guid" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="ReviewItem" /> to delete</param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="RequestResponseDto" /> as result</returns>
        [Authorize]
        public override async Task<RequestResponseDto> DeleteEntity(IEntityManager<ReviewItem> manager, Guid entityId, HttpContext context)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, ProjectKey);

            if (participant == null)
            {
                return new RequestResponseDto();
            }

            return await base.DeleteEntity(manager, entityId, context);
        }

        /// <summary>
        ///     Tries to update an existing <see cref="ReviewItem" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="ReviewItem" /></param>
        /// <param name="dto">The <see cref="ReviewItemDto" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" />as result</returns>
        [Authorize]
        public override async Task UpdateEntity(IEntityManager<ReviewItem> manager, Guid entityId, ReviewItemDto dto, HttpContext context, int deepLevel = 0)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, ProjectKey);

            if (participant == null)
            {
                return;
            }
            
            await base.UpdateEntity(manager, entityId, dto, context, deepLevel);
        }

        /// <summary>
        ///     Adds the <see cref="ReviewItem" /> into the corresponding collection of the <see cref="Review" />
        /// </summary>
        /// <param name="entity">The <see cref="ReviewItem" /></param>
        /// <param name="container">The <see cref="Review" /></param>
        protected override void AddEntityIntoContainerCollection(ReviewItem entity, Review container)
        {
            container.ReviewItems.Add(entity);
        }

        /// <summary>
        ///     Verifies if the provided routes is correctly formatted with all containment
        /// </summary>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /> with the result of the check</returns>
        protected override async Task<bool> AdditionalRouteValidation(HttpContext context)
        {
            var reviewManager = context.RequestServices.GetService<IContainedEntityManager<Review>>();

            if (reviewManager == null)
            {
                context.Response.StatusCode = 500;
                return false;
            }

            var reviewId = this.GetAdditionalRouteId(context.Request, this.ContainerRouteKey);
            var projectId = this.GetAdditionalRouteId(context.Request, ProjectKey);

            if (!await reviewManager.EntityIsContainedBy(reviewId, projectId))
            {
                context.Response.StatusCode = 400;
                return false;
            }

            return true;
        }
    }
}
