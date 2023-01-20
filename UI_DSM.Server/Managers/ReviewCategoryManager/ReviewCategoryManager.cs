// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewCategoryManager.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Managers.ReviewCategoryManager
{
    using UI_DSM.Server.Context;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This manager handles operation to the Database for <see cref="ReviewCategory" />s
    /// </summary>
    public class ReviewCategoryManager : EntityManager<ReviewCategory>, IReviewCategoryManager
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ReviewCategoryManager" /> class
        /// </summary>
        /// <param name="context">The <see cref="DatabaseContext" /></param>
        public ReviewCategoryManager(DatabaseContext context) : base(context)
        {
        }

        /// <summary>
        ///     Resolve all properties for the <see cref="ReviewCategory" />
        /// </summary>
        /// <param name="entity">The <see cref="ReviewCategory" /></param>
        /// <param name="dto">The <see cref="EntityDto" /></param>
        /// <returns>A <see cref="Task" /></returns>
        public override async Task ResolveProperties(ReviewCategory entity, EntityDto dto)
        {
            if (dto is not ReviewCategoryDto reviewCategoryDto)
            {
                return;
            }

            entity.ResolveProperties(reviewCategoryDto, null);
            await Task.CompletedTask;
        }

        /// <summary>
        ///     Gets the <see cref="SearchResultDto"/> based on a <see cref="Guid"/>
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="ReviewCategory" /></param>
        /// <returns>A URL</returns>
        public override async Task<SearchResultDto> GetSearchResult(Guid entityId)
        {
            var reviewCategory = await this.FindEntity(entityId);

            if (reviewCategory == null)
            {
                return null;
            }

            var route=  $"Administration/ReviewCategory/{reviewCategory.Id}";
            
            return new SearchResultDto()
            {
                BaseUrl = route,
                ObjectKind = nameof(ReviewCategory),
                DisplayText = reviewCategory.ReviewCategoryName
            };
        }

        /// <summary>
        ///     Gets all <see cref="Entity" /> that needs to be unindexed when the current <see cref="Entity" /> is delete
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the entity</param>
        /// <returns>A collection of <see cref="Entity" /></returns>
        public override async Task<IEnumerable<Entity>> GetExtraEntitiesToUnindex(Guid entityId)
        {
            await Task.CompletedTask;
            return Enumerable.Empty<Entity>();
        }
    }
}
