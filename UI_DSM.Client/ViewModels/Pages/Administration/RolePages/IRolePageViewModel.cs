// --------------------------------------------------------------------------------------------------------
// <copyright file="IRolePageViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Pages.Administration.RolePages
{
    using UI_DSM.Client.Components.Administration.RoleManagement;
    using UI_DSM.Client.ViewModels.Components;
    using UI_DSM.Client.ViewModels.Components.Administration.RoleManagement;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface definition for <see cref="RolePageViewModel" />
    /// </summary>
    public interface IRolePageViewModel
    {
        /// <summary>
        ///     The <see cref="IRoleDetailsViewModel" /> for the <see cref="RoleDetails" /> component
        /// </summary>
        IRoleDetailsViewModel RoleDetailsViewModel { get; }

        /// <summary>
        ///     Value indicating if <see cref="Role" /> values are editable or not
        /// </summary>
        bool ModificationEnabled { get; set; }

        /// <summary>
        ///     The <see cref="IErrorMessageViewModel" />
        /// </summary>
        IErrorMessageViewModel ErrorMessageViewModel { get; }

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        ///     Override this method if you will perform an asynchronous operation and
        ///     want the component to refresh when that operation is completed.
        /// </summary>
        /// <param name="roleGuid">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        Task OnInitializedAsync(Guid roleGuid);

        /// <summary>
        ///     Update the current <see cref="Role" />
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        Task UpdateRole();
    }
}
