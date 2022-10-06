// --------------------------------------------------------------------------------------------------------
// <copyright file="IReviewObjectiveTasksViewModel.cs" company="RHEA System S.A.">
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
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface definition for <see cref="ReviewObjectiveTasksViewModel" />
    /// </summary>
    public interface IReviewObjectiveTasksViewModel
    {
        /// <summary>
        ///     The <see cref="ReviewObjective" />
        /// </summary>
        ReviewObjective ReviewObjective { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="NavigationManager" />
        /// </summary>
        NavigationManager NavigationManager { get; set; }
    }
}
