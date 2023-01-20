// --------------------------------------------------------------------------------------------------------
// <copyright file="ThingModuleTestFixture.cs" company="RHEA System S.A.">
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
    using System.Text;

    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;

    using CDP4JsonSerializer;

    using Microsoft.AspNetCore.Http;

    using Moq;
    
    using NUnit.Framework;

    using UI_DSM.Client.Extensions;
    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Serializer.Json;
    using UI_DSM.Server.Managers.ArtifactManager;
    using UI_DSM.Server.Managers.ThingManager;
    using UI_DSM.Server.Modules;
    using UI_DSM.Server.Services.SearchService;
    using UI_DSM.Server.Tests.Helpers;
    using UI_DSM.Server.Tests.Managers;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class ThingModuleTestFixture
    {
        private ThingModule module;
        private Mock<HttpContext> context;
        private Mock<HttpResponse> response;
        private Mock<IArtifactManager> artifactManager;
        private Mock<IThingManager> thingManager;
        private Mock<ISearchService> searchService;
        private IJsonService jsonService;

        [SetUp]
        public void Setup()
        {
            this.thingManager = new Mock<IThingManager>();
            this.artifactManager = new Mock<IArtifactManager>();
            this.searchService = new Mock<ISearchService>();
            this.jsonService = new JsonService(new JsonDeserializer(), new JsonSerializer(), new Cdp4JsonSerializer());

            ModuleTestHelper.Setup<ThingModule, EntityDto>(null, out this.context,
                out this.response, out _, out _);

            this.module = new ThingModule();
        }

        [Test]
        public async Task VerifyGetThings()
        {
            var unknownedId = Guid.NewGuid();
            var projectId = Guid.NewGuid();

            this.artifactManager.Setup(x => x.FindEntityWithContainer(unknownedId))
                .ReturnsAsync((Model)null);

            await this.module.GetThings(this.artifactManager.Object, this.thingManager.Object, this.context.Object, projectId, new List<Guid>
            {
                unknownedId
            }.ToGuidArray());

            this.response.VerifySet(x => x.StatusCode = 404, Times.Once);

            var artifact = new InvalidArtifact()
            {
                Id = Guid.NewGuid()
            };

            this.artifactManager.Setup(x => x.FindEntityWithContainer(artifact.Id))
                .ReturnsAsync(artifact);

            await this.module.GetThings(this.artifactManager.Object, this.thingManager.Object, this.context.Object, projectId, new List<Guid>
            {
                artifact.Id
            }.ToGuidArray());

            this.response.VerifySet(x => x.StatusCode = 400, Times.Once);

            var model = new Model(Guid.NewGuid());

            var project = new Project(Guid.NewGuid())
            {
                Artifacts =
                {
                    model
                }
            };

            this.artifactManager.Setup(x => x.FindEntityWithContainer(model.Id))
                .ReturnsAsync(model);

            await this.module.GetThings(this.artifactManager.Object, this.thingManager.Object, this.context.Object, projectId, new List<Guid>
            {
                model.Id
            }.ToGuidArray());

            this.response.VerifySet(x => x.StatusCode = 400, Times.Exactly(2));

            await this.module.GetThings(this.artifactManager.Object, this.thingManager.Object, this.context.Object, project.Id, new List<Guid>
            {
                model.Id
            }.ToGuidArray());

            this.response.VerifySet(x => x.StatusCode = 400, Times.Exactly(2));

            await this.module.GetThings(this.artifactManager.Object, this.thingManager.Object, this.context.Object, project.Id, new List<Guid>
            {
                model.Id
            }.ToGuidArray(), nameof(Model));

            this.response.VerifySet(x => x.StatusCode = 400, Times.Exactly(3));

            await this.module.GetThings(this.artifactManager.Object, this.thingManager.Object, this.context.Object, project.Id, new List<Guid>
            {
                model.Id
            }.ToGuidArray(), nameof(RequirementsSpecification));

            this.response.VerifySet(x => x.StatusCode = 400, Times.Exactly(3));
        }

        [Test]
        public async Task VerifyIndexThings()
        {
            var modelId = Guid.NewGuid();
            var projectId = Guid.NewGuid();

            await this.module.IndexThings(this.artifactManager.Object, this.thingManager.Object, this.searchService.Object,
                this.jsonService, this.context.Object, projectId, new List<Guid>
                {
                    modelId
                }.ToGuidArray());

            this.searchService.Verify(x => x.SearchAfter(It.IsAny<string>()), Times.Never);

            var iterationId = Guid.NewGuid();

            this.artifactManager.Setup(x => x.FindEntityWithContainer(modelId)).ReturnsAsync(new Model(modelId)
            {
                IterationId = iterationId,
                EntityContainer = new Project(projectId)
            });

            var searchResult = $"[{{\"@Type\":\"Iteration\", \"@Id\":\"{iterationId}\"}}]";
            var streamReader = new StringContent(searchResult, Encoding.UTF8);

            this.searchService.Setup(x => x.SearchAfter(It.IsAny<string>())).ReturnsAsync(streamReader.ReadAsStream);

            await this.module.IndexThings(this.artifactManager.Object, this.thingManager.Object, this.searchService.Object,
                this.jsonService, this.context.Object, projectId, new List<Guid>
                {
                    modelId
                }.ToGuidArray());

            Assert.Multiple(() =>
            {
                this.searchService.Verify(x => x.SearchAfter(It.IsAny<string>()), Times.Once);
                this.searchService.Verify(x => x.IndexIteration(It.IsAny<IEnumerable<Thing>>()), Times.Never);
            });

            streamReader = new StringContent("[]", Encoding.UTF8);
            this.searchService.Setup(x => x.SearchAfter(It.IsAny<string>())).ReturnsAsync(streamReader.ReadAsStream);

            await this.module.IndexThings(this.artifactManager.Object, this.thingManager.Object, this.searchService.Object,
                this.jsonService, this.context.Object, projectId, new List<Guid>
                {
                    modelId
                }.ToGuidArray());

            this.searchService.Verify(x => x.IndexIteration(It.IsAny<IEnumerable<Thing>>()), Times.Once);
        }
    }
}
