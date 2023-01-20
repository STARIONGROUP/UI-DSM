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

// --------------------------------------------------------------------------------------------------------
// ------------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!------------
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Shared.DTO.Models
{
    using GP.SearchService.SDK.Definitions;

    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///    The Data Transfer Object representing the <see cref="Role" /> class.
    /// </summary>
    [SearchDto(nameof(RoleDto))]
    public partial class RoleDto : EntityDto
    {
        /// <summary>
        ///    Initializes a new <see cref="RoleDto" /> class.
        /// </summary>
        public RoleDto()
        {
            this.AccessRights = new List<AccessRight>();
        }

        /// <summary>
        ///    Initializes a new <see cref="RoleDto" /> class.
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the represented <see cref="Entity" /></param>
        public RoleDto(Guid id) : base(id)
        {
            this.AccessRights = new List<AccessRight>();
        }

        /// <summary>
        ///    Gets or sets the RoleName of the Role
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        ///    Gets or sets the AccessRights of the Role
        /// </summary>
        public List<AccessRight> AccessRights { get; set; }

        /// <summary>
        ///    Instantiate a <see cref="Entity" /> from a <see cref="EntityDto" />
        /// </summary>
        public override Entity InstantiatePoco()
        {
            return new Role(this.Id);
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------