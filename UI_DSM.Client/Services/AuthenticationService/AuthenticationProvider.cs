// --------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationProvider.cs" company="RHEA System S.A.">
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
    using System.IdentityModel.Tokens.Jwt;
    using System.Net.Http.Headers;
    using System.Security.Claims;

    using Blazored.SessionStorage;

    using Microsoft.AspNetCore.Components.Authorization;

    /// <summary>
    ///     Provides information about the authentication state of the current user.
    /// </summary>
    public class AuthenticationProvider : AuthenticationStateProvider
    {
        /// <summary>
        ///     Value for the <see cref="ISessionStorageService" /> key for the JWT
        /// </summary>
        public const string SessionStorageKey = "authenticationToken";

        /// <summary>
        ///     Value for the <see cref="AuthenticationHeaderValue" /> key
        /// </summary>
        public const string AuthenticationHeaderKey = "Bearer";

        /// <summary>
        ///     Value for the <see cref="ClaimsIdentity.AuthenticationType" />
        /// </summary>
        public const string AuthenticationType = "jwtAuthenticationType";

        /// <summary>
        ///     The <see cref="HttpClient" /> to reach the API
        /// </summary>
        private readonly HttpClient httpClient;

        /// <summary>
        ///     The <see cref="ISessionStorageService" /> to retrieve saved JWT
        /// </summary>
        private readonly ISessionStorageService sessionStorage;

        /// <summary>
        ///     Initializes a new <see cref="AuthenticationProvider" />
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient" /></param>
        /// <param name="sessionStorage">The <see cref="ISessionStorageService" /></param>
        public AuthenticationProvider(HttpClient httpClient, ISessionStorageService sessionStorage)
        {
            this.httpClient = httpClient;
            this.sessionStorage = sessionStorage;
        }

        /// <summary>
        ///     Asynchronously gets an <see cref="AuthenticationState" /> that describes the current user.
        /// </summary>
        /// <returns>A task that, when resolved, gives an <see cref="AuthenticationState" /> instance that describes the current user.</returns>
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await this.sessionStorage.GetItemAsync<string>(SessionStorageKey);

            if (string.IsNullOrEmpty(token))
            {
                this.httpClient.DefaultRequestHeaders.Authorization = null;
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthenticationHeaderKey, token);

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(jwt.Claims, AuthenticationType)));
        }

        /// <summary>
        ///     Force the <see cref="NotifyAuthenticationStateChanged" /> event to be raised
        /// </summary>
        public void NotifyAuthenticationStateChanged()
        {
            this.NotifyAuthenticationStateChanged(this.GetAuthenticationStateAsync());
        }
    }
}
