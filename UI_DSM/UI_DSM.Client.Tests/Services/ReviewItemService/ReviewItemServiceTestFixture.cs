// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewItemServiceTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Services.ReviewItemService
{
    using System.Net;

    using NUnit.Framework;

    using RichardSzalay.MockHttp;

    using UI_DSM.Client.Services;
    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Client.Services.ReviewItemService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Extensions;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class ReviewItemServiceTestFixture
    {
        private ReviewItemService service;
        private MockHttpMessageHandler httpMessageHandler;
        private IJsonService jsonService;
        private List<EntityDto> entitiesDto;

        [SetUp]
        public void Setup()
        {
            this.httpMessageHandler = new MockHttpMessageHandler();
            var httpClient = this.httpMessageHandler.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost/api");
            EntityHelper.RegisterEntities();

            this.entitiesDto = new List<EntityDto>
            {
                new ReviewItemDto(Guid.NewGuid())
                {
                    ThingId = Guid.NewGuid()
                }
            };

            ServiceBase.RegisterService<ReviewItemService>();
            this.jsonService = JsonSerializerHelper.CreateService();
            this.service = new ReviewItemService(httpClient, this.jsonService);
        }

        [Test]
        public async Task VerifyGetReviewItems()
        {
            var projectId = Guid.NewGuid();
            var reviewId = Guid.NewGuid();
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.NotFound;

            var request = this.httpMessageHandler.When($"/Project/{projectId}/Review/{reviewId}/ReviewItem");
            request.Respond(_ => httpResponse);

            Assert.That(async () => await this.service.GetReviewItemsOfReview(projectId, reviewId), Throws.Exception);
            httpResponse.StatusCode = HttpStatusCode.OK;

            httpResponse.Content = new StringContent(this.jsonService.Serialize(this.entitiesDto));
            var reviewItems = await this.service.GetReviewItemsOfReview(projectId, reviewId);
            Assert.That(reviewItems, Has.Count.EqualTo(1));
            httpResponse.Content = new StringContent(string.Empty);
            Assert.That(async () => await this.service.GetReviewItemsOfReview(projectId, reviewId), Throws.Exception);
        }

        [Test]
        public async Task VerifyGetReviewItem()
        {
            var projectId = Guid.NewGuid();
            var reviewId = Guid.NewGuid();
            var guid = this.entitiesDto.First().Id;
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.NotFound;

            var request = this.httpMessageHandler.When(HttpMethod.Get, $"/Project/{projectId}/Review/{reviewId}/ReviewItem/{guid}");
            request.Respond(_ => httpResponse);
            var reviewItem = await this.service.GetReviewItemOfReview(projectId, reviewId, guid);

            Assert.That(reviewItem, Is.Null);

            httpResponse.StatusCode = HttpStatusCode.OK;

            httpResponse.Content = new StringContent(this.jsonService.Serialize(this.entitiesDto));
            reviewItem = await this.service.GetReviewItemOfReview(projectId, reviewId, guid);
            Assert.That(reviewItem.Id, Is.EqualTo(guid));

            httpResponse.Content = new StringContent(string.Empty);

            Assert.That(async () => await this.service.GetReviewItemOfReview(projectId, reviewId, guid), Throws.Exception);
        }

        [Test]
        public async Task VerifyCreateReviewItem()
        {
            var projectId = Guid.NewGuid();
            var reviewId = Guid.NewGuid();
            var thingId = Guid.NewGuid();

            var httpResponse = new HttpResponseMessage();

            var entityRequestResponse = new EntityRequestResponseDto()
            {
                IsRequestSuccessful = false
            };

            httpResponse.Content = new StringContent(this.jsonService.Serialize(entityRequestResponse));
            var request = this.httpMessageHandler.When(HttpMethod.Post, $"/Project/{projectId}/Review/{reviewId}/ReviewItem/Create");
            request.Respond(_ => httpResponse);

            var requestResponse = await this.service.CreateReviewItem(projectId, reviewId, thingId);
            Assert.That(requestResponse.IsRequestSuccessful, Is.False);

            entityRequestResponse.IsRequestSuccessful = true;

            entityRequestResponse.Entities = new ReviewItem().GetAssociatedEntities().ToDtos();

            httpResponse.Content = new StringContent(this.jsonService.Serialize(entityRequestResponse));

            requestResponse = await this.service.CreateReviewItem(projectId, reviewId, thingId);

            Assert.Multiple(() =>
            {
                Assert.That(requestResponse.IsRequestSuccessful, Is.True);
                Assert.That(requestResponse.Entity, Is.Not.Null);
            });

            httpResponse.Content = new StringContent(string.Empty);

            Assert.That(async () => await this.service.CreateReviewItem(projectId, reviewId, thingId), Throws.Exception);
        }

        [Test]
        public async Task VerifyGetReviewItemForThing()
        {
            var projectId = Guid.NewGuid();
            var reviewId = Guid.NewGuid();
            var guid = ((ReviewItemDto)this.entitiesDto.First()).ThingId;
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.NotFound;

            var request = this.httpMessageHandler.When(HttpMethod.Get, $"/Project/{projectId}/Review/{reviewId}/ReviewItem/ForThing/{guid}");
            request.Respond(_ => httpResponse);
            var reviewItem = await this.service.GetReviewItemForThing(projectId, reviewId, guid);

            Assert.That(reviewItem, Is.Null);

            httpResponse.StatusCode = HttpStatusCode.OK;

            httpResponse.Content = new StringContent(this.jsonService.Serialize(this.entitiesDto));
            reviewItem = await this.service.GetReviewItemForThing(projectId, reviewId, guid);
            Assert.That(reviewItem.ThingId, Is.EqualTo(guid));

            httpResponse.Content = new StringContent(string.Empty);

            Assert.That(async () => await this.service.GetReviewItemForThing(projectId, reviewId, guid), Throws.Exception);
        }

        [Test]
        public async Task VerifyGetReviewItemsForThings()
        {
            var projectId = Guid.NewGuid();
            var reviewId = Guid.NewGuid();
            var guid = new List<Guid>{((ReviewItemDto)this.entitiesDto.First()).ThingId};
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.NotFound;

            var request = this.httpMessageHandler.When(HttpMethod.Post, $"/Project/{projectId}/Review/{reviewId}/ReviewItem/ForThings");
            request.Respond(_ => httpResponse);
            var reviewItems = await this.service.GetReviewItemsForThings(projectId, reviewId, guid);

            Assert.That(reviewItems, Is.Empty);

            httpResponse.StatusCode = HttpStatusCode.OK;

            httpResponse.Content = new StringContent(this.jsonService.Serialize(this.entitiesDto));
            reviewItems = await this.service.GetReviewItemsForThings(projectId, reviewId, guid);
            Assert.That(reviewItems.First().ThingId, Is.EqualTo(guid.First()));

            httpResponse.Content = new StringContent(string.Empty);

            Assert.That(async () => await this.service.GetReviewItemsForThings(projectId, reviewId, guid), Throws.Exception);
        }
    }
}
