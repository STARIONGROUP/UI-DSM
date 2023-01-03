// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewCategoryModule.cs" company="RHEA System S.A.">
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
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using UI_DSM.Server.Managers;
    using UI_DSM.Server.Services.SearchService;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     The <see cref="ReviewCategoryModule" /> is a <see cref="EntityModule{TEntity,TEntityDto}" /> to manage
    ///     <see cref="ReviewCategory" />
    /// </summary>
    [Microsoft.AspNetCore.Components.Route("api/ReviewCategory")]
    public class ReviewCategoryModule : EntityModule<ReviewCategory, ReviewCategoryDto>
    {
        /// <summary>
        ///     Gets a collection of all <see cref="Entity" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public override Task GetEntities(IEntityManager<ReviewCategory> manager, HttpContext context, [FromQuery] int deepLevel = 0)
        {
            return base.GetEntities(manager, context, deepLevel);
        }

        /// <summary>
        ///     Get a <see cref="ReviewCategoryDto" /> based on its <see cref="Guid" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public override Task GetEntity(IEntityManager<ReviewCategory> manager, Guid entityId, HttpContext context, [FromQuery] int deepLevel = 0)
        {
            return base.GetEntity(manager, entityId, context, deepLevel);
        }

        /// <summary>
        ///     Tries to create a new <see cref="ReviewCategory" /> based on its <see cref="ReviewCategoryDto" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="dto">The <see cref="ReviewCategoryDto" /></param>
        /// <param name="searchService">The <see cref="ISearchService"/></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize(Roles = "Administrator")]
        public override Task CreateEntity(IEntityManager<ReviewCategory> manager, ReviewCategoryDto dto, ISearchService searchService, HttpContext context, [FromQuery] int deepLevel = 0)
        {
            return base.CreateEntity(manager, dto, searchService, context, deepLevel);
        }

        /// <summary>
        ///     Tries to delete an <see cref="ReviewCategory" /> defined by the given <see cref="Guid" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="ReviewCategory" /> to delete</param>
        /// <param name="searchService">The <see cref="ISearchService"/></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="RequestResponseDto" /> as result</returns>
        [Authorize(Roles = "Administrator")]
        public override Task<RequestResponseDto> DeleteEntity(IEntityManager<ReviewCategory> manager, Guid entityId, ISearchService searchService, HttpContext context)
        {
            return base.DeleteEntity(manager, entityId, searchService, context);
        }

        /// <summary>
        ///     Tries to update an existing <see cref="ReviewCategory" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="ReviewCategory" /></param>
        /// <param name="dto">The <see cref="ReviewCategoryDto" /></param>
        /// <param name="searchService">The <see cref="ISearchService"/></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize(Roles = "Administrator")]
        public override Task UpdateEntity(IEntityManager<ReviewCategory> manager, Guid entityId, ReviewCategoryDto dto, ISearchService searchService,
            HttpContext context, [FromQuery] int deepLevel = 0)
        {
            return base.UpdateEntity(manager, entityId, dto, searchService, context, deepLevel);
        }
    }
}
