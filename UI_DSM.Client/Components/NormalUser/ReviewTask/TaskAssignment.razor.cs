// --------------------------------------------------------------------------------------------------------
// <copyright file="TaskAssignement.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.NormalUser.ReviewTask
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.ViewModels.Components.NormalUser.ProjectReview;
    using UI_DSM.Client.ViewModels.Components.NormalUser.ReviewTask;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This component is used to assign a <see cref="Participant" /> to a <see cref="ReviewTask" />
    /// </summary>
    public partial class TaskAssignment
    {
        /// <summary>
        ///     A collection of <see cref="Participant" />s of project
        /// </summary>
        [Parameter]
        public IEnumerable<Participant> ProjectParticipants { get; set; } 

        /// <summary>
        ///     The <see cref="Participant" /> of the project
        /// </summary>
        public Participant Participant { get; set; } = new Participant();

        /// <summary>
        ///     The <see cref="IReviewCreationViewModel" /> for the component
        /// </summary>
        [Parameter]
        public ITaskAssignmentViewModel ViewModel { get; set; }
    }
}
