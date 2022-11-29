// --------------------------------------------------------------------------------------------------------
// <copyright file="ProjectModuleTestFixture.cs" company="RHEA System S.A.">
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

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Client.Services.Administration.ParticipantService;
    using UI_DSM.Server.Managers;
    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Server.Managers.ProjectManager;
    using UI_DSM.Server.Modules;
    using UI_DSM.Server.Tests.Helpers;
    using UI_DSM.Server.Types;
    using UI_DSM.Server.Validator;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class ProjectModuleTestFixture
    {
        private ProjectModule module;
        private Mock<IEntityManager<Project>> projectManager;
        private List<Project> projects;
        private Mock<HttpContext> httpContext;
        private Mock<HttpResponse> httpResponse;
        private Mock<IServiceProvider> provider;
        private Mock<IParticipantManager> participantManager;

        [SetUp]
        public void Setup()
        {
            this.projects = new List<Project>
            {
                new(Guid.NewGuid())
                {
                    ProjectName = "Project 1"
                }
            };

            this.participantManager = new Mock<IParticipantManager>();
            this.projectManager = new Mock<IEntityManager<Project>>();
            this.projectManager.As<IProjectManager>();
            this.projectManager.Setup(x => x.GetEntities(0)).ReturnsAsync(this.projects);

             ModuleTestHelper.Setup<ProjectModule, ProjectDto>(new ProjectDtoValidator(), out this.httpContext, out this.httpResponse, out _, out this.provider);
             this.module = new ProjectModule();

             this.httpContext.Setup(x => x.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()
             {
                 new (ClaimTypes.Name, "user")
             })));

             Program.RegisterEntities();
        }

        [Test]
        public async Task VerifyGetProjects()
        {
            await this.module.GetEntities(this.projectManager.Object, this.httpContext.Object);
            this.httpResponse.VerifySet(x => x.StatusCode = 500, Times.Once);

            this.provider.Setup(x => x.GetService(typeof(IParticipantManager))).Returns(this.participantManager.Object);

            this.participantManager.Setup(x => x.GetParticipants(It.IsAny<string>()))
                .ReturnsAsync(new List<Participant>());

            await this.module.GetEntities(this.projectManager.Object, this.httpContext.Object);
            this.httpResponse.VerifySet(x => x.StatusCode = 403, Times.Once);

            this.participantManager.Setup(x => x.GetParticipants(It.IsAny<string>()))
                .ReturnsAsync(new List<Participant>(){new (Guid.NewGuid()){Role = new Role(Guid.NewGuid()){AccessRights = { AccessRight.ProjectManagement }}}});

            this.projectManager.As<IProjectManager>().Setup(x => x.GetProjectsForManagement(It.IsAny<string>())).ReturnsAsync(new List<Project>());
            await this.module.GetEntities(this.projectManager.Object, this.httpContext.Object);
            this.projectManager.As<IProjectManager>().Verify(x => x.GetProjectsForManagement(It.IsAny<string>()), Times.Once);
            
            this.httpContext.Setup(x => x.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()
            {
                new (ClaimTypes.Name, "user"),
                new (ClaimTypes.Role, "Administrator")
            })));

            await this.module.GetEntities(this.projectManager.Object, this.httpContext.Object);
            this.projectManager.Verify(x => x.GetEntities(0), Times.Once);

            this.projectManager.Setup(x => x.GetEntity(this.projects.First().Id,0)).ReturnsAsync(this.projects);

            var invalidGuid = Guid.NewGuid();
            this.projectManager.Setup(x => x.GetEntity(invalidGuid,0)).ReturnsAsync(Enumerable.Empty<Project>());
            await this.module.GetEntity(this.projectManager.Object, invalidGuid, this.httpContext.Object);
            this.httpResponse.VerifySet(x => x.StatusCode = 404, Times.Once);

            await this.module.GetEntity(this.projectManager.Object, this.projects.First().Id, this.httpContext.Object);
            this.httpResponse.VerifySet(x => x.StatusCode = 404, Times.Once);
        }

        [Test]
        public async Task VerifyCreateProject()
        {
            var projectDto = new ProjectDto();

            await this.module.CreateEntity(this.projectManager.Object, projectDto, this.httpContext.Object);
            this.httpResponse.VerifySet(x => x.StatusCode = 422, Times.Once);
            projectDto.ProjectName = "Project";
            projectDto.Id = Guid.NewGuid();

            await this.module.CreateEntity(this.projectManager.Object, projectDto, this.httpContext.Object);
            this.httpResponse.VerifySet(x => x.StatusCode = 422, Times.Exactly(2));

            projectDto.Id = Guid.Empty;

            this.projectManager.Setup(x => x.CreateEntity(It.IsAny<Project>()))
                .ReturnsAsync(EntityOperationResult<Project>.Failed());

            await this.module.CreateEntity(this.projectManager.Object, projectDto, this.httpContext.Object);
            this.httpResponse.VerifySet(x => x.StatusCode = 500, Times.Once);

            this.projectManager.Setup(x => x.CreateEntity(It.IsAny<Project>()))
                .ReturnsAsync(EntityOperationResult<Project>.Success(new Project(Guid.NewGuid())
                {
                    ProjectName = projectDto.ProjectName
                }));

            await this.module.CreateEntity(this.projectManager.Object, projectDto, this.httpContext.Object);
            this.httpResponse.VerifySet(x => x.StatusCode = 201, Times.Once);
        }

        [Test]
        public async Task VerifyDeleteProject()
        {
            var guid = Guid.NewGuid();
            this.projectManager.Setup(x => x.FindEntity(guid)).ReturnsAsync((Project)null);

            var response = await this.module.DeleteEntity(this.projectManager.Object, guid, this.httpContext.Object);
            Assert.That(response.IsRequestSuccessful, Is.False);
            this.httpResponse.VerifySet(x => x.StatusCode = 404, Times.Once);

            var project = new Project(guid)
            {
                ProjectName = "Project"
            };

            this.projectManager.Setup(x => x.FindEntity(guid)).ReturnsAsync(project);
            this.projectManager.Setup(x => x.DeleteEntity(project)).ReturnsAsync(EntityOperationResult<Project>.Failed());
            response = await this.module.DeleteEntity(this.projectManager.Object, guid, this.httpContext.Object);
            Assert.That(response.IsRequestSuccessful, Is.False);
            this.httpResponse.VerifySet(x => x.StatusCode = 500, Times.Once);

            this.projectManager.Setup(x => x.DeleteEntity(project)).ReturnsAsync(EntityOperationResult<Project>.Success(project));
            response = await this.module.DeleteEntity(this.projectManager.Object, guid, this.httpContext.Object);
            Assert.That(response.IsRequestSuccessful, Is.True);
        }

        [Test]
        public async Task VerifyUpdateProject()
        {
            var project = new Project(Guid.NewGuid());

            this.projectManager.Setup(x => x.FindEntity(project.Id)).ReturnsAsync((Project)null);
            await this.module.UpdateEntity(this.projectManager.Object, project.Id, (ProjectDto)project.ToDto(), this.httpContext.Object);
            this.httpResponse.VerifySet(x => x.StatusCode = 404, Times.Once);

            this.projectManager.Setup(x => x.FindEntity(project.Id)).ReturnsAsync(project);
            await this.module.UpdateEntity(this.projectManager.Object, project.Id, (ProjectDto)project.ToDto(), this.httpContext.Object);
            this.httpResponse.VerifySet(x => x.StatusCode = 422, Times.Once);

            project.ProjectName = "Project";
            this.projectManager.Setup(x => x.UpdateEntity(It.IsAny<Project>())).ReturnsAsync(EntityOperationResult<Project>.Failed());

            await this.module.UpdateEntity(this.projectManager.Object, project.Id, (ProjectDto)project.ToDto(), this.httpContext.Object);
            this.httpResponse.VerifySet(x => x.StatusCode = 500, Times.Once);

            this.projectManager.Setup(x => x.UpdateEntity(It.IsAny<Project>())).ReturnsAsync(EntityOperationResult<Project>.Success(project));
            await this.module.UpdateEntity(this.projectManager.Object, project.Id, (ProjectDto)project.ToDto(), this.httpContext.Object);
            this.httpResponse.VerifySet(x => x.StatusCode = 200, Times.Once);
        }

        [Test]
        public async Task VerifyGetProjectsForUser()
        {
            var username = "user";

            var claimPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()
            {
                new (ClaimTypes.Name, username)
            }));

            this.httpContext.Setup(x => x.User).Returns(claimPrincipal);

            this.projectManager.As<IProjectManager>().Setup(x => x.GetAvailableProjectsForUser(username))
                .ReturnsAsync(this.projects);

            await this.module.GetProjectsForUser(this.projectManager.As<IProjectManager>().Object, this.httpContext.Object);
            
            this.projectManager.As<IProjectManager>().Verify(x => x.GetAvailableProjectsForUser(username), Times.Once);
        }

        [Test]
        public async Task VerifyGetOpenTasksAndComments()
        {
            this.projectManager.As<IProjectManager>().Setup(x => x.GetAvailableProjectsForUser(It.IsAny<string>()))
                .ReturnsAsync(this.projects);

            var guids = this.projects.Select(x => x.Id).ToList();

            var result = new Dictionary<Guid, ComputedProjectProperties>
            {
                [guids[0]] = new()
                {
                    CommentCount = 15,
                    TaskCount = 12
                }
            };

            this.projectManager.As<IProjectManager>().Setup(x => x.GetOpenTasksAndComments(guids, "user"))
                .ReturnsAsync(result);

            await this.module.GetOpenTasksAndComments(this.projectManager.As<IProjectManager>().Object, this.httpContext.Object);
            this.projectManager.As<IProjectManager>().Verify(x => x.GetOpenTasksAndComments(guids, "user"), Times.Once);
        }
    }
}
