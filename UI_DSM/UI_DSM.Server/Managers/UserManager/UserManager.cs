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

    using UI_DSM.Server.Context;
    using UI_DSM.Server.Types;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This manager handles operation to the Database for <see cref="UserEntity" />s
    /// </summary>
    public class UserManager : EntityManager<UserEntity>, IUserManager
    {
        /// <summary>
        ///     The <see cref="UserManager{TUser}" />
        /// </summary>
        private readonly UserManager<User> authenticationUserManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserManager" /> class.
        /// </summary>
        /// <param name="context">The <see cref="DatabaseContext" /></param>
        /// <param name="authenticationUserManager">The <see cref="authenticationUserManager" /></param>
        public UserManager(DatabaseContext context, UserManager<User> authenticationUserManager) : base(context)
        {
            this.authenticationUserManager = authenticationUserManager;
        }

        /// <summary>
        ///     Creates a new <see cref="UserEntity" /> and store it into the <see cref="DatabaseContext" />
        /// </summary>
        /// <param name="entity">The <see cref="UserEntity" /> to create</param>
        /// <returns>A <see cref="Task" /> with the result of the creation</returns>
        public override Task<EntityOperationResult<UserEntity>> CreateEntity(UserEntity entity)
        {
            return HandleNotSupportedOperation();
        }

        /// <summary>
        ///     Updates a <see cref="UserEntity" />
        /// </summary>
        /// <param name="entity">The <see cref="UserEntity" /> to update</param>
        /// <returns>A <see cref="Task" /> with the result of the update</returns>
        public override Task<EntityOperationResult<UserEntity>> UpdateEntity(UserEntity entity)
        {
            return HandleNotSupportedOperation();
        }

        /// <summary>
        ///     Deletes a <see cref="UserEntity" />
        /// </summary>
        /// <param name="entity">The <see cref="UserEntity" /> to delete</param>
        /// <returns>A <see cref="Task" /> with the result of the deletion</returns>
        public override async Task<EntityOperationResult<UserEntity>> DeleteEntity(UserEntity entity)
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

            if (!identityResult.Succeeded)
            {
                return EntityOperationResult<UserEntity>.Failed(identityResult.Errors.Select(x => x.Description).ToArray());
            }

            return await base.DeleteEntity(entity);
        }

        /// <summary>
        ///     Resolve all properties for the <see cref="UserEntity" />
        /// </summary>
        /// <param name="entity">The <see cref="UserEntity" /></param>
        /// <param name="dto">The <see cref="EntityDto" /></param>
        /// <returns>A <see cref="Task" /></returns>
        public override async Task ResolveProperties(UserEntity entity, EntityDto dto)
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

            return await this.AddEntityToContext(userEntity);
        }

        /// <summary>
        ///     Gets a <see cref="UserEntity" /> by its name
        /// </summary>
        /// <param name="userName">The name of the <see cref="UserEntity" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="UserEntity" /> if found</returns>
        public async Task<UserEntity> GetUserByName(string userName)
        {
            return await this.EntityDbSet.FirstOrDefaultAsync(x => x.UserName == userName);
        }

        /// <summary>
        ///     Gets a collection of <see cref="UserEntity" /> by predicate
        /// </summary>
        /// <param name="predicate">The <see cref="Predicate{T}" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="UserEntity" /></returns>
        public async Task<IEnumerable<UserEntity>> GetUsers(Func<UserEntity, bool> predicate)
        {
            var users = this.EntityDbSet.Where(predicate);
            await Task.CompletedTask;
            return users;
        }
    }
}
