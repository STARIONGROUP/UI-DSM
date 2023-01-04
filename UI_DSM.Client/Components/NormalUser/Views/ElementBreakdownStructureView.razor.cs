// --------------------------------------------------------------------------------------------------------
// <copyright file="ElementBreakdownStructureView.razor.cs" company="RHEA System S.A.">
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
    using CDP4Common.EngineeringModelData;

    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;

    using Radzen;
    using Radzen.Blazor;

    using ReactiveUI;

    using UI_DSM.Client.Components.App.ColumnChooser;
    using UI_DSM.Client.ViewModels.App.ColumnChooser;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Abstract component for any breakdown view that is related to <see cref="ElementBase" />
    /// </summary>
    /// <typeparam name="TViewModel">an <see cref="IElementBreakdownStructureViewViewModel" /></typeparam>
    public abstract partial class ElementBreakdownStructureView<TViewModel> : GenericBaseView<TViewModel>, IDisposable where TViewModel : IElementBreakdownStructureViewViewModel
    {
        /// <summary>
        ///     Reference to the <see cref="ColumnChooser{TItem}" />
        /// </summary>
        protected readonly IColumnChooserViewModel<ElementBaseRowViewModel> ColumnChooser = new ColumnChooserViewModel<ElementBaseRowViewModel>();

        /// <summary>
        ///     A collection of <see cref="IDisposable" />
        /// </summary>
        private readonly List<IDisposable> disposables = new();

        /// <summary>
        ///     Value indicating if the first expand process has been called once
        /// </summary>
        private bool hasAlreadyExpandOnce;

        /// <summary>
        ///     Reference to the <see cref="RadzenDataGrid{TItem}" />
        /// </summary>
        public RadzenDataGrid<ElementBaseRowViewModel> Grid { get; set; }

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
        ///     Tries to navigate to a corresponding item
        /// </summary>
        /// <param name="itemName">The name of the item to navigate to</param>
        /// <returns>A <see cref="Task" /></returns>
        public override async Task TryNavigateToItem(string itemName)
        {
            var existingItem = this.ViewModel.GetAvailablesRows().OfType<ElementBaseRowViewModel>()
                .FirstOrDefault(x => x.Id == itemName);

            if (existingItem != null)
            {
                var path = this.ComputeTreePath(existingItem);
                await this.ExpandAllRows(path);
                await this.JsRuntime.InvokeVoidAsync("scrollToElement", $"row_{itemName}", "center", "center");
            }
        }

        /// <summary>
        ///     Initialize the correspondant ViewModel for this component
        /// </summary>
        /// <param name="things">The collection of <see cref="Thing" /></param>
        /// <param name="projectId">The <see cref="Project" /> id</param>
        /// <param name="reviewId">The <see cref="Review" /> id</param>
        /// <param name="reviewTaskId">The <see cref="ReviewTask" /> id</param>
        /// <param name="prefilters">A collection of prefilters</param>
        /// <param name="additionnalColumnsVisibleAtStart">A collection of columns name that can be visible by default at start</param>
        /// <returns>A <see cref="Task" /></returns>
        public override async Task InitializeViewModel(IEnumerable<Thing> things, Guid projectId, Guid reviewId, Guid reviewTaskId, List<string> prefilters, List<string> additionnalColumnsVisibleAtStart)
        {
            this.hasAlreadyExpandOnce = false;
            await base.InitializeViewModel(things, projectId, reviewId, reviewTaskId, prefilters, additionnalColumnsVisibleAtStart);
            this.IsLoading = false;
        }

        /// <summary>
        ///     Handle the on expand event
        /// </summary>
        /// <param name="row">The <see cref="ElementBaseRowViewModel" /></param>
        public void OnRowExpand(ElementBaseRowViewModel row)
        {
            row.IsExpanded = !row.IsExpanded;
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
        ///     Method invoked after each time the component has been rendered. Note that the component does
        ///     not automatically re-render after the completion of any returned <see cref="T:System.Threading.Tasks.Task" />, because
        ///     that would cause an infinite render loop.
        /// </summary>
        /// <param name="firstRender">
        ///     Set to <c>true</c> if this is the first time
        ///     <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRender(System.Boolean)" /> has been invoked
        ///     on this component instance; otherwise <c>false</c>.
        /// </param>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> representing any asynchronous operation.</returns>
        /// <remarks>
        ///     The <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRender(System.Boolean)" /> and
        ///     <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRenderAsync(System.Boolean)" /> lifecycle methods
        ///     are useful for performing interop, or interacting with values received from <c>@ref</c>.
        ///     Use the <paramref name="firstRender" /> parameter to ensure that initialization work is only performed
        ///     once.
        /// </remarks>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (!this.IsLoading && !this.hasAlreadyExpandOnce && this.ViewModel.TopElement.Any() && this.Grid != null)
            {
                var topElement = this.ViewModel.TopElement.First();
                await this.Grid.ExpandRow(topElement);

                foreach (var child in this.ViewModel.LoadChildren(topElement))
                {
                    await this.Grid.ExpandRow(child);
                }

                await this.InvokeAsync(this.StateHasChanged);

                this.hasAlreadyExpandOnce = true;

                this.ColumnChooser.InitializeProperties(this.Grid.ColumnsCollection);

                this.disposables.Add(this.WhenAnyValue(x => x.ColumnChooser.ColumnChooserVisible)
                    .Subscribe(_ => this.InvokeAsync(this.StateHasChanged)));

                this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.OptionChooserViewModel.SelectedOption)
                    .Subscribe(_ => this.InvokeAsync(this.OnOptionSelectionClose)));

                this.HideColumnsAtStart();

                this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.FilterViewModel.IsFilterVisible)
                    .Where(x => !x)
                    .Subscribe(_ => this.InvokeAsync(this.OnRowFilteringClose)));
            }
        }

        /// <summary>
        ///     Supplies information to a row render
        /// </summary>
        /// <param name="row">The <see cref="RowRenderEventArgs{T}" /></param>
        protected void RowRender(RowRenderEventArgs<ElementBaseRowViewModel> row)
        {
            row.Expandable = this.ViewModel.HasChildren(row.Data);
            row.Attributes["class"] = this.ViewModel.IsVisible(row.Data) ? string.Empty : "invisible-row";
            row.Attributes["id"] = $"row_{row.Data.Id}";
        }

        /// <summary>
        ///     Loads children of a row
        /// </summary>
        /// <param name="parent">The <see cref="DataGridLoadChildDataEventArgs{T}" /></param>
        protected void LoadChildren(DataGridLoadChildDataEventArgs<ElementBaseRowViewModel> parent)
        {
            parent.Data = this.ViewModel.LoadChildren(parent.Item);
        }

        /// <summary>
        ///     Selects a new <see cref="ElementBaseRowViewModel" />
        /// </summary>
        /// <param name="row">The new selected <see cref="ElementBaseRowViewModel" /></param>
        protected void SelectRow(ElementBaseRowViewModel row)
        {
            this.ViewModel.SelectedElement = row;
        }

        /// <summary>
        ///     Hides columns on start
        /// </summary>
        protected abstract void HideColumnsAtStart();

        /// <summary>
        ///     Computes the path inside the tree to access to the <see cref="ElementBaseRowViewModel" />
        /// </summary>
        /// <param name="item">The <see cref="ElementBaseRowViewModel" /></param>
        /// <returns>A collection of <see cref="ElementBaseRowViewModel" /> to have the path</returns>
        private IEnumerable<ElementBaseRowViewModel> ComputeTreePath(ElementBaseRowViewModel item)
        {
            var path = new List<ElementBaseRowViewModel>();
            var currentItem = item;

            while (currentItem.ContainerId != Guid.Empty)
            {
                var existingParent = this.ViewModel.GetAvailablesRows().OfType<ElementBaseRowViewModel>()
                    .FirstOrDefault(x => x.ThingId == currentItem.ContainerId || x.ElementDefinitionId == currentItem.ContainerId);

                if (existingParent != null)
                {
                    path.Add(existingParent);
                    currentItem = existingParent;
                }
                else
                {
                    break;
                }
            }

            path.Reverse();
            return path;
        }

        /// <summary>
        ///     Expands all rows that are contained into the collection
        /// </summary>
        /// <param name="rows">The collection <see cref="ElementBaseRowViewModel" />"/></param>
        /// <returns>A <see cref="Task" /></returns>
        private async Task ExpandAllRows(IEnumerable<ElementBaseRowViewModel> rows)
        {
            foreach (var row in rows)
            {
                if (!row.IsExpanded)
                {
                    await this.Grid.ExpandRow(row);
                }
            }
        }

        /// <summary>
        ///     Apply the filtering on rows
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private Task OnRowFilteringClose()
        {
            this.ViewModel.FilterRows(this.ViewModel.FilterViewModel.GetSelectedFilters());
            return this.HasChanged();
        }

        /// <summary>
        ///     Handles the change of <see cref="Option" />
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private Task OnOptionSelectionClose()
        {
            this.ViewModel.OptionChanged();

            if (this.ViewModel.SelectedElement is ElementBaseRowViewModel row)
            {
                this.SelectRow(null);
                this.SelectRow(row);
            }

            return this.OnRowFilteringClose();
        }
    }
}
