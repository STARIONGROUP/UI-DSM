// --------------------------------------------------------------------------------------------------------
// <copyright file="User.cs" company="RHEA System S.A.">
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
    using Microsoft.AspNetCore.Identity;

    using UI_DSM.Shared.DTO.Models;

    /// <summary>
    ///     The <see cref="User" /> is used for authentication
    /// </summary>
    public sealed class User : IdentityUser
    {
        /// <summary>
        ///     Initializes a new instance of <see cref="User" />.
        /// </summary>
        /// <param name="id">The <see cref="Guid"/></param>
        public User(Guid id)
        {
            this.Id = id.ToString();
        }

        /// <summary>
        ///     Initializes a new instance of <see cref="User" />.
        /// </summary>
        public User()
        {
        }

        /// <summary>
        ///     Value indicating if the current <see cref="User" /> is an Administrator or not
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        ///     Instantiate a <see cref="UserEntityDto" /> from a <see cref="User" />
        /// </summary>
        /// <returns>A new <see cref="UserEntityDto" /></returns>
        public Entity ToEntity()
        {
            return new UserEntity(new Guid(this.Id))
            {
                UserName = this.UserName,
                IsAdmin = this.IsAdmin
            };
        }
    }
}
