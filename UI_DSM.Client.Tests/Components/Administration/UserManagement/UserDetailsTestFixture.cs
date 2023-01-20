// --------------------------------------------------------------------------------------------------------
// <copyright file="UserDetailsTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Components.Administration.UserManagement
{
    using Bunit;

    using DevExpress.Blazor;

    using NUnit.Framework;

    using UI_DSM.Client.Components.Administration.UserManagement;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.Components.Administration.UserManagement;
    using UI_DSM.Shared.DTO.Models;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class UserDetailsTestFixture
    {
        private TestContext context;
        private IUserDetailsViewModel viewModel;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.context.ConfigureDevExpressBlazor();
            this.viewModel = new UserDetailsViewModel();
        }

        [TearDown]
        public void TearDown()
        {
            this.context.CleanContext();
        }

        [Test]
        public void VerifyRender()
        {
            this.viewModel.UserEntity = new UserEntityDto()
            {
                UserName = "admin",
                IsAdmin = true,
                Id = Guid.NewGuid()
            };

            var renderer = this.context.RenderComponent<UserDetails>(parameters => 
                parameters.Add(p => p.ViewModel, this.viewModel));

            var inputTexts = renderer.FindComponents<DxTextBox>();

            Assert.That(inputTexts[0].Instance.Text, Is.EqualTo(this.viewModel.UserEntity.UserName));

            var userId = renderer.Find("#userId");
            Assert.That(userId.TextContent, Is.EqualTo(this.viewModel.UserEntity.Id.ToString()));
        }
    }
}
