// --------------------------------------------------------------------------------------------------------
// <copyright file="ProjectPageViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Pages.NormalUser.ReviewPage
{
    using ReactiveUI;

    using UI_DSM.Client.Services.ReviewService;
    using UI_DSM.Client.ViewModels.Components;
    using UI_DSM.Client.ViewModels.Components.NormalUser.ProjectReview;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     View model for the <see cref="ReviewPageViewModel" /> page
    /// </summary>
    public class ReviewPageViewModel : ReactiveObject, IReviewPageViewModel
    {

        /// <summary>
        ///     The <see cref="IReviewService" />
        /// </summary>
        private readonly IReviewService reviewService;


        /// <summary>
        ///     Initializes a new instance of the <see cref="ReviewPageViewModel" /> class.
        /// </summary>
        /// <param name="reviewService">The <see cref="IReviewService" /></param>
        public ReviewPageViewModel(IReviewService reviewService, IReviewObjectiveViewModel reviewObjectiveViewModel)
        {
            this.reviewService = reviewService;
            this.ReviewObjectiveViewModel = reviewObjectiveViewModel;
        }

        /// <summary>
        ///     The <see cref="IReviewObjectiveViewModel" /> for the <see cref="ReviewObjective" /> component
        /// </summary>
        public IReviewObjectiveViewModel ReviewObjectiveViewModel { get; }

        /// <summary>
        ///     Gets the <see cref="IErrorMessageViewModel" />
        /// </summary>
        public IErrorMessageViewModel ErrorMessageViewModel { get; } = new ErrorMessageViewModel();


        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        ///     Override this method if you will perform an asynchronous operation and
        ///     want the component to refresh when that operation is completed.
        /// </summary>
        /// <param name="projectGuid">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <param name="reviewGuid">The <see cref="Guid" /> of the <see cref="Review" /></param>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        public async Task OnInitializedAsync(Guid projectGuid, Guid reviewGuid)
        {
            var reviewResponse = await this.reviewService.GetReviewOfProject(projectGuid, reviewGuid, 1);
            this.ReviewObjectiveViewModel.Review = reviewResponse;
        }
    }
}
