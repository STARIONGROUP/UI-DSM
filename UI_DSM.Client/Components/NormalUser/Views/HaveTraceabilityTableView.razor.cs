// --------------------------------------------------------------------------------------------------------
// <copyright file="HaveTraceabilityTableView.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.NormalUser.Views
{
    using System.Reactive.Linq;

    using CDP4Common.CommonData;

    using ReactiveUI;

    using UI_DSM.Client.Components.App.TraceabilityTable;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Component for view that have a <see cref="TraceabilityTable" /> component
    /// </summary>
    /// <typeparam name="TViewModel">A <see cref="IHaveTraceabilityTableViewModel" /></typeparam>
    public abstract partial class HaveTraceabilityTableView<TViewModel> : GenericBaseView<TViewModel>, IDisposable
        where TViewModel : IHaveTraceabilityTableViewModel
    {
        /// <summary>
        ///     A collection of <see cref="IDisposable" />
        /// </summary>
        protected readonly List<IDisposable> Disposables = new();

        /// <summary>
        ///     The <see cref="TraceabilityTable" /> reference
        /// </summary>
        public TraceabilityTable Table { get; set; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Disposables.ForEach(x => x.Dispose());
        }

        /// <summary>
        ///     Handle the fact that something has changed and needs to update the view
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        public override async Task HasChanged()
        {
            await base.HasChanged();
            await this.Table.Update();
        }

        /// <summary>
        ///     Initialize the correspondant ViewModel for this component
        /// </summary>
        /// <param name="things">The collection of <see cref="Thing" /></param>
        /// <param name="projectId">The <see cref="Project" /> id</param>
        /// <param name="reviewId">The <see cref="Review" /> id</param>
        /// <param name="prefilters">A collection of prefilters</param>
        /// <param name="additionnalColumnsVisibleAtStart">A collection of columns name that can be visible by default at start</param>
        /// <returns>A <see cref="Task" /></returns>
        public override async Task InitializeViewModel(IEnumerable<Thing> things, Guid projectId, Guid reviewId, List<string> prefilters, List<string> additionnalColumnsVisibleAtStart)
        {
            this.Disposables.Add(this.ViewModel);
            this.Disposables.Add(this.Table);

            await this.ViewModel.InitializeProperties(things, projectId, reviewId, prefilters, additionnalColumnsVisibleAtStart);
            await this.Table.InitiliazeProperties(this.ViewModel.TraceabilityTableViewModel);

            this.Disposables.Add(this.WhenAnyValue(x => x.ViewModel.IsViewSettingsVisible)
                .Subscribe(_ => this.InvokeAsync(this.StateHasChanged)));

            this.Disposables.Add(this.WhenAnyValue(x => x.ViewModel.ColumnsFilterViewModel.IsFilterVisible)
                .Where(x => !x)
                .Subscribe(_ => this.InvokeAsync(this.OnColumnFilteringClose)));

            this.Disposables.Add(this.WhenAnyValue(x => x.ViewModel.RowsFilterViewModel.IsFilterVisible)
                .Where(x => !x)
                .Subscribe(_ => this.InvokeAsync(this.OnRowFilteringClose)));

            this.IsLoading = false;
        }

        /// <summary>
        ///     Apply the filtering on rows
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private async Task OnRowFilteringClose()
        {
            this.ViewModel.FilterRows(this.ViewModel.RowsFilterViewModel.GetSelectedFilters());
            await this.Table.Update();
        }

        /// <summary>
        ///     Toggle the view type dropdown
        /// </summary>
        protected void ToggleViewSettingsDropdown()
        {
            this.ViewModel.IsViewSettingsVisible = !this.ViewModel.IsViewSettingsVisible;
        }

        /// <summary>
        ///     Apply the filtering on rows
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private async Task OnColumnFilteringClose()
        {
            this.ViewModel.FilterColumns(this.ViewModel.ColumnsFilterViewModel.GetSelectedFilters());
            await this.Table.Update();
        }
    }
}
