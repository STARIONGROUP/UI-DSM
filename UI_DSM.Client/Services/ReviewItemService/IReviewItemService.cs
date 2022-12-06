// --------------------------------------------------------------------------------------------------------
// <copyright file="IReviewItemService.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Services.ReviewItemService
{
    using CDP4Common.CommonData;

    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.Types;

    /// <summary>
    ///     Interface definition for <see cref="ReviewItemService" />
    /// </summary>
    public interface IReviewItemService
    {
        /// <summary>
        ///     Gets all <see cref="ReviewItem" />s contained inside a <see cref="Review" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /> of the <see cref="Review" /></param>
        /// <param name="reviewId">The <see cref="Entity.Id" /> of the <see cref="Review" /></param>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> with the collection of <see cref="ReviewItem" /></returns>
        Task<List<ReviewItem>> GetReviewItemsOfReview(Guid projectId, Guid reviewId, int deepLevel = 0);

        /// <summary>
        ///     Gets a <see cref="ReviewItem" /> contained inside a <see cref="Review" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="reviewId">The <see cref="Guid" /> of the <see cref="Review" /></param>
        /// <param name="reviewItemId">The <see cref="Guid" /> of the <see cref="ReviewItem" /></param>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> with the collection of <see cref="ReviewItem" /></returns>
        Task<ReviewItem> GetReviewItemOfReview(Guid projectId, Guid reviewId, Guid reviewItemId, int deepLevel = 0);

        /// <summary>
        ///     Creates a new <see cref="ReviewItem" /> inside a <see cref="Review" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="reviewId">The <see cref="Guid" /> of the <see cref="Review" /></param>
        /// <param name="thingId">The <see cref="Guid" /> of the linked <see cref="Thing"/></param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityRequestResponse{ReviewItem}" /></returns>
        Task<EntityRequestResponse<ReviewItem>> CreateReviewItem(Guid projectId, Guid reviewId, Guid thingId);

        /// <summary>
        ///     Gets a <see cref="ReviewItem" /> that is linked to a <see cref="Thing" />
        /// </summary>
        /// <param name="projectId">The <see cref="Project" /> of the <see cref="Project" /></param>
        /// <param name="reviewId">The <see cref="Review" /> of the <see cref="Review" /></param>
        /// <param name="thingId">The <see cref="Thing" /> of the <see cref="Thing" /></param>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="ReviewItem" /> with the retrieved <see cref="Thing" /></returns>
        Task<ReviewItem> GetReviewItemForThing(Guid projectId, Guid reviewId, Guid thingId, int deepLevel = 0);

        /// <summary>
        ///     Gets a collection of <see cref="ReviewItem" />s that are linked to a <see cref="Thing" />
        /// </summary>
        /// <param name="projectId">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <param name="reviewId">The <see cref="Guid" /> of the <see cref="Review" /></param>
        /// <param name="thingIds">A collection of <see cref="Guid" />s of the <see cref="Thing" /></param>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> with the retrieved collection of <see cref="ReviewItem" /></returns>
        Task<List<ReviewItem>> GetReviewItemsForThings(Guid projectId, Guid reviewId, IEnumerable<Guid> thingIds, int deepLevel = 0);

        /// <summary>
        ///     Updates a <see cref="ReviewItem" />
        /// </summary>
        /// <param name="projectId">The <see cref="Project" /> id</param>
        /// <param name="reviewId">The <see cref="Review" /> id</param>
        /// <param name="reviewItem">The <see cref="ReviewItem" /> to update</param>
        /// <returns>A <see cref="Task" /> with the updated <see cref="ReviewItem" /></returns>
        Task<EntityRequestResponse<ReviewItem>> UpdateReviewItem(Guid projectId, Guid reviewId, ReviewItem reviewItem);
    }
}
