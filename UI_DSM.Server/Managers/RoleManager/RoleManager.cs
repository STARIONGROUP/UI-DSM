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
    using UI_DSM.Server.Context;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This manager handles operation to the Database for <see cref="Role" />s
    /// </summary>
    public class RoleManager : EntityManager<Role>, IRoleManager
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RoleManager" /> class
        /// </summary>
        /// <param name="context">The <see cref="DatabaseContext" /></param>
        public RoleManager(DatabaseContext context) : base(context)
        {
        }

        /// <summary>
        ///     Resolve all properties for the <see cref="Role" />
        /// </summary>
        /// <param name="entity">The <see cref="Role" /></param>
        /// <param name="dto">The <see cref="EntityDto" /></param>
        /// <returns>A <see cref="Task" /></returns>
        public override async Task ResolveProperties(Role entity, EntityDto dto)
        {
            if (dto is not RoleDto roleDto)
            {
                return;
            }

            entity.ResolveProperties(roleDto, null);
            await Task.CompletedTask;
        }

        /// <summary>
        ///     Gets the <see cref="SearchResultDto"/> based on a <see cref="Guid"/>
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="Role" /></param>
        /// <returns>A URL</returns>
        public override async Task<SearchResultDto> GetSearchResult(Guid entityId)
        {
            var role = await this.FindEntity(entityId);

            if (role == null)
            {
                return null;
            }

            return new SearchResultDto()
            {
                ObjectKind = nameof(Role),
                DisplayText = role.RoleName,
                BaseUrl = $"Role/{role.Id}"
            };
        }

        /// <summary>
        ///     Sets specific properties before the creation of the <see cref="Role" />
        /// </summary>
        /// <param name="entity">The <see cref="Role" /></param>
        protected override void SetSpecificPropertiesBeforeCreate(Role entity)
        {
            entity.AccessRights.Sort();
        }

        /// <summary>
        ///     Gets all <see cref="Entity" /> that needs to be unindexed when the current <see cref="Entity" /> is delete
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the entity</param>
        /// <returns>A collection of <see cref="Entity" /></returns>
        public override async Task<IEnumerable<Entity>> GetExtraEntitiesToUnindex(Guid entityId)
        {
            await Task.CompletedTask;
            return Enumerable.Empty<Entity>();
        }

        /// <summary>
        ///     Sets specific properties before the update of the <see cref="Role" />
        /// </summary>
        /// <param name="entity">The <see cref="Role" /></param>
        protected override void SetSpecificPropertiesBeforeUpdate(Role entity)
        {
            this.SetSpecificPropertiesBeforeCreate(entity);
        }
    }
}
