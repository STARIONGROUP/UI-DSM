// --------------------------------------------------------------------------------------------------------
// <copyright file="SearchModule.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Modules
{
    using Carter.Response;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Server.Services.ResolverService;
    using UI_DSM.Server.Services.SearchService;
    using UI_DSM.Shared.DTO.Common;

    /// <summary>
    ///     Module that enables to index and search
    /// </summary>
    [Route("api/Search")]
    public class SearchModule : ModuleBase
    {
        /// <summary>
        ///     Adds routes to the <see cref="IEndpointRouteBuilder" />
        /// </summary>
        /// <param name="app">The <see cref="IEndpointRouteBuilder" /></param>
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet(this.MainRoute, this.Search)
                .Produces<IEnumerable<SearchResultDto>>()
                .WithTags("Search")
                .WithName("Search/Search");
        }

        /// <summary>
        ///     Searches after a keyword and retrieves the result of the search
        /// </summary>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="keyword">The keyword to search after</param>
        /// <param name="searchService">The <see cref="ISearchService" /></param>
        /// <param name="jsonService">The <see cref="IJsonService" /></param>
        /// <param name="resolverService">The <see cref="IResolverService" /></param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public async Task Search(HttpContext context, string keyword, ISearchService searchService, IJsonService jsonService, IResolverService resolverService)
        {
            var response = await searchService.SearchAfter(keyword);

            if (response.Length > 0)
            {
                var searchDtos = jsonService.Deserialize<IEnumerable<CommonBaseSearchDto>>(response);
                var resolvedDtos = await resolverService.ResolveSearchResult(searchDtos.ToList(), context.User!.Identity!.Name);
                await context.Response.Negotiate(resolvedDtos);
            }
            else
            {
                await context.Response.Negotiate(new List<SearchResultDto>());
            }
        }
    }
}
