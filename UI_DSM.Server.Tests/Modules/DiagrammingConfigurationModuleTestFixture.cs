// --------------------------------------------------------------------------------------------------------
// <copyright file="DiagrammingConfigurationModuleTestFixture.cs" company="RHEA System S.A.">
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

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Server.Modules;
    using UI_DSM.Server.Services.FileService;
    using UI_DSM.Server.Tests.Helpers;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class DiagrammingConfigurationModuleTestFixture
    {
        private DiagrammingConfigurationModule module;
        private Mock<IFileService> fileService;
        private Mock<HttpContext> context;
        private Mock<HttpResponse> response;
        private Mock<IParticipantManager> participantManager;
        private Guid projectId;
        private Guid reviewTaskId;
        private Participant participant;

        [SetUp]
        public void Setup()
        {
            this.fileService = new Mock<IFileService>();
        
            this.module = new DiagrammingConfigurationModule(this.fileService.Object);

            this.participantManager = new Mock<IParticipantManager>();

            ModuleTestHelper.Setup<DiagrammingConfigurationModule, DiagramNodeDto>(null, out this.context,
                out this.response, out _, out _);

            this.projectId = Guid.NewGuid();
            this.reviewTaskId = Guid.NewGuid();

            this.context.Setup(x => x.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()
            {
                new (ClaimTypes.Name, "user")
            })));

            this.participant = new Participant(Guid.NewGuid())
            {
                Role = new Role(Guid.NewGuid()),
                User = new UserEntity(Guid.NewGuid())
            };
        }

        [TearDown]
        public void Teardown()
        {
            var basePath = Path.Combine(TestContext.CurrentContext.TestDirectory, DiagrammingConfigurationModule.DirectoryName);

            if (Directory.Exists(basePath))
            {
                Directory.Delete(basePath, true);
            }
        }

        [Test]
        public async Task VerifySaveLayoutConfiguration()
        {
            this.fileService.Setup(x => x.GetTempFolder()).Returns(TestContext.CurrentContext.TestDirectory);

            var diagramDto = new DiagramDto()
            {
                Nodes = new List<DiagramNodeDto>()
                {
                    new()
                    {
                        ThingId = Guid.NewGuid(),
                        Point = new PointDto()
                        {
                            X = 650,
                            Y = 447
                        }
                    }
                }
            };

            const string configFileName = "config1";
            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync((Participant)null);

            await this.module.SaveLayoutConfiguration(this.participantManager.Object, this.projectId, this.reviewTaskId, configFileName, diagramDto, this.context.Object);
            this.response.VerifySet( x => x.StatusCode = 403, Times.Once);

            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync(this.participant);
            await this.module.SaveLayoutConfiguration(this.participantManager.Object, this.projectId, this.reviewTaskId, configFileName, diagramDto, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 403, Times.Exactly(2));

            this.participant.Role = new Role() { AccessRights = { AccessRight.CreateDiagramConfiguration } };

            await this.module.SaveLayoutConfiguration(this.participantManager.Object, this.projectId, this.reviewTaskId, configFileName, diagramDto, this.context.Object);

            this.fileService.Verify(x => x.MoveFile(It.IsAny<string>(), It.IsAny<string>()), Times.Once());

            var basePath = Path.Combine(TestContext.CurrentContext.TestDirectory, DiagrammingConfigurationModule.DirectoryName);
            this.fileService.Setup(x => x.GetDirectoryPath(DiagrammingConfigurationModule.DirectoryName)).Returns(basePath);
            var path = Path.Combine(basePath, this.reviewTaskId.ToString());
            Directory.CreateDirectory(path);
            var filePath = Path.Combine(path, configFileName);
            await using var _ =File.Create(filePath);
            await this.module.SaveLayoutConfiguration(this.participantManager.Object, this.projectId, this.reviewTaskId, configFileName, diagramDto, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 300, Times.Once);

            for (var fileCount = 0; fileCount < 20; fileCount++)
            {
                filePath = Path.Combine(path, $"{configFileName}{fileCount}");
                await using var tmp = File.Create(filePath);
            }

            await this.module.SaveLayoutConfiguration(this.participantManager.Object, this.projectId, this.reviewTaskId, "newConfig", diagramDto, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 300, Times.Exactly(2));
        }

        [Test]
        public async Task VerifyLoadLayoutConfigurationNames()
        {
            this.fileService.Setup(x => x.GetDirectoryPath(DiagrammingConfigurationModule.DirectoryName)).Returns(TestContext.CurrentContext.TestDirectory);

            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync((Participant)null);

            await this.module.LoadLayoutConfigurationNames(this.participantManager.Object, this.projectId, this.reviewTaskId, this.context.Object);
            this.fileService.Verify(x => x.GetDirectoryPath(It.IsAny<string>()), Times.Never());

            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync(this.participant);
            await this.module.LoadLayoutConfigurationNames(this.participantManager.Object, this.projectId, this.reviewTaskId, this.context.Object);

            this.fileService.Verify(x => x.GetDirectoryPath(It.IsAny<string>()), Times.Once());
        }

        [Test]
        public async Task VerifyLoadLayoutConfiguration()
        {
            const string configName = "config1";
            this.fileService.Setup(x => x.GetDirectoryPath(DiagrammingConfigurationModule.DirectoryName)).Returns(TestContext.CurrentContext.TestDirectory);

            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync((Participant)null);

            await this.module.LoadLayoutConfiguration(this.participantManager.Object, this.projectId, this.reviewTaskId, configName, this.context.Object);
            this.fileService.Verify(x => x.GetDirectoryPath(It.IsAny<string>()), Times.Never());

            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync(this.participant);
            await this.module.LoadLayoutConfiguration(this.participantManager.Object, this.projectId, this.reviewTaskId, configName, this.context.Object);

            this.fileService.Verify(x => x.GetDirectoryPath(It.IsAny<string>()), Times.Once());
        }

        [Test]
        public async Task VerifyDeleteLayoutConfiguration()
        {
            const string configFileName = "config1";
            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync((Participant)null);

            await this.module.DeleteLayoutConfiguration(this.participantManager.Object, this.projectId, this.reviewTaskId, configFileName, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 403, Times.Once);

            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync(this.participant);
            await this.module.DeleteLayoutConfiguration(this.participantManager.Object, this.projectId, this.reviewTaskId, configFileName, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 403, Times.Exactly(2));

            this.participant.Role = new Role() { AccessRights = { AccessRight.CreateDiagramConfiguration } };

            this.fileService.Setup(x => x.Exists(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            await this.module.DeleteLayoutConfiguration(this.participantManager.Object, this.projectId, this.reviewTaskId, configFileName, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 300, Times.Once);

            this.fileService.Setup(x => x.Exists(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            await this.module.DeleteLayoutConfiguration(this.participantManager.Object, this.projectId, this.reviewTaskId, configFileName, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 300, Times.Once);

            this.fileService.Setup(x => x.DeleteFile(It.IsAny<string>())).Throws(new ArgumentException("A reason to throw exception"));
            await this.module.DeleteLayoutConfiguration(this.participantManager.Object, this.projectId, this.reviewTaskId, configFileName, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 500, Times.Once);
        }
    }
}
