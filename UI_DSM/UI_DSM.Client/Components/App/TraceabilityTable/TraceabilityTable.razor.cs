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
        ///     The <see cref="ITraceabilityTableViewModel" />
        /// </summary>
        private ITraceabilityTableViewModel ViewModel { get; set; }

        /// <summary>
        ///     Initialize this component
        /// </summary>
        /// <param name="viewModel">The <see cref="ITraceabilityTableViewModel" /></param>
        public async Task InitiliazeProperties(ITraceabilityTableViewModel viewModel)
        {
            this.ViewModel = viewModel;
            await this.InvokeAsync(this.StateHasChanged);
            
            this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.VisibilityState.CurrentState)
                .Subscribe(_ => this.InvokeAsync(this.StateHasChanged)));
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
            var cssClass = this.ViewModel.VisibilityState?.CurrentState switch
            {
                ConnectionToVisibilityState.Connected => this.ViewModel.DoesRowTraceAnyColumns(currentRow) ? string.Empty : "invisible-row",
                ConnectionToVisibilityState.NotConnected => !this.ViewModel.DoesRowTraceAnyColumns(currentRow) ? string.Empty : "invisible-row",
                _ => string.Empty
            };

            if (string.IsNullOrEmpty(cssClass))
            {
                cssClass = this.ViewModel.IsValidRow(currentRow) ? string.Empty : "invalid";
            }

            return cssClass;
        }

        /// <summary>
        ///     Gets the valid css class for a header element
        /// </summary>
        /// <param name="row">The current <see cref="IHaveThingRowViewModel" /> row</param>
        /// <returns>The css class</returns>
        private string GetHeaderClass(IHaveThingRowViewModel row)
        {
            var cssClass = row.Id.Length < 30 ? string.Empty : "app-traceability-table__header--large";
            return cssClass;
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.disposables.ForEach(x => x.Dispose());
        }
    }
}
