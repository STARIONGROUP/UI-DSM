// --------------------------------------------------------------------------------------------------------
// <copyright file="ParticipantModuleTestFixture.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft
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
    using UI_DSM.Server.Modules;
    using UI_DSM.Server.Services.SearchService;
    using UI_DSM.Server.Tests.Helpers;
    using UI_DSM.Server.Types;
    using UI_DSM.Server.Validator;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class ParticipantModuleTestFixture
    {
        private ParticipantModule module;
        private Mock<IEntityManager<Participant>> participantManager;
        private Mock<HttpContext> context;
        private Mock<HttpResponse> response;
        private Mock<HttpRequest> request;
        private Mock<IServiceProvider> serviceProvider;
        private Mock<IEntityManager<Project>> projectManager;
        private Mock<ISearchService> searchService;
        private RouteValueDictionary routeValues;

        [SetUp]
        public void Setup()
        {
            this.projectManager = new Mock<IEntityManager<Project>>();
            this.participantManager = new Mock<IEntityManager<Participant>>();
            this.participantManager.As<IParticipantManager>();
            this.participantManager.As<IContainedEntityManager<Participant>>();
            this.searchService = new Mock<ISearchService>();

            ModuleTestHelper.Setup<ParticipantModule, ParticipantDto>(new ParticipantDtoValidator(), out this.context, 
                out this.response, out this.request, out this.serviceProvider);

            this.routeValues = new RouteValueDictionary();
            this.request.Setup(x => x.RouteValues).Returns(this.routeValues);

            this.serviceProvider.Setup(x => x.GetService(typeof(IEntityManager<Project>))).Returns(this.projectManager.Object);
            
            this.context.Setup(x => x.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()
            {
                new (ClaimTypes.Name, "user")
            })));

            this.module = new ParticipantModule();
            Program.RegisterEntities();
        }

        [Test]
        public async Task VerifyGetEntities()
        {
            var participants = new List<Participant>()
            {
                new(Guid.NewGuid())
                {
                    Role = new Role(Guid.NewGuid()),
                    User = new UserEntity(Guid.NewGuid())
                },
                new(Guid.NewGuid())
                {
                    Role = new Role(Guid.NewGuid()),
                    User = new UserEntity(Guid.NewGuid())
                }
            };

            var projectId = Guid.NewGuid();
            
            this.participantManager.As<IParticipantManager>().Setup(x => x.GetContainedEntities(projectId, 0))
                .ReturnsAsync(participants);

            this.routeValues["projectId"] = projectId.ToString();

            await this.module.GetEntities(this.participantManager.Object, this.context.Object);
            this.participantManager.As<IParticipantManager>().Verify(x => x.GetContainedEntities(projectId, 0), Times.Once);
        }

        [Test]
        public async Task VerifyGetEntity()
        {
            var project = new Project(Guid.NewGuid());

            var participant = new Participant(Guid.NewGuid())
            {
                User = new UserEntity(Guid.NewGuid()),
                Role = new Role(Guid.NewGuid())
            };

            this.routeValues["projectId"] = Guid.NewGuid().ToString();

            this.participantManager.As<IContainedEntityManager<Participant>>()
                .Setup(x => x.FindEntityWithContainer(participant.Id)).ReturnsAsync((Participant)null);

            await this.module.GetEntity(this.participantManager.Object, participant.Id, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 404, Times.Once);

            this.participantManager.As<IContainedEntityManager<Participant>>()
                .Setup(x => x.FindEntityWithContainer(participant.Id)).ReturnsAsync(participant);

            await this.module.GetEntity(this.participantManager.Object, participant.Id, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400, Times.Once);

            project.Participants.Add(participant);
            await this.module.GetEntity(this.participantManager.Object, participant.Id, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400, Times.Exactly(2));

            this.routeValues["projectId"] = project.Id.ToString();

            await this.module.GetEntity(this.participantManager.Object, participant.Id, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400, Times.Exactly(2));
        }

        [Test]
        public async Task VerifyCreateEntity()
        {
            var participantDto = new ParticipantDto()
            {
                Role = Guid.NewGuid()
            };

            var project = new Project(Guid.NewGuid());
            this.routeValues["projectId"] = project.Id.ToString();

            await this.module.CreateEntity(this.participantManager.Object, participantDto, this.searchService.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 403, Times.Once);

            this.participantManager.As<IParticipantManager>().Setup(x => x.GetParticipantForProject(project.Id, It.IsAny<string>()))
                .ReturnsAsync(new Participant(Guid.NewGuid())
                {
                    Role = new Role(Guid.NewGuid())
                    {
                        AccessRights = { AccessRight.ProjectManagement }
                    }
                });

            await this.module.CreateEntity(this.participantManager.Object, participantDto, this.searchService.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 422, Times.Once);

            participantDto.User = Guid.NewGuid();

            this.projectManager.Setup(x => x.FindEntity(project.Id)).ReturnsAsync((Project)null);
            await this.module.CreateEntity(this.participantManager.Object, participantDto, this.searchService.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode=400, Times.Once);

            this.projectManager.Setup(x => x.GetEntity(project.Id,0)).ReturnsAsync(project.GetAssociatedEntities());

            this.participantManager.Setup(x => x.CreateEntity(It.IsAny<Participant>()))
                .ReturnsAsync(EntityOperationResult<Participant>.Success(new Participant(Guid.NewGuid())
                {
                    Role = new Role(Guid.NewGuid()),
                    User = new UserEntity(Guid.NewGuid())
                }));

            await this.module.CreateEntity(this.participantManager.Object, participantDto, this.searchService.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 201, Times.Once);
        }

        [Test]
        public async Task VerifyUpdateEntity()
        {
            var participant = new Participant(Guid.NewGuid())
            {
                User = new UserEntity(Guid.NewGuid()),
                Role = new Role(Guid.NewGuid())
            };

            var participantDto = new ParticipantDto();

            var project = new Project(Guid.NewGuid());
            this.routeValues["entityId"] = participant.Id.ToString();
            this.routeValues["projectId"] = Guid.NewGuid().ToString();

            this.participantManager.As<IContainedEntityManager<Participant>>().Setup(x => x.FindEntityWithContainer(participant.Id)).ReturnsAsync((Participant)null);
            await this.module.UpdateEntity(this.participantManager.Object, participant.Id, participantDto, this.searchService.Object, this.context.Object);

            this.response.VerifySet(x => x.StatusCode = 403, Times.Once);

            this.participantManager.As<IParticipantManager>().Setup(x => x.GetParticipantForProject(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(new Participant(Guid.NewGuid())
                {
                    Role = new Role(Guid.NewGuid())
                    {
                         AccessRights = { AccessRight.ProjectManagement }
                    }
                });

            await this.module.UpdateEntity(this.participantManager.Object, participant.Id, participantDto, this.searchService.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode= 404, Times.Once);

            project.Participants.Add(participant);
            this.participantManager.As<IContainedEntityManager<Participant>>().Setup(x => x.FindEntityWithContainer(participant.Id)).ReturnsAsync(participant);
            await this.module.UpdateEntity(this.participantManager.Object, participant.Id, participantDto, this.searchService.Object, this.context.Object);

            this.response.VerifySet(x => x.StatusCode = 400, Times.Once);

            this.routeValues["projectId"] = project.Id.ToString();
            await this.module.UpdateEntity(this.participantManager.Object, participant.Id, participantDto, this.searchService.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 422, Times.Once);

            participantDto = participant.ToDto() as ParticipantDto;
            this.participantManager.Setup(x => x.UpdateEntity(It.IsAny<Participant>())).ReturnsAsync(EntityOperationResult<Participant>.Success(participant));
            await this.module.UpdateEntity(this.participantManager.Object, participant.Id, participantDto, this.searchService.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 200, Times.Once);
        }

        [Test]
        public async Task VerifyDeleteEntity()
        {
            var participant = new Participant(Guid.NewGuid());

            var project = new Project(Guid.NewGuid())
            {
                Participants = { participant }
            };

            this.routeValues["projectId"] = project.Id.ToString();

            this.participantManager.As<IParticipantManager>().Setup(x => x.GetParticipantForProject(project.Id, It.IsAny<string>()))
                .ReturnsAsync((Participant)null);

            await this.module.DeleteEntity(this.participantManager.Object, participant.Id, this.searchService.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 403, Times.Once);

            this.participantManager.As<IParticipantManager>().Setup(x => x.GetParticipantForProject(project.Id, It.IsAny<string>()))
                .ReturnsAsync(participant);

            await this.module.DeleteEntity(this.participantManager.Object, participant.Id, this.searchService.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode= 403, Times.Exactly(2));

            participant.Role = new Role(Guid.NewGuid())
            {
                AccessRights = { AccessRight.ProjectManagement }
            };

            this.participantManager.As<IParticipantManager>().Setup(x => x.GetParticipantForProject(project.Id, It.IsAny<string>()))
                .ReturnsAsync(participant);

            this.participantManager.Setup(x => x.DeleteEntity(participant)).ReturnsAsync(EntityOperationResult<Participant>
                .Failed());

            this.participantManager.As<IContainedEntityManager<Participant>>().Setup(x => x.FindEntityWithContainer(It.IsAny<Guid>()))
                .ReturnsAsync(participant);

            await this.module.DeleteEntity(this.participantManager.Object, participant.Id, this.searchService.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 500, Times.Once);

            this.participantManager.Setup(x => x.DeleteEntity(participant)).ReturnsAsync(EntityOperationResult<Participant>.Success(participant));
            await this.module.DeleteEntity(this.participantManager.Object, participant.Id, this.searchService.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = It.IsAny<int>(), Times.Exactly(3));
        }

        [Test]
        public async Task VerifyGetAvailableUsers()
        {
            var users = new List<UserEntity>()
            {
                new (Guid.NewGuid())
            };

            var project = new Project(Guid.NewGuid());

            this.projectManager.Setup(x => x.FindEntity(project.Id)).ReturnsAsync((Project)null);
            
            await this.module.GetAvailableUsers(this.participantManager.As<IParticipantManager>().Object, this.projectManager.Object,
                project.Id, this.context.Object);

            this.response.VerifySet(x => x.StatusCode = 403, Times.Once);

            this.participantManager.As<IParticipantManager>().Setup(x => x.GetParticipantForProject(project.Id, It.IsAny<string>()))
                .ReturnsAsync(new Participant(Guid.NewGuid())
                {
                    Role = new Role(Guid.NewGuid())
                    {
                        AccessRights = { AccessRight.ProjectManagement }
                    }
                });

            await this.module.GetAvailableUsers(this.participantManager.As<IParticipantManager>().Object, this.projectManager.Object, 
                project.Id, this.context.Object);

            this.response.VerifySet(x => x.StatusCode = 404, Times.Once);

            this.projectManager.Setup(x => x.FindEntity(project.Id)).ReturnsAsync(project);

            this.participantManager.As<IParticipantManager>().Setup(x => x.GetAvailableUsersForParticipantCreation(project.Id))
                .ReturnsAsync(users);

            await this.module.GetAvailableUsers(this.participantManager.As<IParticipantManager>().Object, this.projectManager.Object,
                project.Id, this.context.Object);

            this.response.VerifySet(x => x.StatusCode = 404, Times.Once);
        }

        [Test]
        public async Task VerifyGetLoggedParticipant()
        {
            var project = new Project(Guid.NewGuid());

            this.projectManager.Setup(x => x.FindEntity(project.Id)).ReturnsAsync((Project)null);

            await this.module.GetLoggedParticipant(this.participantManager.As<IParticipantManager>().Object, this.projectManager.Object,
                project.Id, this.context.Object);

            this.response.VerifySet(x => x.StatusCode = 404, Times.Once);

            this.projectManager.Setup(x => x.FindEntity(project.Id)).ReturnsAsync(project);

            this.participantManager.As<IParticipantManager>().Setup(x => x.GetParticipantForProject(project.Id, "user"))
                .ReturnsAsync((Participant)null);

            await this.module.GetLoggedParticipant(this.participantManager.As<IParticipantManager>().Object, this.projectManager.Object,
                project.Id, this.context.Object);

            this.response.VerifySet(x => x.StatusCode = 401, Times.Once);

            this.participantManager.As<IParticipantManager>().Setup(x => x.GetParticipantForProject(project.Id, "user"))
                .ReturnsAsync(new Participant());
         
            this.response.VerifySet(x => x.StatusCode = It.IsAny<int>(), Times.Exactly(2));
        }
    }
}
