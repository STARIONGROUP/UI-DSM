// --------------------------------------------------------------------------------------------------------
// <copyright file="RoleCreationTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Components.Administration.RoleManagement
{
    using Bunit;

    using DevExpress.Blazor;

    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Forms;

    using NUnit.Framework;

    using UI_DSM.Client.Components.Administration.RoleManagement;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.Components;
    using UI_DSM.Client.ViewModels.Components.Administration.RoleManagement;
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.Wrappers;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class RoleCreationTestFixture
    {
        private TestContext context;
        private IRoleCreationViewModel roleCreationViewModel;
        private IErrorMessageViewModel errorMessageViewModel;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.context.ConfigureDevExpressBlazor();
            this.errorMessageViewModel = new ErrorMessageViewModel();

            this.roleCreationViewModel = new RoleCreationViewModel()
            {
                Role = new Role(),
                OnValidSubmit = new EventCallbackFactory().Create(this, () => this.roleCreationViewModel.Role = new Role())
            };
        }

        [TearDown]
        public void TearDown()
        {
            this.context.CleanContext();
        }

        [Test]
        public async Task VerifyComponent()
        {
            var renderer = this.context.RenderComponent<RoleCreation>(parameters =>
            {
                parameters.AddCascadingValue(this.errorMessageViewModel);
                parameters.Add(p => p.ViewModel, this.roleCreationViewModel);
            });

            var textBox = renderer.FindComponent<DxTextBox>();
            var listBox = renderer.FindComponent<DxListBox<AccessRightWrapper, AccessRightWrapper>>();
            Assert.That(textBox.Instance.Text, Is.Null);
            Assert.That(listBox.Instance.Values, Is.Empty);

            this.roleCreationViewModel.Role.RoleName = "role";

            renderer.Render();
            Assert.That(textBox.Instance.Text, Is.EqualTo(this.roleCreationViewModel.Role.RoleName));

            var dxButton = renderer.FindComponent<EditForm>();
            await renderer.InvokeAsync(dxButton.Instance.OnValidSubmit.InvokeAsync);

            Assert.That(this.roleCreationViewModel.Role.RoleName, Is.Null);
        }
    }
}
