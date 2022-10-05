// --------------------------------------------------------------------------------------------------------
// <copyright file="ArtifactManagerTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Tests.Managers
{
    using System.Diagnostics.CodeAnalysis;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Server.Managers.ArtifactManager;
    using UI_DSM.Server.Managers.ModelManager;
    using UI_DSM.Server.Types;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class ArtifactManagerTestFixture
    {
        private ArtifactManager manager;
        private Mock<IModelManager> modelManager;

        [SetUp]
        public void Setup()
        {
            this.modelManager = new Mock<IModelManager>();
            this.manager = new ArtifactManager(this.modelManager.Object);
            Program.RegisterEntities();
        }

        [Test]
        public async Task VerifyGetEntities()
        {
            var models = new List<Model>
            {
                new(Guid.NewGuid())
                {
                    FileName = "AFileName.zip",
                    ModelName = "Envision - Iteration 1"
                },
                new(Guid.NewGuid())
                {
                    FileName = "AFileName2.zip",
                    ModelName = "Envision - Iteration 2"
                }
            };

            var project = new Project(Guid.NewGuid());
            project.Artifacts.Add(models.First());

            var project2 = new Project(Guid.NewGuid());
            project2.Artifacts.Add(models.Last());

            this.modelManager.Setup(x => x.GetEntities(0))
                .ReturnsAsync(models.SelectMany(x => x.GetAssociatedEntities()).DistinctBy(x => x.Id));

            var entities = await this.manager.GetEntities();
            Assert.That(entities.ToList(), Has.Count.EqualTo(2));

            foreach (var model in models)
            {
                this.modelManager.Setup(x => x.FindEntity(model.Id)).ReturnsAsync(model);
                this.modelManager.Setup(x => x.FindEntityWithContainer(model.Id)).ReturnsAsync(model);
                this.modelManager.Setup(x => x.GetEntity(model.Id,0)).ReturnsAsync(model.GetAssociatedEntities());
            }

            entities = await this.manager.GetEntity(Guid.NewGuid());
            Assert.That(entities, Is.Empty);

            var foundEntities = await this.manager.FindEntities(new List<Guid>
            {
                models.First().Id,
                Guid.NewGuid()
            });

            Assert.That(foundEntities.ToList(), Has.Count.EqualTo(1));

            var entity = await this.manager.FindEntityWithContainer(models.First().Id);

            Assert.Multiple(async () =>
            {
                Assert.That(entity, Is.Not.Null);
                Assert.That(await this.manager.EntityIsContainedBy(models.First().Id, Guid.NewGuid()), Is.False);
                Assert.That(await this.manager.EntityIsContainedBy(models.Last().Id, Guid.NewGuid()), Is.False);
                Assert.That(await this.manager.EntityIsContainedBy(models.First().Id, project.Id), Is.True);
            });

            this.modelManager.Setup(x => x.GetContainedEntities(project.Id,0))
                .ReturnsAsync(new List<Entity>
                {
                    models.First()
                });

            Assert.That(await this.manager.GetContainedEntities(project.Id), Is.Not.Empty);
        }

        [Test]
        public async Task VerifyCreateEntity()
        {
            var artifact = new Model();
            this.modelManager.Setup(x => x.CreateEntity(artifact)).ReturnsAsync(EntityOperationResult<Model>.Failed());
            Assert.That((await this.manager.CreateEntity(artifact)).Succeeded, Is.False);

            this.modelManager.Setup(x => x.CreateEntity(artifact)).ReturnsAsync(EntityOperationResult<Model>.Success(artifact));
            Assert.That((await this.manager.CreateEntity(artifact)).Succeeded, Is.True);

            var invalidArtifact = new InvalidArtifact();
            Assert.That((await this.manager.CreateEntity(invalidArtifact)).Succeeded, Is.False);
        }

        [Test]
        public async Task VerifyUpdateEntity()
        {
            var artifact = new Model();
            this.modelManager.Setup(x => x.UpdateEntity(artifact)).ReturnsAsync(EntityOperationResult<Model>.Failed());
            Assert.That((await this.manager.UpdateEntity(artifact)).Succeeded, Is.False);

            this.modelManager.Setup(x => x.UpdateEntity(artifact)).ReturnsAsync(EntityOperationResult<Model>.Success(artifact));
            Assert.That((await this.manager.UpdateEntity(artifact)).Succeeded, Is.True);

            var invalidArtifact = new InvalidArtifact();
            Assert.That((await this.manager.UpdateEntity(invalidArtifact)).Succeeded, Is.False);
        }

        [Test]
        public async Task VerifyDeleteEntity()
        {
            var artifact = new Model();
            this.modelManager.Setup(x => x.DeleteEntity(artifact)).ReturnsAsync(EntityOperationResult<Model>.Failed());
            Assert.That((await this.manager.DeleteEntity(artifact)).Succeeded, Is.False);

            this.modelManager.Setup(x => x.DeleteEntity(artifact)).ReturnsAsync(EntityOperationResult<Model>.Success(artifact));
            Assert.That((await this.manager.DeleteEntity(artifact)).Succeeded, Is.True);

            var invalidArtifact = new InvalidArtifact();
            Assert.That((await this.manager.DeleteEntity(invalidArtifact)).Succeeded, Is.False);
        }

        [Test]
        public async Task VerifyResolveProperties()
        {
            var artifact = new Model();
            var dto = new ModelDto();
            var invalidArtifact = new InvalidArtifact();

            await this.manager.ResolveProperties(artifact, dto);
            await this.manager.ResolveProperties(invalidArtifact, dto);

            this.modelManager.Verify(x => x.ResolveProperties(artifact, dto), Times.Once);
        }
    }

    [ExcludeFromCodeCoverage]
    internal class InvalidArtifact : Artifact
    {
        /// <summary>
        ///     Instantiate a <see cref="EntityDto" /> from a <see cref="Entity" />
        /// </summary>
        /// <returns>A new <see cref="EntityDto" /></returns>
        public override EntityDto ToDto()
        {
            return null;
        }
    }
}
