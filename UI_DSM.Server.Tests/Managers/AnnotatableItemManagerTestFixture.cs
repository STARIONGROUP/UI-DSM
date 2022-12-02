// --------------------------------------------------------------------------------------------------------
// <copyright file="AnnotatableItemManagerTestFixture.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw
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

    using UI_DSM.Server.Managers.AnnotatableItemManager;
    using UI_DSM.Server.Managers.AnnotationManager;
    using UI_DSM.Server.Managers.ReviewItemManager;
    using UI_DSM.Server.Managers.ReviewObjectiveManager;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class AnnotatableItemManagerTestFixture
    {
        private AnnotatableItemManager manager;
        private Mock<IReviewObjectiveManager> reviewObjectiveManager;
        private Mock<IAnnotationManager> annotationManager;
        private Mock<IReviewItemManager> reviewItemManager;
        private Participant participant;

        [SetUp]
        public void Setup()
        {
            this.participant = new Participant(Guid.NewGuid())
            {
                Role = new Role(Guid.NewGuid()),
                User = new UserEntity(Guid.NewGuid())
            };

            this.reviewObjectiveManager = new Mock<IReviewObjectiveManager>();
            this.annotationManager = new Mock<IAnnotationManager>();
            this.reviewItemManager = new Mock<IReviewItemManager>();
            this.manager = new AnnotatableItemManager(this.reviewObjectiveManager.Object, this.annotationManager.Object, this.reviewItemManager.Object);
            Program.RegisterEntities();
        }

        [Test]
        public async Task VerifyGetEntities()
        {
            var reviewObjectives = new List<ReviewObjective>
            {
                new (Guid.NewGuid())
                {
                    Annotations =
                    {
                        new Comment(Guid.NewGuid())
                        {
                            Author = this.participant
                        }
                    }
                },
                new (Guid.NewGuid())
                {
                    Annotations =
                    {
                        new Comment(Guid.NewGuid())
                        {
                            Author = this.participant
                        }
                    }
                }
            };

            this.reviewObjectiveManager.Setup(x => x.GetEntities(0))
                .ReturnsAsync(reviewObjectives.SelectMany(x => x.GetAssociatedEntities()).DistinctBy(x => x.Id));

            var reviewItems = new List<ReviewItem>
            {
                new(Guid.NewGuid())
                {
                    ThingId = Guid.NewGuid()
                },
                new(Guid.NewGuid())
                {
                    ThingId = Guid.NewGuid()
                }
            };

            this.reviewItemManager.Setup(x => x.GetEntities(0))
                .ReturnsAsync(reviewItems.SelectMany(x => x.GetAssociatedEntities()).DistinctBy(x => x.Id));

            var foundEntities = await this.manager.GetEntities();
            Assert.That(foundEntities.ToList(), Has.Count.EqualTo(9));

            foreach (var reviewObjective in reviewObjectives)
            {
                this.reviewObjectiveManager.Setup(x => x.GetEntity(reviewObjective.Id,0))
                    .ReturnsAsync(reviewObjective.GetAssociatedEntities());

                this.reviewObjectiveManager.Setup(x => x.FindEntity(reviewObjective.Id))
                    .ReturnsAsync(reviewObjective);
            }

            foreach (var reviewItem in reviewItems)
            {
                this.reviewItemManager.Setup(x => x.GetEntity(reviewItem.Id, 0))
                    .ReturnsAsync(reviewItem.GetAssociatedEntities());

                this.reviewItemManager.Setup(x => x.FindEntity(reviewItem.Id))
                    .ReturnsAsync(reviewItem);
            }

            var getEntityResult = await this.manager.GetEntity(Guid.NewGuid());
            Assert.That(getEntityResult, Is.Empty);
            getEntityResult = await this.manager.GetEntity(reviewObjectives.First().Id);
            Assert.That(getEntityResult.ToList(), Has.Count.EqualTo(5));
            getEntityResult = await this.manager.GetEntity(reviewItems.First().Id);
            Assert.That(getEntityResult.ToList(), Has.Count.EqualTo(1));
            var foundAnnotabaleItems = await this.manager.FindEntities(reviewObjectives.Select(x => x.Id));
            Assert.That(foundAnnotabaleItems.ToList(), Has.Count.EqualTo(2));
            foundAnnotabaleItems = await this.manager.FindEntities(reviewItems.Select(x => x.Id));
            Assert.That(foundAnnotabaleItems.ToList(), Has.Count.EqualTo(2));
        }

        [Test]
        public void VerifyUnsupportedMethod()
        {
            Assert.Multiple(() =>
            {
                Assert.That(async () => (await this.manager.CreateEntity(null)).Succeeded, Is.False);
                Assert.That(async () => (await this.manager.UpdateEntity(null)).Succeeded, Is.False);
                Assert.That(async () => (await this.manager.DeleteEntity(null)).Succeeded, Is.False);
                Assert.That(this.manager.ResolveProperties(null, null).IsCompleted, Is.True);
            });
        }
    }
}
