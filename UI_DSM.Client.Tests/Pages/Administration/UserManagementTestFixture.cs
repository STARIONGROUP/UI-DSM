// --------------------------------------------------------------------------------------------------------
// <copyright file="UserManagementTestFixture.cs" company="RHEA System S.A.">
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
    using Bunit.TestDoubles;

    using DevExpress.Blazor;

    using DynamicData;

    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Client.Pages.Administration;
    using UI_DSM.Client.Services.Administration.UserService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.Pages.Administration;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.DTO.UserManagement;

    using TestContext = Bunit.TestContext;

    using AppComponents;

    [TestFixture]
    public class UserManagementTestFixture
    {
        private TestContext context;
        private IUserManagementViewModel viewModel;
        private Mock<IUserService> userService;
        private List<UserEntityDto> users;

        [SetUp]
        public void Setup()
        {
            this.userService = new Mock<IUserService>();

            this.users = new List<UserEntityDto>
            {
                new ()
                {
                    UserName = "admin",
                    IsAdmin = true,
                    Id = Guid.NewGuid()
                },
                new ()
                {
                    UserName = "notAdmin",
                    IsAdmin = false,
                    Id = Guid.NewGuid()
                }
            };

            this.userService.Setup(x => x.GetUsers()).ReturnsAsync(this.users);
            this.viewModel = new UserManagementViewModel(this.userService.Object);
            this.context = new TestContext();
            this.context.Services.AddSingleton(this.viewModel);
            this.context.ConfigureDevExpressBlazor();
            this.context.AddTestAuthorization();
        }

        [TearDown]
        public void Teardown()
        {
            this.context.CleanContext();
        }

        [Test]
        public async Task VerifyRegisterUser()
        {
            var renderer = this.context.RenderComponent<UserManagement>();

            var registrationDto = this.viewModel.UserRegistrationViewModel.Registration;
            var dxButton = renderer.FindComponent<AppButton>();
            await renderer.InvokeAsync(() => dxButton.Instance.Click.InvokeAsync());
            Assert.That(this.viewModel.IsOnCreationMode, Is.True);
            Assert.That(this.viewModel.UserRegistrationViewModel.Registration, Is.Not.EqualTo(registrationDto));

            this.userService.Setup(x => x.RegisterUser(It.IsAny<RegistrationDto>())).ReturnsAsync(new RegistrationResponseDto()
            {
                IsRequestSuccessful = false,
                Errors = new List<string>()
                {
                    "User already exists"
                }
            });

            await renderer.InvokeAsync(() => this.viewModel.UserRegistrationViewModel.OnValidSubmit.InvokeAsync());
            Assert.That(this.viewModel.IsOnCreationMode, Is.True);

            this.userService.Setup(x => x.RegisterUser(It.IsAny<RegistrationDto>())).ReturnsAsync(new RegistrationResponseDto()
            {
                IsRequestSuccessful = true,
                CreatedUserEntity = new UserEntityDto()
                {
                    UserName = "aNewUser"
                }
            });
            
            await renderer.InvokeAsync(() => this.viewModel.UserRegistrationViewModel.OnValidSubmit.InvokeAsync());
            Assert.That(this.viewModel.IsOnCreationMode, Is.False);
            Assert.That(this.viewModel.Users.Count, Is.EqualTo(3));
        }

        [Test]
        public void VerifyOnInitializedAsync()
        {
            var renderer = this.context.RenderComponent<UserManagement>();

            var table = renderer.Find("table");
            var tableBody = table.LastElementChild;
            Assert.That(tableBody!.Children.Length, Is.EqualTo(2));

            this.viewModel.Users.Add(new UserEntityDto()
            {
                UserName = "anotherUser"
            });

            table = renderer.Find("table");
            tableBody = table.LastElementChild;
            Assert.That(tableBody!.Children.Length, Is.EqualTo(3));
        }

        [Test]
        public async Task VerifyDeleteUser()
        {
            var renderer = this.context.RenderComponent<UserManagement>();

            var deleteButton = renderer.FindComponents<DxButton>()
                .FirstOrDefault(x => x.Instance.RenderStyle == ButtonRenderStyle.Danger);

            Assert.That(this.viewModel.ConfirmCancelPopup.IsVisible, Is.False);
            await renderer.InvokeAsync(() => deleteButton!.Instance.Click.InvokeAsync());

            Assert.That(this.viewModel.ConfirmCancelPopup.IsVisible, Is.True);

            this.userService.Setup(x => x.DeleteUser(It.IsAny<UserEntityDto>())).ReturnsAsync(new RequestResponseDto()
            {
                IsRequestSuccessful = true
            });

            await renderer.InvokeAsync(() => this.viewModel.ConfirmCancelPopup.OnConfirm.InvokeAsync());
            Assert.That(this.viewModel.Users.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task VerifyOpenDetails()
        {
            var renderer = this.context.RenderComponent<UserManagement>();
            var detailsButtons = renderer.FindComponents<DxButton>().Where(x => x.Instance.Text == "View details");

            await renderer.InvokeAsync(() => detailsButtons.First().Instance.Click.InvokeAsync());
            Assert.That(this.viewModel.IsOnDetailsViewMode, Is.True);
        }
    }
}
