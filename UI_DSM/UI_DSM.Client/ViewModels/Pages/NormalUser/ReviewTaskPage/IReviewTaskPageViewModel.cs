// --------------------------------------------------------------------------------------------------------
// <copyright file="IReviewTaskPageViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Pages.NormalUser.ReviewTaskPage
{
    using CDP4Common.CommonData;

    using DynamicData;

    using UI_DSM.Client.Components.NormalUser.Views;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface definition for <see cref="ReviewTaskPageViewModel" />
    /// </summary>
    public interface IReviewTaskPageViewModel
    {
        /// <summary>
        ///     The current <see cref="ReviewObjective" />
        /// </summary>
        ReviewObjective ReviewObjective { get; set; }

        /// <summary>
        ///     A collection of <see cref="Thing" /> that are suitable for the current <see cref="ReviewTask" />
        /// </summary>
        IEnumerable<Thing> Things { get; set; }

        /// <summary>
        ///     The current <see cref="Type" /> for the <see cref="GenericBaseView{TViewModel}" />
        /// </summary>
        Type CurrentBaseView { get; set; }

        /// <summary>
        ///     The <see cref="IReviewTaskPageViewModel.ReviewTask" />
        /// </summary>
        ReviewTask ReviewTask { get; set; }

        /// <summary>
        ///     A collection of <see cref="Comment" />
        /// </summary>
        SourceList<Comment> Comments { get; set; }

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        ///     Override this method if you will perform an asynchronous operation and
        ///     want the component to refresh when that operation is completed.
        /// </summary>
        /// <param name="projectId">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <param name="reviewId">The <see cref="Guid" /> of the <see cref="Review" /></param>
        /// <param name="reviewObjectiveId">The <see cref="Guid" /> of the <see cref="ReviewTaskPageViewModel.ReviewObjective" /></param>
        /// <param name="reviewTaskId">The <see cref="Guid" /> of the <see cref="IReviewTaskPageViewModel.ReviewTask" /></param>
        /// <returns>A <see cref="Task" /></returns>
        Task OnInitializedAsync(Guid projectId, Guid reviewId, Guid reviewObjectiveId, Guid reviewTaskId);
    }
}
