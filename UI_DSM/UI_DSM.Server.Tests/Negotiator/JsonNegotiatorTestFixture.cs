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
    using System.Text.Json;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Net.Http.Headers;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Serializer.Json;
    using UI_DSM.Server.ResponseNegotiator;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;

    [TestFixture]
    public class JsonNegotiatorTestFixture
    {
        private JsonNegotiator negotiator;
        private Mock<IJsonSerializer> serializer;

        [SetUp]
        public void Setup()
        {
            this.serializer = new Mock<IJsonSerializer>();
            this.negotiator = new JsonNegotiator(this.serializer.Object);
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

        [Test]
        public async Task VerifyHandle()
        {
            var request = new Mock<HttpRequest>();
            var response = new Mock<HttpResponse>();
            var cancellationToken = new CancellationToken();

            await this.negotiator.Handle(request.Object, response.Object, new ProjectDto(), cancellationToken);
            
            this.serializer.Verify(x =>
                x.SerializeAsync(It.IsAny<EntityDto>(), It.IsAny<Stream>(), It.IsAny<JsonWriterOptions>()),
                Times.Once);

            await this.negotiator.Handle(request.Object, response.Object, new List<EntityDto>(), cancellationToken);
            
            this.serializer.Verify(x =>
                    x.SerializeAsync(It.IsAny<IEnumerable<EntityDto>>(), It.IsAny<Stream>(), It.IsAny<JsonWriterOptions>()),
                Times.Once);

            await this.negotiator.Handle(request.Object, response.Object, new EntityRequestResponseDto(), cancellationToken);
            
            this.serializer.Verify(x =>
                    x.SerializeEntityRequestDtoAsync(It.IsAny<EntityRequestResponseDto>(), It.IsAny<Stream>(), It.IsAny<JsonWriterOptions>()),
                Times.Once);

            Assert.That(async ()=> await this.negotiator.Handle(request.Object, response.Object, "45", cancellationToken), Throws.Exception);
        }
    }
}
