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
    using UI_DSM.Server.Types;
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
        ///     Gets all existing <see cref="Role" />s
        /// </summary>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Role" /> as result</returns>
        public async Task<IEnumerable<Role>> GetRoles()
        {
            return await this.context.Roles.ToListAsync();
        }

        /// <summary>
        ///     Creates a new <see cref="Role" /> and store it into the <see cref="DatabaseContext" />
        /// </summary>
        /// <param name="role">The <see cref="Role" /> to create</param>
        /// <returns>A <see cref="Task" /> with the result of the creation</returns>
        public async Task<EntityOperationResult<Role>> CreateRole(Role role)
        {
            if (this.context.Roles.AsEnumerable().Any(x => x.RoleName.Equals(role.RoleName, StringComparison.InvariantCultureIgnoreCase)))
            {
                return EntityOperationResult<Role>.Failed("A role with the same name already exists");
            }

            var operationResult = new EntityOperationResult<Role>(this.context.Add(role), EntityState.Added);

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
        ///     Updates a <see cref="Role" />
        /// </summary>
        /// <param name="role">The <see cref="Role" /> to update</param>
        /// <returns>A <see cref="Task" /> with the result of the update</returns>
        public async Task<EntityOperationResult<Role>> UpdateRole(Role role)
        {
            if (this.context.Roles.AsEnumerable().Any(x => x.RoleName.Equals(role.RoleName, StringComparison.InvariantCultureIgnoreCase) 
                                                           && x.Id != role.Id))
            {
                return EntityOperationResult<Role>.Failed("A role with the same name already exists");
            }

            role.AccessRights.Sort();

            var operationResult = new EntityOperationResult<Role>(this.context.Update(role), EntityState.Modified, EntityState.Unchanged);

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
        ///     Delete a <see cref="Role" />
        /// </summary>
        /// <param name="role">The <see cref="Role" /> to delete</param>
        /// <returns>A <see cref="Task" /> with the result of the deletion</returns>
        public async Task<EntityOperationResult<Role>> DeleteRole(Role role)
        {
            var operationResult = new EntityOperationResult<Role>(this.context.Remove(role), EntityState.Deleted);

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
        ///     Tries to get a <see cref="Role" /> based on its <see cref="Role.Id" />
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the <see cref="Role" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="Role" /> if existing</returns>
        public async Task<Role> GetRole(Guid id)
        {
            return await this.context.Roles.FindAsync(id);
        }
    }
}
