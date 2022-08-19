// --------------------------------------------------------------------------------------------------------
// <copyright file="IRoleManagementViewModel.cs" company="RHEA System S.A.">
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
    using DevExpress.Blazor;

    using DynamicData;

    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.Components.Administration.RoleManagement;
    using UI_DSM.Client.ViewModels.Components;
    using UI_DSM.Client.ViewModels.Components.Administration.RoleManagement;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface definition for <see cref="RoleManagementViewModel" />
    /// </summary>
    public interface IRoleManagementViewModel
    {
        /// <summary>
        ///     A collection of <see cref="Role" />
        /// </summary>
        SourceList<Role> Roles { get; }

        /// <summary>
        ///     Value indicating if the <see cref="DxPopup" /> for the <see cref="RoleCreation" /> component is visible
        /// </summary>
        bool IsCreationPopupVisible { get; set; }

        /// <summary>
        ///     Gets the <see cref="IRoleCreationViewModel" />
        /// </summary>
        IRoleCreationViewModel RoleCreationViewModel { get; }

        /// <summary>
        ///     The <see cref="IErrorMessageViewModel" />
        /// </summary>
        IErrorMessageViewModel ErrorMessageViewModel { get; }

        /// <summary>
        ///     Gets or sets the <see cref="NavigationManager" />
        /// </summary>
        NavigationManager NavigationManager { get; set; }

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        ///     Override this method if you will perform an asynchronous operation and
        ///     want the component to refresh when that operation is completed.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        Task OnInitializedAsync();

        /// <summary>
        ///     Opens the <see cref="RoleCreation" /> component as a <see cref="DxPopup" />
        /// </summary>
        void OpenCreationPopup();

        /// <summary>
        ///     Navigate to the page dedicated to the given <see cref="Role" />
        /// </summary>
        /// <param name="role">The <see cref="Role" /></param>
        void GoToRolePage(Role role);
    }
}
