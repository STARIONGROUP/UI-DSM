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
    using Microsoft.AspNetCore.Components;
    using ReactiveUI;

    using UI_DSM.Client.Components.NormalUser.ProjectReview;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     View model for the <see cref="ProjectReview" /> component
    /// </summary>
    public class ProjectReviewViewModel : ReactiveObject, IProjectReviewViewModel
    {
        /// <summary>
        ///     Backing field for <see cref="Project" />
        /// </summary>
        private Project project;

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
        ///     Initializes a new instance of the <see cref="ProjectReviewViewModel" /> class.
        /// </summary>
        /// <param name="navigationManager">The <see cref="NavigationManager" /></param>
        public ProjectReviewViewModel(NavigationManager navigationManager)
        {
            this.NavigationManager = navigationManager;
        }

        /// <summary>
        ///     Gets or sets the <see cref="NavigationManager" />
        /// </summary>
        public NavigationManager NavigationManager { get; set; }

        /// <summary>
        ///     Navigate to the page dedicated to the given <see cref="Review" />
        /// </summary>
        /// <param name="review">The <see cref="Review" /></param>
        public void GoToReviewPage(Review review)
        {
            this.NavigationManager.NavigateTo($"{this.NavigationManager.Uri}/Review/{review.Id}");
        }
    }
}
