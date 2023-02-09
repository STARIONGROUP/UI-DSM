// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewObjectiveComponent.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.NormalUser.ProjectReview
{
    using DynamicData;

    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.WebUtilities;

    using ReactiveUI;

    using UI_DSM.Client.Components.App.Comments;
    using UI_DSM.Client.Pages.NormalUser.ReviewTaskPage;
    using UI_DSM.Client.ViewModels.Components.NormalUser.ProjectReview;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This components provide <see cref="Review" /> objectives
    /// </summary>
    public partial class ReviewObjectiveComponent : IDisposable
    {
        /// <summary>
        ///     A collection of <see cref="IDisposable" />
        /// </summary>
        private readonly List<IDisposable> disposables = new();

        /// <summary>
        ///     Value asserting that this view has been completely initiliazed
        /// </summary>
        private bool isFullyInitialized;

        /// <summary>
        ///     The <see cref="IReviewObjectiveViewModel" /> for the component
        /// </summary>
        [Parameter]
        public IReviewObjectiveViewModel ViewModel { get; set; }

        /// <summary>
        ///     The <see cref="NavigationManager" />
        /// </summary>
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        /// <summary>
        ///     A reference to <see cref="Comments" /> component
        /// </summary>
        public Comments Comments { get; set; }

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
        /// </summary>
        protected override void OnInitialized()
        {
            this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.IsOnCreationMode)
                .Subscribe(_ => this.InvokeAsync(this.StateHasChanged)));
        }

        /// <summary>
        ///     Method invoked after each time the component has been rendered.
        /// </summary>
        /// <param name="firstRender">
        ///     Set to <c>true</c> if this is the first time
        ///     <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRender(System.Boolean)" /> has been invoked
        ///     on this component instance; otherwise <c>false</c>.
        /// </param>
        /// <remarks>
        ///     The <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRender(System.Boolean)" /> and
        ///     <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRenderAsync(System.Boolean)" /> lifecycle methods
        ///     are useful for performing interop, or interacting with values received from <c>@ref</c>.
        ///     Use the <paramref name="firstRender" /> parameter to ensure that initialization work is only performed
        ///     once.
        /// </remarks>
        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (this.Comments != null && !this.isFullyInitialized)
            {
                this.isFullyInitialized = true;
                this.Comments.ViewModel.OnNavigateCallback = new EventCallbackFactory().Create<Comment>(this, this.NavigateTo);
                this.Comments.ViewModel.Comments.AddRange(this.ViewModel.Annotations.OfType<Comment>().OrderBy(x => x.CreatedOn));
            }
        }

        /// <summary>
        ///     Navigates to the correct <see cref="ReviewTaskPage" />
        /// </summary>
        /// <param name="comment"></param>
        private void NavigateTo(Comment comment)
        {
            var url = Path.Combine(this.NavigationManager.Uri, "ReviewObjective", comment.CreatedInside.EntityContainer.Id.ToString(),
                "ReviewTask", comment.CreatedInside.Id.ToString());

            url = QueryHelpers.AddQueryString(url, "commentId", comment.Id.ToString());
            this.NavigationManager.NavigateTo(url);
        }
    }
}
