// --------------------------------------------------------------------------------------------------------
// <copyright file="ParticipantManagerTestFixture.cs" company="RHEA System S.A.">
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

    using NUnit.Framework;

    using UI_DSM.Server.Context;
    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Server.Managers.RoleManager;
    using UI_DSM.Server.Managers.UserManager;
    using UI_DSM.Server.Tests.Helpers;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    public class ParticipantManagerTestFixture
    {
        private ParticipantManager manager;
        private Mock<DatabaseContext> context;
        private Mock<IRoleManager> roleManager;
        private Mock<IUserManager> userManager;
        private Mock<DbSet<Participant>> participantDbSet;
        private Mock<DbSet<Project>> projectDbSet;

        [SetUp]
        public void Setup()
        {
            this.context = new Mock<DatabaseContext>();
            this.context.CreateDbSetForContext(out this.participantDbSet, out this.projectDbSet);
            this.roleManager = new Mock<IRoleManager>();
            this.userManager = new Mock<IUserManager>();
            this.manager = new ParticipantManager(this.context.Object, this.userManager.Object, this.roleManager.Object);
            Program.RegisterEntities();
        }

        [Test]
        public async Task VerifyGetEntities()
        {
            var user = new UserEntity(Guid.NewGuid())
            {
                UserName = "user"
            };

            var admin = new UserEntity(Guid.NewGuid())
            {
                UserName = "admin"
            };

            var data = new List<Participant>()
            {
                new(Guid.NewGuid())
                {
                    Role = new Role(Guid.NewGuid()),
                    User = admin
                },
                new(Guid.NewGuid())
                {
                    Role = new Role(Guid.NewGuid()),
                    User = user
                },
                new(Guid.NewGuid())
                {
                    Role = new Role(Guid.NewGuid()),
                    User = admin
                }
            };

            this.participantDbSet.UpdateDbSetCollection(data);

            var allEntities = await this.manager.GetEntities();
            Assert.That(allEntities.Count(), Is.EqualTo(8));

            var invalidGuid = Guid.NewGuid();
            var participant = await this.manager.FindEntity(invalidGuid);
            Assert.That(participant, Is.Null);

            var foundEntities = await this.manager.FindEntities(data.Select(x => x.Id));
            Assert.That(foundEntities.Count(), Is.EqualTo(3));

            var deepEntity = await this.manager.GetEntity(data.First().Id);
            Assert.That(deepEntity.Count(), Is.EqualTo(3));

            this.userManager.Setup(x => x.GetUserByName(user.UserName)).ReturnsAsync(user);
            this.userManager.Setup(x => x.GetUserByName(admin.UserName)).ReturnsAsync(admin);
            var nullParticipant = await this.manager.GetParticipants("aName");
            var adminParticipants = await this.manager.GetParticipants(admin.UserName);
            var userParticipants = await this.manager.GetParticipants(user.UserName);
            
            Assert.Multiple(() =>
            {
                Assert.That(nullParticipant, Is.Empty);
                Assert.That(adminParticipants.ToList(), Has.Count.EqualTo(2));
                Assert.That(userParticipants.ToList(), Has.Count.EqualTo(1));
            });
        }

        [Test]
        public async Task VerifyCreateEntity()
        {
            var participant = new Participant();

            var operationResult = await this.manager.CreateEntity(participant);
            Assert.That(operationResult.Succeeded, Is.False);

            participant.Role = new Role(Guid.NewGuid());
            participant.User = new UserEntity(Guid.NewGuid());
            operationResult = await this.manager.CreateEntity(participant);
            Assert.That(operationResult.Succeeded, Is.False);

            var projects = new List<Project>()
            {
                new(Guid.NewGuid())
                {
                    Participants =
                    {
                        new Participant(Guid.NewGuid())
                        {
                            User = participant.User
                        }
                    }
                }
            };

            this.projectDbSet.UpdateDbSetCollection(projects);

            participant.EntityContainer = new Project(Guid.NewGuid());
            operationResult = await this.manager.CreateEntity(participant);
            Assert.That(operationResult.Succeeded, Is.False);

            participant.EntityContainer = projects.First();
            operationResult = await this.manager.CreateEntity(participant);
            Assert.That(operationResult.Succeeded, Is.False);

            participant.User = new UserEntity(Guid.NewGuid());

            await this.manager.CreateEntity(participant);
            this.context.Verify(x => x.Add(participant), Times.Once);
            
            this.context.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(new InvalidOperationException());
            operationResult = await this.manager.CreateEntity(participant);
            Assert.That(operationResult.Succeeded, Is.False);
        }

        [Test]
        public async Task VerifyUpdate()
        {
            var participant = new Participant(Guid.NewGuid());
            var operationResult = await this.manager.UpdateEntity(participant);
            Assert.That(operationResult.Succeeded, Is.False);

            participant.User = new UserEntity(Guid.NewGuid());
            participant.Role = new Role(Guid.NewGuid());
            await this.manager.UpdateEntity(participant);
            Assert.That(operationResult.Succeeded, Is.False);

            var projects = new List<Project>()
            {
                new(Guid.NewGuid())
                {
                    Participants =
                    {
                        participant
                    }
                }
            };

            this.projectDbSet.UpdateDbSetCollection(projects);
            await this.manager.UpdateEntity(participant);

            this.context.Verify(x => x.Update(participant), Times.Once);

            this.context.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(new InvalidOperationException());
            operationResult = await this.manager.UpdateEntity(participant);
            Assert.That(operationResult.Succeeded, Is.False);
        }

        [Test]
        public async Task VerifyDelete()
        {
            var participant = new Participant();
            var operationResult = await this.manager.DeleteEntity(participant);
            Assert.That(operationResult.Succeeded, Is.False);

            var projects = new List<Project>()
            {
                new(Guid.NewGuid())
                {
                    Participants =
                    {
                        participant
                    }
                }
            };

            this.projectDbSet.UpdateDbSetCollection(projects); 

            await this.manager.DeleteEntity(participant);
            this.context.Verify(x => x.Remove(participant), Times.Once);

            this.context.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(new InvalidOperationException());
            operationResult = await this.manager.DeleteEntity(participant);
            Assert.That(operationResult.Succeeded, Is.False);
        }

        [Test]
        public async Task VerifyResolveProperties()
        {
            var role = new Role(Guid.NewGuid());
            var user = new UserEntity(Guid.NewGuid());
            this.userManager.Setup(x => x.FindEntity(user.Id)).ReturnsAsync(user);
            this.roleManager.Setup(x => x.FindEntity(role.Id)).ReturnsAsync(role);
            var participant = new Participant();
            await this.manager.ResolveProperties(participant, new RoleDto());
            Assert.That(participant.Role, Is.Null);

            var participantDto = new ParticipantDto()
            {
                Role = role.Id,
                User = user.Id
            };

            await this.manager.ResolveProperties(participant, participantDto);
           
            Assert.Multiple(() =>
            {
                Assert.That(participant.Role, Is.EqualTo(role));
                Assert.That(participant.User, Is.EqualTo(user));
            });
        }

        [Test]
        public async Task VerifyParticipantsOfProject()
        {
            var project1 = new Project(Guid.NewGuid())
            {
                Participants = 
                {
                    new Participant(Guid.NewGuid()),
                    new Participant(Guid.NewGuid())
                }
            };

            var project2 = new Project(Guid.NewGuid())
            {
                Participants =
                {
                    new Participant(Guid.NewGuid()),
                    new Participant(Guid.NewGuid())
                }
            };

            var allParticipants = new List<Participant>(project1.Participants);
            allParticipants.AddRange(project2.Participants);
            this.participantDbSet.UpdateDbSetCollection(allParticipants);
            var entities = await this.manager.GetContainedEntities(project1.Id);
            Assert.That(entities.Count(), Is.EqualTo(2));
            entities = await this.manager.GetContainedEntities(Guid.NewGuid());
            Assert.That(entities, Is.Empty);
        }

        [Test]
        public async Task VerifyGetAvailableUsersForParticipantCreation()
        {
            var users = new List<UserEntity>()
            {
                new (Guid.NewGuid())
                {
                    UserName = "user1"
                },
                new (Guid.NewGuid())
                {
                    UserName = "user2"
                },
                new (Guid.NewGuid())
                {
                    UserName = "user3"
                }
            };

            var participants = new List<Participant>()
            {
                new(Guid.NewGuid())
                {
                    User = users[2]
                }
            };

            var project = new Project(Guid.NewGuid())
            {
                Participants = { participants.First() }
            };

            this.participantDbSet.UpdateDbSetCollection(participants);

            this.userManager.Setup(x => x.GetUsers(It.IsAny<Func<UserEntity, bool>>()))
                .ReturnsAsync(users.Where(x => participants.All(p => p.User.Id != x.Id)));

            var availableUsers = (await this.manager.GetAvailableUsersForParticipantCreation(project.Id)).ToList();
            
            Assert.Multiple(() =>
            {
                Assert.That(availableUsers, Has.Count.EqualTo(2));
                Assert.That(availableUsers.All(x => x.Id != participants.First().User.Id), Is.True);
            });

            var participant = await this.manager.GetParticipantForProject(project.Id, "user3");
            Assert.That(participant, Is.EqualTo(participants.First()));
        }

        [Test]
        public async Task VerifyGetSearchResult()
        {
            var participant = new Participant(Guid.NewGuid())
            {
                EntityContainer = new Project(Guid.NewGuid())
            };

            var result = await this.manager.GetSearchResult(participant.Id);
            Assert.That(result, Is.Null);

            this.participantDbSet.UpdateDbSetCollection(new List<Participant> { participant });
            result = await this.manager.GetSearchResult(participant.Id);
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task VerifyGetExtraEntitiesToUnindex()
        {
            var participant = new Participant(Guid.NewGuid())
            {
                Annotations = new List<Annotation>
                {
                    new Comment(Guid.NewGuid())
                    {
                        Replies = { new Reply(Guid.NewGuid()) }
                    }
                }
            };

            var result = await this.manager.GetExtraEntitiesToUnindex(participant.Id);
            Assert.That(result, Is.Empty);

            this.participantDbSet.UpdateDbSetCollection(new List<Participant>{participant});

            result = await this.manager.GetExtraEntitiesToUnindex(participant.Id);
            Assert.That(result.ToList(), Has.Count.EqualTo(2));
        }
    }
}
