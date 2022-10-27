// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewTaskPage.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Pages.NormalUser.ReviewTaskPage
{
    using Microsoft.AspNetCore.Components;

    using ReactiveUI;

    using UI_DSM.Client.Components.App.Comments;
    using UI_DSM.Client.Components.App.SelectedItemCard;
    using UI_DSM.Client.Components.NormalUser.Views;
    using UI_DSM.Client.ViewModels.Pages.NormalUser.ReviewTaskPage;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Page to proceed to a <see cref="ReviewTask" />
    /// </summary>
    public partial class ReviewTaskPage : IDisposable
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
        ///     The <see cref="Guid" /> of the review
        /// </summary>
        [Parameter]
        public string ReviewId { get; set; }

        /// <summary>
        ///     The <see cref="Guid" /> of the <see cref="ReviewObjective" />
        /// </summary>
        [Parameter]
        public string ReviewObjectiveId { get; set; }

        /// <summary>
        ///     The <see cref="Guid" /> of the <see cref="ReviewTask" />
        /// </summary>
        [Parameter]
        public string ReviewTaskId { get; set; }

        /// <summary>
        ///     The <see cref="IReviewTaskPageViewModel" />
        /// </summary>
        [Inject]
        public IReviewTaskPageViewModel ViewModel { get; set; }

        /// <summary>
        ///     The <see cref="DynamicComponent" />
        /// </summary>
        public DynamicComponent BaseView { get; set; }

        /// <summary>
        ///     The currently selected item in the BaseView
        /// </summary>
        public object SelectedItem { get; set; }

        /// <summary>
        ///     The <see cref="SelectedItemCard" />
        /// </summary>
        public SelectedItemCard SelectedItemCard { get; set; }

        /// <summary>
        ///     The <see cref="Comments" />
        /// </summary>
        public Comments Comments { get; set; }

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
            this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.ReviewTask)
                .Subscribe(async _ => await this.OnSelectedItemChanged(this.ViewModel.ReviewTask)));

            this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.CurrentBaseView)
                .Subscribe(async _ => await this.OnCurrentBaseViewChanged()));

            await this.ViewModel.OnInitializedAsync(new Guid(this.ProjectId), new Guid(this.ReviewId),
                new Guid(this.ReviewObjectiveId), new Guid(this.ReviewTaskId));
        }

        /// <summary>
        ///     Initialize the viewModel of the <see cref="BaseView" /> when it changes
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private async Task OnCurrentBaseViewChanged()
        {
            await this.InvokeAsync(this.StateHasChanged);

            if (this.BaseView?.Instance is BaseView baseView)
            {
                var projectId = new Guid(this.ProjectId);
                var reviewId = new Guid(this.ReviewId);
                await this.Comments.ViewModel.InitializesProperties(projectId, reviewId, this.ViewModel.CurrentView);
                await baseView.InitializeViewModel(this.ViewModel.Things, projectId, reviewId);
                this.disposables.Add(baseView.SelectedItemObservable.Subscribe(async x => await this.OnSelectedItemChanged(x)));
                this.disposables.Add(this.Comments.ViewModel.Comments.CountChanged.Subscribe(async _ => await baseView.HasChanged()));
            }
        }

        /// <summary>
        ///     Handle the changed of the baseView Selected Item
        /// </summary>
        /// <param name="newSelectedItem">The new selected Item</param>
        /// <returns>A <see cref="Task" /></returns>
        private async Task OnSelectedItemChanged(object newSelectedItem)
        {
            if (this.SelectedItemCard == null)
            {
                return;
            }

            newSelectedItem ??= this.ViewModel?.ReviewTask;

            this.SelectedItem = newSelectedItem;
            this.SelectedItemCard.ViewModel.SelectedItem = this.SelectedItem;
            this.Comments.ViewModel.SelectedItem = this.SelectedItem;
            await this.InvokeAsync(this.StateHasChanged);
        }
    }
}
