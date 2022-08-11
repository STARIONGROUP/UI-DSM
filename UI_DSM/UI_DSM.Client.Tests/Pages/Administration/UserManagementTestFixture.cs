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
    using UI_DSM.Client.ViewModels.Pages.Administration;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.UserManagement;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class UserManagementTestFixture
    {
        private TestContext context;
        private IUserManagementViewModel viewModel;
        private Mock<IUserService> userService;
        private List<UserDto> users;

        [SetUp]
        public void Setup()
        {
            this.userService = new Mock<IUserService>();

            this.users = new List<UserDto>
            {
                new ()
                {
                    UserName = "admin",
                    IsAdmin = true,
                    UserId = Guid.NewGuid().ToString()
                },
                new ()
                {
                    UserName = "notAdmin",
                    IsAdmin = false,
                    UserId = Guid.NewGuid().ToString()
                }
            };

            this.userService.Setup(x => x.GetUsers()).ReturnsAsync(this.users);
            this.viewModel = new UserManagementViewModel(this.userService.Object);
            this.context = new TestContext();
            this.context.Services.AddSingleton(this.viewModel);
            this.context.Services.AddDevExpressBlazor();
            this.context.AddTestAuthorization();
        }

        [TearDown]
        public void Teardown()
        {
            this.context.Dispose();
        }

        [Test]
        public void VerifyRegisterUser()
        {
            var renderer = this.context.RenderComponent<UserManagement>();

            var registrationDto = this.viewModel.UserRegistrationViewModel.Registration;
            var dxButton = renderer.FindComponent<DxButton>();
            renderer.InvokeAsync(() => dxButton.Instance.Click.InvokeAsync());
            Assert.That(this.viewModel.RegistrationPopupVisible, Is.True);
            Assert.That(this.viewModel.UserRegistrationViewModel.Registration, Is.Not.EqualTo(registrationDto));

            this.userService.Setup(x => x.RegisterUser(It.IsAny<RegistrationDto>())).ReturnsAsync(new RegistrationResponseDto()
            {
                IsRequestSuccessful = false,
                Errors = new List<string>()
                {
                    "User already exists"
                }
            });

            renderer.InvokeAsync(() => this.viewModel.UserRegistrationViewModel.OnValidSubmit.InvokeAsync());
            Assert.That(this.viewModel.RegistrationPopupVisible, Is.True);

            this.userService.Setup(x => x.RegisterUser(It.IsAny<RegistrationDto>())).ReturnsAsync(new RegistrationResponseDto()
            {
                IsRequestSuccessful = true,
                CreatedUser = new UserDto()
                {
                    UserName = "aNewUser"
                }
            });
            
            renderer.InvokeAsync(() => this.viewModel.UserRegistrationViewModel.OnValidSubmit.InvokeAsync());
            Assert.That(this.viewModel.RegistrationPopupVisible, Is.False);
            Assert.That(this.viewModel.Users.Count, Is.EqualTo(3));
        }

        [Test]
        public void VerifyOnInitializedAsync()
        {
            var renderer = this.context.RenderComponent<UserManagement>();

            var table = renderer.Find("table");
            var tableBody = table.LastElementChild;
            Assert.That(tableBody!.Children.Length, Is.EqualTo(2));

            this.viewModel.Users.Add(new UserDto()
            {
                UserName = "anotherUser"
            });

            table = renderer.Find("table");
            tableBody = table.LastElementChild;
            Assert.That(tableBody!.Children.Length, Is.EqualTo(3));
        }

        [Test]
        public void VerifyDeleteUser()
        {
            var renderer = this.context.RenderComponent<UserManagement>();

            var deleteButton = renderer.FindComponents<DxButton>()
                .FirstOrDefault(x => x.Instance.RenderStyle == ButtonRenderStyle.Danger);

            Assert.That(this.viewModel.ConfirmCancelPopup.IsVisible, Is.False);
            renderer.InvokeAsync(() => deleteButton!.Instance.Click.InvokeAsync());

            Assert.That(this.viewModel.ConfirmCancelPopup.IsVisible, Is.True);

            this.userService.Setup(x => x.DeleteUser(It.IsAny<UserDto>())).ReturnsAsync(new RequestResponseDto()
            {
                IsRequestSuccessful = true
            });

            renderer.InvokeAsync(() => this.viewModel.ConfirmCancelPopup.OnConfirm.InvokeAsync());
            Assert.That(this.viewModel.Users.Count, Is.EqualTo(1));
        }

        [Test]
        public void VerifyOpenDetails()
        {
            var renderer = this.context.RenderComponent<UserManagement>();
            var detailsButtons = renderer.FindComponents<DxButton>().Where(x => x.Instance.Text == "View details");

            renderer.InvokeAsync(() => detailsButtons.First().Instance.Click.InvokeAsync());
            Assert.That(this.viewModel.UserDetailsPopupVisible, Is.True);
        }
    }
}
