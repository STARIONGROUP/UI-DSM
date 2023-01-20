// --------------------------------------------------------------------------------------------------------
// <copyright file="ITopMenuViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Shared.TopMenu
{
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface definition for <see cref="TopMenuViewModel" />
    /// </summary>
    public interface ITopMenuViewModel
    {
        /// <summary>
        ///     The current logged user
        /// </summary>
        string UserName { get; }

        /// <summary>
        ///     Initializes this view model
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        Task InitializesViewModel();

        /// <summary>
        ///     Verify that the current user has access to the project management page
        /// </summary>
        /// <returns>The assert</returns>
        bool HasAccessToProjectManagement();

        /// <summary>
        ///     Gets the <see cref="Participant" /> for a <see cref="Project" />
        /// </summary>
        /// <param name="projectId">The <see cref="Project" /> id</param>
        /// <returns>The <see cref="Participant"/></returns>
        Participant GetParticipantForProject(Guid projectId);
    }
}
