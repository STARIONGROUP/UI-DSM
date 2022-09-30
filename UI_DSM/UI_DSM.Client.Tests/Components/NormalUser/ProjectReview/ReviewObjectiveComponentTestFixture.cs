// --------------------------------------------------------------------------------------------------------
// <copyright file="ProjectCreationTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Components.NormalUser.ProjectReview
{
    using AppComponents;
    using Bunit;
    using Bunit.TestDoubles;
    using DevExpress.Blazor;


    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;

    using UI_DSM.Client.Components.NormalUser.ProjectReview;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.Components;
    using UI_DSM.Client.ViewModels.Components.NormalUser.ProjectReview;
    using UI_DSM.Shared.Models;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class ReviewObjectiveComponentTestFixture
    {
        private TestContext context;
        private IReviewObjectiveViewModel viewModel;
        private IErrorMessageViewModel errorMessage;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.errorMessage = new ErrorMessageViewModel();
            this.context.ConfigureDevExpressBlazor();
            this.viewModel = new ReviewObjectiveViewModel(null)
            {
                Review = new Review()
            };
            this.viewModel.NavigationManager = this.context.Services.GetRequiredService<FakeNavigationManager>();
        }

        [TearDown]
        public void TearDown()
        {
            this.context.CleanContext();
        }

        [Test]
        public async Task VerifyComponent()
        {
            var renderer = this.context.RenderComponent<ReviewObjectiveComponent>(parameters =>
            {
                parameters.Add(p => p.ViewModel, this.viewModel);
                parameters.AddCascadingValue(this.errorMessage);
            });

            var reviewObjectiveItem = renderer.FindComponents<AppObjectiveItem>();
            Assert.That(reviewObjectiveItem.Count(), Is.EqualTo(0));

            this.viewModel.Review.Title = "Review1";
            this.viewModel.Review.ReviewObjectives.Add(new ReviewObjective(Guid.NewGuid()));

            renderer.Render();
            var reviewObjectiveItem1 = renderer.FindComponents<AppObjectiveItem>();
            Assert.That(reviewObjectiveItem1.Count, Is.EqualTo(1));
        }
    }
}
