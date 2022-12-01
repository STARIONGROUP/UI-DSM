// --------------------------------------------------------------------------------------------------------
// <copyright file="AnnotationManagerTestFixture.cs" company="RHEA System S.A.">
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
    using System.Diagnostics.CodeAnalysis;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Server.Managers.AnnotationManager;
    using UI_DSM.Server.Managers.CommentManager;
    using UI_DSM.Server.Managers.FeedbackManager;
    using UI_DSM.Server.Managers.NoteManager;
    using UI_DSM.Server.Types;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class AnnotationManagerTestFixture
    {
        private AnnotationManager manager;
        private Mock<ICommentManager> commentManager;
        private Mock<IFeedbackManager> feedbackManager;
        private Mock<INoteManager> noteManager;
        private Participant participant;
        private List<AnnotatableItem> annotatableItems;

        [SetUp]
        public void Setup()
        {
            this.participant = new Participant(Guid.NewGuid())
            {
                Role = new Role(Guid.NewGuid()),
                User = new UserEntity(Guid.NewGuid())
            };

            this.annotatableItems = new List<AnnotatableItem>()
            {
                new ReviewObjective(Guid.NewGuid())
                {
                    Author = this.participant
                }
            };

            this.commentManager = new Mock<ICommentManager>();
            this.feedbackManager = new Mock<IFeedbackManager>();
            this.noteManager = new Mock<INoteManager>();
            this.manager = new AnnotationManager(this.commentManager.Object, this.feedbackManager.Object, this.noteManager.Object);
            Program.RegisterEntities();
        }

        [Test]
        public async Task VerifyGetEntities()
        {
            var comments = new List<Comment>
            {
                new (Guid.NewGuid())
                {
                    AnnotatableItems = this.annotatableItems,
                    Author = this.participant
                }
            };

            var feedbacks = new List<Feedback>
            {
                new (Guid.NewGuid())
                {
                    AnnotatableItems = this.annotatableItems,
                    Author = this.participant
                }
            };

            var notes = new List<Note>
            {
                new(Guid.NewGuid())
                {
                    AnnotatableItems = this.annotatableItems,
                    Author = this.participant
                }
            };

            this.commentManager.Setup(x => x.GetEntities(0)).ReturnsAsync(comments.SelectMany(x => x.GetAssociatedEntities()));
            this.feedbackManager.Setup(x => x.GetEntities(0)).ReturnsAsync(feedbacks.SelectMany(x => x.GetAssociatedEntities()));
            this.noteManager.Setup(x => x.GetEntities(0)).ReturnsAsync(notes.SelectMany(x => x.GetAssociatedEntities()));

            var getEntitiesResult = await this.manager.GetEntities();
            Assert.That(getEntitiesResult.ToList(), Has.Count.EqualTo(7));

            foreach (var comment in comments)
            {
                this.commentManager.Setup(x => x.FindEntity(comment.Id)).ReturnsAsync(comment);
                this.commentManager.Setup(x => x.GetEntity(comment.Id,0)).ReturnsAsync(comment.GetAssociatedEntities());
            }

            foreach (var feedback in feedbacks)
            {
                this.feedbackManager.Setup(x => x.FindEntity(feedback.Id)).ReturnsAsync(feedback);
                this.feedbackManager.Setup(x => x.GetEntity(feedback.Id, 0)).ReturnsAsync(feedback.GetAssociatedEntities());
            }

            foreach (var note in notes)
            {
                this.noteManager.Setup(x => x.FindEntity(note.Id)).ReturnsAsync(note);
                this.noteManager.Setup(x => x.GetEntity(note.Id, 0)).ReturnsAsync(note.GetAssociatedEntities());
            }

            getEntitiesResult = await this.manager.GetEntity(comments.First().Id);
            Assert.That(getEntitiesResult, Is.Not.Empty);

            getEntitiesResult = await this.manager.GetEntity(feedbacks.First().Id);
            Assert.That(getEntitiesResult, Is.Not.Empty);

            getEntitiesResult = await this.manager.GetEntity(notes.First().Id);
            Assert.That(getEntitiesResult, Is.Not.Empty);

            getEntitiesResult = await this.manager.GetEntity(Guid.NewGuid());
            Assert.That(getEntitiesResult, Is.Empty);

            var allEntities = new List<Guid>();
            allEntities.AddRange(comments.Select(x=> x.Id));
            allEntities.AddRange(feedbacks.Select(x=> x.Id));
            allEntities.AddRange(notes.Select(x=> x.Id));
            allEntities.Add(Guid.NewGuid());

            var findEntitiesResult =await this.manager.FindEntities(allEntities);
            Assert.That(findEntitiesResult.ToList(), Has.Count.EqualTo(3));
        }

        [Test]
        public async Task VerifyCreateEntity()
        {
            var comment = new Comment();
            var feedback = new Feedback();
            var note = new Note();

            this.commentManager.Setup(x => x.CreateEntity(comment)).ReturnsAsync(EntityOperationResult<Comment>.Failed());
            this.feedbackManager.Setup(x => x.CreateEntity(feedback)).ReturnsAsync(EntityOperationResult<Feedback>.Failed());
            this.noteManager.Setup(x => x.CreateEntity(note)).ReturnsAsync(EntityOperationResult<Note>.Failed());

            var creationResult = await this.manager.CreateEntity(comment);
            Assert.That(creationResult.Succeeded, Is.False);

            creationResult = await this.manager.CreateEntity(feedback);
            Assert.That(creationResult.Succeeded, Is.False);

            creationResult = await this.manager.CreateEntity(note);
            Assert.That(creationResult.Succeeded, Is.False);

            this.commentManager.Setup(x => x.CreateEntity(comment)).ReturnsAsync(EntityOperationResult<Comment>.Success(comment));
            this.feedbackManager.Setup(x => x.CreateEntity(feedback)).ReturnsAsync(EntityOperationResult<Feedback>.Success(feedback));
            this.noteManager.Setup(x => x.CreateEntity(note)).ReturnsAsync(EntityOperationResult<Note>.Success(note));

            creationResult = await this.manager.CreateEntity(comment);
            Assert.That(creationResult.Succeeded, Is.True);

            creationResult = await this.manager.CreateEntity(feedback);
            Assert.That(creationResult.Succeeded, Is.True);

            creationResult = await this.manager.CreateEntity(note);
            Assert.That(creationResult.Succeeded, Is.True);

            creationResult = await this.manager.CreateEntity(new TestAnnotation());
            Assert.That(creationResult.Succeeded, Is.False);
        }

        [Test]
        public async Task VerifyUpdateEntity()
        {
            var comment = new Comment(Guid.NewGuid());
            var feedback = new Feedback(Guid.NewGuid());
            var note = new Note(Guid.NewGuid());

            this.commentManager.Setup(x => x.UpdateEntity(comment)).ReturnsAsync(EntityOperationResult<Comment>.Failed());
            this.feedbackManager.Setup(x => x.UpdateEntity(feedback)).ReturnsAsync(EntityOperationResult<Feedback>.Failed());
            this.noteManager.Setup(x => x.UpdateEntity(note)).ReturnsAsync(EntityOperationResult<Note>.Failed());

            var creationResult = await this.manager.UpdateEntity(comment);
            Assert.That(creationResult.Succeeded, Is.False);

            creationResult = await this.manager.UpdateEntity(feedback);
            Assert.That(creationResult.Succeeded, Is.False);

            creationResult = await this.manager.UpdateEntity(note);
            Assert.That(creationResult.Succeeded, Is.False);

            this.commentManager.Setup(x => x.UpdateEntity(comment)).ReturnsAsync(EntityOperationResult<Comment>.Success(comment));
            this.feedbackManager.Setup(x => x.UpdateEntity(feedback)).ReturnsAsync(EntityOperationResult<Feedback>.Success(feedback));
            this.noteManager.Setup(x => x.UpdateEntity(note)).ReturnsAsync(EntityOperationResult<Note>.Success(note));

            creationResult = await this.manager.UpdateEntity(comment);
            Assert.That(creationResult.Succeeded, Is.True);

            creationResult = await this.manager.UpdateEntity(feedback);
            Assert.That(creationResult.Succeeded, Is.True);

            creationResult = await this.manager.UpdateEntity(note);
            Assert.That(creationResult.Succeeded, Is.True);

            creationResult = await this.manager.UpdateEntity(new TestAnnotation());
            Assert.That(creationResult.Succeeded, Is.False);
        }

        [Test]
        public async Task VerifyDeleteEntity()
        {
            var comment = new Comment(Guid.NewGuid());
            var feedback = new Feedback(Guid.NewGuid());
            var note = new Note(Guid.NewGuid());

            this.commentManager.Setup(x => x.DeleteEntity(comment)).ReturnsAsync(EntityOperationResult<Comment>.Failed());
            this.feedbackManager.Setup(x => x.DeleteEntity(feedback)).ReturnsAsync(EntityOperationResult<Feedback>.Failed());
            this.noteManager.Setup(x => x.DeleteEntity(note)).ReturnsAsync(EntityOperationResult<Note>.Failed());

            var creationResult = await this.manager.DeleteEntity(comment);
            Assert.That(creationResult.Succeeded, Is.False);

            creationResult = await this.manager.DeleteEntity(feedback);
            Assert.That(creationResult.Succeeded, Is.False);

            creationResult = await this.manager.DeleteEntity(note);
            Assert.That(creationResult.Succeeded, Is.False);

            this.commentManager.Setup(x => x.DeleteEntity(comment)).ReturnsAsync(EntityOperationResult<Comment>.Success(comment));
            this.feedbackManager.Setup(x => x.DeleteEntity(feedback)).ReturnsAsync(EntityOperationResult<Feedback>.Success(feedback));
            this.noteManager.Setup(x => x.DeleteEntity(note)).ReturnsAsync(EntityOperationResult<Note>.Success(note));

            creationResult = await this.manager.DeleteEntity(comment);
            Assert.That(creationResult.Succeeded, Is.True);

            creationResult = await this.manager.DeleteEntity(feedback);
            Assert.That(creationResult.Succeeded, Is.True);

            creationResult = await this.manager.DeleteEntity(note);
            Assert.That(creationResult.Succeeded, Is.True);

            creationResult = await this.manager.DeleteEntity(new TestAnnotation());
            Assert.That(creationResult.Succeeded, Is.False);
        }

        [Test]
        public async Task VerifyResolveProperties()
        {
            var comment = new Comment(Guid.NewGuid())
            {
                AnnotatableItems = this.annotatableItems,
                Author = this.participant
            };
            
            var feedback = new Feedback(Guid.NewGuid())
            {
                AnnotatableItems = this.annotatableItems,
                Author = this.participant
            };

            var note = new Note(Guid.NewGuid())
            {
                AnnotatableItems = this.annotatableItems,
                Author = this.participant
            };

            var commentDto = comment.ToDto();
            var feedbackDto = feedback.ToDto();
            var noteDto = note.ToDto();

            this.commentManager.Setup(x => x.ResolveProperties(comment, commentDto));
            this.feedbackManager.Setup(x => x.ResolveProperties(feedback, feedbackDto));
            this.noteManager.Setup(x => x.ResolveProperties(note, noteDto));

            await this.manager.ResolveProperties(comment, commentDto);
            await this.manager.ResolveProperties(feedback, feedbackDto);
            await this.manager.ResolveProperties(note, noteDto);

            Assert.Multiple(() =>
            {
                this.commentManager.Verify(x => x.ResolveProperties(comment, commentDto), Times.Once);
                this.feedbackManager.Verify(x => x.ResolveProperties(feedback, feedbackDto), Times.Once);
                this.noteManager.Verify(x => x.ResolveProperties(note, noteDto), Times.Once);
            });
        }

        [Test]
        public async Task VerifyGetAnnotationOfAnnotatableItem()
        {
            var projectId = Guid.NewGuid();
            var annotatableItemId = Guid.NewGuid();
            await this.manager.GetAnnotationsOfAnnotatableItem(projectId, annotatableItemId);
            this.commentManager.Verify(x => x.GetCommentsOfAnnotatableItem(projectId, annotatableItemId), Times.Once);
        }
    }

    [ExcludeFromCodeCoverage]
    internal class TestAnnotation : Annotation
    {
        /// <summary>
        ///     Instantiate a <see cref="EntityDto" /> from a <see cref="Entity" />
        /// </summary>
        /// <returns>A new <see cref="EntityDto" /></returns>
        public override EntityDto ToDto()
        {
            return null;
        }
    }
}
