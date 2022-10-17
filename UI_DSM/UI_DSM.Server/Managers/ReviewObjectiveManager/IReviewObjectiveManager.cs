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
        /// <param name="author">The <see cref="Participant" /> author</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityOperationResult{TEntity}" /></returns>
        Task<EntityOperationResult<ReviewObjective>> CreateEntityBasedOnTemplate(ReviewObjective template, Review container, Participant author);
    }
}
