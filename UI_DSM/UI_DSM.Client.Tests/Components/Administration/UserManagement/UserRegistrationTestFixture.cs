// --------------------------------------------------------------------------------------------------------
// <copyright file="UserRegistrationTestFixture.cs" company="RHEA System S.A.">
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

    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Forms;

    using NUnit.Framework;

    using UI_DSM.Client.Components.Administration.UserManagement;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.Components;
    using UI_DSM.Client.ViewModels.Components.Administration.UserManagement;
    using UI_DSM.Shared.DTO.UserManagement;

    using TestContext = Bunit.TestContext;

    [TestFixture, Apartment(ApartmentState.STA)]
    public class UserRegistrationTestFixture
    {
        private TestContext testContext;
        private IUserRegistrationViewModel viewModel;
        private IErrorMessageViewModel errorMessage;

        [SetUp]
        public void Setup()
        {
            this.testContext = new TestContext();
            this.testContext.ConfigureDevExpressBlazor();

            this.errorMessage = new ErrorMessageViewModel();

            this.viewModel = new UserRegistrationViewModel()
            {
                OnValidSubmit = new EventCallbackFactory().Create(this, () => this.viewModel.Registration = new RegistrationDto()),
                Registration = new RegistrationDto()
            };
        }

        [TearDown]
        public void Teardown()
        {
            this.testContext.CleanContext();
        }

        [Test]
        public async Task VerifyComponent()
        {
            var renderer = this.testContext
                .RenderComponent<UserRegistration>(parameters =>
                {
                    parameters.Add(p => p.ViewModel, this.viewModel);
                    parameters.AddCascadingValue(this.errorMessage);
                });

            var inputs = renderer.FindComponents<DxTextBox>();
            Assert.That(inputs.Count(), Is.EqualTo(3));

            this.viewModel.Registration.UserName = "admin";
            
            renderer.Render();
            Assert.That(inputs[0].Instance.Text, Is.EqualTo(this.viewModel.Registration.UserName));

            var formSubmit = renderer.FindComponent<EditForm>();
            await formSubmit.Instance.OnValidSubmit.InvokeAsync();
            Assert.That(this.viewModel.Registration.UserName, Is.Null);
        }
    }
}
