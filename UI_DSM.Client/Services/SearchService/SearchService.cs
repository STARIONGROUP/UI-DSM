// --------------------------------------------------------------------------------------------------------
// <copyright file="SearchService.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Services.SearchService
{
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.WebUtilities;

    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Shared.DTO.Common;

    /// <summary>
    ///     This service enables search queries
    /// </summary>
    [Route("Search")]
    public class SearchService : ServiceBase, ISearchService
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ServiceBase" /> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="ServiceBase.HttpClient" /></param>
        /// <param name="jsonService">The <see cref="IJsonService" /></param>
        public SearchService(HttpClient httpClient, IJsonService jsonService) : base(httpClient, jsonService)
        {
        }

        /// <summary>
        ///     Gets search result
        /// </summary>
        /// <param name="searchKey">The keyword to search after</param>
        /// <returns>A <see cref="Task" /> with the collection of <see cref="SearchResultDto" /> response</returns>
        public async Task<IEnumerable<SearchResultDto>> SearchAfter(string searchKey)
        {
            try
            {
                var uri = QueryHelpers.AddQueryString(this.MainRoute, "keyword", searchKey);
                var response = await this.HttpClient.GetAsync(uri);

                return !response.IsSuccessStatusCode
                    ? Enumerable.Empty<SearchResultDto>()
                    : this.jsonService.Deserialize<IEnumerable<SearchResultDto>>(await response.Content.ReadAsStreamAsync());
            }
            catch
            {
                return Enumerable.Empty<SearchResultDto>();
            }
        }
    }
}
