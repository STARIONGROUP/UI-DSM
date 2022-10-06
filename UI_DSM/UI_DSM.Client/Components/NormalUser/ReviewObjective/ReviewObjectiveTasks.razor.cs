// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewObjectiveTasks.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.NormalUser.ReviewObjective
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.ViewModels.Components.NormalUser.ReviewObjective;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This components provide <see cref="ReviewObjective" /> tasks
    /// </summary>
    public partial class ReviewObjectiveTasks
    {
        /// <summary>
        ///     The <see cref="IReviewObjectiveTasksViewModel" /> for the component
        /// </summary>
        [Parameter]
        public IReviewObjectiveTasksViewModel ViewModel { get; set; }
    }
}
