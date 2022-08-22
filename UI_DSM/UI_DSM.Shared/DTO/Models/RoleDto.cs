// --------------------------------------------------------------------------------------------------------
// <copyright file="RoleDto.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Shared.DTO.Models
{
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     The Data Transfer Object representing the <see cref="Role" /> class.
    /// </summary>
    public class RoleDto : EntityDto
    {
        /// <summary>
        ///     Initializes a new <see cref="RoleDto" />
        /// </summary>
        /// <param name="id">The <see cref="Guid" /></param>
        public RoleDto(Guid id) : base(id)
        {
        }

        /// <summary>
        ///     Initiazes a new <see cref="RoleDto" />
        /// </summary>
        public RoleDto()
        {
        }

        /// <summary>
        ///     The name of the specific <see cref="RoleDto" />
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        ///     A collection of <see cref="AccessRight" />
        /// </summary>
        public List<AccessRight> AccessRights { get; set; } = new();

        /// <summary>
        ///     Instantiate a <see cref="Entity" /> from a <see cref="EntityDto" />
        /// </summary>
        /// <returns>A new <see cref="Entity" /></returns>
        public override Entity InstantiatePoco()
        {
            return new Role(this.Id);
        }
    }
}
