// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewObjectiveManager.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Managers.ReviewObjectiveManager
{
    using Microsoft.EntityFrameworkCore;

    using UI_DSM.Server.Context;
    using UI_DSM.Server.Extensions;
    using UI_DSM.Server.Managers.AnnotationManager;
    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Server.Managers.ReviewCategoryManager;
    using UI_DSM.Server.Managers.ReviewTaskManager;
    using UI_DSM.Server.Types;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This manager handles operation to the Database for <see cref="ReviewObjective" />s
    /// </summary>
    public class ReviewObjectiveManager : ContainedEntityManager<ReviewObjective, Review>, IReviewObjectiveManager
    {
        /// <summary>
        ///     The <see cref="IAnnotationManager" />
        /// </summary>
        private readonly IAnnotationManager annotationManager;

        /// <summary>
        ///     The <see cref="IParticipantManager" />
        /// </summary>
        private readonly IParticipantManager participantManager;

        /// <summary>
        ///     The <see cref="IReviewCategoryManager" />
        /// </summary>
        private readonly IReviewCategoryManager reviewCategoryManager;

        /// <summary>
        ///     The <see cref="IReviewTaskManager" />
        /// </summary>
        private readonly IReviewTaskManager reviewTaskManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReviewObjectiveManager" /> class.
        /// </summary>
        /// <param name="context">The <see cref="DatabaseContext" /></param>
        /// <param name="participantManager">The <see cref="IParticipantManager" /></param>
        /// <param name="reviewTaskManager">The <see cref="IReviewTaskManager" /></param>
        /// <param name="annotationManager">The <see cref="IAnnotationManager" /></param>
        /// <param name="reviewCategoryManager">The <see cref="IReviewCategoryManager"/></param>
        public ReviewObjectiveManager(DatabaseContext context, IParticipantManager participantManager, IReviewTaskManager reviewTaskManager,
            IAnnotationManager annotationManager, IReviewCategoryManager reviewCategoryManager) : base(context)
        {
            this.participantManager = participantManager;
            this.reviewTaskManager = reviewTaskManager;
            this.annotationManager = annotationManager;
            this.reviewCategoryManager = reviewCategoryManager;
        }

        /// <summary>
        ///     Resolve all properties for the <see cref="ReviewObjective" />
        /// </summary>
        /// <param name="entity">The <see cref="ReviewObjective" /></param>
        /// <param name="dto">The <see cref="EntityDto" /></param>
        /// <returns>A <see cref="Task" /></returns>
        public override async Task ResolveProperties(ReviewObjective entity, EntityDto dto)
        {
            if (dto is not ReviewObjectiveDto reviewObjectiveDto)
            {
                return;
            }

            var relatedEntities = new Dictionary<Guid, Entity>();
            relatedEntities.InsertEntity(await this.participantManager.FindEntity(reviewObjectiveDto.Author));
            relatedEntities.InsertEntityCollection(await this.reviewTaskManager.FindEntities(reviewObjectiveDto.ReviewTasks));
            relatedEntities.InsertEntityCollection(await this.annotationManager.FindEntities(reviewObjectiveDto.Annotations));
            relatedEntities.InsertEntityCollection(await this.reviewCategoryManager.FindEntities(reviewObjectiveDto.ReviewCategories));
            entity.ResolveProperties(reviewObjectiveDto, relatedEntities);
        }

        /// <summary>
        ///     Updates a <see cref="ReviewObjective" />
        /// </summary>
        /// <param name="entity">The <see cref="ReviewObjective" /> to update</param>
        /// <returns>A <see cref="Task" /> with the result of the update</returns>
        public override async Task<EntityOperationResult<ReviewObjective>> UpdateEntity(ReviewObjective entity)
        {
            if (!this.ValidateCurrentEntity(entity, out var entityOperationResult))
            {
                return entityOperationResult;
            }

            var foundEntity = await this.FindEntity(entity.Id);
            entity.ReviewObjectiveKind = foundEntity.ReviewObjectiveKind;

            return await this.UpdateEntityIntoContext(entity);
        }

        /// <summary>
        ///     Creates an <see cref="ReviewObjective" /> based on a template
        /// </summary>
        /// <param name="template">The <see cref="ReviewObjective" /> template</param>
        /// <param name="container">The <see cref="Review" /> container</param>
        /// <param name="author">The <see cref="Participant" /> author</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityOperationResult{TEntity}" /></returns>
        public async Task<EntityOperationResult<ReviewObjective>> CreateEntityBasedOnTemplate(ReviewObjective template, Review container, Participant author)
        {
            if (container.ReviewObjectives.Any(x => x.ReviewObjectiveKind == template.ReviewObjectiveKind
                                                    && x.ReviewObjectiveKindNumber == template.ReviewObjectiveKindNumber))
            {
                return EntityOperationResult<ReviewObjective>.Failed($"A ReviewObjective {template.Title} already exists inside the Review");
            }

            var reviewObjective = new ReviewObjective(template)
            {
                Author = author
            };

            container.ReviewObjectives.Add(reviewObjective);
            this.SetSpecificPropertiesBeforeCreate(reviewObjective);
            var operationResult = new EntityOperationResult<ReviewObjective>(this.Context.Add(reviewObjective), EntityState.Added);

            if (operationResult.Entity != null)
            {
                this.reviewTaskManager.CreateEntitiesBasedOnTemplate(operationResult.Entity, template.ReviewTasks, author);
            }

            try
            {
                await this.Context.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                this.HandleException(exception, operationResult);
            }

            return operationResult;
        }

        /// <summary>
        ///     Sets specific properties before the creation of the <see cref="ReviewObjective" />
        /// </summary>
        /// <param name="entity">The <see cref="ReviewObjective" /></param>
        protected override void SetSpecificPropertiesBeforeCreate(ReviewObjective entity)
        {
            var review = entity.EntityContainer as Review;
            entity.ReviewObjectiveNumber = 0;
            entity.ReviewObjectiveNumber = review!.ReviewObjectives.Max(x => x.ReviewObjectiveNumber) + 1;
            entity.CreatedOn = DateTime.UtcNow;
            entity.Status = StatusKind.Open;
        }
    }
}
