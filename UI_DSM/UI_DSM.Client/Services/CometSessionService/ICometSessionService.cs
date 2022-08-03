// --------------------------------------------------------------------------------------------------------
// <copyright file="ICometSessionService.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Services.CometSessionService
{
    using CDP4Dal;
    using CDP4Dal.DAL;

    /// <summary>
    /// Interface definition for <see cref="CometSessionService"/>
    /// </summary>
    public interface ICometSessionService
    {
        /// <summary>
        /// Gets or sets the <see cref="ISession"/>
        /// </summary>
        ISession? Session { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if an <see cref="ISession"/> is opened
        /// </summary>
        bool IsSessionOpen { get; set; }

        /// <summary>
        /// Close the <see cref="ISession"/>
        /// </summary>
        /// <returns>A <see cref="Task"/></returns>
        Task Close();

        /// <summary>
        /// Opens an <see cref="ISession"/>
        /// </summary>
        /// <param name="credentials"></param>
        /// <returns></returns>
        Task<bool> Open(Credentials credentials);
    }
}
