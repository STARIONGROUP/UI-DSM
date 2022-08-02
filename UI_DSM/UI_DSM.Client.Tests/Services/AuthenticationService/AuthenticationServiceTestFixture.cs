// --------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationServiceTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Services.AuthenticationService
{
    using CDP4Dal.DAL;
    using CDP4Dal.Exceptions;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Client.Enumerator;
    using UI_DSM.Client.Services.AuthenticationService;
    using UI_DSM.Client.Services.AuthenticationService.Dto;
    using UI_DSM.Client.Services.CometSessionService;

    [TestFixture]
    public class AuthenticationServiceTestFixture
    {
        private AuthenticationService service;
        private Mock<ICometSessionService> cometSessionService;

        [SetUp]
        public void Setup()
        {
            this.cometSessionService = new Mock<ICometSessionService>();

            this.service = new AuthenticationService(this.cometSessionService.Object);
        }

        [Test]
        public void VerifyLogin()
        {
            var authentication = new AuthenticationDto();

            Assert.That(this.service.Login(authentication).Result, Is.EqualTo(AuthenticationStatus.Fail));

            this.cometSessionService.Setup(x => x.Open(It.IsAny<Credentials>()))
                .ReturnsAsync(false);

            authentication.SourceAddress = "http://test.com";
            authentication.UserName = "a";
            authentication.Password = "b";
            Assert.That(this.service.Login(authentication).Result, Is.EqualTo(AuthenticationStatus.Fail));

            this.cometSessionService.Setup(x => x.Open(It.IsAny<Credentials>()))
                .ReturnsAsync(true);

            Assert.That(this.service.Login(authentication).Result, Is.EqualTo(AuthenticationStatus.Success));

            this.cometSessionService.Setup(x => x.Close()).Returns(Task.CompletedTask);

            this.cometSessionService.Setup(x => x.Open(It.IsAny<Credentials>()))
                .ThrowsAsync(new DalReadException());

            Assert.That(this.service.Login(authentication).Result, Is.EqualTo(AuthenticationStatus.Fail));

            this.cometSessionService.Setup(x => x.Open(It.IsAny<Credentials>()))
                .ThrowsAsync(new HttpRequestException());

            Assert.That(this.service.Login(authentication).Result, Is.EqualTo(AuthenticationStatus.ServerFailure));

            this.cometSessionService.Verify(x => x.Close(), Times.Exactly(2));
        }

        [Test]
        public void VerifyLogout()
        {
            this.cometSessionService.Setup(x => x.Close()).Returns(Task.CompletedTask);
            Assert.That(async () => await this.service.Logout(), Throws.Nothing);
            this.cometSessionService.Verify(x => x.Close(), Times.Once);
        }
    }
}
