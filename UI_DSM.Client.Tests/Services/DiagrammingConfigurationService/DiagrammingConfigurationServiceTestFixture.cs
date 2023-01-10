// --------------------------------------------------------------------------------------------------------
// <copyright file="DiagrammingConfigurationServiceTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Services.DiagrammingConfigurationService
{
    using System.Net;

    using CDP4JsonSerializer;
    
    using NUnit.Framework;
    
    using RichardSzalay.MockHttp;

    using UI_DSM.Client.Services;
    using UI_DSM.Client.Services.DiagrammingConfigurationService;
    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Serializer.Json;
    using UI_DSM.Shared.DTO.Common;

    [TestFixture]
    public class DiagrammingConfigurationServiceTestFixture
    {
        private IDiagrammingConfigurationService service;
        private MockHttpMessageHandler httpMessageHandler;
        private IJsonService jsonService;

        [SetUp]
        public void Setup()
        {
            this.httpMessageHandler = new MockHttpMessageHandler();
            var httpClient = this.httpMessageHandler.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost/api");

            ServiceBase.RegisterService<DiagrammingConfigurationService>();
            this.jsonService = new JsonService(new JsonDeserializer(), new JsonSerializer(), new Cdp4JsonSerializer());
            this.service = new DiagrammingConfigurationService(httpClient, this.jsonService);
        }

        [Test]
        public async Task VerifySaveLayoutConfiguration()
        {
            var projectId = Guid.NewGuid();
            var reviewTaskId = Guid.NewGuid();

            IEnumerable<DiagramNodeDto> diagramLayoutInformationDtos = new List<DiagramNodeDto>
            {
                new()
                {
                    ThingId = Guid.NewGuid(),
                    xPosition = 650,
                    yPosition = 447
                }
            };

            const string configurationName = "config1";
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.NotFound;
            var errors = new List<string> { "Already exist" };

            httpResponse.Content = new StringContent(this.jsonService.Serialize(errors));
            var request = this.httpMessageHandler.When(HttpMethod.Post, $"/Layout/{projectId}/{reviewTaskId}/{configurationName}/Save");
            request.Respond(_ => httpResponse);

            var result = await this.service.SaveDiagramLayout(projectId, reviewTaskId, configurationName, diagramLayoutInformationDtos);
            Assert.That(result.result, Is.False);

            httpResponse.StatusCode = HttpStatusCode.OK;
            result = await this.service.SaveDiagramLayout(projectId, reviewTaskId, configurationName, diagramLayoutInformationDtos);
            Assert.That(result.result, Is.True);
        }

        [Test]
        public async Task VerifyLoadDiagramLayoutConfigurationNames()
        {
            var projectId = Guid.NewGuid();
            var reviewTaskId = Guid.NewGuid();

            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.NotFound;
            var request = this.httpMessageHandler.When(HttpMethod.Get, $"/Layout/{projectId}/{reviewTaskId}/Load");
            request.Respond(_ => httpResponse);

            Assert.That(async () => await this.service.LoadDiagramLayoutConfigurationNames(projectId, reviewTaskId), Throws.Exception);

            httpResponse.StatusCode = HttpStatusCode.OK;
            httpResponse.Content = new StringContent(this.jsonService.Serialize(new List<string> { "config1", "config2" }));
            
            var configurationsName = await this.service.LoadDiagramLayoutConfigurationNames(projectId, reviewTaskId);

            Assert.That(configurationsName, Has.Count.EqualTo(2));
        }

        [Test]
        public async Task VerifyLoadDiagramLayoutConfiguration()
        {
            var projectId = Guid.NewGuid();
            var reviewTaskId = Guid.NewGuid();
            var configurationName = "config1";

            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.NotFound;
            var request = this.httpMessageHandler.When(HttpMethod.Get, $"/Layout/{projectId}/{reviewTaskId}/{configurationName}/Load");
            request.Respond(_ => httpResponse);

            Assert.That(async () => await this.service.LoadDiagramLayoutConfiguration(projectId, reviewTaskId, configurationName), Throws.Exception);

            httpResponse.StatusCode = HttpStatusCode.OK;
            
            httpResponse.Content = new StringContent(this.jsonService.Serialize(new List<DiagramNodeDto>
            {
                new()
                {
                    ThingId = Guid.NewGuid(),
                    xPosition = 650,
                    yPosition = 447
                }
            }));

            var configurationsName = await this.service.LoadDiagramLayoutConfiguration(projectId, reviewTaskId, configurationName);

            Assert.That(configurationsName, Has.Count.EqualTo(1));
        }
    }
}
