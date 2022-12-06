// --------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationService.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Services.AuthenticationService
{
    using System.Net.Http.Headers;
    using System.Text;

    using Blazored.SessionStorage;

    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Authorization;

    using Newtonsoft.Json;

    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Shared.DTO.UserManagement;

    /// <summary>
    ///     The <see cref="AuthenticationService" /> allow the user to login and logout to the UI-DSM data source
    /// </summary>
    [Route("User")]
    public class AuthenticationService : ServiceBase, IAuthenticationService
    {
        /// <summary>
        ///     The <see cref="ISessionStorageService" />
        /// </summary>
        private readonly ISessionStorageService sessionStorageService;

        /// <summary>
        ///     The <see cref="AuthenticationStateProvider" />
        /// </summary>
        private readonly AuthenticationStateProvider stateProvider;

        /// <summary>Initializes a new instance of the <see cref="AuthenticationService" /> class.</summary>
        /// <param name="httpClient">The <see cref="HttpClient" /></param>
        /// <param name="jsonService">The <see cref="IJsonService" /></param>
        /// <param name="stateProvider">The <see cref="AuthenticationStateProvider" /></param>
        /// <param name="sessionStorageService">The <see cref="ISessionStorageService" /></param>
        public AuthenticationService(HttpClient httpClient, IJsonService jsonService, AuthenticationStateProvider stateProvider, ISessionStorageService sessionStorageService) :
            base(httpClient, jsonService)
        {
            this.stateProvider = stateProvider;
            this.sessionStorageService = sessionStorageService;
        }

        /// <summary>
        ///     Login to the UI-DSM data source with the provided <see cref="AuthenticationDto" />
        /// </summary>
        /// <param name="authentication">The <see cref="AuthenticationDto" /> to use to authenticate</param>
        /// <returns>A <see cref="Task" /> with a <see cref="AuthenticationResponseDto" /> result </returns>
        public async Task<AuthenticationResponseDto> Login(AuthenticationDto authentication)
        {
            var content = JsonConvert.SerializeObject(authentication);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

            var authenticationResult = await this.HttpClient.PostAsync(Path.Combine(this.MainRoute, "Login"),
                bodyContent);

            var result = this.jsonService.Deserialize<AuthenticationResponseDto>(await authenticationResult.Content.ReadAsStreamAsync());

            if (!authenticationResult.IsSuccessStatusCode)
            {
                return result;
            }

            await this.sessionStorageService.SetItemAsync(AuthenticationProvider.SessionStorageKey, result.Token);
            ((AuthenticationProvider)this.stateProvider).NotifyAuthenticationStateChanged();

            this.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthenticationProvider.AuthenticationHeaderKey, result.Token);

            return new AuthenticationResponseDto
            {
                IsRequestSuccessful = true
            };
        }

        /// <summary>
        ///     Logout from the data source
        /// </summary>
        /// <returns>
        ///     a <see cref="Task" />
        /// </returns>
        public async Task Logout()
        {
            await this.sessionStorageService.RemoveItemAsync(AuthenticationProvider.SessionStorageKey);
            ((AuthenticationProvider)this.stateProvider).NotifyAuthenticationStateChanged();
        }
    }
}
