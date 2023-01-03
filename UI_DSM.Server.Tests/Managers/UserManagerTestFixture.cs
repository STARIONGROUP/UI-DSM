// --------------------------------------------------------------------------------------------------------
// <copyright file="UserManagerTestFixture.cs" company="RHEA System S.A.">
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
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Server.Context;
    using UI_DSM.Server.Managers.UserManager;
    using UI_DSM.Server.Tests.Helpers;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    public class UserManagerTestFixture
    {
        private UserManager manager;
        private Mock<UserManager<User>> authenticationManager;
        private Mock<DatabaseContext> context;
        private Mock<DbSet<UserEntity>> userDbSet;

        [SetUp]
        public void Setup()
        {
            this.authenticationManager = new Mock<UserManager<User>>(new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);
            this.context = new Mock<DatabaseContext>();
            this.context.CreateDbSetForContext(out this.userDbSet);
            this.manager = new UserManager(this.context.Object, this.authenticationManager.Object);
            Program.RegisterEntities();
        }

        [Test]
        public async Task VerifyGetEntities()
        {
            var users = new List<UserEntity>()
            {
                new (Guid.NewGuid())
                {
                    UserName = "admin",
                    IsAdmin = true
                },
                new (Guid.NewGuid())
                {
                    UserName = "user"
                }
            };

            this.userDbSet.UpdateDbSetCollection(users);
            var getEntitiesResult = await this.manager.GetEntities();
            Assert.That(getEntitiesResult.Count(), Is.EqualTo(2));

            var getEntityResult = await this.manager.GetEntity(Guid.NewGuid());
            Assert.That(getEntityResult, Is.Empty);
            getEntityResult = await this.manager.GetEntity(users.First().Id);
            Assert.That(getEntityResult, Is.Not.Empty);
            var foundEntities = await this.manager.FindEntities(users.Select(x => x.Id));
            Assert.That(foundEntities.Count(), Is.EqualTo(2));

            var adminUser = (await this.manager.GetUsers(x => x.IsAdmin)).ToList();
            
            Assert.Multiple(() =>
            {
                Assert.That(adminUser.All(x => x.IsAdmin), Is.True);
                Assert.That(adminUser, Has.Count.EqualTo(1));
            });
        }

        [Test]
        public async Task VerifyUnavailableActions()
        {
            var operationResult = await this.manager.CreateEntity(null);
            Assert.That(operationResult.Succeeded, Is.False);
            operationResult = await this.manager.UpdateEntity(null);
            Assert.That(operationResult.Succeeded, Is.False);
        }

        [Test]
        public async Task VerifyDelete()
        {
            var userEntity = new UserEntity(Guid.NewGuid())
            {
                IsAdmin = true,
                UserName = "admin"
            };

            var operationResult = await this.manager.DeleteEntity(userEntity);
            Assert.That(operationResult.Succeeded, Is.False);

            userEntity.IsAdmin = false;
            this.authenticationManager.Setup(x => x.FindByNameAsync(userEntity.UserName)).ReturnsAsync((User)null);

            operationResult = await this.manager.DeleteEntity(userEntity);
            Assert.That(operationResult.Succeeded, Is.False);

            this.authenticationManager.Setup(x => x.FindByNameAsync(userEntity.UserName)).ReturnsAsync(new User());
            this.authenticationManager.Setup(x => x.DeleteAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Failed());

            operationResult = await this.manager.DeleteEntity(userEntity);
            Assert.That(operationResult.Succeeded, Is.False);

            this.authenticationManager.Setup(x => x.DeleteAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success);
            await this.manager.DeleteEntity(userEntity);
            this.context.Verify(x => x.Remove(userEntity), Times.Once);
        }

        [Test]
        public async Task VerifyResolveProperties()
        {
            var user = new UserEntity();
            await this.manager.ResolveProperties(user, new ParticipantDto());
            Assert.That(user.UserName, Is.Null);
            
            await this.manager.ResolveProperties(user, new UserEntityDto()
            {
                UserName = "admin",
                IsAdmin = true
            });

            Assert.Multiple(() =>
            {
                Assert.That(user.UserName, Is.Not.Null);
                Assert.That(user.IsAdmin, Is.True);
            });
        }

        [Test]
        public async Task VerifyRegisterUser()
        {
            var user = new User()
            {
                UserName = "admin"
            };

            this.authenticationManager.Setup(x => x.CreateAsync(user, It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed());
            var operationResult = await this.manager.RegisterUser(user, "pass");
            Assert.That(operationResult.Succeeded, Is.False);
            this.authenticationManager.Setup(x => x.CreateAsync(user, It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            this.authenticationManager.Setup(x => x.FindByNameAsync("admin")).ReturnsAsync(user);
            await this.manager.RegisterUser(user, "pass");
            this.context.Verify(x => x.Add(It.IsAny<UserEntity>()),Times.Once);
            this.context.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(new InvalidOperationException());
            operationResult = await this.manager.RegisterUser(user, "pass");
            Assert.That(operationResult.Succeeded, Is.False);
        }

        [Test]
        public async Task VerifyGetSearchResult()
        {
            var userEntity = new UserEntity(Guid.NewGuid())
            {
                EntityContainer = new Project(Guid.NewGuid())
            };

            var result = await this.manager.GetSearchResult(userEntity.Id);
            Assert.That(result, Is.Null);

            this.userDbSet.UpdateDbSetCollection(new List<UserEntity> { userEntity });
            result = await this.manager.GetSearchResult(userEntity.Id);
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
