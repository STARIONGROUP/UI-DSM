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

namespace UI_DSM.Client.Tests.Pages.NormalUser.ReviewPage
{
    using Bunit;

    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Client.Components.NormalUser.ProjectReview;
    using UI_DSM.Client.Pages.NormalUser.ReviewPage;
    using UI_DSM.Client.Services.ReviewService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.Components.NormalUser.ProjectReview;
    using UI_DSM.Client.ViewModels.Pages.NormalUser.ReviewPage;
    using UI_DSM.Shared.Models;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class ReviewPageTestFixture
    {
        private TestContext context;
        private IReviewPageViewModel viewModel;
        private IReviewObjectiveViewModel reviewObjectiveViewModel;
        private Mock<IReviewService> reviewService;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.reviewObjectiveViewModel = new ReviewObjectiveViewModel(null);
            this.reviewService = new Mock<IReviewService>();
            this.viewModel = new ReviewPageViewModel(this.reviewService.Object, this.reviewObjectiveViewModel);
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

            var renderer = this.context.RenderComponent<ReviewPage>(parameters =>
            {
                parameters.Add(p => p.ProjectId, projectGuid.ToString());
                parameters.Add(p => p.ReviewId, reviewGuid.ToString());
            });

            var noReviewFound = renderer.Find("div");
            Assert.That(noReviewFound.InnerHtml, Does.Contain("Review not found"));

            var project = new Project(projectGuid)
            {
                ProjectName = "Project",
            };
            
            var review = new Review(reviewGuid)
            {
                Title = "Review",
            };

            this.reviewService.Setup(x => x.GetReviewOfProject(projectGuid, reviewGuid, 1)).ReturnsAsync(review);

            await this.viewModel.OnInitializedAsync(projectGuid, reviewGuid);
            renderer.Render();

            Assert.That(this.viewModel.ReviewObjectiveViewModel.Review, Is.Not.Null);

            var reviewObjectiveComponent = renderer.FindComponents<ReviewObjectiveComponent>();
            Assert.That(reviewObjectiveComponent, Has.Count.EqualTo(1));
        }
    }
}
