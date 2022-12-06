// --------------------------------------------------------------------------------------------------------
// <copyright file="ModelManagerTestFixture.cs" company="RHEA System S.A.">
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
    using Microsoft.EntityFrameworkCore;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Server.Context;
    using UI_DSM.Server.Managers.ModelManager;
    using UI_DSM.Server.Services.CometService;
    using UI_DSM.Server.Tests.Helpers;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class ModelManagerTestFixture
    {
        private ModelManager manager;
        private Mock<DatabaseContext> context;
        private Mock<DbSet<Model>> modelDbSet;
        private Mock<DbSet<Project>> projectDbSet;

        [SetUp]
        public void Setup()
        {
            this.context = new Mock<DatabaseContext>();
            this.context.CreateDbSetForContext(out this.modelDbSet, out this.projectDbSet);
            
            this.manager = new ModelManager(this.context.Object);
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

            this.modelDbSet.UpdateDbSetCollection(models);

            var allEntities = await this.manager.GetEntities(1);
            Assert.That(allEntities.ToList(), Has.Count.EqualTo(2));

            var invalidGuid = Guid.NewGuid();
            var foundReview = await this.manager.FindEntity(invalidGuid);
            Assert.That(foundReview, Is.Null);

            var foundEntities = await this.manager.FindEntities(models.Select(x => x.Id));
            Assert.That(foundEntities.ToList(), Has.Count.EqualTo(2));

            var deepEntity = await this.manager.GetEntity(models.First().Id, 1);
            Assert.That(deepEntity.ToList(), Has.Count.EqualTo(1));

            var project = new Project(Guid.NewGuid())
            {
                Artifacts = { models.First() }
            };

            var containedEntities = await this.manager.GetContainedEntities(project.Id);
            Assert.That(containedEntities.ToList(), Has.Count.EqualTo(1));
        }

        [Test]
        public async Task VerifyCreateEntity()
        {
            var model = new Model()
            {
                FileName = "filename.zip",
                ModelName = "Envision - Iteration 1"
            };

            var operationResult = await this.manager.CreateEntity(model);
            Assert.That(operationResult.Succeeded, Is.False);

            var projects = new List<Project>()
            {
                new(Guid.NewGuid())
                {
                    Artifacts = 
                    {
                        model
                    }
                }
            };

            this.projectDbSet.UpdateDbSetCollection(projects);

            await this.manager.CreateEntity(model);
            this.context.Verify(x => x.Add(model), Times.Once);

            this.context.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(new InvalidOperationException());
            operationResult = await this.manager.CreateEntity(model);
            Assert.That(operationResult.Succeeded, Is.False);
        }

        [Test]
        public async Task VerifyUpdateAndDelete()
        {
            var model = new Model(Guid.NewGuid())
            {
                FileName = "filename.zip",
                ModelName = "Envision - Iteration 1"
            };

            var operationResult = await this.manager.DeleteEntity(model);
            Assert.That(operationResult.Succeeded, Is.False);

            operationResult = await this.manager.UpdateEntity(model);
            Assert.That(operationResult.Succeeded, Is.False);
        }

        [Test]
        public async Task VerifyUpdateProperties()
        {
            var model = new Model();
            var dto = new ReviewDto() as EntityDto;

            Assert.That(async () => await this.manager.ResolveProperties(model, dto), Throws.Nothing);

            dto = new ModelDto()
            {
                FileName = "file.zip",
                ModelName = "model name"
            };

            await this.manager.ResolveProperties(model, dto);
            Assert.That(model.ModelName, Is.EqualTo(((ModelDto)dto).ModelName));
        }
    }
}
