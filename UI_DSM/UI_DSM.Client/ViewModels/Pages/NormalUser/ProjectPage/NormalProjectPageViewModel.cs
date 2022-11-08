// --------------------------------------------------------------------------------------------------------
// <copyright file="NormalProjectPageViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Pages.NormalUser.ProjectPage
{
    using ReactiveUI;

    using UI_DSM.Client.Components.NormalUser.ProjectReview;
    using UI_DSM.Client.Services.Administration.ProjectService;
    using UI_DSM.Client.Services.ReviewService;
    using UI_DSM.Client.ViewModels.Components;
    using UI_DSM.Client.ViewModels.Components.NormalUser.ProjectReview;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     View model for the <see cref="ProjectPage" /> page
    /// </summary>
    public class NormalProjectPageViewModel : ReactiveObject, INormalProjectPageViewModel
    {
        /// <summary>
        ///     The <see cref="IProjectService" />
        /// </summary>
        private readonly IProjectService projectService;

        /// <summary>
        ///     The <see cref="IReviewService" />
        /// </summary>
        private readonly IReviewService reviewService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NormalProjectPageViewModel" /> class.
        /// </summary>
        /// <param name="projectService">The <see cref="IProjectService" /></param>
        /// <param name="reviewService">The <see cref="IReviewService" /></param>
        /// <param name="projectReviewViewModel">The <see cref="IProjectReviewViewModel" /></param>
        public NormalProjectPageViewModel(IProjectService projectService, IReviewService reviewService, IProjectReviewViewModel projectReviewViewModel)
        {
            this.projectService = projectService;
            this.reviewService = reviewService;
            this.ProjectReviewViewModel = projectReviewViewModel;
        }

        /// <summary>
        ///     The <see cref="IProjectReviewViewModel" /> for the <see cref="ProjectReview" /> component
        /// </summary>
        public IProjectReviewViewModel ProjectReviewViewModel { get; }

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
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        public async Task OnInitializedAsync(Guid projectGuid)
        {
            var projectResponse = await this.projectService.GetProject(projectGuid, 1);

            if (projectResponse != null)
            {
                this.ProjectReviewViewModel.Project = projectResponse;
                this.ProjectReviewViewModel.CommentsAndTasks = await this.reviewService.GetOpenTasksAndComments(projectGuid);
            }
        }
    }
}
