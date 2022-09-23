// --------------------------------------------------------------------------------------------------------
// <copyright file="IndexTestFixture.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Client.Tests.Pages
{
    using System.Security.Claims;

    using Bunit;
    using Bunit.TestDoubles;

    using Microsoft.AspNetCore.Components.Authorization;
    using Microsoft.Extensions.DependencyInjection;

    using Moq;
    
    using NUnit.Framework;

    using UI_DSM.Client.Pages;
    using UI_DSM.Client.Services.Administration.ProjectService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.Components;
    using UI_DSM.Client.ViewModels.Pages;
    using UI_DSM.Shared.DTO.UserManagement;
    using UI_DSM.Shared.Models;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class IndexTestFixture
    {
        private TestContext context;
        private IIndexViewModel viewModel;
        private Mock<IProjectService> projectService;
        private Mock<AuthenticationStateProvider> authenticationProvider;
        private AuthenticationState authenticationState;
        private Mock<ILoginViewModel> loginViewModel;

        [SetUp]
        public void Setup()
        {
            this.authenticationState = new AuthenticationState(new ClaimsPrincipal());

            this.context = new TestContext();
            this.context.ConfigureDevExpressBlazor();
            this.projectService = new Mock<IProjectService>();
            this.loginViewModel = new Mock<ILoginViewModel>();
            this.loginViewModel.Setup(x => x.Authentication).Returns(new AuthenticationDto());
            this.authenticationProvider = new Mock<AuthenticationStateProvider>();
            this.authenticationProvider.Setup(x => x.GetAuthenticationStateAsync()).ReturnsAsync(this.authenticationState);
            this.viewModel = new IndexViewModel(this.projectService.Object, this.authenticationProvider.Object, null);
            this.context.Services.AddSingleton(this.viewModel);
            this.context.Services.AddSingleton(this.loginViewModel.Object);
            this.context.AddTestAuthorization().SetAuthorized("user");
        }

        [TearDown]
        public void Teardown()
        {
            this.context.CleanContext();
        }

        [Test]
        public void VerifyRenderer()
        {
            var availableProject = new List<Project>()
            {
                new(Guid.NewGuid())
                {
                    ProjectName = "Project 1"
                },
                new(Guid.NewGuid())
                {
                    ProjectName = "Project 2"
                }
            };

            this.projectService.Setup(x => x.GetUserParticipation()).ReturnsAsync(availableProject);
            var renderer = this.context.RenderComponent<Index>();

            var divProject1 = renderer.FindAll("div");
            Assert.That(divProject1, Has.Count.Zero);

            this.authenticationState = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()
            {
                new(ClaimTypes.Name, "user"),
            }, "basic")));

            this.authenticationProvider.Setup(x => x.GetAuthenticationStateAsync()).ReturnsAsync(this.authenticationState);
            this.viewModel.PopulateAvailableProjects();
        }
    }
}
