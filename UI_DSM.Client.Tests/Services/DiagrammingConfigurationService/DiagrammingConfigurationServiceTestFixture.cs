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
            IEnumerable<DiagramLayoutInformationDto> diagramLayoutInformationDtos = new List<DiagramLayoutInformationDto>
            {
                new DiagramLayoutInformationDto
                {
                    ThingId = Guid.NewGuid(),
                    xPosition = 650,
                    yPosition = 447
                }
            };

            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.NotFound;
            var request = this.httpMessageHandler.When(HttpMethod.Post, $"/Layout/{projectId}/{reviewTaskId}/Save");
            request.Respond(_ => httpResponse);
            
            Assert.That(await this.service.SaveDiagramLayout(projectId, reviewTaskId, diagramLayoutInformationDtos), Is.False);

            httpResponse.StatusCode = HttpStatusCode.OK;
            Assert.That(await this.service.SaveDiagramLayout(projectId, reviewTaskId, diagramLayoutInformationDtos), Is.True);
        }
    }
}
