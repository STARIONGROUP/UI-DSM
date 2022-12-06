// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewItemModuleTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Tests.Modules
{
    using System.Security.Claims;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Server.Managers;
    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Server.Managers.ReviewItemManager;
    using UI_DSM.Server.Managers.ReviewManager;
    using UI_DSM.Server.Modules;
    using UI_DSM.Server.Tests.Helpers;
    using UI_DSM.Server.Types;
    using UI_DSM.Server.Validator;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class ReviewItemModuleTestFixture
    {
        private ReviewItemModule module;
        private Mock<IEntityManager<ReviewItem>> reviewItemManager;
        private Mock<HttpContext> context;
        private Mock<HttpResponse> response;
        private Mock<HttpRequest> request;
        private Mock<IServiceProvider> serviceProvider;
        private RouteValueDictionary routeValues;
        private Mock<IParticipantManager> participantManager;
        private Mock<IEntityManager<Review>> reviewManager;
        private Guid projectId;
        private Guid reviewId;

        [SetUp]
        public void Setup()
        {
            this.reviewItemManager = new Mock<IEntityManager<ReviewItem>>();
            this.reviewItemManager.As<IReviewItemManager>();
            this.reviewItemManager.As<IContainedEntityManager<ReviewItem>>();
            this.reviewManager = new Mock<IEntityManager<Review>>();
            this.reviewManager.As<IContainedEntityManager<Review>>();
            this.reviewManager.As<IReviewManager>();
            this.participantManager = new Mock<IParticipantManager>();

            ModuleTestHelper.Setup<ReviewItemModule, ReviewItemDto>(new ReviewItemDtoValidator(), out this.context,
                out this.response, out this.request, out this.serviceProvider);

            this.routeValues = new RouteValueDictionary();
            this.request.Setup(x => x.RouteValues).Returns(this.routeValues);
            this.projectId = Guid.NewGuid();
            this.reviewId = Guid.NewGuid();

            this.routeValues["projectId"] = this.projectId.ToString();
            this.routeValues["reviewId"] = this.reviewId.ToString();

            this.serviceProvider.Setup(x => x.GetService(typeof(IContainedEntityManager<Review>))).Returns(this.reviewManager.Object);
            this.serviceProvider.Setup(x => x.GetService(typeof(IEntityManager<Review>))).Returns(this.reviewManager.Object);
            this.serviceProvider.Setup(x => x.GetService(typeof(IParticipantManager))).Returns(this.participantManager.Object);

            this.context.Setup(x => x.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()
            {
                new (ClaimTypes.Name, "user")
            })));

            this.module = new ReviewItemModule();
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

            var reviewItems = new List<ReviewItem>()
            {
                new(Guid.NewGuid())
                {
                   ThingId = Guid.NewGuid()
                },
                new(Guid.NewGuid())
                {
                    ThingId = Guid.NewGuid()
                }
            };

            this.serviceProvider.Setup(x => x.GetService(typeof(IParticipantManager))).Returns(this.participantManager.Object);
            this.participantManager.Setup(x => x.GetParticipantForProject(It.IsAny<Guid>(), "user")).ReturnsAsync((Participant)null);

            await this.module.GetEntities(this.reviewItemManager.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 401, Times.Once);
            this.participantManager.Setup(x => x.GetParticipantForProject(It.IsAny<Guid>(), "user")).ReturnsAsync(participant);
            
            this.reviewManager.As<IContainedEntityManager<Review>>()
                .Setup(x => x.EntityIsContainedBy(this.reviewId, this.projectId)).ReturnsAsync(true);

            this.reviewItemManager.As<IReviewItemManager>().Setup(x => x.GetContainedEntities(this.reviewId, 0))
                .ReturnsAsync(reviewItems);

            await this.module.GetEntities(this.reviewItemManager.Object, this.context.Object);
            this.reviewItemManager.As<IReviewItemManager>().Verify(x => x.GetContainedEntities(this.reviewId, 0), Times.Once);
        }

        [Test]
        public async Task VerifyGetEntity()
        {
            var participant = new Participant(Guid.NewGuid())
            {
                Role = new Role(Guid.NewGuid()),
                User = new UserEntity(Guid.NewGuid())
            };

            var reviewItem = new ReviewItem(Guid.NewGuid())
            {
                ThingId = Guid.NewGuid()
            };

            _ = new Review(this.reviewId)
            {
                ReviewItems = { reviewItem }
            };

            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync((Participant)null);

            await this.module.GetEntity(this.reviewItemManager.Object, reviewItem.Id, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 401, Times.Once);
            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync(participant);

            this.reviewManager.As<IContainedEntityManager<Review>>()
                .Setup(x => x.EntityIsContainedBy(this.reviewId, this.projectId)).ReturnsAsync(true);

            await this.module.GetEntity(this.reviewItemManager.Object, reviewItem.Id, this.context.Object);

            this.reviewItemManager.As<IContainedEntityManager<ReviewItem>>()
                .Verify(x => x.FindEntityWithContainer(reviewItem.Id), Times.Once);
        }

        [Test]
        public async Task VerifyCreateEntity()
        {
            var participant = new Participant(Guid.NewGuid())
            {
                Role = new Role(Guid.NewGuid()),
                User = new UserEntity(Guid.NewGuid())
            };

            var reviewItemDto = new ReviewItemDto()
            {
                ThingId = Guid.NewGuid()
            };

            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync((Participant)null);

            await this.module.CreateEntity(this.reviewItemManager.Object, reviewItemDto, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 401, Times.Once);
            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync(participant);

            this.reviewItemManager.As<IReviewItemManager>().Setup(x => x.GetReviewItemForThing(this.reviewId, reviewItemDto.ThingId, 0))
                .ReturnsAsync(new List<Entity>
                {
                    new ReviewItem(Guid.NewGuid())
                });

            await this.module.CreateEntity(this.reviewItemManager.Object, reviewItemDto, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 409, Times.Once);

            this.reviewItemManager.As<IReviewItemManager>().Setup(x => x.GetReviewItemForThing(this.reviewId, reviewItemDto.ThingId, 0))
                .ReturnsAsync(Enumerable.Empty<Entity>());

            this.reviewManager.As<IContainedEntityManager<Review>>()
                .Setup(x => x.EntityIsContainedBy(this.reviewId, this.projectId)).ReturnsAsync(true);

            this.reviewManager.Setup(x => x.GetEntity(this.reviewId, 0)).ReturnsAsync(new List<Entity>
            {
                new Review()
            });

            this.reviewItemManager.Setup(x => x.CreateEntity(It.IsAny<ReviewItem>()))
                .ReturnsAsync(EntityOperationResult<ReviewItem>.Success(new ReviewItem(Guid.NewGuid())));

            await this.module.CreateEntity(this.reviewItemManager.Object, reviewItemDto, this.context.Object);
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

            this.reviewManager.As<IContainedEntityManager<Review>>()
                .Setup(x => x.EntityIsContainedBy(this.reviewId, this.projectId)).ReturnsAsync(true);

            var reviewItem = new ReviewItem(Guid.NewGuid());

            _ = new Review(this.reviewId)
            {
                ReviewItems = { reviewItem }
            };

            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync((Participant)null);

            await this.module.DeleteEntity(this.reviewItemManager.Object, reviewItem.Id, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 401, Times.Once);

            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync(participant);

            this.reviewItemManager.As<IContainedEntityManager<ReviewItem>>()
                .Setup(x => x.FindEntityWithContainer(reviewItem.Id)).ReturnsAsync(reviewItem);

            this.reviewItemManager.Setup(x => x.DeleteEntity(reviewItem))
                .ReturnsAsync(EntityOperationResult<ReviewItem>.Success(null));

            var deleteResponse = await this.module.DeleteEntity(this.reviewItemManager.Object, reviewItem.Id, this.context.Object);
            Assert.That(deleteResponse.IsRequestSuccessful, Is.True);
        }

        [Test]
        public async Task VerifyUpdateEntity()
        {
            await this.module.UpdateEntity(this.reviewItemManager.Object, Guid.NewGuid(), new ReviewItemDto(), this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 401, Times.Once);

            var participant = new Participant(Guid.NewGuid())
            {
                Role = new Role(Guid.NewGuid()),
                User = new UserEntity(Guid.NewGuid())
            };

            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync(participant);
            await this.module.UpdateEntity(this.reviewItemManager.Object, Guid.NewGuid(), new ReviewItemDto(), this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 401, Times.Once);
        }

        [Test]
        public async Task VerifyGetEntitiesForThings()
        {
            var participant = new Participant(Guid.NewGuid())
            {
                Role = new Role(Guid.NewGuid()),
                User = new UserEntity(Guid.NewGuid())
            };

            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync((Participant)null);

            await this.module.GetEntityForThing(this.reviewItemManager.As<IReviewItemManager>().Object, Guid.NewGuid(), this.context.Object);
            await this.module.GetEntitiesForThings(this.reviewItemManager.As<IReviewItemManager>().Object, new List<Guid>(), this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 401, Times.Exactly(2));

            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync(participant);

            this.reviewManager.As<IContainedEntityManager<Review>>()
                .Setup(x => x.EntityIsContainedBy(this.reviewId, this.projectId)).ReturnsAsync(false);

            await this.module.GetEntityForThing(this.reviewItemManager.As<IReviewItemManager>().Object, Guid.NewGuid(), this.context.Object);
            await this.module.GetEntitiesForThings(this.reviewItemManager.As<IReviewItemManager>().Object, new List<Guid>(), this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400, Times.Exactly(2));

            this.reviewManager.As<IContainedEntityManager<Review>>()
                .Setup(x => x.EntityIsContainedBy(this.reviewId, this.projectId)).ReturnsAsync(true);

            await this.module.GetEntityForThing(this.reviewItemManager.As<IReviewItemManager>().Object, Guid.NewGuid(), this.context.Object);
            await this.module.GetEntitiesForThings(this.reviewItemManager.As<IReviewItemManager>().Object, new List<Guid>(), this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400, Times.Exactly(2));
        }
    }
}
