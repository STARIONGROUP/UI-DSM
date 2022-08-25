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
    using Microsoft.AspNetCore.Http;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Server.Managers;
    using UI_DSM.Server.Modules;
    using UI_DSM.Server.Tests.Helpers;
    using UI_DSM.Server.Types;
    using UI_DSM.Server.Validator;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class ProjectModuleTestFixture
    {
        private ProjectModule module;
        private Mock<IEntityManager<Project>> projectManager;
        private List<Project> projects;
        private Mock<HttpContext> httpContext;
        private Mock<HttpResponse> httpResponse;

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

            this.projectManager = new Mock<IEntityManager<Project>>();
            this.projectManager.Setup(x => x.GetEntities()).ReturnsAsync(this.projects);

             ModuleTestHelper.Setup<ProjectModule, ProjectDto>(new ProjectDtoValidator(), out this.httpContext, out this.httpResponse);
             this.module = new ProjectModule();
        }

        [Test]
        public async Task VerifyGetProjects()
        {
            var request = await this.module.GetEntities(this.projectManager.Object);
            Assert.That(request.Count, Is.EqualTo(this.projects.Count));

            this.projectManager.Setup(x => x.GetEntity(this.projects.First().Id)).ReturnsAsync(this.projects.First());

            var invalidGuid = Guid.NewGuid();
            this.projectManager.Setup(x => x.GetEntity(invalidGuid)).ReturnsAsync((Project)null);
            var entity = await this.module.GetEntity(this.projectManager.Object, invalidGuid, this.httpResponse.Object);
            this.httpResponse.VerifySet(x => x.StatusCode = 404, Times.Once);
            Assert.That(entity, Is.Null);

            entity = await this.module.GetEntity(this.projectManager.Object, this.projects.First().Id, this.httpResponse.Object);
            this.httpResponse.VerifySet(x => x.StatusCode = 404, Times.Once);
            Assert.That(entity, Is.Not.Null);
        }

        [Test]
        public async Task VerifyCreateProject()
        {
            var projectDto = new ProjectDto();

            var response =await this.module.CreateEntity(this.projectManager.Object, projectDto, this.httpContext.Object);
            Assert.That(response.IsRequestSuccessful, Is.False);
            this.httpResponse.VerifySet(x => x.StatusCode = 422, Times.Once);
            projectDto.ProjectName = "Project";
            projectDto.Id = Guid.NewGuid();

            response = await this.module.CreateEntity(this.projectManager.Object, projectDto, this.httpContext.Object);
            Assert.That(response.IsRequestSuccessful, Is.False);
            this.httpResponse.VerifySet(x => x.StatusCode = 422, Times.Exactly(2));

            projectDto.Id = Guid.Empty;

            this.projectManager.Setup(x => x.CreateEntity(It.IsAny<Project>()))
                .ReturnsAsync(EntityOperationResult<Project>.Failed());

            response = await this.module.CreateEntity(this.projectManager.Object, projectDto, this.httpContext.Object);
            Assert.That(response.IsRequestSuccessful, Is.False);
            this.httpResponse.VerifySet(x => x.StatusCode = 500, Times.Once);

            this.projectManager.Setup(x => x.CreateEntity(It.IsAny<Project>()))
                .ReturnsAsync(EntityOperationResult<Project>.Success(new Project(Guid.NewGuid())
                {
                    ProjectName = projectDto.ProjectName
                }));

            response = await this.module.CreateEntity(this.projectManager.Object, projectDto, this.httpContext.Object);
            Assert.That(response.IsRequestSuccessful, Is.True);
            this.httpResponse.VerifySet(x => x.StatusCode = 201, Times.Once);
        }

        [Test]
        public async Task VerifyDeleteProject()
        {
            var guid = Guid.NewGuid();
            this.projectManager.Setup(x => x.GetEntity(guid)).ReturnsAsync((Project)null);

            var response = await this.module.DeleteEntity(this.projectManager.Object, guid, this.httpResponse.Object);
            Assert.That(response.IsRequestSuccessful, Is.False);
            this.httpResponse.VerifySet(x => x.StatusCode = 404, Times.Once);

            var project = new Project(guid)
            {
                ProjectName = "Project"
            };

            this.projectManager.Setup(x => x.GetEntity(guid)).ReturnsAsync(project);
            this.projectManager.Setup(x => x.DeleteEntity(project)).ReturnsAsync(EntityOperationResult<Project>.Failed());
            response = await this.module.DeleteEntity(this.projectManager.Object, guid, this.httpResponse.Object);
            Assert.That(response.IsRequestSuccessful, Is.False);
            this.httpResponse.VerifySet(x => x.StatusCode = 500, Times.Once);

            this.projectManager.Setup(x => x.DeleteEntity(project)).ReturnsAsync(EntityOperationResult<Project>.Success(project));
            response = await this.module.DeleteEntity(this.projectManager.Object, guid, this.httpResponse.Object);
            Assert.That(response.IsRequestSuccessful, Is.True);
        }

        [Test]
        public async Task VerifyUpdateProject()
        {
            var project = new Project(Guid.NewGuid());

            this.projectManager.Setup(x => x.GetEntity(project.Id)).ReturnsAsync((Project)null);
            var response = await this.module.UpdateEntity(this.projectManager.Object, project.Id, (ProjectDto)project.ToDto(), this.httpContext.Object);
            Assert.That(response.IsRequestSuccessful, Is.False);
            this.httpResponse.VerifySet(x => x.StatusCode = 404, Times.Once);

            this.projectManager.Setup(x => x.GetEntity(project.Id)).ReturnsAsync(project);
            response = await this.module.UpdateEntity(this.projectManager.Object, project.Id, (ProjectDto)project.ToDto(), this.httpContext.Object);
            Assert.That(response.IsRequestSuccessful, Is.False);
            this.httpResponse.VerifySet(x => x.StatusCode = 422, Times.Once);

            project.ProjectName = "Project";
            this.projectManager.Setup(x => x.UpdateEntity(It.IsAny<Project>())).ReturnsAsync(EntityOperationResult<Project>.Failed());

            response = await this.module.UpdateEntity(this.projectManager.Object, project.Id, (ProjectDto)project.ToDto(), this.httpContext.Object);
            Assert.That(response.IsRequestSuccessful, Is.False);
            this.httpResponse.VerifySet(x => x.StatusCode = 500, Times.Once);

            this.projectManager.Setup(x => x.UpdateEntity(It.IsAny<Project>())).ReturnsAsync(EntityOperationResult<Project>.Success(project));
            response = await this.module.UpdateEntity(this.projectManager.Object, project.Id, (ProjectDto)project.ToDto(), this.httpContext.Object);
            Assert.That(response.IsRequestSuccessful, Is.True);
        }
    }
}
