// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewManagerTestFixture.cs" company="RHEA System S.A.">
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
    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Server.Managers.ReviewManager;
    using UI_DSM.Server.Managers.ReviewObjectiveManager;
    using UI_DSM.Server.Tests.Helpers;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class ReviewManagerTestFixture
    {
        private ReviewManager manager;
        private Mock<DatabaseContext> context;
        private Mock<IParticipantManager> participantManager;
        private Mock<IReviewObjectiveManager> reviewObjectiveManager;

        [SetUp]
        public void Setup()
        {
            this.context = new Mock<DatabaseContext>();
            this.participantManager = new Mock<IParticipantManager>();
            this.reviewObjectiveManager = new Mock<IReviewObjectiveManager>();
            this.manager = new ReviewManager(this.context.Object, this.participantManager.Object, this.reviewObjectiveManager.Object);
        }

        [Test]
        public async Task VerifyGetEntities()
        {
            var participant = new Participant(Guid.NewGuid())
            {
                Role = new Role(Guid.NewGuid()),
                User = new UserEntity(Guid.NewGuid())
            };

            var reviews = new List<Review>()
            {
                new(Guid.NewGuid())
                {
                    Author = participant,
                    CreatedOn = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)),
                    Description = "A review",
                    ReviewNumber = 1, 
                    Status = StatusKind.Closed,
                    Title = "Review 1",
                    ReviewObjectives = { new ReviewObjective(Guid.NewGuid()) }
                },
                new(Guid.NewGuid())
                {
                    Author = participant,
                    CreatedOn = DateTime.UtcNow,
                    Description = "Another review",
                    ReviewNumber = 2,
                    Status = StatusKind.Closed,
                    Title = "Review 2"
                }
            };

            var dbSet = DbSetMockHelper.CreateMock(reviews);
            this.context.Setup(x => x.Reviews).Returns(dbSet.Object);

            foreach (var review in reviews)
            {
                dbSet.Setup(x => x.FindAsync(review.Id)).ReturnsAsync(review);
            }

            var allEntities = await this.manager.GetEntities(1);
            Assert.That(allEntities.ToList(), Has.Count.EqualTo(6));

            var invalidGuid = Guid.NewGuid();
            dbSet.Setup(x => x.FindAsync(invalidGuid)).ReturnsAsync((Review)null);
            var foundReview = await this.manager.FindEntity(invalidGuid);
            Assert.That(foundReview, Is.Null);

            var foundEntities = await this.manager.FindEntities(reviews.Select(x => x.Id));
            Assert.That(foundEntities.ToList(), Has.Count.EqualTo(2));

            var deepEntity = await this.manager.GetEntity(reviews.First().Id, 1);
            Assert.That(deepEntity.ToList(), Has.Count.EqualTo(5));

            var project = new Project(Guid.NewGuid())
            {
                Reviews = { reviews.First() }
            };

            var containedEntities = await this.manager.GetContainedEntities(project.Id);
            Assert.That(containedEntities.ToList(), Has.Count.EqualTo(4));
        }

        [Test]
        public async Task VerifyCreateEntity()
        {
            var review = new Review()
            {
                Title = "Review Title",
                Author = new Participant(Guid.NewGuid())
            };

            var operationResult = await this.manager.CreateEntity(review);
            Assert.That(operationResult.Succeeded, Is.False);
            review.Description = "Review Description";

            operationResult = await this.manager.CreateEntity(review);
            Assert.That(operationResult.Succeeded, Is.False);

            var projects = new List<Project>()
            {
                new(Guid.NewGuid())
                {
                    Reviews =
                    {
                        review
                    }
                }
            };

            var projectDbSet = DbSetMockHelper.CreateMock(projects);
            this.context.Setup(x => x.Projects).Returns(projectDbSet.Object);
            projectDbSet.Setup(x => x.FindAsync(projects.First().Id)).ReturnsAsync(projects.First());

            await this.manager.CreateEntity(review);
            this.context.Verify(x => x.Add(review), Times.Once);

            this.context.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(new InvalidOperationException());
            operationResult = await this.manager.CreateEntity(review);
            Assert.That(operationResult.Succeeded, Is.False);
        }

        [Test]
        public async Task VerifyUpdateEntity()
        {
            var review = new Review(Guid.NewGuid())
            {
                Title = "Review Title",
                Author = new Participant(Guid.NewGuid())
            };

            var operationResult = await this.manager.UpdateEntity(review);
            Assert.That(operationResult.Succeeded, Is.False);
            review.Description = "Review new Description";
            await this.manager.UpdateEntity(review);
            this.context.Verify(x => x.Update(review), Times.Once);

            this.context.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(new InvalidOperationException());
            operationResult = await this.manager.UpdateEntity(review);
            Assert.That(operationResult.Succeeded, Is.False);
        }

        [Test]
        public async Task VerifyDelete()
        {
            var review = new Review();
            var operationResult = await this.manager.DeleteEntity(review);
            Assert.That(operationResult.Succeeded, Is.False);

            review.EntityContainer = new Project(Guid.NewGuid());
            var dbSet = DbSetMockHelper.CreateMock(new List<Project>());
            this.context.Setup(x => x.Projects).Returns(dbSet.Object);

            dbSet.Setup(x => x.FindAsync(review.EntityContainer.Id)).ReturnsAsync((Project)null);
            operationResult = await this.manager.DeleteEntity(review);
            Assert.That(operationResult.Succeeded, Is.False);

            dbSet.Setup(x => x.FindAsync(review.EntityContainer.Id)).ReturnsAsync((Project)review.EntityContainer);
            await this.manager.DeleteEntity(review);
            this.context.Verify(x => x.Remove(review), Times.Once);

            this.context.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(new InvalidOperationException());
            operationResult = await this.manager.DeleteEntity(review);
            Assert.That(operationResult.Succeeded, Is.False);
        }

        [Test]
        public async Task VerifyResolveProperties()
        {
            var participant = new Participant(Guid.NewGuid());
            this.participantManager.Setup(x => x.FindEntity(participant.Id)).ReturnsAsync(participant);

            var reviewObjective = new ReviewObjective(Guid.NewGuid());

            this.reviewObjectiveManager.Setup(x => x.FindEntities(It.IsAny<List<Guid>>())).ReturnsAsync(
                new List<ReviewObjective>()
                {
                    reviewObjective
                });

            var review = new Review();
            await this.manager.ResolveProperties(review, new UserDto());
            Assert.That(review.Author, Is.Null);

            var reviewDto = new ReviewDto()
            {
                Author = participant.Id,
                ReviewObjectives = new List<Guid>()
                {
                    reviewObjective.Id
                }
            };

            await this.manager.ResolveProperties(review, reviewDto);

            Assert.Multiple(() =>
            {
                Assert.That(review.Author, Is.EqualTo(participant));
                Assert.That(review.ReviewObjectives, Has.Count.EqualTo(1));
            });
        }
    }
}
