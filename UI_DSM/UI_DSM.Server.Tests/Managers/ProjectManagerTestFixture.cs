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
    using Microsoft.EntityFrameworkCore;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    using UI_DSM.Server.Context;
    using UI_DSM.Server.Managers.AnnotationManager;
    using UI_DSM.Server.Managers.ArtifactManager;
    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Server.Managers.ProjectManager;
    using UI_DSM.Server.Managers.ReviewManager;
    using UI_DSM.Server.Tests.Helpers;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class ProjectManagerTestFixture
    {
        private ProjectManager manager;
        private Mock<DatabaseContext> context;
        private Mock<IParticipantManager> participantManager;
        private Mock<IReviewManager> reviewManager;
        private Mock<IAnnotationManager> annotationManager;
        private Mock<IArtifactManager> artifactManager;
        private Mock<DbSet<Project>> projectDbSet;

        [SetUp]
        public void Setup()
        {
            this.context = new Mock<DatabaseContext>();
            this.context.CreateDbSetForContext(out this.projectDbSet);
            this.participantManager = new Mock<IParticipantManager>();
            this.reviewManager = new Mock<IReviewManager>();
            this.annotationManager = new Mock<IAnnotationManager>();
            this.artifactManager = new Mock<IArtifactManager>();
            
            this.manager = new ProjectManager(this.context.Object, this.participantManager.Object, this.reviewManager.Object,
                this.annotationManager.Object, this.artifactManager.Object);
            
            Program.RegisterEntities();
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

            this.projectDbSet.UpdateDbSetCollection(data);
            
            var invalidGuid = Guid.NewGuid();
            
            Assert.Multiple(() =>
            {
                Assert.That(this.manager.GetEntities().Result.Count(), Is.EqualTo(data.Count));
                Assert.That(this.manager.GetEntity(invalidGuid).Result, Is.Empty);
                Assert.That(this.manager.GetEntity(data.Last().Id).Result, Is.Not.Null);
            });

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

            this.projectDbSet.UpdateDbSetCollection(data);

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

            this.projectDbSet.UpdateDbSetCollection(data);

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
            Assert.That(updateResult.Errors.First(), Does.Contain("already exists"));

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

        [Test]
        public async Task VerifyGetOpenTasksAndComments()
        {
            var participant = new Participant(Guid.NewGuid())
            {
                User = new UserEntity()
                {
                    UserName = "user"
                }
            };

            var participant2 = new Participant(Guid.NewGuid())
            {
                User = new UserEntity()
                {
                    UserName = "user2"
                }
            };

            var projects = new List<Project>();
            var project = new Project(Guid.NewGuid());
            project.Reviews.AddRange(CreateEntity<Review>(2));
            project.Reviews[0].ReviewObjectives.AddRange(CreateEntity<ReviewObjective>(3));
            project.Reviews[1].ReviewObjectives.AddRange(CreateEntity<ReviewObjective>(5));

            this.participantManager.Setup(x => x.GetParticipantForProject(project.Id, participant.User.UserName))
                .ReturnsAsync(participant);

            this.participantManager.Setup(x => x.GetParticipantForProject(project.Id, participant2.User.UserName))
                .ReturnsAsync(participant2);

            foreach (var reviewReviewObjective in project.Reviews.SelectMany(review => review.ReviewObjectives))
            {
                reviewReviewObjective.ReviewTasks.AddRange(CreateEntity<ReviewTask>(4));

                reviewReviewObjective.ReviewTasks[0].IsAssignedTo = participant;
                reviewReviewObjective.ReviewTasks[1].IsAssignedTo = participant2;
                reviewReviewObjective.ReviewTasks[2].IsAssignedTo = participant2;
            }
            
            project.Annotations.AddRange(CreateEntity<Comment>(8));

            projects.Add(project);

            this.projectDbSet.UpdateDbSetCollection(projects);

            var guids = projects.Select(x => x.Id).ToList();
            guids.Add(Guid.NewGuid());
            var computedProjectProperties =await this.manager.GetOpenTasksAndComments(guids, participant.User.UserName);

            var expectedComputed = new ComputedProjectProperties
            {
                    CommentCount = 8,
                    TaskCount = 8
            };

            Assert.Multiple(() =>
            {
                Assert.That(computedProjectProperties[project.Id], Is.EqualTo(expectedComputed));
                Assert.That(computedProjectProperties.Keys, Has.Count.EqualTo(1));
            });

            computedProjectProperties = await this.manager.GetOpenTasksAndComments(guids, participant2.User.UserName);

            expectedComputed = new ComputedProjectProperties
            {
                CommentCount = 8,
                TaskCount = 16
            };

            Assert.That(computedProjectProperties[project.Id], Is.EqualTo(expectedComputed));

            computedProjectProperties = await this.manager.GetOpenTasksAndComments(guids, "anotherUser");

            Assert.That(computedProjectProperties.Keys, Has.Count.EqualTo(0));
        }

        private static IEnumerable<TEntity> CreateEntity<TEntity>(int amountOfEntities) where TEntity : Entity, new()
        {
            var entities = new List<TEntity>();

            for (var entityCount = 0; entityCount < amountOfEntities; entityCount++)
            {
                entities.Add(new TEntity()
                {
                    Id = Guid.NewGuid()
                });
            }

            return entities;
        }
    }
}
