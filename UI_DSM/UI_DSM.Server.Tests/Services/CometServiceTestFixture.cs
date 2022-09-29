// --------------------------------------------------------------------------------------------------------
// <copyright file="CometServiceTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Tests.Services
{
    using CDP4Common.SiteDirectoryData;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Server.Services.CometService;
    using UI_DSM.Server.Services.FileService;
    using UI_DSM.Shared.DTO.CometData;

    [TestFixture]
    public class CometServiceTestFixture
    {
        private CometService cometService;
        private Mock<IFileService> fileService;

        [SetUp]
        public void Setup()
        {
            this.fileService = new Mock<IFileService>();
            this.cometService = new CometService(this.fileService.Object);
        }

        [Test]
        public void VerifyLogin()
        {
            var authenticationData = new CometAuthenticationData()
            {
                Url = "http://test.abc:5000",
                UserName = "admin",
                Password = "pass"
            };

            Assert.That(async () => await this.cometService.Login(authenticationData), Throws.Nothing);
        }

        [Test]
        public void VerifyThatOtherMethodsThrows()
        {
            var sessionId = Guid.NewGuid();

            Assert.Multiple( () => 
            {
                Assert.That(async () => await this.cometService.Close(sessionId), Throws.Exception);
                Assert.That(async () => await this.cometService.ReadIteration(sessionId, new IterationSetup()), Throws.Exception);
                Assert.That(() => this.cometService.GetIterationSetup(sessionId, Guid.NewGuid(), Guid.NewGuid()), Throws.Exception);
                Assert.That(() => this.cometService.GetAvailableEngineeringModel(sessionId), Throws.Exception);
            });
        }
    }
}
