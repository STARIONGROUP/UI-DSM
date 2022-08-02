// --------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationStatus.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Enumerator
{
    /// <summary>
    /// Enumeration that defines possible status for the authentication process
    /// </summary>
    public enum AuthenticationStatus
    {
        /// <summary>
        /// Defaut status, when no authentication process has been performed 
        /// </summary>
        None,

        /// <summary>
        /// Status when the authentication process is in progress
        /// </summary>
        Authenticating,

        /// <summary>
        /// Status when the authentication process ends succesfully
        /// </summary>
        Success,

        /// <summary>
        /// Status when the authentication process failed
        /// </summary>
        Fail,

        /// <summary>
        /// Status when the target server is not reachable
        /// </summary>
        ServerFailure
    }
}
