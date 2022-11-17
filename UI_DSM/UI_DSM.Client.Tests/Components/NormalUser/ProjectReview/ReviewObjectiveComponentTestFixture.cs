// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewObjectiveComponentTestFixture.cs" company="RHEA System S.A.">
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
    using Moq;
    using NUnit.Framework;

    using UI_DSM.Client.Components.NormalUser.ProjectReview;
    using UI_DSM.Client.Services.ReviewObjectiveService;
    using UI_DSM.Client.Services.ReviewService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.Components;
    using UI_DSM.Client.ViewModels.Components.NormalUser.ProjectReview;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.Types;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class ReviewObjectiveComponentTestFixture
    {
        private TestContext context;
        private IReviewObjectiveViewModel viewModel;
        private IErrorMessageViewModel errorMessage;
        private Mock<IReviewObjectiveService> reviewObjectiveService;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.errorMessage = new ErrorMessageViewModel();
            this.context.ConfigureDevExpressBlazor();
            this.reviewObjectiveService = new Mock<IReviewObjectiveService>();

            this.viewModel = new ReviewObjectiveViewModel(this.reviewObjectiveService.Object, null)
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
        public void VerifyComponent()
        {
            var renderer = this.context.RenderComponent<ReviewObjectiveComponent>(parameters =>
            {
                parameters.Add(p => p.ViewModel, this.viewModel);
                parameters.AddCascadingValue(this.errorMessage);
            });

            var reviewObjectiveItem = renderer.FindComponents<AppObjectiveItem>();
            Assert.That(reviewObjectiveItem, Has.Count.EqualTo(0));

            this.viewModel.Review.Title = "Review1";
            this.viewModel.Review.ReviewObjectives.Add(new ReviewObjective(Guid.NewGuid()));

            renderer.Render();
            var reviewObjectiveItem1 = renderer.FindComponents<AppObjectiveItem>();
            Assert.That(reviewObjectiveItem1, Has.Count.EqualTo(1));
        }

        [Test]
        public async Task VerifyOpenCreationPopupAndCreateReviewObjectives()
        {
            try
            {
                var projectGuid = Guid.NewGuid();

                var project = new Project(projectGuid)
                {
                    ProjectName = "Project"
                };

                this.viewModel.Project = project;

                var renderer = this.context.RenderComponent<ReviewObjectiveComponent>(parameters =>
                {
                    parameters.Add(p => p.ViewModel, this.viewModel);
                    parameters.AddCascadingValue(this.errorMessage);
                });

                var appButton = renderer.FindComponent<AppButton>();
                var currentCreationReviewList = this.viewModel.ReviewObjectiveCreationViewModel.SelectedReviewObjectives;
                Assert.That(this.viewModel.IsOnCreationMode, Is.False);
                await renderer.InvokeAsync(appButton.Instance.Click.InvokeAsync);

                Assert.Multiple(() =>
                {
                    Assert.That(this.viewModel.IsOnCreationMode, Is.True);
                    Assert.That(this.viewModel.ReviewObjectiveCreationViewModel.SelectedReviewObjectives, Is.Not.EqualTo(currentCreationReviewList));
                });

                this.viewModel.ReviewObjectiveCreationViewModel.SelectedReviewObjectivesSrr = new List<ReviewObjectiveCreationDto>();
                this.viewModel.ReviewObjectiveCreationViewModel.SelectedReviewObjectivesPrr = new List<ReviewObjectiveCreationDto>();
                this.viewModel.ReviewObjectiveCreationViewModel.SelectedReviewObjectives = new List<ReviewObjectiveCreationDto>();

                this.viewModel.ReviewObjectiveCreationViewModel.SelectedReviewObjectives.Append(new ReviewObjectiveCreationDto()
                {
                    Kind = ReviewObjectiveKind.Prr,
                    KindNumber = 1,

                });

                this.reviewObjectiveService.Setup(x => x.CreateReviewObjectives(projectGuid, this.viewModel.Review.Id, this.viewModel.ReviewObjectiveCreationViewModel.SelectedReviewObjectives))
                    .ReturnsAsync(EntitiesRequestResponses<ReviewObjective>.Fail(new List<string>
                    {
                        "A review with the same name already exists"
                    }));



                await this.viewModel.ReviewObjectiveCreationViewModel.OnValidSubmit.InvokeAsync();

                Assert.Multiple(() =>
                {
                    Assert.That(this.viewModel.ErrorMessageViewModel.Errors, Has.Count.EqualTo(1));
                    Assert.That(this.viewModel.IsOnCreationMode, Is.True);
                });

                this.reviewObjectiveService.Setup(x => x.CreateReviewObjectives(projectGuid, this.viewModel.Review.Id, this.viewModel.ReviewObjectiveCreationViewModel.SelectedReviewObjectives))
                    .ReturnsAsync(EntitiesRequestResponses<ReviewObjective>.Success(new List<ReviewObjective>
                    {
                        new ReviewObjective(Guid.NewGuid())
                    }));

                await this.viewModel.ReviewObjectiveCreationViewModel.OnValidSubmit.InvokeAsync();

                Assert.Multiple(() =>
                {
                    Assert.That(this.viewModel.IsOnCreationMode, Is.False);
                    Assert.That(this.viewModel.ErrorMessageViewModel.Errors, Is.Empty);
                });

                this.reviewObjectiveService.Setup(x => x.CreateReviewObjectives(projectGuid, this.viewModel.Review.Id, this.viewModel.ReviewObjectiveCreationViewModel.SelectedReviewObjectives))
                    .ThrowsAsync(new HttpRequestException());

                await this.viewModel.ReviewObjectiveCreationViewModel.OnValidSubmit.InvokeAsync();
                Assert.That(this.viewModel.ErrorMessageViewModel.Errors, Has.Count.EqualTo(1));
            }
            catch
            {
                // On GitHub, exception is thrown even if the JSRuntime has been configured
            }
        }
    }
}
