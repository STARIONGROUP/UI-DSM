// --------------------------------------------------------------------------------------------------------
// <copyright file="IParticipantManager.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Managers.ParticipantManager
{
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface definition for <see cref="ParticipantManager" />
    /// </summary>
    public interface IParticipantManager : IEntityManager<Participant>
    {
        /// <summary>
        ///     Gets all <see cref="Participant" /> that are inside a <see cref="Project" /> and associated <see cref="Entity" />
        /// </summary>
        /// <param name="projectId">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <param name="deepLevel">The deep level</param>
        /// <returns>A <see cref="Task"/> with a collection of <see cref="Entity"/></returns>
        Task<IEnumerable<Entity>> GetParticipantsOfProject(Guid projectId, int deepLevel = 0);

        /// <summary>
        /// Gets all <see cref="Participant"/> where the <see cref="Participant.User"/> is the provided <see cref="UserEntity"/>
        /// </summary>
        /// <param name="userName">The name of the <see cref="UserEntity"/></param>
        /// <returns>A <see cref="Task"/> with a collection of <see cref="Participant"/></returns>
        Task<IEnumerable<Participant>> GetParticipants(string userName);

        /// <summary>
        /// Gets all <see cref="UserEntity"/> that are available for the creation of <see cref="Participant"/> inside a certain project
        /// </summary>
        /// <param name="projectId">The <see cref="projectId"/></param>
        /// <returns>A <see cref="Task"/> collection of <see cref="UserEntity"/> as result</returns>
        Task<IEnumerable<UserEntity>> GetAvailableUsersForParticipantCreation(Guid projectId);
    }
}
