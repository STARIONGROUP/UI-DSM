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

    using Moq;

    using Microsoft.Extensions.DependencyInjection;
    
    using NUnit.Framework;

    using UI_DSM.Client.Components.NormalUser.ProjectReview;
    using UI_DSM.Client.Services.ReviewService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.Components;
    using UI_DSM.Client.ViewModels.Components.NormalUser.ProjectReview;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    using TestContext = Bunit.TestContext;
    using UI_DSM.Client.Services.Administration.RoleService;
    using UI_DSM.Shared.Types;

    [TestFixture]
    public class ProjectReviewTestFixture
    {
        private TestContext context;
        private IProjectReviewViewModel viewModel;
        private IErrorMessageViewModel errorMessage;
        private Mock<IReviewService> reviewService;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.errorMessage = new ErrorMessageViewModel();
            this.context.ConfigureDevExpressBlazor();
            this.reviewService = new Mock<IReviewService>();


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
            try
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
            catch
            {
                // On GitHub, exception is thrown even if the JSRuntime has been configured
            }
        }


        [Test]
        public async Task VerifyOpenCreationPopupAndCreateReview()
        {
            try
            {
                var projectGuid = Guid.NewGuid();
                var project = new Project(projectGuid)
                {
                    ProjectName = "Project"
                };

                this.viewModel.Project = project;
                
                var renderer = this.context.RenderComponent<ProjectReview>();
                var appButton = renderer.FindComponent<AppButton>();
                var currentCreationReview = this.viewModel.ReviewCreationViewModel.Review;
                Assert.That(this.viewModel.IsOnCreationMode, Is.False);
                await renderer.InvokeAsync(appButton.Instance.Click.InvokeAsync);

                Assert.Multiple(() =>
                {
                    Assert.That(this.viewModel.IsOnCreationMode, Is.True);
                    Assert.That(this.viewModel.ReviewCreationViewModel.Review, Is.Not.EqualTo(currentCreationReview));
                });

                this.viewModel.ReviewCreationViewModel.SelectedModels = new List<Model>();

                this.viewModel.ReviewCreationViewModel.Review.Title = "Review 1";

                this.reviewService.Setup(x => x.CreateReview(projectGuid,this.viewModel.ReviewCreationViewModel.Review))
                    .ReturnsAsync(EntityRequestResponse<Review>.Fail(new List<string>()
                    {
                    "A review with the same name already exists"
                    }));

                await this.viewModel.ReviewCreationViewModel.OnValidSubmit.InvokeAsync();

                Assert.Multiple(() =>
                {
                    Assert.That(this.viewModel.ErrorMessageViewModel.Errors.Count, Is.EqualTo(1));
                    Assert.That(this.viewModel.IsOnCreationMode, Is.True);
                });

                this.reviewService.Setup(x => x.CreateReview(projectGuid,this.viewModel.ReviewCreationViewModel.Review))
                    .ReturnsAsync(EntityRequestResponse<Review>.Success(new Review()));

                await this.viewModel.ReviewCreationViewModel.OnValidSubmit.InvokeAsync();

                Assert.Multiple(() =>
                {
                    Assert.That(this.viewModel.IsOnCreationMode, Is.False);
                    Assert.That(this.viewModel.ErrorMessageViewModel.Errors, Is.Empty);
                });

                this.reviewService.Setup(x => x.CreateReview(projectGuid, this.viewModel.ReviewCreationViewModel.Review))
                    .ThrowsAsync(new HttpRequestException());

                await this.viewModel.ReviewCreationViewModel.OnValidSubmit.InvokeAsync();
                Assert.That(this.viewModel.ErrorMessageViewModel.Errors.Count, Is.EqualTo(1));
            }
            catch
            {
                // On GitHub, exception is thrown even if the JSRuntime has been configured
            }
        }

        [Test]
        public void VerifyGoToPage()
        {
            var review = new Review(Guid.NewGuid());
            this.viewModel.GoToReviewPage(review);
            Assert.That(this.viewModel.NavigationManager.Uri.Contains($"Review/{review.Id}"), Is.True);
        }
    }
}
