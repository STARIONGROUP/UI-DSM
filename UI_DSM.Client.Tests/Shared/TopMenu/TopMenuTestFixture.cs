// --------------------------------------------------------------------------------------------------------
// <copyright file="TopMenuTestFixture.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw, Nabil Abbar
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Client.Tests.Shared.TopMenu
{
    using System.Security.Claims;

    using Blazored.SessionStorage;

    using Bunit;
    using Bunit.TestDoubles;

    using Microsoft.AspNetCore.Components.Authorization;
    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Client.Components.App.UserAvatar;
    using UI_DSM.Client.Services.Administration.UserService;
    using UI_DSM.Client.Services.AuthenticationService;
    using UI_DSM.Client.Shared.TopMenu;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.Shared.TopMenu;
    using UI_DSM.Shared.Models;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class TopMenuTestFixture
    {
        private ITopMenuViewModel viewModel;
        private TestContext context;
        private Mock<IUserService> userService;
        private Mock<ISessionStorageService> sessionStorage;
        private Mock<AuthenticationProvider> authenticationProvider;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.context.ConfigureDevExpressBlazor();
            this.sessionStorage = new Mock<ISessionStorageService>();
            this.userService = new Mock<IUserService>();
            this.authenticationProvider = new Mock<AuthenticationProvider>(new HttpClient(), this.sessionStorage.Object);
            this.viewModel = new TopMenuViewModel(this.userService.Object, this.authenticationProvider.Object);
            this.context.Services.AddSingleton(this.viewModel);
            this.context.AddTestAuthorization();

            this.authenticationProvider.Setup(x => x.GetAuthenticationStateAsync())
                .ReturnsAsync(new AuthenticationState(new ClaimsPrincipal(new List<ClaimsIdentity>())));
        }

        [TearDown]
        public void Teardown()
        {
            this.context.CleanContext();
        }

        [Test]
        public async Task VerifyComponent()
        {
            var renderer = this.context.RenderComponent<TopMenu>();

            Assert.That(renderer.FindComponents<UserAvatar>(), Is.Empty);

            this.authenticationProvider.Setup(x => x.GetAuthenticationStateAsync())
                .ReturnsAsync(new AuthenticationState(new ClaimsPrincipal(new List<ClaimsIdentity>()
                {
                    new(new List<Claim>()
                    {
                        new(ClaimTypes.Name, "user")
                    }, "mock")
                })));

            await renderer.Instance.ViewModel.InitializesViewModel();
            renderer.Render();
            Assert.That(renderer.FindComponents<UserAvatar>(), Is.Not.Empty);

            var projectId = Guid.NewGuid();

            this.userService.Setup(x => x.GetParticipantsForUser()).ReturnsAsync(new List<Participant>
            {
                new ()
                {
                    Role = new Role()
                    {
                        RoleName = "Project Manager"
                    },
                    EntityContainer = new Project(projectId)
                }
            });

            await renderer.InvokeAsync(renderer.Instance.OpenUserMenu);
            renderer.Render();
            Assert.That(() => renderer.Find("#roleName"), Throws.Exception);

            await renderer.InvokeAsync(renderer.Instance.OpenUserMenu);
            renderer.Render();
            renderer.Instance.NavigationManager.NavigateTo($"/Project/{Guid.NewGuid()}");

            await renderer.InvokeAsync(renderer.Instance.OpenUserMenu);
            renderer.Render();
            Assert.That(() => renderer.Find("#roleName"), Throws.Exception);
            
            await renderer.InvokeAsync(renderer.Instance.OpenUserMenu);
            renderer.Render();
            renderer.Instance.NavigationManager.NavigateTo($"/Project/{projectId}");

            Assert.That(renderer.Instance.GetRoleName(), Is.Not.Empty);
        }
    }
}
