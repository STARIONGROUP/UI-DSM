// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewPage.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Pages.NormalUser.ReviewPage
{
    using Microsoft.AspNetCore.Components;

    using ReactiveUI;

    using UI_DSM.Client.ViewModels.Pages.NormalUser.ReviewPage;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This page provide information for a <see cref="Review" />
    /// </summary>
    public partial class ReviewPage : IDisposable
    {
        /// <summary>
        /// Gets or sets if the <see cref="Components.App.LoadingComponent.LoadingComponent"/> is loading
        /// </summary>
        [Parameter]
        public bool IsLoading { get; set; }

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
        ///     The <see cref="Guid" /> of the review
        /// </summary>
        [Parameter]
        public string ReviewId { get; set; }

        /// <summary>
        ///     The <see cref="IReviewPageViewModel" /> for this page
        /// </summary>
        [Inject]
        public IReviewPageViewModel ViewModel { get; set; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.disposables.ForEach(x => x.Dispose());
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
            this.IsLoading = true;
            this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.ReviewObjectiveViewModel.Review)
                .Subscribe(_ => this.InvokeAsync(this.StateHasChanged)));

            await this.ViewModel.OnInitializedAsync(new Guid(this.ProjectId), new Guid(this.ReviewId));
            this.IsLoading = false;
        }
    }
}
