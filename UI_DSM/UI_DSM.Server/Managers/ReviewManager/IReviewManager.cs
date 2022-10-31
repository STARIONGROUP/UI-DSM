// --------------------------------------------------------------------------------------------------------
// <copyright file="IReviewManager.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Managers.ReviewManager
{
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface definition for <see cref="ReviewManager" />
    /// </summary>
    public interface IReviewManager : IContainedEntityManager<Review>
    {

        /// <summary>
        ///     Gets the number of open <see cref="ReviewTask" /> where the logged user is assigned to
        ///     and <see cref="Comment" /> for each <see cref="Review" />
        /// </summary>
        /// <param name="reviewsId">A collection of <see cref="Guid" /> for <see cref="Review" />s</param>
        /// <param name="userName">The name of the current logged user</param>
        /// <returns>A <see cref="Task" /> with the <see cref="Dictionary{Guid,ComputedProjectProperties}" /></returns>
        Task<Dictionary<Guid, ComputedProjectProperties>> GetOpenTasksAndComments(IEnumerable<Guid> reviewsId, string userName);
    }
}
