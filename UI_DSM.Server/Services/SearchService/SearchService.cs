// --------------------------------------------------------------------------------------------------------
// <copyright file="SearchService.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Services.SearchService
{
    using System.Text;
    using System.Text.Json;

    using CometSearch.Common;
    using CometSearch.Common.SearchDto;

    using GP.SearchService.SDK.Definitions;

    using Microsoft.AspNetCore.WebUtilities;

    using UI_DSM.Client.Extensions;

    using BinaryRelationship = CDP4Common.EngineeringModelData.BinaryRelationship;
    using ElementDefinition = CDP4Common.EngineeringModelData.ElementDefinition;
    using Requirement = CDP4Common.EngineeringModelData.Requirement;
    using Thing = CDP4Common.CommonData.Thing;

    /// <summary>
    ///     Service that enable the indexing and searching of UI-DSM data also than 10-25 data
    /// </summary>
    public class SearchService : ISearchService
    {
        /// <summary>
        ///     The main route of any query
        /// </summary>
        private const string MainRoute = "GP.SearchService";

        /// <summary>
        ///     The <see cref="HttpClient" /> to reach the search engine
        /// </summary>
        private readonly HttpClient httpClient;

        /// <summary>
        ///     Initialize a new <see cref="SearchService" />
        /// </summary>
        /// <param name="client">The <see cref="HttpClient"/></param>
        public SearchService(HttpClient client)
        {
            this.httpClient = client;
        }

        /// <summary>
        ///     Search after a keyword
        /// </summary>
        /// <param name="searchKeyword">The keyword</param>
        /// <returns>A <see cref="Task" /> with the result of the search</returns>
        public async Task<Stream> SearchAfter(string searchKeyword)
        {
            const string baseUri = $"{MainRoute}/Search";
            var uri = QueryHelpers.AddQueryString(baseUri, "query", searchKeyword);
            var request = await this.httpClient.GetAsync(uri);
            return await request.Content.ReadAsStreamAsync();
        }

        /// <summary>
        ///     Indexes an <see cref="ISearchDto" />
        /// </summary>
        /// <param name="dto">The <see cref="ISearchDto" /> to index</param>
        /// <returns>A <see cref="Task" /> with the result of the index query</returns>
        /// <typeparam name="TSearchDto">A <see cref="ISearchDto" /></typeparam>
        public async Task<bool> IndexData<TSearchDto>(TSearchDto dto) where TSearchDto : ISearchDto
        {
            var baseUri = $"{MainRoute}/Index/{dto.Type}";
            var serializedJson = JsonSerializer.Serialize(dto);
            var content = new StringContent(serializedJson, Encoding.UTF8, "application/json");
            var request = await this.httpClient.PutAsync(baseUri, content);
            return request.IsSuccessStatusCode;
        }

        /// <summary>
        ///     Indexes a collection of <see cref="ISearchDto" />
        /// </summary>
        /// <param name="dtos">The collection of <see cref="ISearchDto" /> to index</param>
        /// <returns>A <see cref="Task" /> with the result of the index query</returns>
        /// <typeparam name="TSearchDto">A <see cref="ISearchDto" /></typeparam>
        public async Task<bool> IndexData<TSearchDto>(IEnumerable<TSearchDto> dtos) where TSearchDto : ISearchDto
        {
            var dtosList = dtos.ToList();

            if (!dtosList.Any())
            {
                return false;
            }

            var baseUri = $"{MainRoute}/Index/{dtosList.First().Type}s";
            var serializedJson = JsonSerializer.Serialize(dtosList);
            var content = new StringContent(serializedJson, Encoding.UTF8, "application/json");
            var request = await this.httpClient.PutAsync(baseUri, content);
            return request.IsSuccessStatusCode;
        }

        /// <summary>
        ///     Deletes an indexed data
        /// </summary>
        /// <param name="dto">The <see cref="ISearchDto" /></param>
        /// <returns>A <see cref="Task" /> with the result of the delete query</returns>
        public async Task<bool> DeleteIndexedData(ISearchDto dto)
        {
            var baseUri = $"{MainRoute}/Index/{dto.Type}/{dto.Id}";
            var request = await this.httpClient.DeleteAsync(baseUri);
            return request.IsSuccessStatusCode;
        }

        /// <summary>
        ///     Indexes an <see cref="CDP4Common.EngineeringModelData.Iteration" />
        /// </summary>
        /// <param name="things">A collection of <see cref="CDP4Common.CommonData.Thing" /> that is contained into an <see cref="CDP4Common.EngineeringModelData.Iteration" /></param>
        /// <returns>A <see cref="Task" /></returns>
        public async Task IndexIteration(IEnumerable<Thing> things)
        {
            things = things.ToList();
            var requirements = Converter.Convert(things.OfType<Requirement>()).OfType<CometSearch.Common.SearchDto.Requirement>();
            var elementDefinitions = Converter.Convert(things.OfType<ElementDefinition>()).OfType<CometSearch.Common.SearchDto.ElementDefinition>();
            var relationShips = Converter.Convert(things.OfType<BinaryRelationship>().Where(x => x.GetAvailableViews().Any())).OfType<CometSearch.Common.SearchDto.BinaryRelationship>();
            var hyperlinks = things.OfType<Requirement>().Where(x => x.HasReviewExternalContent()).SelectMany(x => x.HyperLink).ToList();
            hyperlinks.AddRange(things.OfType<ElementDefinition>().Where(x => x.HasReviewExternalContent()).SelectMany(x => x.HyperLink));
            var hyperlinksDto = Converter.Convert(hyperlinks).OfType<CometSearch.Common.SearchDto.HyperLink>();

            await this.IndexData(requirements);
            await this.IndexData(elementDefinitions);
            await this.IndexData(relationShips);
            await this.IndexData(hyperlinksDto);

            await this.IndexData(new Iteration()
            {
                Id = things.OfType<CDP4Common.EngineeringModelData.Iteration>().First().Iid
            });
        }
    }
}
