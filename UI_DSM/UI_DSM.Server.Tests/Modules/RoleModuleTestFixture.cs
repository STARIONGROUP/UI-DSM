// --------------------------------------------------------------------------------------------------------
// <copyright file="RoleModuleTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Tests.Modules
{
    using Microsoft.AspNetCore.Http;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Server.Managers;
    using UI_DSM.Server.Modules;
    using UI_DSM.Server.Tests.Helpers;
    using UI_DSM.Server.Types;
    using UI_DSM.Server.Validator;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class RoleModuleTestFixture
    {
        private RoleModule module;
        private Mock<IEntityManager<Role>> roleManager;
        private List<Role> roles;
        private Mock<HttpContext> httpContext;
        private Mock<HttpResponse> httpResponse;

        [SetUp]
        public void Setup()
        {
            this.roles = new List<Role>
            {
                new(Guid.NewGuid())
                {
                    RoleName = "Role 1",
                    AccessRights = new List<AccessRight>()
                    {
                        AccessRight.ReviewTask
                    }
                }
            };

            this.roleManager = new Mock<IEntityManager<Role>>();
            this.roleManager.Setup(x => x.GetEntities(0)).ReturnsAsync(this.roles);

            ModuleTestHelper.Setup<RoleModule, RoleDto>(new RoleDtoValidator(), out this.httpContext, out this.httpResponse, out _, out _);
            this.module = new RoleModule();
            Program.RegisterEntities();
        }

        [Test]
        public async Task VerifyGetRoles()
        {
            await this.module.GetEntities(this.roleManager.Object,this.httpContext.Object);
            this.roleManager.Verify(x => x.GetEntities(0), Times.Once);

            this.roleManager.Setup(x => x.GetEntity(this.roles.First().Id,0)).ReturnsAsync(this.roles);

            var invalidGuid = Guid.NewGuid();
            this.roleManager.Setup(x => x.GetEntity(invalidGuid,0)).ReturnsAsync(Enumerable.Empty<Role>());
            await this.module.GetEntity(this.roleManager.Object, invalidGuid, this.httpContext.Object);
            this.httpResponse.VerifySet(x => x.StatusCode = 404, Times.Once);

            await this.module.GetEntity(this.roleManager.Object, this.roles.First().Id, this.httpContext.Object);
            this.httpResponse.VerifySet(x => x.StatusCode = 404, Times.Once);
        }

        [Test]
        public async Task VerifyCreateRole()
        {
            var projectDto = new RoleDto();

            await this.module.CreateEntity(this.roleManager.Object, projectDto, this.httpContext.Object);
            this.httpResponse.VerifySet(x => x.StatusCode = 422, Times.Once);
            projectDto.RoleName = "Role";
            projectDto.Id = Guid.NewGuid();

            await this.module.CreateEntity(this.roleManager.Object, projectDto, this.httpContext.Object);
            this.httpResponse.VerifySet(x => x.StatusCode = 422, Times.Exactly(2));

            projectDto.Id = Guid.Empty;

            this.roleManager.Setup(x => x.CreateEntity(It.IsAny<Role>()))
                .ReturnsAsync(EntityOperationResult<Role>.Failed());

            await this.module.CreateEntity(this.roleManager.Object, projectDto, this.httpContext.Object);
            this.httpResponse.VerifySet(x => x.StatusCode = 500, Times.Once);

            this.roleManager.Setup(x => x.CreateEntity(It.IsAny<Role>()))
                .ReturnsAsync(EntityOperationResult<Role>.Success(new Role(Guid.NewGuid())
                {
                    RoleName = projectDto.RoleName
                }));

            await this.module.CreateEntity(this.roleManager.Object, projectDto, this.httpContext.Object);
            this.httpResponse.VerifySet(x => x.StatusCode = 201, Times.Once);
        }

        [Test]
        public async Task VerifyDeleteRole()
        {
            var guid = Guid.NewGuid();
            this.roleManager.Setup(x => x.FindEntity(guid)).ReturnsAsync((Role)null);

            var response = await this.module.DeleteEntity(this.roleManager.Object, guid, this.httpContext.Object);
            Assert.That(response.IsRequestSuccessful, Is.False);
            this.httpResponse.VerifySet(x => x.StatusCode = 404, Times.Once);

            var role = new Role(guid)
            {
                RoleName = "Role"
            };

            this.roleManager.Setup(x => x.FindEntity(guid)).ReturnsAsync(role);
            this.roleManager.Setup(x => x.DeleteEntity(role)).ReturnsAsync(EntityOperationResult<Role>.Failed());
            response = await this.module.DeleteEntity(this.roleManager.Object, guid, this.httpContext.Object);
            Assert.That(response.IsRequestSuccessful, Is.False);
            this.httpResponse.VerifySet(x => x.StatusCode = 500, Times.Once);

            this.roleManager.Setup(x => x.DeleteEntity(role)).ReturnsAsync(EntityOperationResult<Role>.Success(role));
            response = await this.module.DeleteEntity(this.roleManager.Object, guid, this.httpContext.Object);
            Assert.That(response.IsRequestSuccessful, Is.True);
        }

        [Test]
        public async Task VerifyUpdateRole()
        {
            var role = new Role(Guid.NewGuid());

            this.roleManager.Setup(x => x.FindEntity(role.Id)).ReturnsAsync((Role)null);
            await this.module.UpdateEntity(this.roleManager.Object, role.Id, (RoleDto)role.ToDto(), this.httpContext.Object);
            this.httpResponse.VerifySet(x => x.StatusCode = 404, Times.Once);

            this.roleManager.Setup(x => x.FindEntity(role.Id)).ReturnsAsync(role);
            await this.module.UpdateEntity(this.roleManager.Object, role.Id, (RoleDto)role.ToDto(), this.httpContext.Object);
            this.httpResponse.VerifySet(x => x.StatusCode = 422, Times.Once);

            role.RoleName = "Role";
            this.roleManager.Setup(x => x.UpdateEntity(It.IsAny<Role>())).ReturnsAsync(EntityOperationResult<Role>.Failed());

            await this.module.UpdateEntity(this.roleManager.Object, role.Id, (RoleDto)role.ToDto(), this.httpContext.Object);
            this.httpResponse.VerifySet(x => x.StatusCode = 500, Times.Once);

            this.roleManager.Setup(x => x.UpdateEntity(It.IsAny<Role>())).ReturnsAsync(EntityOperationResult<Role>.Success(role));
            await this.module.UpdateEntity(this.roleManager.Object, role.Id, (RoleDto)role.ToDto(), this.httpContext.Object);
            this.httpResponse.VerifySet(x => x.StatusCode = 200, Times.Once);
        }
    }
}
