// --------------------------------------------------------------------------------------------------------
// <copyright file="EntityManager.cs" company="RHEA System S.A.">
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

    using NLog;

    using UI_DSM.Server.Context;
    using UI_DSM.Server.Extensions;
    using UI_DSM.Server.Types;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Base class for an <see cref="IEntityManager{TEntity}" />
    /// </summary>
    /// <typeparam name="TEntity">An <see cref="Entity" /></typeparam>
    public abstract class EntityManager<TEntity> : IEntityManager<TEntity> where TEntity : Entity
    {
        /// <summary>
        ///     The <see cref="NLog" /> logger
        /// </summary>
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        protected EntityManager(DatabaseContext context)
        {
            this.Context = context;
            this.EntityDbSet = context.Set<TEntity>();
        }

        /// <summary>
        ///     The <see cref="DatabaseContext" />
        /// </summary>
        protected DatabaseContext Context { get; private set; }

        /// <summary>
        ///     The <see cref="EntityDbSet" /> to use for the current <see cref="TEntity" />
        /// </summary>
        protected DbSet<TEntity> EntityDbSet { get; private set; }

        /// <summary>
        ///     Gets a collection of all <see cref="Entity" />s and associated <see cref="Entity" />
        /// </summary>
        /// <param name="deepLevel">The level of deepnest that we need to retrieve associated <see cref="Entity" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /> as result</returns>
        public async Task<IEnumerable<Entity>> GetEntities(int deepLevel = 0)
        {
            var entities = await this.EntityDbSet.BuildIncludeEntityQueryable(deepLevel).ToListAsync();
            return entities.SelectMany(x => x.GetAssociatedEntities(deepLevel)).DistinctBy(x => x.Id);
        }

        /// <summary>
        ///     Tries to get a <see cref="Entity" /> based on its <see cref="Guid" /> and its associated <see cref="Entity" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <param name="deepLevel">The level of deepnest that we need to retrieve associated <see cref="Entity" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /> if found</returns>
        public async Task<IEnumerable<Entity>> GetEntity(Guid entityId, int deepLevel = 0)
        {
            var entity = await this.EntityDbSet.Where(x => x.Id == entityId)
                .BuildIncludeEntityQueryable(deepLevel).FirstOrDefaultAsync();

            return entity == null ? Enumerable.Empty<Entity>() : entity.GetAssociatedEntities(deepLevel);
        }

        /// <summary>
        ///     Tries to get a <see cref="TEntity" /> based on its <see cref="Guid" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="TEntity" /> if found</returns>
        public async Task<TEntity> FindEntity(Guid entityId)
        {
            return await this.EntityDbSet.FindAsync(entityId);
        }

        /// <summary>
        ///     Tries to get all <see cref="TEntity" /> based on their <see cref="Guid" />
        /// </summary>
        /// <param name="entitiesId">A collection of <see cref="Guid" /></param>
        /// <returns>A collection of <see cref="TEntity" /></returns>
        public async Task<IEnumerable<TEntity>> FindEntities(IEnumerable<Guid> entitiesId)
        {
            var entities = new List<TEntity>();

            foreach (var id in entitiesId)
            {
                var entity = await this.FindEntity(id);

                if (entity != null)
                {
                    entities.Add(entity);
                }
            }

            return entities;
        }

        /// <summary>
        ///     Creates a new <see cref="TEntity" /> and store it into the <see cref="DatabaseContext" />
        /// </summary>
        /// <param name="entity">The <see cref="TEntity" /> to create</param>
        /// <returns>A <see cref="Task" /> with the result of the creation</returns>
        public virtual async Task<EntityOperationResult<TEntity>> CreateEntity(TEntity entity)
        {
            if (!this.ValidateCurrentEntity(entity, out var entityOperationResult))
            {
                return entityOperationResult;
            }

            return await this.AddEntityToContext(entity);
        }

        /// <summary>
        ///     Updates a <see cref="TEntity" />
        /// </summary>
        /// <param name="entity">The <see cref="TEntity" /> to update</param>
        /// <returns>A <see cref="Task" /> with the result of the update</returns>
        public virtual async Task<EntityOperationResult<TEntity>> UpdateEntity(TEntity entity)
        {
            if (!this.ValidateCurrentEntity(entity, out var entityOperationResult))
            {
                return entityOperationResult;
            }

            return await this.UpdateEntityIntoContext(entity);
        }

        /// <summary>
        ///     Deletes a <see cref="TEntity" />
        /// </summary>
        /// <param name="entity">The <see cref="TEntity" /> to delete</param>
        /// <returns>A <see cref="Task" /> with the result of the deletion</returns>
        public virtual async Task<EntityOperationResult<TEntity>> DeleteEntity(TEntity entity)
        {
            var operationResult = new EntityOperationResult<TEntity>(this.Context.Remove(entity), EntityState.Deleted, EntityState.Detached);

            try
            {
                await this.Context.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                operationResult.HandleExpection(exception);
                this.logger.Error(exception.Message);
            }

            return operationResult;
        }

        /// <summary>
        ///     Resolve all properties for the <see cref="TEntity" />
        /// </summary>
        /// <param name="entity">The <see cref="TEntity" /></param>
        /// <param name="dto">The <see cref="EntityDto" /></param>
        /// <returns>A <see cref="Task" /></returns>
        public abstract Task ResolveProperties(TEntity entity, EntityDto dto);

        /// <summary>
        ///     Gets the URL to access the <see cref="TEntity" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="TEntity" /></param>
        /// <returns>A URL</returns>
        public abstract Task<SearchResultDto> GetSearchResult(Guid entityId);

        /// <summary>
        ///     Gets all <see cref="Entity" /> that needs to be unindexed when the current <see cref="Entity" /> is delete
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the entity</param>
        /// <returns>A collection of <see cref="Entity" /></returns>
        public abstract Task<IEnumerable<Entity>> GetExtraEntitiesToUnindex(Guid entityId);

        /// <summary>
        ///     Validates the <see cref="TEntity" /> before doing any operation into the <see cref="DatabaseContext" />
        /// </summary>
        /// <param name="entity">The <see cref="TEntity" /> to validate</param>
        /// <param name="entityOperationResult">The <see cref="EntityOperationResult{TEntity}" /></param>
        /// <returns>A value indicating if the <see cref="TEntity" /> is valid or not</returns>
        protected virtual bool ValidateCurrentEntity(TEntity entity, out EntityOperationResult<TEntity> entityOperationResult)
        {
            var validations = this.Context.ValidateModel(entity);
            entityOperationResult = null;

            if (validations.Any())
            {
                entityOperationResult = EntityOperationResult<TEntity>.Failed(validations.ToArray());
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Treis to update the <see cref="TEntity" /> into the <see cref="DatabaseContext" />
        /// </summary>
        /// <param name="entity">The <see cref="TEntity" /> to update</param>
        /// <returns>A <see cref="Task" /> with the result of the update</returns>
        protected async Task<EntityOperationResult<TEntity>> UpdateEntityIntoContext(TEntity entity)
        {
            this.SetSpecificPropertiesBeforeUpdate(entity);

            var operationResult = new EntityOperationResult<TEntity>(this.Context.Update(entity), EntityState.Modified, EntityState.Unchanged);

            try
            {
                await this.Context.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                this.HandleException(exception, operationResult);
            }

            return operationResult;
        }

        /// <summary>
        ///     Tries the <see cref="TEntity" /> into the <see cref="DatabaseContext" />
        /// </summary>
        /// <param name="entity">The <see cref="TEntity" /> to add</param>
        /// <returns>A <see cref="Task" /> with the result of the creation</returns>
        protected async Task<EntityOperationResult<TEntity>> AddEntityToContext(TEntity entity)
        {
            this.SetSpecificPropertiesBeforeCreate(entity);

            var operationResult = new EntityOperationResult<TEntity>(this.Context.Add(entity), EntityState.Added);

            try
            {
                await this.Context.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                this.HandleException(exception, operationResult);
            }

            return operationResult;
        }

        /// <summary>
        ///     Handles an <see cref="Exception" /> and adds the correct message to the
        ///     <see cref="EntityOperationResult{TEntity}" />
        /// </summary>
        /// <param name="exception">The <see cref="Exception" /></param>
        /// <param name="operationResult">The <see cref="EntityOperationResult{TEntity}" /></param>
        protected void HandleException(Exception exception, EntityOperationResult<TEntity> operationResult)
        {
            if (ExceptionHelper.IsUniqueConstraintViolation(exception, out var uniqueException))
            {
                operationResult.HandleExpection($"An object with the same {uniqueException.ColumnName} property already exists");
            }
            else
            {
                operationResult.HandleExpection(exception);
            }

            this.logger.Error(exception.Message);
        }

        /// <summary>
        ///     Provides the correct returns values on unsupported operation
        /// </summary>
        /// <returns>A <see cref="Task" /> with the result of the unsupportd operation</returns>
        public static async Task<EntityOperationResult<TEntity>> HandleNotSupportedOperation()
        {
            await Task.CompletedTask;
            return EntityOperationResult<TEntity>.Failed("Operation not supported");
        }

        /// <summary>
        ///     Sets specific properties before the creation of the <see cref="TEntity" />
        /// </summary>
        /// <param name="entity">The <see cref="TEntity" /></param>
        protected virtual void SetSpecificPropertiesBeforeCreate(TEntity entity)
        {
        }

        /// <summary>
        ///     Sets specific properties before the update of the <see cref="TEntity" />
        /// </summary>
        /// <param name="entity">The <see cref="TEntity" /></param>
        protected virtual void SetSpecificPropertiesBeforeUpdate(TEntity entity)
        {
        }
    }
}
