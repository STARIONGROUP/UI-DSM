// --------------------------------------------------------------------------------------------------------
// <copyright file="ISearchService.cs" company="RHEA System S.A.">
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
    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;

    using GP.SearchService.SDK.Definitions;

    /// <summary>
    ///     Interface definition for <see cref="SearchService" />
    /// </summary>
    public interface ISearchService
    {
        /// <summary>
        ///     Search after a keyword
        /// </summary>
        /// <param name="searchKeyword">The keyword</param>
        /// <returns>A <see cref="Task" /> with the result of the search</returns>
        Task<Stream> SearchAfter(string searchKeyword);

        /// <summary>
        ///     Indexes an <see cref="ISearchDto" />
        /// </summary>
        /// <param name="dto">The <see cref="ISearchDto" /> to index</param>
        /// <returns>A <see cref="Task" /> with the result of the index query</returns>
        /// <typeparam name="TSearchDto">A <see cref="ISearchDto" /></typeparam>
        Task<bool> IndexData<TSearchDto>(TSearchDto dto) where TSearchDto : ISearchDto;

        /// <summary>
        ///     Indexes a collection of <see cref="ISearchDto" />
        /// </summary>
        /// <param name="dtos">The collection of <see cref="ISearchDto" /> to index</param>
        /// <returns>A <see cref="Task" /> with the result of the index query</returns>
        /// <typeparam name="TSearchDto">A <see cref="ISearchDto" /></typeparam>
        Task<bool> IndexData<TSearchDto>(IEnumerable<TSearchDto> dtos) where TSearchDto : ISearchDto;

        /// <summary>
        ///     Deletes an indexed data
        /// </summary>
        /// <param name="dto">The <see cref="ISearchDto" /></param>
        /// <returns>A <see cref="Task" /> with the result of the delete query</returns>
        Task<bool> DeleteIndexedData(ISearchDto dto);

        /// <summary>
        ///     Indexes an <see cref="Iteration" />
        /// </summary>
        /// <param name="things">A collection of <see cref="Thing" /> that is contained into an <see cref="Iteration" /></param>
        /// <returns>A <see cref="Task" /></returns>
        Task IndexIteration(IEnumerable<Thing> things);
    }
}
