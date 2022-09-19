// --------------------------------------------------------------------------------------------------------
// <copyright file="UserManager.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Managers.UserManager
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    using NLog;

    using UI_DSM.Server.Context;
    using UI_DSM.Server.Types;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This manager handles operation to the Database for <see cref="UserEntity" />s
    /// </summary>
    public class UserManager : IUserManager
    {
        /// <summary>
        ///     The <see cref="UserManager{TUser}" />
        /// </summary>
        private readonly UserManager<User> authenticationUserManager;

        /// <summary>
        ///     The <see cref="DatabaseContext" />
        /// </summary>
        private readonly DatabaseContext context;

        /// <summary>
        ///     The <see cref="NLog" /> logger
        /// </summary>
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserManager" /> class.
        /// </summary>
        /// <param name="context">The <see cref="DatabaseContext" /></param>
        /// <param name="authenticationUserManager">The <see cref="authenticationUserManager" /></param>
        public UserManager(DatabaseContext context, UserManager<User> authenticationUserManager)
        {
            this.context = context;
            this.authenticationUserManager = authenticationUserManager;
        }

        /// <summary>
        ///     Gets a collection of all <see cref="Entity" />s and associated <see cref="Entity" />
        /// </summary>
        /// <param name="deepLevel">The level of deepnest that we need to retrieve associated <see cref="Entity" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /> as result</returns>
        public async Task<IEnumerable<Entity>> GetEntities(int deepLevel = 0)
        {
            var users = await this.context.UsersEntities.ToListAsync();
            return users.SelectMany(x => x.GetAssociatedEntities(deepLevel)).DistinctBy(x => x.Id);
        }

        /// <summary>
        ///     Tries to get a <see cref="Entity" /> based on its <see cref="Guid" /> and its associated <see cref="Entity" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <param name="deepLevel">The level of deepnest that we need to retrieve associated <see cref="Entity" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /> if found</returns>
        public async Task<IEnumerable<Entity>> GetEntity(Guid entityId, int deepLevel = 0)
        {
            var user = await this.FindEntity(entityId);

            if (user == null)
            {
                return Enumerable.Empty<Entity>();
            }

            return user.GetAssociatedEntities(deepLevel);
        }

        /// <summary>
        ///     Tries to get a <see cref="UserEntity" /> based on its <see cref="Guid" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="UserEntity" /> if found</returns>
        public async Task<UserEntity> FindEntity(Guid entityId)
        {
            return await this.context.UsersEntities.FindAsync(entityId);
        }

        /// <summary>
        ///     Tries to get all <see cref="UserEntity" /> based on their <see cref="Guid" />
        /// </summary>
        /// <param name="entitiesId">A collection of <see cref="Guid" /></param>
        /// <returns>A collection of <see cref="UserEntity" /></returns>
        public async Task<IEnumerable<UserEntity>> FindEntities(IEnumerable<Guid> entitiesId)
        {
            var entities = new List<UserEntity>();

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
        ///     Creates a new <see cref="UserEntity" /> and store it into the <see cref="DatabaseContext" />
        /// </summary>
        /// <param name="entity">The <see cref="UserEntity" /> to create</param>
        /// <returns>A <see cref="Task" /> with the result of the creation</returns>
        public async Task<EntityOperationResult<UserEntity>> CreateEntity(UserEntity entity)
        {
            await Task.CompletedTask;
            return EntityOperationResult<UserEntity>.Failed("Cannot create an UserEntity");
        }

        /// <summary>
        ///     Updates a <see cref="UserEntity" />
        /// </summary>
        /// <param name="entity">The <see cref="UserEntity" /> to update</param>
        /// <returns>A <see cref="Task" /> with the result of the update</returns>
        public async Task<EntityOperationResult<UserEntity>> UpdateEntity(UserEntity entity)
        {
            await Task.CompletedTask;
            return EntityOperationResult<UserEntity>.Failed("Cannot update an UserEntity");
        }

        /// <summary>
        ///     Deletes a <see cref="UserEntity" />
        /// </summary>
        /// <param name="entity">The <see cref="UserEntity" /> to delete</param>
        /// <returns>A <see cref="Task" /> with the result of the deletion</returns>
        public async Task<EntityOperationResult<UserEntity>> DeleteEntity(UserEntity entity)
        {
            if (entity.IsAdmin)
            {
                return EntityOperationResult<UserEntity>.Failed("Forbidden to remove a admin user");
            }

            var user = await this.authenticationUserManager.FindByNameAsync(entity.UserName);

            if (user == null)
            {
                return EntityOperationResult<UserEntity>.Failed("Linked user not found");
            }

            var identityResult = await this.authenticationUserManager.DeleteAsync(user);

            this.context.UsersEntities.Remove(entity);

            await this.context.SaveChangesAsync();

            return identityResult.Succeeded
                ? EntityOperationResult<UserEntity>.Success(entity)
                : EntityOperationResult<UserEntity>.Failed(identityResult.Errors.Select(x => x.Description).ToArray());
        }

        /// <summary>
        ///     Resolve all properties for the <see cref="UserEntity" />
        /// </summary>
        /// <param name="entity">The <see cref="UserEntity" /></param>
        /// <param name="dto">The <see cref="EntityDto" /></param>
        /// <returns>A <see cref="Task" /></returns>
        public async Task ResolveProperties(UserEntity entity, EntityDto dto)
        {
            if (dto is not UserEntityDto userDto)
            {
                return;
            }

            entity.ResolveProperties(userDto, null);
            await Task.CompletedTask;
        }

        /// <summary>
        ///     Register a new <see cref="User" />
        /// </summary>
        /// <param name="user">The <see cref="User" /> to create</param>
        /// <param name="password">The <see cref="User" /> password</param>
        /// <returns>A <see cref="Task" /> with a <see cref="EntityOperationResult{TEntity}" /> as result</returns>
        public async Task<EntityOperationResult<UserEntity>> RegisterUser(User user, string password)
        {
            var identityResult = await this.authenticationUserManager.CreateAsync(user, password);

            if (!identityResult.Succeeded)
            {
                return EntityOperationResult<UserEntity>.Failed(identityResult.Errors.Select(x => x.Description).ToArray());
            }

            var createdUser = await this.authenticationUserManager.FindByNameAsync(user.UserName);

            var userEntity = new UserEntity
            {
                User = createdUser,
                IsAdmin = createdUser.IsAdmin,
                UserName = createdUser.UserName
            };

            var operationResult = new EntityOperationResult<UserEntity>(this.context.Add(userEntity), EntityState.Added);

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
        ///     Gets a <see cref="UserEntity" /> by its name
        /// </summary>
        /// <param name="userName">The name of the <see cref="UserEntity" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="UserEntity" /> if found</returns>
        public async Task<UserEntity> GetUserByName(string userName)
        {
            return await this.context.UsersEntities.FirstOrDefaultAsync(x => x.UserName == userName);
        }

        /// <summary>
        ///     Gets a collection of <see cref="UserEntity" /> by predicate
        /// </summary>
        /// <param name="predicate">The <see cref="Predicate{T}" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="UserEntity" /></returns>
        public async Task<IEnumerable<UserEntity>> GetUsers(Func<UserEntity, bool> predicate)
        {
            var users = this.context.UsersEntities.Where(predicate);
            await Task.CompletedTask;
            return users;
        }
    }
}
