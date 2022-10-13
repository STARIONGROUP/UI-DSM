// --------------------------------------------------------------------------------------------------------
// <copyright file="ArtifactModuleTestFixture.cs" company="RHEA System S.A.">
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
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;
  
    using Moq;
    
    using NUnit.Framework;

    using UI_DSM.Server.Managers;
    using UI_DSM.Server.Managers.ArtifactManager;
    using UI_DSM.Server.Modules;
    using UI_DSM.Server.Services.FileService;
    using UI_DSM.Server.Tests.Helpers;
    using UI_DSM.Server.Validator;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class ArtifactModuleTestFixture
    {
        private ArtifactModule module;
        private Mock<IEntityManager<Artifact>> artifactManager;
        private Mock<HttpContext> context;
        private Mock<HttpResponse> response;
        private Mock<HttpRequest> request;
        private Mock<IServiceProvider> serviceProvider;
        private Mock<IEntityManager<Project>> projectManager;
        private RouteValueDictionary routeValues;
        private Guid projectId;
        private Mock<IFileService> fileService;

        [SetUp]
        public void Setup()
        {
            this.projectManager = new Mock<IEntityManager<Project>>();
            this.artifactManager = new Mock<IEntityManager<Artifact>>();
            this.artifactManager.As<IArtifactManager>();
            this.artifactManager.As<IContainedEntityManager<Artifact>>();

            ModuleTestHelper.Setup<ArtifactModule, ArtifactDto>(new ArtifactDtoValidator(), out this.context,
                out this.response, out this.request, out this.serviceProvider);

            this.projectId = Guid.NewGuid();

            this.routeValues = new RouteValueDictionary
            {
                ["projectId"] = this.projectId.ToString()
            };

            this.request.Setup(x => x.RouteValues).Returns(this.routeValues);
            this.serviceProvider.Setup(x => x.GetService(typeof(IEntityManager<Project>))).Returns(this.projectManager.Object);
            this.fileService = new Mock<IFileService>();
            this.module = new ArtifactModule(this.fileService.Object);
        }

        [Test]
        public async Task VerifyGetEntities()
        {
            var artifacts = new List<Artifact>
            {
                new Model(Guid.NewGuid())
            };

            this.artifactManager.As<IArtifactManager>().Setup(x => x.GetContainedEntities(this.projectId, 0))
                .ReturnsAsync(artifacts);

            await this.module.GetEntities(this.artifactManager.Object, this.context.Object);
            this.artifactManager.As<IArtifactManager>().Verify(x => x.GetContainedEntities(this.projectId, 0), Times.Once);
        }

        [Test]
        public async Task VerifyGetEntity()
        {
            var project = new Project(Guid.NewGuid());

            var artifact = new Model(Guid.NewGuid());

            this.artifactManager.As<IContainedEntityManager<Artifact>>()
                .Setup(x => x.FindEntityWithContainer(artifact.Id)).ReturnsAsync((Artifact)null);

            await this.module.GetEntity(this.artifactManager.Object, artifact.Id, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 404, Times.Once);

            this.artifactManager.As<IContainedEntityManager<Artifact>>()
                .Setup(x => x.FindEntityWithContainer(artifact.Id)).ReturnsAsync(artifact);

            await this.module.GetEntity(this.artifactManager.Object, artifact.Id, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400, Times.Once);

            project.Artifacts.Add(artifact);
            await this.module.GetEntity(this.artifactManager.Object, artifact.Id, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400, Times.Exactly(2));

            this.routeValues["projectId"] = project.Id.ToString();
            await this.module.GetEntity(this.artifactManager.Object, artifact.Id, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400, Times.Exactly(2));
        }

        [Test]
        public async Task VerifyCreateEntity()
        {
            var modelDto = new ModelDto()
            {
                FileName = $"{Guid.NewGuid()}.zip",
                ModelName = "Model"
            };
            
            this.fileService.Setup(x => x.TempFileExists(modelDto.FileName)).Returns(false);
            await this.module.CreateEntity(this.artifactManager.Object, modelDto, this.context.Object);
            this.response.VerifySet(x => x.StatusCode=404, Times.Once);

            this.fileService.Setup(x => x.TempFileExists(modelDto.FileName)).Returns(true);
            this.fileService.Setup(x => x.Exists(this.projectId.ToString(), modelDto.FileName)).Returns(true);
            await this.module.CreateEntity(this.artifactManager.Object, modelDto, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400, Times.Once);

            this.fileService.Setup(x => x.Exists(this.projectId.ToString(), modelDto.FileName)).Returns(false);
            this.fileService.Setup(x => x.IsAnnexC3File(modelDto.FileName)).ReturnsAsync(false);
            await this.module.CreateEntity(this.artifactManager.Object, modelDto, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400, Times.Exactly(2));

            this.fileService.Setup(x => x.IsAnnexC3File(modelDto.FileName)).ReturnsAsync(true);
            await this.module.CreateEntity(this.artifactManager.Object, modelDto, this.context.Object);
            this.fileService.Verify(x => x.MoveFile(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }

        [Test]
        public async Task VerifyUpdateAndDeleteEntity()
        {
            var dto = new ModelDto(Guid.NewGuid());

            await this.module.UpdateEntity(this.artifactManager.Object, dto.Id, dto, this.context.Object);
            await this.module.DeleteEntity(this.artifactManager.Object, dto.Id, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 403, Times.Exactly(2));
        }
    }
}
