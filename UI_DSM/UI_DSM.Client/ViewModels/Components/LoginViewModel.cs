// --------------------------------------------------------------------------------------------------------
// <copyright file="LoginViewModel.cs" company="RHEA System S.A.">
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
    using Microsoft.AspNetCore.Components.Forms;

    using ReactiveUI;

    using UI_DSM.Client.Components;
    using UI_DSM.Client.Enumerator;
    using UI_DSM.Client.Services.AuthenticationService;
    using UI_DSM.Shared.DTO.UserManagement;

    /// <summary>
    ///     View model for <see cref="Login" /> component
    /// </summary>
    public class LoginViewModel : ReactiveObject, ILoginViewModel
    {
        /// <summary>
        ///     Gets or sets the <see cref="IAuthenticationService" />
        /// </summary>
        private readonly IAuthenticationService authenticationService;

        /// <summary>
        ///     Backing field for <see cref="AuthenticationStatus" />
        /// </summary>
        private AuthenticationStatus authenticationStatus;

        /// <summary>
        ///     Backing field for <see cref="ErrorMessage" />
        /// </summary>
        private string errorMessage;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LoginViewModel" /> class.
        /// </summary>
        public LoginViewModel(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        /// <summary>
        ///     The <see cref="AuthenticationDto" /> used for the <see cref="EditForm" />
        /// </summary>
        public AuthenticationDto Authentication { get; private set; } = new();

        /// <summary>
        ///     Gets or sets the <see cref="Enumerator.AuthenticationStatus" />
        /// </summary>
        public AuthenticationStatus AuthenticationStatus
        {
            get => this.authenticationStatus;
            set => this.RaiseAndSetIfChanged(ref this.authenticationStatus, value);
        }

        /// <summary>
        ///     Gets or sets the login error message
        /// </summary>
        public string ErrorMessage
        {
            get => this.errorMessage;
            set => this.RaiseAndSetIfChanged(ref this.errorMessage, value);
        }

        /// <summary>
        ///     Tries to authenticate to the server
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        public async Task ExecuteLogin()
        {
            this.AuthenticationStatus = AuthenticationStatus.Authenticating;
            this.ErrorMessage = string.Empty;

            var response = await this.authenticationService.Login(this.Authentication);
            this.AuthenticationStatus = response.IsRequestSuccessful ? AuthenticationStatus.Success : AuthenticationStatus.Fail;

            if (response.Errors != null && response.Errors.Any())
            {
                this.ErrorMessage = string.Join("\n", response.Errors);
            }
        }
    }
}
