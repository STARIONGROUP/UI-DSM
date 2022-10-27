// --------------------------------------------------------------------------------------------------------
// <copyright file="IReviewObjectivePageViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Pages.NormalUser.ReviewObjectivePage
{
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface definition for <see cref="ReviewObjectivePageViewModel" />
    /// </summary>
    public interface IReviewObjectivePageViewModel
    {
        /// <summary>
        ///     The <see cref="ReviewObjective" /> of the page
        /// </summary>
        ReviewObjective ReviewObjective { get; }

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        ///     Override this method if you will perform an asynchronous operation and
        ///     want the component to refresh when that operation is completed.
        /// </summary>
        /// <param name="projectGuid">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <param name="reviewGuid">The <see cref="Guid" /> of the <see cref="Review" /></param>
        /// <param name="reviewObjectiveId">The <see cref="Guid" /> of the <see cref="ReviewObjective" /></param>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        Task OnInitializedAsync(Guid projectGuid, Guid reviewGuid, Guid reviewObjectiveId);
    }
}
