// --------------------------------------------------------------------------------------------------------
// <copyright file="ProjectReviewTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Components.NormalUser.ProjectReview
{
    using AppComponents;

    using Bunit;
    using Bunit.TestDoubles;

    using Microsoft.Extensions.DependencyInjection;
    
    using NUnit.Framework;

    using UI_DSM.Client.Components.NormalUser.ProjectReview;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.Components;
    using UI_DSM.Client.ViewModels.Components.NormalUser.ProjectReview;
    using UI_DSM.Shared.Models;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class ProjectReviewTestFixture
    {
        private TestContext context;
        private IProjectReviewViewModel viewModel;
        private IErrorMessageViewModel errorMessage;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.errorMessage = new ErrorMessageViewModel();
            this.context.ConfigureDevExpressBlazor();

            this.viewModel = new ProjectReviewViewModel(null,null,null)
            {
                Project = new Project()
            };

            this.viewModel.NavigationManager = this.context.Services.GetRequiredService<FakeNavigationManager>();
        }

        [TearDown]
        public void TearDown()
        {
            this.context.CleanContext();
        }

        [Test]
        public void VerifyComponent()
        {
            var renderer = this.context.RenderComponent<ProjectReview>(parameters =>
            {
                parameters.Add(p => p.ViewModel, this.viewModel);
                parameters.AddCascadingValue(this.errorMessage);
            });

            var projectReviewsCard = renderer.FindComponents<AppProjectCard>();
            Assert.That(projectReviewsCard, Has.Count.EqualTo(0));

            this.viewModel.Project.ProjectName = "Project";
            this.viewModel.Project.Reviews.Add(new Review(Guid.NewGuid()));
            
            renderer.Render();
            var projectReviewsCard1 = renderer.FindComponents<AppProjectCard>();
            Assert.That(projectReviewsCard1, Has.Count.EqualTo(1));
        }
    }
}
