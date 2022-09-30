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

namespace UI_DSM.Client.ViewModels.Pages.NormalUser.ReviewObjectivePage
{
    using ReactiveUI;

    using UI_DSM.Client.Components.NormalUser.ReviewObjective;
    using UI_DSM.Client.Services.ReviewObjectiveService;

    using UI_DSM.Client.ViewModels.Components;
    using UI_DSM.Client.ViewModels.Components.NormalUser.ReviewObjective;

    using UI_DSM.Shared.Models;

    /// <summary>
    ///     View model for the <see cref="ReviewObjectivePageViewModel" /> page
    /// </summary>
    public class ReviewObjectivePageViewModel : ReactiveObject, IReviewObjectivePageViewModel
    {
        /// <summary>
        ///     The <see cref="IReviewObjectiveService" />
        /// </summary>
        private readonly IReviewObjectiveService reviewObjectiveService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReviewObjectivePageViewModel" /> class.
        /// </summary>
        /// <param name="reviewObjectiveService">The <see cref="IReviewObjectiveService" /></param>
        public ReviewObjectivePageViewModel(IReviewObjectiveTasksViewModel reviewObjectiveTasksViewModal,IReviewObjectiveService reviewObjectiveService)
        {
            this.ReviewObjectiveTasksViewModel = reviewObjectiveTasksViewModal;
            this.reviewObjectiveService = reviewObjectiveService;
        }

        /// <summary>
        ///     The <see cref="IReviewObjectiveTasksViewModel" /> for the <see cref="ReviewObjectiveTasks" /> component
        /// </summary>
        public IReviewObjectiveTasksViewModel ReviewObjectiveTasksViewModel { get; }

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
        /// <param name="reviewObjectiveId">The <see cref="Guid" /> of the <see cref="ReviewObjective" /></param>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        public async Task OnInitializedAsync(Guid projectGuid, Guid reviewGuid, Guid reviewObjectiveId)
        {
            var reviewObjectiveResponse = await this.reviewObjectiveService.GetReviewObjectiveOfReview(projectGuid, reviewGuid, reviewObjectiveId, 1);
            this.ReviewObjectiveTasksViewModel.ReviewObjective = reviewObjectiveResponse;
        }
    }
}
