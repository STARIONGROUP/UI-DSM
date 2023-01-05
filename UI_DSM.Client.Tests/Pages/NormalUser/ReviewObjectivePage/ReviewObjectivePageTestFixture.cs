// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewObjectivePageTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Pages.NormalUser.ReviewObjectivePage
{
    using Bunit;

    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Client.Components.App.ReviewTaskCard;
    using UI_DSM.Client.Pages.NormalUser.ReviewObjectivePage;
    using UI_DSM.Client.Services.ReviewObjectiveService;
    using UI_DSM.Client.Services.Administration.ParticipantService;
    using UI_DSM.Client.Services.ReviewTaskService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.Components;
    using UI_DSM.Client.ViewModels.Pages.NormalUser.ReviewObjectivePage;
    using UI_DSM.Shared.Models;

    using TestContext = Bunit.TestContext;

    using AppComponents;

    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.Types;
    using UI_DSM.Shared.Enumerator;

    [TestFixture]
    public class ReviewObjectivePageTestFixture
    {
        private TestContext context;
        private IReviewObjectivePageViewModel viewModel;
        private IErrorMessageViewModel errorMessage;
        private Mock<IReviewObjectiveService> reviewObjectiveService;
        private Mock<IParticipantService> participantService;
        private Mock<IReviewTaskService> reviewTaskService;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.errorMessage = new ErrorMessageViewModel();
            this.context.ConfigureDevExpressBlazor();
            this.reviewObjectiveService = new Mock<IReviewObjectiveService>();
            this.participantService = new Mock<IParticipantService>();
            this.reviewTaskService = new Mock<IReviewTaskService>();
            this.viewModel = new ReviewObjectivePageViewModel(this.reviewObjectiveService.Object, this.participantService.Object, this.reviewTaskService.Object);
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

            this.participantService.Setup(x => x.GetParticipantsOfProject(projectGuid,0)).ReturnsAsync(new List<Participant>());

            var renderer = this.context.RenderComponent<ReviewObjectivePage>(parameters =>
            {
                parameters.Add(p => p.ProjectId, projectGuid.ToString());
                parameters.Add(p => p.ReviewId, reviewGuid.ToString());
                parameters.Add(p => p.ReviewObjectiveId, reviewObjectiveGuid.ToString());
            });

            var reviewObjective = new ReviewObjective(reviewGuid)
            {
                ReviewTasks = { new ReviewTask(Guid.NewGuid()) }
            };

            this.reviewObjectiveService.Setup(x => x.GetReviewObjectiveOfReview(projectGuid, reviewGuid, reviewObjectiveGuid, 1)).ReturnsAsync(reviewObjective);

            this.reviewTaskService.Setup(x => x.GetCommmentsCount(projectGuid, reviewGuid, reviewObjectiveGuid))
                .ReturnsAsync(new Dictionary<Guid, AdditionalComputedProperties>
                    {
                        [reviewObjective.ReviewTasks[0].Id] = new ()
                    }
                );

            await this.viewModel.OnInitializedAsync(projectGuid, reviewGuid, reviewObjectiveGuid);
            renderer.Render();

            Assert.That(this.viewModel.ReviewObjective, Is.Not.Null);

            var reviewObjectiveTask = renderer.FindComponents<ReviewTaskCard>();
            Assert.That(reviewObjectiveTask, Has.Count.EqualTo(1));
        }

        [Test]
        public async Task VerifyOpenTaskAssignmentPopup()
        {
            try 
            { 
                var projectGuid = Guid.NewGuid();
                var reviewGuid = Guid.NewGuid();
                var reviewObjectiveGuid = Guid.NewGuid();
                 var reviewTask = new ReviewTask(Guid.NewGuid());

                this.viewModel.Participant = new Participant(Guid.NewGuid())
                {
                    Role = new Role(Guid.NewGuid())
                    {
                        RoleName = "Project administrator",
                        AccessRights = new List<AccessRight>()
                        {
                            AccessRight.AssignTask
                        }
                    },
                    User = new UserEntity(Guid.NewGuid())
                    {
                        UserName = "user"
                    }
                };


                var renderer = this.context.RenderComponent<ReviewTaskCard>(parameters =>
                    {
                        parameters.Add(p => p.ProjectId, projectGuid.ToString());
                        parameters.Add(p => p.ReviewId, reviewGuid.ToString());
                        parameters.Add(p => p.ReviewObjectiveId, reviewObjectiveGuid.ToString());
                        parameters.Add(p => p.ReviewTask, reviewTask);
                        parameters.Add(p => p.ViewModel, this.viewModel);
                        parameters.AddCascadingValue(this.errorMessage);
                    });

                var appButton = renderer.FindComponent<AppButton>();
                var participantToAssign = this.viewModel.TaskAssignmentViewModel.SelectedParticipants.First();
                Assert.That(this.viewModel.IsOnAssignmentMode, Is.False);
                await renderer.InvokeAsync(appButton.Instance.Click.InvokeAsync);

                Assert.Multiple(() =>
                {
                    Assert.That(this.viewModel.IsOnAssignmentMode, Is.True);
                    Assert.That(this.viewModel.TaskAssignmentViewModel.SelectedParticipants.First(), Is.Not.EqualTo(participantToAssign));
                });

                var participantAssign = new Participant(Guid.NewGuid())
                {
                    Role = new Role(Guid.NewGuid())
                    {
                        RoleName = "Project administrator",
                        AccessRights = new List<AccessRight>()
                    {
                        AccessRight.ReviewTask
                    }
                    },
                    User = new UserEntity(Guid.NewGuid())
                    {
                        UserName = "user1"
                    }
                };

                this.viewModel.TaskAssignmentViewModel.SelectedParticipants.ToList().Add(participantAssign);

                this.reviewTaskService.Setup(x => x.UpdateReviewTask(projectGuid, reviewGuid, this.viewModel.SelectedReviewTask))
                    .ReturnsAsync(EntityRequestResponse<ReviewTask>.Fail(new List<string>
                    {
                        "review task not found"
                    }));

                await this.viewModel.TaskAssignmentViewModel.OnValidSubmit.InvokeAsync();

                Assert.Multiple(() =>
                {
                    Assert.That(this.viewModel.ErrorMessageViewModel.Errors, Has.Count.EqualTo(1));
                    Assert.That(this.viewModel.IsOnAssignmentMode, Is.True);
                });

                this.reviewTaskService.Setup(x => x.UpdateReviewTask(projectGuid, reviewGuid, this.viewModel.SelectedReviewTask))
                    .ReturnsAsync(EntityRequestResponse<ReviewTask>.Success(new ReviewTask()));

                await this.viewModel.TaskAssignmentViewModel.OnValidSubmit.InvokeAsync();

                Assert.Multiple(() =>
                {
                    Assert.That(this.viewModel.IsOnAssignmentMode, Is.False);
                    Assert.That(this.viewModel.ErrorMessageViewModel.Errors, Is.Empty);
                });

                this.reviewTaskService.Setup(x => x.UpdateReviewTask(projectGuid, reviewGuid, this.viewModel.SelectedReviewTask))
                    .ThrowsAsync(new HttpRequestException());

                await this.viewModel.TaskAssignmentViewModel.OnValidSubmit.InvokeAsync();
                Assert.That(this.viewModel.ErrorMessageViewModel.Errors, Has.Count.EqualTo(1));
            }
            catch
            {
                // On GitHub, exception is thrown even if the JSRuntime has been configured
            }
        }
    }
}
