// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewTaskManagerTestFixture.cs" company="RHEA System S.A.">
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
    using Microsoft.EntityFrameworkCore;

    using Moq;
 
    using NUnit.Framework;
    
    using UI_DSM.Server.Context;
    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Server.Managers.ReviewObjectiveManager;
    using UI_DSM.Server.Managers.ReviewTaskManager;
    using UI_DSM.Server.Tests.Helpers;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class ReviewTaskManagerTestFixture
    {
        private ReviewTaskManager manager;
        private Mock<DatabaseContext> context;
        private Mock<IParticipantManager> participantManager;
        private Mock<DbSet<ReviewTask>> reviewTaskDbSet;
        private Mock<DbSet<ReviewObjective>> reviewObjectiveDbSet;
        private Mock<IReviewObjectiveManager> reviewObjectiveManager;

        [SetUp]
        public void Setup()
        {
            this.context = new Mock<DatabaseContext>();
            this.context.CreateDbSetForContext(out this.reviewTaskDbSet, out this.reviewObjectiveDbSet);
            this.participantManager = new Mock<IParticipantManager>();
            this.reviewObjectiveManager = new Mock<IReviewObjectiveManager>();
            this.manager = new ReviewTaskManager(this.context.Object, this.participantManager.Object);
            Program.RegisterEntities();
        }

        [Test]
        public async Task VerifyGetEntities()
        {
            var participant = new Participant(Guid.NewGuid())
            {
                Role = new Role(Guid.NewGuid()),
                User = new UserEntity(Guid.NewGuid())
            };

            var reviewTasks = new List<ReviewTask>()
            {
                new(Guid.NewGuid())
                {
                    Author = participant,
                    CreatedOn = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)),
                    Description = "A review objective",
                    TaskNumber = 1,
                    Status = StatusKind.Closed,
                    IsAssignedTo = participant,
                    Title = "Review objective 1"
                },
                new(Guid.NewGuid())
                {
                    Author = participant,
                    CreatedOn = DateTime.UtcNow,
                    Description = "Another review objective",
                    TaskNumber = 1,
                    Status = StatusKind.Open,
                    Title = "Review objective 2"
                }
            };

            this.reviewTaskDbSet.UpdateDbSetCollection(reviewTasks);

            var allEntities = await this.manager.GetEntities(1);
            Assert.That(allEntities.ToList(), Has.Count.EqualTo(5));

            var invalidGuid = Guid.NewGuid();
            var foundReview = await this.manager.FindEntity(invalidGuid);
            Assert.That(foundReview, Is.Null);

            var foundEntities = await this.manager.FindEntities(reviewTasks.Select(x => x.Id));
            Assert.That(foundEntities.ToList(), Has.Count.EqualTo(2));

            var deepEntity = await this.manager.GetEntity(reviewTasks.First().Id, 1);
            Assert.That(deepEntity.ToList(), Has.Count.EqualTo(4));

            var reviewObjective = new ReviewObjective(Guid.NewGuid())
            {
                ReviewTasks = { reviewTasks.First() }
            };

            var containedEntities = await this.manager.GetContainedEntities(reviewObjective.Id);
            Assert.That(containedEntities.ToList(), Has.Count.EqualTo(4));
        }

        [Test]
        public async Task VerifyCreateEntity()
        {
            var reviewTask = new ReviewTask()
            {
                Title = "Reveiw Task Title",
                Author = new Participant(Guid.NewGuid())
            };

            var operationResult = await this.manager.CreateEntity(reviewTask);
            Assert.That(operationResult.Succeeded, Is.False);
            reviewTask.Description = "Review Task Description";

            operationResult = await this.manager.CreateEntity(reviewTask);
            Assert.That(operationResult.Succeeded, Is.False);

            var reviewObjectives = new List<ReviewObjective>()
            {
                new(Guid.NewGuid())
                {
                    ReviewTasks =
                    {
                        reviewTask
                    }
                }
            };

            this.reviewObjectiveDbSet.UpdateDbSetCollection(reviewObjectives);

            await this.manager.CreateEntity(reviewTask);
            this.context.Verify(x => x.Add(reviewTask), Times.Once);

            this.context.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(new InvalidOperationException());
            operationResult = await this.manager.CreateEntity(reviewTask);
            Assert.That(operationResult.Succeeded, Is.False);
        }

        [Test]
        public async Task VerifyUpdateEntity()
        {
            var reviewTask = new ReviewTask(Guid.NewGuid())
            {
                Title = "Review Title",
                Author = new Participant(Guid.NewGuid())
            };

            var operationResult = await this.manager.UpdateEntity(reviewTask);
            Assert.That(operationResult.Succeeded, Is.False);

            reviewTask.Description = "Review new Description";
            
            var reviewObjectives = new List<ReviewObjective>()
            {
                new(Guid.NewGuid())
                {
                    ReviewTasks =
                    {
                        reviewTask
                    }
                }
            };

            this.reviewTaskDbSet.UpdateDbSetCollection(reviewObjectives.SelectMany(x => x.ReviewTasks).ToList());
            this.reviewObjectiveDbSet.UpdateDbSetCollection(reviewObjectives);

            await this.manager.UpdateEntity(reviewTask);
            this.context.Verify(x => x.Update(reviewTask), Times.Once);
            this.reviewObjectiveManager.Verify(x => x.UpdateStatus(It.IsAny<Guid>()), Times.Never);

            this.manager.InjectManager(this.reviewObjectiveManager.Object);
            await this.manager.UpdateEntity(reviewTask);
            this.reviewObjectiveManager.Verify(x => x.UpdateStatus(It.IsAny<Guid>()), Times.Once);

            this.context.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(new InvalidOperationException());
            operationResult = await this.manager.UpdateEntity(reviewTask);
            Assert.That(operationResult.Succeeded, Is.False);
        }

        [Test]
        public async Task VerifyDelete()
        {
            var reviewTask = new ReviewTask();
            var operationResult = await this.manager.DeleteEntity(reviewTask);
            Assert.That(operationResult.Succeeded, Is.False);

            var reviewObjectives = new List<ReviewObjective>()
            {
                new(Guid.NewGuid())
                {
                    ReviewTasks =
                    {
                        reviewTask
                    }
                }
            };

            this.reviewObjectiveDbSet.UpdateDbSetCollection(reviewObjectives);

            await this.manager.DeleteEntity(reviewTask);
            this.context.Verify(x => x.Remove(reviewTask), Times.Once);

            this.context.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(new InvalidOperationException());
            operationResult = await this.manager.DeleteEntity(reviewTask);
            Assert.That(operationResult.Succeeded, Is.False);
        }

        [Test]
        public async Task VerifyResolveProperties()
        {
            var participant = new Participant(Guid.NewGuid());
            var assignedParticipant = new Participant(Guid.NewGuid());
            this.participantManager.Setup(x => x.FindEntity(participant.Id)).ReturnsAsync(participant);
            this.participantManager.Setup(x => x.FindEntity(assignedParticipant.Id)).ReturnsAsync(assignedParticipant);

            var reviewObjective = new ReviewTask();
            await this.manager.ResolveProperties(reviewObjective, new UserEntityDto());
            Assert.That(reviewObjective.Author, Is.Null);

            var reviewDto = new ReviewTaskDto()
            {
                Author = participant.Id,
                IsAssignedTo = assignedParticipant.Id
            };

            await this.manager.ResolveProperties(reviewObjective, reviewDto);

            Assert.Multiple(() =>
            {
                Assert.That(reviewObjective.Author, Is.EqualTo(participant));
                Assert.That(reviewObjective.IsAssignedTo, Is.EqualTo(assignedParticipant));
            });
        }
    }
}
