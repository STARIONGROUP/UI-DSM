// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewCategoryManagerTestFixture.cs" company="RHEA System S.A.">
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

    using Npgsql;

    using NUnit.Framework;

    using UI_DSM.Server.Context;
    using UI_DSM.Server.Managers.ReviewCategoryManager;
    using UI_DSM.Server.Tests.Helpers;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class ReviewCategoryManagerTestFixture
    {
        private ReviewCategoryManager manager;
        private Mock<DatabaseContext> context;
        private List<ReviewCategory> data;
        private Mock<DbSet<ReviewCategory>> reviewCategoryDbSet;

        [SetUp]
        public void Setup()
        {
            this.context = new Mock<DatabaseContext>();
            this.context.CreateDbSetForContext(out this.reviewCategoryDbSet);
            this.manager = new ReviewCategoryManager(this.context.Object);

            this.data = new List<ReviewCategory>()
            {
                new(Guid.NewGuid())
                {
                    ReviewCategoryName = "ReviewCategory Name 1",
                    Description = "ReviewCategory Description 1",
                    TagColor = "#951CD1"
                },
                new(Guid.NewGuid())
                {
                    ReviewCategoryName = "ReviewCategory Name 2",
                    Description = "ReviewCategory Description 2",
                    TagColor = "#E00D30"
                }
            };

            this.reviewCategoryDbSet.UpdateDbSetCollection(this.data);
            Program.RegisterEntities();
        }

        [Test]
        public async Task VerifyGetReviewCategories()
        {
            var invalidGuid = Guid.NewGuid();

            Assert.Multiple(() =>
            {
                Assert.That(this.manager.GetEntities().Result.Count(), Is.EqualTo(this.data.Count));
                Assert.That(this.manager.GetEntity(invalidGuid).Result, Is.Empty);
                Assert.That(this.manager.GetEntity(this.data.Last().Id).Result, Is.Not.Null);
            });

            var foundEntities = await this.manager.FindEntities(this.data.Select(x => x.Id));
            Assert.That(foundEntities.Count(), Is.EqualTo(this.data.Count));
        }

        [Test]
        public async Task VerifyCreateReviewCategory()
        {
            var dbSet = DbSetMockHelper.CreateMock(this.data);
            this.context.Setup(x => x.ReviewCategories).Returns(dbSet.Object);

            var newReviewCategory = new ReviewCategory()
            {
                ReviewCategoryName = "ReviewCategory",
                Description = "ReviewCategory",
                TagColor = "#CB7777"
            };

            var creationResult = await this.manager.CreateEntity(newReviewCategory);
            Assert.That(creationResult.Errors, Is.Empty);

            newReviewCategory.ReviewCategoryName = this.data.First().ReviewCategoryName;

            this.context.Setup(x => x.SaveChangesAsync(default))
                .ThrowsAsync(new PostgresException("unique constraint", "High", "High", "23505"));

            creationResult = await this.manager.CreateEntity(newReviewCategory);
            Assert.That(creationResult.Errors.First(), Does.Contain("already exists"));

            this.context.Setup(x => x.SaveChangesAsync(default))
                .ThrowsAsync(new InvalidOperationException());

            creationResult = this.manager.CreateEntity(newReviewCategory).Result;
            Assert.That(creationResult.Errors, Is.Not.Empty);
        }

        [Test]
        public async Task VerifyUpdateReviewCategory()
        {
            var dbSet = DbSetMockHelper.CreateMock(this.data);
            this.context.Setup(x => x.ReviewCategories).Returns(dbSet.Object);

            var reviewCategory = new ReviewCategory(this.data.First().Id)
            {
                ReviewCategoryName = "ReviewCategory",
                Description = "ReviewCategory",
                TagColor = "#B08416"
            };

            var updateResult = await this.manager.UpdateEntity(reviewCategory);
            Assert.That(updateResult.Errors, Is.Empty);

            this.context.Setup(x => x.SaveChangesAsync(default))
                .ThrowsAsync(new PostgresException("unique constraint", "High", "High", "23505"));

            reviewCategory.ReviewCategoryName = this.data.First().ReviewCategoryName;
            updateResult = await this.manager.UpdateEntity(reviewCategory);
            Assert.That(updateResult.Errors.First(), Does.Contain("already exist"));

            this.context.Setup(x => x.SaveChangesAsync(default))
                .ThrowsAsync(new InvalidOperationException());

            updateResult = await this.manager.UpdateEntity(reviewCategory);
            Assert.That(updateResult.Errors, Is.Not.Empty);
        }

        [Test]
        public void VerifyDeleteReviewCategory()
        {
            var reviewCategory = new ReviewCategory();
            _ = this.manager.DeleteEntity(reviewCategory).Result;

            this.context.Verify(x => x.Remove(It.IsAny<ReviewCategory>()), Times.Once);

            this.context.Setup(x => x.SaveChangesAsync(default))
                .ThrowsAsync(new InvalidOperationException());

            var creationResult = this.manager.DeleteEntity(reviewCategory).Result;
            Assert.That(creationResult.Succeeded, Is.False);
        }
    }
}
