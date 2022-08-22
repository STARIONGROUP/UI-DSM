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
    using System.Text;
    using System.Text.Json;

    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.Types;

    /// <summary>
    ///     The <see cref="RoleService" /> provide capability to manage <see cref="Role" />s.
    /// </summary>
    public class RoleService : ServiceBase, IRoleService
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
        /// <returns>A <see cref="Task" /> where the result is a collection of <see cref="Project" /></returns>
        public async Task<List<Role>> GetRoles()
        {
            try
            {
                var response = await this.HttpClient.GetAsync("Role");
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(content);
                }

                var rolesDtos = JsonSerializer.Deserialize<List<RoleDto>>(content, this.JsonSerializerOptions);

                return rolesDtos!.Select(x =>
                {
                    var role = (Role)x.InstantiatePoco();
                    role.ResolveProperties(x);
                    return role;
                }).ToList();
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
        /// <returns>A <see cref="Task" /> with the <see cref="Role" /> if found</returns>
        public async Task<Role> GetRole(Guid roleId)
        {
            try
            {
                var url = Path.Combine("Role", roleId.ToString());
                var response = await this.HttpClient.GetAsync(url);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var roleDto = JsonSerializer.Deserialize<RoleDto>(responseContent, this.JsonSerializerOptions);
                var role = (Role)roleDto!.InstantiatePoco();
                role.ResolveProperties(roleDto);
                return role;
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
                var content = JsonSerializer.Serialize((RoleDto)newRole.ToDto());
                var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

                var response = await this.HttpClient.PostAsync("Role/Create", bodyContent);
                var responseContent = await response.Content.ReadAsStringAsync();
                var entityRequestResponse = JsonSerializer.Deserialize<EntityRequestResponseDto<RoleDto>>(responseContent, this.JsonSerializerOptions);

                if (!entityRequestResponse!.IsRequestSuccessful)
                {
                    return EntityRequestResponse<Role>.Fail(entityRequestResponse.Errors);
                }

                var role = (Role)entityRequestResponse.Entity.InstantiatePoco();
                role.ResolveProperties(entityRequestResponse.Entity);

                return EntityRequestResponse<Role>.Success(role);
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
                var url = Path.Combine("Role", role.Id.ToString());
                var content = JsonSerializer.Serialize((RoleDto)role.ToDto());
                var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

                var response = await this.HttpClient.PutAsync(url, bodyContent);
                var responseContent = await response.Content.ReadAsStringAsync();
                var entityRequest = JsonSerializer.Deserialize<EntityRequestResponseDto<RoleDto>>(responseContent, this.JsonSerializerOptions);

                if (!entityRequest!.IsRequestSuccessful)
                {
                    return EntityRequestResponse<Role>.Fail(entityRequest.Errors);
                }

                role = (Role)entityRequest.Entity.InstantiatePoco();
                role.ResolveProperties(entityRequest.Entity);

                return EntityRequestResponse<Role>.Success(role);
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
                var url = Path.Combine("Role", roleToDelete.Id.ToString());
                var deleteResponse = await this.HttpClient.DeleteAsync(url);
                var deleteContent = await deleteResponse.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<RequestResponseDto>(deleteContent, this.JsonSerializerOptions);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }
    }
}
