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

    using UI_DSM.Client.Components.NormalUser.ProjectReview;
    using UI_DSM.Shared.DTO.Common;
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
        ///     A collection of comments and tasks <see cref="Review" /> for the user
        /// </summary>
        Dictionary<Guid, AdditionalComputedProperties> CommentsAndTasks { get; set; }

        /// <summary>
        ///     Value indicating the user is currently creating a new <see cref="Review" />
        /// </summary>
        bool IsOnCreationMode { get; set; }

        /// <summary>
        ///     The <see cref="IErrorMessageViewModel" />
        /// </summary>
        IErrorMessageViewModel ErrorMessageViewModel { get; }

        /// <summary>
        ///     The <see cref="IReviewCreationViewModel" />
        /// </summary>
        IReviewCreationViewModel ReviewCreationViewModel { get; }

        /// <summary>
        ///     Gets or sets the <see cref="NavigationManager" />
        /// </summary>
        NavigationManager NavigationManager { get; set; }

        /// <summary>
        ///     The current <see cref="Participant" />
        /// </summary>
        Participant Participant { get; set; }
        
        /// <summary>
        ///     Navigate to the page dedicated to the given <see cref="Review" />
        /// </summary>
        /// <param name="review">The <see cref="Review" /></param>
        void GoToReviewPage(Review review);

        /// <summary>
        ///     Opens the <see cref="ReviewCreation" /> as a popup
        /// </summary>
        void OpenCreatePopup();
    }
}
