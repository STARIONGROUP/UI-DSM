// --------------------------------------------------------------------------------------------------------
// <copyright file="SearchServiceTestFixture.cs" company="RHEA System S.A.">
//  Copyright (c) 2023 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw, Nabil Abbar
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Client.Tests.Services.SearchService
{
    using CDP4JsonSerializer;
    using NUnit.Framework;
    using RichardSzalay.MockHttp;
    using System.Net;
    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Client.Services;
    using UI_DSM.Client.Services.SearchService;
    using UI_DSM.Serializer.Json;
    using UI_DSM.Shared.DTO.Common;

    [TestFixture]
    public class SearchServiceTestFixture
    {
        private ISearchService service;
        private MockHttpMessageHandler httpMessageHandler;
        private IJsonService jsonService;

        [SetUp]
        public void Setup()
        {
            this.httpMessageHandler = new MockHttpMessageHandler();
            var httpClient = this.httpMessageHandler.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost/api");

            ServiceBase.RegisterService<SearchService>();
            this.jsonService = new JsonService(new JsonDeserializer(), new JsonSerializer(), new Cdp4JsonSerializer());
            this.service = new SearchService(httpClient, this.jsonService);
        }

        [Test]
        public async Task VerifySearch()
        {
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.BadRequest;
            var request = this.httpMessageHandler.When(HttpMethod.Get, "/Search");
            request.Respond(_ => httpResponse);
            var response = await this.service.SearchAfter("word");
            Assert.That(response, Is.Empty);

            var search = new List<SearchResultDto>
            {
                new()
            };

            httpResponse.StatusCode = HttpStatusCode.OK;
            httpResponse.Content = new StringContent(this.jsonService.Serialize(search));
            response = await this.service.SearchAfter("word");
            Assert.That(response, Is.Not.Empty);
            response = await this.service.SearchAfter("word");
            Assert.That(response, Is.Empty);
        }
    }
}
