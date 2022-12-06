// --------------------------------------------------------------------------------------------------------
// <copyright file="OptionChooserViewModel.cs" company="RHEA System S.A.">
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

    using ReactiveUI;

    /// <summary>
    ///     View model for the <see cref="Client.Components.App.OptionChooser.OptionChooser" /> component
    /// </summary>
    public class OptionChooserViewModel : ReactiveObject, IOptionChooserViewModel
    {
        /// <summary>
        ///     Backing field for <see cref="IsVisible" />
        /// </summary>
        private bool isVisible;

        /// <summary>
        ///     Backing field for <see cref="SelectedOption" />
        /// </summary>
        private Option selectedOption;

        /// <summary>
        ///     Value indicating if the selection of <see cref="Option" /> is visible
        /// </summary>
        public bool IsVisible
        {
            get => this.isVisible;
            set => this.RaiseAndSetIfChanged(ref this.isVisible, value);
        }

        /// <summary>
        ///     The currently selected option
        /// </summary>
        public Option SelectedOption
        {
            get => this.selectedOption;
            set => this.RaiseAndSetIfChanged(ref this.selectedOption, value);
        }

        /// <summary>
        ///     A collection of available <see cref="Option" />
        /// </summary>
        public List<Option> AvailableOptions { get; private set; } = new();

        /// <summary>
        ///     Initializes this view model properties
        /// </summary>
        /// <param name="availableOptions">A collection of available <see cref="Option" /></param>
        public void InitializesViewModel(List<Option> availableOptions)
        {
            this.AvailableOptions = availableOptions;
            this.SelectedOption = this.AvailableOptions.FirstOrDefault(x => x.IsDefault) ?? this.AvailableOptions.FirstOrDefault();
        }
    }
}
