// --------------------------------------------------------------------------------------------------------
// <copyright file="RoleManager.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Managers.RoleManager
{
    using Microsoft.EntityFrameworkCore;

    using NLog;

    using UI_DSM.Server.Context;
    using UI_DSM.Server.Extensions;
    using UI_DSM.Server.Types;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This manager handles operation to the Database for <see cref="Role" />s
    /// </summary>
    public class RoleManager : IRoleManager
    {
        /// <summary>
        ///     The <see cref="DatabaseContext" />
        /// </summary>
        private readonly DatabaseContext context;

        /// <summary>
        ///     The <see cref="NLog" /> logger
        /// </summary>
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Initializes a new instance of the <see cref="RoleManager" /> class
        /// </summary>
        /// <param name="context">The <see cref="DatabaseContext" /></param>
        public RoleManager(DatabaseContext context)
        {
            this.context = context;
        }

        /// <summary>
        ///     Gets a collection of all <see cref="Entity" />s and associated <see cref="Entity" />
        /// </summary>
        /// <param name="deepLevel">The level of deepnest that we need to retrieve associated <see cref="Entity" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /> as result</returns>
        public async Task<IEnumerable<Entity>> GetEntities(int deepLevel = 0)
        {
            var roles = await this.context.UiDsmRoles.ToListAsync();
            return roles.SelectMany(x => x.GetAssociatedEntities(deepLevel)).DistinctBy(x => x.Id);
        }

        /// <summary>
        ///     Tries to get all <see cref="Role" /> based on their <see cref="Guid" />
        /// </summary>
        /// <param name="entitiesId">A collection of <see cref="Guid" /></param>
        /// <returns>A collection of <see cref="Role" /></returns>
        public async Task<IEnumerable<Role>> FindEntities(IEnumerable<Guid> entitiesId)
        {
            var entities = new List<Role>();

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
        ///     Creates a new <see cref="Role" /> and store it into the <see cref="DatabaseContext" />
        /// </summary>
        /// <param name="entity">The <see cref="Role" /> to create</param>
        /// <returns>A <see cref="Task" /> with the result of the creation</returns>
        public async Task<EntityOperationResult<Role>> CreateEntity(Role entity)
        {
            var operationResult = new EntityOperationResult<Role>(this.context.Add(entity), EntityState.Added);

            try
            {
                await this.context.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                if (ExceptionHelper.IsUniqueConstraintViolation(exception))
                {
                    operationResult.HandleExpection($"The name {entity.RoleName} is already used");
                }
                else
                {
                    operationResult.HandleExpection(exception);
                    this.logger.Error(exception.Message);
                }
            }

            return operationResult;
        }

        /// <summary>
        ///     Updates a <see cref="Role" />
        /// </summary>
        /// <param name="entity">The <see cref="Role" /> to update</param>
        /// <returns>A <see cref="Task" /> with the result of the update</returns>
        public async Task<EntityOperationResult<Role>> UpdateEntity(Role entity)
        {
            entity.AccessRights.Sort();

            var operationResult = new EntityOperationResult<Role>(this.context.Update(entity), EntityState.Modified, EntityState.Unchanged);

            try
            {
                await this.context.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                if (ExceptionHelper.IsUniqueConstraintViolation(exception))
                {
                    operationResult.HandleExpection($"The name {entity.RoleName} is already used");
                }
                else
                {
                    operationResult.HandleExpection(exception);
                    this.logger.Error(exception.Message);
                }
            }

            return operationResult;
        }

        /// <summary>
        ///     Deletes a <see cref="Role" />
        /// </summary>
        /// <param name="entity">The <see cref="Role" /> to delete</param>
        /// <returns>A <see cref="Task" /> with the result of the deletion</returns>
        public async Task<EntityOperationResult<Role>> DeleteEntity(Role entity)
        {
            var operationResult = new EntityOperationResult<Role>(this.context.Remove(entity), EntityState.Deleted, EntityState.Detached);

            try
            {
                await this.context.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                operationResult.HandleExpection(exception);
                this.logger.Error(exception.Message);
            }

            return operationResult;
        }

        /// <summary>
        ///     Resolve all properties for the <see cref="Role" />
        /// </summary>
        /// <param name="entity">The <see cref="Role" /></param>
        /// <param name="dto">The <see cref="EntityDto" /></param>
        /// <returns>A <see cref="Task" /></returns>
        public async Task ResolveProperties(Role entity, EntityDto dto)
        {
            if (dto is not RoleDto roleDto)
            {
                return;
            }

            entity.ResolveProperties(roleDto, null);
            await Task.CompletedTask;
        }

        /// <summary>
        ///     Tries to get a <see cref="Entity" /> based on its <see cref="Guid" /> and its associated <see cref="Entity" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <param name="deepLevel">The level of deepnest that we need to retrieve associated <see cref="Entity" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" />  if found</returns>
        public async Task<IEnumerable<Entity>> GetEntity(Guid entityId, int deepLevel = 0)
        {
            var role = await this.FindEntity(entityId);

            if (role == null)
            {
                return Enumerable.Empty<Entity>();
            }

            return role.GetAssociatedEntities(deepLevel);
        }

        /// <summary>
        ///     Tries to get a <see cref="Role" /> based on its <see cref="Guid" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="Role" /> if found</returns>
        public async Task<Role> FindEntity(Guid entityId)
        {
            return await this.context.UiDsmRoles.FindAsync(entityId);
        }
    }
}
