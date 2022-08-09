// --------------------------------------------------------------------------------------------------------
// <copyright file="ErrorMessageTestFixture.cs" company="RHEA System S.A.">
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
    using System.Reactive.Concurrency;

    using Bunit;

    using DynamicData;

    using NUnit.Framework;

    using ReactiveUI;

    using UI_DSM.Client.Components;
    using UI_DSM.Client.ViewModels.Components;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class ErrorMessageTestFixture
    {
        private TestContext context;
        private IErrorMessageViewModel errorMessageViewModel;

        [SetUp]
        public void Setup()
        {
            RxApp.MainThreadScheduler = Scheduler.Immediate;
            this.context = new TestContext();
            this.errorMessageViewModel = new ErrorMessageViewModel();
        }

        [TearDown]
        public void Teardown()
        {
            this.context.Dispose();
        }

        [Test]
        public void VerifyRenderErrors()
        {
            var renderer = this.context.RenderComponent<ErrorMessage>(parameters => parameters
                .Add(p => p.ViewModel, this.errorMessageViewModel));

            Assert.That(renderer.Nodes, Is.Empty);

            this.errorMessageViewModel.Errors.Add("The password must contains at least 8 characters");

            Assert.That(renderer.Nodes, Is.Not.Empty);
            Assert.That(renderer.FindAll("li").Count, Is.EqualTo(1));
        }
    }
}
