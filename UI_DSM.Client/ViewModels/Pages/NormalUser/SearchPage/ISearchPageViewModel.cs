// --------------------------------------------------------------------------------------------------------
// <copyright file="ISearchPageViewModel.cs" company="RHEA System S.A.">
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

    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.Enumerator;

    /// <summary>
    ///     Interface definition for <see cref="SearchPageViewModel" />
    /// </summary>
    public interface ISearchPageViewModel
    {
        /// <summary>
        ///     The <see cref="Microsoft.AspNetCore.Components.NavigationManager" />
        /// </summary>
        NavigationManager NavigationManager { get; set; }

        /// <summary>
        ///     The <see cref="SearchResultDto" /> that has been selected
        /// </summary>
        SearchResultDto CurrentSearchResult { get; }

        /// <summary>
        ///     Value indicating if the current mode is the view selection mode
        /// </summary>
        bool IsOnViewSelectionMode { get; set; }

        /// <summary>
        ///     Proceed to a search query
        /// </summary>
        /// <param name="keyword">The keyword to search</param>
        /// <returns>A <see cref="Task" /> with the collection of <see cref="SearchResultDto" /></returns>
        Task<IEnumerable<SearchResultDto>> SearchAfter(string keyword);

        /// <summary>
        ///     Navigates to the link to see the current item
        /// </summary>
        /// <param name="searchResult">The <see cref="SearchResultDto" /></param>
        void NavigateTo(SearchResultDto searchResult);

        /// <summary>
        ///     Navigates to the link to see the current item with the selected <see cref="View"/>
        /// </summary>
        /// <param name="selectedView">The selected <see cref="View" /></param>
        void NavigateTo(View selectedView);
    }
}
