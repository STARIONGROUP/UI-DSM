// --------------------------------------------------------------------------------------------------------
// <copyright file="ReplyModuleTestFixture.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to comment an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Server.Tests.Modules
{
    using System.Security.Claims;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Server.Managers;
    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Server.Managers.ReplyManager;
    using UI_DSM.Server.Modules;
    using UI_DSM.Server.Services.SearchService;
    using UI_DSM.Server.Tests.Helpers;
    using UI_DSM.Server.Types;
    using UI_DSM.Server.Validator;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class ReplyModuleTestFixture
    {
        private ReplyModule module;
        private Mock<IEntityManager<Reply>> replyManager;
        private Mock<HttpContext> context;
        private Mock<HttpResponse> response;
        private Mock<HttpRequest> request;
        private Mock<IServiceProvider> serviceProvider;
        private Mock<IEntityManager<Annotation>> annotationManager;
        private Mock<IParticipantManager> participantManager;
        private Mock<ISearchService> searchService;
        private RouteValueDictionary routeValues;
        private Guid projectId;
        private Guid commentId;

        [SetUp]
        public void Setup()
        {
            this.replyManager = new Mock<IEntityManager<Reply>>();
            this.replyManager.As<IReplyManager>();
            this.replyManager.As<IContainedEntityManager<Reply>>();
            this.annotationManager = new Mock<IEntityManager<Annotation>>();
            this.annotationManager.As<IContainedEntityManager<Annotation>>();
            this.participantManager = new Mock<IParticipantManager>();
            this.searchService = new Mock<ISearchService>();

            ModuleTestHelper.Setup<ReplyModule, ReplyDto>(new ReplyDtoValidator(), out this.context,
                out this.response, out this.request, out this.serviceProvider);

            this.routeValues = new RouteValueDictionary();
            this.request.Setup(x => x.RouteValues).Returns(this.routeValues);
            this.projectId = Guid.NewGuid();
            this.commentId = Guid.NewGuid();

            this.routeValues["projectId"] = this.projectId.ToString();
            this.routeValues["annotationId"] = this.commentId.ToString();

            this.serviceProvider.Setup(x => x.GetService(typeof(IContainedEntityManager<Annotation>))).Returns(this.annotationManager.Object);
            this.serviceProvider.Setup(x => x.GetService(typeof(IEntityManager<Annotation>))).Returns(this.annotationManager.Object);
            this.serviceProvider.Setup(x => x.GetService(typeof(IParticipantManager))).Returns(this.participantManager.Object);

            this.context.Setup(x => x.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()
            {
                new (ClaimTypes.Name, "user")
            })));

            this.module = new ReplyModule();
            Program.RegisterEntities();
        }

        [Test]
        public async Task VerifyGetEntities()
        {
            var participant = new Participant(Guid.NewGuid())
            {
                Role = new Role(Guid.NewGuid()),
                User = new UserEntity(Guid.NewGuid())
            };

            var replys = new List<Reply>()
            {
                new(Guid.NewGuid())
                {
                    Author = participant,
                },
                new(Guid.NewGuid())
                {
                    Author = participant,
                }
            };

            this.serviceProvider.Setup(x => x.GetService(typeof(IParticipantManager))).Returns(null);

            await this.module.GetEntities(this.replyManager.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 500, Times.Once);
            this.serviceProvider.Setup(x => x.GetService(typeof(IParticipantManager))).Returns(this.participantManager.Object);

            this.participantManager.Setup(x => x.GetParticipantForProject(It.IsAny<Guid>(), "user")).ReturnsAsync((Participant)null);

            await this.module.GetEntities(this.replyManager.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 401, Times.Once);
            this.participantManager.Setup(x => x.GetParticipantForProject(It.IsAny<Guid>(), "user")).ReturnsAsync(participant);

            this.annotationManager.As<IContainedEntityManager<Annotation>>()
                .Setup(x => x.FindEntity(this.commentId)).ReturnsAsync(new Comment(this.commentId));

            this.annotationManager.As<IContainedEntityManager<Annotation>>()
                .Setup(x => x.EntityIsContainedBy(this.commentId, this.projectId)).ReturnsAsync(false);

            this.serviceProvider.Setup(x => x.GetService(typeof(IContainedEntityManager<Annotation>))).Returns(null);

            await this.module.GetEntities(this.replyManager.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 500, Times.Exactly(2));

            this.serviceProvider.Setup(x => x.GetService(typeof(IContainedEntityManager<Annotation>))).Returns(this.annotationManager.Object);
            await this.module.GetEntities(this.replyManager.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400, Times.Once);

            this.annotationManager.As<IContainedEntityManager<Annotation>>()
                .Setup(x => x.EntityIsContainedBy(this.commentId, this.projectId)).ReturnsAsync(true);

            this.replyManager.As<IReplyManager>().Setup(x => x.GetContainedEntities(this.commentId, 0))
                .ReturnsAsync(replys);

            await this.module.GetEntities(this.replyManager.Object, this.context.Object);
            this.replyManager.As<IReplyManager>().Verify(x => x.GetContainedEntities(this.commentId, 0), Times.Once);
        }

        [Test]
        public async Task VerifyGetEntity()
        {
            var reply = new Reply(Guid.NewGuid())
            {
                Author = new Participant(Guid.NewGuid())
                {
                    Role = new Role(Guid.NewGuid()),
                    User = new UserEntity(Guid.NewGuid())
                }
            };

            _ = new Comment(this.commentId)
            {
                Replies = { reply }
            };

            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync((Participant)null);

            await this.module.GetEntity(this.replyManager.Object, reply.Id, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 401, Times.Once);
            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync(reply.Author);

            this.annotationManager.As<IContainedEntityManager<Annotation>>()
                .Setup(x => x.FindEntity(this.commentId)).ReturnsAsync(new Feedback(this.commentId));

            await this.module.GetEntity(this.replyManager.Object, reply.Id, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400, Times.Once);

            this.annotationManager.As<IContainedEntityManager<Annotation>>()
                .Setup(x => x.FindEntity(this.commentId)).ReturnsAsync(new Comment(this.commentId));

            this.annotationManager.As<IContainedEntityManager<Annotation>>()
                .Setup(x => x.EntityIsContainedBy(this.commentId, this.projectId)).ReturnsAsync(false);

            this.replyManager.As<IContainedEntityManager<Reply>>()
                .Setup(x => x.FindEntityWithContainer(reply.Id)).ReturnsAsync(reply);

            await this.module.GetEntity(this.replyManager.Object, reply.Id, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400, Times.Exactly(2));

            this.annotationManager.As<IContainedEntityManager<Annotation>>()
                .Setup(x => x.EntityIsContainedBy(this.commentId, this.projectId)).ReturnsAsync(true);

            await this.module.GetEntity(this.replyManager.Object, reply.Id, this.context.Object);
            this.replyManager.As<IContainedEntityManager<Reply>>().Verify(x => x.FindEntityWithContainer(reply.Id), Times.Once);
        }

        [Test]
        public async Task VerifyCreateEntity()
        {
            var participant = new Participant(Guid.NewGuid())
            {
                Role = new Role(Guid.NewGuid()),
                User = new UserEntity(Guid.NewGuid())
            };

            var dto = new ReplyDto()
            {
                Content = "Content"
            };

            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync((Participant)null);

            await this.module.CreateEntity(this.replyManager.Object, dto, this.searchService.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 401, Times.Once);

            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync(participant);

            var comment = new Comment(this.commentId);

            this.annotationManager.As<IContainedEntityManager<Annotation>>()
                .Setup(x => x.FindEntity(this.commentId)).ReturnsAsync(comment);

            this.annotationManager.As<IContainedEntityManager<Annotation>>()
                .Setup(x => x.EntityIsContainedBy(this.commentId, this.projectId)).ReturnsAsync(false);

            await this.module.CreateEntity(this.replyManager.Object, dto, this.searchService.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400, Times.Once);

            this.annotationManager.As<IContainedEntityManager<Annotation>>()
                .Setup(x => x.EntityIsContainedBy(this.commentId, this.projectId)).ReturnsAsync(true);

            this.replyManager.Setup(x => x.CreateEntity(It.IsAny<Reply>())).ReturnsAsync(EntityOperationResult<Reply>
                .Success(new Reply(Guid.NewGuid())
                {
                    Author = participant
                }));

            this.annotationManager.Setup(x => x.GetEntity(this.commentId,0)).ReturnsAsync(comment.GetAssociatedEntities());
            await this.module.CreateEntity(this.replyManager.Object, dto, this.searchService.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 201, Times.Once);
        }

        [Test]
        public async Task VerifyDeleteEntity()
        {
            var participant = new Participant(Guid.NewGuid())
            {
                Role = new Role(Guid.NewGuid()),
                User = new UserEntity(Guid.NewGuid())
            };

            this.annotationManager.As<IContainedEntityManager<Annotation>>()
                .Setup(x => x.EntityIsContainedBy(this.commentId, this.projectId)).ReturnsAsync(false);

            var reply = new Reply(Guid.NewGuid())
            {
                Author = new Participant(Guid.NewGuid())
            };

            var comment = new Comment(this.commentId)
            {
                Replies = { reply }
            };

            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync((Participant)null);

            await this.module.DeleteEntity(this.replyManager.Object, reply.Id, this.searchService.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 401, Times.Once);

            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync(participant);
            this.replyManager.Setup(x => x.GetEntity(reply.Id, 0)).ReturnsAsync(new List<Entity> { reply });

            await this.module.DeleteEntity(this.replyManager.Object, reply.Id, this.searchService.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 403, Times.Once);

            reply.Author = participant;

            await this.module.DeleteEntity(this.replyManager.Object, reply.Id, this.searchService.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400, Times.Once);

            this.annotationManager.As<IContainedEntityManager<Annotation>>()
                .Setup(x => x.FindEntity(this.commentId)).ReturnsAsync(comment);

            this.annotationManager.As<IContainedEntityManager<Annotation>>()
                .Setup(x => x.EntityIsContainedBy(this.commentId, this.projectId)).ReturnsAsync(true);

            this.replyManager.As<IContainedEntityManager<Reply>>()
                .Setup(x => x.FindEntityWithContainer(reply.Id)).ReturnsAsync(reply);

            this.replyManager.Setup(x => x.DeleteEntity(reply))
                .ReturnsAsync(EntityOperationResult<Reply>.Success(null));

            var deleteResponse = await this.module.DeleteEntity(this.replyManager.Object, reply.Id, this.searchService.Object, this.context.Object);
            Assert.That(deleteResponse.IsRequestSuccessful, Is.True);
        }

        [Test]
        public async Task VerifyUpdateEntity()
        {
            this.annotationManager.As<IContainedEntityManager<Annotation>>()
                .Setup(x => x.EntityIsContainedBy(this.commentId, this.projectId)).ReturnsAsync(false);

            var reply = new Reply(Guid.NewGuid())
            {
                Author = new Participant(Guid.NewGuid())
                {
                    Role = new Role(Guid.NewGuid()),
                    User = new UserEntity(Guid.NewGuid())
                },
                Content = "Reply content"
            };

            var comment = new Comment(this.commentId)
            {
                Replies = { reply }
            };

            var dto = reply.ToDto() as ReplyDto;

            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync((Participant)null);

            await this.module.UpdateEntity(this.replyManager.Object, reply.Id, dto, this.searchService.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 401, Times.Once);

            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync(new Participant(Guid.NewGuid()));
            this.replyManager.Setup(x => x.GetEntity(reply.Id, 0)).ReturnsAsync(new List<Entity> { reply });

            await this.module.UpdateEntity(this.replyManager.Object, reply.Id, dto, this.searchService.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 403, Times.Once);

            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync(reply.Author);

            await this.module.UpdateEntity(this.replyManager.Object, reply.Id, dto, this.searchService.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400, Times.Once);

            this.annotationManager.As<IContainedEntityManager<Annotation>>()
                .Setup(x => x.FindEntity(this.commentId)).ReturnsAsync(comment);

            this.annotationManager.As<IContainedEntityManager<Annotation>>()
                .Setup(x => x.EntityIsContainedBy(this.commentId, this.projectId)).ReturnsAsync(true);

            this.replyManager.As<IContainedEntityManager<Reply>>()
                .Setup(x => x.FindEntityWithContainer(reply.Id)).ReturnsAsync(reply);

            this.replyManager.Setup(x => x.UpdateEntity(reply))
                .ReturnsAsync(EntityOperationResult<Reply>.Success(reply));

            await this.module.UpdateEntity(this.replyManager.Object, reply.Id, dto, this.searchService.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 200, Times.Once);
        }
    }
}
