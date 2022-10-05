// --------------------------------------------------------------------------------------------------------
// <copyright file="IProjectManager.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Server.Managers.ProjectManager
{
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface definition for <see cref="ProjectManager" />
    /// </summary>
    public interface IProjectManager : IEntityManager<Project>
    {
        /// <summary>
        ///     Get a collection of <see cref="Project" /> where a <see cref="UserEntity" /> is a <see cref="Participant" />
        /// </summary>
        /// <param name="userName">The name of the <see cref="UserEntity" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Project" /></returns>
        Task<IEnumerable<Project>> GetAvailableProjectsForUser(string userName);

        /// <summary>
        ///     Gets the number of open <see cref="ReviewTask" /> where the logged user is assigned to
        ///     and <see cref="Comment" /> for each <see cref="Project" />
        /// </summary>
        /// <param name="projectsId">A collection of <see cref="Guid" /> for <see cref="Project" />s</param>
        /// <param name="userName">The name of the current logged user</param>
        /// <returns>A <see cref="Task" /> with the <see cref="Dictionary{Guid,ComputedProjectProperties}" /></returns>
        Task<Dictionary<Guid, ComputedProjectProperties>> GetOpenTasksAndComments(IEnumerable<Guid> projectsId, string userName);
    }
}
