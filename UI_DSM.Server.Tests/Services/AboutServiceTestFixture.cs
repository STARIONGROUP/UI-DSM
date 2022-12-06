// --------------------------------------------------------------------------------------------------------
// <copyright file="AboutServiceTestFixture.cs" company="RHEA System S.A.">
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
    using System.Data;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Server.Context;
    using UI_DSM.Server.Services.AboutService;
    using UI_DSM.Shared.DTO.Common;

    [TestFixture]
    public class AboutServiceTestFixture
    {
        private AboutService aboutService;
        private Mock<DatabaseContext> context;

        [SetUp]
        public void Setup()
        {
            this.context = new Mock<DatabaseContext>();
            this.aboutService = new AboutService(DateTime.Now);
        }

        [Test]
        public void VerifyGetInformation()
        {
            this.context.Setup(x => x.GetDatabaseInformation())
                .Returns(new DatabaseInformationDto()
                {
                    State = ConnectionState.Open,
                    DatabaseVersion = "Debian 14",
                    StartTime = DateTime.Now.Subtract(TimeSpan.FromHours(2))
                });

            var information = this.aboutService.GetSystemInformation(this.context.Object);
            Assert.That(information.AssembliesInformation, Is.Not.Empty);
        }
    }
}
