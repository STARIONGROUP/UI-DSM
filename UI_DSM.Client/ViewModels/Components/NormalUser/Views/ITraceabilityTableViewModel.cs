// --------------------------------------------------------------------------------------------------------
// <copyright file="ITraceabilityTableViewModel.cs" company="RHEA System S.A.">
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
    using UI_DSM.Client.ViewModels.App.ConnectionVisibilitySelector;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;

    /// <summary>
    ///     Interface definition for <see cref="TraceabilityTableViewModel" />
    /// </summary>
    public interface ITraceabilityTableViewModel
    {
        /// <summary>
        ///     A collection of rows for the table
        /// </summary>
        List<IHaveThingRowViewModel> Rows { get; }

        /// <summary>
        ///     A collection of columns for the table
        /// </summary>
        List<IHaveThingRowViewModel> Columns { get; }

        /// <summary>
        ///     The header of the table
        /// </summary>
        string HeaderName { get; set; }

        /// <summary>
        ///     Gets the <see cref="RelationshipRowViewModel" /> that links two <see cref="IHaveThingRowViewModel" />
        /// </summary>
        Func<IHaveThingRowViewModel, IHaveThingRowViewModel, RelationshipRowViewModel> GetRelationship { get; }

        /// <summary>
        ///     The selected <see cref="IHaveThingRowViewModel" />
        /// </summary>
        IHaveThingRowViewModel SelectedElement { get; set; }

        /// <summary>
        ///     A collection of visible rows
        /// </summary>
        List<IHaveThingRowViewModel> VisibleRows { get; }

        /// <summary>
        ///     A collection of visible rows
        /// </summary>
        List<IHaveThingRowViewModel> VisibleColumns { get; }

        /// <summary>
        ///     <see cref="Func{T1,TResult}" /> to check if the <see cref="IHaveThingRowViewModel" /> row is valid
        /// </summary>
        Func<IHaveThingRowViewModel, bool> IsValidRow { get; }

        /// <summary>
        ///     The <see cref="IConnectionVisibilitySelectorViewModel" /> for rows
        /// </summary>
        IConnectionVisibilitySelectorViewModel RowVisibilityState { get; }

        /// <summary>
        ///     The <see cref="IConnectionVisibilitySelectorViewModel" /> for columns
        /// </summary>
        IConnectionVisibilitySelectorViewModel ColumnVisibilityState { get; }

        /// <summary>
        ///     Selects the current <see cref="IHaveThingRowViewModel" />
        /// </summary>
        /// <param name="element">The <see cref="IHaveThingRowViewModel" /></param>
        void SelectElement(IHaveThingRowViewModel element);

        /// <summary>
        ///     Initializes this viewmodel property
        /// </summary>
        /// <param name="rows">A collection of <see cref="IHaveThingRowViewModel" /> for rows</param>
        /// <param name="columns">A collection of <see cref="IHaveThingRowViewModel" /> for columns</param>
        void InitializeProperties(List<IHaveThingRowViewModel> rows, List<IHaveThingRowViewModel> columns);

        /// <summary>
        ///     Updates this table
        /// </summary>
        void UpdateTable();

        /// <summary>
        ///     Verifies if the current <see cref="IHaveThingRowViewModel" /> trace any of the visible
        ///     <see cref="IHaveThingRowViewModel" /> column
        /// </summary>
        /// <param name="row">The <see cref="IHaveTraceabilityTableViewModel" /></param>
        /// <returns>The result of the verification</returns>
        bool DoesRowTraceAnyColumns(IHaveThingRowViewModel row);

        /// <summary>
        ///     Verifies if the current <see cref="IHaveThingRowViewModel" /> column trace any of the visible
        ///     <see cref="IHaveThingRowViewModel" /> row
        /// </summary>
        /// <param name="column">The <see cref="IHaveTraceabilityTableViewModel" /> column</param>
        /// <returns>The result of the verification</returns>
        bool DoesRowTraceAnyRows(IHaveThingRowViewModel column);
    }
}
