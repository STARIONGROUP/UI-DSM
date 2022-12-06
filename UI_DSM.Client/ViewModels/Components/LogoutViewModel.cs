// --------------------------------------------------------------------------------------------------------
// <copyright file="LogoutViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Components
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.Components;
    using UI_DSM.Client.Services.AuthenticationService;

    /// <summary>
    ///     View model for the <see cref="Logout" /> component
    /// </summary>
    public class LogoutViewModel : ILogoutViewModel
    {
        /// <summary>
        ///     The <see cref="IAuthenticationService" />
        /// </summary>
        private readonly IAuthenticationService authenticationService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogoutViewModel" /> class.
        /// </summary>
        /// <param name="authenticationService">The <see cref="IAuthenticationService" /></param>
        /// <param name="navigationManager">The <see cref="Microsoft.AspNetCore.Components.NavigationManager" /></param>
        public LogoutViewModel(IAuthenticationService authenticationService, NavigationManager navigationManager)
        {
            this.authenticationService = authenticationService;
            this.NavigationManager = navigationManager;
        }

        /// <summary>
        ///     The <see cref="Microsoft.AspNetCore.Components.NavigationManager" />
        /// </summary>
        public NavigationManager NavigationManager { get; set; }

        /// <summary>
        ///     Logout from the data source
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        public async Task ExecuteLogout()
        {
            await this.authenticationService.Logout();
            this.NavigationManager.NavigateTo("/login");
        }
    }
}
