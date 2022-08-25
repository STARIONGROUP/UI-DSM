// --------------------------------------------------------------------------------------------------------
// <copyright file="EntityController.cs" company="RHEA System S.A.">
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
    using Microsoft.AspNetCore.Mvc;

    using UI_DSM.Server.Types;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Abstract class for all <see cref="ControllerBase" /> controlling <see cref="Entity" /> and
    ///     <see cref="EntityDto" />
    /// </summary>
    /// <typeparam name="TEntity">An <see cref="Entity"/></typeparam>
    /// <typeparam name="TEntityDto">A <see cref="EntityDto" /></typeparam>
    public abstract class EntityController<TEntity, TEntityDto>: ControllerBase where TEntity : Entity where TEntityDto : EntityDto
    {
        /// <summary>
        ///     Gets a collection of all <see cref="TEntityDto" />
        /// </summary>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="TEntityDto" />as result</returns>
        [HttpGet]
        public abstract Task<IActionResult> GetEntities();

        /// <summary>
        ///     Get a <see cref="TEntityDto" /> based on its <see cref="Guid" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="TEntityDto" /> if found</returns>
        [HttpGet("{entityId:guid}")]
        public abstract Task<IActionResult> GetEntity(Guid entityId);

        /// <summary>
        ///     Tries to create a new <see cref="Entity" /> based on its <see cref="TEntityDto" />
        /// </summary>
        /// <param name="dto">The <see cref="TEntityDto" /></param>
        /// <returns>A <see cref="Task" /> with the result of the creation</returns>
        [HttpPost("Create")]
        public abstract Task<IActionResult> CreateEntity([FromBody] TEntityDto dto);

        /// <summary>
        ///     Tries to delete an <see cref="Entity" /> defined by the given <see cref="Guid" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="Entity" /> to delete</param>
        /// <returns>A <see cref="Task" /> with the result of the operation</returns>
        [HttpDelete("{entityId:guid}")]
        public abstract Task<IActionResult> DeleteEntity(Guid entityId);

        /// <summary>
        ///     Tries to update an existing <see cref="Entity" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="Entity" /> to update</param>
        /// <param name="dto">The <see cref="TEntityDto" /> to update the <see cref="Entity" /></param>
        /// <returns>A <see cref="Task" /> with the result of the operation</returns>
        [HttpPut("{entityId:guid}")]
        public abstract Task<IActionResult> UpdateEntity(Guid entityId, [FromBody] TEntityDto dto);

        /// <summary>
        ///     Creates the correct <see cref="IActionResult" /> based on an operation
        /// </summary>
        /// <param name="response">The <see cref="EntityRequestResponseDto{TEntityDto}" /> to reply</param>
        /// <param name="identityResult">The <see cref="EntityRequestResponseDto{TEntityDto}" /></param>
        /// <returns>An <see cref="IActionResult" /></returns>
        protected IActionResult HandleOperationResult(EntityRequestResponseDto<TEntityDto> response, EntityOperationResult<TEntity> identityResult)
        {
            response.IsRequestSuccessful = identityResult.Succeeded;

            if (!identityResult.Succeeded)
            {
                response.Errors = identityResult.Errors;
                return this.BadRequest(response);
            }

            response.Entity = identityResult.Entity.ToDto() as TEntityDto;
            return this.Ok(response);
        }
    }
}
