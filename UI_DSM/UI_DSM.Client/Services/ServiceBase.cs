// --------------------------------------------------------------------------------------------------------
// <copyright file="ServiceBase.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Services
{
    using System.Text.Json;

    /// <summary>
    ///     Base class for any service that needs to access to the API
    /// </summary>
    public abstract class ServiceBase
    {
        /// <summary>
        ///     The <see cref="HttpClient" /> to access to the server API
        /// </summary>
        protected HttpClient HttpClient;

        /// <summary>
        ///     The <see cref="JsonSerializerOptions" /> that is used for JSON action
        /// </summary>
        protected JsonSerializerOptions JsonSerializerOptions;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServiceBase" /> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient" /></param>
        protected ServiceBase(HttpClient httpClient)
        {
            this.HttpClient = httpClient;
            this.JsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }
    }
}
