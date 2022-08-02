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

    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Client.Components;
    using UI_DSM.Client.Enumerator;
    using UI_DSM.Client.Services.AuthenticationService;
    using UI_DSM.Client.Services.AuthenticationService.Dto;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class LoginTestFixture
    {
        private TestContext context;
        private Mock<IAuthenticationService> authenticationService;

        [SetUp]
        public void Setup()
        {
            this.authenticationService = new Mock<IAuthenticationService>();

            this.context = new TestContext();
            this.context.Services.AddSingleton(this.authenticationService.Object);
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

            var login = renderComponent.Instance;
            login.AuthenticationStatus = AuthenticationStatus.Fail;
            renderComponent.Render();

            Assert.That(renderComponent.Find(".text-danger").TextContent, Is.EqualTo("Login fail."));
            Assert.That(renderComponent.Find("#connectbtn").TextContent, Is.EqualTo("Retry"));

            login.AuthenticationStatus = AuthenticationStatus.ServerFailure;
            renderComponent.Render();

            Assert.That(renderComponent.Find(".text-danger").TextContent, Is.EqualTo("Provided server can not be reached."));
            Assert.That(renderComponent.Find("#connectbtn").TextContent, Is.EqualTo("Retry"));

            login.AuthenticationStatus = AuthenticationStatus.Authenticating;
            renderComponent.Render();

            Assert.That(renderComponent.Find("#connectbtn").TextContent, Is.EqualTo("Connecting"));
        }

        [Test]
        public void VerifyLogin()
        {
            this.authenticationService.Setup(x => x.Login(It.IsAny<AuthenticationDto>()))
                .ReturnsAsync(AuthenticationStatus.Success);

            var renderComponent = this.context.RenderComponent<Login>();
            var loginButton = renderComponent.Find("#connectbtn");
            
            loginButton.Click();

            renderComponent.Find("#sourceAddress").Change("https://test.com");
            renderComponent.Find("#username").Change("a");
            renderComponent.Find("#password").Change("b");

            loginButton.Click();

            this.authenticationService.Verify(x => x.Login(It.IsAny<AuthenticationDto>()), Times.Once());
        }
    }
}
