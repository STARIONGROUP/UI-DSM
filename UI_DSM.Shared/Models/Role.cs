// --------------------------------------------------------------------------------------------------------
// <copyright file="Role.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Shared.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Extensions;

    /// <summary>
    ///     A <see cref="Role" /> represents a role that a user can play in a project. Typically linked to privileges.
    /// </summary>
    [Table(nameof(Role))]
    public class Role : Entity
    {
        /// <summary>
        ///     Initializes a new <see cref="Role" />
        /// </summary>
        /// <param name="id"></param>
        public Role(Guid id) : base(id)
        {
        }

        /// <summary>
        ///     Initializes a new <see cref="Role" />
        /// </summary>
        public Role()
        {
        }

        /// <summary>
        ///     The name of the specific <see cref="Role" />
        /// </summary>
        [Required]
        public string RoleName { get; set; }

        /// <summary>
        ///     A collection of given <see cref="AccessRight" />
        /// </summary>
        [Required]
        public List<AccessRight> AccessRights { get; set; } = new();

        /// <summary>
        ///     Instantiate a <see cref="EntityDto" /> from a <see cref="Entity" />
        /// </summary>
        /// <returns>A new <see cref="EntityDto" /></returns>
        public override EntityDto ToDto()
        {
            return new RoleDto(this.Id)
            {
                RoleName = this.RoleName,
                AccessRights = this.AccessRights
            };
        }

        /// <summary>
        ///     Gets a <see cref="string" /> that represents all <see cref="AccessRight" /> contained into
        ///     <see cref="AccessRights" />
        /// </summary>
        /// <returns>A <see cref="string" /></returns>
        public string AccessRightsAsString()
        {
            return string.Join(", ", this.AccessRights.Select(x => x.GetEnumDisplayName()));
        }

        /// <summary>
        ///     Resolve the properties of the current <see cref="Role" /> from its <see cref="EntityDto" /> counter-part
        /// </summary>
        /// <param name="entityDto">The source <see cref="EntityDto" /></param>
        /// <param name="resolvedEntity">A <see cref="Dictionary{TKey,TValue}" /> of all <see cref="Entity" /></param>
        public override void ResolveProperties(EntityDto entityDto, Dictionary<Guid, Entity> resolvedEntity)
        {
            if (entityDto is not RoleDto roleDto)
            {
                throw new InvalidOperationException($"The DTO {entityDto.GetType()} does not match with the current Role POCO");
            }

            this.AccessRights = roleDto.AccessRights;
            this.RoleName = roleDto.RoleName;
        }
    }
}
