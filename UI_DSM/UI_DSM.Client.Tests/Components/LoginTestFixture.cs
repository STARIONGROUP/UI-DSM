// --------------------------------------------------------------------------------------------------------
// <copyright file="LoginTestFixture.cs" company="RHEA System S.A.">
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

    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Client.Components;
    using UI_DSM.Client.Enumerator;
    using UI_DSM.Client.Services.AuthenticationService;
    using UI_DSM.Client.ViewModels.Components;
    using UI_DSM.Shared.DTO.UserManagement;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class LoginTestFixture
    {
        private TestContext context;
        private Mock<IAuthenticationService> authenticationService;
        private ILoginViewModel viewModel;

        [SetUp]
        public void Setup()
        {
            this.authenticationService = new Mock<IAuthenticationService>();
            this.viewModel = new LoginViewModel(this.authenticationService.Object, null);
            this.context = new TestContext();
            this.context.Services.AddSingleton(this.viewModel);
            this.viewModel.NavigationManager = this.context.Services.GetRequiredService<FakeNavigationManager>();
        }

        [TearDown]
        public void TearDown()
        {
            this.context.Dispose();
        }

        [Test]
        public void VerifyAuthenticationStatusStates()
        {
            var renderComponent = this.context.RenderComponent<Login>();

            this.viewModel.AuthenticationStatus = AuthenticationStatus.Fail;
            this.viewModel.ErrorMessage = "Login fail.";
            renderComponent.Render();

            Assert.That(renderComponent.Find(".text-danger").TextContent, Contains.Substring("Login fail."));
            Assert.That(renderComponent.Find("#connectbtn").TextContent, Contains.Substring("Retry login"));

            this.viewModel.AuthenticationStatus = AuthenticationStatus.Authenticating;
            renderComponent.Render();

            Assert.That(renderComponent.Find("#connectbtn").TextContent, Contains.Substring("Processing login"));
        }

        [Test]
        public void VerifyLogin()
        {
            this.authenticationService.Setup(x => x.Login(It.IsAny<AuthenticationDto>()))
                .ReturnsAsync(new AuthenticationResponseDto()
                {
                    IsRequestSuccessful = true
                });

            var renderComponent = this.context.RenderComponent<Login>();
            var loginButton = renderComponent.Find("#connectbtn");
            
            loginButton.Click();

            renderComponent.Find("#username input").Change("a");
            renderComponent.Find("#password input").Change("b");

            loginButton.Click();

            this.authenticationService.Verify(x => x.Login(It.IsAny<AuthenticationDto>()), Times.Once());
        }
    }
}
