// --------------------------------------------------------------------------------------------------------
// <copyright file="SearchServiceTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Tests.Services
{
    using System.Net;

    using CDP4Common.EngineeringModelData;

    using NUnit.Framework;

    using RichardSzalay.MockHttp;

    using UI_DSM.Server.Services.SearchService;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class SearchServiceTestFixture
    {
        private SearchService service;
        private MockHttpMessageHandler httpMessageHandler;

        [SetUp]
        public void Setup()
        {
            this.httpMessageHandler = new MockHttpMessageHandler();
            var httpClient = this.httpMessageHandler.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost/api");
            this.service = new SearchService(httpClient);
        }

        [Test]
        public void VerifySearchAfter()
        {
            var httpResponse = new HttpResponseMessage();

            var request = this.httpMessageHandler.When("/GP.SearchService/Search");
            request.Respond(_ => httpResponse);

            Assert.That(async () => await this.service.SearchAfter("something"), Is.Not.Null);
        }

        [Test]
        public async Task VerifyIndexData()
        {
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.BadRequest;

            var request = this.httpMessageHandler.When("/GP.SearchService/Index/CommentDto");
            request.Respond(_ => httpResponse);
            var comment = new CommentDto();

            var indexResult = await this.service.IndexData(comment);
            Assert.That(indexResult, Is.False);
            httpResponse.StatusCode = HttpStatusCode.OK;
            comment.Id = Guid.NewGuid();
            indexResult = await this.service.IndexData(comment);
            Assert.That(indexResult, Is.True);
        }

        [Test]
        public async Task VerifyIndexDataEnumerable()
        {
            var httpResponse = new HttpResponseMessage();

            var request = this.httpMessageHandler.When("/GP.SearchService/Index/CommentDtos");
            request.Respond(_ => httpResponse);
            var comments = new List<CommentDto>();

            var indexResult = await this.service.IndexData(comments);
            Assert.That(indexResult, Is.False);
            httpResponse.StatusCode = HttpStatusCode.BadRequest;
            comments.Add(new CommentDto());
            
            indexResult = await this.service.IndexData(comments);
            Assert.That(indexResult, Is.False);

            httpResponse.StatusCode = HttpStatusCode.OK;
            indexResult = await this.service.IndexData(comments);
            Assert.That(indexResult, Is.True);
        }

        [Test]
        public async Task VerifyDeleteIndexedData()
        {
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.BadRequest;
            var commentGuid = Guid.NewGuid();
            var request = this.httpMessageHandler.When(HttpMethod.Delete, $"/GP.SearchService/Index/CommentDto/{commentGuid}");
            request.Respond(_ => httpResponse);
            var commentDto = new CommentDto(commentGuid);
            var deleteResult = await this.service.DeleteIndexedData(commentDto);
            Assert.That(deleteResult, Is.False);
            httpResponse.StatusCode = HttpStatusCode.OK;
            deleteResult = await this.service.DeleteIndexedData(commentDto);
            Assert.That(deleteResult, Is.True);
        }

        [Test]
        public async Task VerifyIndexIteration()
        {
            var iteration = new Iteration();

            var elementDefinition = new ElementDefinition()
            {
                Iid = Guid.NewGuid()
            };

            var usage = new ElementUsage()
            {
                Iid = Guid.NewGuid(),
                ElementDefinition = elementDefinition
            };
        }
    }
}
