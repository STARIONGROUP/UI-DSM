// --------------------------------------------------------------------------------------------------------
// <copyright file="AnnotationModuleTestFixture.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to annotation an ECSS-E-TM-10-25 model.
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

    using System.Security.Claims;

    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Server.Managers;
    using UI_DSM.Server.Managers.AnnotationManager;
    using UI_DSM.Server.Modules;
    using UI_DSM.Shared.Models;
    using UI_DSM.Server.Tests.Helpers;
    using UI_DSM.Server.Validator;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Server.Types;

    [TestFixture]
    public class AnnotationModuleTestFixture
    {
        private AnnotationModule module;
        private Mock<IEntityManager<Annotation>> annotationManager;
        private Mock<HttpContext> context;
        private Mock<HttpResponse> response;
        private Mock<HttpRequest> request;
        private Mock<IServiceProvider> serviceProvider;
        private Mock<IEntityManager<Project>> projectManager;
        private Mock<IParticipantManager> participantManager;
        private RouteValueDictionary routeValues;
        private Guid projectId;
        private Participant participant;

        [SetUp]
        public void Setup()
        {
            this.projectManager = new Mock<IEntityManager<Project>>();
            this.participantManager = new Mock<IParticipantManager>();
            this.annotationManager = new Mock<IEntityManager<Annotation>>();
            this.annotationManager.As<IAnnotationManager>();
            this.annotationManager.As<IContainedEntityManager<Annotation>>();

            ModuleTestHelper.Setup<AnnotationModule, AnnotationDto>(new AnnotationDtoValidator(), out this.context,
                out this.response, out this.request, out this.serviceProvider);

            this.projectId = Guid.NewGuid();

            this.routeValues = new RouteValueDictionary
            {
                ["projectId"] = this.projectId.ToString()
            };

            this.request.Setup(x => x.RouteValues).Returns(this.routeValues);

            this.serviceProvider.Setup(x => x.GetService(typeof(IEntityManager<Project>))).Returns(this.projectManager.Object);
            this.serviceProvider.Setup(x => x.GetService(typeof(IParticipantManager))).Returns(this.participantManager.Object);

            this.context.Setup(x => x.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()
            {
                new(ClaimTypes.Name, "user")
            })));

            this.module = new AnnotationModule();

            this.participant = new Participant(Guid.NewGuid())
            {
                Role = new Role(Guid.NewGuid()),
                User = new UserEntity(Guid.NewGuid())
            };
        }

        [Test]
        public async Task VerifyGetEntities()
        {
            var annotations = new List<Annotation>
            {
                new Comment(Guid.NewGuid())
                {
                    Author = this.participant
                },
                new Feedback(Guid.NewGuid())
                {
                    Author = this.participant
                },
                new Note(Guid.NewGuid())
                {
                    Author = this.participant
                }
            };

            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync((Participant)null);

            await this.module.GetEntities(this.annotationManager.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 401, Times.Once);

            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync(this.participant);

            this.annotationManager.As<IAnnotationManager>().Setup(x => x.GetContainedEntities(this.projectId, 0))
                .ReturnsAsync(annotations);

            await this.module.GetEntities(this.annotationManager.Object, this.context.Object);
            this.annotationManager.As<IAnnotationManager>().Verify(x => x.GetContainedEntities(this.projectId, 0), Times.Once);
        }

        [Test]
        public async Task VerifyGetEntity()
        {
            var project = new Project(Guid.NewGuid());

            var annotation = new Comment(Guid.NewGuid())
            {
                Author = this.participant
            };

            this.participantManager.Setup(x => x.GetParticipantForProject(It.IsAny<Guid>(), "user")).ReturnsAsync((Participant)null);

            await this.module.GetEntities(this.annotationManager.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 401, Times.Once);

            this.participantManager.Setup(x => x.GetParticipantForProject(It.IsAny<Guid>(), "user")).ReturnsAsync(this.participant);

            this.annotationManager.Setup(x => x.FindEntity(annotation.Id)).ReturnsAsync((Annotation)null);
            await this.module.GetEntity(this.annotationManager.Object, annotation.Id, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 404, Times.Once);

            this.annotationManager.Setup(x => x.FindEntity(annotation.Id)).ReturnsAsync(annotation);
            await this.module.GetEntity(this.annotationManager.Object, annotation.Id, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400, Times.Once);

            project.Annotations.Add(annotation);
            await this.module.GetEntity(this.annotationManager.Object, annotation.Id, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400, Times.Exactly(2));

            this.routeValues["projectId"] = project.Id.ToString();
            await this.module.GetEntity(this.annotationManager.Object, annotation.Id, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400, Times.Exactly(2));
        }

        [Test]
        public async Task VerifyCreateEntity()
        {
            var project = new Project(Guid.NewGuid());
            var dto = new CommentDto();

            this.serviceProvider.Setup(x => x.GetService(typeof(IParticipantManager))).Returns(null);

            await this.module.CreateEntity(this.annotationManager.Object, dto, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 500, Times.Once);

            this.routeValues["projectId"] = project.Id.ToString();
            this.serviceProvider.Setup(x => x.GetService(typeof(IParticipantManager))).Returns(this.participantManager.Object);

            this.participantManager.Setup(x => x.GetParticipantForProject(project.Id, "user")).ReturnsAsync((Participant)null);

            await this.module.CreateEntity(this.annotationManager.Object, dto, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 401, Times.Once);

            this.participantManager.Setup(x => x.GetParticipantForProject(project.Id, "user")).ReturnsAsync(this.participant);
            await this.module.CreateEntity(this.annotationManager.Object, dto, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 422, Times.Once);

            dto.Author = this.participant.Id;
            dto.Content = "Comment";
            
            dto.AnnotatableItems = new List<Guid>()
            {
                Guid.NewGuid()
            };

            await this.module.CreateEntity(this.annotationManager.Object, dto, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400, Times.Once);
            this.projectManager.Setup(x => x.FindEntity(project.Id)).ReturnsAsync(project);

            this.annotationManager.Setup(x => x.CreateEntity(It.IsAny<Annotation>()))
                .ReturnsAsync(EntityOperationResult<Annotation>.Success(new Comment(Guid.NewGuid())));

            await this.module.CreateEntity(this.annotationManager.Object, dto, this.context.Object);

            this.response.VerifySet(x => x.StatusCode = 201, Times.Once);
        }

        [Test]
        public async Task VerifyDeleteEntity()
        {
            var annotation = new Comment(Guid.NewGuid())
            {
                Author = this.participant
            };

            var project = new Project(Guid.NewGuid())
            {
                Annotations = { annotation }
            };

            this.routeValues["projectId"] = project.Id.ToString();

            this.participantManager.Setup(x => x.GetParticipantForProject(It.IsAny<Guid>(), "user")).ReturnsAsync((Participant)null);

            await this.module.DeleteEntity(this.annotationManager.Object, annotation.Id, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 401, Times.Once);

            this.participantManager.Setup(x => x.GetParticipantForProject(It.IsAny<Guid>(), "user")).ReturnsAsync(annotation.Author);

            this.annotationManager.As<IContainedEntityManager<Annotation>>().Setup(x => x.FindEntityWithContainer(annotation.Id)).ReturnsAsync(annotation);
            this.annotationManager.Setup(x => x.DeleteEntity(annotation)).ReturnsAsync(EntityOperationResult<Annotation>.Success(null));
            var deleteResult = await this.module.DeleteEntity(this.annotationManager.Object, annotation.Id, this.context.Object);
            Assert.That(deleteResult.IsRequestSuccessful, Is.True);
        }

        [Test]
        public async Task VerifyUpdateEntity()
        {
            var annotation = new Comment(Guid.NewGuid())
            {
                Author = this.participant,
                Content = "Content",
                AnnotatableItems = new List<AnnotatableItem>()
                {
                    new ReviewObjective(Guid.NewGuid())
                    {
                        Author = this.participant
                    }
                }
            };

            var annotationDto = annotation.ToDto() as CommentDto;

            var project = new Project(Guid.NewGuid())
            {
                Annotations = { annotation }
            };

            this.routeValues["entityId"] = annotation.Id.ToString();
            this.routeValues["projectId"] = project.Id.ToString();

            this.participantManager.Setup(x => x.GetParticipantForProject(It.IsAny<Guid>(), "user")).ReturnsAsync((Participant)null);

            await this.module.UpdateEntity(this.annotationManager.Object, annotation.Id, annotationDto, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 401, Times.Once);

            this.participantManager.Setup(x => x.GetParticipantForProject(It.IsAny<Guid>(), "user")).ReturnsAsync(annotation.Author);

            this.annotationManager.As<IContainedEntityManager<Annotation>>().Setup(x => x.FindEntityWithContainer(annotation.Id)).ReturnsAsync(annotation);
            this.annotationManager.Setup(x => x.UpdateEntity(It.IsAny<Annotation>())).ReturnsAsync(EntityOperationResult<Annotation>.Success(annotation));
            await this.module.UpdateEntity(this.annotationManager.Object, annotation.Id, annotationDto, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 200, Times.Once);
        }
    }
}
