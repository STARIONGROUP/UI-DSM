// --------------------------------------------------------------------------------------------------------
// <copyright file="TopMenu.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Shared.TopMenu
{
    using Microsoft.AspNetCore.Components;

    using ReactiveUI;

    using UI_DSM.Client.ViewModels.Shared.TopMenu;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     The TopMenu component
    /// </summary>
    public partial class TopMenu : IDisposable
    {
        /// <summary>
        ///     A collection of <see cref="IDisposable" />
        /// </summary>
        private List<IDisposable> disposables = new();

        /// <summary>
        ///     The <see cref="ITopMenuViewModel" />
        /// </summary>
        [Inject]
        public ITopMenuViewModel ViewModel { get; set; }

        /// <summary>
        ///     The <see cref="NavigationManager" />
        /// </summary>
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        /// <summary>
        ///     Bool to keep track of the open state of the user dropdown menu
        /// </summary>
        private bool IsUserMenuOpen { get; set; } = false;

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.disposables.ForEach(x => x.Dispose());
        }

        /// <summary>
        ///     Opens the user menu
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        public async Task OpenUserMenu()
        {
            if (!this.IsUserMenuOpen)
            {
                await this.ViewModel.InitializesViewModel();
            }

            this.IsUserMenuOpen = !this.IsUserMenuOpen;
        }

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        ///     Override this method if you will perform an asynchronous operation and
        ///     want the component to refresh when that operation is completed.
        /// </summary>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> representing any asynchronous operation.</returns>
        protected override Task OnInitializedAsync()
        {
            this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.UserName)
                .Subscribe(_ => this.InvokeAsync(this.StateHasChanged)));

            return this.ViewModel.InitializesViewModel();
        }

        /// <summary>
        ///     Gets the name of the current <see cref="Role" /> for the current <see cref="Project" />
        /// </summary>
        /// <returns>The name of the <see cref="Role" /></returns>
        public string GetRoleName()
        {
            var projectId = this.GetProjectId();
            return projectId != Guid.Empty ? this.ViewModel.GetRoleForProject(projectId) : string.Empty;
        }

        /// <summary>
        ///     Gets the <see cref="Guid" /> of the <see cref="Project" /> based on the current url
        /// </summary>
        /// <returns>The <see cref="Guid" /></returns>
        private Guid GetProjectId()
        {
            var url = new Uri(this.NavigationManager.Uri);
            var splittedPath = url.LocalPath.Split('/');

            if (splittedPath.Length < 3 || !Guid.TryParse(splittedPath[2], out var projectId))
            {
                return Guid.Empty;
            }

            return projectId;
        }
    }
}
