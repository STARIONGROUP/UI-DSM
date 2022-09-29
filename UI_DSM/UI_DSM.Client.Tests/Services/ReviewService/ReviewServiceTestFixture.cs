// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewServiceTestFixture.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Client.Tests.Services.ReviewService
{
    using System.Net;

    using NUnit.Framework;

    using RichardSzalay.MockHttp;

    using UI_DSM.Client.Services;
    using UI_DSM.Client.Services.JsonDeserializerProvider;
    using UI_DSM.Client.Services.ReviewService;
    using UI_DSM.Serializer.Json;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Extensions;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class ReviewServiceTestFixture
    {
        private ReviewService service;
        private MockHttpMessageHandler httpMessageHandler;
        private List<EntityDto> entitiesDto;
        private IJsonService jsonService;

        [SetUp]
        public void Setup()
        {
            this.httpMessageHandler = new MockHttpMessageHandler();
            var httpClient = this.httpMessageHandler.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost/api");

            var participantId = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            this.entitiesDto = new List<EntityDto>
            {
                new ReviewDto(Guid.NewGuid())
                {
                    Author = participantId,
                    CreatedOn = DateTime.UtcNow,
                    Description = "A review",
                    Title = "Review Title",
                    ReviewNumber = 1
                },
                new ParticipantDto(participantId)
                {
                    Role = roleId,
                    User = userId
                },
                new RoleDto(roleId)
                {
                    RoleName = "Reviewer"
                },
                new UserEntityDto(userId)
                {
                    UserName = "user"
                }
            };

            ServiceBase.RegisterService<ReviewService>();
            this.jsonService = new JsonService(new JsonDeserializer(), new JsonSerializer());
            this.service = new ReviewService(httpClient, this.jsonService);
        }

        [Test]
        public async Task VerifyGetReviews()
        {
            var projectId = Guid.NewGuid();
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.NotFound;

            var request = this.httpMessageHandler.When($"/Project/{projectId}/Review");
            request.Respond(_ => httpResponse);

            Assert.That(async () => await this.service.GetReviewsOfProject(projectId), Throws.Exception);
            httpResponse.StatusCode = HttpStatusCode.OK;

            httpResponse.Content = new StringContent(this.jsonService.Serialize(this.entitiesDto));
            var reviews = await this.service.GetReviewsOfProject(projectId);
            Assert.That(reviews, Has.Count.EqualTo(1));
            httpResponse.Content = new StringContent(string.Empty);
            Assert.That(async () => await this.service.GetReviewsOfProject(projectId), Throws.Exception);
        }

        [Test]
        public async Task VerifyGetReview()
        {
            var projectId = Guid.NewGuid();
            var guid = this.entitiesDto.First().Id;
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.NotFound;

            var request = this.httpMessageHandler.When(HttpMethod.Get, $"/Project/{projectId}/Review/{guid}");
            request.Respond(_ => httpResponse);
            var review = await this.service.GetReviewOfProject(projectId, guid);

            Assert.That(review, Is.Null);

            httpResponse.StatusCode = HttpStatusCode.OK;

            httpResponse.Content = new StringContent(this.jsonService.Serialize(this.entitiesDto));
            review = await this.service.GetReviewOfProject(projectId, guid);
            Assert.That(review.Id, Is.EqualTo(guid));

            httpResponse.Content = new StringContent(string.Empty);

            Assert.That(async () => await this.service.GetReviewOfProject(projectId, guid), Throws.Exception);
        }

        [Test]
        public async Task VerifyCreateReview()
        {
            var review = new Review()
            {
                Description = "Review description",
                Title = "Review title"
            };

            var projectId = Guid.NewGuid();

            var httpResponse = new HttpResponseMessage();

            var entityRequestResponse = new EntityRequestResponseDto()
            {
                IsRequestSuccessful = false
            };

            httpResponse.Content = new StringContent(this.jsonService.Serialize(entityRequestResponse));
            var request = this.httpMessageHandler.When(HttpMethod.Post, $"/Project/{projectId}/Review/Create");
            request.Respond(_ => httpResponse);

            var requestResponse = await this.service.CreateReview(projectId, review);
            Assert.That(requestResponse.IsRequestSuccessful, Is.False);

            entityRequestResponse.IsRequestSuccessful = true;

            entityRequestResponse.Entities = review.GetAssociatedEntities().ToDtos();

            httpResponse.Content = new StringContent(this.jsonService.Serialize(entityRequestResponse));

            requestResponse = await this.service.CreateReview(projectId, review);

            Assert.Multiple(() =>
            {
                Assert.That(requestResponse.IsRequestSuccessful, Is.True);
                Assert.That(requestResponse.Entity, Is.Not.Null);
            });

            httpResponse.Content = new StringContent(string.Empty);

            Assert.That(async () => await this.service.CreateReview(projectId, review), Throws.Exception);
        }

        [Test]
        public async Task VerifyDeleteReview()
        {
            var review = new Review(Guid.NewGuid())
            {
                Description = "Review description",
                Title = "Review title", 
                ReviewNumber = 1,
                Author = new Participant(Guid.NewGuid())
            };

            var project = new Project(Guid.NewGuid());

            var httpResponse = new HttpResponseMessage();
            var requestResponse = new RequestResponseDto() { IsRequestSuccessful = true };
            httpResponse.Content = new StringContent(this.jsonService.Serialize(requestResponse));
            var request = this.httpMessageHandler.When(HttpMethod.Delete, $"/Project/{project.Id}/Review/{review.Id}");
            request.Respond(_ => httpResponse);

            Assert.That(async () => await this.service.DeleteReview(review), Throws.Exception);

            project.Reviews.Add(review);
            var result = await this.service.DeleteReview(review);
            Assert.That(result.IsRequestSuccessful, Is.True);

            httpResponse.Content = new StringContent(string.Empty);
            Assert.That(async () => await this.service.DeleteReview(review), Throws.Exception);
        }

        [Test]
        public async Task VerifyUpdateReview()
        {
            var review = new Review(Guid.NewGuid())
            {
                Description = "Review description",
                Title = "Review title",
                ReviewNumber = 1,
                Author = new Participant(Guid.NewGuid())
                {
                    Role = new Role(Guid.NewGuid()),
                    User = new UserEntity(Guid.NewGuid())
                }
            };

            var project = new Project(Guid.NewGuid());

            var httpResponse = new HttpResponseMessage();
            var requestResponse = new EntityRequestResponseDto() { IsRequestSuccessful = false };
            httpResponse.Content = new StringContent(this.jsonService.Serialize(requestResponse));

            var request = this.httpMessageHandler.When(HttpMethod.Put, $"/Project/{project.Id}/Review/{review.Id}");
            request.Respond(_ => httpResponse);

            Assert.That(async () => await this.service.UpdateReview(review), Throws.Exception);
            project.Reviews.Add(review);

            var requestResult = await this.service.UpdateReview(review);
            Assert.That(requestResult.IsRequestSuccessful, Is.False);

            requestResponse.IsRequestSuccessful = true;

            requestResponse.Entities = review.GetAssociatedEntities().ToDtos();

            httpResponse.Content = new StringContent(this.jsonService.Serialize(requestResponse));

            requestResult = await this.service.UpdateReview(review);
            Assert.That(requestResult.IsRequestSuccessful, Is.True);
            httpResponse.Content = new StringContent(string.Empty);
            Assert.That(async () => await this.service.UpdateReview(review), Throws.Exception);
        }
    }
}
