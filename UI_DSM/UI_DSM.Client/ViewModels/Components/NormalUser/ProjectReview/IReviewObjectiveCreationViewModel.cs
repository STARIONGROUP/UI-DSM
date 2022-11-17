// --------------------------------------------------------------------------------------------------------
// <copyright file="IReviewObjectiveCreationViewModel.cs" company="RHEA System S.A.">
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
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface definition for <see cref="ReviewObjectiveCreationViewModel" />
    /// </summary>
    public interface IReviewObjectiveCreationViewModel
    {
        /// <summary>
        ///     The <see cref="ReviewObjective" /> to create
        /// </summary>
        ReviewObjective ReviewObjective { get; set; }

        /// <summary>
        ///     The <see cref="Guid" /> of the <see cref="Project" />
        /// </summary>
        Guid ProjectId { get; set; }

        /// <summary>
        ///     The <see cref="Guid" /> of the <see cref="Review" />
        /// </summary>
        Guid ReviewId { get; set; }

        /// <summary>
        ///     The <see cref="EventCallback" /> to call for data submit
        /// </summary>
        EventCallback OnValidSubmit { get; set; }

        /// <summary>
        ///     Value indicating the current status of the <see cref="List{ReviewObjective}" /> creation
        /// </summary>
        CreationStatus ReviewObjectivesCreationStatus { get; set; }

        /// <summary>
        ///     A collection of <see cref="ReviewObjectiveCreationDto" /> of kind Prr that has been selected
        /// </summary>
        IEnumerable<ReviewObjectiveCreationDto> SelectedReviewObjectivesPrr { get; set; }

        /// <summary>
        ///     A collection of <see cref="ReviewObjectiveCreationDto" /> of kind Srr that has been selected
        /// </summary>
        IEnumerable<ReviewObjectiveCreationDto> SelectedReviewObjectivesSrr { get; set; }

        /// <summary>
        ///     A collection of <see cref="ReviewObjectiveCreationDto" /> that has been selected
        /// </summary>
        IEnumerable<ReviewObjectiveCreationDto> SelectedReviewObjectives { get; set; }

        /// <summary>
        ///     A collection of <see cref="ReviewObjectiveCreationDto" /> available for a review
        /// </summary>
        List<ReviewObjectiveCreationDto> AvailableReviewObjectiveCreationDto { get; set; }

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        ///     Override this method if you will perform an asynchronous operation and
        ///     want the component to refresh when that operation is completed.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        Task OnInitializedAsync();
    }
}
