// --------------------------------------------------------------------------------------------------------
// <copyright file="IOptionChooserViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.App.OptionChooser
{
    using CDP4Common.EngineeringModelData;

    /// <summary>
    ///     Interface definition for <see cref="OptionChooserViewModel" />
    /// </summary>
    public interface IOptionChooserViewModel
    {
        /// <summary>
        ///     Value indicating if the selection of <see cref="Option" /> is visible
        /// </summary>
        bool IsVisible { get; set; }

        /// <summary>
        ///     The currently selected option
        /// </summary>
        Option SelectedOption { get; set; }

        /// <summary>
        ///     A collection of available <see cref="Option" />
        /// </summary>
        List<Option> AvailableOptions { get; }

        /// <summary>
        ///     Initializes this view model properties
        /// </summary>
        /// <param name="availableOptions">A collection of available <see cref="Option" /></param>
        void InitializesViewModel(List<Option> availableOptions);
    }
}
