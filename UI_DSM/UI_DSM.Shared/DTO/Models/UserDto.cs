// --------------------------------------------------------------------------------------------------------
// <copyright file="UserDto.cs" company="RHEA System S.A.">
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
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     DTO used to transfer non-private user data stored on the data base
    /// </summary>
    public class UserDto : EntityDto
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="UserDto" /> class.
        /// </summary>
        public UserDto()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserDto" /> class.
        /// </summary>
        /// <param name="id">The <see cref="Guid" /></param>
        public UserDto(Guid id) : base(id)
        {
        }

        /// <summary>
        ///     The name of the user
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        ///     Value indicating if the current user is the site administrator
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        ///     Instantiate a <see cref="User" /> from a <see cref="UserDto" />
        /// </summary>
        /// <returns>A new <see cref="User" /></returns>
        public override Entity InstantiatePoco()
        {
            return new UserEntity(this.Id);
        }
    }
}
