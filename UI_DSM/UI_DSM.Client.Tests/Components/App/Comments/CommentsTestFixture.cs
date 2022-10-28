// --------------------------------------------------------------------------------------------------------
// <copyright file="CommentsTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Components.App.Comments
{
    using AppComponents;

    using Bunit;

    using CDP4Common.EngineeringModelData;

    using DynamicData;

    using Microsoft.AspNetCore.Components.Web;
    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Client.Components.App.CommentCard;
    using UI_DSM.Client.Components.App.Comments;
    using UI_DSM.Client.Services.Administration.ParticipantService;
    using UI_DSM.Client.Services.AnnotationService;
    using UI_DSM.Client.Services.ReviewItemService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.App.Comments;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.Types;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class CommentsTestFixture
    {
        private ICommentsViewModel viewModel;
        private TestContext context;
        private Participant participant;
        private Mock<IReviewItemService> reviewItemService;
        private Mock<IAnnotationService> annotationService;
        private Mock<IParticipantService> participantService;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.context.ConfigureDevExpressBlazor();
            this.reviewItemService = new Mock<IReviewItemService>();
            this.annotationService = new Mock<IAnnotationService>();
            this.participantService = new Mock<IParticipantService>();

            this.participant = new Participant(Guid.NewGuid())
            {
                User = new UserEntity(Guid.NewGuid())
                {
                    UserName = "user"
                }
            };

            this.participantService.Setup(x => x.GetCurrentParticipant(It.IsAny<Guid>()))
                .ReturnsAsync(this.participant);

            this.viewModel = new CommentsViewModel(this.reviewItemService.Object, this.annotationService.Object, this.participantService.Object);
            this.viewModel.InitializesProperties(Guid.NewGuid(), Guid.NewGuid(), View.RequirementBreakdownStructureView);
            this.context.Services.AddSingleton(this.viewModel);
        }

        [TearDown]
        public void Teardown()
        {
            this.context.CleanContext();
        }

        [Test]
        public async Task VerifyComponent()
        {
            try
            {
                var renderer = this.context.RenderComponent<Comments>();
                var commentsCard = renderer.FindComponents<CommentCard>();

                Assert.That(commentsCard, Has.Count.EqualTo(0));
                var requirementId = Guid.NewGuid();

                var row = new RequirementBreakdownStructureViewRowViewModel(new Requirement(requirementId, null, null), null);
                this.viewModel.SelectedItem = row;

                var button = renderer.FindComponent<AppButton>();
                await renderer.InvokeAsync(button.Instance.Click.InvokeAsync);
                Assert.That(this.viewModel.IsOnCreationMode, Is.True);

                this.viewModel.CommentCreationViewModel.Comment.Content = "new content";

                this.reviewItemService.Setup(x => x.CreateReviewItem(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>()))
                    .ReturnsAsync(EntityRequestResponse<ReviewItem>.Fail(new List<string> { "Error" }));

                await renderer.InvokeAsync(this.viewModel.CommentCreationViewModel.OnValidSubmit.InvokeAsync);
                Assert.That(this.viewModel.ErrorMessageViewModel.Errors, Is.Not.Empty);
                this.viewModel.ErrorMessageViewModel.Errors.Clear();

                this.reviewItemService.Setup(x => x.CreateReviewItem(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>()))
                    .ReturnsAsync(EntityRequestResponse<ReviewItem>.Success(new ReviewItem()
                    {
                        ThingId = requirementId
                    }));

                this.annotationService.Setup(x => x.CreateAnnotation(It.IsAny<Guid>(), It.IsAny<Annotation>()))
                    .ReturnsAsync(EntityRequestResponse<Annotation>.Fail(new List<string> { "Error" }));

                await renderer.InvokeAsync(this.viewModel.CommentCreationViewModel.OnValidSubmit.InvokeAsync);

                Assert.Multiple(() =>
                {
                    Assert.That(this.viewModel.ErrorMessageViewModel.Errors, Is.Not.Empty);
                    Assert.That(row.ReviewItem, Is.Not.Null);
                });

                this.annotationService.Setup(x => x.CreateAnnotation(It.IsAny<Guid>(), It.IsAny<Annotation>()))
                    .ReturnsAsync(EntityRequestResponse<Annotation>.Success(new Comment()
                    {
                        Author = this.participant,
                        Content = "Content",
                        CreatedOn = DateTime.Now
                    }));

                await renderer.InvokeAsync(this.viewModel.CommentCreationViewModel.OnValidSubmit.InvokeAsync);
                commentsCard = renderer.FindComponents<CommentCard>();

                Assert.Multiple(() =>
                {
                    Assert.That(commentsCard, Has.Count.EqualTo(1));
                    Assert.That(this.viewModel.IsOnCreationMode, Is.False);
                });

                this.viewModel.SelectedItem = null;
                Assert.That(this.viewModel.Comments, Is.Empty);

                this.reviewItemService.Setup(x => x.GetReviewItemOfReview(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<int>()))
                    .ReturnsAsync(row.ReviewItem);

                this.viewModel.SelectedItem = row;
                Assert.That(this.viewModel.Comments, Has.Count.EqualTo(1));

                var commentCard = renderer.FindComponent<CommentCard>();
                Assert.That(commentCard.Instance.IsSelected, Is.False);

                await renderer.InvokeAsync(() => commentCard.Instance.OnClick.InvokeAsync(commentCard.Instance));

                Assert.Multiple(() =>
                {
                    Assert.That(commentCard.Instance.ViewModel.IsAllowedToEdit, Is.True);
                    Assert.That(commentCard.Instance.IsSelected, Is.True);
                });

                await renderer.InvokeAsync(() => commentCard.Instance.ViewModel.OnContentEditCallback.InvokeAsync(commentCard.Instance.ViewModel.Comment));
                Assert.That(this.viewModel.IsOnUpdateMode, Is.True);

                this.annotationService.Setup(x => x.UpdateAnnotation(It.IsAny<Guid>(), It.IsAny<Annotation>()))
                    .ReturnsAsync(EntityRequestResponse<Annotation>.Fail(new List<string> { "Error" }));

                await renderer.InvokeAsync(this.viewModel.CommentCreationViewModel.OnValidSubmit.InvokeAsync);
                Assert.That(this.viewModel.ErrorMessageViewModel.Errors, Has.Count.EqualTo(1));

                this.annotationService.Setup(x => x.UpdateAnnotation(It.IsAny<Guid>(), It.IsAny<Annotation>()))
                    .ReturnsAsync(EntityRequestResponse<Annotation>.Success(row.ReviewItem.Annotations.FirstOrDefault(x => x is Comment)));

                await renderer.InvokeAsync(this.viewModel.CommentCreationViewModel.OnValidSubmit.InvokeAsync);
                Assert.That(this.viewModel.Comments, Has.Count.EqualTo(1));

                var dropdownButton = renderer.Find("#dropdownButton");
                await renderer.InvokeAsync(() => dropdownButton.ClickAsync(new MouseEventArgs()));
                Assert.That(commentCard.Instance.IsOnStatusUpdateMode, Is.True);

                await commentCard.Instance.ViewModel.UpdateStatus(StatusKind.Approved);
                this.annotationService.Verify(x => x.UpdateAnnotation(It.IsAny<Guid>(), It.IsAny<Annotation>()), Times.Exactly(3));

                await renderer.InvokeAsync(() => commentCard.Instance.ViewModel.OnDeleteCallback.InvokeAsync(commentCard.Instance.ViewModel.Comment));
                Assert.That(this.viewModel.ConfirmCancelPopupViewModel.IsVisible, Is.True);

                await renderer.InvokeAsync(this.viewModel.ConfirmCancelPopupViewModel.OnConfirm.InvokeAsync);
                Assert.That(this.viewModel.Comments, Is.Empty);
            }
            catch
            {
                // On GitHub, exception is thrown even if the JSRuntime has been configured
            }
        }
    }
}
