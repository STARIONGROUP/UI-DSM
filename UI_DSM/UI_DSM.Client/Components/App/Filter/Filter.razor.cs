// --------------------------------------------------------------------------------------------------------
// <copyright file="Filter.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.App.Filter
{
    using DevExpress.Blazor;

    using Microsoft.AspNetCore.Components;

    using ReactiveUI;

    using UI_DSM.Client.Model;
    using UI_DSM.Client.ViewModels.App.Filter;

    /// <summary>
    ///     Component that allow to select filter criteria
    /// </summary>
    public partial class Filter : IDisposable
    {
        /// <summary>
        ///     A collection of <see cref="IDisposable" />
        /// </summary>
        private readonly List<IDisposable> disposables = new();

        /// <summary>
        ///     The <see cref="IFilterViewModel" />
        /// </summary>
        [Inject]
        public IFilterViewModel ViewModel { get; set; }

        /// <summary>
        ///     The id of the filter
        /// </summary>
        [Parameter]
        public string Id { get; set; }

        /// <summary>
        ///     The content of the button
        /// </summary>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        ///     The target position css selector
        /// </summary>
        public string TargetPosition => $"#{this.Id}";

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.disposables.ForEach(x => x.Dispose());
        }

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        /// </summary>
        protected override void OnInitialized()
        {
            base.OnInitialized();

            this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.IsFilterVisible)
                .Subscribe(_ => this.InvokeAsync(this.StateHasChanged)));

            this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.SelectedFilterModel)
                .Subscribe(_ => this.InvokeAsync(this.StateHasChanged)));
        }

        /// <summary>
        ///     Opens or closes the filter dropdown
        /// </summary>
        public void OpenCloseFilter()
        {
            this.ViewModel.IsFilterVisible = !this.ViewModel.IsFilterVisible;
        }

        /// <summary>
        ///     Selects or deselects all <see cref="FilterRow" />
        /// </summary>
        /// <param name="isSelected">The state to apply</param>
        /// <returns>A <see cref="Task" /></returns>
        public async Task SelectDeselectAll(bool isSelected)
        {
            this.ViewModel.SelectDeselectAll(isSelected);
            await this.InvokeAsync(this.StateHasChanged);
        }
    }
}
