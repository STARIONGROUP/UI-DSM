// --------------------------------------------------------------------------------------------------------
// <copyright file="NavMenuTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Shared
{
    using Bunit;
    using Bunit.TestDoubles;

    using NUnit.Framework;

    using UI_DSM.Client.Shared;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class NavMenuTestFixture
    {
        private TestContext context;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.context.AddTestAuthorization();
        }

        [TearDown]
        public void TearDown()
        {
            this.context.Dispose();
        }

        [Test]
        public void VerifyToggleNavMenu()
        {
            var renderComponent = this.context.RenderComponent<NavMenu>();
            var toggleButton = renderComponent.Find(".collapse");

            Assert.That(toggleButton, Is.Not.Null);

            toggleButton.Click();
            Assert.That(() => renderComponent.Find(".collapse"), Throws.Exception);
        }
    }
}
