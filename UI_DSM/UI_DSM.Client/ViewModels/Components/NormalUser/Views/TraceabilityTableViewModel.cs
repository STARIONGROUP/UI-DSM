// --------------------------------------------------------------------------------------------------------
// <copyright file="TraceabilityTableViewModel.cs" company="RHEA System S.A.">
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
    using ReactiveUI;

    using UI_DSM.Client.Components.App.TraceabilityTable;
    using UI_DSM.Client.Enumerator;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;

    /// <summary>
    ///     ViewModel for the abstract component <see cref="TraceabilityTable" />
    /// </summary>
    public class TraceabilityTableViewModel : ReactiveObject, ITraceabilityTableViewModel
    {
        /// <summary>
        ///     Backing field for <see cref="SelectedElement" />
        /// </summary>
        private IHaveThingRowViewModel selectedElement;

        /// <summary>
        ///     Backing field for <see cref="VisibilityState" />
        /// </summary>
        private TraceabilityToVisibilityState visibilityState;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TraceabilityTableViewModel" /> class.
        /// </summary>
        /// <param name="headerName">The name of the header</param>
        /// <param name="getRelationship"><see cref="Func{T1,T2,TResult}" /> for defining how the trace is computed</param>
        /// <param name="isValidRow"><see cref="Func{T1,TResult}" /> to check if the <see cref="IHaveThingRowViewModel" /> row is valid</param>
        public TraceabilityTableViewModel(string headerName, Func<IHaveThingRowViewModel, IHaveThingRowViewModel, RelationshipRowViewModel> getRelationship,
            Func<IHaveThingRowViewModel, bool> isValidRow)
        {
            this.HeaderName = headerName;
            this.GetRelationship = getRelationship;
            this.IsValidRow = isValidRow;
        }

        /// <summary>
        ///     <see cref="Func{T1,TResult}" /> to check if the <see cref="IHaveThingRowViewModel" /> row is valid
        /// </summary>
        public Func<IHaveThingRowViewModel, bool> IsValidRow { get; }

        /// <summary>
        ///     The <see cref="TraceabilityToVisibilityState" />
        /// </summary>
        public TraceabilityToVisibilityState VisibilityState
        {
            get => this.visibilityState;
            set => this.RaiseAndSetIfChanged(ref this.visibilityState, value);
        }

        /// <summary>
        ///     A collection of visible rows
        /// </summary>
        public List<IHaveThingRowViewModel> VisibleRows { get; protected set; }

        /// <summary>
        ///     A collection of visible rows
        /// </summary>
        public List<IHaveThingRowViewModel> VisibleColumns { get; protected set; }

        /// <summary>
        ///     The selected <see cref="IHaveThingRowViewModel" />
        /// </summary>
        public IHaveThingRowViewModel SelectedElement
        {
            get => this.selectedElement;
            set => this.RaiseAndSetIfChanged(ref this.selectedElement, value);
        }

        /// <summary>
        ///     Gets the <see cref="RelationshipRowViewModel" /> between two <see cref="IHaveThingRowViewModel" />
        /// </summary>
        public Func<IHaveThingRowViewModel, IHaveThingRowViewModel, RelationshipRowViewModel> GetRelationship { get; }

        /// <summary>
        ///     A collection of rows for the table
        /// </summary>
        public List<IHaveThingRowViewModel> Rows { get; protected set; }

        /// <summary>
        ///     A collection of columns for the table
        /// </summary>
        public List<IHaveThingRowViewModel> Columns { get; protected set; }

        /// <summary>
        ///     The header of the table
        /// </summary>
        public string HeaderName { get; }

        /// <summary>
        ///     Selects the current <see cref="IHaveThingRowViewModel" />
        /// </summary>
        /// <param name="element">The <see cref="IHaveThingRowViewModel" /></param>
        public void SelectElement(IHaveThingRowViewModel element)
        {
            this.SelectedElement = element;
        }

        /// <summary>
        ///     Initializes this viewmodel property
        /// </summary>
        /// <param name="rows">A collection of <see cref="IHaveThingRowViewModel" /> for rows</param>
        /// <param name="columns">A collection of <see cref="IHaveThingRowViewModel" /> for columns</param>
        public void InitializeProperties(List<IHaveThingRowViewModel> rows, List<IHaveThingRowViewModel> columns)
        {
            this.Rows = rows;
            this.Columns = columns;
            this.UpdateTable();
        }

        /// <summary>
        ///     Updates this table
        /// </summary>
        public void UpdateTable()
        {
            this.VisibleRows = this.Rows.Where(x => x.IsVisible).ToList();
            this.VisibleColumns = this.Columns.Where(x => x.IsVisible).ToList();
        }

        /// <summary>
        ///     Verifies if the current <see cref="IHaveThingRowViewModel" /> trace any of the visible
        ///     <see cref="IHaveThingRowViewModel" /> column
        /// </summary>
        /// <param name="row">The <see cref="IHaveTraceabilityTableViewModel" />The <see cref="IHaveThingRowViewModel" /></param>
        /// <returns>The result of the verification</returns>
        public bool DoesRowTraceAnyColumns(IHaveThingRowViewModel row)
        {
            return this.VisibleColumns.Any(column => this.GetRelationship(row, column) != null);
        }
    }
}
