// --------------------------------------------------------------------------------------------------------
// <copyright file="SearchPageViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Pages.NormalUser.SearchPage
{
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.WebUtilities;

    using ReactiveUI;

    using UI_DSM.Client.Services.SearchService;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.Enumerator;

    /// <summary>
    ///     View Model for <see cref="Client.Pages.NormalUser.SearchPage.SearchPage" /> Page
    /// </summary>
    public class SearchPageViewModel : ReactiveObject, ISearchPageViewModel
    {
        /// <summary>
        ///     The <see cref="ISearchService" />
        /// </summary>
        private readonly ISearchService searchService;

        /// <summary>
        ///     Backing field for <see cref="IsOnViewSelectionMode" />
        /// </summary>
        private bool isOnViewSelectionMode;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchPageViewModel" /> class.
        /// </summary>
        /// <param name="searchService">The <see cref="ISearchService" /></param>
        /// <param name="navigationManager">The <see cref="Microsoft.AspNetCore.Components.NavigationManager" /></param>
        public SearchPageViewModel(ISearchService searchService, NavigationManager navigationManager)
        {
            this.searchService = searchService;
            this.NavigationManager = navigationManager;
        }

        /// <summary>
        ///     The <see cref="Microsoft.AspNetCore.Components.NavigationManager" />
        /// </summary>
        public NavigationManager NavigationManager { get; set; }

        /// <summary>
        ///     The <see cref="SearchResultDto" /> that has been selected
        /// </summary>
        public SearchResultDto CurrentSearchResult { get; private set; }

        /// <summary>
        ///     Value indicating if the current mode is the view selection mode
        /// </summary>
        public bool IsOnViewSelectionMode
        {
            get => this.isOnViewSelectionMode;
            set => this.RaiseAndSetIfChanged(ref this.isOnViewSelectionMode, value);
        }

        /// <summary>
        ///     Proceed to a search query
        /// </summary>
        /// <param name="keyword">The keyword to search</param>
        /// <returns>A <see cref="Task" /> with the collection of <see cref="SearchResultDto" /></returns>
        public Task<IEnumerable<SearchResultDto>> SearchAfter(string keyword)
        {
            return this.searchService.SearchAfter(keyword);
        }

        /// <summary>
        ///     Navigates to the link to see the current item
        /// </summary>
        /// <param name="searchResult">The <see cref="SearchResultDto" /></param>
        public void NavigateTo(SearchResultDto searchResult)
        {
            if (searchResult.AvailableViews == null)
            {
                this.NavigationManager.NavigateTo(searchResult.BaseUrl);
            }
            else
            {
                this.CurrentSearchResult = searchResult;

                if (this.CurrentSearchResult.AvailableViews.Count() == 1)
                {
                    this.NavigateTo(this.CurrentSearchResult.AvailableViews.First());
                }
                else
                {
                    this.IsOnViewSelectionMode = true;
                }
            }
        }

        /// <summary>
        ///     Navigates to the link to see the current item with the selected <see cref="View"/>
        /// </summary>
        /// <param name="selectedView">The selected <see cref="View" /></param>
        public void NavigateTo(View selectedView)
        {
            if (this.CurrentSearchResult == null)
            {
                return;
            }

            var urlParameters = new Dictionary<string, string>
            {
                ["view"] = selectedView.ToString(),
                ["id"] = this.CurrentSearchResult.ItemId.ToString()
            };

            var url = QueryHelpers.AddQueryString(this.CurrentSearchResult.BaseUrl, urlParameters);
            this.NavigationManager.NavigateTo(url);
            this.IsOnViewSelectionMode = false;
        }
    }
}
