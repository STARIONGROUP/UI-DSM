// --------------------------------------------------------------------------------------------------------
// <copyright file="RouteParserService.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Services.RouteParserService
{
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.WebUtilities;

    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Shared.DTO.Common;

    /// <summary>
    ///     This <see cref="ServiceBase" /> provides capabilities to parse an url, for breadcrumb use
    /// </summary>
    [Route("RouteParser")]
    public class RouteParserService : ServiceBase, IRouteParserService
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RouteParserService" /> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="ServiceBase.HttpClient" /></param>
        /// <param name="jsonService">The <see cref="IJsonService" /></param>
        public RouteParserService(HttpClient httpClient, IJsonService jsonService) : base(httpClient, jsonService)
        {
        }

        /// <summary>
        ///     Parses the current url
        /// </summary>
        /// <param name="currentUrl">The url, without the base address</param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="ParsedUrlDto" /></returns>
        public async Task<List<ParsedUrlDto>> ParseUrl(string currentUrl)
        {
            try
            {
                var url = QueryHelpers.AddQueryString(this.MainRoute, "url", currentUrl);
                var response = await this.HttpClient.GetAsync(url);

                return !response.IsSuccessStatusCode
                    ? new List<ParsedUrlDto>()
                    : this.jsonService.Deserialize<IEnumerable<ParsedUrlDto>>(await response.Content.ReadAsStreamAsync()).ToList();
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }
    }
}
