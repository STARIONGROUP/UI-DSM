// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewObjectiveServiceTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Services.ReviewObjectiveService
{
    using System.Net;

    using NUnit.Framework;
    
    using RichardSzalay.MockHttp;

    using UI_DSM.Client.Services;
    using UI_DSM.Client.Services.ReviewObjectiveService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Extensions;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class ReviewObjectiveServiceTestFixture
    {
        private ReviewObjectiveService service;
        private MockHttpMessageHandler httpMessageHandler;
        private List<EntityDto> entitiesDto;

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
                new ReviewObjectiveDto(Guid.NewGuid())
                {
                    Author = participantId,
                    CreatedOn = DateTime.UtcNow,
                    Description = "A review",
                    Title = "Review Title",
                    ReviewObjectiveNumber = 1,
                    Status = StatusKind.Open
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
                new UserDto(userId)
                {
                    UserName = "user"
                }
            };

            ServiceBase.RegisterService<ReviewObjectiveService>();
            this.service = new ReviewObjectiveService(httpClient);
        }

        [Test]
        public async Task VerifyGetReviewObjectives()
        {
            var projectId = Guid.NewGuid();
            var reviewId = Guid.NewGuid();
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.NotFound;

            var request = this.httpMessageHandler.When($"/Project/{projectId}/Review/{reviewId}/ReviewObjective");
            request.Respond(_ => httpResponse);

            Assert.That(async () => await this.service.GetReviewsObjectivesOfReview(projectId, reviewId), Throws.Exception);
            httpResponse.StatusCode = HttpStatusCode.OK;

            httpResponse.Content = new StringContent(JsonSerializerHelper.SerializeObject(this.entitiesDto));
            var reviews = await this.service.GetReviewsObjectivesOfReview(projectId, reviewId);
            Assert.That(reviews, Has.Count.EqualTo(1));
            httpResponse.Content = new StringContent(string.Empty);
            Assert.That(async () => await this.service.GetReviewsObjectivesOfReview(projectId, reviewId), Throws.Exception);
        }

        [Test]
        public async Task VerifyGetReviewObjective()
        {
            var projectId = Guid.NewGuid();
            var reviewId = Guid.NewGuid();
            var guid = this.entitiesDto.First().Id;
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.NotFound;

            var request = this.httpMessageHandler.When(HttpMethod.Get, $"/Project/{projectId}/Review/{reviewId}/ReviewObjective/{guid}");
            request.Respond(_ => httpResponse);
            var review = await this.service.GetReviewObjectiveOfReview(projectId, reviewId, guid);

            Assert.That(review, Is.Null);

            httpResponse.StatusCode = HttpStatusCode.OK;

            httpResponse.Content = new StringContent(JsonSerializerHelper.SerializeObject(this.entitiesDto));
            review = await this.service.GetReviewObjectiveOfReview(projectId, reviewId, guid);
            Assert.That(review.Id, Is.EqualTo(guid));

            httpResponse.Content = new StringContent(string.Empty);
            review = await this.service.GetReviewObjectiveOfReview(projectId, reviewId, guid);

            Assert.That(review, Is.Null);
        }

        [Test]
        public async Task VerifyCreateReviewObjective()
        {
            var reviewObjective = new ReviewObjective()
            {
                Description = "Review description",
                Title = "Review title"
            };

            var projectId = Guid.NewGuid();
            var reviewId = Guid.NewGuid();

            var httpResponse = new HttpResponseMessage();

            var entityRequestResponse = new EntityRequestResponseDto()
            {
                IsRequestSuccessful = false
            };

            httpResponse.Content = new StringContent(JsonSerializerHelper.SerializeObject(entityRequestResponse));
            var request = this.httpMessageHandler.When(HttpMethod.Post, $"/Project/{projectId}/Review/{reviewId}/ReviewObjective/Create");
            request.Respond(_ => httpResponse);

            var requestResponse = await this.service.CreateReviewObjective(projectId, reviewId, reviewObjective);
            Assert.That(requestResponse.IsRequestSuccessful, Is.False);

            entityRequestResponse.IsRequestSuccessful = true;

            entityRequestResponse.Entities = reviewObjective.GetAssociatedEntities().ToDtos();

            httpResponse.Content = new StringContent(JsonSerializerHelper.SerializeObject(entityRequestResponse));

            requestResponse = await this.service.CreateReviewObjective(projectId, reviewId, reviewObjective);

            Assert.Multiple(() =>
            {
                Assert.That(requestResponse.IsRequestSuccessful, Is.True);
                Assert.That(requestResponse.Entity, Is.Not.Null);
            });

            httpResponse.Content = new StringContent(string.Empty);

            Assert.That(async () => await this.service.CreateReviewObjective(projectId, reviewId, reviewObjective), Throws.Exception);
        }

        [Test]
        public async Task VerifyDeleteReviewObjective()
        {
            var reviewObjective = new ReviewObjective(Guid.NewGuid())
            {
                Description = "Review description",
                Title = "Review title",
                ReviewObjectiveNumber = 1,
                Author = new Participant(Guid.NewGuid())
            };

            var projectId = Guid.NewGuid();
            var review = new Review(Guid.NewGuid());

            var httpResponse = new HttpResponseMessage();
            var requestResponse = new RequestResponseDto() { IsRequestSuccessful = true };
            httpResponse.Content = new StringContent(JsonSerializerHelper.SerializeObject(requestResponse));
            var request = this.httpMessageHandler.When(HttpMethod.Delete, $"/Project/{projectId}/Review/{review.Id}/ReviewObjective/{reviewObjective.Id}");
            request.Respond(_ => httpResponse);

            Assert.That(async () => await this.service.DeleteReviewObjective(projectId, reviewObjective), Throws.Exception);

            review.ReviewObjectives.Add(reviewObjective);
            var result = await this.service.DeleteReviewObjective(projectId, reviewObjective);
            Assert.That(result.IsRequestSuccessful, Is.True);

            httpResponse.Content = new StringContent(string.Empty);
            Assert.That(async () => await this.service.DeleteReviewObjective(projectId, reviewObjective), Throws.Exception);
        }

        [Test]
        public async Task VerifyUpdateReview()
        {
            var reviewObjective = new ReviewObjective(Guid.NewGuid())
            {
                Description = "Review description",
                Title = "Review title",
                ReviewObjectiveNumber = 1,
                Author = new Participant(Guid.NewGuid())
                {
                    Role = new Role(Guid.NewGuid()),
                    User = new UserEntity(Guid.NewGuid())
                }
            };

            var projectId = Guid.NewGuid();
            var review = new Review(Guid.NewGuid());

            var httpResponse = new HttpResponseMessage();
            var requestResponse = new EntityRequestResponseDto() { IsRequestSuccessful = false };
            httpResponse.Content = new StringContent(JsonSerializerHelper.SerializeObject(requestResponse));

            var request = this.httpMessageHandler.When(HttpMethod.Put, $"/Project/{projectId}/Review/{review.Id}/ReviewObjective/{reviewObjective.Id}");
            request.Respond(_ => httpResponse);

            Assert.That(async () => await this.service.UpdateReviewObjective(projectId,reviewObjective), Throws.Exception);
            review.ReviewObjectives.Add(reviewObjective);

            var requestResult = await this.service.UpdateReviewObjective(projectId, reviewObjective);
            Assert.That(requestResult.IsRequestSuccessful, Is.False);

            requestResponse.IsRequestSuccessful = true;

            requestResponse.Entities = reviewObjective.GetAssociatedEntities().ToDtos();

            httpResponse.Content = new StringContent(JsonSerializerHelper.SerializeObject(requestResponse));

            requestResult = await this.service.UpdateReviewObjective(projectId, reviewObjective);
            Assert.That(requestResult.IsRequestSuccessful, Is.True);
            httpResponse.Content = new StringContent(string.Empty);
            Assert.That(async () => await this.service.UpdateReviewObjective(projectId, reviewObjective), Throws.Exception);
        }
    }
}
