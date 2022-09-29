// --------------------------------------------------------------------------------------------------------
// <copyright file="ProjectPage.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Pages.Administration.ProjectPages
{
    using Microsoft.AspNetCore.Components;

    using ReactiveUI;

    using UI_DSM.Client.ViewModels.Pages.Administration.ProjectPages;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This page provide management for a <see cref="Project" />
    /// </summary>
    public partial class ProjectPage : IDisposable
    {
        /// <summary>
        ///     The collection of <see cref="IDisposable" />
        /// </summary>
        private readonly List<IDisposable> disposables = new();

        /// <summary>
        ///     The <see cref="Guid" /> of the project
        /// </summary>
        [Parameter]
        public string ProjectId { get; set; }

        /// <summary>
        ///     The <see cref="IProjectPageViewModel" /> for this page
        /// </summary>
        [Inject]
        public IProjectPageViewModel ViewModel { get; set; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.disposables.ForEach(x => x.Dispose());
            this.disposables.Clear();
        }

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        ///     Override this method if you will perform an asynchronous operation and
        ///     want the component to refresh when that operation is completed.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        protected override async Task OnInitializedAsync()
        {
            this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.ProjectDetailsViewModel.Project)
                .Subscribe(_ => this.InvokeAsync(this.StateHasChanged)));

            this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.IsOnCreationMode)
                .Subscribe(_ => this.InvokeAsync(this.StateHasChanged)));

            this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.IsOnCometConnectionMode)
                .Subscribe(_ => this.InvokeAsync(this.StateHasChanged)));
                
            this.disposables.Add(this.ViewModel);

            await this.ViewModel.OnInitializedAsync(new Guid(this.ProjectId));
        }
    }
}
