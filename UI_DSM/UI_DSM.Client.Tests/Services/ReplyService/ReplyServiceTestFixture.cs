// --------------------------------------------------------------------------------------------------------
// <copyright file="ReplyServiceTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Services.ReplyService
{
    using System.Net;

    using NUnit.Framework;

    using RichardSzalay.MockHttp;

    using UI_DSM.Client.Services;
    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Client.Services.ReplyService;
    using UI_DSM.Serializer.Json;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Extensions;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class ReplyServiceTestFixture
    {
        private ReplyService service;
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
                new ReplyDto(Guid.NewGuid())
                {
                    Author = participantId,
                    CreatedOn = DateTime.UtcNow,
                    Content = "A reply"
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

            ServiceBase.RegisterService<ReplyService>();
            this.jsonService = new JsonService(new JsonDeserializer(), new JsonSerializer());
            this.service = new ReplyService(httpClient, this.jsonService);
        }

        [Test]
        public async Task VerifyGetReplies()
        {
            var projectId = Guid.NewGuid();
            var annotationId = Guid.NewGuid();
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.NotFound;

            var request = this.httpMessageHandler.When($"/Project/{projectId}/Annotation/{annotationId}/Reply");
            request.Respond(_ => httpResponse);

            Assert.That(async () => await this.service.GetRepliesOfComment(projectId, annotationId), Throws.Exception);
            httpResponse.StatusCode = HttpStatusCode.OK;

            httpResponse.Content = new StringContent(this.jsonService.Serialize(this.entitiesDto));
            var replies = await this.service.GetRepliesOfComment(projectId, annotationId);
            Assert.That(replies, Has.Count.EqualTo(1));
            httpResponse.Content = new StringContent(string.Empty);
            Assert.That(async () => await this.service.GetRepliesOfComment(projectId, annotationId), Throws.Exception);
        }

        [Test]
        public async Task VerifyGetReply()
        {
            var projectId = Guid.NewGuid();
            var annotationId = Guid.NewGuid();
            var guid = this.entitiesDto.First().Id;
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.NotFound;

            var request = this.httpMessageHandler.When($"/Project/{projectId}/Annotation/{annotationId}/Reply/{guid}");
            request.Respond(_ => httpResponse);
            var reply = await this.service.GetReplyOfComment(projectId, annotationId, guid);

            Assert.That(reply, Is.Null);

            httpResponse.StatusCode = HttpStatusCode.OK;

            httpResponse.Content = new StringContent(this.jsonService.Serialize(this.entitiesDto));
            reply = await this.service.GetReplyOfComment(projectId, annotationId, guid);
            Assert.That(reply.Id, Is.EqualTo(guid));

            httpResponse.Content = new StringContent(string.Empty);

            Assert.That(async () => await this.service.GetReplyOfComment(projectId, annotationId, guid), Throws.Exception);
        }

        [Test]
        public async Task VerifyCreateReply()
        {
            var reply = new Reply()
            {
                Content = "Reply"
            };

            var projectId = Guid.NewGuid();
            var annotationId = Guid.NewGuid();

            var httpResponse = new HttpResponseMessage();

            var entityRequestResponse = new EntityRequestResponseDto()
            {
                IsRequestSuccessful = false
            };

            httpResponse.Content = new StringContent(this.jsonService.Serialize(entityRequestResponse));
            var request = this.httpMessageHandler.When(HttpMethod.Post, $"/Project/{projectId}/Annotation/{annotationId}/Reply/Create");
            request.Respond(_ => httpResponse);

            var requestResponse = await this.service.CreateReply(projectId, annotationId, reply);
            Assert.That(requestResponse.IsRequestSuccessful, Is.False);

            entityRequestResponse.IsRequestSuccessful = true;

            entityRequestResponse.Entities = reply.GetAssociatedEntities().ToDtos();

            httpResponse.Content = new StringContent(this.jsonService.Serialize(entityRequestResponse));

            requestResponse = await this.service.CreateReply(projectId, annotationId, reply);

            Assert.Multiple(() =>
            {
                Assert.That(requestResponse.IsRequestSuccessful, Is.True);
                Assert.That(requestResponse.Entity, Is.Not.Null);
            });

            httpResponse.Content = new StringContent(string.Empty);

            Assert.That(async () => await this.service.CreateReply(projectId, annotationId, reply), Throws.Exception);
        }

        [Test]
        public async Task VerifyDeleteReply()
        {
            var reply = new Reply(Guid.NewGuid())
            {
                Content = "Reply",
                Author = new Participant(Guid.NewGuid())
            };

            var projectId = Guid.NewGuid();
            var annotation = new Comment(Guid.NewGuid());

            var httpResponse = new HttpResponseMessage();
            var requestResponse = new RequestResponseDto() { IsRequestSuccessful = true };
            httpResponse.Content = new StringContent(this.jsonService.Serialize(requestResponse));
            var request = this.httpMessageHandler.When(HttpMethod.Delete, $"/Project/{projectId}/Annotation/{annotation.Id}/Reply/{reply.Id}");
            request.Respond(_ => httpResponse);

            Assert.That(async () => await this.service.DeleteReply(projectId, reply), Throws.Exception);

            annotation.Replies.Add(reply);
            var result = await this.service.DeleteReply(projectId, reply);
            Assert.That(result.IsRequestSuccessful, Is.True);

            httpResponse.Content = new StringContent(string.Empty);
            Assert.That(async () => await this.service.DeleteReply(projectId, reply), Throws.Exception);
        }

        [Test]
        public async Task VerifyUpdateReview()
        {
            var reply = new Reply(Guid.NewGuid())
            {
                Content = "Reply",
                Author = new Participant(Guid.NewGuid())
                {
                    Role = new Role(Guid.NewGuid()),
                    User = new UserEntity(Guid.NewGuid())
                }
            };

            var projectId = Guid.NewGuid();
            var comment = new Comment(Guid.NewGuid());

            var httpResponse = new HttpResponseMessage();
            var requestResponse = new EntityRequestResponseDto() { IsRequestSuccessful = false };
            httpResponse.Content = new StringContent(this.jsonService.Serialize(requestResponse));

            var request = this.httpMessageHandler.When(HttpMethod.Put, $"/Project/{projectId}/Annotation/{comment.Id}/Reply/{reply.Id}");
            request.Respond(_ => httpResponse);

            Assert.That(async () => await this.service.UpdateReply(projectId, reply), Throws.Exception);
            comment.Replies.Add(reply);

            var requestResult = await this.service.UpdateReply(projectId, reply);
            Assert.That(requestResult.IsRequestSuccessful, Is.False);

            requestResponse.IsRequestSuccessful = true;

            requestResponse.Entities = reply.GetAssociatedEntities().ToDtos();

            httpResponse.Content = new StringContent(this.jsonService.Serialize(requestResponse));

            requestResult = await this.service.UpdateReply(projectId, reply);
            Assert.That(requestResult.IsRequestSuccessful, Is.True);
            httpResponse.Content = new StringContent(string.Empty);
            Assert.That(async () => await this.service.UpdateReply(projectId, reply), Throws.Exception);
        }
    }
}
