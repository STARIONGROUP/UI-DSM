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

    using UI_DSM.Client.Services.JsonService;

    /// <summary>
    ///     <see cref="IResponseNegotiator" /> to use the custom Json serializer
    /// </summary>
    public class JsonNegotiator : IResponseNegotiator
    {
        /// <summary>
        ///     The <see cref="IJsonService" />
        /// </summary>
        private readonly IJsonService jsonService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="JsonNegotiator" /> class.
        /// </summary>
        public JsonNegotiator(IJsonService jsonService)
        {
            this.jsonService = jsonService;
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
            res.Headers.ContentType = "application/json";
            return res.WriteAsync(this.jsonService.Serialize(model), cancellationToken);
        }
    }
}
