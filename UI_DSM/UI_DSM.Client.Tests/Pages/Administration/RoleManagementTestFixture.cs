// --------------------------------------------------------------------------------------------------------
// <copyright file="RoleManagementTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Pages.Administration
{
    using Bunit;

    using DevExpress.Blazor;

    using Microsoft.AspNetCore.Components;
    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Client.Pages.Administration;
    using UI_DSM.Client.Services.Administration.RoleService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.Pages.Administration;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.Types;
    using UI_DSM.Shared.Wrappers;

    using TestContext = Bunit.TestContext;

    using AppComponents;

    [TestFixture]
    public class RoleManagementTestFixture
    {
        private TestContext context;
        private IRoleManagementViewModel viewModel;
        private Mock<IRoleService> roleService;
        private List<Role> roles;

        [SetUp]
        public void Setup()
        {
            this.roles = new List<Role>();
            this.context = new TestContext();
            this.context.ConfigureDevExpressBlazor();
            this.roleService = new Mock<IRoleService>();
            this.roleService.Setup(x => x.GetRoles(0)).ReturnsAsync(this.roles);
            this.viewModel = new RoleManagementViewModel(this.roleService.Object, null);
            this.context.Services.AddSingleton(this.roleService);
            this.context.Services.AddSingleton(this.viewModel);
            this.viewModel.NavigationManager = this.context.Services.GetRequiredService<NavigationManager>();
        }

        [TearDown]
        public void Teardown()
        {
            this.context.CleanContext();
        }

        [Test]
        public async Task VerifyOnInitialized()
        {
            var renderer = this.context.RenderComponent<RoleManagement>();
            var notFound = renderer.Find("#noRoleFound");
            Assert.That(notFound, Is.Not.Null);

            this.roles.Add(new Role(Guid.NewGuid())
            {
                RoleName = "Project manager",
                AccessRights = new List<AccessRight>()
                {
                    AccessRight.ManageParticipant,
                    AccessRight.CreateTask
                }
            });

            await renderer.InvokeAsync(this.viewModel.OnInitializedAsync);
            
            Assert.That(() => renderer.Find("#noRoleFound"), Throws.Exception);
            Assert.That(this.viewModel.Roles.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task VerifyOpenCreationPopupAndCreateRole()
        {
            var renderer = this.context.RenderComponent<RoleManagement>();
            var appButton = renderer.FindComponent<AppButton>();
            var dxPopup = renderer.FindComponent<DxPopup>();
            var currentCreationRole = this.viewModel.RoleCreationViewModel.Role;
            Assert.That(dxPopup.Instance.Visible, Is.False);
            await renderer.InvokeAsync(appButton.Instance.Click.InvokeAsync);
            Assert.That(dxPopup.Instance.Visible, Is.True);
            Assert.That(this.viewModel.RoleCreationViewModel.Role, Is.Not.EqualTo(currentCreationRole));

            this.viewModel.RoleCreationViewModel.SelectedAccessRights = new List<AccessRightWrapper>()
            {
                new(AccessRight.ManageParticipant)
            };

            this.viewModel.RoleCreationViewModel.Role.RoleName = "Project manager";

            this.roleService.Setup(x => x.CreateRole(this.viewModel.RoleCreationViewModel.Role))
                .ReturnsAsync(EntityRequestResponse<Role>.Fail(new List<string>()
                {
                    "A role with the same name already exists"
                }));

            await this.viewModel.RoleCreationViewModel.OnValidSubmit.InvokeAsync();
            Assert.That(this.viewModel.ErrorMessageViewModel.Errors.Count, Is.EqualTo(1));
            Assert.That(dxPopup.Instance.Visible, Is.True);

            this.roleService.Setup(x => x.CreateRole(this.viewModel.RoleCreationViewModel.Role))
                .ReturnsAsync(EntityRequestResponse<Role>.Success(new Role()));

            await this.viewModel.RoleCreationViewModel.OnValidSubmit.InvokeAsync();
            Assert.That(this.viewModel.Roles.Count, Is.EqualTo(1));
            Assert.That(this.viewModel.IsOnCreationMode, Is.False);
            Assert.That(this.viewModel.ErrorMessageViewModel.Errors, Is.Empty);

            this.roleService.Setup(x => x.CreateRole(this.viewModel.RoleCreationViewModel.Role))
                .ThrowsAsync(new HttpRequestException());

            await this.viewModel.RoleCreationViewModel.OnValidSubmit.InvokeAsync();
            Assert.That(this.viewModel.ErrorMessageViewModel.Errors.Count, Is.EqualTo(1));
        }

        [Test]
        public void VerifyGoToPage()
        {
            var role = new Role(Guid.NewGuid());
            this.viewModel.GoToRolePage(role);
            Assert.That(this.viewModel.NavigationManager.Uri.Contains($"Administration/Role/{role.Id}"), Is.True);
        }
    }
}
