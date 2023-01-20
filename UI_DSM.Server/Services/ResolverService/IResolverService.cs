// --------------------------------------------------------------------------------------------------------
// <copyright file="IResolverService.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Services.ResolverService
{
    using UI_DSM.Shared.DTO.Common;

    /// <summary>
    ///     Interface definition for <see cref="ResolverService" />
    /// </summary>
    public interface IResolverService
    {
        /// <summary>
        ///     Resolves the collection of <see cref="CommonBaseSearchDto" /> into a collection of <see cref="SearchResultDto" />
        /// </summary>
        /// <param name="dtos">The collection of <see cref="SearchResultDto" /></param>
        /// <param name="userName">The name of the current user</param>
        /// <returns>A collection of <see cref="SearchResultDto" /></returns>
        Task<List<SearchResultDto>> ResolveSearchResult(List<CommonBaseSearchDto> dtos, string userName);
    }
}
