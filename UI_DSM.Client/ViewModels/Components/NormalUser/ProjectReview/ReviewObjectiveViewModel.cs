// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewObjectiveViewModel.cs" company="RHEA System S.A.">
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

    using UI_DSM.Client.Enumerator;
    using UI_DSM.Client.Services.ReviewObjectiveService;
    using UI_DSM.Client.Services.ReviewService;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     View model for the <see cref="ReviewObjective" /> component
    /// </summary>
    public class ReviewObjectiveViewModel : ReactiveObject, IReviewObjectiveViewModel
    {
        /// <summary>
        ///     The <see cref="IReviewService" />
        /// </summary>
        private readonly IReviewObjectiveService reviewObjectiveService;

        /// <summary>
        ///     Backing field for <see cref="IsOnCreationMode" />
        /// </summary>
        private bool isOnCreationMode;

        /// <summary>
        ///     Backing field for <see cref="Project" />
        /// </summary>
        private Project project;

        /// <summary>
        ///     Backing field for <see cref="Review" />
        /// </summary>
        private Review review;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProjectReviewViewModel" /> class.
        /// </summary>
        /// <param name="reviewObjectiveService">The <see cref="IReviewObjectiveService" /></param>
        /// <param name="navigationManager">The <see cref="NavigationManager" /></param>
        public ReviewObjectiveViewModel(IReviewObjectiveService reviewObjectiveService, NavigationManager navigationManager)
        {
            this.NavigationManager = navigationManager;
            this.reviewObjectiveService = reviewObjectiveService;

            this.ReviewObjectiveCreationViewModel = new ReviewObjectiveCreationViewModel(reviewObjectiveService)
            {
                OnValidSubmit = new EventCallbackFactory().Create(this, this.CreateReviewObjective)
            };
        }

        /// <summary>
        ///     The <see cref="Review" />
        /// </summary>
        public Review Review
        {
            get => this.review;
            set => this.RaiseAndSetIfChanged(ref this.review, value);
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
        ///     A collection of comments and tasks of a <see cref="ReviewObjective" /> for the user
        /// </summary>
        public Dictionary<Guid, AdditionalComputedProperties> CommentsAndTasks { get; set; } = new();

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
        public IReviewObjectiveCreationViewModel ReviewObjectiveCreationViewModel { get; private set; }

        /// <summary>
        ///     Gets or sets the <see cref="NavigationManager" />
        /// </summary>
        public NavigationManager NavigationManager { get; set; }

        /// <summary>
        ///     The current <see cref="Participant" />
        /// </summary>
        public Participant Participant { get; set; }

        /// <summary>
        ///     A collection of <see cref="Annotation" />
        /// </summary>
        public List<Annotation> Annotations { get; set; }

        /// <summary>
        ///     Gets the navLink link to navigate to the <see cref="ReviewObjective"/> page
        /// </summary>
        /// <param name="reviewObjective">The <see cref="ReviewObjective" /></param>
        /// <returns>The navigation link</returns>
        public string GetNavLink(ReviewObjective reviewObjective)
        {
            return $"{this.NavigationManager.Uri}/ReviewObjective/{reviewObjective.Id}";
        }

        /// <summary>
        ///     Opens the <see cref="ReviewObjectiveCreation" /> as a popup
        /// </summary>
        public void OpenCreatePopup()
        {
            this.ReviewObjectiveCreationViewModel.ProjectId = this.Project.Id;
            this.ReviewObjectiveCreationViewModel.ReviewId = this.Review.Id;
            this.ReviewObjectiveCreationViewModel.ReviewObjective = new ReviewObjective();
            this.ReviewObjectiveCreationViewModel.SelectedReviewObjectives = new List<ReviewObjectiveCreationDto>();
            this.ReviewObjectiveCreationViewModel.SelectedReviewObjectivesPrr = new List<ReviewObjectiveCreationDto>();
            this.ReviewObjectiveCreationViewModel.SelectedReviewObjectivesSrr = new List<ReviewObjectiveCreationDto>();
            this.ErrorMessageViewModel.Errors.Clear();
            this.IsOnCreationMode = true;
        }

        /// <summary>
        ///     Tries to create a new <see cref="ReviewObjective" />
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private async Task CreateReviewObjective()
        {
            try
            {
                this.ReviewObjectiveCreationViewModel.ReviewObjectivesCreationStatus = CreationStatus.Creating;
                var selectedReviewObjectives = this.ReviewObjectiveCreationViewModel.SelectedReviewObjectivesPrr.Union(this.ReviewObjectiveCreationViewModel.SelectedReviewObjectivesSrr.ToList()).ToList();
                var creationResult = await this.reviewObjectiveService.CreateReviewObjectives(this.Project.Id, this.Review.Id, selectedReviewObjectives);
                this.ErrorMessageViewModel.Errors.Clear();

                if (creationResult.Errors.Any())
                {
                    this.ErrorMessageViewModel.Errors.AddRange(creationResult.Errors);
                }

                if (creationResult.IsRequestSuccessful)
                {
                    this.Review.ReviewObjectives.Add(creationResult.Entities);
                    this.ReviewObjectiveCreationViewModel.ReviewObjectivesCreationStatus = CreationStatus.Done;
                    foreach(var reviewObjective in creationResult.Entities)
                    {
                        this.CommentsAndTasks[reviewObjective.Id] = new AdditionalComputedProperties();
                    }
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
