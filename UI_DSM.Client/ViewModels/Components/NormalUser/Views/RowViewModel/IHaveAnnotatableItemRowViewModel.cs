// --------------------------------------------------------------------------------------------------------
// <copyright file="IHaveAnnotatableItemRowViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel
{
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface that defines that a row view model has an <see cref="AnnotatableItem" />
    /// </summary>
    public interface IHaveAnnotatableItemRowViewModel
    {
        /// <summary>
        ///     The <see cref="Guid" /> of the <see cref="AnnotatableItem" /> if exists
        /// </summary>
        public Guid? AnnotatableItemId { get; }

        /// <summary>
        ///     Gets the Id of the current <see cref="IHaveThingRowViewModel" />
        /// </summary>
        public string Id { get; }

        /// <summary>
        ///     A value indicating if the row is visible or not
        /// </summary>
        bool IsVisible { get; set; }

        /// <summary>
        ///     Indicates if the current associated <see cref="AnnotatableItem" /> has some <see cref="Comment" />
        /// </summary>
        /// <returns>The assert</returns>
        bool HasComment();
    }
}
