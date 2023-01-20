// --------------------------------------------------------------------------------------------------------
// <copyright file="IProjectPageViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Pages.Administration.ProjectPages
{
    using UI_DSM.Client.Components.Administration.BudgetManagement;
    using UI_DSM.Client.Components.Administration.ModelManagement;
    using UI_DSM.Client.Components.Administration.ParticipantManagement;
    using UI_DSM.Client.Components.Administration.ProjectManagement;
    using UI_DSM.Client.ViewModels.Components;
    using UI_DSM.Client.ViewModels.Components.Administration.BudgetManagement;
    using UI_DSM.Client.ViewModels.Components.Administration.ModelManagement;
    using UI_DSM.Client.ViewModels.Components.Administration.ParticipantManagement;
    using UI_DSM.Client.ViewModels.Components.Administration.ProjectManagement;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface definition for <see cref="ProjectPageViewModel" />
    /// </summary>
    public interface IProjectPageViewModel : IDisposable
    {
        /// <summary>
        ///     The <see cref="IProjectDetailsViewModel" /> for the <see cref="ProjectDetails" /> component
        /// </summary>
        IProjectDetailsViewModel ProjectDetailsViewModel { get; }

        /// <summary>
        ///     Value indicating the user is currently creating a new <see cref="Participant" />
        /// </summary>
        bool IsOnCreationMode { get; set; }

        /// <summary>
        ///     Value indicating whether the user is currently trying to establish a connection to COMET
        /// </summary>
        bool IsOnCometConnectionMode { get; set; }

        /// <summary>
        ///     Value indicating whether the user is authorized to access the page or not
        /// </summary>
        bool IsAuthorized { get; set; }

        /// <summary>
        ///     Gets the <see cref="IErrorMessageViewModel" />
        /// </summary>
        IErrorMessageViewModel ErrorMessageViewModel { get; }

        /// <summary>
        ///     Gets the <see cref="IParticipantCreationViewModel" />
        /// </summary>
        IParticipantCreationViewModel ParticipantCreationViewModel { get; }

        /// <summary>
        ///     The <see cref="ICometUploadViewModel" /> for the <see cref="CometUpload" /> component
        /// </summary>
        ICometUploadViewModel CometUploadViewModel { get; }

        /// <summary>
        ///     Value indicating that the user wants to upload a budget template
        /// </summary>
        bool IsOnBudgetUploadMode { get; set; }

        /// <summary>
        ///     The <see cref="IBudgetUploadViewModel" />
        /// </summary>
        IBudgetUploadViewModel BudgetUploadViewModel { get; set; }

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        ///     Override this method if you will perform an asynchronous operation and
        ///     want the component to refresh when that operation is completed.
        /// </summary>
        /// <param name="projectGuid">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        Task OnInitializedAsync(Guid projectGuid);

        /// <summary>
        ///     Opens the <see cref="ParticipantCreation" /> popup
        /// </summary>
        Task OpenCreateParticipantPopup();

        /// <summary>
        ///     Opens the <see cref="CometUpload" /> popup
        /// </summary>
        void OpenCometConnectionPopup();

        /// <summary>
        ///     Opens the <see cref="BudgetUpload" /> popup
        /// </summary>
        void OpenBudgetUploadPopup();
    }
}
