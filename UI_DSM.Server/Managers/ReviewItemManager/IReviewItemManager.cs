// --------------------------------------------------------------------------------------------------------
// <copyright file="IReviewItemManager.cs" company="RHEA System S.A.">
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

    using UI_DSM.Server.Types;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface definition for <see cref="ReviewItemManager" />
    /// </summary>
    public interface IReviewItemManager : IContainedEntityManager<ReviewItem>
    {
        /// <summary>
        ///     Gets a <see cref="ReviewItem" /> that targets a <see cref="Thing" />
        /// </summary>
        /// <param name="reviewId">The <see cref="Guid" /> of the container of the <see cref="ReviewItem" /></param>
        /// <param name="thingId">The <see cref="Guid" /> of the <see cref="Thing" /></param>
        /// <param name="deepLevel">The deepLevel</param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /></returns>
        Task<IEnumerable<Entity>> GetReviewItemForThing(Guid reviewId, Guid thingId, int deepLevel = 0);

        /// <summary>
        ///     Gets all <see cref="ReviewItem" /> that targets <see cref="Thing" />
        /// </summary>
        /// <param name="reviewId">The <see cref="Guid" /> of the container of the <see cref="ReviewItem" /></param>
        /// <param name="thingIds">A collection of <see cref="Guid" /> for <see cref="Thing" />s</param>
        /// <param name="deepLevel">The deepLevel</param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /></returns>
        Task<IEnumerable<Entity>> GetReviewItemsForThings(Guid reviewId, IEnumerable<Guid> thingIds, int deepLevel = 0);

        /// <summary>
        ///     Link the <see cref="Annotation" /> to all <see cref="ReviewItem" /> that are linked to a <see cref="Thing" /> id
        /// </summary>
        /// <param name="review">The <see cref="Review" /> container</param>
        /// <param name="annotation">The <see cref="Annotation" /></param>
        /// <param name="thingsId">A collection of <see cref="Guid" /> of <see cref="Thing" />s</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityOperationResult{ReviewItem}" /></returns>
        Task<EntityOperationResult<ReviewItem>> LinkAnnotationToItems(Review review, Annotation annotation, IEnumerable<Guid> thingsId);
    }
}
