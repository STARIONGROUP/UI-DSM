// --------------------------------------------------------------------------------------------------------
// <copyright file="IReviewService.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Services.ReviewService
{
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.Types;

    /// <summary>
    ///     Interface definition for <see cref="ReviewService" />
    /// </summary>
    public interface IReviewService
    {
        /// <summary>
        ///     Gets all <see cref="Review" />s contained inside a <see cref="Project" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> with the collection of <see cref="Review" /></returns>
        Task<List<Review>> GetReviewsOfProject(Guid projectId, int deepLevel = 0);

        /// <summary>
        ///     Gets a <see cref="Review" /> contained inside a <see cref="Project" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="reviewId">The <see cref="Guid" /> of the <see cref="Review" /></param>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> with the collection of <see cref="Review" /></returns>
        Task<Review> GetReviewOfProject(Guid projectId, Guid reviewId, int deepLevel = 0);

        /// <summary>
        ///     Creates a new <see cref="Review" /> inside a <see cref="Project" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="review">The <see cref="Review" />to create</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityRequestResponse{Review}" /></returns>
        Task<EntityRequestResponse<Review>> CreateReview(Guid projectId, Review review);

        /// <summary>
        ///     Updates a <see cref="Review" />
        /// </summary>
        /// <param name="review">The <see cref="Review" />to update</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityRequestResponse{Review}" /></returns>
        Task<EntityRequestResponse<Review>> UpdateReview(Review review);

        /// <summary>
        ///     Deletes a <see cref="Review" />
        /// </summary>
        /// <param name="review">The <see cref="Review" /> to delete</param>
        /// <returns>A <see cref="Task" /> with the <see cref="RequestResponseDto" /></returns>
        Task<RequestResponseDto> DeleteReview(Review review);
    }
}
