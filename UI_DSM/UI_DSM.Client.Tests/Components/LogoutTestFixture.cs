// --------------------------------------------------------------------------------------------------------
// <copyright file="LogoutTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Components
{
    using Bunit;
    using Bunit.TestDoubles;

    using Microsoft.AspNetCore.Components;
    using Microsoft.Extensions.DependencyInjection;

    using Moq;
    using Moq.Protected;

    using NUnit.Framework;

    using UI_DSM.Client.Components;
    using UI_DSM.Client.Services.AuthenticationService;
    using UI_DSM.Client.ViewModels.Components;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class LogoutTestFixture
    {
        private TestContext context;
        private Mock<IAuthenticationService> authenticationService;
        private Mock<NavigationManager> navigationManager;
        private ILogoutViewModel logoutViewModel;

        [SetUp]
        public void Setup()
        {
            this.authenticationService = new Mock<IAuthenticationService>();
            this.context = new TestContext();
            this.logoutViewModel = new LogoutViewModel(this.authenticationService.Object, null);
            this.context.Services.AddSingleton(this.logoutViewModel);
            this.logoutViewModel.NavigationManager = this.context.Services.GetRequiredService<NavigationManager>();
        }

        [TearDown]
        public void TearDown()
        {
            this.context.Dispose();
        }

        [Test]
        public void VerifyLogout()
        {
            this.authenticationService.Setup(x => x.Logout()).Returns(Task.CompletedTask);

            var renderComponent = this.context.RenderComponent<Logout>();

            renderComponent.Find("[type=submit]").Click();

            this.authenticationService.Verify(x => x.Logout(), Times.Once);
        }
    }
}
