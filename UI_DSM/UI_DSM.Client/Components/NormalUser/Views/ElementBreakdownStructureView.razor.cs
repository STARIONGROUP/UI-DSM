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

    using CDP4Common.EngineeringModelData;

    using Radzen;
    using Radzen.Blazor;

    using ReactiveUI;

    using UI_DSM.Client.Components.App.ColumnChooser;
    using UI_DSM.Client.ViewModels.App.ColumnChooser;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;

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
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.disposables.ForEach(x => x.Dispose());
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

            if (!this.hasAlreadyExpandOnce && this.ViewModel.TopElement.Any())
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
                    .Subscribe(_ => this.InvokeAsync(this.OnRowFilteringClose)));

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
        ///     Apply the filtering on rows
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private Task OnRowFilteringClose()
        {
            this.ViewModel.FilterRows(this.ViewModel.FilterViewModel.GetSelectedFilters());
            return this.HasChanged();
        }
    }
}
