// --------------------------------------------------------------------------------------------------------
// <copyright file="ProjectDetailsViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Components.NormalUser.ProjectReview
{
    using Microsoft.AspNetCore.Components;
    using ReactiveUI;

    using UI_DSM.Shared.Models;

    /// <summary>
    ///     View model for the <see cref="ReviewObjective" /> component
    /// </summary>
    public class ReviewObjectiveViewModel : ReactiveObject, IReviewObjectiveViewModel
    {
        /// <summary>
        ///     Backing field for <see cref="Review" />
        /// </summary>
        private Review review;

        /// <summary>
        ///     The <see cref="Review" />
        /// </summary>
        public Review Review
        {
            get => this.review;
            set => this.RaiseAndSetIfChanged(ref this.review, value);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProjectReviewViewModel" /> class.
        /// </summary>
        /// <param name="navigationManager">The <see cref="NavigationManager" /></param>
        public ReviewObjectiveViewModel(NavigationManager navigationManager)
        {
            this.NavigationManager = navigationManager;
        }

        /// <summary>
        ///     Gets or sets the <see cref="NavigationManager" />
        /// </summary>
        public NavigationManager NavigationManager { get; set; }

        /// <summary>
        ///     Navigate to the page dedicated to the given <see cref="ReviewObjective" />
        /// </summary>
        /// <param name="reviewObjective">The <see cref="ReviewObjective" /></param>
        public void GoToReviewObjectivePage(ReviewObjective reviewObjective)
        {
            this.NavigationManager.NavigateTo($"{this.NavigationManager.Uri}/ReviewObjective/{reviewObjective.Id}");
        }
    }
}
