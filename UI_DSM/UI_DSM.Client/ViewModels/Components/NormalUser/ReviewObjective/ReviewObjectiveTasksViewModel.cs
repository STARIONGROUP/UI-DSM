// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewObjectiveTasksViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Components.NormalUser.ReviewObjective
{
    using Microsoft.AspNetCore.Components;
    using ReactiveUI;

    using UI_DSM.Client.Components.NormalUser.ReviewObjective;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     View model for the <see cref="ReviewObjectiveTasks" /> component
    /// </summary>
    public class ReviewObjectiveTasksViewModel : ReactiveObject, IReviewObjectiveTasksViewModel
    {
        /// <summary>
        ///     Backing field for <see cref="ReviewObjective" />
        /// </summary>
        private ReviewObjective reviewObjective;

        /// <summary>
        ///     The <see cref="ReviewObjective" />
        /// </summary>
        public ReviewObjective ReviewObjective
        {
            get => this.reviewObjective;
            set => this.RaiseAndSetIfChanged(ref this.reviewObjective, value);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProjectReviewViewModel" /> class.
        /// </summary>
        /// <param name="navigationManager">The <see cref="NavigationManager" /></param>
        public ReviewObjectiveTasksViewModel(NavigationManager navigationManager)
        {
            this.NavigationManager = navigationManager;
        }

        /// <summary>
        ///     Gets or sets the <see cref="NavigationManager" />
        /// </summary>
        public NavigationManager NavigationManager { get; set; }
    }
}
