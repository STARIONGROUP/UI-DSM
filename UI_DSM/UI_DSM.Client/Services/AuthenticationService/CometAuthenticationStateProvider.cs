// --------------------------------------------------------------------------------------------------------
// <copyright file="Class1.cs" company="RHEA System S.A.">
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
    using System.Security.Claims;

    using CDP4Common.SiteDirectoryData;

    using Microsoft.AspNetCore.Components.Authorization;

    using ReactiveUI;

    using UI_DSM.Client.Services.CometSessionService;

    /// <summary>
    /// Provides information about the authentication state of the current user connected to the Comet Data source.
    /// </summary>
    public class CometAuthenticationStateProvider : AuthenticationStateProvider
    {
        /// <summary>
        /// The <see cref="ICometSessionService" />
        /// </summary>
        private readonly ICometSessionService cometSessionService;

        /// <summary>
        /// Initializes a new instance of <see cref="CometAuthenticationStateProvider" />
        /// </summary>
        /// <param name="cometSessionService">The <see cref="ICometSessionService"/></param>
        public CometAuthenticationStateProvider(ICometSessionService cometSessionService)
        {
            this.cometSessionService = cometSessionService;

            this.WhenAnyValue(x => x.cometSessionService.IsSessionOpen)
                .Subscribe(_ => this.NotifyAuthenticationStateChanged(this.GetAuthenticationStateAsync()));
        }

        /// <summary>
        /// Asynchronously gets an <see cref="AuthenticationState" /> that describes the current user.
        /// </summary>
        /// <returns>A task that, when resolved, gives an <see cref="AuthenticationState" /> instance that describes the current user.</returns>
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var identity = this.cometSessionService.IsSessionOpen
                ? this.CreateClaimsIdentity(this.cometSessionService.Session!.ActivePerson)
                : new ClaimsIdentity();

            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
        }

        /// <summary>
        /// Creates a <see cref="ClaimsIdentity"/> based on a <see cref="Person" />
        /// </summary>
        /// <param name="person">The <see cref="Person" /> used to created the <see cref="ClaimsIdentity"/></param>
        /// <returns>The created <see cref="ClaimsIdentity"/></returns>
        private ClaimsIdentity CreateClaimsIdentity(Person person)
        {
            if (person.Name == null || !person.IsActive || person.IsDeprecated)
            {
                return new ClaimsIdentity();
            }

            var claims = new List<Claim>()
            {
                new(ClaimTypes.Name, person.Name)
            };

            return new ClaimsIdentity(claims, "Comet Authentication");
        }
    }
}
