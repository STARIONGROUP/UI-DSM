// --------------------------------------------------------------------------------------------------------
// <copyright file="RolePage.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Pages.Administration.RolePages
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.ViewModels.Pages.Administration.RolePages;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This page provide management for a <see cref="Role" />
    /// </summary>
    public partial class RolePage
    {
        /// <summary>
        ///     The <see cref="Guid" /> of the project
        /// </summary>
        [Parameter]
        public string RoleId { get; set; }

        /// <summary>
        ///     The <see cref="IRolePageViewModel" /> for this page
        /// </summary>
        [Inject]
        public IRolePageViewModel ViewModel { get; set; }

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        ///     Override this method if you will perform an asynchronous operation and
        ///     want the component to refresh when that operation is completed.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        protected override async Task OnInitializedAsync()
        {
            await this.ViewModel.OnInitializedAsync(new Guid(this.RoleId));
        }
    }
}
