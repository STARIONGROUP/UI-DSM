// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewModule.cs" company="RHEA System S.A.">
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
    using Carter.Response;
    
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Server.Managers;
    using UI_DSM.Server.Managers.ReviewManager;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     The <see cref="ReviewModule" /> is a <see cref="ContainedEntityModule{TEntity,TEntityDto, TEntityContainer}" /> to manage
    ///     <see cref="Review" /> related to a <see cref="Project" />
    /// </summary>
    [Route("api/Project/{projectId:guid}/Review")]
    public class ReviewModule : ContainedEntityModule<Review, ReviewDto, Project>
    {
        /// <summary>
        ///     Adds routes to the <see cref="IEndpointRouteBuilder" />
        /// </summary>
        /// <param name="app">The <see cref="IEndpointRouteBuilder" /></param>
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            base.AddRoutes(app);

            app.MapGet($"{this.MainRoute}/OpenTasksAndComments", this.GetOpenTasksAndComments)
                .Produces<Dictionary<Guid, ComputedProjectProperties>>()
                .WithTags(this.EntityName)
                .WithName($"{this.EntityName}/GetOpenTasksAndComments");
        }

        /// <summary>
        ///     Gets a collection of all <see cref="Review" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public override async Task GetEntities(IEntityManager<Review> manager, HttpContext context, int deepLevel = 0)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, this.ContainerRouteKey);

            if (participant == null)
            {
                return;
            }

            await base.GetEntities(manager, context, deepLevel);
        }

        /// <summary>
        ///     Get a <see cref="Review" /> based on its <see cref="Guid" /> with all associated entities
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public override async Task GetEntity(IEntityManager<Review> manager, Guid entityId, HttpContext context, int deepLevel = 0)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, this.ContainerRouteKey);

            if (participant == null)
            {
                return;
            }

            await base.GetEntity(manager, entityId, context, deepLevel);
        }

        /// <summary>
        ///     Tries to create a new <see cref="Review" /> based on its <see cref="ReviewDto" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="dto">The <see cref="ReviewDto" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public override async Task CreateEntity(IEntityManager<Review> manager, ReviewDto dto, HttpContext context, int deepLevel = 0)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, this.ContainerRouteKey);

            if (participant == null)
            {
                return;
            }

            dto.Author = participant.Id;
            await base.CreateEntity(manager, dto, context, deepLevel);
        }

        /// <summary>
        ///     Tries to delete an <see cref="Review" /> defined by the given <see cref="Guid" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="Review" /> to delete</param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="RequestResponseDto" /> as result</returns>
        [Authorize]
        public override async Task<RequestResponseDto> DeleteEntity(IEntityManager<Review> manager, Guid entityId, HttpContext context)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, this.ContainerRouteKey);

            if (participant == null)
            {
                return new RequestResponseDto();
            }

            return await base.DeleteEntity(manager, entityId, context);
        }

        /// <summary>
        ///     Tries to update an existing <see cref="Review" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="Review" /></param>
        /// <param name="dto">The <see cref="ReviewDto" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" />as result</returns>
        [Authorize]
        public override async Task UpdateEntity(IEntityManager<Review> manager, Guid entityId, ReviewDto dto, HttpContext context, int deepLevel = 0)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, this.ContainerRouteKey);

            if (participant == null)
            {
                return;
            }

            await base.UpdateEntity(manager, entityId, dto, context, deepLevel);
        }

        /// <summary>
        ///     Adds the <see cref="Review" /> into the corresponding collection of the <see cref="Project" />
        /// </summary>
        /// <param name="entity">The <see cref="Review" /></param>
        /// <param name="container">The <see cref="Project" /></param>
        protected override void AddEntityIntoContainerCollection(Review entity, Project container)
        {
            container.Reviews.Add(entity);
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
        ///     Gets, for all <see cref="Review" />, the number of open <see cref="ReviewTask" /> and <see cref="Comment" />
        ///     related to the <see cref="Review" />
        /// </summary>
        /// <param name="reviewManager">The <see cref="IReviewManager"/></param>
        /// <param name="projectId"> The <see cref="Guid"/> of the <see cref="Project" /></param>
        /// <param name="context">The <see cref="HttpContext"/></param>
        /// <returns>A <see cref="Task"/></returns>
        [Authorize]
        public async Task GetOpenTasksAndComments(IReviewManager reviewManager,Guid projectId, HttpContext context)
        {
            var reviewsId = (await reviewManager.GetContainedEntities(projectId)).Select(x => x.Id);
            
            var computedProperties = await reviewManager.GetOpenTasksAndComments(reviewsId, projectId, context.User.Identity?.Name);
            await context.Response.Negotiate(computedProperties);
        }
    }
}
