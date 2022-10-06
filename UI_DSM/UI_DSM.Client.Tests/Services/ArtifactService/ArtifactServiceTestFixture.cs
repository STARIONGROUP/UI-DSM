// --------------------------------------------------------------------------------------------------------
// <copyright file="ArtifactServiceTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Services.ArtifactService
{
    using NUnit.Framework;

    using RichardSzalay.MockHttp;

    using UI_DSM.Client.Services;
    using UI_DSM.Client.Services.ArtifactService;
    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Serializer.Json;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.Extensions;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class ArtifactServiceTestFixture
    {
        private ArtifactService service;
        private MockHttpMessageHandler httpMessageHandler;
        private JsonService jsonService;

        [SetUp]
        public void Setup()
        {
            this.httpMessageHandler = new MockHttpMessageHandler();
            var httpClient = this.httpMessageHandler.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost/api");

            ServiceBase.RegisterService<ArtifactService>();

            this.jsonService = new JsonService(new JsonDeserializer(), new JsonSerializer());
            this.service = new ArtifactService(httpClient, this.jsonService);
            EntityHelper.RegisterEntities();
        }

        [Test]
        public async Task VerifyUploadModel()
        {
            var projectId = Guid.NewGuid();
            var fileName = $"{Guid.NewGuid()}.zip";
            var modelName = "A Model - Iteration 1";

            var httpResponse = new HttpResponseMessage();

            var entityRequestResponse = new EntityRequestResponseDto()
            {
                IsRequestSuccessful = false
            };

            httpResponse.Content = new StringContent(this.jsonService.Serialize(entityRequestResponse));
            var request = this.httpMessageHandler.When(HttpMethod.Post, $"/Project/{projectId}/Artifact/Create");
            request.Respond(_ => httpResponse);

            var requestResponse = await this.service.UploadModel(projectId, fileName, modelName);
            Assert.That(requestResponse.IsRequestSuccessful, Is.False);

            entityRequestResponse.IsRequestSuccessful = true;

            entityRequestResponse.Entities = new Model().GetAssociatedEntities().ToDtos();

            httpResponse.Content = new StringContent(this.jsonService.Serialize(entityRequestResponse));

            requestResponse = await this.service.UploadModel(projectId, fileName, modelName);

            Assert.Multiple(() =>
            {
                Assert.That(requestResponse.IsRequestSuccessful, Is.True);
                Assert.That(requestResponse.Entity, Is.Not.Null);
            });

            httpResponse.Content = new StringContent(string.Empty);

            Assert.That(async () => await this.service.UploadModel(projectId, fileName, modelName), Throws.Exception);
        }
    }
}
