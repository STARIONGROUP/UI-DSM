// --------------------------------------------------------------------------------------------------------
// <copyright file="IParticipantService.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Services.Administration.ParticipantService
{
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.Types;

    /// <summary>
    ///     Interface definition for <see cref="ParticipantService" />
    /// </summary>
    public interface IParticipantService
    {
        /// <summary>
        ///     Gets all <see cref="Participant" />s contained inside a <see cref="Project" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> with the collection of <see cref="Participant" /></returns>
        Task<List<Participant>> GetParticipantsOfProject(Guid projectId, int deepLevel = 0);

        /// <summary>
        ///     Gets a <see cref="Participant" /> contained inside a <see cref="Project" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="participantId">The <see cref="Guid" /> of the <see cref="Participant" /></param>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> with the collection of <see cref="Participant" /></returns>
        Task<Participant> GetParticipantOfProject(Guid projectId, Guid participantId, int deepLevel = 0);

        /// <summary>
        ///     Creates a new <see cref="Participant" /> inside a <see cref="Project" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="participant">The <see cref="Participant" />to create</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityRequestResponse{Participant}" /></returns>
        Task<EntityRequestResponse<Participant>> CreateParticipant(Guid projectId, Participant participant);

        /// <summary>
        ///     Updates a <see cref="Participant" />
        /// </summary>
        /// <param name="participant">The <see cref="Participant" />to create</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityRequestResponse{Participant}" /></returns>
        Task<EntityRequestResponse<Participant>> UpdateParticipant(Participant participant);

        /// <summary>
        ///     Deletes a <see cref="Participant" />
        /// </summary>
        /// <param name="participant">The <see cref="Participant" /> to delete</param>
        /// <returns>A <see cref="Task" /> with the <see cref="RequestResponseDto" /></returns>
        Task<RequestResponseDto> DeleteParticipant(Participant participant);
    }
}
