// --------------------------------------------------------------------------------------------------------
// <copyright file="ProjectManagementTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Pages.NormalUser.ReviewObjectivePage
{
    using Bunit;

    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Client.Components.NormalUser.ReviewObjective;
    using UI_DSM.Client.Pages.NormalUser.ReviewObjectivePage;
    using UI_DSM.Client.Services.ReviewObjectiveService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.Components.NormalUser.ReviewObjective;
    using UI_DSM.Client.ViewModels.Pages.NormalUser.ReviewObjectivePage;
    using UI_DSM.Shared.Models;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class ReviewObjectiveTestFixture
    {
        private TestContext context;
        private IReviewObjectivePageViewModel viewModel;
        private IReviewObjectiveTasksViewModel reviewObjectiveTasksViewModel;
        private Mock<IReviewObjectiveService> reviewObjectiveService;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.reviewObjectiveTasksViewModel = new ReviewObjectiveTasksViewModel(null);
            this.reviewObjectiveService = new Mock<IReviewObjectiveService>();
            this.viewModel = new ReviewObjectivePageViewModel(this.reviewObjectiveTasksViewModel, this.reviewObjectiveService.Object);
            this.context.Services.AddSingleton(this.viewModel);
        }

        [TearDown]
        public void Teardown()
        {
            this.context.CleanContext();
        }

        [Test]
        public async Task VerifyOnInitialized()
        {
            var projectGuid = Guid.NewGuid();
            var reviewGuid = Guid.NewGuid();
            var reviewObjectiveGuid = Guid.NewGuid();

            var renderer = this.context.RenderComponent<ReviewObjectivePage>(parameters =>
            {
                parameters.Add(p => p.ProjectId, projectGuid.ToString());
                parameters.Add(p => p.ReviewId, reviewGuid.ToString());
                parameters.Add(p => p.ReviewObjectiveId, reviewObjectiveGuid.ToString());
            });

            var noReviewObjectiveFound = renderer.Find("div");
            Assert.That(noReviewObjectiveFound.InnerHtml, Does.Contain("Review Objective not found"));

            var project = new Project(projectGuid)
            {
                ProjectName = "Project",
            };

            var review = new Review(reviewGuid)
            {
                Title = "Review",
            };

            var reviewObjective = new ReviewObjective(reviewGuid)
            {
                Title = "Review Objective",
            };

            this.reviewObjectiveService.Setup(x => x.GetReviewObjectiveOfReview(projectGuid, reviewGuid, reviewObjectiveGuid, 1)).ReturnsAsync(reviewObjective);

            await this.viewModel.OnInitializedAsync(projectGuid, reviewGuid, reviewObjectiveGuid);
            renderer.Render();

            Assert.That(this.viewModel.ReviewObjectiveTasksViewModel.ReviewObjective, Is.Not.Null);

            var reviewObjectiveTask = renderer.FindComponents<ReviewObjectiveTasks>();
            Assert.That(reviewObjectiveTask, Has.Count.EqualTo(1));
        }
    }
}
