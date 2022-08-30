// --------------------------------------------------------------------------------------------------------
// <copyright file="NewtonSoftNegotiator.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.ResponseNegotiator
{
    using Carter;

    using Microsoft.Extensions.Primitives;
    using Microsoft.Net.Http.Headers;

    using Newtonsoft.Json;

    /// <summary>
    ///     <see cref="IResponseNegotiator" /> to use the Newtonsoft serializer with the <see cref="TypeNameHandling" />
    ///     settings
    /// </summary>
    public class NewtonSoftNegotiator : IResponseNegotiator
    {
        /// <summary>
        ///     The <see cref="JsonSerializerSettings" />
        /// </summary>
        private readonly JsonSerializerSettings settings;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NewtonSoftNegotiator" /> class.
        /// </summary>
        public NewtonSoftNegotiator()
        {
            this.settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
        }

        /// <summary>
        ///     Checks whether any given media types are valid for the current response
        /// </summary>
        /// <param name="accept">
        ///     <see cref="IList{MediaTypeHeaderValue}" />
        /// </param>
        /// <returns>True if any media types are acceptable, false if not</returns>
        public bool CanHandle(MediaTypeHeaderValue accept)
        {
            return accept.MatchesMediaType(new StringSegment("application/json"));
        }

        /// <summary>
        ///     Handles the response utilizing the given view model
        /// </summary>
        /// <param name="req">Current <see cref="HttpRequest" /></param>
        /// <param name="res">Current <see cref="HttpResponse" /></param>
        /// <param name="model">View model</param>
        /// <param name="cancellationToken">
        ///     <see cref="CancellationToken" />
        /// </param>
        /// <returns>
        ///     <see cref="Task" />
        /// </returns>
        public Task Handle(HttpRequest req, HttpResponse res, object model, CancellationToken cancellationToken)
        {
            return res.WriteAsync(JsonConvert.SerializeObject(model, Formatting.Indented, this.settings), cancellationToken);
        }
    }
}
