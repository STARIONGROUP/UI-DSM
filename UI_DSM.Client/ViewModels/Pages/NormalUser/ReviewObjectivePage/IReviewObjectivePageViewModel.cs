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
    using UI_DSM.Client.ViewModels.Components;
    using UI_DSM.Client.ViewModels.Components.NormalUser.ReviewTask;
    using UI_DSM.Client.Components.NormalUser.ReviewTask;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface definition for <see cref="ReviewObjectivePageViewModel" />
    /// </summary>
    public interface IReviewObjectivePageViewModel
    {
        /// <summary>
        ///     The <see cref="Project" /> id
        /// </summary>
        Guid ProjectId { get; set; }

        /// <summary>
        ///     The <see cref="Review" /> id
        /// </summary>
        Guid ReviewId { get; set; }
        
        /// <summary>
        ///     The <see cref="ReviewObjective" /> of the page
        /// </summary>
        ReviewObjective ReviewObjective { get; }

        /// <summary>
        ///     The <see cref="Participant" />s of the project
        /// </summary>
        List<Participant> ProjectParticipants { get; set; }

        /// <summary>
        ///     Value indicating the user is currently assigning a <see cref="Participant" /> to a task
        /// </summary>
        bool IsOnAssignmentMode { get; set; }

        /// <summary>
        ///     The <see cref="IErrorMessageViewModel" />
        /// </summary>
        IErrorMessageViewModel ErrorMessageViewModel { get; }

        /// <summary>
        ///     The <see cref="ITaskAssignmentViewModel" />
        /// </summary>
        ITaskAssignmentViewModel TaskAssignmentViewModel { get; }

        /// <summary>
        ///     Opens the <see cref="TaskAssignment" /> as a popup
        /// </summary>
        void OpenTaskAssignmentPopup(ReviewTask selectedReviewTask);

        /// <summary>
        ///     The current <see cref="Participant" />
        /// </summary>
        Participant Participant { get; set; }

        /// <summary>
        ///     The selected <see cref="ReviewTask" /> 
        /// </summary>
        ReviewTask SelectedReviewTask { get; set; }

        /// <summary>
        ///     A <see cref="Dictionary{Guid, AdditionalComputedProperties}" /> for the <see cref="Comment" /> count
        /// </summary>
        Dictionary<Guid, AdditionalComputedProperties> CommentsCount { get; set; }

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
