// --------------------------------------------------------------------------------------------------------
// <copyright file="UserEntity.cs" company="RHEA System S.A.">
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
    using UI_DSM.Shared.Models;

    /// <summary>
    ///    The Data Transfer Object representing the <see cref="UserEntity" /> class.
    /// </summary>
    public partial class UserEntityDto : EntityDto
    {
        /// <summary>
        ///    Initializes a new <see cref="UserEntityDto" /> class.
        /// </summary>
        public UserEntityDto()
        {
        }

        /// <summary>
        ///    Initializes a new <see cref="UserEntityDto" /> class.
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the represented <see cref="Entity" /></param>
        public UserEntityDto(Guid id) : base(id)
        {
        }

        /// <summary>
        ///    Gets or sets the UserName of the UserEntity
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        ///    Gets or sets the IsAdmin of the UserEntity
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        ///    Instantiate a <see cref="Entity" /> from a <see cref="EntityDto" />
        /// </summary>
        public override Entity InstantiatePoco()
        {
            return new UserEntity(this.Id);
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------