// --------------------------------------------------------------------------------------------------------
// <copyright file="IParticipantCreationViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Components.Administration.ParticipantManagement
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface definition for <see cref="ParticipantCreationViewModel" />
    /// </summary>
    public interface IParticipantCreationViewModel
    {
        /// <summary>
        ///     A collection of available <see cref="Role" />
        /// </summary>
        IEnumerable<Role> AvailableRoles { get; set; }

        /// <summary>
        ///     A collection of <see cref="AvailableUsers" />
        /// </summary>
        IEnumerable<UserEntity> AvailableUsers { get; set; }

        /// <summary>
        ///     An <see cref="EventCallback" /> to invoke on form submit
        /// </summary>
        EventCallback OnValidSubmit { get; set; }

        /// <summary>
        ///     The <see cref="Participant" /> to create
        /// </summary>
        Participant Participant { get; set; }

        /// <summary>
        ///     Updates this view model properties
        /// </summary>
        /// <param name="projectId">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <returns>A <see cref="Task" /></returns>
        Task UpdateProperties(Guid projectId);
    }
}
