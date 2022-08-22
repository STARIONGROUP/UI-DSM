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
    using UI_DSM.Server.Types;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     The <see cref="RoleController" /> is a <see cref="Controller" /> to manage <see cref="Role" />s
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
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
        ///     Gets a collection of <see cref="Role" />s
        /// </summary>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="RoleDto" />as result</returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await this.roleManager.GetRoles();
            return this.Ok(roles.Select(x => (RoleDto)x.ToDto()));
        }

        /// <summary>
        ///     Gets a <see cref="Role" /> based on its <see cref="Guid" />
        /// </summary>
        /// <param name="roleId">The <see cref="Guid" /> of the <see cref="Role" /> to get</param>
        /// <returns>A <see cref="Task" /> with the <see cref="RoleDto" /> if found</returns>
        [HttpGet("{roleId:guid}")]
        [Authorize]
        public async Task<IActionResult> GetRole(Guid roleId)
        {
            var role = await this.roleManager.GetRole(roleId);
            return role == null ? this.NotFound() : this.Ok((RoleDto)role.ToDto());
        }

        /// <summary>
        ///     Trues ti create a new <see cref="Role" />
        /// </summary>
        /// <param name="newRole">The <see cref="RoleDto" /></param>
        /// <returns>A <see cref="Task" /> with the result of the creation</returns>
        [HttpPost("Create")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateRole([FromBody] RoleDto newRole)
        {
            var response = new EntityRequestResponseDto<RoleDto>();

            if (newRole.InstantiatePoco() is not Role role || role.Id != Guid.Empty)
            {
                response.Errors = new List<string>
                {
                    "Invalid DTO or the Id has to be empty"
                };

                return this.BadRequest(response);
            }

            role.ResolveProperties(newRole);

            var identityResult = await this.roleManager.CreateRole(role);
            return this.HandleOperationResult(response, identityResult);
        }

        /// <summary>
        ///     Tries to update an existing <see cref="Role" />
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the <see cref="Role" /> to update</param>
        /// <param name="role">The <see cref="RoleDto" /> to update the <see cref="Role" /></param>
        /// <returns>A <see cref="Task" /> with the result of the operation</returns>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UpdateRole(Guid id, [FromBody] RoleDto role)
        {
            var existingRole = await this.roleManager.GetRole(id);
            var response = new EntityRequestResponseDto<RoleDto>();

            if (existingRole == null)
            {
                response.Errors = new List<string>
                {
                    $"Role with the id {id} does not exist"
                };

                return this.NotFound(response);
            }

            existingRole.ResolveProperties(role);
            var identityResult = await this.roleManager.UpdateRole(existingRole);
            return this.HandleOperationResult(response, identityResult);
        }

        /// <summary>
        ///     Tries to delete a <see cref="Role" /> defined by the given <see cref="roleId" />
        /// </summary>
        /// <param name="roleId">The <see cref="Guid" /> of the <see cref="Role" /> to delete</param>
        /// <returns>A <see cref="Task" /> with the result of the operation</returns>
        [HttpDelete("{roleId:guid}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteRole(Guid roleId)
        {
            var role = await this.roleManager.GetRole(roleId);
            var response = new RequestResponseDto();

            if (role == null)
            {
                response.Errors = new List<string>
                {
                    $"Role with the id {roleId} does not exist"
                };

                return this.NotFound(response);
            }

            var identityResult = await this.roleManager.DeleteRole(role);
            response.IsRequestSuccessful = identityResult.Succeeded;

            if (identityResult.Succeeded)
            {
                return this.Ok(response);
            }

            response.Errors = identityResult.Errors;
            return this.StatusCode(500, response);
        }

        /// <summary>
        ///     Creates the correct <see cref="IActionResult" /> based on an operation
        /// </summary>
        /// <param name="response">The <see cref="EntityRequestResponseDto{TEntityDto}" /> to reply</param>
        /// <param name="identityResult">The <see cref="EntityRequestResponseDto{TEntityDto}" /></param>
        /// <returns>An <see cref="IActionResult" /></returns>
        private IActionResult HandleOperationResult(EntityRequestResponseDto<RoleDto> response, EntityOperationResult<Role> identityResult)
        {
            response.IsRequestSuccessful = identityResult.Succeeded;

            if (!identityResult.Succeeded)
            {
                response.Errors = identityResult.Errors;
                return this.BadRequest(response);
            }

            response.Entity = identityResult.Entity.ToDto() as RoleDto;
            return this.Ok(response);
        }
    }
}
