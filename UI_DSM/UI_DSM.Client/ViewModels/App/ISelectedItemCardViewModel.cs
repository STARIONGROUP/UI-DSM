// --------------------------------------------------------------------------------------------------------
// <copyright file="ISelectedItemCardViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.App
{
    /// <summary>
    ///     Interface definition for <see cref="SelectedItemCardViewModel" />
    /// </summary>
    public interface ISelectedItemCardViewModel
    {
        /// <summary>
        ///     The currently selected item
        /// </summary>
        object SelectedItem { get; set; }

        /// <summary>
        ///     The label to display
        /// </summary>
        string Label { get; }

        /// <summary>
        ///     The <see cref="Type" /> of the component
        /// </summary>
        Type ComponentType { get; }
    }
}
