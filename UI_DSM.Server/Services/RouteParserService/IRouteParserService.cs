// --------------------------------------------------------------------------------------------------------
// <copyright file="IRouteParserService.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Services.RouteParserService
{
    using UI_DSM.Shared.DTO.Common;

    /// <summary>
    ///     Interface definition for <see cref="RouteParserService" />
    /// </summary>
    public interface IRouteParserService
    {
        /// <summary>
        ///     Parses the url to generate a collection of <see cref="ParsedUrlDto" />
        /// </summary>
        /// <param name="url">The <see cref="url" /> to parse</param>
        /// <returns>A <see cref="Task" /> with the collection of <see cref="ParsedUrlDto" /></returns>
        Task<List<ParsedUrlDto>> ParseUrl(string url);
    }
}
