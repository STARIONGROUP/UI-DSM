// --------------------------------------------------------------------------------------------------------
// <copyright file="RoleService.cs" company="RHEA System S.A.">
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
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.Types;

    /// <summary>
    ///     The <see cref="RoleService" /> provide capability to manage <see cref="Role" />s.
    /// </summary>
    [Route("Role")]
    public class RoleService : EntityServiceBase<Role, RoleDto>, IRoleService
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RoleService" /> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="ServiceBase.HttpClient" /></param>
        public RoleService(HttpClient httpClient) : base(httpClient)
        {
        }

        /// <summary>
        ///     Provide sa collection of <see cref="Role" />
        /// </summary>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> where the result is a collection of <see cref="Project" /></returns>
        public async Task<List<Role>> GetRoles(int deepLevel = 0)
        {
            try
            {
                return await this.GetEntities(deepLevel);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Gets a <see cref="Role" /> based on its <see cref="Guid" />
        /// </summary>
        /// <param name="roleId">The <see cref="Guid" /> of the<see cref="Role" /></param>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> with the <see cref="Role" /> if found</returns>
        public async Task<Role> GetRole(Guid roleId, int deepLevel = 0)
        {
            try
            {
                return await this.GetEntity(roleId, deepLevel);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Creates a new <see cref="Role" />
        /// </summary>
        /// <param name="newRole">The <see cref="Role" /> to create</param>
        /// <returns>A <see cref="Task" /> with the <see cref="Role" /> if created correctly</returns>
        public async Task<EntityRequestResponse<Role>> CreateRole(Role newRole)
        {
            try
            {
                return await this.CreateEntity(newRole,0);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Updates a <see cref="Role" />
        /// </summary>
        /// <param name="role">The <see cref="Role" /> to update with new data</param>
        /// <returns>A <see cref="Task" /> with the updated <see cref="Role" /> if successful</returns>
        public async Task<EntityRequestResponse<Role>> UpdateRole(Role role)
        {
            try
            {
                return await this.UpdateEntity(role,0);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Deletes a <see cref="Role" />
        /// </summary>
        /// <param name="roleToDelete">The <see cref="Role" /> to delete</param>
        /// <returns>A <see cref="Task" /> with the <see cref="RequestResponseDto" /></returns>
        public async Task<RequestResponseDto> DeleteRole(Role roleToDelete)
        {
            try
            {
                return await this.DeleteEntity(roleToDelete);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }
    }
}
