// --------------------------------------------------------------------------------------------------------
// <copyright file="FileServiceTestFixture.cs" company="RHEA System S.A.">
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
    using NUnit.Framework;

    using UI_DSM.Server.Services.FileService;

    [TestFixture]
    public class FileServiceTestFixture
    {
        private FileService fileService;
        private Guid projectId;

        [SetUp]
        public void Setup()
        {
            this.fileService= new FileService(TestContext.CurrentContext.TestDirectory);
            this.projectId = Guid.NewGuid();
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete(this.fileService.GetTempFolder(), true);
            Directory.Delete(Path.Combine(TestContext.CurrentContext.TestDirectory, this.projectId.ToString()), true);
        }

        [Test]
        public void VerifyService()
        {
            const string fileName = "26699a14-79d0-4f10-bcf1-15979ef3968f.zip";

            Assert.Multiple(async () =>
            {
                Assert.That(this.fileService.Exists("Data", fileName), Is.True);
                Assert.That(this.fileService.TempFileExists(fileName), Is.False);
                Assert.That(await this.fileService.IsAnnexC3File(fileName), Is.False);
            });

            File.Copy(Path.Combine("Data", fileName), 
                Path.Combine(this.fileService.GetTempFolder(), fileName));

            Assert.Multiple(async () =>
            {
                Assert.That(this.fileService.TempFileExists(fileName), Is.True);
                Assert.That(await this.fileService.IsAnnexC3File(fileName), Is.True);
            });

            Assert.That(this.fileService.Exists(this.projectId.ToString(), fileName), Is.False);
            this.fileService.MoveFile(fileName, this.projectId.ToString());
            
            Assert.Multiple(() =>
            {
                Assert.That(this.fileService.Exists(this.projectId.ToString(), fileName), Is.True);
                Assert.That(() => this.fileService.DeleteFile(Path.Combine(this.projectId.ToString(), fileName)), Throws.Nothing);
                Assert.That(this.fileService.Exists(this.projectId.ToString(), fileName), Is.False);
                Assert.That(() => this.fileService.DeleteFile(Path.Combine(this.projectId.ToString(), fileName)), Throws.Nothing);
            });

            File.Copy(Path.Combine("Data", fileName),
                Path.Combine(this.fileService.GetTempFolder(), fileName));

            Assert.Multiple(() =>
            {
                Assert.That(this.fileService.TempFileExists(fileName), Is.True);
                Assert.That(() => this.fileService.DeleteTemporaryFile(fileName), Throws.Nothing);
                Assert.That(this.fileService.TempFileExists(fileName), Is.False);
                Assert.That(() => this.fileService.DeleteTemporaryFile(fileName), Throws.Nothing);
            });
        }
    }
}
