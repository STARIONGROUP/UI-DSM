// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewObjectiveManagerTestFixture.cs" company="RHEA System S.A.">
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

    using UI_DSM.Server.Context;
    using UI_DSM.Server.Managers.AnnotationManager;
    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Server.Managers.ReviewObjectiveManager;
    using UI_DSM.Server.Managers.ReviewTaskManager;
    using UI_DSM.Server.Tests.Helpers;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class ReviewObjectiveManagerTestFixture
    {
        private ReviewObjectiveManager manager;
        private Mock<DatabaseContext> context;
        private Mock<IParticipantManager> participantManager;
        private Mock<IReviewTaskManager> reviewTaskManager;
        private Mock<IAnnotationManager> annotationManager;

        [SetUp]
        public void Setup()
        {
            this.context = new Mock<DatabaseContext>();
            this.participantManager = new Mock<IParticipantManager>();
            this.reviewTaskManager = new Mock<IReviewTaskManager>();
            this.annotationManager = new Mock<IAnnotationManager>();

            this.manager = new ReviewObjectiveManager(this.context.Object, this.participantManager.Object, this.reviewTaskManager.Object,
                this.annotationManager.Object);
        }

        [Test]
        public async Task VerifyGetEntities()
        {
            var participant = new Participant(Guid.NewGuid())
            {
                Role = new Role(Guid.NewGuid()),
                User = new UserEntity(Guid.NewGuid())
            };

            var reviewObjectives = new List<ReviewObjective>()
            {
                new(Guid.NewGuid())
                {
                    Author = participant,
                    CreatedOn = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)),
                    Description = "A review objective",
                    ReviewObjectiveNumber = 1,
                    Status = StatusKind.Closed,
                    Title = "Review objective 1"
                },
                new(Guid.NewGuid())
                {
                    Author = participant,
                    CreatedOn = DateTime.UtcNow,
                    Description = "Another review objective",
                    ReviewObjectiveNumber = 1,
                    Status = StatusKind.Open,
                    Title = "Review objective 2"
                }
            };

            var dbSet = DbSetMockHelper.CreateMock(reviewObjectives);
            this.context.Setup(x => x.ReviewObjectives).Returns(dbSet.Object);

            foreach (var reviewObjective in reviewObjectives)
            {
                dbSet.Setup(x => x.FindAsync(reviewObjective.Id)).ReturnsAsync(reviewObjective);
            }

            var allEntities = await this.manager.GetEntities(1);
            Assert.That(allEntities.ToList(), Has.Count.EqualTo(5));

            var invalidGuid = Guid.NewGuid();
            dbSet.Setup(x => x.FindAsync(invalidGuid)).ReturnsAsync((ReviewObjective)null);
            var foundReview = await this.manager.FindEntity(invalidGuid);
            Assert.That(foundReview, Is.Null);

            var foundEntities = await this.manager.FindEntities(reviewObjectives.Select(x => x.Id));
            Assert.That(foundEntities.ToList(), Has.Count.EqualTo(2));

            var deepEntity = await this.manager.GetEntity(reviewObjectives.First().Id, 1);
            Assert.That(deepEntity.ToList(), Has.Count.EqualTo(4));

            var review = new Review(Guid.NewGuid())
            {
                ReviewObjectives = { reviewObjectives.First() }
            };

            var containedEntities = await this.manager.GetContainedEntities(review.Id);
            Assert.That(containedEntities.ToList(), Has.Count.EqualTo(4));
        }

        [Test]
        public async Task VerifyCreateEntity()
        {
            var reviewObjective = new ReviewObjective()
            {
                Title = "ReviewObjective Title",
                Author = new Participant(Guid.NewGuid())
            };

            var operationResult = await this.manager.CreateEntity(reviewObjective);
            Assert.That(operationResult.Succeeded, Is.False);
            reviewObjective.Description = "Review Description";

            operationResult = await this.manager.CreateEntity(reviewObjective);
            Assert.That(operationResult.Succeeded, Is.False);

            var reviews = new List<Review>()
            {
                new(Guid.NewGuid())
                {
                    ReviewObjectives =
                    {
                        reviewObjective
                    }
                }
            };

            var reviewDbSet = DbSetMockHelper.CreateMock(reviews);
            this.context.Setup(x => x.Reviews).Returns(reviewDbSet.Object);
            reviewDbSet.Setup(x => x.FindAsync(reviews.First().Id)).ReturnsAsync(reviews.First());

            await this.manager.CreateEntity(reviewObjective);
            this.context.Verify(x => x.Add(reviewObjective), Times.Once);

            this.context.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(new InvalidOperationException());
            operationResult = await this.manager.CreateEntity(reviewObjective);
            Assert.That(operationResult.Succeeded, Is.False);
        }

        [Test]
        public async Task VerifyUpdateEntity()
        {
            var reviewObjective = new ReviewObjective(Guid.NewGuid())
            {
                Title = "Review Title",
                Author = new Participant(Guid.NewGuid())
            };

            var operationResult = await this.manager.UpdateEntity(reviewObjective);
            Assert.That(operationResult.Succeeded, Is.False);
            reviewObjective.Description = "Review new Description";
            await this.manager.UpdateEntity(reviewObjective);
            this.context.Verify(x => x.Update(reviewObjective), Times.Once);

            this.context.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(new InvalidOperationException());
            operationResult = await this.manager.UpdateEntity(reviewObjective);
            Assert.That(operationResult.Succeeded, Is.False);
        }

        [Test]
        public async Task VerifyDelete()
        {
            var reviewObjective = new ReviewObjective();
            var operationResult = await this.manager.DeleteEntity(reviewObjective);
            Assert.That(operationResult.Succeeded, Is.False);

            reviewObjective.EntityContainer = new Review(Guid.NewGuid());
            var dbSet = DbSetMockHelper.CreateMock(new List<Review>());
            this.context.Setup(x => x.Reviews).Returns(dbSet.Object);

            dbSet.Setup(x => x.FindAsync(reviewObjective.EntityContainer.Id)).ReturnsAsync((Review)null);
            operationResult = await this.manager.DeleteEntity(reviewObjective);
            Assert.That(operationResult.Succeeded, Is.False);

            dbSet.Setup(x => x.FindAsync(reviewObjective.EntityContainer.Id)).ReturnsAsync((Review)reviewObjective.EntityContainer);
            await this.manager.DeleteEntity(reviewObjective);
            this.context.Verify(x => x.Remove(reviewObjective), Times.Once);

            this.context.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(new InvalidOperationException());
            operationResult = await this.manager.DeleteEntity(reviewObjective);
            Assert.That(operationResult.Succeeded, Is.False);
        }

        [Test]
        public async Task VerifyResolveProperties()
        {
            var participant = new Participant(Guid.NewGuid());
            this.participantManager.Setup(x => x.FindEntity(participant.Id)).ReturnsAsync(participant);

            var tasks = new List<ReviewTask>
            {
                new(Guid.NewGuid()),
                new(Guid.NewGuid()),
                new(Guid.NewGuid())
            };

            this.reviewTaskManager.Setup(x => x.FindEntities(It.IsAny<List<Guid>>())).ReturnsAsync(tasks);

            var annotations = new List<Annotation>()
            {
                new Comment(Guid.NewGuid()),
                new Feedback(Guid.NewGuid()),
                new Note(Guid.NewGuid())
            };

            this.annotationManager.Setup(x => x.FindEntities(It.IsAny<List<Guid>>())).ReturnsAsync(annotations);

            var reviewObjective = new ReviewObjective();
            await this.manager.ResolveProperties(reviewObjective, new UserEntityDto());
            Assert.That(reviewObjective.Author, Is.Null);

            var reviewDto = new ReviewObjectiveDto()
            {
                Author = participant.Id,
                ReviewTasks = tasks.Select(x => x.Id).ToList(),
                Annotations = annotations.Select(x => x.Id).ToList()
            };

            await this.manager.ResolveProperties(reviewObjective, reviewDto);
           
            Assert.Multiple(() =>
            {
                Assert.That(reviewObjective.Author, Is.EqualTo(participant));
                Assert.That(reviewObjective.ReviewTasks, Has.Count.EqualTo(3));
                Assert.That(reviewObjective.Annotations, Has.Count.EqualTo(3));
            });
        }
    }
}
