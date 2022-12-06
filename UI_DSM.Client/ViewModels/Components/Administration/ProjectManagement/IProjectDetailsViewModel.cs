// --------------------------------------------------------------------------------------------------------
// <copyright file="IProjectDetailsViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Components.Administration.ProjectManagement
{
    using UI_DSM.Client.ViewModels.Components.Administration.ParticipantManagement;
    using UI_DSM.Client.Components.Administration.ParticipantManagement;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface definition for <see cref="ProjectDetailsViewModel" />
    /// </summary>
    public interface IProjectDetailsViewModel
    {
        /// <summary>
        ///     The <see cref="Project" />
        /// </summary>
        Project Project { get; set; }

        /// <summary>
        ///     The <see cref="IParticipantDetailsViewModel" />
        /// </summary>
        IParticipantDetailsViewModel ParticipantDetailsViewModel { get; }

        /// <summary>
        ///     Opens a popup to update role for the given <see cref="Participant" />
        /// </summary>
        /// <param name="participant">The <see cref="Participant" /></param>
        Task OpenUpdatePopup(Participant participant);

        /// <summary>
        ///     Value indicating if the <see cref="ParticipantDetails" /> popup should be visible
        /// </summary>
        bool IsOnUpdateViewMode { get; set; }

        /// <summary>
        ///     The <see cref="IConfirmCancelPopupViewModel" />
        /// </summary>
        IConfirmCancelPopupViewModel ConfirmCancelPopup { get; }

        /// <summary>
        ///     The <see cref="ErrorMessageViewModel" />
        /// </summary>
        IErrorMessageViewModel ErrorMessageViewModel { get; }

        /// <summary>
        ///     Opens a popup to confirm to delete a participant
        /// </summary>
        /// <param name="participant">The <see cref="Participant" /> to remove</param>
        void AskConfirmDeleteUser(Participant participant);
    }
}
