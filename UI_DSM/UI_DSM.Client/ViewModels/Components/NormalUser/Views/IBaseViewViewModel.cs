// --------------------------------------------------------------------------------------------------------
// <copyright file="IBaseViewViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Components.NormalUser.Views
{
    using CDP4Common.CommonData;

    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface definition for <see cref="BaseViewViewModel" />
    /// </summary>
    public interface IBaseViewViewModel
    {
        /// <summary>
        ///     A collection of <see cref="Thing" />
        /// </summary>
        IEnumerable<Thing> Things { get; }

        /// <summary>
        ///     The currently selected element
        /// </summary>
        object SelectedElement { get; set; }

        /// <summary>
        ///     Initialize this view model properties
        /// </summary>
        /// <param name="things">A collection of <see cref="BaseViewViewModel.Things" /></param>
        /// <param name="projectId">The <see cref="Project" /> id</param>
        /// <param name="reviewId">The <see cref="Review" /> id</param>
        /// <returns>A <see cref="Task" /></returns>
        Task InitializeProperties(IEnumerable<Thing> things, Guid projectId, Guid reviewId);
    }
}
