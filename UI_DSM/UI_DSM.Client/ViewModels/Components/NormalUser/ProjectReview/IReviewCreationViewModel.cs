// --------------------------------------------------------------------------------------------------------
// <copyright file="IReviewCreationViewModel.cs" company="RHEA System S.A.">
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

    using UI_DSM.Client.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface definition for <see cref="ReviewCreationViewModel" />
    /// </summary>
    public interface IReviewCreationViewModel
    {
        /// <summary>
        ///     The <see cref="Review" /> to create
        /// </summary>
        Review Review { get; set; }

        /// <summary>
        ///     The <see cref="EventCallback" /> to call for data submit
        /// </summary>
        EventCallback OnValidSubmit { get; set; }

        /// <summary>
        ///     A collection of <see cref="Model" /> that has been selected
        /// </summary>
        IEnumerable<Model> SelectedModels { get; set; }

        /// <summary>
        ///     The current <see cref="CreationStatus" />
        /// </summary>
        CreationStatus CreationStatus { get; set; }
    }
}
