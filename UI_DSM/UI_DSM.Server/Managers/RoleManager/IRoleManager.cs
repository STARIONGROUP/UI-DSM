// --------------------------------------------------------------------------------------------------------
// <copyright file="IRoleManager.cs" company="RHEA System S.A.">
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
    using UI_DSM.Server.Context;
    using UI_DSM.Server.Types;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface definition for <see cref="RoleManager" />
    /// </summary>
    public interface IRoleManager
    {
        /// <summary>
        ///     Gets all existing <see cref="Role" />s
        /// </summary>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Role" /> as result</returns>
        Task<IEnumerable<Role>> GetRoles();

        /// <summary>
        ///     Creates a new <see cref="Role" /> and store it into the <see cref="DatabaseContext" />
        /// </summary>
        /// <param name="role">The <see cref="Role" /> to create</param>
        /// <returns>A <see cref="Task" /> with the result of the creation</returns>
        Task<EntityOperationResult<Role>> CreateRole(Role role);

        /// <summary>
        ///     Updates a <see cref="Role" />
        /// </summary>
        /// <param name="role">The <see cref="Role" /> to update</param>
        /// <returns>A <see cref="Task" /> with the result of the update</returns>
        Task<EntityOperationResult<Role>> UpdateRole(Role role);

        /// <summary>
        ///     Delete a <see cref="Role" />
        /// </summary>
        /// <param name="role">The <see cref="Role" /> to delete</param>
        /// <returns>A <see cref="Task" /> with the result of the deletion</returns>
        Task<EntityOperationResult<Role>> DeleteRole(Role role);

        /// <summary>
        ///     Tries to get a <see cref="Role" /> based on its <see cref="Role.Id" />
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the <see cref="Role" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="Role" /> if existing</returns>
        Task<Role> GetRole(Guid id);
    }
}
