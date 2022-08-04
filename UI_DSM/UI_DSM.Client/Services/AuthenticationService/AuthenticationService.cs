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
    using System.Text.Json;

    using Blazored.SessionStorage;

    using Microsoft.AspNetCore.Components.Authorization;

    using UI_DSM.Shared.DTO;

    /// <summary>
    ///     The <see cref="AuthenticationService" /> allow the user to login and logout to the UI-DSM data source
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        /// <summary>
        ///     The <see cref="HttpClient" /> to acess to the API
        /// </summary>
        private readonly HttpClient httpClient;

        /// <summary>
        ///     The <see cref="JsonSerializerOptions" /> that is used for JSON action
        /// </summary>
        private readonly JsonSerializerOptions jsonSerializerOptions;

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
        /// <param name="stateProvider">The <see cref="AuthenticationStateProvider" /></param>
        /// <param name="sessionStorageService">The <see cref="ISessionStorageService" /></param>
        public AuthenticationService(HttpClient httpClient, AuthenticationStateProvider stateProvider, ISessionStorageService sessionStorageService)
        {
            this.httpClient = httpClient;
            this.stateProvider = stateProvider;
            this.sessionStorageService = sessionStorageService;
            this.jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        /// <summary>
        ///     Login to the UI-DSM data source with the provided <see cref="AuthenticationDto" />
        /// </summary>
        /// <param name="authentication">The <see cref="AuthenticationDto" /> to use to authenticate</param>
        /// <returns>A <see cref="Task" /> with a <see cref="AuthenticationResponseDto" /> result </returns>
        public async Task<AuthenticationResponseDto> Login(AuthenticationDto authentication)
        {
            var content = JsonSerializer.Serialize(authentication);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

            var authenticationResult = await this.httpClient.PostAsync("User/Login", bodyContent);
            var authenticationContent = await authenticationResult.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AuthenticationResponseDto>(authenticationContent, this.jsonSerializerOptions);

            if (!authenticationResult.IsSuccessStatusCode)
            {
                return result;
            }

            await this.sessionStorageService.SetItemAsync(AuthenticationProvider.SessionStorageKey, result.Token);
            ((AuthenticationProvider)this.stateProvider).NotifyAuthenticationStateChanged();

            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthenticationProvider.AuthenticationHeaderKey, result.Token);

            return new AuthenticationResponseDto
            {
                IsAuthenticated = true
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
