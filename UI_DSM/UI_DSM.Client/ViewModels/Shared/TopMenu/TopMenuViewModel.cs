// --------------------------------------------------------------------------------------------------------
// <copyright file="TopMenuViewModel.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw, Nabil Abbar
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Client.ViewModels.Shared.TopMenu
{
    using Microsoft.AspNetCore.Components.Authorization;

    using ReactiveUI;

    using UI_DSM.Client.Services.Administration.UserService;
    using UI_DSM.Client.Services.AuthenticationService;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     View model for the <see cref="Client.Shared.TopMenu.TopMenu" /> components
    /// </summary>
    public class TopMenuViewModel : ReactiveObject, ITopMenuViewModel
    {
        /// <summary>
        ///     The <see cref="AuthenticationProvider" />
        /// </summary>
        private readonly AuthenticationStateProvider authenticationProvider;

        /// <summary>
        ///     The <see cref="IUserService" />
        /// </summary>
        private readonly IUserService userService;

        /// <summary>
        ///     A collection of <see cref="Participant" /> linked to the current user
        /// </summary>
        private List<Participant> participants = new();

        /// <summary>
        ///     Backing field for <see cref="UserName" />
        /// </summary>
        private string userName;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TopMenuViewModel" /> class.
        /// </summary>
        /// <param name="userService">The <see cref="IUserService" /></param>
        /// <param name="authenticationProvider">The <see cref="AuthenticationStateProvider" /></param>
        public TopMenuViewModel(IUserService userService, AuthenticationStateProvider authenticationProvider)
        {
            this.userService = userService;
            this.authenticationProvider = authenticationProvider;
            this.authenticationProvider.AuthenticationStateChanged += this.OnAuthenticationStateChanged;
        }

        /// <summary>
        ///     The current logged user
        /// </summary>
        public string UserName
        {
            get => this.userName;
            private set => this.RaiseAndSetIfChanged(ref this.userName, value);
        }

        /// <summary>
        ///     Initializes this view model
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        public async Task InitializesViewModel()
        {
            var authenticationState = await this.authenticationProvider.GetAuthenticationStateAsync();

            if (authenticationState.User.Identity is { IsAuthenticated: true })
            {
                this.UserName = authenticationState.User.Identity?.Name;
                this.participants = await this.userService.GetParticipantsForUser();
            }
            else
            {
                this.UserName = null;
                this.participants.Clear();
            }
        }

        /// <summary>
        ///     Verify that the current user has access to the project management page
        /// </summary>
        /// <returns>The assert</returns>
        public bool HasAccessToProjectManagement()
        {
            return this.participants.Any(x => x.IsAllowedTo(AccessRight.ProjectManagement));
        }

        /// <summary>
        ///     Handle the change of authentication state
        /// </summary>
        /// <param name="state">The <see cref="AuthenticationState" /> Task</param>
        private void OnAuthenticationStateChanged(Task<AuthenticationState> state)
        {
            Task.Run(this.InitializesViewModel);
        }
    }
}
