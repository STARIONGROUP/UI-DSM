// --------------------------------------------------------------------------------------------------------
// <copyright file="IProjectManagementViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Pages.Administration
{
    using DynamicData;

    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.Components.Administration.ProjectManagement;
    using UI_DSM.Client.ViewModels.Components;
    using UI_DSM.Client.ViewModels.Components.Administration.ProjectManagement;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface definition for <see cref="ProjectManagementViewModel" />
    /// </summary>
    public interface IProjectManagementViewModel
    {
        /// <summary>
        ///     A collection of <see cref="Projects" />
        /// </summary>
        SourceList<Project> Projects { get; }

        /// <summary>
        ///     Value indicating the user is currently creating a new <see cref="Project" />
        /// </summary>
        bool IsOnCreationMode { get; set; }

        /// <summary>
        ///     The <see cref="IErrorMessageViewModel" />
        /// </summary>
        IErrorMessageViewModel ErrorMessageViewModel { get; }

        /// <summary>
        ///     The <see cref="IProjectCreationViewModel" />
        /// </summary>
        IProjectCreationViewModel ProjectCreationViewModel { get; }

        /// <summary>
        ///     Gets or sets the <see cref="NavigationManager" />
        /// </summary>
        NavigationManager NavigationManager { get; set; }

        /// <summary>
        ///     Indicates if the current user is allowed to manage project even if he is not the site admin
        /// </summary>
        bool IsAuthorized { get; set; }

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        ///     Override this method if you will perform an asynchronous operation and
        ///     want the component to refresh when that operation is completed.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        Task OnInitializedAsync();

        /// <summary>
        ///     Opens the <see cref="ProjectCreation" /> as a popup
        /// </summary>
        void OpenCreatePopup();

        /// <summary>
        ///     Navigate to the page dedicated to the given <see cref="Project" />
        /// </summary>
        /// <param name="project">The <see cref="Project" /></param>
        void GoToProjectPage(Project project);
    }
}
