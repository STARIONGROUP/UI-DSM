// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewTaskServiceTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Services.ReviewTaskService
{
    using System.Net;

    using NUnit.Framework;

    using RichardSzalay.MockHttp;

    using UI_DSM.Client.Services;
    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Client.Services.ReviewTaskService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Extensions;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class ReviewTaskServiceTestFixture
    {
        private ReviewTaskService service;
        private MockHttpMessageHandler httpMessageHandler;
        private List<EntityDto> entitiesDto;
        private Guid projectId;
        private Guid reviewId;
        private Guid reviewObjectiveId;
        private IJsonService jsonService;

        [SetUp]
        public void Setup()
        {
            this.projectId = Guid.NewGuid();
            this.reviewId = Guid.NewGuid();
            this.reviewObjectiveId = Guid.NewGuid();
            this.httpMessageHandler = new MockHttpMessageHandler();
            var httpClient = this.httpMessageHandler.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost/api");
            EntityHelper.RegisterEntities();
            
            var participantId = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            List<Guid> participants = new()
            {
                participantId,
            };

            this.entitiesDto = new List<EntityDto>
            {
                new ReviewTaskDto(Guid.NewGuid())
                {
                    CreatedOn = DateTime.UtcNow,
                    Description = "A task",
                    Title = "Task Title",
                    TaskNumber = 1,
                    Status = StatusKind.Open,
                    IsAssignedTo = participants
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

            ServiceBase.RegisterService<ReviewTaskService>();
            this.jsonService = JsonSerializerHelper.CreateService();
            this.service = new ReviewTaskService(httpClient, this.jsonService);
        }

        [Test]
        public async Task VerifyGetReviewTasks()
        {
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.NotFound;

            var request = this.httpMessageHandler.When($"/Project/{this.projectId}/Review/{this.reviewId}/ReviewObjective/{this.reviewObjectiveId}/ReviewTask");
            request.Respond(_ => httpResponse);

            Assert.That(async () => await this.service.GetTasksOfReviewObjectives(this.projectId, this.reviewId, this.reviewObjectiveId), Throws.Exception);
            httpResponse.StatusCode = HttpStatusCode.OK;

            httpResponse.Content = new StringContent(this.jsonService.Serialize(this.entitiesDto));
            var reviewTasks = await this.service.GetTasksOfReviewObjectives(this.projectId, this.reviewId, this.reviewObjectiveId);
            Assert.That(reviewTasks, Has.Count.EqualTo(1));
            httpResponse.Content = new StringContent(string.Empty);
            Assert.That(async () => await this.service.GetTasksOfReviewObjectives(this.projectId, this.reviewId, this.reviewObjectiveId), Throws.Exception);
        }

        [Test]
        public async Task VerifyGetReviewTask()
        {
            var guid = this.entitiesDto.First().Id;
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.NotFound;

            var request = this.httpMessageHandler.When(HttpMethod.Get, $"/Project/{this.projectId}/Review/{this.reviewId}/ReviewObjective/{this.reviewObjectiveId}/ReviewTask/{guid}");
            request.Respond(_ => httpResponse);
            var reviewTask = await this.service.GetTaskOfReviewObjective(this.projectId, this.reviewId, this.reviewObjectiveId, guid);

            Assert.That(reviewTask, Is.Null);

            httpResponse.StatusCode = HttpStatusCode.OK;

            httpResponse.Content = new StringContent(this.jsonService.Serialize(this.entitiesDto));
            reviewTask = await this.service.GetTaskOfReviewObjective(this.projectId, this.reviewId, this.reviewObjectiveId, guid);
            Assert.That(reviewTask.Id, Is.EqualTo(guid));

            httpResponse.Content = new StringContent(string.Empty);

            Assert.That(async () => await this.service.GetTaskOfReviewObjective(this.projectId, 
                this.reviewId, this.reviewObjectiveId, guid), Throws.Exception);
        }

        [Test]
        public async Task VerifyDeleteReviewTask()
        {
            var reviewTask = new ReviewTask(Guid.NewGuid())
            {
                Description = "Review description",
                Title = "Review title",
                TaskNumber = 1,
            };

            var reviewObjective = new ReviewObjective(this.reviewObjectiveId);
            var httpResponse = new HttpResponseMessage();
            var requestResponse = new RequestResponseDto() { IsRequestSuccessful = true };
            httpResponse.Content = new StringContent(this.jsonService.Serialize(requestResponse));
            var request = this.httpMessageHandler.When(HttpMethod.Delete, $"/Project/{this.projectId}/Review/{this.reviewId}/ReviewObjective/{this.reviewObjectiveId}/ReviewTask/{reviewTask.Id}");
            request.Respond(_ => httpResponse);

            Assert.That(async () => await this.service.DeleteReviewTask(this.projectId, this.reviewId, reviewTask), Throws.Exception);

            reviewObjective.ReviewTasks.Add(reviewTask);
            var result = await this.service.DeleteReviewTask(this.projectId, this.reviewId, reviewTask);
            Assert.That(result.IsRequestSuccessful, Is.True);

            httpResponse.Content = new StringContent(string.Empty);
            Assert.That(async () => await this.service.DeleteReviewTask(this.projectId, this.reviewId, reviewTask), Throws.Exception);
        }

        [Test]
        public async Task VerifyUpdateReview()
        {
            var reviewTask = new ReviewTask(Guid.NewGuid())
            {
                Description = "Review description",
                Title = "Review title",
                TaskNumber = 1,
            };

            var reviewObjective = new ReviewObjective(this.reviewObjectiveId);

            var httpResponse = new HttpResponseMessage();
            var requestResponse = new EntityRequestResponseDto() { IsRequestSuccessful = false };
            httpResponse.Content = new StringContent(this.jsonService.Serialize(requestResponse));

            var request = this.httpMessageHandler.When(HttpMethod.Put, $"/Project/{this.projectId}/Review/{this.reviewId}/ReviewObjective/{this.reviewObjectiveId}/ReviewTask/{reviewTask.Id}");
            request.Respond(_ => httpResponse);

            Assert.That(async () => await this.service.UpdateReviewTask(this.projectId, this.reviewId, reviewTask), Throws.Exception);
            reviewObjective.ReviewTasks.Add(reviewTask);

            var requestResult = await this.service.UpdateReviewTask(this.projectId, this.reviewId, reviewTask);
            Assert.That(requestResult.IsRequestSuccessful, Is.False);

            requestResponse.IsRequestSuccessful = true;

            requestResponse.Entities = reviewTask.GetAssociatedEntities().ToDtos();

            httpResponse.Content = new StringContent(this.jsonService.Serialize(requestResponse));

            requestResult = await this.service.UpdateReviewTask(this.projectId, this.reviewId, reviewTask);
            Assert.That(requestResult.IsRequestSuccessful, Is.True);
            httpResponse.Content = new StringContent(string.Empty);
            Assert.That(async () => await this.service.UpdateReviewTask(this.projectId, this.reviewId, reviewTask), Throws.Exception);
        }

        [Test]
        public async Task VerifyGetCommentsCount()
        {
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.BadRequest;

            var project = new Project(Guid.NewGuid());
            var review = new Review(Guid.NewGuid());

            var request = this.httpMessageHandler.When(HttpMethod.Get, $"/Project/{project.Id}/Review/{review.Id}/ReviewObjective/{this.reviewObjectiveId}/ReviewTask/CommentsCount");
            request.Respond(_ => httpResponse);

            Assert.That(async () => await this.service.GetCommmentsCount(project.Id, review.Id, this.reviewObjectiveId), Throws.Exception);

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
                    TaskCount = 0
                }
            };

            httpResponse.Content = new StringContent(this.jsonService.Serialize(requestResults));
            Assert.That(await this.service.GetCommmentsCount(project.Id, review.Id, this.reviewObjectiveId), Is.EquivalentTo(requestResults));
        }

        [Test]
        public async Task GetReviewTasksForView()
        {
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.BadRequest;

            var project = new Project(Guid.NewGuid());
            var review = new Review(Guid.NewGuid());

            var request = this.httpMessageHandler.When(HttpMethod.Get, $"/Project/{project.Id}/Review/{review.Id}/ReviewObjective/{Guid.Empty}/ReviewTask/View/InterfaceView");
            request.Respond(_ => httpResponse);

            Assert.That(async () => await this.service.GetReviewTasksForView(project.Id, review.Id, View.InterfaceView), Throws.Exception);

            httpResponse.StatusCode = HttpStatusCode.OK;

            var requestResults = new List<EntityDto>();

            httpResponse.Content = new StringContent(this.jsonService.Serialize(requestResults));
            Assert.That(await this.service.GetReviewTasksForView(project.Id, review.Id, View.InterfaceView), Is.Empty);
        }
    }
}
