// --------------------------------------------------------------------------------------------------------
// <copyright file="IReviewObjectiveManager.cs" company="RHEA System S.A.">
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
    using UI_DSM.Server.Types;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface definition for <see cref="ReviewObjectiveManager" />
    /// </summary>
    public interface IReviewObjectiveManager : IContainedEntityManager<ReviewObjective>
    {
        /// <summary>
        ///     Creates an <see cref="ReviewObjective" /> based on a template
        /// </summary>
        /// <param name="template">The <see cref="ReviewObjective" /> template</param>
        /// <param name="container">The <see cref="Review" /> container</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityOperationResult{TEntity}" /></returns>
        Task<EntityOperationResult<ReviewObjective>> CreateEntityBasedOnTemplate(ReviewObjective template, Review container);

        /// <summary>
        ///     Creates an <see cref="ReviewObjective" /> based on a template
        /// </summary>
        /// <param name="templates">The <see cref="ReviewObjective" /> templates</param>
        /// <param name="container">The <see cref="Review" /> container</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityOperationResult{TEntity}" /></returns>
        Task<EntityOperationResult<ReviewObjective>> CreateEntityBasedOnTemplates(IEnumerable<ReviewObjective> templates, Review container);

        /// <summary>
        ///     Get a collection of existing <see cref="ReviewObjectiveCreationDto" /> of a <see cref="Review" />
        /// </summary>
        /// <param name="reviewId">The id of the <see cref="Review" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="ReviewObjective" /></returns>
        IEnumerable<ReviewObjectiveCreationDto> GetReviewObjectiveCreationForReview(Guid reviewId);

        /// <summary>
        ///     Gets the number of open <see cref="ReviewTask" /> where the logged user is assigned to
        ///     and <see cref="Comment" /> for each <see cref="Review" />
        /// </summary>
        /// <param name="reviewObjectivesId">A collection of <see cref="Guid" /> for <see cref="ReviewObjective" />s</param>
        /// <param name="projectId">A <see cref="Guid" /> of <see cref="Project" />s</param>
        /// <param name="userName">The name of the current logged user</param>
        /// <returns>A <see cref="Task" /> with the <see cref="Dictionary{Guid,ComputedProjectProperties}" /></returns>
        Task<Dictionary<Guid, AdditionalComputedProperties>> GetOpenTasksAndComments(IEnumerable<Guid> reviewObjectivesId, Guid projectId, string userName);

        /// <summary>
        ///     Update the status of the <see cref="ReviewObjective" /> if necessary
        /// </summary>
        /// <param name="reviewObjectiveId">The <see cref="Guid" /> of the <see cref="ReviewObjective" /></param>
        Task UpdateStatus(Guid reviewObjectiveId);
    }
}
