// --------------------------------------------------------------------------------------------------------
// <copyright file="RolePageTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Pages.Administration.RolePages
{
    using Bunit;

    using Microsoft.AspNetCore.Components.Forms;
    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Client.Pages.Administration.RolePages;
    using UI_DSM.Client.Services.Administration.RoleService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.Pages.Administration.RolePages;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class RolePageTestFixture
    {
        private TestContext context;
        private IRolePageViewModel viewModel;
        private Mock<IRoleService> roleService;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.context.ConfigureDevExpressBlazor();
            this.roleService = new Mock<IRoleService>();
            this.viewModel = new RolePageViewModel(this.roleService.Object);
            this.context.Services.AddSingleton(this.viewModel);
        }

        [TearDown]
        public void Teardown()
        {
            this.context.CleanContext();
        }

        [Test]
        public void VerifyPage()
        {
            var roleId = Guid.NewGuid();
            this.roleService.Setup(x => x.GetRole(roleId)).ReturnsAsync((Role)null);

            var renderer = this.context.RenderComponent<RolePage>(parameters =>
            {
                parameters.Add(p => p.RoleId, roleId.ToString());
            });

            var notFoundDiv = renderer.Find("div");
            Assert.That(notFoundDiv.TextContent.Contains("Role not found"), Is.True);

            var role = new Role(roleId)
            {
                RoleName = "Project Manager",
                AccessRights = new List<AccessRight>()
                {
                    AccessRight.ManageParticipant
                }
            };

            this.roleService.Setup(x => x.GetRole(roleId)).ReturnsAsync(role);
            this.viewModel.OnInitializedAsync(roleId);
            renderer.Render();
            Assert.That(renderer.FindComponents<EditForm>().Count, Is.EqualTo(1));
        }
    }
}
