// --------------------------------------------------------------------------------------------------------
// <copyright file="InterfaceView.razor.cs" company="RHEA System S.A.">
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

    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;

    using Radzen;
    using Radzen.Blazor;

    using ReactiveUI;

    using UI_DSM.Client.Components.App.ConnectionVisibilitySelector;
    using UI_DSM.Client.Enumerator;
    using UI_DSM.Client.ViewModels.App.ColumnChooser;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Component for the <see cref="View.InterfaceView" />
    /// </summary>
    public partial class InterfaceView : GenericBaseView<IInterfaceViewViewModel>, IDisposable, IReusableView
    {
        /// <summary>
        ///     Reference to the <see cref="IColumnChooserViewModel{TItem}" />
        /// </summary>
        private readonly IColumnChooserViewModel<IBelongsToInterfaceView> columnChooser = new ColumnChooserViewModel<IBelongsToInterfaceView>();

        /// <summary>
        ///     A collection of <see cref="IDisposable" />
        /// </summary>
        private readonly List<IDisposable> disposables = new();

        /// <summary>
        ///     Value indicating the the view is fully initialized
        /// </summary>
        private bool isFullyInitialized;

        /// <summary>
        /// The <see cref="Guid"/> of the item to navigate to
        /// </summary>
        private Guid navigationId = Guid.Empty;

        /// <summary>
        ///     Reference to the <see cref="RadzenDataGrid{TItem}" />
        /// </summary>
        public RadzenDataGrid<IBelongsToInterfaceView> Grid { get; set; }

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
        ///     Tries to copy components from another <see cref="BaseView" />
        /// </summary>
        /// <param name="otherView">The other <see cref="BaseView" /></param>
        /// <returns>Value indicating if it could copy components</returns>
        public async Task<bool> CopyComponents(BaseView otherView)
        {
            if (otherView is not GenericBaseView<IInterfaceViewViewModel> interfaceView)
            {
                return false;
            }

            this.ViewModel = interfaceView.ViewModel;
            this.IsLoading = false;
            this.isFullyInitialized = false;
            await Task.CompletedTask;
            return true;
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
        /// <param name="participant">The current <see cref="Participant" /></param>
        /// <returns>A <see cref="Task" /></returns>
        public override async Task InitializeViewModel(IEnumerable<Thing> things, Guid projectId, Guid reviewId, Guid reviewTaskId, List<string> prefilters, List<string> additionnalColumnsVisibleAtStart, Participant participant)
        {
            await base.InitializeViewModel(things, projectId, reviewId, reviewTaskId, prefilters, additionnalColumnsVisibleAtStart, participant);
            this.IsLoading = false;
        }

        /// <summary>
        ///     Handle the change if the view should display products or not
        /// </summary>
        /// <param name="newValue">The new value</param>
        public void OnProductVisibilityChanged(bool newValue)
        {
            this.ViewModel.SetProductsVisibility(newValue);
        }

        /// <summary>
        ///     Tries to navigate to a corresponding item
        /// </summary>
        /// <param name="itemName">The name of the item to navigate to</param>
        /// <returns>A <see cref="Task" /></returns>
        public override async Task TryNavigateToItem(string itemName)
        {
            var existingItem = this.ViewModel.GetAvailablesRows().OfType<IBelongsToInterfaceView>()
                .FirstOrDefault(x => x.Id == itemName);

            if (existingItem != null)
            {
                var path = this.ComputeTreePath(existingItem);
                await this.ExpandAllRows(path);
                this.navigationId = existingItem.ThingId;
                await this.InvokeAsync(this.StateHasChanged);
            }
        }

        /// <summary>
        ///     Handle the on expand event
        /// </summary>
        /// <param name="row">The <see cref="IBelongsToInterfaceView" /></param>
        public void OnRowExpand(IBelongsToInterfaceView row)
        {
            if (row is ElementBaseRowViewModel elementBaseRow)
            {
                elementBaseRow.IsExpanded = !elementBaseRow.IsExpanded;
            }
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
        ///     Method invoked after each time the component has been rendered.
        /// </summary>
        /// <param name="firstRender">
        ///     Set to <c>true</c> if this is the first time
        ///     <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRender(System.Boolean)" /> has been invoked
        ///     on this component instance; otherwise <c>false</c>.
        /// </param>
        /// <remarks>
        ///     The <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRender(System.Boolean)" /> and
        ///     <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRenderAsync(System.Boolean)" /> lifecycle methods
        ///     are useful for performing interop, or interacting with values received from <c>@ref</c>.
        ///     Use the <paramref name="firstRender" /> parameter to ensure that initialization work is only performed
        ///     once.
        /// </remarks>
        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (!this.IsLoading && !this.isFullyInitialized && this.Grid != null && this.columnChooser != null)
            {
                this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.PortVisibilityState.CurrentState)
                    .Subscribe(_ => this.InvokeAsync(this.OnVisibilityStateChanged)));

                this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.ProductVisibilityState.CurrentState)
                    .Subscribe(_ => this.InvokeAsync(this.OnVisibilityStateChanged)));

                this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.FilterViewModel.IsFilterVisible)
                    .Where(x => !x)
                    .Subscribe(_ => this.InvokeAsync(this.OnRowFilteringClose)));

                this.columnChooser.InitializeProperties(this.Grid.ColumnsCollection);

                this.disposables.Add(this.WhenAnyValue(x => x.columnChooser.ColumnChooserVisible)
                    .Subscribe(_ => this.InvokeAsync(this.StateHasChanged)));

                this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.ShouldShowProducts)
                    .Subscribe(_ => this.InvokeAsync(this.StateHasChanged)));

                this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.IsViewSettingsVisible)
                    .Subscribe(_ => this.InvokeAsync(this.StateHasChanged)));

                this.HideColumnsAtStart();
                this.isFullyInitialized = true;
            }
        }

        /// <summary>
        /// Method invoked after each time the component has been rendered. Note that the component does
        /// not automatically re-render after the completion of any returned <see cref="T:System.Threading.Tasks.Task" />, because
        /// that would cause an infinite render loop.
        /// </summary>
        /// <param name="firstRender">
        /// Set to <c>true</c> if this is the first time <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRender(System.Boolean)" /> has been invoked
        /// on this component instance; otherwise <c>false</c>.
        /// </param>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> representing any asynchronous operation.</returns>
        /// <remarks>
        /// The <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRender(System.Boolean)" /> and <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRenderAsync(System.Boolean)" /> lifecycle methods
        /// are useful for performing interop, or interacting with values received from <c>@ref</c>.
        /// Use the <paramref name="firstRender" /> parameter to ensure that initialization work is only performed
        /// once.
        /// </remarks>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (this.navigationId != Guid.Empty)
            {
                await this.JsRuntime.InvokeVoidAsync("scrollToElement", $"row_{this.navigationId}", "center", "center");
                this.navigationId = Guid.Empty;
            }
        }

        /// <summary>
        ///     Supplies information to a row render
        /// </summary>
        /// <param name="row">The <see cref="RowRenderEventArgs{T}" /></param>
        protected void RowRender(RowRenderEventArgs<IBelongsToInterfaceView> row)
        {
            switch (row.Data)
            {
                case ProductRowViewModel product:
                    row.Expandable = this.ViewModel.HasChildren(product);
                    row.Attributes["class"] = product.IsVisible ? string.Empty : "invisible-row";
                    break;
                case PortRowViewModel port:
                    row.Expandable = this.ViewModel.HasChildren(port);
                    row.Attributes["class"] = port.IsVisible ? string.Empty : "invisible-row";
                    break;
                case InterfaceRowViewModel interfaceRow:
                    row.Expandable = false;
                    row.Attributes["class"] = interfaceRow.IsVisible ? string.Empty : "invisible-row";
                    break;
            }

            row.Attributes["id"] = $"row_{row.Data.ThingId}";
        }

        /// <summary>
        ///     Selects a new <see cref="IBelongsToInterfaceView" />
        /// </summary>
        /// <param name="row">The new selected <see cref="IBelongsToInterfaceView" /></param>
        protected void SelectRow(IBelongsToInterfaceView row)
        {
            this.ViewModel.SelectedElement = row;
        }

        /// <summary>
        ///     Loads children of a row
        /// </summary>
        /// <param name="parent">The <see cref="DataGridLoadChildDataEventArgs{T}" /></param>
        protected void LoadChildren(DataGridLoadChildDataEventArgs<IBelongsToInterfaceView> parent)
        {
            parent.Data = parent.Item switch
            {
                ProductRowViewModel product => this.ViewModel.LoadChildren(product),
                PortRowViewModel port => this.ViewModel.LoadChildren(port),
                _ => parent.Data
            };
        }

        /// <summary>
        ///     Expands all rows that are contained into the collection
        /// </summary>
        /// <param name="rows">The collection <see cref="IBelongsToInterfaceView" />"/></param>
        /// <returns>A <see cref="Task" /></returns>
        private async Task ExpandAllRows(IEnumerable<IBelongsToInterfaceView> rows)
        {
            foreach (var row in rows)
            {
                if (row is ElementBaseRowViewModel { IsExpanded: false })
                {
                    await this.Grid.ExpandRow(row);
                }
            }
        }

        /// <summary>
        ///     Computes the path inside the tree to access to the <see cref="IBelongsToInterfaceView" />
        /// </summary>
        /// <param name="item">The <see cref="IBelongsToInterfaceView" /></param>
        /// <returns>A collection of <see cref="IBelongsToInterfaceView" /> to have the path</returns>
        private  IEnumerable<IBelongsToInterfaceView> ComputeTreePath(IBelongsToInterfaceView item)
        {
            var path = new List<IBelongsToInterfaceView>();
            var currentItem = item;

            if (item is InterfaceRowViewModel && !this.ViewModel.ShouldShowProducts)
            {
                return path;
            }

            if (!this.ViewModel.ShouldShowProducts)
            {
                this.ViewModel.SetProductsVisibility(true);
            }

            while (currentItem is not null)
            {
                if (currentItem is ProductRowViewModel product)
                {
                    if (this.ViewModel.ProductVisibilityState.CurrentState == ConnectionToVisibilityState.NotConnected && this.ViewModel.HasChildren(product)
                        || this.ViewModel.ProductVisibilityState.CurrentState == ConnectionToVisibilityState.Connected && !this.ViewModel.HasChildren(product))
                    {
                        this.ViewModel.ProductVisibilityState.CurrentState = ConnectionToVisibilityState.All;
                    }

                    currentItem = null;
                }

                if (currentItem is PortRowViewModel port)
                {
                    if (this.ViewModel.PortVisibilityState.CurrentState == ConnectionToVisibilityState.NotConnected && this.ViewModel.HasChildren(port)
                        || this.ViewModel.PortVisibilityState.CurrentState == ConnectionToVisibilityState.Connected && !this.ViewModel.HasChildren(port))
                    {
                        this.ViewModel.PortVisibilityState.CurrentState = ConnectionToVisibilityState.All;
                    }

                    var productRow = this.ViewModel.GetAvailablesRows().OfType<ProductRowViewModel>()
                        .FirstOrDefault(x => (x.ThingId == port.ContainerId || x.ElementDefinitionId == port.ContainerId) &&
                                             this.ViewModel.LoadChildren(x).Any(p => p.ThingId == currentItem.ThingId));

                    if (productRow != null)
                    {
                        path.Add(productRow);
                        currentItem = productRow;
                    }
                    else
                    {
                        break;
                    }
                }

                if (currentItem is InterfaceRowViewModel interfaceRow)
                {
                    var portRow = interfaceRow.SourceRow as PortRowViewModel;
                    path.Add(portRow);
                    currentItem = portRow;
                }
            }

            path.Reverse();
            return path;
        }

        /// <summary>
        ///     Hides columns on start
        /// </summary>
        private void HideColumnsAtStart()
        {
            var sourceOwner = this.Grid.ColumnsCollection.FirstOrDefault(x => x.Property == nameof(PortRowViewModel.SourceOwner));
            var targetOwner = this.Grid.ColumnsCollection.FirstOrDefault(x => x.Property == nameof(PortRowViewModel.TargetOwner));

            if (sourceOwner != null)
            {
                this.columnChooser.OnChangeValue(sourceOwner);
            }

            if (targetOwner != null)
            {
                this.columnChooser.OnChangeValue(targetOwner);
            }
        }

        /// <summary>
        ///     Handle the change of visibility
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private Task OnVisibilityStateChanged()
        {
            this.ViewModel.OnVisibilityStateChanged();
            return this.InvokeAsync(this.StateHasChanged);
        }

        /// <summary>
        ///     Apply the filtering on rows
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private Task OnRowFilteringClose()
        {
            this.ViewModel.FilterRows(this.ViewModel.FilterViewModel.GetSelectedFilters());
            return this.InvokeAsync(this.StateHasChanged);
        }

        /// <summary>
        ///     Toggle the view type dropdown
        /// </summary>
        private void ToggleViewSettingsDropdown()
        {
            this.ViewModel.IsViewSettingsVisible = !this.ViewModel.IsViewSettingsVisible;
        }

        /// <summary>
        ///     Gets the css class for <see cref="ConnectionVisibilitySelector" />
        /// </summary>
        /// <returns>The css class</returns>
        private string GetSelectorsClass()
        {
            return this.ViewModel.ShouldShowProducts ? string.Empty : "invisible";
        }
    }
}
