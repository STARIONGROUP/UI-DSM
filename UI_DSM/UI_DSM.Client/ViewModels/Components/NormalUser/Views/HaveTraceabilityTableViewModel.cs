// --------------------------------------------------------------------------------------------------------
// <copyright file="HaveTraceabilityTableViewModel.cs" company="RHEA System S.A.">
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

    using ReactiveUI;

    using UI_DSM.Client.Components.App.TraceabilityTable;
    using UI_DSM.Client.Extensions;
    using UI_DSM.Client.Model;
    using UI_DSM.Client.Services.ReviewItemService;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;

    /// <summary>
    ///     View model for any View that have a <see cref="TraceabilityTable" /> component
    /// </summary>
    public abstract class HaveTraceabilityTableViewModel : BaseViewViewModel, IHaveTraceabilityTableViewModel
    {
        /// <summary>
        ///     A collection of <see cref="IDisposable" />
        /// </summary>
        private readonly List<IDisposable> disposables = new();

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseViewViewModel" /> class.
        /// </summary>
        /// <param name="reviewItemService">The <see cref="IReviewItemService" /></param>
        protected HaveTraceabilityTableViewModel(IReviewItemService reviewItemService) : base(reviewItemService)
        {
            this.InitializeTable();
        }

        /// <summary>
        ///     The header name of the column
        /// </summary>
        protected abstract string HeaderName { get; }

        /// <summary>
        ///     The name of the category
        /// </summary>
        protected abstract string TraceCategoryName { get; }

        /// <summary>
        ///     The <see cref="ITraceabilityTableViewModel" />
        /// </summary>
        public ITraceabilityTableViewModel TraceabilityTableViewModel { get; private set; }

        /// <summary>
        ///     A collection of available <see cref="FilterModel" /> for rows
        /// </summary>
        public List<FilterModel> AvailableRowFilters { get; } = new();

        /// <summary>
        ///     A collection of available <see cref="FilterModel" /> for columns
        /// </summary>
        public List<FilterModel> AvailableColumnFilters { get; } = new();

        /// <summary>
        ///     Apply a filtering on rows
        /// </summary>
        /// <param name="selectedFilters">The selected filters</param>
        public abstract void FilterRows(Dictionary<ClassKind, List<FilterRow>> selectedFilters);

        /// <summary>
        ///     Apply a filtering on columns
        /// </summary>
        /// <param name="selectedFilters">The selected filters</param>
        public abstract void FilterColumns(Dictionary<ClassKind, List<FilterRow>> selectedFilters);

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.disposables.ForEach(x => x.Dispose());
        }

        /// <summary>
        ///     Verifies that a <see cref="IHaveThingRowViewModel" /> is valid
        /// </summary>
        /// <param name="row">A <see cref="IHaveThingRowViewModel" /></param>
        /// <returns>The result of the verification</returns>
        protected abstract bool IsValidRow(IHaveThingRowViewModel row);

        /// <summary>
        ///     Verifies if the current <see cref="row" /> traces the <see cref="column" />
        /// </summary>
        /// <param name="row">The row <see cref="IHaveThingRowViewModel" /></param>
        /// <param name="column">The column <see cref="IHaveThingRowViewModel" /></param>
        /// <returns>A value indicating the traceability</returns>
        private bool DoesRowTracesColumn(IHaveThingRowViewModel row, IHaveThingRowViewModel column)
        {
            if (row is RequirementRowViewModel requirementRow)
            {
                return requirementRow.Thing.IsLinkedTo(column.ThingId, this.TraceCategoryName);
            }

            if (row is ProductRowViewModel productRow)
            {
                return productRow.Thing.IsLinkedTo(column.ThingId, this.TraceCategoryName);
            }

            return false;
        }

        /// <summary>
        ///     Initializes the <see cref="TraceabilityTableViewModel" /> properties
        /// </summary>
        private void InitializeTable()
        {
            this.TraceabilityTableViewModel = new TraceabilityTableViewModel(this.HeaderName, this.DoesRowTracesColumn, this.IsValidRow);

            this.disposables.Add(this.WhenAnyValue(x => x.TraceabilityTableViewModel.SelectedElement)
                .Subscribe(this.UpdateSelectedElement));
        }

        /// <summary>
        ///     Update the current selected element
        /// </summary>
        /// <param name="selectedElement">A <see cref="IHaveThingRowViewModel" /></param>
        private void UpdateSelectedElement(IHaveThingRowViewModel selectedElement)
        {
            this.SelectedElement = selectedElement;
        }
    }
}
