// --------------------------------------------------------------------------------------------------------
// <copyright file="IContainedEntityManager.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Server.Managers
{
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     The <see cref="IContainedEntityManager{TEntity}" /> is a <see cref="IEntityManager{TEntity}" /> for
    ///     <see cref="Entity" /> that have a container
    /// </summary>
    /// <typeparam name="TEntity">A <see cref="Entity" /></typeparam>
    public interface IContainedEntityManager<TEntity> : IEntityManager<TEntity> where TEntity : Entity
    {
        /// <summary>
        ///     Finds an <see cref="TEntity" /> and includes his <see cref="Entity" /> container
        /// </summary>
        /// <param name="entityId">The <see cref="Entity" /> id</param>
        /// <returns>A <see cref="Task" /> with the <see cref="TEntity" /></returns>
        Task<TEntity> FindEntityWithContainer(Guid entityId);

        /// <summary>
        ///     Gets all <see cref="TEntity" /> that are contained by the <see cref="Entity" /> with the Id
        ///     <see cref="containerId" /> and associated <see cref="Entity" />
        /// </summary>
        /// <param name="containerId">The <see cref="Guid" /> of the container</param>
        /// <param name="deepLevel">The deep level</param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /></returns>
        Task<IEnumerable<Entity>> GetContainedEntities(Guid containerId, int deepLevel = 0);

        /// <summary>
        ///     Verifies if the container of the <see cref="Entity" /> has the given <see cref="Guid" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="Entity" /></param>
        /// <param name="containerId">The <see cref="Guid" /> of the container</param>
        /// <returns>A <see cref="Task" /> with the value of the check</returns>
        Task<bool> EntityIsContainedBy(Guid entityId, Guid containerId);
    }
}
