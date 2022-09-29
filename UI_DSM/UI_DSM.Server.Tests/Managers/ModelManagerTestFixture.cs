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
    using Moq;

    using NUnit.Framework;

    using UI_DSM.Server.Context;
    using UI_DSM.Server.Managers.ModelManager;
    using UI_DSM.Server.Tests.Helpers;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class ModelManagerTestFixture
    {
        private ModelManager manager;
        private Mock<DatabaseContext> context;

        [SetUp]
        public void Setup()
        {
            this.context = new Mock<DatabaseContext>();

            this.manager = new ModelManager(this.context.Object);
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

            var dbSet = DbSetMockHelper.CreateMock(models);
            this.context.Setup(x => x.Models).Returns(dbSet.Object);

            foreach (var model in models)
            {
                dbSet.Setup(x => x.FindAsync(model.Id)).ReturnsAsync(model);
            }

            var allEntities = await this.manager.GetEntities(1);
            Assert.That(allEntities.ToList(), Has.Count.EqualTo(2));

            var invalidGuid = Guid.NewGuid();
            dbSet.Setup(x => x.FindAsync(invalidGuid)).ReturnsAsync((Model)null);
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

            var projectDbSet = DbSetMockHelper.CreateMock(projects);
            this.context.Setup(x => x.Projects).Returns(projectDbSet.Object);
            projectDbSet.Setup(x => x.FindAsync(projects.First().Id)).ReturnsAsync(projects.First());

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
