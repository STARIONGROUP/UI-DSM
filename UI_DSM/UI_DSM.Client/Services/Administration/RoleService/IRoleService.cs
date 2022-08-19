// --------------------------------------------------------------------------------------------------------
// <copyright file="IRoleService.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Services.Administration.RoleService
{
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.Types;

    /// <summary>
    ///     Interface definition for <see cref="RoleService" />
    /// </summary>
    public interface IRoleService
    {
        /// <summary>
        ///     Provide sa collection of <see cref="Role" />
        /// </summary>
        /// <returns>A <see cref="Task" /> where the result is a collection of <see cref="Project" /></returns>
        Task<List<Role>> GetRoles();

        /// <summary>
        ///     Gets a <see cref="Role" /> based on its <see cref="Guid" />
        /// </summary>
        /// <param name="roleId">The <see cref="Guid" /> of the<see cref="Role" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="Role" /> if found</returns>
        Task<Role> GetRole(Guid roleId);

        /// <summary>
        ///     Creates a new <see cref="Role" />
        /// </summary>
        /// <param name="newRole">The <see cref="Role" /> to create</param>
        /// <returns>A <see cref="Task" /> with the <see cref="Role" /> if created correctly</returns>
        Task<EntityRequestResponse<Role>> CreateRole(Role newRole);

        /// <summary>
        ///     Updates a <see cref="Role" />
        /// </summary>
        /// <param name="role">The <see cref="Role" /> to update with new data</param>
        /// <returns>A <see cref="Task" /> with the updated <see cref="Role" /> if successful</returns>
        Task<EntityRequestResponse<Role>> UpdateRole(Role role);

        /// <summary>
        ///     Deletes a <see cref="Role" />
        /// </summary>
        /// <param name="roleToDelete">The <see cref="Role" /> to delete</param>
        /// <returns>A <see cref="Task" /> with the <see cref="RequestResponseDto" /></returns>
        Task<RequestResponseDto> DeleteRole(Role roleToDelete);
    }
}
