// --------------------------------------------------------------------------------------------------------
// <copyright file="SearchPage.razor.cs" company="RHEA System S.A.">
//  Copyright (c) 2023 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw, Nabil Abbar
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Client.Pages.NormalUser.SearchPage
{
    using Microsoft.AspNetCore.Components;

    using ReactiveUI;

    using UI_DSM.Client.ViewModels.Pages.NormalUser.SearchPage;
    using UI_DSM.Shared.DTO.Common;

    /// <summary>
    ///     This page enables the user the see results of a search query
    /// </summary>
    public partial class SearchPage : IDisposable
    {
        /// <summary>
        ///     A collection of <see cref="IDisposable" />
        /// </summary>
        private readonly List<IDisposable> disposables = new();

        /// <summary>
        ///     Value indicating if the page is loading
        /// </summary>
        private bool isLoading;

        /// <summary>
        ///     The collection of <see cref="SearchResultDto" />
        /// </summary>
        private IEnumerable<SearchResultDto> searchResults;

        /// <summary>
        ///     The keyword to search after
        /// </summary>
        [Parameter]
        public string Keyword { get; set; }

        /// <summary>
        ///     The <see cref="ISearchPageViewModel" />
        /// </summary>
        [Inject]
        public ISearchPageViewModel ViewModel { get; set; }

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

            this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.IsOnViewSelectionMode)
                .Subscribe(_ => this.InvokeAsync(this.StateHasChanged)));

            this.isLoading = true;
        }

        /// <summary>
        ///     Method invoked when the component has received parameters from its parent in
        ///     the render tree, and the incoming values have been assigned to properties.
        /// </summary>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> representing any asynchronous operation.</returns>
        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            this.isLoading = true;
            this.searchResults = await this.ViewModel.SearchAfter(this.Keyword);
            this.isLoading = false;
            await this.InvokeAsync(this.StateHasChanged);
        }
    }
}
