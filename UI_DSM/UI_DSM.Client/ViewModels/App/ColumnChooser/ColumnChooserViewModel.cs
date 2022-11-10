// --------------------------------------------------------------------------------------------------------
// <copyright file="ColumnChooserViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.App.ColumnChooser
{
    using ReactiveUI;

    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;

    /// <summary>
    ///     View model for the <see cref="Client.Components.App.ColumnChooser.ColumnChooser" /> component
    /// </summary>
    public class ColumnChooserViewModel : ReactiveObject, IColumnChooserViewModel
    {
        /// <summary>
        ///     Backing field for <see cref="ColumnChooserVisible" />
        /// </summary>
        private bool columnChooserVisible;

        /// <summary>
        ///     A collection of available <see cref="IHaveThingRowViewModel" />
        /// </summary>
        public List<IHaveThingRowViewModel> AvailableColumns { get; private set; } = new();

        /// <summary>
        ///     Value indicating if the column chooser is visible or not
        /// </summary>
        public bool ColumnChooserVisible
        {
            get => this.columnChooserVisible;
            set => this.RaiseAndSetIfChanged(ref this.columnChooserVisible, value);
        }

        /// <summary>
        ///     Initializes the properties
        /// </summary>
        /// <param name="columns">All available rows</param>
        public void InitializeProperties(IEnumerable<IHaveThingRowViewModel> columns)
        {
            this.AvailableColumns = new List<IHaveThingRowViewModel>(columns);
        }

        /// <summary>
        ///     Opens the column chooser
        /// </summary>
        public void OpenColumnChooser()
        {
            this.ColumnChooserVisible = true;
        }

        /// <summary>
        ///     Modifies the current visibility value for <see cref="IHaveThingRowViewModel" />
        /// </summary>
        /// <param name="column">The <see cref="IHaveThingRowViewModel" /></param>
        public void OnChangeValue(IHaveThingRowViewModel column)
        {
            column.IsVisible = !column.IsVisible;
        }
    }
}
