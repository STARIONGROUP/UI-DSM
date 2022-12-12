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

    using Radzen;
    using Radzen.Blazor;

    using ReactiveUI;

    using UI_DSM.Client.Components.App.ConnectionVisibilitySelector;
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
        ///     Reference to the <see cref="ConnectionVisibilitySelector" /> for ports
        /// </summary>
        public ConnectionVisibilitySelector ConnectionVisibilitySelectorPort { get; set; }

        /// <summary>
        ///     Reference to the <see cref="ConnectionVisibilitySelector" /> for products
        /// </summary>
        public ConnectionVisibilitySelector ConnectionVisibilitySelectorProduct { get; set; }

        /// <summary>
        ///     Reference to the <see cref="RadzenDataGrid{TItem}" />
        /// </summary>
        public RadzenDataGrid<IBelongsToInterfaceView> Grid { get; set; }

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
            await Task.CompletedTask;
            return true;
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
            await base.InitializeViewModel(things, projectId, reviewId, prefilters, additionnalColumnsVisibleAtStart);
            this.IsLoading = false;

            this.ViewModel.PortVisibilityState = this.ConnectionVisibilitySelectorPort.ViewModel;
            this.ViewModel.ProductVisibilityState = this.ConnectionVisibilitySelectorProduct.ViewModel;

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

            this.HideColumnsAtStart();
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
        ///     Handle the change if the view should display products or not
        /// </summary>
        /// <param name="newValue">The new value</param>
        private void OnProductVisibilityChanged(bool newValue)
        {
            this.ViewModel.SetProductsVisibility(newValue);
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
