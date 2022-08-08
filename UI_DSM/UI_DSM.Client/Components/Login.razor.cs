// --------------------------------------------------------------------------------------------------------
// <copyright file="Login.cs" company="RHEA System S.A.">
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
    using Microsoft.AspNetCore.Components.Forms;

    using UI_DSM.Client.Enumerator;
    using UI_DSM.Client.Services.AuthenticationService;
    using UI_DSM.Shared.DTO;

    /// <summary>
    /// The <see cref="Login"/> enables the user to login to a E-TM-10-25 data source
    /// </summary>
    public partial class Login
    {
        /// <summary>
        /// The <see cref="AuthenticationDto"/> used for the <see cref="EditForm"/>
        /// </summary>
	    private AuthenticationDto authentication = new();

        /// <summary>
        /// Gets or sets the <see cref="Enumerator.AuthenticationStatus"/>
        /// </summary>
        public AuthenticationStatus AuthenticationStatus { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IAuthenticationService"/>
        /// </summary>
        [Inject]
        public IAuthenticationService AuthenticationService { get; set; }

        /// <summary>
        /// Gets or sets the login error message
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Tries to authenticate to the server
        /// </summary>
        /// <returns>A <see cref="Task"/></returns>
        public async Task ExecuteLogin()
        {
            this.AuthenticationStatus = AuthenticationStatus.Authenticating;
            this.ErrorMessage = string.Empty;

            this.StateHasChanged();

            if (this.AuthenticationService != null)
            {
                var response = await this.AuthenticationService.Login(this.authentication);
                this.AuthenticationStatus = response.IsAuthenticated ? AuthenticationStatus.Success : AuthenticationStatus.Fail;
                this.ErrorMessage = response.ErrorMessage;
            }

            this.StateHasChanged();
        }
    }
}
