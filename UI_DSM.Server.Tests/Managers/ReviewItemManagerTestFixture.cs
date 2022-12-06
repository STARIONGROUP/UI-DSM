// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewItemManagerTestFixture.cs" company="RHEA System S.A.">
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
    using UI_DSM.Server.Managers.AnnotationManager;
    using UI_DSM.Server.Managers.ReviewCategoryManager;
    using UI_DSM.Server.Managers.ReviewItemManager;
    using UI_DSM.Server.Tests.Helpers;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class ReviewItemManagerTestFixture
    {
        private ReviewItemManager manager;
        private Mock<DatabaseContext> context;
        private Mock<IReviewCategoryManager> reviewCategoryManager;
        private Mock<IAnnotationManager> annotationManager;
        private Mock<DbSet<ReviewItem>> reviewItemDbSet;
        private Mock<DbSet<Review>> reviewDbSet;

        [SetUp]
        public void Setup()
        {
            this.context = new Mock<DatabaseContext>();
            this.context.CreateDbSetForContext(out this.reviewItemDbSet, out this.reviewDbSet);
            this.annotationManager = new Mock<IAnnotationManager>();
            this.reviewCategoryManager = new Mock<IReviewCategoryManager>();
            this.manager = new ReviewItemManager(this.context.Object,this.reviewCategoryManager.Object, this.annotationManager.Object);
            Program.RegisterEntities();
        }

        [Test]
        public void VerifyResolveProperties()
        {
            Assert.Multiple(() =>
            {
                Assert.That(async () => await this.manager.ResolveProperties(new ReviewItem(), new CommentDto()), Throws.Nothing);
                Assert.That(async () => await this.manager.ResolveProperties(new ReviewItem(), new ReviewItemDto()), Throws.Nothing);
            });
        }

        [Test]
        public async Task VerifyGetReviewItemsForThings()
        {
            var reviewItems = new List<ReviewItem>
            {
                new (Guid.NewGuid())
                {
                    ThingId = Guid.NewGuid()
                },
                new (Guid.NewGuid())
                {
                    ThingId = Guid.NewGuid()
                }
            };

            var firstReview = new Review(Guid.NewGuid())
            {
                ReviewItems = { reviewItems[0] }
            };

            _ = new Review(Guid.NewGuid())
            {
                ReviewItems = { reviewItems[1] }
            };

            var thingIds = new List<Guid>(reviewItems.Select(x => x.ThingId)) { Guid.NewGuid() };
            this.reviewItemDbSet.UpdateDbSetCollection(reviewItems);
            var retrievedItems = await this.manager.GetReviewItemsForThings(firstReview.Id, thingIds);
            Assert.That(retrievedItems.ToList(), Has.Count.EqualTo(1));
        }

        [Test]
        public async Task VerifyUpdate()
        {
            var reviewItem = new ReviewItem(Guid.NewGuid());
            var result = await this.manager.UpdateEntity(reviewItem);
            Assert.That(result.Succeeded, Is.False);

            reviewItem.ThingId = Guid.NewGuid();

            var review = new Review(Guid.NewGuid())
            {
                ReviewItems = { reviewItem }
            };

            this.reviewDbSet.UpdateDbSetCollection(new List<Review>{review});
            this.reviewItemDbSet.UpdateDbSetCollection(new List<ReviewItem>{reviewItem});
            await this.manager.UpdateEntity(reviewItem);
            this.context.Verify(x => x.Update(reviewItem), Times.Once);

            this.context.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(new InvalidOperationException());
            result = await this.manager.UpdateEntity(reviewItem);
            Assert.That(result.Succeeded, Is.False);
        }
    }
}
