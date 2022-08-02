// --------------------------------------------------------------------------------------------------------
// <copyright file="CometSessionServiceTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Services.CometSessionService
{
    using CDP4Dal;
    using CDP4Dal.DAL;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Client.Services.CometSessionService;

    [TestFixture]
    public class CometSessionServiceTestFixture
    {
        private CometSessionService service;
        private Mock<ISession> session;

        [SetUp]
        public void Setup()
        {
            this.session = new Mock<ISession>();

            this.service = new CometSessionService
            {
                Session = this.session.Object
            };
        }

        [Test]
        public void VerifyProperties()
        {
            Assert.That(this.service.IsSessionOpen, Is.False);
            Assert.That(this.service.Session, Is.EqualTo(this.session.Object));
        }

        [Test]
        public void VerifyOpenSession()
        {
            Assert.That(async () => await this.service.Open(new Credentials("a", "b", new Uri("http://test.com"))), Throws.Exception);
        }

        [Test]
        public void VerifyClose()
        {
            this.session.Setup(x => x.Close()).Returns(Task.CompletedTask);
            Assert.That(() => this.service.Close(), Throws.Nothing);

            this.service.Session = null;
            Assert.That(() => this.service.Close(), Throws.Nothing);

            this.session.Verify(x => x.Close(), Times.Once);
        }
    }
}
