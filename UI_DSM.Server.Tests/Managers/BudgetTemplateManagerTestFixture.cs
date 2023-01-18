// --------------------------------------------------------------------------------------------------------
// <copyright file="BudgetTemplateManagerTestFixture.cs" company="RHEA System S.A.">
//  Copyright (c) 2023 RHEA System S.A.
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
    using UI_DSM.Server.Managers.BudgetTemplateManager;
    using UI_DSM.Server.Tests.Helpers;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class BudgetTemplateManagerTestFixture
    {
        private BudgetTemplateManager manager;
        private Mock<DatabaseContext> context;
        private Mock<DbSet<BudgetTemplate>> budgetDbSet;
        private Mock<DbSet<Project>> projectDbSet;

        [SetUp]
        public void Setup()
        {
            this.context = new Mock<DatabaseContext>();
            this.context.CreateDbSetForContext(out this.budgetDbSet, out this.projectDbSet);

            this.manager = new BudgetTemplateManager(this.context.Object);
            Program.RegisterEntities();
        }

        [Test]
        public async Task VerifyGetEntities()
        {
            var budgetTemplates = new List<BudgetTemplate>
            {
                new(Guid.NewGuid())
                {
                    FileName = "AFileName.zip",
                    BudgetName = "MassBudget"
                },
                new(Guid.NewGuid())
                {
                    FileName = "AFileName2.zip",
                    BudgetName = "powerBudget"
                }
            };

            this.budgetDbSet.UpdateDbSetCollection(budgetTemplates);

            var allEntities = await this.manager.GetEntities(1);
            Assert.That(allEntities.ToList(), Has.Count.EqualTo(2));

            var invalidGuid = Guid.NewGuid();
            var foundReview = await this.manager.FindEntity(invalidGuid);
            Assert.That(foundReview, Is.Null);

            var foundEntities = await this.manager.FindEntities(budgetTemplates.Select(x => x.Id));
            Assert.That(foundEntities.ToList(), Has.Count.EqualTo(2));

            var deepEntity = await this.manager.GetEntity(budgetTemplates.First().Id, 1);
            Assert.That(deepEntity.ToList(), Has.Count.EqualTo(1));

            var project = new Project(Guid.NewGuid())
            {
                Artifacts = { budgetTemplates.First() }
            };

            var containedEntities = await this.manager.GetContainedEntities(project.Id);
            Assert.That(containedEntities.ToList(), Has.Count.EqualTo(1));
        }

        [Test]
        public async Task VerifyCreateEntity()
        {
            var budgetTemplate = new BudgetTemplate()
            {
                FileName = "filename.zip",
                BudgetName = "Mass budget"
            };

            var operationResult = await this.manager.CreateEntity(budgetTemplate);
            Assert.That(operationResult.Succeeded, Is.False);

            var projects = new List<Project>()
            {
                new(Guid.NewGuid())
                {
                    Artifacts =
                    {
                        budgetTemplate
                    }
                }
            };

            this.projectDbSet.UpdateDbSetCollection(projects);

            await this.manager.CreateEntity(budgetTemplate);
            this.context.Verify(x => x.Add(budgetTemplate), Times.Once);

            this.context.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(new InvalidOperationException());
            operationResult = await this.manager.CreateEntity(budgetTemplate);
            Assert.That(operationResult.Succeeded, Is.False);
        }

        [Test]
        public async Task VerifyUpdateAndDelete()
        {
            var budgetTemplate = new BudgetTemplate(Guid.NewGuid())
            {
                FileName = "filename.zip",
                BudgetName = "pwr budget"
            };

            var operationResult = await this.manager.DeleteEntity(budgetTemplate);
            Assert.That(operationResult.Succeeded, Is.False);

            operationResult = await this.manager.UpdateEntity(budgetTemplate);
            Assert.That(operationResult.Succeeded, Is.False);
        }

        [Test]
        public async Task VerifyUpdateProperties()
        {
            var budgetTemplate = new BudgetTemplate();
            var dto = new ReviewDto() as EntityDto;

            Assert.That(async () => await this.manager.ResolveProperties(budgetTemplate, dto), Throws.Nothing);

            dto = new BudgetTemplateDto()
            {
                FileName = "file.zip",
                BudgetName = "budget name"
            };

            await this.manager.ResolveProperties(budgetTemplate, dto);
            Assert.That(budgetTemplate.BudgetName, Is.EqualTo(((BudgetTemplateDto)dto).BudgetName));
        }

        [Test]
        public async Task VerifyGetSearchResult()
        {
            var budgetTemplate = new BudgetTemplate(Guid.NewGuid())
            {
                EntityContainer = new Project(Guid.NewGuid())
            };

            var result = await this.manager.GetSearchResult(budgetTemplate.Id);
            Assert.That(result, Is.Null);

            this.budgetDbSet.UpdateDbSetCollection(new List<BudgetTemplate> { budgetTemplate });
            result = await this.manager.GetSearchResult(budgetTemplate.Id);
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task VerifyGetExtraEntitiesToUnindex()
        {
            var result = await this.manager.GetExtraEntitiesToUnindex(Guid.NewGuid());
            Assert.That(result, Is.Empty);
        }
    }
}
