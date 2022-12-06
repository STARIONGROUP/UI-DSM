// --------------------------------------------------------------------------------------------------------
// <copyright file="JsonNegotiatorTestFixture.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw, Nabil Abbar
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Server.Tests.Negotiator
{
    using CDP4JsonSerializer;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Primitives;
    using Microsoft.Net.Http.Headers;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Serializer.Json;
    using UI_DSM.Server.ResponseNegotiator;
    using UI_DSM.Shared.DTO.Models;

    [TestFixture]
    public class JsonNegotiatorTestFixture
    {
        private JsonNegotiator negotiator;
        private IJsonService serializer;

        [SetUp]
        public void Setup()
        {
            this.serializer = new JsonService(new JsonDeserializer(), new JsonSerializer(), new Cdp4JsonSerializer());
            this.negotiator = new JsonNegotiator(this.serializer);
        }

        [Test]
        public void VerifyCanHandle()
        {
            Assert.Multiple(() =>
            {
                Assert.That(this.negotiator.CanHandle(new MediaTypeHeaderValue("application/json")), Is.True);
                Assert.That(this.negotiator.CanHandle(new MediaTypeHeaderValue("application/xml")), Is.False);
            });
        }
    }
}
