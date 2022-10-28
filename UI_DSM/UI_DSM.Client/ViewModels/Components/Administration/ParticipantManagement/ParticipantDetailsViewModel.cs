// --------------------------------------------------------------------------------------------------------
// <copyright file="ParticipantDetailsViewModel.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw, Nabil Abbar
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
    using UI_DSM.Client.Components.Administration.ParticipantManagement;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     View model for <see cref="ParticipantDetails" /> component
    /// </summary>
    public class ParticipantDetailsViewModel : IParticipantDetailsViewModel
    {
        /// <summary>
        ///     The <see cref="Participant" />
        /// </summary>
        public Participant Participant { get; set; }

        /// <summary>
        ///     A collection of available <see cref="Role" />
        /// </summary>
        public IEnumerable<Role> AvailableRoles { get; set; } = new List<Role>();

        /// <summary>
        ///     The <see cref="EventCallback" /> to call for data submit
        /// </summary>
        public EventCallback OnValidSubmit { get; set; }
    }
}
