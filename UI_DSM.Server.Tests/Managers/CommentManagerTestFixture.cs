// --------------------------------------------------------------------------------------------------------
// <copyright file="CommentManagerTestFixture.cs" company="RHEA System S.A.">
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
    using UI_DSM.Server.Managers.AnnotatableItemManager;
    using UI_DSM.Server.Managers.CommentManager;
    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Server.Managers.ReplyManager;
    using UI_DSM.Server.Managers.ReviewTaskManager;
    using UI_DSM.Server.Tests.Helpers;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class CommentManagerTestFixture
    {
        private CommentManager manager;
        private Mock<DatabaseContext> context;
        private Mock<IParticipantManager> participantManager;
        private Mock<IAnnotatableItemManager> annotatableItemManager;
        private Participant participant;
        private List<AnnotatableItem> annotatableItems;
        private Mock<IReplyManager> replyManager;
        private Mock<DbSet<Project>> projectDbSet;
        private Mock<DbSet<Comment>> commentDbSet;
        private Mock<IReviewTaskManager> reviewTaskManager;

        [SetUp]
        public void Setup()
        {
            this.context = new Mock<DatabaseContext>();
            this.context.CreateDbSetForContext(out this.commentDbSet, out this.projectDbSet);
            this.context.Setup(x => x.Set<Project>()).Returns(this.projectDbSet.Object);
            this.context.Setup(x => x.Set<Comment>()).Returns(this.commentDbSet.Object);
            this.participantManager = new Mock<IParticipantManager>();
            this.annotatableItemManager = new Mock<IAnnotatableItemManager>();
            this.reviewTaskManager = new Mock<IReviewTaskManager>();
            this.replyManager = new Mock<IReplyManager>();

            this.manager = new CommentManager(this.context.Object, this.participantManager.Object, this.replyManager.Object, this.reviewTaskManager.Object);
            this.manager.InjectManager(this.annotatableItemManager.Object);

            this.participant = new Participant(Guid.NewGuid())
            {
                Role = new Role(Guid.NewGuid()),
                User = new UserEntity(Guid.NewGuid())
            };

            this.annotatableItems = new List<AnnotatableItem>
            {
                new ReviewObjective(Guid.NewGuid())
            };

            Program.RegisterEntities();
        }

        [Test]
        public async Task VerifyGetComments()
        {
            var comments = new List<Comment>
            {
                new (Guid.NewGuid())
                {
                    Author = this.participant,
                    AnnotatableItems = this.annotatableItems, 
                    Replies = 
                    { 
                        new Reply(Guid.NewGuid())
                        {
                            Author = this.participant
                        },
                        new Reply(Guid.NewGuid())
                        {
                            Author = this.participant
                        }
                    }
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

            comments.First().AnnotatableItems.First().Annotations.Add(comments.First());

            this.commentDbSet.UpdateDbSetCollection(comments);

            foreach (var comment in comments)
            {
                this.commentDbSet.Setup(x => x.FindAsync(comment.Id)).ReturnsAsync(comment);
            }

            var getEntities = await this.manager.GetEntities(1);
            Assert.That(getEntities.ToList(), Has.Count.EqualTo(9));
            getEntities = await this.manager.GetEntity(Guid.NewGuid());
            Assert.That(getEntities, Is.Empty);
            getEntities = await this.manager.GetEntity(comments.First().Id, 1);
            Assert.That(getEntities.ToList(), Has.Count.EqualTo(7));

            var foundEntities = await this.manager.FindEntities(comments.Select(x => x.Id));
            Assert.That(foundEntities.ToList(), Has.Count.EqualTo(3));
        }

        [Test]
        public async Task VerifyCreateComment()
        {
            var comment = new Comment();
            var operationResult = await this.manager.CreateEntity(comment);
            Assert.That(operationResult.Succeeded, Is.False);

            comment.Author = this.participant;
            comment.Content = "A comment";
            comment.AnnotatableItems = this.annotatableItems;

            operationResult = await this.manager.CreateEntity(comment);
            Assert.That(operationResult.Succeeded, Is.False);

            var projects = new List<Project>()
            {
                new(Guid.NewGuid())
                {
                    Annotations =
                    {
                        comment
                    }
                }
            };

            this.projectDbSet.UpdateDbSetCollection(projects);
            await this.manager.CreateEntity(comment);

            this.context.Verify(x => x.Add(comment), Times.Once);
            this.context.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(new InvalidOperationException());
            operationResult = await this.manager.CreateEntity(comment);
            Assert.That(operationResult.Succeeded, Is.False);
        }

        [Test]
        public async Task VerifyUpdateComment()
        {
            var comment = new Comment(Guid.NewGuid());
            var operationResult = await this.manager.UpdateEntity(comment);
            Assert.That(operationResult.Succeeded, Is.False);

            comment.Author = this.participant;
            comment.AnnotatableItems = this.annotatableItems;

            operationResult = await this.manager.UpdateEntity(comment);
            Assert.That(operationResult.Succeeded, Is.False);

            comment.Content = "A comment";
            operationResult = await this.manager.UpdateEntity(comment);
            Assert.That(operationResult.Succeeded, Is.False);

            var projects = new List<Project>()
            {
                new(Guid.NewGuid())
                {
                    Annotations =
                    {
                        comment
                    }
                }
            };

            this.projectDbSet.UpdateDbSetCollection(projects);
            this.commentDbSet.UpdateDbSetCollection(new List<Comment>{comment});
            await this.manager.UpdateEntity(comment);
            this.context.Verify(x => x.Update(comment), Times.Once);
            this.context.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(new InvalidOperationException());
            operationResult = await this.manager.UpdateEntity(comment);
            Assert.That(operationResult.Succeeded, Is.False);
        }

        [Test]
        public async Task VerifyDeleteComment()
        {
            var comment = new Comment(Guid.NewGuid());
            var operationResult = await this.manager.DeleteEntity(comment);
            Assert.That(operationResult.Succeeded, Is.False);

            Assert.That(operationResult.Succeeded, Is.False);

            var projects = new List<Project>()
            {
                new(Guid.NewGuid())
                {
                    Annotations =
                    {
                        comment
                    }
                }
            };

            this.projectDbSet.UpdateDbSetCollection(projects);

            await this.manager.DeleteEntity(comment);
            this.context.Verify(x => x.Remove(comment), Times.Once);
            this.context.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(new InvalidOperationException());
            operationResult = await this.manager.DeleteEntity(comment);
            Assert.That(operationResult.Succeeded, Is.False);
        }

        [Test]
        public async Task VerifyResolveProperties()
        {
            var replyGuid = Guid.NewGuid();

            var commentDto = new CommentDto(Guid.NewGuid())
            {
                AnnotatableItems = this.annotatableItems.Select(x => x.Id).ToList(),
                Author = this.participant.Id,
                Content = "A content",
                CreatedOn = DateTime.UtcNow.Subtract(TimeSpan.FromDays(2)),
                Replies = new List<Guid>
                {
                    replyGuid
                }
            };

            this.participantManager.Setup(x => x.FindEntity(this.participant.Id)).ReturnsAsync(this.participant);
            this.annotatableItemManager.Setup(x => x.FindEntities(It.IsAny<List<Guid>>())).ReturnsAsync(this.annotatableItems);

            this.replyManager.Setup(x => x.FindEntities(commentDto.Replies)).ReturnsAsync(new List<Reply>
            {
                new(replyGuid)
                {
                    Author = this.participant
                }
            });

            var comment = (Comment)commentDto.InstantiatePoco();
            await this.manager.ResolveProperties(comment, new NoteDto(Guid.NewGuid()));

            Assert.Multiple(() =>
            {
                this.participantManager.Verify(x => x.FindEntity(this.participant.Id), Times.Never());
                this.annotatableItemManager.Verify(x => x.FindEntities(It.IsAny<List<Guid>>()), Times.Never());
                this.replyManager.Verify(x => x.FindEntities(commentDto.Replies), Times.Never());
            });

            await this.manager.ResolveProperties(comment, commentDto);

            Assert.Multiple(() =>
            {
                Assert.That(comment.AnnotatableItems, Is.Not.Empty);
                Assert.That(comment.Author, Is.Not.Null);
                Assert.That(comment.Replies, Is.Not.Empty);
            });
        }

        [Test]
        public async Task VerifyGetCommentsForAnnotatableItem()
        {
            var comments = new List<Comment>()
            {
                new(Guid.NewGuid())
                {
                    AnnotatableItems = this.annotatableItems
                },
                new(Guid.NewGuid()),
                new(Guid.NewGuid())
            };

            var project1 = new Project(Guid.NewGuid());
            project1.Annotations.Add(comments[0]);
            project1.Annotations.Add(comments[1]);

            var project2 = new Project(Guid.NewGuid());
            project2.Annotations.Add(comments[2]);

            this.commentDbSet.UpdateDbSetCollection(comments);
            var retrievedComments = await this.manager.GetCommentsOfAnnotatableItem(project1.Id, this.annotatableItems[0].Id);
            Assert.That(retrievedComments.OfType<Comment>().ToList(), Has.Count.EqualTo(1));
        }

        [Test]
        public async Task VerifyGetSearchResult()
        {
            var comment = new Comment(Guid.NewGuid())
            {
                CreatedInside = new ReviewTask()
                {
                    EntityContainer = new ReviewObjective()
                    {
                        EntityContainer = new Review()
                        {
                            EntityContainer = new Project()
                        }
                    }
                },
                Author = new Participant()
            };

            var result = await this.manager.GetSearchResult(comment.Id);
            Assert.That(result, Is.Null);
            
            this.commentDbSet.UpdateDbSetCollection(new List<Comment>{comment});
            result = await this.manager.GetSearchResult(comment.Id);
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task VerifyGetExtraEntitiesToUnindex()
        {
            var comment = new Comment(Guid.NewGuid())
            {
                Replies = { new Reply(Guid.NewGuid()) }
            };

            var result = await this.manager.GetExtraEntitiesToUnindex(comment.Id);
            Assert.That(result, Is.Empty);

            this.commentDbSet.UpdateDbSetCollection(new List<Comment> { comment });
            result = await this.manager.GetExtraEntitiesToUnindex(comment.Id);
            Assert.That(result, Is.Not.Empty);
        }
    }
}
