// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewItemManager.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Managers.ReviewItemManager
{
    using CDP4Common.CommonData;

    using Microsoft.EntityFrameworkCore;

    using UI_DSM.Server.Context;
    using UI_DSM.Server.Extensions;
    using UI_DSM.Server.Managers.AnnotationManager;
    using UI_DSM.Server.Managers.ReviewCategoryManager;
    using UI_DSM.Server.Types;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This manager handles operation to the Database for <see cref="ReviewItem" />s
    /// </summary>
    public class ReviewItemManager : ContainedEntityManager<ReviewItem, Review>, IReviewItemManager
    {
        /// <summary>
        ///     The <see cref="IAnnotationManager" />
        /// </summary>
        private readonly IAnnotationManager annotationManager;

        /// <summary>
        ///     The <see cref="IReviewCategoryManager" />
        /// </summary>
        private readonly IReviewCategoryManager reviewCategoryManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReviewItemManager" /> class.
        /// </summary>
        /// <param name="context">The <see cref="DatabaseContext" /></param>
        /// <param name="reviewCategoryManager">The <see cref="IReviewCategoryManager" /></param>
        /// <param name="annotationManager">The <see cref="IAnnotationManager" /></param>
        public ReviewItemManager(DatabaseContext context, IReviewCategoryManager reviewCategoryManager, IAnnotationManager annotationManager) : base(context)
        {
            this.reviewCategoryManager = reviewCategoryManager;
            this.annotationManager = annotationManager;
        }

        /// <summary>
        ///     Resolve all properties for the <see cref="ReviewItem" />
        /// </summary>
        /// <param name="entity">The <see cref="ReviewItem" /></param>
        /// <param name="dto">The <see cref="EntityDto" /></param>
        /// <returns>A <see cref="Task" /></returns>
        public override async Task ResolveProperties(ReviewItem entity, EntityDto dto)
        {
            if (dto is not ReviewItemDto reviewItemDto)
            {
                return;
            }

            var relatedEntities = new Dictionary<Guid, Entity>();
            relatedEntities.InsertEntityCollection(await this.reviewCategoryManager.FindEntities(reviewItemDto.ReviewCategories));
            relatedEntities.InsertEntityCollection(await this.annotationManager.FindEntities(reviewItemDto.Annotations));
            entity.ResolveProperties(reviewItemDto, relatedEntities);
        }

        /// <summary>
        ///     Gets a <see cref="ReviewItem" /> that targets a <see cref="Thing" />
        /// </summary>
        /// <param name="reviewId">The <see cref="Guid"/> of the container of the <see cref="ReviewItem"/></param>
        /// <param name="thingId">The <see cref="Guid" /> of the <see cref="Thing" /></param>
        /// <param name="deepLevel">The deepLevel</param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /></returns>
        public async Task<IEnumerable<Entity>> GetReviewItemForThing(Guid reviewId, Guid thingId, int deepLevel = 0)
        {
            var reviewItem = await this.EntityDbSet.Where(x => x.ThingId == thingId && x.EntityContainer.Id == reviewId)
                .BuildIncludeEntityQueryable(deepLevel)
                .ToListAsync();

            return reviewItem.SelectMany(x => x.GetAssociatedEntities(deepLevel)).ToList();
        }

        /// <summary>
        ///     Updates a <see cref="ReviewItem" />
        /// </summary>
        /// <param name="entity">The <see cref="ReviewItem" /> to update</param>
        /// <returns>A <see cref="Task" /> with the result of the update</returns>
        public override async Task<EntityOperationResult<ReviewItem>> UpdateEntity(ReviewItem entity)
        {
            if (!this.ValidateCurrentEntity(entity, out var entityOperationResult))
            {
                return entityOperationResult;
            }

            var foundEntity = await this.FindEntity(entity.Id);
            entity.ThingId = foundEntity.ThingId;

            return await this.UpdateEntityIntoContext(entity);
        }

        /// <summary>
        ///     Gets all <see cref="ReviewItem" /> that targets <see cref="Thing" />
        /// </summary>
        /// <param name="reviewId">The <see cref="Guid"/> of the container of the <see cref="ReviewItem"/></param>
        /// <param name="thingIds">A collection of <see cref="Guid" /> for <see cref="Thing" />s</param>
        /// <param name="deepLevel">The deepLevel</param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /></returns>
        public async Task<IEnumerable<Entity>> GetReviewItemsForThings(Guid reviewId, IEnumerable<Guid> thingIds, int deepLevel = 0)
        {
            var reviewItems = new List<Entity>();

            foreach (var thingId in thingIds)
            {
                reviewItems.AddRange(await this.GetReviewItemForThing(reviewId, thingId, deepLevel));
            }

            return reviewItems.DistinctBy(x => x.Id).ToList();
        }
    }
}
