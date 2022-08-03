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
    using CDP4Dal;
    using CDP4Dal.DAL;
    using CDP4Dal.Exceptions;

    using UI_DSM.Client.Enumerator;
    using UI_DSM.Client.Services.AuthenticationService.Dto;
    using UI_DSM.Client.Services.CometSessionService;

    /// <summary>
    /// The <see cref="AuthenticationService" /> allow the user to conenct to an E-TM-10-25 data source
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        /// <summary>
        /// The <see cref="ICometSessionService"/>
        /// </summary>
        private readonly ICometSessionService cometSessionService;

        /// <summary>Initializes a new instance of the <see cref="AuthenticationService" /> class.</summary>
        /// <param name="cometSessionService">The <see cref="ICometSessionService"/></param>
        public AuthenticationService(ICometSessionService cometSessionService)
        {
            this.cometSessionService = cometSessionService;
        }

        /// <summary>
        /// Login to a data source with the provided <see cref="AuthenticationDto"/>
        /// </summary>
        /// <param name="authentication">The <see cref="AuthenticationDto"/> to use to authenticate</param>
        /// <returns>A <see cref="Task"/> with a <see cref="AuthenticationStatus.Success"/> result when a <see cref="ISession"/> has been fully opened</returns>
        public async Task<AuthenticationStatus> Login(AuthenticationDto authentication)
        {
            if (authentication.SourceAddress == null)
            {
                return AuthenticationStatus.Fail;
            }

            try
            {
                var uri = new Uri(authentication.SourceAddress);
                var credentials = new Credentials(authentication.UserName, authentication.Password, uri);

                return await this.cometSessionService.Open(credentials) ? AuthenticationStatus.Success : AuthenticationStatus.Fail;
            }
            catch (DalReadException)
            {
                await this.cometSessionService.Close();
                return AuthenticationStatus.Fail;
            }
            catch (HttpRequestException)
            {
                await this.cometSessionService.Close();
                return AuthenticationStatus.ServerFailure;
            }
        }

        /// <summary>
        /// Logout from the data source
        /// </summary>
        /// <returns>
        /// a <see cref="Task"/>
        /// </returns>
        public async Task Logout()
        {
            await this.cometSessionService.Close();
        }
    }
}
