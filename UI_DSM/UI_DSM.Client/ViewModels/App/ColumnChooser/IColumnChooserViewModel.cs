// --------------------------------------------------------------------------------------------------------
// <copyright file="IColumnChooserViewModel.cs" company="RHEA System S.A.">
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
    using Radzen.Blazor;

    /// <summary>
    ///     Interface definition for <see cref="ColumnChooserViewModel{TItem}" />
    /// </summary>
    public interface IColumnChooserViewModel<TItem>
    {
        /// <summary>
        ///     A collection of available <see cref="RadzenDataGridColumn{TItem}" />
        /// </summary>
        List<RadzenDataGridColumn<TItem>> AvailableColumns { get; }

        /// <summary>
        ///     Value indicating if the column chooser is visible or not
        /// </summary>
        bool ColumnChooserVisible { get; set; }

        /// <summary>
        ///     Initializes the properties
        /// </summary>
        /// <param name="columns">All available columns</param>
        void InitializeProperties(IEnumerable<RadzenDataGridColumn<TItem>> columns);

        /// <summary>
        ///     Opens the column chooser
        /// </summary>
        void OpenColumnChooser();

        /// <summary>
        ///     Modifies the current visibility value for <see cref="RadzenDataGridColumn{TItem}" />
        /// </summary>
        /// <param name="column">The <see cref="RadzenDataGridColumn{TItem}" /></param>
        void OnChangeValue(RadzenDataGridColumn<TItem> column);
    }
}
