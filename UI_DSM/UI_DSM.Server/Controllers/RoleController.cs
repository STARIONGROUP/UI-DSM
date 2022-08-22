// --------------------------------------------------------------------------------------------------------
// <copyright file="RoleController.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using UI_DSM.Server.Managers.RoleManager;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     The <see cref="RoleController" /> is a <see cref="Controller" /> to manage <see cref="Role" />s
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : EntityController<Role, RoleDto>
    {
        /// <summary>
        ///     The <see cref="IRoleManager" />
        /// </summary>
        private readonly IRoleManager roleManager;

        /// <summary>
        ///     Initialize a new <see cref="RoleController" />
        /// </summary>
        /// <param name="roleManager">The <see cref="IRoleManager" /></param>
        public RoleController(IRoleManager roleManager)
        {
            this.roleManager = roleManager;
        }

        /// <summary>
        ///     Gets a collection of all <see cref="RoleDto" />
        /// </summary>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="RoleDto" />as result</returns>
        [Authorize]
        public override async Task<IActionResult> GetEntities()
        {
            var roles = await this.roleManager.GetEntities();
            return this.Ok(roles.Select(x => (RoleDto)x.ToDto()));
        }

        /// <summary>
        ///     Get a <see cref="RoleDto" /> based on its <see cref="Guid" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="RoleDto" /> if found</returns>
        [Authorize]
        public override async Task<IActionResult> GetEntity(Guid entityId)
        {
            var role = await this.roleManager.GetEntity(entityId);
            return role == null ? this.NotFound() : this.Ok((RoleDto)role.ToDto());
        }

        /// <summary>
        ///     Tries to create a new <see cref="Entity" /> based on its <see cref="RoleDto" />
        /// </summary>
        /// <param name="dto">The <see cref="RoleDto" /></param>
        /// <returns>A <see cref="Task" /> with the result of the creation</returns>
        [Authorize(Roles = "Administrator")]
        public override async Task<IActionResult> CreateEntity(RoleDto dto)
        {
            var response = new EntityRequestResponseDto<RoleDto>();

            if (dto.InstantiatePoco() is not Role role || role.Id != Guid.Empty)
            {
                response.Errors = new List<string>
                {
                    "Invalid DTO or the Id has to be empty"
                };

                return this.BadRequest(response);
            }

            role.ResolveProperties(dto);

            var identityResult = await this.roleManager.CreateEntity(role);
            return this.HandleOperationResult(response, identityResult);
        }

        /// <summary>
        ///     Tries to delete an <see cref="Entity" /> defined by the given <see cref="Guid" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="Entity" /> to delete</param>
        /// <returns>A <see cref="Task" /> with the result of the operation</returns>
        [Authorize(Roles = "Administrator")]
        public override async Task<IActionResult> DeleteEntity(Guid entityId)
        {
            var role = await this.roleManager.GetEntity(entityId);
            var response = new RequestResponseDto();

            if (role == null)
            {
                response.Errors = new List<string>
                {
                    $"Role with the id {entityId} does not exist"
                };

                return this.NotFound(response);
            }

            var identityResult = await this.roleManager.DeleteEntity(role);
            response.IsRequestSuccessful = identityResult.Succeeded;

            if (identityResult.Succeeded)
            {
                return this.Ok(response);
            }

            response.Errors = identityResult.Errors;
            return this.StatusCode(500, response);
        }

        /// <summary>
        ///     Tries to update an existing <see cref="Entity" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="Entity" /> to update</param>
        /// <param name="dto">The <see cref="RoleDto" /> to update the <see cref="Entity" /></param>
        /// <returns>A <see cref="Task" /> with the result of the operation</returns>
        [Authorize(Roles = "Administrator")]
        public override async Task<IActionResult> UpdateEntity(Guid entityId, RoleDto dto)
        {
            var existingRole = await this.roleManager.GetEntity(entityId);
            var response = new EntityRequestResponseDto<RoleDto>();

            if (existingRole == null)
            {
                response.Errors = new List<string>
                {
                    $"Role with the id {entityId} does not exist"
                };

                return this.NotFound(response);
            }

            existingRole.ResolveProperties(dto);
            var identityResult = await this.roleManager.UpdateEntity(existingRole);
            return this.HandleOperationResult(response, identityResult);
        }
    }
}
