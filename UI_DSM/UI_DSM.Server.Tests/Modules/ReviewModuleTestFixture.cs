// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewModuleTestFixture.cs" company="RHEA System S.A.">
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
    using System.Security.Claims;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;
    
    using Moq;

    using NUnit.Framework;

    using UI_DSM.Server.Managers;
    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Server.Managers.ReviewManager;
    using UI_DSM.Server.Modules;
    using UI_DSM.Server.Tests.Helpers;
    using UI_DSM.Server.Types;
    using UI_DSM.Server.Validator;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class ReviewModuleTestFixture
    {
        private ReviewModule module;
        private Mock<IEntityManager<Review>> reviewManager;
        private Mock<HttpContext> context;
        private Mock<HttpResponse> response;
        private Mock<HttpRequest> request;
        private Mock<IServiceProvider> serviceProvider;
        private Mock<IEntityManager<Project>> projectManager;
        private Mock<IParticipantManager> participantManager;
        private RouteValueDictionary routeValues;

        [SetUp]
        public void Setup()
        {
            this.projectManager = new Mock<IEntityManager<Project>>();
            this.participantManager = new Mock<IParticipantManager>();
            this.reviewManager = new Mock<IEntityManager<Review>>();
            this.reviewManager.As<IReviewManager>();
            this.reviewManager.As<IContainedEntityManager<Review>>();

            ModuleTestHelper.Setup<ReviewModule, ReviewDto>(new ReviewDtoValidator(), out this.context,
                out this.response, out this.request, out this.serviceProvider);

            this.routeValues = new RouteValueDictionary();
            this.request.Setup(x => x.RouteValues).Returns(this.routeValues);
            
            this.serviceProvider.Setup(x => x.GetService(typeof(IEntityManager<Project>))).Returns(this.projectManager.Object);
            this.serviceProvider.Setup(x => x.GetService(typeof(IParticipantManager))).Returns(this.participantManager.Object);

            this.context.Setup(x => x.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()
            {
                new (ClaimTypes.Name, "user")
            })));

            this.module = new ReviewModule();
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

            var reviews = new List<Review>()
            {
                new(Guid.NewGuid())
                {
                    Author = participant,
                    Description = "A review",
                    Title = "Title review",
                    ReviewNumber = 1
                },
                new(Guid.NewGuid())
                {
                    Author = participant,
                    Description = "An other review",
                    Title = "Title review 2 ",
                    ReviewNumber = 2
                }
            };

            var projectId = Guid.NewGuid();
            this.routeValues["projectId"] = projectId.ToString();

            this.participantManager.Setup(x => x.GetParticipantForProject(projectId, "user")).ReturnsAsync((Participant)null);

            await this.module.GetEntities(this.reviewManager.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 401, Times.Once);

            this.participantManager.Setup(x => x.GetParticipantForProject(projectId, "user")).ReturnsAsync(participant);

            this.reviewManager.As<IReviewManager>().Setup(x => x.GetContainedEntities(projectId, 0))
                .ReturnsAsync(reviews);

            await this.module.GetEntities(this.reviewManager.Object, this.context.Object);
            this.reviewManager.As<IReviewManager>().Verify(x => x.GetContainedEntities(projectId, 0), Times.Once);
        }

        [Test]
        public async Task VerifyGetEntity()
        {
            var project = new Project(Guid.NewGuid());

            var review = new Review(Guid.NewGuid())
            {
                Author = new Participant(Guid.NewGuid())
                {
                    Role = new Role(Guid.NewGuid()),
                    User = new UserEntity(Guid.NewGuid())
                }
            };

            this.routeValues["projectId"] = Guid.NewGuid().ToString();

            this.participantManager.Setup(x => x.GetParticipantForProject(It.IsAny<Guid>(), "user")).ReturnsAsync((Participant)null);

            await this.module.GetEntities(this.reviewManager.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 401, Times.Once);

            this.participantManager.Setup(x => x.GetParticipantForProject(It.IsAny<Guid>(), "user")).ReturnsAsync(review.Author);

            this.reviewManager.As<IContainedEntityManager<Review>>()
                .Setup(x => x.FindEntityWithContainer(review.Id)).ReturnsAsync((Review)null);

            await this.module.GetEntity(this.reviewManager.Object, review.Id, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 404, Times.Once);

            this.reviewManager.As<IContainedEntityManager<Review>>()
                .Setup(x => x.FindEntityWithContainer(review.Id)).ReturnsAsync(review);

            await this.module.GetEntity(this.reviewManager.Object, review.Id, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400, Times.Once);

            project.Reviews.Add(review);
            await this.module.GetEntity(this.reviewManager.Object, review.Id, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400, Times.Exactly(2));

            this.routeValues["projectId"] = project.Id.ToString();
            await this.module.GetEntity(this.reviewManager.Object, review.Id, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400, Times.Exactly(2));
        }

        [Test]
        public async Task VerifyCreateEntity()
        {
            var project = new Project(Guid.NewGuid());
            var dto = new ReviewDto();

            this.serviceProvider.Setup(x => x.GetService(typeof(IParticipantManager))).Returns(null);

            await this.module.CreateEntity(this.reviewManager.Object, dto, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 500, Times.Once);

            this.routeValues["projectId"] = project.Id.ToString();
            this.serviceProvider.Setup(x => x.GetService(typeof(IParticipantManager))).Returns(this.participantManager.Object);

            this.participantManager.Setup(x => x.GetParticipantForProject(project.Id, "user")).ReturnsAsync((Participant)null);

            await this.module.CreateEntity(this.reviewManager.Object, dto, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 401, Times.Once);

            var participant = new Participant(Guid.NewGuid())
            {
                User = new UserEntity(Guid.NewGuid()),
                Role = new Role(Guid.NewGuid())
            };

            this.participantManager.Setup(x => x.GetParticipantForProject(project.Id, "user")).ReturnsAsync(participant);
            await this.module.CreateEntity(this.reviewManager.Object, dto, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 422, Times.Once);

            dto.Author = participant.Id;
            dto.Description = "A description";
            dto.Title = "a title";

            await this.module.CreateEntity(this.reviewManager.Object, dto, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400, Times.Once);
            this.projectManager.Setup(x => x.FindEntity(project.Id)).ReturnsAsync(project);
            this.reviewManager.Setup(x => x.CreateEntity(It.IsAny<Review>())).ReturnsAsync(EntityOperationResult<Review>.Success(new Review(Guid.NewGuid())));
            await this.module.CreateEntity(this.reviewManager.Object, dto, this.context.Object);

            this.response.VerifySet(x => x.StatusCode = 201, Times.Once);
        }

        [Test]
        public async Task VerifyDeleteEntity()
        {
            var review = new Review(Guid.NewGuid())
            {
                Author = new Participant(Guid.NewGuid())
            };

            var project = new Project(Guid.NewGuid())
            {
                Reviews = { review }
            };

            this.routeValues["projectId"] = project.Id.ToString();

            this.participantManager.Setup(x => x.GetParticipantForProject(It.IsAny<Guid>(), "user")).ReturnsAsync((Participant)null);

            await this.module.DeleteEntity(this.reviewManager.Object, review.Id, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 401, Times.Once);

            this.participantManager.Setup(x => x.GetParticipantForProject(It.IsAny<Guid>(), "user")).ReturnsAsync(review.Author);

            this.reviewManager.As<IContainedEntityManager<Review>>().Setup(x => x.FindEntityWithContainer(review.Id)).ReturnsAsync(review);
            this.reviewManager.Setup(x => x.DeleteEntity(review)).ReturnsAsync(EntityOperationResult<Review>.Success(null));
            var deleteResult = await this.module.DeleteEntity(this.reviewManager.Object, review.Id, this.context.Object);
            Assert.That(deleteResult.IsRequestSuccessful, Is.True);
        }

        [Test]
        public async Task VerifyUpdateEntity()
        {
            var participant = new Participant(Guid.NewGuid())
            {
                Role = new Role(Guid.NewGuid()),
                User = new UserEntity(Guid.NewGuid())
            };

            var review = new Review(Guid.NewGuid())
            {
                Author = participant,
                Description = "Description",
                Title = "Title"
            };

            var reviewDto = new ReviewDto()
            {
                Author = participant.Id,
                Description = "Description",
                Title = "Title",
                Status = StatusKind.Closed
            };

            var project = new Project(Guid.NewGuid())
            {
                Reviews = { review }
            };

            this.routeValues["entityId"] = review.Id.ToString();
            this.routeValues["projectId"] = project.Id.ToString();

            this.participantManager.Setup(x => x.GetParticipantForProject(It.IsAny<Guid>(), "user")).ReturnsAsync((Participant)null);

            await this.module.UpdateEntity(this.reviewManager.Object, review.Id, reviewDto, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 401, Times.Once);

            this.participantManager.Setup(x => x.GetParticipantForProject(It.IsAny<Guid>(), "user")).ReturnsAsync(review.Author);

            this.reviewManager.As<IContainedEntityManager<Review>>().Setup(x => x.FindEntityWithContainer(review.Id)).ReturnsAsync(review);
            this.reviewManager.Setup(x => x.UpdateEntity(It.IsAny<Review>())).ReturnsAsync(EntityOperationResult<Review>.Success(review));
            await this.module.UpdateEntity(this.reviewManager.Object, review.Id, reviewDto, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 200, Times.Once);
        }

         [Test]
        public async Task VerifyGetOpenTasksAndComments()
        {
            var reviews = new List<Review>
            {
                new(Guid.NewGuid())
            };

            Project project = new(Guid.NewGuid());
            project.Reviews.AddRange(reviews);


            var guids = reviews.Select(x => x.Id).ToList();
            var result = new Dictionary<Guid, ComputedProjectProperties>
            {
                [guids[0]] = new()
                {
                    CommentCount = 15,
                    TaskCount = 12
                }
            };
            this.reviewManager.As<IReviewManager>().Setup(x => x.GetOpenTasksAndComments(It.IsAny<IEnumerable<Guid>>())).Returns(result);
            await this.module.GetOpenTasksAndComments(this.reviewManager.As<IReviewManager>().Object,project.Id, this.context.Object);
            this.reviewManager.As<IReviewManager>().Verify(x => x.GetOpenTasksAndComments(It.IsAny<IEnumerable<Guid>>()), Times.Once);
        }
    }
}
