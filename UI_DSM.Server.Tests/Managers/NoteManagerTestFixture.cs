// --------------------------------------------------------------------------------------------------------
// <copyright file="NoteManagerTestFixture.cs" company="RHEA System S.A.">
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
    using UI_DSM.Server.Managers.NoteManager;
    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Server.Tests.Helpers;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class NoteManagerTestFixture
    {
        private NoteManager manager;
        private Mock<DatabaseContext> context;
        private Mock<IParticipantManager> participantManager;
        private Mock<IAnnotatableItemManager> annotatableItemManager;
        private Participant participant;
        private List<AnnotatableItem> annotatableItems;
        private Mock<DbSet<Note>> noteDbSet;
        private Mock<DbSet<Project>> projectDbSet;

        [SetUp]
        public void Setup()
        {
            this.context = new Mock<DatabaseContext>();
            this.context.CreateDbSetForContext(out this.noteDbSet, out this.projectDbSet);
            this.participantManager = new Mock<IParticipantManager>();
            this.annotatableItemManager = new Mock<IAnnotatableItemManager>();
            this.manager = new NoteManager(this.context.Object, this.participantManager.Object);
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
        public async Task VerifyGetNotes()
        {
            var notes = new List<Note>
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

            this.noteDbSet.UpdateDbSetCollection(notes);

            var getEntities = await this.manager.GetEntities();
            Assert.That(getEntities.ToList(), Has.Count.EqualTo(7));
            getEntities = await this.manager.GetEntity(Guid.NewGuid());
            Assert.That(getEntities, Is.Empty);
            getEntities = await this.manager.GetEntity(notes.First().Id);
            Assert.That(getEntities.ToList(), Has.Count.EqualTo(5));

            var foundEntities = await this.manager.FindEntities(notes.Select(x => x.Id));
            Assert.That(foundEntities.ToList(), Has.Count.EqualTo(3));
        }

        [Test]
        public async Task VerifyCreateNote()
        {
            var note = new Note();
            var operationResult = await this.manager.CreateEntity(note);
            Assert.That(operationResult.Succeeded, Is.False);

            note.Author = this.participant;
            note.Content = "A note";
            note.AnnotatableItems = this.annotatableItems;

            operationResult = await this.manager.CreateEntity(note);
            Assert.That(operationResult.Succeeded, Is.False);

            var projects = new List<Project>()
            {
                new(Guid.NewGuid())
                {
                    Annotations =
                    {
                        note
                    }
                }
            };

            this.projectDbSet.UpdateDbSetCollection(projects);

            await this.manager.CreateEntity(note);
            this.context.Verify(x => x.Add(note), Times.Once);
            this.context.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(new InvalidOperationException());
            operationResult = await this.manager.CreateEntity(note);
            Assert.That(operationResult.Succeeded, Is.False);
        }

        [Test]
        public async Task VerifyUpdateNote()
        {
            var note = new Note(Guid.NewGuid());
            var operationResult = await this.manager.UpdateEntity(note);
            Assert.That(operationResult.Succeeded, Is.False);

            note.Author = this.participant;
            note.Content = "A note";
            note.AnnotatableItems = this.annotatableItems;

            operationResult = await this.manager.CreateEntity(note);
            Assert.That(operationResult.Succeeded, Is.False);

            var projects = new List<Project>()
            {
                new(Guid.NewGuid())
                {
                    Annotations =
                    {
                        note
                    }
                }
            };

            this.projectDbSet.UpdateDbSetCollection(projects);

            await this.manager.UpdateEntity(note);
            this.context.Verify(x => x.Update(note), Times.Once);
            this.context.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(new InvalidOperationException());
            operationResult = await this.manager.UpdateEntity(note);
            Assert.That(operationResult.Succeeded, Is.False);
        }

        [Test]
        public async Task VerifyDeleteNote()
        {
            var note = new Note(Guid.NewGuid());

            var operationResult = await this.manager.CreateEntity(note);
            Assert.That(operationResult.Succeeded, Is.False);

            var projects = new List<Project>()
            {
                new(Guid.NewGuid())
                {
                    Annotations =
                    {
                        note
                    }
                }
            };

            this.projectDbSet.UpdateDbSetCollection(projects);

            await this.manager.DeleteEntity(note);
            this.context.Verify(x => x.Remove(note), Times.Once);
            this.context.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(new InvalidOperationException());
            operationResult = await this.manager.DeleteEntity(note);
            Assert.That(operationResult.Succeeded, Is.False);
        }

        [Test]
        public async Task VerifyResolveProperties()
        {
            var noteDto = new NoteDto(Guid.NewGuid())
            {
                AnnotatableItems = this.annotatableItems.Select(x => x.Id).ToList(),
                Author = this.participant.Id,
                Content = "A content",
                CreatedOn = DateTime.UtcNow.Subtract(TimeSpan.FromDays(2))
            };

            this.participantManager.Setup(x => x.FindEntity(this.participant.Id)).ReturnsAsync(this.participant);
            this.annotatableItemManager.Setup(x => x.FindEntities(It.IsAny<List<Guid>>())).ReturnsAsync(this.annotatableItems);
            var note = (Note)noteDto.InstantiatePoco();
            await this.manager.ResolveProperties(note, new CommentDto(Guid.NewGuid()));

            Assert.Multiple(() =>
            {
                this.participantManager.Verify(x => x.FindEntity(this.participant.Id), Times.Never());
                this.annotatableItemManager.Verify(x => x.FindEntities(It.IsAny<List<Guid>>()), Times.Never());
            });

            await this.manager.ResolveProperties(note, noteDto);

            Assert.Multiple(() =>
            {
                Assert.That(note.AnnotatableItems, Is.Not.Empty);
                Assert.That(note.Author, Is.Not.Null);
            });
        }

        [Test]
        public async Task VerifyGetSearchResult()
        {
            var note = new Note(Guid.NewGuid())
            {
                EntityContainer = new Project(Guid.NewGuid())
            };

            var result = await this.manager.GetSearchResult(note.Id);
            Assert.That(result, Is.Null);

            this.noteDbSet.UpdateDbSetCollection(new List<Note> { note });
            result = await this.manager.GetSearchResult(note.Id);
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task VerifyGetExtraEntitiesToUnindex()
        {
            var result = await this.manager.GetExtraEntitiesToUnindex(Guid.NewGuid());
            Assert.That(result, Is.Empty);
        }
    }
}
