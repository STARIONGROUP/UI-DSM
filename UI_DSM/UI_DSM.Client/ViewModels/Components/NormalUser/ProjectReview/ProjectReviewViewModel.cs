// --------------------------------------------------------------------------------------------------------
// <copyright file="ProjectReviewViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Components.NormalUser.ProjectReview
{
    using DynamicData;

    using Microsoft.AspNetCore.Components;

    using ReactiveUI;

    using UI_DSM.Client.Components.NormalUser.ProjectReview;
    using UI_DSM.Client.Services.ReviewService;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     View model for the <see cref="ProjectReview" /> component
    /// </summary>
    public class ProjectReviewViewModel : ReactiveObject, IProjectReviewViewModel
    {
        /// <summary>
        ///     The <see cref="IReviewService" />
        /// </summary>
        private readonly IReviewService reviewService;

        /// <summary>
        ///     Backing field for <see cref="IsOnCreationMode" />
        /// </summary>
        private bool isOnCreationMode;

        /// <summary>
        ///     Backing field for <see cref="Project" />
        /// </summary>
        private Project project;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProjectReviewViewModel" /> class.
        /// </summary>
        /// <param name="navigationManager">The <see cref="NavigationManager" /></param>
        /// <param name="reviewService">The <see cref="IReviewService" /></param>
        public ProjectReviewViewModel(NavigationManager navigationManager, IReviewService reviewService)
        {
            this.NavigationManager = navigationManager;
            this.reviewService = reviewService;

            this.ReviewCreationViewModel = new ReviewCreationViewModel
            {
                OnValidSubmit = new EventCallbackFactory().Create(this, this.CreateReview)
            };
        }

        /// <summary>
        ///     The <see cref="Project" />
        /// </summary>
        public Project Project
        {
            get => this.project;
            set => this.RaiseAndSetIfChanged(ref this.project, value);
        }

        /// <summary>
        ///     A collection of comments and tasks <see cref="Review" /> for the user
        /// </summary>
        public Dictionary<Guid, ComputedProjectProperties> CommentsAndTasks { get; set; } = new();

        /// <summary>
        ///     Gets or sets the <see cref="NavigationManager" />
        /// </summary>
        public NavigationManager NavigationManager { get; set; }

        /// <summary>
        ///     Value indicating the user is currently creating a new <see cref="Review" />
        /// </summary>
        public bool IsOnCreationMode
        {
            get => this.isOnCreationMode;
            set => this.RaiseAndSetIfChanged(ref this.isOnCreationMode, value);
        }

        /// <summary>
        ///     The <see cref="IErrorMessageViewModel" />
        /// </summary>
        public IErrorMessageViewModel ErrorMessageViewModel { get; } = new ErrorMessageViewModel();

        /// <summary>
        ///     The <see cref="IReviewCreationViewModel" />
        /// </summary>
        public IReviewCreationViewModel ReviewCreationViewModel { get; private set; }

        /// <summary>
        ///     Navigate to the page dedicated to the given <see cref="Review" />
        /// </summary>
        /// <param name="review">The <see cref="Review" /></param>
        public void GoToReviewPage(Review review)
        {
            this.NavigationManager.NavigateTo($"{this.NavigationManager.Uri}/Review/{review.Id}");
        }

        /// <summary>
        ///     Opens the <see cref="ReviewCreation" /> as a popup
        /// </summary>
        public void OpenCreatePopup()
        {
            this.ReviewCreationViewModel.Review = new Review();
            this.ReviewCreationViewModel.SelectedModels = new List<Model>();
            this.ErrorMessageViewModel.Errors.Clear();
            this.IsOnCreationMode = true;
        }

        /// <summary>
        ///     Tries to create a new <see cref="Review" />
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private async Task CreateReview()
        {
            try
            {
                this.ReviewCreationViewModel.Review.Artifacts.Add(this.ReviewCreationViewModel.SelectedModels);

                var creationResult = await this.reviewService.CreateReview(this.project.Id, this.ReviewCreationViewModel.Review);
                this.ErrorMessageViewModel.Errors.Clear();

                if (creationResult.Errors.Any())
                {
                    this.ErrorMessageViewModel.Errors.AddRange(creationResult.Errors);
                }

                if (creationResult.IsRequestSuccessful)
                {
                    this.Project.Reviews.Add(creationResult.Entity);
                }

                this.IsOnCreationMode = !creationResult.IsRequestSuccessful;
            }
            catch (Exception exception)
            {
                this.ErrorMessageViewModel.Errors.Clear();
                this.ErrorMessageViewModel.Errors.Add(exception.Message);
            }
        }
    }
}
