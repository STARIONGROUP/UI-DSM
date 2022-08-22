// --------------------------------------------------------------------------------------------------------
// <copyright file="RoleControllerTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Tests.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Server.Controllers;
    using UI_DSM.Server.Managers.RoleManager;
    using UI_DSM.Server.Types;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class RoleControllerTestFixture
    {
        private RoleController controller;
        private Mock<IRoleManager> roleManager;

        [SetUp]
        public void Setup()
        {
            this.roleManager = new Mock<IRoleManager>();
            this.controller = new RoleController(this.roleManager.Object);
        }

        [Test]
        public void VerifyGetRoles()
        {
            var roles = new List<Role>();
            this.roleManager.Setup(x => x.GetEntities()).ReturnsAsync(roles);

            var rolesReponses = this.controller.GetEntities().Result as OkObjectResult;
            Assert.That(rolesReponses, Is.Not.Null);
            Assert.That((IEnumerable<RoleDto>)rolesReponses.Value, Is.Empty);
            roles.Add(new Role(Guid.NewGuid()));
            rolesReponses = this.controller.GetEntities().Result as OkObjectResult;
            Assert.That(rolesReponses, Is.Not.Null);
            Assert.That(((IEnumerable<RoleDto>)rolesReponses.Value!).Count(), Is.EqualTo(1));
            this.roleManager.Setup(x => x.GetEntity(roles.First().Id)).ReturnsAsync(roles.First());
            Assert.That(this.controller.GetEntity(roles.First().Id).Result, Is.TypeOf<OkObjectResult>());
            Assert.That(this.controller.GetEntity(Guid.NewGuid()).Result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public void VerifyCreateRole()
        {
            var newRole = new RoleDto(Guid.NewGuid());

            var badRequestResponse = this.controller.CreateEntity(newRole).Result as BadRequestObjectResult;
            Assert.That(badRequestResponse, Is.Not.Null);

            newRole = new RoleDto()
            {
                RoleName = "Project administrator",
                AccessRights = new List<AccessRight>()
                {
                    AccessRight.ManageParticipant
                }
            };

            this.roleManager.Setup(x => x.CreateEntity(It.IsAny<Role>())).ReturnsAsync(EntityOperationResult<Role>.Failed());
            Assert.That(this.controller.CreateEntity(newRole).Result, Is.TypeOf<BadRequestObjectResult>());

            var role = (Role)newRole.InstantiatePoco();
            role.ResolveProperties(newRole);
            this.roleManager.Setup(x => x.CreateEntity(It.IsAny<Role>())).ReturnsAsync(EntityOperationResult<Role>.Success(role));
            Assert.That(this.controller.CreateEntity(newRole).Result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        public void VerifyUpdateRole()
        {
            var role = new Role(Guid.NewGuid())
            {
                RoleName = "Project administrator",
                AccessRights = new List<AccessRight>()
                {
                    AccessRight.ManageParticipant
                }
            };

            var roleDto = (RoleDto)role.ToDto();
            this.roleManager.Setup(x => x.GetEntity(role.Id)).ReturnsAsync((Role)null);
            Assert.That(this.controller.UpdateEntity(roleDto.Id, roleDto).Result, Is.TypeOf<NotFoundObjectResult>());

            this.roleManager.Setup(x => x.GetEntity(role.Id)).ReturnsAsync(role);
            this.roleManager.Setup(x => x.UpdateEntity(role)).ReturnsAsync(EntityOperationResult<Role>.Failed());
            Assert.That(this.controller.UpdateEntity(roleDto.Id, roleDto).Result, Is.TypeOf<BadRequestObjectResult>());

            this.roleManager.Setup(x => x.UpdateEntity(role)).ReturnsAsync(EntityOperationResult<Role>.Success(role));
            Assert.That(this.controller.UpdateEntity(roleDto.Id, roleDto).Result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        public void VerifyDeleteRole()
        {
            var role = new Role(Guid.NewGuid())
            {
                RoleName = "Project administrator",
                AccessRights = new List<AccessRight>()
                {
                    AccessRight.ManageParticipant
                }
            };

            this.roleManager.Setup(x => x.GetEntity(role.Id)).ReturnsAsync((Role)null);
            Assert.That(this.controller.DeleteEntity(role.Id).Result, Is.TypeOf<NotFoundObjectResult>());
            this.roleManager.Setup(x => x.GetEntity(role.Id)).ReturnsAsync(role);
            this.roleManager.Setup(x => x.DeleteEntity(role)).ReturnsAsync(EntityOperationResult<Role>.Failed());
            Assert.That(((ObjectResult)this.controller.DeleteEntity(role.Id).Result).StatusCode, Is.EqualTo(500));
            this.roleManager.Setup(x => x.DeleteEntity(role)).ReturnsAsync(EntityOperationResult<Role>.Success(role));
            Assert.That(this.controller.DeleteEntity(role.Id).Result, Is.TypeOf<OkObjectResult>());
        }
    }
}
