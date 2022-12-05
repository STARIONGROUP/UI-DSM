// --------------------------------------------------------------------------------------------------------
// <copyright file="IReviewObjectiveService.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Services.ReviewObjectiveService
{
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.Types;

    /// <summary>
    ///     Interface definition for <see cref="ReviewObjectiveService" />
    /// </summary>
    public interface IReviewObjectiveService
    {
        /// <summary>
        ///     Gets all <see cref="ReviewObjective" />s contained inside a <see cref="Review" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /> of the <see cref="Review" /></param>
        /// <param name="reviewId">The <see cref="Entity.Id" /> of the <see cref="Review" /></param>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> with the collection of <see cref="ReviewObjective" /></returns>
        Task<List<ReviewObjective>> GetReviewsObjectivesOfReview(Guid projectId, Guid reviewId, int deepLevel = 0);

        /// <summary>
        ///     Gets a <see cref="ReviewObjective" /> contained inside a <see cref="Review" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="reviewId">The <see cref="Guid" /> of the <see cref="Review" /></param>
        /// <param name="reviewObjectiveId">The <see cref="Guid" /> of the <see cref="ReviewObjective" /></param>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> with the collection of <see cref="ReviewObjective" /></returns>
        Task<ReviewObjective> GetReviewObjectiveOfReview(Guid projectId, Guid reviewId, Guid reviewObjectiveId, int deepLevel = 0);

        /// <summary>
        ///     Creates a new <see cref="ReviewObjective" /> inside a <see cref="Review" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="reviewId">The <see cref="Guid" /> of the <see cref="Review" /></param>
        /// <param name="reviewObjective">The <see cref="ReviewObjectiveCreationDto" />to create</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityRequestResponse{ReviewObjective}" /></returns>
        Task<EntityRequestResponse<ReviewObjective>> CreateReviewObjective(Guid projectId, Guid reviewId, ReviewObjectiveCreationDto reviewObjective);

        /// <summary>
        ///     Creates a new <see cref="ReviewObjective" /> inside a <see cref="Review" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="reviewId">The <see cref="Guid" /> of the <see cref="Review" /></param>
        /// <param name="reviewObjectives">The <see cref="IEnumerable{ReviewObjectiveCreationDto}" />to create</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityRequestResponse{ReviewObjective}" /></returns>
        Task<EntitiesRequestResponses<ReviewObjective>> CreateReviewObjectives(Guid projectId, Guid reviewId, IEnumerable<ReviewObjectiveCreationDto> reviewObjectives);

        /// <summary>
        ///     Updates a <see cref="ReviewObjective" />
        /// </summary>
        /// <param name="projectId">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <param name="reviewObjective">The <see cref="ReviewObjective" />to update</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityRequestResponse{ReviewObjective}" /></returns>
        Task<EntityRequestResponse<ReviewObjective>> UpdateReviewObjective(Guid projectId, ReviewObjective reviewObjective);

        /// <summary>
        ///     Deletes a <see cref="ReviewObjective" />
        /// </summary>
        /// <param name="projectId">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <param name="reviewObjective">The <see cref="ReviewObjective" /> to delete</param>
        /// <returns>A <see cref="Task" /> with the <see cref="RequestResponseDto" /></returns>
        Task<RequestResponseDto> DeleteReviewObjective(Guid projectId, ReviewObjective reviewObjective);

        /// <summary>
        ///     Gets, all <see cref="ReviewObjectiveCreationDto" />s from the json file
        ///     and filters them based on the given <see cref="ReviewObjectiveKind" />
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /> of the <see cref="Review" /></param>
        /// <param name="reviewId">The <see cref="Entity.Id" /> of the <see cref="Review" /></param>
        /// </summary>
        /// <returns>A <see cref="Task" /> with the <see cref="List{ReviewObjectiveCreationDto}" /></returns>
        Task<List<ReviewObjectiveCreationDto>> GetAvailableTemplates(Guid projectId, Guid reviewId);

        /// <summary>
        ///     Gets, for a <see cref="ReviewObjective" />, the number of open <see cref="ReviewTask" /> and <see cref="Comment" />
        ///     related to the <see cref="ReviewObjective" />
        /// </summary>
        /// <returns>A <see cref="Task" /> with a <see cref="Dictionary{Guid, ComputedProjectProperties}" /></returns>
        Task<Dictionary<Guid, ComputedProjectProperties>> GetOpenTasksAndComments(Guid projectId, Guid reviewId);
    }
}
