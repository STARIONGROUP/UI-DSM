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
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface definition for all managers working with <see cref="TEntity" />
    /// </summary>
    /// <typeparam name="TEntity">An <see cref="Entity" /></typeparam>
    public interface IEntityManager<TEntity> where TEntity : Entity
    {
        /// <summary>
        ///     Gets a collection of all <see cref="TEntity" />s
        /// </summary>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="TEntity" /> as result</returns>
        Task<IEnumerable<TEntity>> GetEntities();

        /// <summary>
        ///     Tries to get a <see cref="TEntity" /> based on its <see cref="Guid" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="TEntity" /> if found</returns>
        Task<TEntity> GetEntity(Guid entityId);

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
    }
}
