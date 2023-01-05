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
    using UI_DSM.Client.Services.JsonService;
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
        private IJsonService jsonService;

        [SetUp]
        public void Setup()
        {
            this.httpMessageHandler = new MockHttpMessageHandler();
            var httpClient = this.httpMessageHandler.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost/api");
            EntityHelper.RegisterEntities();

            var participantId = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            this.entitiesDto = new List<EntityDto>
            {
                new ReviewObjectiveDto(Guid.NewGuid())
                {
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
                new UserEntityDto(userId)
                {
                    UserName = "user"
                }
            };

            ServiceBase.RegisterService<ReviewObjectiveService>();
            this.jsonService = JsonSerializerHelper.CreateService();
            this.service = new ReviewObjectiveService(httpClient, this.jsonService);
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

            httpResponse.Content = new StringContent(this.jsonService.Serialize(this.entitiesDto));
            var reviewObjectives = await this.service.GetReviewsObjectivesOfReview(projectId, reviewId);
            Assert.That(reviewObjectives, Has.Count.EqualTo(1));
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
            var reviewObjective = await this.service.GetReviewObjectiveOfReview(projectId, reviewId, guid);

            Assert.That(reviewObjective, Is.Null);

            httpResponse.StatusCode = HttpStatusCode.OK;

            httpResponse.Content = new StringContent(this.jsonService.Serialize(this.entitiesDto));
            reviewObjective = await this.service.GetReviewObjectiveOfReview(projectId, reviewId, guid);
            Assert.That(reviewObjective.Id, Is.EqualTo(guid));

            httpResponse.Content = new StringContent(string.Empty);

            Assert.That(async () => await this.service.GetReviewObjectiveOfReview(projectId, reviewId, guid), Throws.Exception);
        }

        [Test]
        public async Task VerifyCreateReviewObjective()
        {
            var reviewObjective = new ReviewObjectiveCreationDto()
            {
                Kind = ReviewObjectiveKind.Prr,
                KindNumber = 0
            };

            var projectId = Guid.NewGuid();
            var reviewId = Guid.NewGuid();

            var httpResponse = new HttpResponseMessage();

            var entityRequestResponse = new EntityRequestResponseDto()
            {
                IsRequestSuccessful = false
            };

            httpResponse.Content = new StringContent(this.jsonService.Serialize(entityRequestResponse));
            var request = this.httpMessageHandler.When(HttpMethod.Post, $"/Project/{projectId}/Review/{reviewId}/ReviewObjective/CreateTemplate");
            request.Respond(_ => httpResponse);

            var requestResponse = await this.service.CreateReviewObjective(projectId, reviewId, reviewObjective);
            Assert.That(requestResponse.IsRequestSuccessful, Is.False);

            entityRequestResponse.IsRequestSuccessful = true;

            entityRequestResponse.Entities = new ReviewObjective().GetAssociatedEntities().ToDtos();

            httpResponse.Content = new StringContent(this.jsonService.Serialize(entityRequestResponse));

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
        public async Task VerifyCreateReviewObjectives()
        {
            var reviewObjectives = new List<ReviewObjectiveCreationDto>()
            {
                new ReviewObjectiveCreationDto()
                {
                    Kind = ReviewObjectiveKind.Prr,
                    KindNumber = 0
                }
            };

            var projectId = Guid.NewGuid();
            var reviewId = Guid.NewGuid();

            var httpResponse = new HttpResponseMessage();

            var entityRequestResponse = new EntityRequestResponseDto()
            {
                IsRequestSuccessful = false
            };

            httpResponse.Content = new StringContent(this.jsonService.Serialize(entityRequestResponse));
            var request = this.httpMessageHandler.When(HttpMethod.Post, $"/Project/{projectId}/Review/{reviewId}/ReviewObjective/CreateTemplates");
            request.Respond(_ => httpResponse);

            var requestResponse = await this.service.CreateReviewObjectives(projectId, reviewId, reviewObjectives);
            Assert.That(requestResponse.IsRequestSuccessful, Is.False);

            entityRequestResponse.IsRequestSuccessful = true;

            entityRequestResponse.Entities = new ReviewObjective().GetAssociatedEntities().ToDtos();

            httpResponse.Content = new StringContent(this.jsonService.Serialize(entityRequestResponse));

            requestResponse = await this.service.CreateReviewObjectives(projectId, reviewId, reviewObjectives);

            Assert.Multiple(() =>
            {
                Assert.That(requestResponse.IsRequestSuccessful, Is.True);
                Assert.That(requestResponse.Entities, Is.Not.Empty);
            });

            httpResponse.Content = new StringContent(string.Empty);

            Assert.That(async () => await this.service.CreateReviewObjectives(projectId, reviewId, reviewObjectives), Throws.Exception);
        }

        [Test]
        public async Task VerifyDeleteReviewObjective()
        {
            var reviewObjective = new ReviewObjective(Guid.NewGuid())
            {
                Description = "Review description",
                ReviewObjectiveNumber = 1,
            };

            var projectId = Guid.NewGuid();
            var review = new Review(Guid.NewGuid());

            var httpResponse = new HttpResponseMessage();
            var requestResponse = new RequestResponseDto() { IsRequestSuccessful = true };
            httpResponse.Content = new StringContent(this.jsonService.Serialize(requestResponse));
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
                ReviewObjectiveNumber = 1,
            };

            var projectId = Guid.NewGuid();
            var review = new Review(Guid.NewGuid());

            var httpResponse = new HttpResponseMessage();
            var requestResponse = new EntityRequestResponseDto() { IsRequestSuccessful = false };
            httpResponse.Content = new StringContent(this.jsonService.Serialize(requestResponse));

            var request = this.httpMessageHandler.When(HttpMethod.Put, $"/Project/{projectId}/Review/{review.Id}/ReviewObjective/{reviewObjective.Id}");
            request.Respond(_ => httpResponse);

            Assert.That(async () => await this.service.UpdateReviewObjective(projectId,reviewObjective), Throws.Exception);
            review.ReviewObjectives.Add(reviewObjective);

            var requestResult = await this.service.UpdateReviewObjective(projectId, reviewObjective);
            Assert.That(requestResult.IsRequestSuccessful, Is.False);

            requestResponse.IsRequestSuccessful = true;

            requestResponse.Entities = reviewObjective.GetAssociatedEntities().ToDtos();

            httpResponse.Content = new StringContent(this.jsonService.Serialize(requestResponse));

            requestResult = await this.service.UpdateReviewObjective(projectId, reviewObjective);
            Assert.That(requestResult.IsRequestSuccessful, Is.True);
            httpResponse.Content = new StringContent(string.Empty);
            Assert.That(async () => await this.service.UpdateReviewObjective(projectId, reviewObjective), Throws.Exception);
        }

        [Test]
        public async Task VerifyGetAvailableTemplates()
        {
            var projectId = Guid.NewGuid();
            var reviewId = Guid.NewGuid();
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.NotFound;

            var request = this.httpMessageHandler.When($"/Project/{projectId}/Review/{reviewId}/ReviewObjective/GetAvailableTemplates");
            request.Respond(_ => httpResponse);

            Assert.That(async () => await this.service.GetAvailableTemplates(projectId, reviewId), Throws.Exception);
            httpResponse.StatusCode = HttpStatusCode.OK;

            httpResponse.Content = new StringContent(this.jsonService.Serialize(this.entitiesDto));
            var reviewObjectives = await this.service.GetAvailableTemplates(projectId, reviewId);
            Assert.That(reviewObjectives, Has.Count.EqualTo(4));
            httpResponse.Content = new StringContent(string.Empty);
            Assert.That(async () => await this.service.GetAvailableTemplates(projectId, reviewId), Throws.Exception);
        }

        [Test]
        public async Task VerifyGetOpenTaskAndComments()
        {
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.BadRequest;

            var project = new Project(Guid.NewGuid());
            var review = new Review(Guid.NewGuid());

            var request = this.httpMessageHandler.When(HttpMethod.Get, $"/Project/{project.Id}/Review/{review.Id}/ReviewObjective/OpenTasksAndComments");
            request.Respond(_ => httpResponse);

            Assert.That(async () => await this.service.GetOpenTasksAndComments(project.Id, review.Id), Throws.Exception);

            httpResponse.StatusCode = HttpStatusCode.OK;

            var guids = new List<Guid>
            {
                Guid.NewGuid()
            };

            var requestResults = new Dictionary<Guid, AdditionalComputedProperties>
            {
                [guids[0]] = new()
                {
                    OpenCommentCount = 15,
                    TaskCount = 12
                }
            };

            httpResponse.Content = new StringContent(this.jsonService.Serialize(requestResults));
            Assert.That(await this.service.GetOpenTasksAndComments(project.Id, review.Id), Is.EquivalentTo(requestResults));
        }
    }
}
