// --------------------------------------------------------------------------------------------------------
// <copyright file="IReviewTaskService.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Services.ReviewTaskService
{
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.Types;

    /// <summary>
    ///     Interface definition for <see cref="ReviewTaskService" />
    /// </summary>
    public interface IReviewTaskService
    {
        /// <summary>
        ///     Gets all <see cref="ReviewTask" />s contained inside a <see cref="ReviewObjective" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /> of the <see cref="Review" /></param>
        /// <param name="reviewId">
        ///     The <see cref="Entity.Id" /> of the <see cref="Review" /> of the <see cref="ReviewObjective" />
        /// </param>
        /// <param name="reviewObjectiveId">
        ///     The <see cref="Entity.Id" /> of the <see cref="ReviewObjective" /> of
        ///     <see cref="ReviewTask" />s
        /// </param>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> with the collection of <see cref="ReviewTask" /></returns>
        Task<List<ReviewTask>> GetTasksOfReviewObjectives(Guid projectId, Guid reviewId, Guid reviewObjectiveId, int deepLevel = 0);

        /// <summary>
        ///     Gets a <see cref="ReviewTask" /> contained inside a <see cref="ReviewObjective" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="reviewId">The <see cref="Guid" /> of the <see cref="Review" /></param>
        /// <param name="reviewObjectiveId">The <see cref="Guid" /> of the <see cref="ReviewObjective" /></param>
        /// <param name="taskReviewId">The <see cref="Guid" /> of the <see cref="ReviewTask" /></param>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> with the collection of <see cref="ReviewTask" /></returns>
        Task<ReviewTask> GetTaskOfReviewObjective(Guid projectId, Guid reviewId, Guid reviewObjectiveId, Guid taskReviewId, int deepLevel = 0);

        /// <summary>
        ///     Updates a <see cref="ReviewTask" />
        /// </summary>
        /// <param name="projectId">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <param name="reviewId">The <see cref="Guid" /> of the <see cref="Review" /></param>
        /// <param name="reviewTask">The <see cref="ReviewTask" />to update</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityRequestResponse{ReviewObjective}" /></returns>
        Task<EntityRequestResponse<ReviewTask>> UpdateReviewTask(Guid projectId, Guid reviewId, ReviewTask reviewTask);

        /// <summary>
        ///     Deletes a <see cref="ReviewTask" />
        /// </summary>
        /// <param name="projectId">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <param name="reviewId">The <see cref="Guid" /> of the <see cref="Review" /></param>
        /// <param name="reviewTask">The <see cref="ReviewTask" /> to delete</param>
        /// <returns>A <see cref="Task" /> with the <see cref="RequestResponseDto" /></returns>
        Task<RequestResponseDto> DeleteReviewTask(Guid projectId, Guid reviewId, ReviewTask reviewTask);
    }
}
