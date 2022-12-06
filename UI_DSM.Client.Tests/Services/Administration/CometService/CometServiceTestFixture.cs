// --------------------------------------------------------------------------------------------------------
// <copyright file="CometServiceTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Services.Administration.CometService
{
    using System.Net;

    using CDP4JsonSerializer;

    using Microsoft.AspNetCore.Components.Forms;
    
    using Moq;
    
    using NUnit.Framework;

    using RichardSzalay.MockHttp;

    using UI_DSM.Client.Services;
    using UI_DSM.Client.Services.Administration.CometService;
    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Serializer.Json;
    using UI_DSM.Shared.DTO.CometData;

    [TestFixture]
    public class CometServiceTestFixture
    {
        private CometService service;
        private MockHttpMessageHandler httpMessageHandler;
        private IJsonService jsonService;

        [SetUp]
        public void Setup()
        {
            this.httpMessageHandler = new MockHttpMessageHandler();
            var httpClient = this.httpMessageHandler.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost/api");

            ServiceBase.RegisterService<CometService>();
            this.jsonService = new JsonService(new JsonDeserializer(), new JsonSerializer(), new Cdp4JsonSerializer());
            this.service = new CometService(httpClient, this.jsonService);
        }

        [Test]
        public async Task VerifyLogin()
        {
            var httpResponse = new HttpResponseMessage();
            var request = this.httpMessageHandler.When(HttpMethod.Post, "/Comet/Login");

            var cometAuthentication = new CometAuthenticationData();

            request.Respond(_ => httpResponse);
            Assert.That(async () => await this.service.Login(cometAuthentication), Throws.Exception);

            httpResponse.Content = new StringContent(this.jsonService.Serialize(new CometAuthenticationResponse()
            {
                IsRequestSuccessful = false
            }));

            Assert.That((await this.service.Login(cometAuthentication)).IsRequestSuccessful, Is.False);

            httpResponse.Content = new StringContent(this.jsonService.Serialize(new CometAuthenticationResponse()
            {
                SessionId = Guid.NewGuid(),
                IsRequestSuccessful = true
            }));

            Assert.That((await this.service.Login(cometAuthentication)).IsRequestSuccessful, Is.True);
        }

        [Test]
        public async Task VerifyLogout()
        {
            var sessionId = Guid.NewGuid();
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.NotFound;
            var request = this.httpMessageHandler.When(HttpMethod.Delete, $"/Comet/{sessionId}");
            request.Respond(_ => httpResponse);

            Assert.That(await this.service.Logout(sessionId), Is.False);

            httpResponse.StatusCode = HttpStatusCode.OK;
            Assert.That(await this.service.Logout(sessionId), Is.True);
        }

        [Test]
        public async Task VerifyGetAvailableEngineeringModels()
        {
            var sessionId = Guid.NewGuid();
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.NotFound;
            var request = this.httpMessageHandler.When(HttpMethod.Get, $"/Comet/{sessionId}/Models");
            request.Respond(_ => httpResponse);

            Assert.That(async () => await this.service.GetAvailableEngineeringModels(sessionId), Throws.Exception);

            httpResponse.StatusCode = HttpStatusCode.OK;

            var modelDataResponse = new ModelsDataResponse()
            {
                IsRequestSuccessful = true,
            };

            httpResponse.Content = new StringContent(this.jsonService.Serialize(modelDataResponse));
            Assert.That((await this.service.GetAvailableEngineeringModels(sessionId)).IsRequestSuccessful, Is.True);
        }

        [Test]
        public async Task VerifyUploadIteration()
        {
            var sessionId = Guid.NewGuid();
            var httpResponse = new HttpResponseMessage();
            var request = this.httpMessageHandler.When(HttpMethod.Post, $"/Comet/{sessionId}/Models/Upload");
            request.Respond(_ => httpResponse);

            var modelUploadResponse = new ModelUploadResponse()
            {
                IsRequestSuccessful = true
            };

            httpResponse.Content = new StringContent(this.jsonService.Serialize(modelUploadResponse));
            Assert.That((await this.service.UploadIteration(sessionId, Guid.NewGuid(),Guid.NewGuid())).IsRequestSuccessful, Is.True);
        }

        [Test]
        public async Task VerifyUploadAnnexC3File()
        {
            var browserFile = new Mock<IBrowserFile>();
            browserFile.Setup(x => x.Size).Returns(512000 * 3);

            var authenticationData = new CometAuthenticationData()
            {
                UploadFromFile = true,
                UserName = "admin",
                Password = "pass"
            };

            var cometAuthenticationResponse = await this.service.UploadAnnexC3File(authenticationData, browserFile.Object);
            Assert.That(cometAuthenticationResponse.Errors, Is.Not.Empty);

            browserFile.Setup(x => x.Size).Returns(512000);
            browserFile.Setup(x => x.ContentType).Returns("application/x-zip-compressed");
            browserFile.Setup(x => x.Name).Returns("LOFT.zip");
            browserFile.Setup(x => x.OpenReadStream(It.IsAny<long>(), default)).Returns(new MemoryStream());

            var httpResponse = new HttpResponseMessage();
            var request = this.httpMessageHandler.When(HttpMethod.Post, $"/Comet/Upload");
            request.Respond(_ => httpResponse);

            httpResponse.Content = new StringContent(this.jsonService.Serialize(new CometAuthenticationResponse()
            {
                IsRequestSuccessful = false
            }));

            cometAuthenticationResponse = await this.service.UploadAnnexC3File(authenticationData, browserFile.Object);

            Assert.That(cometAuthenticationResponse.IsRequestSuccessful, Is.False);

            httpResponse.Content = new StringContent(this.jsonService.Serialize(new CometAuthenticationResponse()
            {
                SessionId = Guid.NewGuid(),
                IsRequestSuccessful = true
            }));

            cometAuthenticationResponse = await this.service.UploadAnnexC3File(authenticationData, browserFile.Object);

            Assert.That(cometAuthenticationResponse.IsRequestSuccessful, Is.True);
        }
    }
}
