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
        ///     This right provide the capability to add and remove <see cref="Participant" /> for a <see cref="Project" />
        /// </summary>
        [Display(Name = "Manage Participant")]
        ManageParticipant = 0,

        /// <summary>
        ///     This right provide the capability to create a new Task inside a <see cref="Project" />
        /// </summary>
        [Display(Name = "Create Task")]
        CreateTask = 1,

        /// <summary>
        ///     This right provide the capability to delete a Task from a <see cref="Project" />
        /// </summary>
        [Display(Name = "Delete Task")]
        DeleteTask = 2,

        /// <summary>
        ///     This right provide the capability to update a Task inside a <see cref="Project" />
        /// </summary>
        [Display(Name = "Update Task")]
        UpdateTask = 3,

        /// <summary>
        ///     This right provide the capability to review a Task inside a <see cref="Project" />
        /// </summary>
        [Display(Name = "Review Task")]
        ReviewTask = 4
    }
}
