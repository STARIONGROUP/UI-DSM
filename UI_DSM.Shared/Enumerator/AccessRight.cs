// --------------------------------------------------------------------------------------------------------
// <copyright file="AccessRight.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Shared.Enumerator
{
    using System.ComponentModel.DataAnnotations;

    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Enumeration of possible Access Rights for execution of functions in the UI DSM.
    /// </summary>
    public enum AccessRight
    {
        /// <summary>
        ///     This right provide the capability to review a <see cref="ReviewTask" /> (commenting and marking as done)
        /// </summary>
        [Display(Name = "Review Task")] ReviewTask = 0,

        /// <summary>
        ///     This right provide the capability to create a new <see cref="Review" /> inside a <see cref="Project" />
        /// </summary>
        [Display(Name = "Create Review")] CreateReview = 1,

        /// <summary>
        ///     This right provide the capability to delete a <see cref="Review" /> from a <see cref="Project" />
        /// </summary>
        [Display(Name = "Delete Review")] DeleteReview = 2,

        /// <summary>
        ///     This right provide the capability to create a new <see cref="ReviewObjective" /> inside a <see cref="Project" />
        /// </summary>
        [Display(Name = "Create Review Objective")]
        CreateReviewObjective = 3,

        /// <summary>
        ///     This right provide the capability to delete a <see cref="ReviewObjective" /> from a <see cref="Project" />
        /// </summary>
        [Display(Name = "Delete Review Objective")]
        DeleteReviewObjective = 4,

        /// <summary>
        ///     This right provide the capability to assign a <see cref="ReviewTask" /> to a <see cref="Participant" />
        /// </summary>
        [Display(Name = "Assign Task")] AssignTask = 5,

        /// <summary>
        ///     This right provide the capability to manage participants and models inside a project
        /// </summary>
        [Display(Name = "Manage Project")] ProjectManagement = 6,

        /// <summary>
        ///     This right provide the capability to save a diagram configuration
        /// </summary>
        [Display(Name = "Save Diagram Configuration")] CreateDiagramConfiguration = 7
    }
}
