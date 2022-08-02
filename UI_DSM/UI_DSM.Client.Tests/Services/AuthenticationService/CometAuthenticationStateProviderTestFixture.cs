// --------------------------------------------------------------------------------------------------------
// <copyright file="CometAuthenticationStateProviderTestFixture.cs" company="RHEA System S.A.">
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
    using CDP4Common.SiteDirectoryData;

    using CDP4Dal;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Client.Services.AuthenticationService;
    using UI_DSM.Client.Services.CometSessionService;

    [TestFixture]
    public class CometAuthenticationStateProviderTestFixture
    {
        private CometAuthenticationStateProvider stateProvider;
        private Mock<ICometSessionService> cometSessionService;

        [SetUp]
        public void Setup()
        {
            this.cometSessionService = new Mock<ICometSessionService>();
            this.cometSessionService.Setup(x => x.IsSessionOpen).Returns(true);

            this.stateProvider = new CometAuthenticationStateProvider(this.cometSessionService.Object);
        }

        [Test]
        public void VerifyGetAuthenticationState()
        {
            this.cometSessionService.Setup(x => x.IsSessionOpen).Returns(false);
            Assert.That(this.stateProvider.GetAuthenticationStateAsync().Result.User.Identity?.IsAuthenticated, Is.False);

            this.cometSessionService.Setup(x => x.IsSessionOpen).Returns(true);
            var session = new Mock<ISession>();
            this.cometSessionService.Setup(x => x.Session).Returns(session.Object);

            var person = new Person();
            session.Setup(x => x.ActivePerson).Returns(person);
            Assert.That(this.stateProvider.GetAuthenticationStateAsync().Result.User.Identity?.IsAuthenticated, Is.False);

            person.GivenName = "Smith";
            person.IsActive = false;
            Assert.That(this.stateProvider.GetAuthenticationStateAsync().Result.User.Identity?.IsAuthenticated, Is.False);

            person.IsActive = true;
            person.IsDeprecated = true;
            Assert.That(this.stateProvider.GetAuthenticationStateAsync().Result.User.Identity?.IsAuthenticated, Is.False);

            person.IsDeprecated = false;
            Assert.That(this.stateProvider.GetAuthenticationStateAsync().Result.User.Identity?.IsAuthenticated, Is.True);
        }
    }
}
