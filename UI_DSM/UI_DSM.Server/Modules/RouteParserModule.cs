// --------------------------------------------------------------------------------------------------------
// <copyright file="RouteParserModule.cs" company="RHEA System S.A.">
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
    using System.Diagnostics.CodeAnalysis;

    using Carter.Response;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using UI_DSM.Server.Services.RouteParserService;
    using UI_DSM.Shared.DTO.Common;

    /// <summary>
    ///     This <see cref="ModuleBase" /> provides parsing of a URL to get informations related to the current URL (used for the breadcrumb)
    /// </summary>
    [Microsoft.AspNetCore.Components.Route("api/RouteParser")]
    public class RouteParserModule : ModuleBase
    {
        /// <summary>
        ///     Adds routes to the <see cref="IEndpointRouteBuilder" />
        /// </summary>
        /// <param name="app">The <see cref="IEndpointRouteBuilder" /></param>
        [ExcludeFromCodeCoverage]
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet(this.MainRoute, this.ParseRoute)
                .Produces<IEnumerable<ParsedUrlDto>>()
                .WithTags("RouteParser")
                .WithName("RouteParser/Parse");
        }

        /// <summary>
        ///     Parses an url
        /// </summary>
        /// <param name="routeParser">The <see cref="IRouteParserService" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="url">The url to parse</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public async Task ParseRoute(IRouteParserService routeParser, HttpContext context, [FromQuery] string url)
        {
            var parsedResult = await routeParser.ParseUrl(url);
            await context.Response.Negotiate(parsedResult);
        }
    }
}
