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

    using Microsoft.AspNetCore.Http;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Client.Enumerator;
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

        [Test]
        public async Task VerifyReadAnnexC3File()
        {
            this.fileService.Setup(x => x.GetTempFolder()).Returns(TestContext.CurrentContext.TestDirectory);
            var formFile = new Mock<IFormFile>();

            var cometAuthentication = new CometAuthenticationData()
            {
                UserName = "adminn",
                Password = "pass"
            };

            var readResult = await this.cometService.ReadAnnexC3File(formFile.Object, cometAuthentication);

            Assert.That(readResult.Item1, Is.EqualTo(AuthenticationStatus.ServerFailure));

            var filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "26699a14-79d0-4f10-bcf1-15979ef3968f.zip");
            var fileToDelete = string.Empty;

            this.fileService.Setup(x => x.WriteToFile(It.IsAny<IFormFile>(), It.IsAny<string>()))
                .Callback((IFormFile _, string targetPath) =>
                {
                    File.Copy(filePath, targetPath);
                    fileToDelete = targetPath;
                });

            readResult = await this.cometService.ReadAnnexC3File(formFile.Object, cometAuthentication);
            Assert.That(readResult.Item1, Is.EqualTo(AuthenticationStatus.Fail));

            File.Delete(fileToDelete);

            cometAuthentication.UserName = "admin";
            readResult = await this.cometService.ReadAnnexC3File(formFile.Object, cometAuthentication);
            Assert.That(readResult.Item1, Is.EqualTo(AuthenticationStatus.Success));
            var session = Guid.Parse(fileToDelete.Split('.')[0]);

            var engineeringModels = this.cometService.GetAvailableEngineeringModel(session).ToList();
            Assert.That(engineeringModels, Is.Not.Empty);

            var iterationId = engineeringModels.First().IterationSetup.First(x => x.FrozenOn != null).IterationIid;
            var iterationSetup = this.cometService.GetIterationSetup(session, engineeringModels.First().Iid, iterationId);
            var iteration = await this.cometService.ReadIteration(session, iterationSetup);
            var newAnnexC3File = await this.cometService.CreateAnnexC3File(iteration);

            Assert.Multiple(() =>
            {
                Assert.That(File.Exists(newAnnexC3File), Is.True);
                Assert.That(iteration, Is.Not.Null);
                Assert.That(async () => await this.cometService.Close(session), Throws.Nothing);
            });

            File.Delete(fileToDelete);
            File.Delete(newAnnexC3File);
        }
    }
}
