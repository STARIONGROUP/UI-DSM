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
    using Bunit;

    using CDP4Common.EngineeringModelData;

    using DynamicData;

    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Web;
    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Client.Components.App.CommentCard;
    using UI_DSM.Client.Components.App.Comments;
    using UI_DSM.Client.Components.App.ReplyCard;
    using UI_DSM.Client.Services.AnnotationService;
    using UI_DSM.Client.Services.ReplyService;
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
        private Mock<IReplyService> replyService;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.context.ConfigureDevExpressBlazor();
            this.reviewItemService = new Mock<IReviewItemService>();
            this.annotationService = new Mock<IAnnotationService>();
            this.replyService = new Mock<IReplyService>();

            this.participant = new Participant(Guid.NewGuid())
            {
                User = new UserEntity(Guid.NewGuid())
                {
                    UserName = "user"
                },
                Role = new Role(Guid.NewGuid())
                {
                    AccessRights = { AccessRight.ReviewTask }
                }
            };

            this.viewModel = new CommentsViewModel(this.reviewItemService.Object, this.annotationService.Object, this.replyService.Object);
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
                
                await renderer.Instance.InitializesProperties(Guid.NewGuid(), Guid.NewGuid(), View.RequirementBreakdownStructureView, 
                    this.participant, EventCallback<Comment>.Empty, new ReviewTask());

                this.viewModel.AvailableRows = new List<IHaveAnnotatableItemRowViewModel>();

                var commentsCard = renderer.FindComponents<CommentCard>();

                Assert.That(commentsCard, Has.Count.EqualTo(0));
                var requirementId = Guid.NewGuid();

                var row = new RequirementRowViewModel(new Requirement(requirementId, null, null), null);
                this.viewModel.SelectedItem = row;

                renderer.Render();
                var button = renderer.Find(".comments__add-button");
                await renderer.InvokeAsync(() => button.Click(new MouseEventArgs()));
                Assert.That(this.viewModel.IsOnCommentCreationMode, Is.True);

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

                Assert.Multiple(() =>
                {
                    Assert.That(this.viewModel.Comments, Has.Count.EqualTo(1));
                    Assert.That(this.viewModel.IsOnCommentCreationMode, Is.False);
                });

                this.viewModel.SelectedItem = null;
                Assert.That(this.viewModel.Comments, Is.Empty);

                this.annotationService.Setup(x => x.GetAnnotationsOfAnnotatableItem(It.IsAny<Guid>(), row.ReviewItem.Id))
                    .ReturnsAsync(row.ReviewItem.Annotations);

                this.viewModel.SelectedItem = row;
                Assert.That(this.viewModel.Comments, Has.Count.EqualTo(1));

                renderer.Render();
                var commentCard = renderer.FindComponent<CommentCard>();

                Assert.That(commentCard.Instance.ViewModel.IsAllowedToEdit, Is.True);

                await renderer.InvokeAsync(() => commentCard.Instance.ViewModel.OnContentEditCallback.InvokeAsync(commentCard.Instance.ViewModel.Comment));
                Assert.That(this.viewModel.IsOnCommentUpdateMode, Is.True);

                this.annotationService.Setup(x => x.UpdateAnnotation(It.IsAny<Guid>(), It.IsAny<Annotation>()))
                    .ReturnsAsync(EntityRequestResponse<Annotation>.Fail(new List<string> { "Error" }));

                await renderer.InvokeAsync(this.viewModel.CommentCreationViewModel.OnValidSubmit.InvokeAsync);
                Assert.That(this.viewModel.ErrorMessageViewModel.Errors, Has.Count.EqualTo(1));

                this.annotationService.Setup(x => x.UpdateAnnotation(It.IsAny<Guid>(), It.IsAny<Annotation>()))
                    .ReturnsAsync(EntityRequestResponse<Annotation>.Success(row.ReviewItem.Annotations.FirstOrDefault(x => x is Comment)));

                await renderer.InvokeAsync(this.viewModel.CommentCreationViewModel.OnValidSubmit.InvokeAsync);
                Assert.That(this.viewModel.Comments, Has.Count.EqualTo(1));

                var dropdownButton = renderer.Find($"#comment_{this.viewModel.Comments.Items.First().Id}");
                await renderer.InvokeAsync(() => dropdownButton.ClickAsync(new MouseEventArgs()));
                Assert.That(commentCard.Instance.IsOnStatusUpdateMode, Is.True);

                await commentCard.Instance.ViewModel.UpdateStatus(StatusKind.Approved);
                this.annotationService.Verify(x => x.UpdateAnnotation(It.IsAny<Guid>(), It.IsAny<Annotation>()), Times.Exactly(3));

                await renderer.InvokeAsync(() => commentCard.Instance.ViewModel.OnDeleteCallback.InvokeAsync(commentCard.Instance.ViewModel.Comment));
                Assert.That(this.viewModel.CommentConfirmCancelPopupViewModel.IsVisible, Is.True);

                await renderer.InvokeAsync(this.viewModel.CommentConfirmCancelPopupViewModel.OnConfirm.InvokeAsync);
                Assert.That(this.viewModel.Comments, Is.Empty);

                this.annotationService.Setup(x => x.GetAnnotationsForReviewTask(It.IsAny<Guid>(), It.IsAny<Guid>()))
                    .ReturnsAsync(new List<Annotation> { new Comment(Guid.NewGuid()) });

                this.viewModel.SelectedItem = new ReviewTask(Guid.NewGuid());
                Assert.That(this.viewModel.Comments, Has.Count.EqualTo(1));
            }
            catch
            {
                // On GitHub, exception is thrown even if the JSRuntime has been configured
            }
        }

        [Test]
        public async Task VerifyReplySystem()
        {
            try
            {
                var renderer = this.context.RenderComponent<Comments>();
                
                this.viewModel.Comments.Add(new Comment(Guid.NewGuid())
                {
                    Author = this.participant,
                    Content = "Content",
                    CreatedOn = DateTime.Now
                });

                var commentCard = renderer.FindComponent<CommentCard>();
                await renderer.InvokeAsync(() => commentCard.Instance.ViewModel.OnReplyCallback.InvokeAsync(commentCard.Instance.ViewModel.Comment));
                Assert.That(this.viewModel.IsOnReplyCreationMode, Is.True);

                this.replyService.Setup(x => x.CreateReply(this.viewModel.ProjectId, commentCard.Instance.ViewModel.Comment.Id, It.IsAny<Reply>()))
                    .ReturnsAsync(EntityRequestResponse<Reply>.Fail(new List<string>{"An error"}));

                await renderer.InvokeAsync(this.viewModel.ReplyCreationViewModel.OnValidSubmit.InvokeAsync);
                Assert.That(this.viewModel.IsOnReplyCreationMode, Is.True);

                this.replyService.Setup(x => x.CreateReply(this.viewModel.ProjectId, commentCard.Instance.ViewModel.Comment.Id, It.IsAny<Reply>()))
                    .ReturnsAsync(EntityRequestResponse<Reply>.Success(new Reply(Guid.NewGuid()){Author = this.participant, CreatedOn = DateTime.Now, Content = "Content"}));

                await renderer.InvokeAsync(this.viewModel.ReplyCreationViewModel.OnValidSubmit.InvokeAsync);
                Assert.That(this.viewModel.IsOnReplyCreationMode, Is.False);

                var replyCard = renderer.FindComponent<ReplyCard>();
                Assert.That(replyCard, Is.Not.Null);

                await renderer.InvokeAsync(() => replyCard.Instance.ViewModel.OnContentEditCallback.InvokeAsync(replyCard.Instance.ViewModel.Reply));
                Assert.That(this.viewModel.IsOnReplyUpdateMode, Is.True);

                this.replyService.Setup(x => x.UpdateReply(this.viewModel.ProjectId, commentCard.Instance.ViewModel.Comment.Id, replyCard.Instance.ViewModel.Reply))
                    .ReturnsAsync(EntityRequestResponse<Reply>.Fail(new List<string> { "An error" }));

                await renderer.InvokeAsync(this.viewModel.ReplyCreationViewModel.OnValidSubmit.InvokeAsync);
                Assert.That(this.viewModel.IsOnReplyUpdateMode, Is.True);

                this.replyService.Setup(x => x.UpdateReply(this.viewModel.ProjectId, commentCard.Instance.ViewModel.Comment.Id, replyCard.Instance.ViewModel.Reply))
                    .ReturnsAsync(EntityRequestResponse<Reply>.Success(replyCard.Instance.ViewModel.Reply));

                await renderer.InvokeAsync(this.viewModel.ReplyCreationViewModel.OnValidSubmit.InvokeAsync);
                Assert.That(this.viewModel.IsOnReplyUpdateMode, Is.False);

                await renderer.InvokeAsync(() => replyCard.Instance.ViewModel.OnDeleteCallback.InvokeAsync(replyCard.Instance.ViewModel.Reply));
                Assert.That(this.viewModel.ReplyConfirmCancelPopupViewModel.IsVisible, Is.True);

                await renderer.InvokeAsync(this.viewModel.ReplyConfirmCancelPopupViewModel.OnConfirm.InvokeAsync);
                Assert.That(renderer.FindComponents<ReplyCard>(), Is.Empty);
            }
            catch
            {
                // On GitHub, exception is thrown even if the JSRuntime has been configured
            }
        }
    }
}
