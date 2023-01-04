// --------------------------------------------------------------------------------------------------------
// <copyright file="ContainedEntityManager.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Managers
{
    using Microsoft.EntityFrameworkCore;

    using UI_DSM.Server.Context;
    using UI_DSM.Server.Extensions;
    using UI_DSM.Server.Types;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Base class for an <see cref="IContainedEntityManager{TEntity}" />
    /// </summary>
    /// <typeparam name="TEntity">An <see cref="Entity" /></typeparam>
    /// <typeparam name="TEntityContainer">An <see cref="Entity" /></typeparam>
    public abstract class ContainedEntityManager<TEntity, TEntityContainer> : EntityManager<TEntity>, IContainedEntityManager<TEntity>
        where TEntity : Entity
        where TEntityContainer : Entity
    {
        /// <summary>
        ///     The <see cref="DbSet{TEntity}" /> to use for the container of the <see cref="TEntity" />
        /// </summary>
        private readonly DbSet<TEntityContainer> containerDbSet;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContainedEntityManager{TEntity,TEntityContainer}" /> class.
        /// </summary>
        /// <param name="context">The <see cref="DatabaseContext" /></param>
        protected ContainedEntityManager(DatabaseContext context) : base(context)
        {
            this.containerDbSet = context.Set<TEntityContainer>();
        }

        /// <summary>
        ///     Finds an <see cref="TEntity" /> and includes his <see cref="Entity" /> container
        /// </summary>
        /// <param name="entityId">The <see cref="Entity" /> id</param>
        /// <returns>A <see cref="Task" /> with the <see cref="TEntity" /></returns>
        public async Task<TEntity> FindEntityWithContainer(Guid entityId)
        {
            return await this.EntityDbSet.Where(x => x.Id == entityId)
                .Include(x => x.EntityContainer)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Gets all <see cref="TEntity" /> that are contained by the <see cref="Entity" /> with the Id
        ///     <see cref="containerId" /> and associated <see cref="Entity" />
        /// </summary>
        /// <param name="containerId">The <see cref="Guid" /> of the container</param>
        /// <param name="deepLevel">The deep level</param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /></returns>
        public async Task<IEnumerable<Entity>> GetContainedEntities(Guid containerId, int deepLevel = 0)
        {
            var entities =  this.EntityDbSet.Where(x => x.EntityContainer != null && x.EntityContainer.Id == containerId)
                .BuildIncludeEntityQueryable(deepLevel);
                
            var list = await entities.ToListAsync();

            return list.SelectMany(x => x.GetAssociatedEntities(deepLevel)).DistinctBy(x => x.Id);
        }

        /// <summary>
        ///     Verifies if the container of the <see cref="Entity" /> has the given <see cref="Guid" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="Entity" /></param>
        /// <param name="containerId">The <see cref="Guid" /> of the container</param>
        /// <returns>A <see cref="Task" /> with the value of the check</returns>
        public async Task<bool> EntityIsContainedBy(Guid entityId, Guid containerId)
        {
            var entity = await this.FindEntityWithContainer(entityId);
            return entity != null && entity.EntityContainer.Id == containerId;
        }

        /// <summary>
        ///     Deletes a <see cref="TEntity" />
        /// </summary>
        /// <param name="entity">The <see cref="TEntity" /> to delete</param>
        /// <returns>A <see cref="Task" /> with the result of the deletion</returns>
        public override async Task<EntityOperationResult<TEntity>> DeleteEntity(TEntity entity)
        {
            if (!this.VerifyContainer(entity))
            {
                return EntityOperationResult<TEntity>.Failed($"Unable to compute the container the current entity with the id {entity.Id}");
            }

            return await base.DeleteEntity(entity);
        }

        /// <summary>
        ///     Validates the <see cref="TEntity" /> before doing any operation into the <see cref="DatabaseContext" />
        /// </summary>
        /// <param name="entity">The <see cref="TEntity" /> to validate</param>
        /// <param name="entityOperationResult">The <see cref="EntityOperationResult{TEntity}" /></param>
        /// <returns>A value indicating if the <see cref="TEntity"/> is valid or not</returns>
        protected override bool ValidateCurrentEntity(TEntity entity, out EntityOperationResult<TEntity> entityOperationResult)
        {
            if (!base.ValidateCurrentEntity(entity, out entityOperationResult))
            {
                return false;
            }

            entityOperationResult = null;

            if (!this.VerifyContainer(entity))
            {
                entityOperationResult = EntityOperationResult<TEntity>.Failed($"Unable to compute the container the current entity with the id {entity.Id}");
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Check if the container of the <see cref="Entity" /> exists inside the database
        /// </summary>
        /// <param name="entity">The <see cref="Entity" /></param>
        /// <returns>The result of the check</returns>
        private bool VerifyContainer(Entity entity)
        {
            return entity.EntityContainer != null && this.containerDbSet.Find(entity.EntityContainer.Id) != null;
        }
    }
}
