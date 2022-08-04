// --------------------------------------------------------------------------------------------------------
// <copyright file="Logout.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.Services.AuthenticationService;

    /// <summary>
    /// The <see cref="Login"/> enables the user to log out to the E-TM-10-25 data source
    /// </summary>
    public partial class Logout
    {
        /// <summary>
        /// Gets or sets the <see cref="IAuthenticationService"/>
        /// </summary>
        [Inject]
        public IAuthenticationService AuthenticationService { get; set; }

        /// <summary>
        /// Logout from the data source
        /// </summary>
        /// <returns></returns>
        public async Task ExecuteLogout()
        {
            if (this.AuthenticationService != null)
            {
                await this.AuthenticationService.Logout();
            }
        }
    }
}
