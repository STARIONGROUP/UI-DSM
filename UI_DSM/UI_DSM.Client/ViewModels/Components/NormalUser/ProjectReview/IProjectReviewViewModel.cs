// --------------------------------------------------------------------------------------------------------
// <copyright file="IProjectReviewViewModel.cs" company="RHEA System S.A.">
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
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface definition for <see cref="ProjectReviewViewModel" />
    /// </summary>
    public interface IProjectReviewViewModel
    {
        /// <summary>
        ///     The <see cref="Project" />
        /// </summary>
        Project Project { get; set; }


        /// <summary>
        ///     Navigate to the page dedicated to the given <see cref="Review" />
        /// </summary>
        /// <param name="review">The <see cref="Review" /></param>
        void GoToReviewPage(Review review);

        /// <summary>
        ///     Gets or sets the <see cref="NavigationManager" />
        /// </summary>
        NavigationManager NavigationManager { get; set; }
    }
}
