// --------------------------------------------------------------------------------------------------------
// <copyright file="RouteParserServiceTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Services.RouteParserService
{
    using System.Net;

    using CDP4JsonSerializer;
  
    using NUnit.Framework;
    
    using RichardSzalay.MockHttp;
    
    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Client.Services;
    using UI_DSM.Client.Services.RouteParserService;
    using UI_DSM.Serializer.Json;
    using UI_DSM.Shared.DTO.Common;

    [TestFixture]
    public class RouteParserServiceTestFixture
    {
        private RouteParserService service;
        private MockHttpMessageHandler httpMessageHandler;
        private IJsonService jsonService;

        [SetUp]
        public void Setup()
        {
            this.httpMessageHandler = new MockHttpMessageHandler();
            var httpClient = this.httpMessageHandler.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost/api");

            ServiceBase.RegisterService<RouteParserService>();
            this.jsonService = new JsonService(new JsonDeserializer(), new JsonSerializer(), new Cdp4JsonSerializer());
            this.service = new RouteParserService(httpClient, this.jsonService);
        }

        [Test]
        public async Task VerifyLogin()
        {
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.BadRequest;
            var request = this.httpMessageHandler.When(HttpMethod.Get, "/RouteParser");

            var parsedUrl = new List<ParsedUrlDto>()
            {
                new ("Home", "/")
            };

            request.Respond(_ => httpResponse);
            var response = await this.service.ParseUrl("/Project");
            Assert.That(response, Is.Empty);
            httpResponse.StatusCode = HttpStatusCode.OK;

            httpResponse.Content = new StringContent(this.jsonService.Serialize(parsedUrl));
            response = await this.service.ParseUrl("/Project");
            Assert.That(response, Has.Count.EqualTo(1));
            Assert.That(async () => await this.service.ParseUrl("/"), Throws.Exception);
        }
    }
}
