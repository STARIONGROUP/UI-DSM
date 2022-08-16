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
    using UI_DSM.Server.Context;
    using UI_DSM.Server.Types;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface definition for <see cref="ProjectManager" />
    /// </summary>
    public interface IProjectManager
    {
        /// <summary>
        ///     Gets all projects contained insed the <see cref="DatabaseContext" />
        /// </summary>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Project" /> as result</returns>
        Task<IEnumerable<Project>> GetProjects();

        /// <summary>
        ///     Creates a new <see cref="Project" /> and store it into the <see cref="DatabaseContext" />
        /// </summary>
        /// <param name="project">The <see cref="Project" /> to create</param>
        /// <returns>A <see cref="Task" /> with the result of the creation</returns>
        Task<EntityOperationResult<Project>> CreateProject(Project project);

        /// <summary>
        ///     Updates a <see cref="Project" />
        /// </summary>
        /// <param name="project">The <see cref="Project" /> to update</param>
        /// <returns>A <see cref="Task" /> with the result of the update</returns>
        Task<EntityOperationResult<Project>> UpdateProject(Project project);

        /// <summary>
        ///     Delete a <see cref="Project" />
        /// </summary>
        /// <param name="project">The <see cref="Project" /> to delete</param>
        /// <returns>A <see cref="Task" /> with the result of the deletion</returns>
        Task<EntityOperationResult<Project>> DeleteProject(Project project);

        /// <summary>
        ///     Tries to get a <see cref="Project" /> based on its <see cref="Project.Id" />
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="Project" /> if existing</returns>
        Task<Project> GetProject(Guid id);
    }
}
