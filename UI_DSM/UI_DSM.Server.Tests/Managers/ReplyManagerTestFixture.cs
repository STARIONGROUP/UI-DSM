// --------------------------------------------------------------------------------------------------------
// <copyright file="ReplyManagerTestFixture.cs" company="RHEA System S.A.">
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
    using UI_DSM.Server.Managers.ReplyManager;
    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Server.Tests.Helpers;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;
    
    using Microsoft.EntityFrameworkCore;

    [TestFixture]
    public class ReplyManagerTestFixture
    {
        private ReplyManager manager;
        private Mock<DatabaseContext> context;
        private Mock<IParticipantManager> participantManager;
        private Participant participant;
        private Mock<DbSet<Reply>> replyDbSet;
        private Mock<DbSet<Comment>> commentDbSet;

        [SetUp]
        public void Setup()
        {
            this.context = new Mock<DatabaseContext>();
            this.context.CreateDbSetForContext(out this.replyDbSet, out this.commentDbSet);
            this.participantManager = new Mock<IParticipantManager>();
            this.manager = new ReplyManager(this.context.Object, this.participantManager.Object);

            this.participant = new Participant(Guid.NewGuid())
            {
                Role = new Role(Guid.NewGuid()),
                User = new UserEntity(Guid.NewGuid())
            };
            
            Program.RegisterEntities();
        }

        [Test]
        public async Task VerifyGetReplys()
        {
            var replies = new List<Reply>
            {
                new (Guid.NewGuid())
                {
                    Author = this.participant,
                },
                new (Guid.NewGuid())
                {
                    Author = this.participant,
                    EntityContainer = new Comment(Guid.NewGuid())
                },
                new (Guid.NewGuid())
                {
                    Author = this.participant,
                }
            };

            var comment = new Comment(Guid.NewGuid())
            {
                Replies = { replies.First(), replies.Last() }
            };

            this.replyDbSet.UpdateDbSetCollection(replies);

            var getEntities = await this.manager.GetEntities();
            Assert.That(getEntities.ToList(), Has.Count.EqualTo(6));
            getEntities = await this.manager.GetEntity(Guid.NewGuid());
            Assert.That(getEntities, Is.Empty);
            getEntities = await this.manager.GetEntity(replies.First().Id);
            Assert.That(getEntities.ToList(), Has.Count.EqualTo(4));

            var foundEntities = await this.manager.FindEntities(replies.Select(x => x.Id));
            Assert.That(foundEntities.ToList(), Has.Count.EqualTo(3));

            var containedEntities = await this.manager.GetContainedEntities(comment.Id);
            Assert.That(containedEntities.ToList(), Has.Count.EqualTo(5));
        }

        [Test]
        public async Task VerifyCreateReply()
        {
            var reply = new Reply();
            var operationResult = await this.manager.CreateEntity(reply);
            Assert.That(operationResult.Succeeded, Is.False);

            reply.Author = this.participant;
            reply.Content = "A reply";

            operationResult= await this.manager.CreateEntity(reply);
            Assert.That(operationResult.Succeeded, Is.False);

            var comment = new Comment(Guid.NewGuid())
            {
                Replies = { reply }
            };

            this.commentDbSet.UpdateDbSetCollection(new List<Comment>{comment});

            await this.manager.CreateEntity(reply);
            this.context.Verify(x => x.Add(reply), Times.Once);
            this.context.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(new InvalidOperationException());
            operationResult = await this.manager.CreateEntity(reply);
            Assert.That(operationResult.Succeeded, Is.False);
        }

        [Test]
        public async Task VerifyUpdateReply()
        {
            var reply = new Reply(Guid.NewGuid());
            var operationResult = await this.manager.UpdateEntity(reply);
            Assert.That(operationResult.Succeeded, Is.False);

            reply.Author = this.participant;
            reply.Content = "A reply";

            var comment = new Comment(Guid.NewGuid())
            {
                Replies = { reply }
            };

            this.commentDbSet.UpdateDbSetCollection(new List<Comment>{comment});

            await this.manager.UpdateEntity(reply);
            this.context.Verify(x => x.Update(reply), Times.Once);
            this.context.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(new InvalidOperationException());
            operationResult = await this.manager.UpdateEntity(reply);
            Assert.That(operationResult.Succeeded, Is.False);
        }

        [Test]
        public async Task VerifyDeleteReply()
        {
            var reply = new Reply(Guid.NewGuid());

            var operationResult = await this.manager.DeleteEntity(reply);
            Assert.That(operationResult.Succeeded, Is.False);

            var comment = new Comment(Guid.NewGuid())
            {
                Replies = { reply }
            };

            this.commentDbSet.UpdateDbSetCollection(new List<Comment> { comment });
            await this.manager.DeleteEntity(reply);

            this.context.Verify(x => x.Remove(reply), Times.Once);
            this.context.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(new InvalidOperationException());
            operationResult = await this.manager.DeleteEntity(reply);
            Assert.That(operationResult.Succeeded, Is.False);
        }

        [Test]
        public async Task VerifyResolveProperties()
        {
            var replyDto = new ReplyDto(Guid.NewGuid())
            {
                Author = this.participant.Id,
                Content = "A content",
                CreatedOn = DateTime.UtcNow.Subtract(TimeSpan.FromDays(2))
            };

            this.participantManager.Setup(x => x.FindEntity(this.participant.Id)).ReturnsAsync(this.participant);
            var reply = (Reply)replyDto.InstantiatePoco();
            await this.manager.ResolveProperties(reply, new CommentDto(Guid.NewGuid()));

                this.participantManager.Verify(x => x.FindEntity(this.participant.Id), Times.Never());

            await this.manager.ResolveProperties(reply, replyDto);

                Assert.That(reply.Author, Is.Not.Null);
        }
    }
}
