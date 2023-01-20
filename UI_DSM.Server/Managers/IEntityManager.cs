// --------------------------------------------------------------------------------------------------------
// <copyright file="IEntityManager.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Server.Managers
{
    using UI_DSM.Server.Context;
    using UI_DSM.Server.Types;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface definition for all managers working with <see cref="TEntity" />
    /// </summary>
    /// <typeparam name="TEntity">An <see cref="Entity" /></typeparam>
    public interface IEntityManager<TEntity> where TEntity : Entity
    {
        /// <summary>
        ///     Gets a collection of all <see cref="Entity" />s and associated <see cref="Entity" />
        /// </summary>
        /// <param name="deepLevel">The level of deepnest that we need to retrieve associated <see cref="Entity" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /> as result</returns>
        Task<IEnumerable<Entity>> GetEntities(int deepLevel = 0);

        /// <summary>
        ///     Tries to get a <see cref="Entity" /> based on its <see cref="Guid" /> and its associated <see cref="Entity" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <param name="deepLevel">The level of deepnest that we need to retrieve associated <see cref="Entity" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /> if found</returns>
        Task<IEnumerable<Entity>> GetEntity(Guid entityId, int deepLevel = 0);

        /// <summary>
        ///     Tries to get a <see cref="TEntity" /> based on its <see cref="Guid" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="TEntity" /> if found</returns>
        Task<TEntity> FindEntity(Guid entityId);

        /// <summary>
        ///     Tries to get all <see cref="TEntity" /> based on their <see cref="Guid" />
        /// </summary>
        /// <param name="entitiesId">A collection of <see cref="Guid" /></param>
        /// <returns>A collection of <see cref="TEntity" /></returns>
        Task<IEnumerable<TEntity>> FindEntities(IEnumerable<Guid> entitiesId);

        /// <summary>
        ///     Creates a new <see cref="TEntity" /> and store it into the <see cref="DatabaseContext" />
        /// </summary>
        /// <param name="entity">The <see cref="TEntity" /> to create</param>
        /// <returns>A <see cref="Task" /> with the result of the creation</returns>
        Task<EntityOperationResult<TEntity>> CreateEntity(TEntity entity);

        /// <summary>
        ///     Updates a <see cref="TEntity" />
        /// </summary>
        /// <param name="entity">The <see cref="TEntity" /> to update</param>
        /// <returns>A <see cref="Task" /> with the result of the update</returns>
        Task<EntityOperationResult<TEntity>> UpdateEntity(TEntity entity);

        /// <summary>
        ///     Deletes a <see cref="TEntity" />
        /// </summary>
        /// <param name="entity">The <see cref="TEntity" /> to delete</param>
        /// <returns>A <see cref="Task" /> with the result of the deletion</returns>
        Task<EntityOperationResult<TEntity>> DeleteEntity(TEntity entity);

        /// <summary>
        ///     Resolve all properties for the <see cref="TEntity" />
        /// </summary>
        /// <param name="entity">The <see cref="TEntity" /></param>
        /// <param name="dto">The <see cref="EntityDto" /></param>
        /// <returns>A <see cref="Task" /></returns>
        Task ResolveProperties(TEntity entity, EntityDto dto);

        /// <summary>
        ///     Gets the <see cref="SearchResultDto" /> based on a <see cref="Guid" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="TEntity" /></param>
        /// <returns>A URL</returns>
        Task<SearchResultDto> GetSearchResult(Guid entityId);

        /// <summary>
        ///     Gets all <see cref="Entity" /> that needs to be unindexed when the current <see cref="Entity" /> is delete
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the entity</param>
        /// <returns>A collection of <see cref="Entity" /></returns>
        Task<IEnumerable<Entity>> GetExtraEntitiesToUnindex(Guid entityId);
    }
}
