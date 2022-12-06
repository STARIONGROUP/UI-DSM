// --------------------------------------------------------------------------------------------------------
// <copyright file="IProjectService.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Services.Administration.ProjectService
{
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.Types;

    /// <summary>
    ///     Interface definition for <see cref="ProjectService" />
    /// </summary>
    public interface IProjectService
    {
        /// <summary>
        ///     Provide a collection of <see cref="Project" />
        /// </summary>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> where the result is a collection of <see cref="Project" /></returns>
        Task<List<Project>> GetProjects(int deepLevel = 0);

        /// <summary>
        ///     Creates a new <see cref="Project" />
        /// </summary>
        /// <param name="newProject">The <see cref="Project" /> to create</param>
        /// <returns>A <see cref="Task" /> with the <see cref="Project" /> if created correctly</returns>
        Task<EntityRequestResponse<Project>> CreateProject(Project newProject);

        /// <summary>
        ///     Delete a <see cref="Project" />
        /// </summary>
        /// <param name="projectToDelete">The project to delete</param>
        /// <returns>A <see cref="Task" /> with the <see cref="RequestResponseDto" /></returns>
        Task<RequestResponseDto> DeleteProject(Project projectToDelete);

        /// <summary>
        ///     Gets a <see cref="Project" /> based on its <see cref="Guid" />
        /// </summary>
        /// <param name="projectGuid">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> with the <see cref="Project" /> if found</returns>
        Task<Project> GetProject(Guid projectGuid, int deepLevel = 0);

        /// <summary>
        ///     Gets a collection of all <see cref="Project" /> where the current user is participating
        /// </summary>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Project" /> as result</returns>
        Task<List<Project>> GetUserParticipation();

        /// <summary>
        ///     Gets, for all <see cref="Project" />, the number of open <see cref="ReviewTask" /> and <see cref="Comment" />
        ///     related to the <see cref="Project" />
        /// </summary>
        /// <returns>A <see cref="Task" /> with a <see cref="Dictionary{Guid, ComputedProjectProperties}" /></returns>
        Task<Dictionary<Guid, ComputedProjectProperties>> GetOpenTasksAndComments();
    }
}
