// --------------------------------------------------------------------------------------------------------
// <copyright file="ProjectManagerTestFixture.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft
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

    using Npgsql;

    using NUnit.Framework;

    using UI_DSM.Server.Context;
    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Server.Managers.ProjectManager;
    using UI_DSM.Server.Managers.ReviewManager;
    using UI_DSM.Server.Tests.Helpers;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class ProjectManagerTestFixture
    {
        private ProjectManager manager;
        private Mock<DatabaseContext> context;
        private Mock<IParticipantManager> participantManager;
        private Mock<IReviewManager> reviewManager;

        [SetUp]
        public void Setup()
        {
            this.context = new Mock<DatabaseContext>();
            this.participantManager = new Mock<IParticipantManager>();
            this.reviewManager = new Mock<IReviewManager>();
            this.manager = new ProjectManager(this.context.Object, this.participantManager.Object, this.reviewManager.Object);
        }

        [Test]
        public async Task VerifyGetProjects()
        {
            var data = new List<Project>
            {
                new(Guid.NewGuid()) { ProjectName = "P1" },
                new(Guid.NewGuid()) { ProjectName = "P2" },
                new(Guid.NewGuid()) { ProjectName = "P2" }
            };

            var dbSet = DbSetMockHelper.CreateMock(data);
            
            var invalidGuid = Guid.NewGuid();
            dbSet.Setup(x => x.FindAsync(invalidGuid)).ReturnsAsync((Project)null);
            dbSet.Setup(x => x.FindAsync(data.Last().Id)).ReturnsAsync(data.Last());
            this.context.Setup(x => x.Projects).Returns(dbSet.Object);
            
            Assert.Multiple(() =>
            {
                Assert.That(this.manager.GetEntities().Result.Count(), Is.EqualTo(data.Count));
                Assert.That(this.manager.GetEntity(invalidGuid).Result, Is.Empty);
                Assert.That(this.manager.GetEntity(data.Last().Id).Result, Is.Not.Null);
            });

            foreach (var project in data)
            {
                dbSet.Setup(x => x.FindAsync(project.Id)).ReturnsAsync(project);
            }

            var foundEntities = await this.manager.FindEntities(data.Select(x => x.Id));
            Assert.That(foundEntities.Count(), Is.EqualTo(3));
        }

        [Test]
        public async Task VerifyCreateProject()
        {
            var data = new List<Project>
            {
                new(Guid.NewGuid()) { ProjectName = "P1" },
                new(Guid.NewGuid()) { ProjectName = "P2" },
                new(Guid.NewGuid()) { ProjectName = "P2" }
            };

            var dbSet = DbSetMockHelper.CreateMock(data);

            this.context.Setup(x => x.Projects).Returns(dbSet.Object);

            var newProject = new Project()
            {
                ProjectName = "A new project"
            };

            await this.manager.CreateEntity(newProject);

            this.context.Verify(x => x.Add(It.IsAny<Project>()), Times.Exactly(1));

            this.context.Setup(x => x.SaveChangesAsync(default))
                .ThrowsAsync(new PostgresException("unique constraint", "High", "High", "23505"));

            var creationResult = this.manager.CreateEntity(newProject).Result;
            Assert.That(creationResult.Succeeded, Is.False);

            newProject.ProjectName = "P1";

            this.context.Setup(x => x.SaveChangesAsync(default))
                .ThrowsAsync(new InvalidOperationException());

            creationResult = this.manager.CreateEntity(newProject).Result;
            Assert.That(creationResult.Succeeded, Is.False);
        }

        [Test]
        public async Task VerifyUpdateProject()
        {
            var data = new List<Project>
            {
                new(Guid.NewGuid()) { ProjectName = "P1" },
                new(Guid.NewGuid()) { ProjectName = "P2" },
                new(Guid.NewGuid()) { ProjectName = "P3" }
            };

            var dbSet = DbSetMockHelper.CreateMock(data);

            this.context.Setup(x => x.Projects).Returns(dbSet.Object);

            var project = new Project(data.First().Id)
            {
                ProjectName = "New Name"
            };

            await this.manager.UpdateEntity(project);
            this.context.Verify(x => x.Update(It.IsAny<Project>()), Times.Once);

            project.ProjectName = "P2";

            this.context.Setup(x => x.SaveChangesAsync(default))
                .ThrowsAsync(new PostgresException("unique constraint", "High", "High", "23505"));

            var updateResult = await this.manager.UpdateEntity(project);

            this.context.Verify(x => x.Update(It.IsAny<Project>()), Times.Exactly(2));
            Assert.That(updateResult.Errors.First(), Does.Contain("already used"));

            this.context.Setup(x => x.SaveChangesAsync(default))
                .ThrowsAsync(new InvalidOperationException());

            var creationResult = this.manager.UpdateEntity(project).Result;
            Assert.That(creationResult.Succeeded, Is.False);
        }

        [Test]
        public void VerifyDeleteProject()
        {
            var project = new Project();
            _ = this.manager.DeleteEntity(project).Result;

            this.context.Verify(x => x.Remove(It.IsAny<Project>()), Times.Once);

            this.context.Setup(x => x.SaveChangesAsync(default))
                .ThrowsAsync(new InvalidOperationException());

            var creationResult = this.manager.DeleteEntity(project).Result;
            Assert.That(creationResult.Succeeded, Is.False);
        }

        [Test]
        public async Task GetAvailableProjectsForUser()
        {
            var user = new UserEntity()
            {
                UserName = "user"
            };

            var admin = new UserEntity()
            {
                UserName = "admin"
            };

            var participants = new List<Participant>()
            {
                new (Guid.NewGuid())
                {
                    User = user
                },
                new (Guid.NewGuid())
                {
                    User = admin
                },
                new (Guid.NewGuid())
                {
                    User = admin
                }
            };

            _ = new List<Project>()
            {
                new (Guid.NewGuid())
                {
                    Participants = { participants[0], participants[1] }
                },
                new (Guid.NewGuid())
                {
                    Participants = { participants[2] }
                },
            };

            this.participantManager.Setup(x => x.GetParticipants(user.UserName))
                .ReturnsAsync(participants.Where(x => x.User.UserName == user.UserName));

            this.participantManager.Setup(x => x.GetParticipants(admin.UserName))
                .ReturnsAsync(participants.Where(x => x.User.UserName == admin.UserName));

            var projectForUser = await this.manager.GetAvailableProjectsForUser(user.UserName);
            var projectForAdmin = await this.manager.GetAvailableProjectsForUser(admin.UserName);

            Assert.Multiple(() =>
            {
                Assert.That(projectForAdmin.ToList(), Has.Count.EqualTo(2));
                Assert.That(projectForUser.ToList(), Has.Count.EqualTo(1));
            });
        }
    }
}
