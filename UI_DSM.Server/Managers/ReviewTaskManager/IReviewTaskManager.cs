// --------------------------------------------------------------------------------------------------------
// <copyright file="IReviewTaskManager.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Managers.ReviewTaskManager
{
    using UI_DSM.Server.Managers.ReviewObjectiveManager;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface definition for <see cref="ReviewTaskManager" />
    /// </summary>
    public interface IReviewTaskManager : IContainedEntityManager<ReviewTask>
    {
        /// <summary>
        ///     Create <see cref="ReviewTask" />s based on templates
        /// </summary>
        /// <param name="container">The <see cref="ReviewObjective" /> container</param>
        /// <param name="templateReviewTasks">A collection of <see cref="ReviewTask" /></param>
        void CreateEntitiesBasedOnTemplate(ReviewObjective container, List<ReviewTask> templateReviewTasks);

        /// <summary>
        ///     Injects the <see cref="IReviewObjectiveManager" /> to avoid circular dependency
        /// </summary>
        /// <param name="manager">The <see cref="IReviewObjectiveManager" /></param>
        void InjectManager(IReviewObjectiveManager manager);

        /// <summary>
        ///     Gets the number of comments that are associated to <see cref="ReviewTask" />s
        /// </summary>
        /// <param name="reviewTasksId">A collection of <see cref="Guid" /> for <see cref="ReviewTask" /></param>
        /// <returns>A <see cref="Dictionary{Guid, AdditionalComputedProperties}" /></returns>
        Task<Dictionary<Guid, AdditionalComputedProperties>> GetCommentsCount(IEnumerable<Guid> reviewTasksId);
    }
}
