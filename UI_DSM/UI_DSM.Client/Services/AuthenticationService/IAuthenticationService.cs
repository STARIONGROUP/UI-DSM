// --------------------------------------------------------------------------------------------------------
// <copyright file="IAuthenticationService.cs" company="RHEA System S.A.">
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
    using UI_DSM.Shared.DTO;

    /// <summary>
    ///     Interface definition for <see cref="AuthenticationService" />
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        ///     Login to the UI-DSM data source with the provided <see cref="AuthenticationDto" />
        /// </summary>
        /// <param name="authentication">The <see cref="AuthenticationDto" /> to use to authenticate</param>
        /// <returns>A <see cref="Task" /> with a <see cref="AuthenticationResponseDto" /> result </returns>
        Task<AuthenticationResponseDto> Login(AuthenticationDto authentication);

        /// <summary>
        ///     Logout from the data source
        /// </summary>
        /// <returns>
        ///     a <see cref="Task" />
        /// </returns>
        Task Logout();
    }
}
