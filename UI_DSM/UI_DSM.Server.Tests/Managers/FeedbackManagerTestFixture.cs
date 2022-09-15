// --------------------------------------------------------------------------------------------------------
// <copyright file="FeedbackManagerTestFixture.cs" company="RHEA System S.A.">
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
    using UI_DSM.Server.Managers.AnnotatableItemManager;
    using UI_DSM.Server.Managers.FeedbackManager;
    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Server.Tests.Helpers;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class FeedbackManagerTestFixture
    {
        private FeedbackManager manager;
        private Mock<DatabaseContext> context;
        private Mock<IParticipantManager> participantManager;
        private Mock<IAnnotatableItemManager> annotatableItemManager;
        private Participant participant;
        private List<AnnotatableItem> annotatableItems;

        [SetUp]
        public void Setup()
        {
            this.context = new Mock<DatabaseContext>();
            this.participantManager = new Mock<IParticipantManager>();
            this.annotatableItemManager = new Mock<IAnnotatableItemManager>();
            this.manager = new FeedbackManager(this.context.Object, this.participantManager.Object);
            this.manager.InjectManager(this.annotatableItemManager.Object);
            
            this.participant = new Participant(Guid.NewGuid())
            {
                Role = new Role(Guid.NewGuid()),
                User = new UserEntity(Guid.NewGuid())
            };

            this.annotatableItems = new List<AnnotatableItem>
            {
                new ReviewObjective(Guid.NewGuid())
                {
                    Author = this.participant
                }
            };
        }

        [Test]
        public async Task VerifyGetFeedbacks()
        {
            var feedbacks = new List<Feedback>
            {
                new (Guid.NewGuid())
                {
                    Author = this.participant,
                    AnnotatableItems = this.annotatableItems
                },
                new (Guid.NewGuid())
                {
                    Author = this.participant,
                    AnnotatableItems = this.annotatableItems
                },
                new (Guid.NewGuid())
                {
                    Author = this.participant,
                    AnnotatableItems = this.annotatableItems
                }
            };

            var dbSet = DbSetMockHelper.CreateMock(feedbacks);

            foreach (var feedback in feedbacks)
            {
                dbSet.Setup(x => x.FindAsync(feedback.Id)).ReturnsAsync(feedback);
            }

            this.context.Setup(x => x.Feedbacks).Returns(dbSet.Object);

            var getEntities = await this.manager.GetEntities();
            Assert.That(getEntities.ToList(), Has.Count.EqualTo(7));
            getEntities = await this.manager.GetEntity(Guid.NewGuid());
            Assert.That(getEntities, Is.Empty);
            getEntities = await this.manager.GetEntity(feedbacks.First().Id);
            Assert.That(getEntities.ToList(), Has.Count.EqualTo(5));

            var foundEntities = await this.manager.FindEntities(feedbacks.Select(x => x.Id));
            Assert.That(foundEntities.ToList(), Has.Count.EqualTo(3));
        }

        [Test]
        public async Task VerifyCreateFeedback()
        {
            var feedback = new Feedback();
            var operationResult = await this.manager.CreateEntity(feedback);
            Assert.That(operationResult.Succeeded, Is.False);

            feedback.Author = this.participant;
            feedback.Content = "A feedback";
            feedback.AnnotatableItems = this.annotatableItems;

            operationResult = await this.manager.CreateEntity(feedback);
            Assert.That(operationResult.Succeeded, Is.False);

            var projects = new List<Project>()
            {
                new(Guid.NewGuid())
                {
                    Annotations =
                    {
                        feedback
                    }
                }
            };

            var projectDbSet = DbSetMockHelper.CreateMock(projects);
            this.context.Setup(x => x.Projects).Returns(projectDbSet.Object);
            projectDbSet.Setup(x => x.FindAsync(projects.First().Id)).ReturnsAsync(projects.First());

            await this.manager.CreateEntity(feedback);
            this.context.Verify(x => x.Add(feedback), Times.Once);
            this.context.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(new InvalidOperationException());
            operationResult = await this.manager.CreateEntity(feedback);
            Assert.That(operationResult.Succeeded, Is.False);
        }

        [Test]
        public async Task VerifyUpdateFeedback()
        {
            var feedback = new Feedback(Guid.NewGuid());
            var operationResult = await this.manager.UpdateEntity(feedback);
            Assert.That(operationResult.Succeeded, Is.False);

            feedback.Author = this.participant;
            feedback.Content = "A feedback";
            feedback.AnnotatableItems = this.annotatableItems;

            operationResult = await this.manager.CreateEntity(feedback);
            Assert.That(operationResult.Succeeded, Is.False);

            var projects = new List<Project>()
            {
                new(Guid.NewGuid())
                {
                    Annotations =
                    {
                        feedback
                    }
                }
            };

            var projectDbSet = DbSetMockHelper.CreateMock(projects);
            this.context.Setup(x => x.Projects).Returns(projectDbSet.Object);
            projectDbSet.Setup(x => x.FindAsync(projects.First().Id)).ReturnsAsync(projects.First());

            await this.manager.UpdateEntity(feedback);
            this.context.Verify(x => x.Update(feedback), Times.Once);
            this.context.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(new InvalidOperationException());
            operationResult = await this.manager.UpdateEntity(feedback);
            Assert.That(operationResult.Succeeded, Is.False);
        }

        [Test]
        public async Task VerifyDeleteFeedback()
        {
            var feedback = new Feedback(Guid.NewGuid());
            var operationResult = await this.manager.CreateEntity(feedback);
            Assert.That(operationResult.Succeeded, Is.False);

            var projects = new List<Project>()
            {
                new(Guid.NewGuid())
                {
                    Annotations =
                    {
                        feedback
                    }
                }
            };

            var projectDbSet = DbSetMockHelper.CreateMock(projects);
            this.context.Setup(x => x.Projects).Returns(projectDbSet.Object);
            projectDbSet.Setup(x => x.FindAsync(projects.First().Id)).ReturnsAsync(projects.First());
            await this.manager.DeleteEntity(feedback);
            this.context.Verify(x => x.Remove(feedback), Times.Once);
            this.context.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(new InvalidOperationException());
            operationResult = await this.manager.DeleteEntity(feedback);
            Assert.That(operationResult.Succeeded, Is.False);
        }

        [Test]
        public async Task VerifyResolveProperties()
        {
            var feedbackDto = new FeedbackDto(Guid.NewGuid())
            {
                AnnotatableItems = this.annotatableItems.Select(x => x.Id).ToList(),
                Author = this.participant.Id,
                Content = "A content",
                CreatedOn = DateTime.UtcNow.Subtract(TimeSpan.FromDays(2))
            };

            this.participantManager.Setup(x => x.FindEntity(this.participant.Id)).ReturnsAsync(this.participant);
            this.annotatableItemManager.Setup(x => x.FindEntities(It.IsAny<List<Guid>>())).ReturnsAsync(this.annotatableItems);
            var feedback = (Feedback)feedbackDto.InstantiatePoco();
            await this.manager.ResolveProperties(feedback, new CommentDto(Guid.NewGuid()));
           
            Assert.Multiple(() =>
            {
                this.participantManager.Verify(x => x.FindEntity(this.participant.Id), Times.Never());
                this.annotatableItemManager.Verify(x => x.FindEntities(It.IsAny<List<Guid>>()), Times.Never());
            });

            await this.manager.ResolveProperties(feedback, feedbackDto);
            
            Assert.Multiple(() =>
            {
                Assert.That(feedback.AnnotatableItems, Is.Not.Empty);
                Assert.That(feedback.Author, Is.Not.Null);
            });
        }
    }
}
