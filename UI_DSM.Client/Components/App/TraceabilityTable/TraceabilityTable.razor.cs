// --------------------------------------------------------------------------------------------------------
// <copyright file="TraceabilityTable.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.App.TraceabilityTable
{
    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;

    using ReactiveUI;

    using UI_DSM.Client.Enumerator;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;

    /// <summary>
    ///     Generic component for a Traceability Table
    /// </summary>
    public partial class TraceabilityTable : IDisposable
    {
        /// <summary>
        ///     A collection of <see cref="IDisposable" />
        /// </summary>
        private readonly List<IDisposable> disposables = new();

        /// <summary>
        ///     A collection of <see cref="IHaveThingRowViewModel" /> columns that have to be displayed
        /// </summary>
        private List<IHaveThingRowViewModel> columnsToDisplay = new();

        /// <summary>
        ///     Backing field for the <see cref="IsLoading" /> property
        /// </summary>
        private bool isLoading;

        /// <summary>
        ///     A collection of <see cref="IHaveThingRowViewModel" /> rows that have to be displayed
        /// </summary>
        private List<IHaveThingRowViewModel> rowsToDisplay = new();

        /// <summary>
        ///     The <see cref="ITraceabilityTableViewModel" />
        /// </summary>
        private ITraceabilityTableViewModel ViewModel { get; set; }

        /// <summary>
        ///     Gets or sets if the view is loading
        /// </summary>
        [Parameter]
        public bool IsLoading
        {
            get => this.isLoading;
            set
            {
                this.isLoading = value;
                this.InvokeAsync(this.StateHasChanged);
            }
        }

        /// <summary>
        ///     The <see cref="IJSRuntime" />
        /// </summary>
        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.disposables.ForEach(x => x.Dispose());
        }

        /// <summary>
        ///     Initialize this component
        /// </summary>
        /// <param name="viewModel">The <see cref="ITraceabilityTableViewModel" /></param>
        public async Task InitiliazeProperties(ITraceabilityTableViewModel viewModel)
        {
            this.ViewModel = viewModel;
            this.IsLoading = false;

            await this.InvokeAsync(this.StateHasChanged);

            this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.RowVisibilityState.CurrentState)
                .Subscribe(_ => this.InvokeAsync(this.UpdateRowsToDisplay)));

            this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.ColumnVisibilityState.CurrentState)
                .Subscribe(_ => this.InvokeAsync(this.UpdateColumnsToDisplay)));
        }

        /// <summary>
        ///     Updates the view
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        public async Task Update()
        {
            this.ViewModel.UpdateTable();
            await this.InvokeAsync(this.StateHasChanged);
        }

        /// <summary>
        ///     Scrolls the table to the selected item
        /// </summary>
        /// <param name="itemName">The name of the item</param>
        /// <returns>A <see cref="Task" /></returns>
        public async Task ScrollToElement(string itemName)
        {
            if (itemName.Contains(" -> "))
            {
                var splittedName = itemName.Split(" -> ");

                var sourceItem = this.rowsToDisplay
                    .FirstOrDefault(x => string.Equals(splittedName[0], x.Id, StringComparison.InvariantCultureIgnoreCase));

                var targetItem = this.columnsToDisplay
                    .FirstOrDefault(x => string.Equals(splittedName[1], x.Id, StringComparison.InvariantCultureIgnoreCase));

                var relationShip = this.ViewModel.GetRelationship(sourceItem, targetItem);

                if (relationShip != null)
                {
                    await this.JsRuntime.InvokeVoidAsync("scrollToElement", $"cell_{relationShip.ThingId}", "center", "center");
                }

                return;
            }

            if (!await this.TryScrollToItem(this.rowsToDisplay, itemName, true))
            {
                await this.TryScrollToItem(this.columnsToDisplay, itemName, false);
            }
        }

        /// <summary>
        ///     Tries to scroll to an <see cref="IHaveThingRowViewModel" />
        /// </summary>
        /// <param name="items">The collection of <see cref="IHaveThingRowViewModel" /></param>
        /// <param name="itemName">The name of the <see cref="IHaveThingRowViewModel" /></param>
        /// <param name="isRow">Value asserting if the item has to be considered as row or column</param>
        /// <returns>A <see cref="Task" /> with the value asserting if could navigate or not</returns>
        public async Task<bool> TryScrollToItem(List<IHaveThingRowViewModel> items, string itemName, bool isRow)
        {
            var item = items.FirstOrDefault(x => string.Equals(itemName, x.Id, StringComparison.InvariantCultureIgnoreCase));

            if (item != null)
            {
                var relationShip = this.ViewModel.GetRelationship(item, this.ViewModel.SelectedElement)
                                   ?? this.ViewModel.GetRelationship(this.ViewModel.SelectedElement, item);

                if (relationShip != null)
                {
                    await this.JsRuntime.InvokeVoidAsync("scrollToElement", $"cell_{relationShip.ThingId}", "center", "center");
                    return true;
                }

                var idPrefix = isRow ? "row" : "column";
                var blockOption = isRow ? "center" : "nearest";
                var inlineOption = isRow ? "nearest" : "center";
                await this.JsRuntime.InvokeVoidAsync("scrollToElement", $"{idPrefix}_{item.ThingId}", blockOption, inlineOption);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        /// </summary>
        protected override void OnInitialized()
        {
            this.IsLoading = true;
            base.OnInitialized();
        }

        /// <summary>
        ///     Updates the rows to display
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private Task UpdateRowsToDisplay()
        {
            this.rowsToDisplay = this.ViewModel.RowVisibilityState?.CurrentState switch
            {
                ConnectionToVisibilityState.Connected => this.ViewModel.VisibleRows.Where(x => this.ViewModel.DoesRowTraceAnyColumns(x)).ToList(),
                ConnectionToVisibilityState.NotConnected => this.ViewModel.VisibleRows.Where(x => !this.ViewModel.DoesRowTraceAnyColumns(x)).ToList(),
                _ => this.ViewModel.VisibleRows
            };

            return this.InvokeAsync(this.StateHasChanged);
        }

        /// <summary>
        ///     Updates the columns to display
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private Task UpdateColumnsToDisplay()
        {
            this.columnsToDisplay = this.ViewModel.ColumnVisibilityState?.CurrentState switch
            {
                ConnectionToVisibilityState.Connected => this.ViewModel.VisibleColumns.Where(x => this.ViewModel.DoesRowTraceAnyRows(x)).ToList(),
                ConnectionToVisibilityState.NotConnected => this.ViewModel.VisibleColumns.Where(x => !this.ViewModel.DoesRowTraceAnyRows(x)).ToList(),
                _ => this.ViewModel.VisibleColumns
            };

            return this.InvokeAsync(this.StateHasChanged);
        }

        /// <summary>
        ///     Gets the valid css class for a td element
        /// </summary>
        /// <param name="currentRow">The current <see cref="IHaveThingRowViewModel" /> row</param>
        /// <param name="currentColumn">The current <see cref="IHaveThingRowViewModel" /> column</param>
        /// <returns>The css class</returns>
        private static string GetTdClass(IHaveThingRowViewModel currentRow, IHaveThingRowViewModel currentColumn)
        {
            return currentRow.Equals(currentColumn) ? "same" : string.Empty;
        }

        /// <summary>
        ///     Gets the valid css class for a tr element
        /// </summary>
        /// <param name="currentRow">The current <see cref="IHaveThingRowViewModel" /> row</param>
        /// <returns>The css class</returns>
        private string GetTrClass(IHaveThingRowViewModel currentRow)
        {
            return this.ViewModel.IsValidRow(currentRow) ? string.Empty : "invalid";
        }

        /// <summary>
        ///     Gets the valid css class for a header element
        /// </summary>
        /// <param name="row">The current <see cref="IHaveThingRowViewModel" /> row</param>
        /// <returns>The css class</returns>
        private string GetHeaderClass(IHaveThingRowViewModel row)
        {
            return row.Id.Length < 30 ? string.Empty : "app-traceability-table__header--large";
        }
    }
}
