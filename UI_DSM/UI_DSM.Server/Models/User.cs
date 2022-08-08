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

namespace UI_DSM.Server.Models
{
    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// The <see cref="User" /> is used for authentication 
    /// </summary>
    public class User : IdentityUser
    {
        /// <summary>
        /// Value indicating if the current <see cref="User"/> is an Administrator or not
        /// </summary>
        public bool IsAdmin { get; set; }
    }
}
