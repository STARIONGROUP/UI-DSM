// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewObjectiveModuleTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Tests.Modules
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Server.Managers;
    using UI_DSM.Server.Managers.ReviewObjectiveManager;
    using UI_DSM.Server.Modules;
    using UI_DSM.Server.Tests.Helpers;
    using UI_DSM.Server.Validator;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    using System.Security.Claims;

    using CDP4JsonSerializer;

    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Serializer.Json;
    using UI_DSM.Server.Managers.ReviewManager;
    using UI_DSM.Server.Managers.ReviewTaskManager;
    using UI_DSM.Server.Types;
    using UI_DSM.Shared.DTO.Common;
    using Microsoft.EntityFrameworkCore;
    using UI_DSM.Server.Managers.ProjectManager;

    [TestFixture]
    public class ReviewObjectiveModuleTestFixture
    {
        private ReviewObjectiveModule module;
        private Mock<IEntityManager<ReviewObjective>> reviewObjectiveManager;
        private Mock<HttpContext> context;
        private Mock<HttpResponse> response;
        private Mock<HttpRequest> request;
        private Mock<IServiceProvider> serviceProvider;
        private Mock<IEntityManager<Review>> reviewManager;
        private Mock<IParticipantManager> participantManager;
        private Mock<IReviewTaskManager> reviewTaskManager;
        private RouteValueDictionary routeValues;
        private Guid projectId;
        private Guid reviewId;
        private IJsonService jsonService;

        [SetUp]
        public void Setup()
        {
            this.reviewObjectiveManager = new Mock<IEntityManager<ReviewObjective>>();
            this.reviewObjectiveManager.As<IReviewObjectiveManager>();
            this.reviewObjectiveManager.As<IContainedEntityManager<ReviewObjective>>();
            this.reviewManager = new Mock<IEntityManager<Review>>();
            this.reviewManager.As<IContainedEntityManager<Review>>();
            this.reviewManager.As<IReviewManager>();
            this.participantManager = new Mock<IParticipantManager>();
            this.reviewTaskManager = new Mock<IReviewTaskManager>();

            ModuleTestHelper.Setup<ReviewObjectiveModule, ReviewObjectiveDto>(new ReviewObjectiveDtoValidator(), out this.context,
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

            this.jsonService = new JsonService(new JsonDeserializer(), new JsonSerializer(), new Cdp4JsonSerializer());
            this.module = new ReviewObjectiveModule(this.jsonService);
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

            var reviewObjectives = new List<ReviewObjective>()
            {
                new(Guid.NewGuid())
                {
                    Author = participant,
                    Description = "A review",
                    ReviewObjectiveNumber = 1,
                    Status = StatusKind.Closed
                },
                new(Guid.NewGuid())
                {
                    Author = participant,
                    Description = "An other review",
                    ReviewObjectiveNumber = 2,
                    Status = StatusKind.Open
                }
            };

            this.serviceProvider.Setup(x => x.GetService(typeof(IParticipantManager))).Returns(null);

            await this.module.GetEntities(this.reviewObjectiveManager.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 500, Times.Once);
            this.serviceProvider.Setup(x => x.GetService(typeof(IParticipantManager))).Returns(this.participantManager.Object);

            this.participantManager.Setup(x => x.GetParticipantForProject(It.IsAny<Guid>(), "user")).ReturnsAsync((Participant)null);

            await this.module.GetEntities(this.reviewObjectiveManager.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 401, Times.Once);
            this.participantManager.Setup(x => x.GetParticipantForProject(It.IsAny<Guid>(), "user")).ReturnsAsync(participant);

            this.reviewManager.As<IContainedEntityManager<Review>>()
                .Setup(x => x.EntityIsContainedBy(this.reviewId, this.projectId)).ReturnsAsync(false);

            this.serviceProvider.Setup(x => x.GetService(typeof(IContainedEntityManager<Review>))).Returns(null);

            await this.module.GetEntities(this.reviewObjectiveManager.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode =500, Times.Exactly(2));

            this.serviceProvider.Setup(x => x.GetService(typeof(IContainedEntityManager<Review>))).Returns(this.reviewManager.Object);
            await this.module.GetEntities(this.reviewObjectiveManager.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400, Times.Once);

            this.reviewManager.As<IContainedEntityManager<Review>>()
                .Setup(x => x.EntityIsContainedBy(this.reviewId, this.projectId)).ReturnsAsync(true);

            this.reviewObjectiveManager.As<IReviewObjectiveManager>().Setup(x => x.GetContainedEntities(this.reviewId, 0))
                .ReturnsAsync(reviewObjectives);

            await this.module.GetEntities(this.reviewObjectiveManager.Object, this.context.Object);
            this.reviewObjectiveManager.As<IReviewObjectiveManager>().Verify(x => x.GetContainedEntities(this.reviewId, 0), Times.Once);
        }

        [Test]
        public async Task VerifyGetEntity()
        {
            var reviewObjective = new ReviewObjective(Guid.NewGuid())
            {
                Author = new Participant(Guid.NewGuid())
                {
                    Role = new Role(Guid.NewGuid()),
                    User = new UserEntity(Guid.NewGuid())
                }
            };

            _ = new Review(this.reviewId)
            {
                ReviewObjectives = { reviewObjective }
            };

            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync((Participant)null);

            await this.module.GetEntity(this.reviewObjectiveManager.Object, reviewObjective.Id, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 401, Times.Once);
            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync(reviewObjective.Author);

            this.reviewManager.As<IContainedEntityManager<Review>>()
                .Setup(x => x.EntityIsContainedBy(this.reviewId, this.projectId)).ReturnsAsync(false);

            this.reviewObjectiveManager.As<IContainedEntityManager<ReviewObjective>>()
                .Setup(x => x.FindEntityWithContainer(reviewObjective.Id)).ReturnsAsync(reviewObjective);

            await this.module.GetEntity(this.reviewObjectiveManager.Object, reviewObjective.Id, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400, Times.Once);

            this.reviewManager.As<IContainedEntityManager<Review>>()
                .Setup(x => x.EntityIsContainedBy(this.reviewId, this.projectId)).ReturnsAsync(true);

            await this.module.GetEntity(this.reviewObjectiveManager.Object, reviewObjective.Id, this.context.Object);
            
            this.reviewObjectiveManager.As<IContainedEntityManager<ReviewObjective>>()
                .Verify(x => x.FindEntityWithContainer(reviewObjective.Id), Times.Once);
        }

        [Test]
        public async Task VerifyCreateEntity()
        {
            var dto = new ReviewObjectiveDto()
            {
                Description = "Description",
                Title = "Title"
            };

            await this.module.CreateEntity(this.reviewObjectiveManager.Object, dto, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 405, Times.Once);
        }

        [Test]
        public async Task VerifyCreateEntityWithTemplate()
        {
            var reviewCreationDto = new ReviewObjectiveCreationDto()
            {
                Kind = ReviewObjectiveKind.Prr,
                KindNumber = 15
            };

            await this.module.CreateEntityTemplate(this.reviewManager.As<IReviewManager>().Object, this.reviewObjectiveManager.As<IReviewObjectiveManager>().Object,
                this.reviewTaskManager.Object, this.context.Object, reviewCreationDto);

            this.response.VerifySet(x => x.StatusCode = 400, Times.Once);

            reviewCreationDto.KindNumber = 2;
            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync((Participant)null);

            await this.module.CreateEntityTemplate(this.reviewManager.As<IReviewManager>().Object, this.reviewObjectiveManager.As<IReviewObjectiveManager>().Object,
                this.reviewTaskManager.Object, this.context.Object, reviewCreationDto);

            this.response.VerifySet(x => x.StatusCode = 401, Times.Once);

            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync(new Participant(Guid.NewGuid())
            {
                Role = new Role(Guid.NewGuid()),
                User = new UserEntity(Guid.NewGuid())
            });

            await this.module.CreateEntityTemplate(this.reviewManager.As<IReviewManager>().Object, this.reviewObjectiveManager.As<IReviewObjectiveManager>().Object,
                this.reviewTaskManager.Object, this.context.Object, reviewCreationDto);

            this.response.VerifySet(x => x.StatusCode = 400, Times.Exactly(2));

            this.reviewManager.As<IContainedEntityManager<Review>>()
                .Setup(x => x.EntityIsContainedBy(this.reviewId, this.projectId)).ReturnsAsync(true);

            this.reviewManager.Setup(x => x.GetEntity(this.reviewId, 0)).ReturnsAsync(Enumerable.Empty<Entity>());

            await this.module.CreateEntityTemplate(this.reviewManager.As<IReviewManager>().Object, this.reviewObjectiveManager.As<IReviewObjectiveManager>().Object,
                this.reviewTaskManager.Object, this.context.Object, reviewCreationDto);

            this.response.VerifySet(x => x.StatusCode = 400, Times.Exactly(3));

            this.reviewManager.Setup(x => x.GetEntity(this.reviewId, 0)).ReturnsAsync(new List<Entity>
            {
                new Review()
            });

            this.reviewObjectiveManager.As<IReviewObjectiveManager>().Setup(x => x.CreateEntityBasedOnTemplate(It.IsAny<ReviewObjective>()
                , It.IsAny<Review>(), It.IsAny<Participant>())).ReturnsAsync(EntityOperationResult<ReviewObjective>.Success(new ReviewObjective(new ReviewObjective())));

            await this.module.CreateEntityTemplate(this.reviewManager.As<IReviewManager>().Object, this.reviewObjectiveManager.As<IReviewObjectiveManager>().Object,
                this.reviewTaskManager.Object, this.context.Object, reviewCreationDto);

            this.response.VerifySet(x => x.StatusCode=201, Times.Once);
        }

        [Test]
        public async Task VerifyCreateEntitiesWithTemplate()
        {
            var reviewCreationDto = new ReviewObjectiveCreationDto()
            {
                Kind = ReviewObjectiveKind.Prr,
                KindNumber = 15
            };

            var reviewCreationDto1 = new ReviewObjectiveCreationDto()
            {
                Kind = ReviewObjectiveKind.Prr,
                KindNumber = 3
            };

            List<ReviewObjectiveCreationDto> reviewCreationDtos = new List<ReviewObjectiveCreationDto>
            {
                reviewCreationDto
            };

            await this.module.CreateEntityTemplates(this.reviewManager.As<IReviewManager>().Object, this.reviewObjectiveManager.As<IReviewObjectiveManager>().Object,
                this.context.Object, reviewCreationDtos);

            this.response.VerifySet(x => x.StatusCode = 400, Times.Once);

            reviewCreationDto.KindNumber = 2;
            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync((Participant)null);

            reviewCreationDtos.Add(reviewCreationDto1);

            await this.module.CreateEntityTemplates(this.reviewManager.As<IReviewManager>().Object, this.reviewObjectiveManager.As<IReviewObjectiveManager>().Object,
                this.context.Object, reviewCreationDtos);

            this.response.VerifySet(x => x.StatusCode = 401, Times.Once);

            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync(new Participant(Guid.NewGuid())
            {
                Role = new Role(Guid.NewGuid()),
                User = new UserEntity(Guid.NewGuid())
            });

            await this.module.CreateEntityTemplates(this.reviewManager.As<IReviewManager>().Object, this.reviewObjectiveManager.As<IReviewObjectiveManager>().Object,
                this.context.Object, reviewCreationDtos);

            this.response.VerifySet(x => x.StatusCode = 400, Times.Exactly(2));

            this.reviewManager.As<IContainedEntityManager<Review>>()
                .Setup(x => x.EntityIsContainedBy(this.reviewId, this.projectId)).ReturnsAsync(true);

            this.reviewManager.Setup(x => x.GetEntity(this.reviewId, 0)).ReturnsAsync(Enumerable.Empty<Entity>());

            await this.module.CreateEntityTemplates(this.reviewManager.As<IReviewManager>().Object, this.reviewObjectiveManager.As<IReviewObjectiveManager>().Object,
                this.context.Object, reviewCreationDtos);

            this.response.VerifySet(x => x.StatusCode = 400, Times.Exactly(3));

            this.reviewManager.Setup(x => x.GetEntity(this.reviewId, 0)).ReturnsAsync(new List<Entity>
            {
                new Review()
            });

            this.reviewObjectiveManager.As<IReviewObjectiveManager>().Setup(x => x.CreateEntityBasedOnTemplates(It.IsAny<IEnumerable<ReviewObjective>>()
                , It.IsAny<Review>(), It.IsAny<Participant>())).ReturnsAsync(EntityOperationResult<ReviewObjective>.AllSuccess(new List<ReviewObjective> { new ReviewObjective() }));

            await this.module.CreateEntityTemplates(this.reviewManager.As<IReviewManager>().Object, this.reviewObjectiveManager.As<IReviewObjectiveManager>().Object,
                this.context.Object, reviewCreationDtos);

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
                .Setup(x => x.EntityIsContainedBy(this.reviewId, this.projectId)).ReturnsAsync(false);

            var reviewObjective = new ReviewObjective(Guid.NewGuid());

            _ = new Review(this.reviewId)
            {
                ReviewObjectives = { reviewObjective }
            };

            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync((Participant)null);

            await this.module.DeleteEntity(this.reviewObjectiveManager.Object, reviewObjective.Id, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 401, Times.Once);

            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync(participant);

            await this.module.DeleteEntity(this.reviewObjectiveManager.Object, reviewObjective.Id, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400, Times.Once);

            this.reviewManager.As<IContainedEntityManager<Review>>()
                .Setup(x => x.EntityIsContainedBy(this.reviewId, this.projectId)).ReturnsAsync(true);

            this.reviewObjectiveManager.As<IContainedEntityManager<ReviewObjective>>()
                .Setup(x => x.FindEntityWithContainer(reviewObjective.Id)).ReturnsAsync(reviewObjective);

            this.reviewObjectiveManager.Setup(x => x.DeleteEntity(reviewObjective))
                .ReturnsAsync(EntityOperationResult<ReviewObjective>.Success(null));

            var deleteResponse = await this.module.DeleteEntity(this.reviewObjectiveManager.Object, reviewObjective.Id, this.context.Object);
            Assert.That(deleteResponse.IsRequestSuccessful, Is.True);
        }

        [Test]
        public async Task VerifyUpdateEntity()
        {
            this.reviewManager.As<IContainedEntityManager<Review>>()
                .Setup(x => x.EntityIsContainedBy(this.reviewId, this.projectId)).ReturnsAsync(false);

            var reviewObjective = new ReviewObjective(Guid.NewGuid())
            {
                Author = new Participant(Guid.NewGuid())
                {
                    Role = new Role(Guid.NewGuid()),
                    User = new UserEntity(Guid.NewGuid())
                },
                Description = "Description",
            };

            _ = new Review(this.reviewId)
            {
                ReviewObjectives = { reviewObjective }
            };

            var dto = reviewObjective.ToDto() as ReviewObjectiveDto;

            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync((Participant)null);

            await this.module.UpdateEntity(this.reviewObjectiveManager.Object, reviewObjective.Id, dto, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 401, Times.Once);

            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync(reviewObjective.Author);

            await this.module.UpdateEntity(this.reviewObjectiveManager.Object, reviewObjective.Id, dto, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400, Times.Once);

            this.reviewManager.As<IContainedEntityManager<Review>>()
                .Setup(x => x.EntityIsContainedBy(this.reviewId, this.projectId)).ReturnsAsync(true);

            this.reviewObjectiveManager.As<IContainedEntityManager<ReviewObjective>>()
                .Setup(x => x.FindEntityWithContainer(reviewObjective.Id)).ReturnsAsync(reviewObjective);

            this.reviewObjectiveManager.Setup(x => x.UpdateEntity(reviewObjective))
                .ReturnsAsync(EntityOperationResult<ReviewObjective>.Success(reviewObjective));

            await this.module.UpdateEntity(this.reviewObjectiveManager.Object, reviewObjective.Id, dto, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 200, Times.Once);
        }

        [Test]
        public async Task VerifyGetAvailableTemplates()
        {
            var reviewObjective = new ReviewObjective(Guid.NewGuid());
            Review review = new Review(this.reviewId);

            this.reviewObjectiveManager.As<IReviewObjectiveManager>().Setup(x => x.GetReviewObjectiveCreationForReview(review.Id)).ReturnsAsync(new List<ReviewObjectiveCreationDto>());

            await this.module.GetAvailableTemplates(this.reviewObjectiveManager.As<IReviewObjectiveManager>().Object, this.context.Object);
            this.reviewObjectiveManager.As<IReviewObjectiveManager>().Verify(x => x.GetReviewObjectiveCreationForReview(review.Id), Times.Once);
        }
    }
}
