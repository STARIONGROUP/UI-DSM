// --------------------------------------------------------------------------------------------------------
// <copyright file="AnnotationModule.cs" company="RHEA System S.A.">
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
    using System.Diagnostics.CodeAnalysis;

    using Carter.Response;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Server.Managers;
    using UI_DSM.Server.Managers.AnnotatableItemManager;
    using UI_DSM.Server.Managers.AnnotationManager;
    using UI_DSM.Server.Services.SearchService;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Extensions;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     The <see cref="AnnotationModule" />  is a
    ///     <see cref="ContainedEntityModule{TEntity,TEntityDto,TEntityContainer}" /> to manage
    ///     <see cref="Annotation" /> related to a <see cref="Project" />
    /// </summary>
    [Route("api/Project/{projectId:guid}/Annotation")]
    public class AnnotationModule : ContainedEntityModule<Annotation, AnnotationDto, Project>
    {
        /// <summary>
        ///     Gets a collection of all <see cref="Entity" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public override async Task GetEntities(IEntityManager<Annotation> manager, HttpContext context, int deepLevel = 0)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, this.ContainerRouteKey);

            if (participant == null)
            {
                return;
            }

            await base.GetEntities(manager, context, deepLevel);
        }

        /// <summary>
        ///     Adds routes to the <see cref="IEndpointRouteBuilder" />
        /// </summary>
        /// <param name="app">The <see cref="IEndpointRouteBuilder" /></param>
        [ExcludeFromCodeCoverage]
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            base.AddRoutes(app);

            app.MapGet(this.MainRoute + "/AnnotatableItem/{annotatableItemId:guid}", this.GetAnnotationsOfAnnotatableItem)
                .Produces<IEnumerable<EntityDto>>()
                .WithTags(this.EntityName)
                .WithName($"{this.EntityName}/GetAnnotationsOfAnnotatableItem");

            app.MapGet(this.MainRoute + "/ReviewTask/{reviewTaskId:guid}", this.GetAnnotationsForReviewTask)
                .Produces<IEnumerable<EntityDto>>()
                .WithTags(this.EntityName)
                .WithName($"{this.EntityName}/GetAnnotationsForReviewTask");

            app.MapGet(this.MainRoute + "/Review/{reviewId:guid}", this.GetAnnotationsForReview)
                .Produces<IEnumerable<EntityDto>>()
                .WithTags(this.EntityName)
                .WithName($"{this.EntityName}/GetAnnotationsForReview");
        }

        /// <summary>
        ///     Gets all <see cref="Annotation" /> that are linked to a <see cref="ReviewTask" />
        /// </summary>
        /// <param name="annotationManager">The <see cref="IAnnotationManager" /></param>
        /// <param name="projectId">The <see cref="Project" /> id</param>
        /// <param name="reviewTaskId">The <see cref="ReviewTask" /> id</param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public async Task GetAnnotationsForReviewTask(IAnnotationManager annotationManager, Guid projectId, Guid reviewTaskId, HttpContext context)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, this.ContainerRouteKey);

            if (participant == null)
            {
                return;
            }

            var annotations = await annotationManager.GetAnnotationsForReviewTask(projectId, reviewTaskId);
            await context.Response.Negotiate(annotations.ToDtos());
        }

        /// <summary>
        ///     Gets all <see cref="Annotation" /> that are linked to a <see cref="Review" />
        /// </summary>
        /// <param name="annotationManager">The <see cref="IAnnotationManager" /></param>
        /// <param name="projectId">The <see cref="Project" /> id</param>
        /// <param name="reviewId">The <see cref="Review" /> id</param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public async Task GetAnnotationsForReview(IAnnotationManager annotationManager, Guid projectId, Guid reviewId, HttpContext context)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, this.ContainerRouteKey);

            if (participant == null)
            {
                return;
            }

            var annotations = await annotationManager.GetAnnotationsForReview(projectId, reviewId);
            await context.Response.Negotiate(annotations.ToDtos());
        }

        /// <summary>
        ///     Get a <see cref="AnnotationDto" /> based on its <see cref="Guid" /> with all associated entities
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public override async Task GetEntity(IEntityManager<Annotation> manager, Guid entityId, HttpContext context, int deepLevel = 0)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, this.ContainerRouteKey);

            if (participant == null)
            {
                return;
            }

            await base.GetEntity(manager, entityId, context, deepLevel);
        }

        /// <summary>
        ///     Tries to create a new <see cref="Annotation" /> based on its <see cref="AnnotationDto" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="dto">The <see cref="AnnotationDto" /></param>
        /// <param name="searchService">The <see cref="ISearchService" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public override async Task CreateEntity(IEntityManager<Annotation> manager, AnnotationDto dto, ISearchService searchService, HttpContext context, int deepLevel = 0)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, this.ContainerRouteKey);

            if (participant == null)
            {
                return;
            }

            if (!await IsAllowedTo(context, participant, AccessRight.ReviewTask))
            {
                return;
            }

            _ = context.RequestServices.GetService<IAnnotatableItemManager>();
            dto.Author = participant.Id;
            await base.CreateEntity(manager, dto, searchService, context, deepLevel);
        }

        /// <summary>
        ///     Tries to delete an <see cref="Annotation" /> defined by the given <see cref="Guid" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="Annotation" /> to delete</param>
        /// <param name="searchService">The <see cref="ISearchService" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="RequestResponseDto" /> as result</returns>
        [Authorize]
        public override async Task<RequestResponseDto> DeleteEntity(IEntityManager<Annotation> manager, Guid entityId, ISearchService searchService, HttpContext context)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, this.ContainerRouteKey);

            if (participant == null)
            {
                return new RequestResponseDto();
            }

            var annotation = (await manager.GetEntity(entityId)).OfType<Annotation>().FirstOrDefault();

            if (annotation?.Author.Id != participant.Id)
            {
                context.Response.StatusCode = 403;

                return new RequestResponseDto
                {
                    Errors = new List<string> { "Unable to delete a Comment from someone else" }
                };
            }

            return await base.DeleteEntity(manager, entityId, searchService, context);
        }

        /// <summary>
        ///     Tries to update an existing <see cref="Annotation" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="Annotation" /></param>
        /// <param name="dto">The <see cref="AnnotationDto" /></param>
        /// <param name="searchService">The <see cref="ISearchService" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" />as result</returns>
        [Authorize]
        public override async Task UpdateEntity(IEntityManager<Annotation> manager, Guid entityId, AnnotationDto dto, ISearchService searchService, HttpContext context, int deepLevel = 0)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, this.ContainerRouteKey);

            if (participant == null)
            {
                return;
            }

            _ = context.RequestServices.GetService<IAnnotatableItemManager>();
            var annotation = (await manager.GetEntity(entityId)).OfType<Annotation>().FirstOrDefault();

            if (annotation?.Author.Id != participant.Id && annotation?.Content != dto.Content)
            {
                context.Response.StatusCode = 403;
                return;
            }

            await base.UpdateEntity(manager, entityId, dto, searchService, context, deepLevel);
        }

        /// <summary>
        ///     Gets all <see cref="Annotation" /> that are linked to the a <see cref="AnnotatableItem" />
        /// </summary>
        /// <param name="manager">The <see cref="IAnnotationManager" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="projectId">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <param name="annotatableItemId">The <see cref="Guid" /> of the <see cref="AnnotatableItem" /></param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public async Task GetAnnotationsOfAnnotatableItem(IAnnotationManager manager, HttpContext context, Guid projectId, Guid annotatableItemId)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, this.ContainerRouteKey);

            if (participant == null)
            {
                return;
            }

            var annotations = await manager.GetAnnotationsOfAnnotatableItem(projectId, annotatableItemId);
            await context.Response.Negotiate(annotations.ToDtos());
        }

        /// <summary>
        ///     Adds the <see cref="Annotation" /> into the corresponding collection of the <see cref="Project" />
        /// </summary>
        /// <param name="entity">The <see cref="Annotation" /></param>
        /// <param name="container">The <see cref="Project" /></param>
        protected override void AddEntityIntoContainerCollection(Annotation entity, Project container)
        {
            container.Annotations.Add(entity);
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
    }
}
