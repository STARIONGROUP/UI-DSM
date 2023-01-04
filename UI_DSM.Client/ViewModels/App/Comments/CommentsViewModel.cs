// --------------------------------------------------------------------------------------------------------
// <copyright file="CommentsViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.App.Comments
{
    using DynamicData;

    using Microsoft.AspNetCore.Components;

    using ReactiveUI;

    using UI_DSM.Client.Services.AnnotationService;
    using UI_DSM.Client.Services.ReplyService;
    using UI_DSM.Client.Services.ReviewItemService;
    using UI_DSM.Client.ViewModels.App.CommentCreation;
    using UI_DSM.Client.ViewModels.App.ReplyCreation;
    using UI_DSM.Client.ViewModels.Components;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     View model for <see cref="Client.Components.App.Comments" /> component
    /// </summary>
    public class CommentsViewModel : ReactiveObject, ICommentsViewModel
    {
        /// <summary>
        ///     The <see cref="IAnnotationService" />
        /// </summary>
        private readonly IAnnotationService annotationService;

        /// <summary>
        ///     A collection of <see cref="IDisposable" />
        /// </summary>
        private readonly List<IDisposable> disposables = new();

        /// <summary>
        ///     The <see cref="IReplyService" />
        /// </summary>
        private readonly IReplyService replyService;

        /// <summary>
        ///     The <see cref="IReviewItemService" />
        /// </summary>
        private readonly IReviewItemService reviewItemService;

        /// <summary>
        ///     Backing field for <see cref="IsOnCommentCreationMode" />
        /// </summary>
        private bool isOnCommentCreationMode;

        /// <summary>
        ///     Backing field for <see cref="IsOnCommentUpdateMode" />
        /// </summary>
        private bool isOnCommentUpdateMode;

        /// <summary>
        ///     Backing field for <see cref="IsOnReplyCreationMode" />
        /// </summary>
        private bool isOnReplyCreationMode;

        /// <summary>
        ///     Backing field for <see cref="IsOnReplyUpdateMode" />
        /// </summary>
        private bool isOnReplyUpdateMode;

        /// <summary>
        ///     The currently selected <see cref="Comment" /> to update/delete
        /// </summary>
        private Comment selectedComment;

        /// <summary>
        ///     Backing field for <see cref="SelectedItem" />
        /// </summary>
        private object selectedItem;

        /// <summary>
        ///     The currently selected <see cref="Reply" /> to update/delete
        /// </summary>
        private Reply selectedReply;

        /// <summary>
        /// Reference to the <see cref="reviewTask"/>
        /// </summary>
        private ReviewTask reviewTask;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CommentsViewModel" /> class.
        /// </summary>
        /// <param name="reviewItemService">The <see cref="IReviewItemService" /></param>
        /// <param name="annotationService">The <see cref="IAnnotationService" /></param>
        /// <param name="replyService">The <see cref="IReplyService" /></param>
        public CommentsViewModel(IReviewItemService reviewItemService, IAnnotationService annotationService, IReplyService replyService)
        {
            this.reviewItemService = reviewItemService;
            this.annotationService = annotationService;
            this.replyService = replyService;

            this.CommentCreationViewModel = new CommentCreationViewModel
            {
                OnValidSubmit = new EventCallbackFactory().Create(this, this.AddOrUpdateComment)
            };

            this.ReplyCreationViewModel = new ReplyCreationViewModel
            {
                OnValidSubmit = new EventCallbackFactory().Create(this, this.AddOrUpdateReply)
            };

            this.disposables.Add(this.WhenAnyValue(x => x.SelectedItem)
                .Subscribe(async _ => await this.UpdateProperties()));

            this.OnCommentEditCallback = new EventCallbackFactory().Create(this, (Comment comment) => this.OpenUpdateComment(comment));
            this.OnDeleteCallback = new EventCallbackFactory().Create(this, (Comment comment) => this.AskToDeleteComment(comment));
            this.OnUpdateStatusCallback = new EventCallbackFactory().Create(this, (Comment comment) => this.UpdateCommentStatus(comment));
            this.OnReplyCallback = new EventCallbackFactory().Create(this, (Comment comment) => this.OpenReplyCreationPopup(comment));
            this.OnReplyEditContentCallback = new EventCallbackFactory().Create(this, (Reply reply) => this.OpenUpdateReply(reply));
            this.OnDeleteReplyCallback = new EventCallbackFactory().Create(this, (Reply reply) => this.AskToDeleteReply(reply));

            this.ReplyConfirmCancelPopupViewModel = new ConfirmCancelPopupViewModel
            {
                OnConfirm = new EventCallbackFactory().Create(this, this.DeleteReply),
                HeaderText = "Delete Reply",
                ContentText = "Are you sure to want to delete this reply ?",
                OnCancel = new EventCallbackFactory().Create(this, () => this.ReplyConfirmCancelPopupViewModel.IsVisible = false)
            };

            this.CommentConfirmCancelPopupViewModel = new ConfirmCancelPopupViewModel
            {
                OnConfirm = new EventCallbackFactory().Create(this, this.DeleteComment),
                HeaderText = "Delete Comment",
                ContentText = "Are you sure to want to delete this comment ?",
                OnCancel = new EventCallbackFactory().Create(this, () => this.CommentConfirmCancelPopupViewModel.IsVisible = false)
            };
        }

        /// <summary>
        ///     The current <see cref="View" />
        /// </summary>
        public View CurrentView { get; set; }

        /// <summary>
        ///     Value indicating if the current view is on creation mode for <see cref="Reply" />
        /// </summary>
        public bool IsOnReplyCreationMode
        {
            get => this.isOnReplyCreationMode;
            set => this.RaiseAndSetIfChanged(ref this.isOnReplyCreationMode, value);
        }

        /// <summary>
        ///     Value indicating if the current view is on update mode for <see cref="Reply" />
        /// </summary>
        public bool IsOnReplyUpdateMode
        {
            get => this.isOnReplyUpdateMode;
            set => this.RaiseAndSetIfChanged(ref this.isOnReplyUpdateMode, value);
        }

        /// <summary>
        ///     The <see cref="IReplyCreationViewModel" />
        /// </summary>
        public IReplyCreationViewModel ReplyCreationViewModel { get; set; }

        /// <summary>
        ///     Value indicating if the current view is on update mode for <see cref="Comment" />
        /// </summary>
        public bool IsOnCommentUpdateMode
        {
            get => this.isOnCommentUpdateMode;
            set => this.RaiseAndSetIfChanged(ref this.isOnCommentUpdateMode, value);
        }

        /// <summary>
        ///     Event callback when a user wants to update the stauts of a <see cref="Comment" />
        /// </summary>
        public EventCallback<Comment> OnUpdateStatusCallback { get; set; }

        /// <summary>
        ///     Event callback when a user wants to reply to a <see cref="Comment" />
        /// </summary>
        public EventCallback<Comment> OnReplyCallback { get; set; }

        /// <summary>
        ///     Event callback when a user wants to edit the content of a <see cref="Reply" />
        /// </summary>
        public EventCallback<Reply> OnReplyEditContentCallback { get; set; }

        /// <summary>
        ///     Event callback when a user wants to delete a <see cref="Reply" />
        /// </summary>
        public EventCallback<Reply> OnDeleteReplyCallback { get; set; }

        /// <summary>
        ///     Event callback when a user wants to delete a <see cref="Comment" />
        /// </summary>
        public EventCallback<Comment> OnDeleteCallback { get; set; }

        /// <summary>
        ///     Event callback when a user wants to update a <see cref="Comment" />
        /// </summary>
        public EventCallback<Comment> OnCommentEditCallback { get; private set; }

        /// <summary>
        ///     The <see cref="IConfirmCancelPopupViewModel" /> for <see cref="Comment" />
        /// </summary>
        public IConfirmCancelPopupViewModel CommentConfirmCancelPopupViewModel { get; set; }

        /// <summary>
        ///     The <see cref="IConfirmCancelPopupViewModel" /> for <see cref="Reply" />
        /// </summary>
        public IConfirmCancelPopupViewModel ReplyConfirmCancelPopupViewModel { get; set; }

        /// <summary>
        ///     Event callback when a user wants to link a <see cref="Comment"/> to another element
        /// </summary>
        public EventCallback<Comment> OnLinkCallback { get; private set; }

        /// <summary>
        ///     A collection of <see cref="ICommentsViewModel.AvailableRows" />
        /// </summary>
        public List<IHaveAnnotatableItemRowViewModel> AvailableRows { get; set; }

        /// <summary>
        ///     The current <see cref="Participant" />
        /// </summary>
        public Participant Participant { get; private set; }

        /// <summary>
        ///     The <see cref="ICommentCreationViewModel" />
        /// </summary>
        public ICommentCreationViewModel CommentCreationViewModel { get; set; }

        /// <summary>
        ///     The <see cref="IErrorMessageViewModel" />
        /// </summary>
        public IErrorMessageViewModel ErrorMessageViewModel { get; set; } = new ErrorMessageViewModel();

        /// <summary>
        ///     Value indicating if the current view is on creation mode for <see cref="Comment" />
        /// </summary>
        public bool IsOnCommentCreationMode
        {
            get => this.isOnCommentCreationMode;
            set => this.RaiseAndSetIfChanged(ref this.isOnCommentCreationMode, value);
        }

        /// <summary>
        ///     The currently selected item in a View
        /// </summary>
        public object SelectedItem
        {
            get => this.selectedItem;
            set => this.RaiseAndSetIfChanged(ref this.selectedItem, value);
        }

        /// <summary>
        ///     The <see cref="SourceList{T}" /> for <see cref="Comment" />
        /// </summary>
        public SourceList<Comment> Comments { get; } = new();

        /// <summary>
        ///     The <see cref="Guid" /> of the <see cref="Project" />
        /// </summary>
        public Guid ProjectId { get; private set; }

        /// <summary>
        ///     The <see cref="Guid" /> of the <see cref="Review" />
        /// </summary>
        public Guid ReviewId { get; private set; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.disposables.ForEach(x => x.Dispose());
        }

        /// <summary>
        ///     Initializes this viewModel properties
        /// </summary>
        /// <param name="projectId">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <param name="reviewId">The <see cref="Guid" /> of the <see cref="Review" /></param>
        /// <param name="currentView">The <see cref="View" /></param>
        /// <param name="currentParticipant">The current <see cref="Participant"/></param>
        /// <param name="onLinkCallback">The <see cref="EventCallback{TValue}" /> for linking a <see cref="Comment" /> on other element</param>
        /// <param name="task">The <see cref="reviewTask"/></param>
        public void InitializesProperties(Guid projectId, Guid reviewId, View currentView, Participant currentParticipant, EventCallback<Comment> onLinkCallback, 
            ReviewTask task)
        {
            this.ProjectId = projectId;
            this.ReviewId = reviewId;
            this.CurrentView = currentView;
            this.Participant = currentParticipant;
            this.OnLinkCallback = onLinkCallback;
            this.reviewTask = task;
        }

        /// <summary>
        ///     Opens the creation popup for <see cref="Comment" />
        /// </summary>
        public void OpenCommentCreationPopup()
        {
            this.CommentCreationViewModel.Comment = new Comment();
            this.ErrorMessageViewModel.Errors.Clear();
            this.IsOnCommentCreationMode = true;
        }

        /// <summary>
        ///     Sets the current selected <see cref="Comment" />
        /// </summary>
        /// <param name="comment">The <see cref="Comment" /></param>
        public void SetCurrentComment(Comment comment)
        {
            this.selectedComment = comment;
        }

        /// <summary>
        ///     Deletes the current selected <see cref="Reply" />
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private async Task DeleteReply()
        {
            if (this.selectedReply != null && this.selectedComment != null)
            {
                await this.replyService.DeleteReply(this.ProjectId, this.selectedComment.Id, this.selectedReply);
                var comment = this.Comments.Items.First(x => x.Id == this.selectedComment.Id);
                comment.Replies.Remove(comment.Replies.FirstOrDefault(x => x.Id == this.selectedReply.Id));
                this.selectedReply = null;
                this.ReplyConfirmCancelPopupViewModel.IsVisible = false;
            }
        }

        /// <summary>
        ///     Asks the user to confirm the deletion of the <see cref="Reply" />
        /// </summary>
        /// <param name="reply">The <see cref="Reply" /> to delete</param>
        private void AskToDeleteReply(Reply reply)
        {
            this.selectedReply = reply;
            this.ReplyConfirmCancelPopupViewModel.IsVisible = true;
        }

        /// <summary>
        ///     Open the update pop-up
        /// </summary>
        /// <param name="reply">The <see cref="Reply" /> to update</param>
        private void OpenUpdateReply(Reply reply)
        {
            this.selectedComment = reply.EntityContainer as Comment;
            this.ReplyCreationViewModel.Reply = reply;
            this.ErrorMessageViewModel.Errors.Clear();
            this.IsOnReplyUpdateMode = true;
        }

        /// <summary>
        ///     Add a <see cref="Reply" /> on the selected comment
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private async Task AddOrUpdateReply()
        {
            if (this.selectedComment != null)
            {
                var commentFromList = this.Comments.Items.First(x => x.Id == this.selectedComment.Id);

                if (this.IsOnReplyCreationMode)
                {
                    await this.CreateReply(commentFromList, this.ReplyCreationViewModel.Reply);
                }

                if (this.IsOnReplyUpdateMode)
                {
                    await this.UpdateReply(commentFromList, this.ReplyCreationViewModel.Reply);
                }
            }
        }

        /// <summary>
        ///     Updates a <see cref="Reply" />
        /// </summary>
        /// <param name="comment">The <see cref="Comment" /></param>
        /// <param name="reply">The <see cref="Reply" /> to update</param>
        /// <returns>A <see cref="Task" /></returns>
        private async Task UpdateReply(Comment comment, Reply reply)
        {
            var replyUpdate = await this.replyService.UpdateReply(this.ProjectId, comment.Id, reply);

            if (replyUpdate.IsRequestSuccessful)
            {
                var updatedReply = replyUpdate.Entity;
                comment.Replies.Replace(comment.Replies.FirstOrDefault(x => x.Id == updatedReply!.Id), updatedReply);
                this.IsOnReplyUpdateMode = false;
            }
            else
            {
                this.ErrorMessageViewModel.HandleErrors(replyUpdate.Errors);
            }
        }

        /// <summary>
        ///     Creates a new <see cref="Reply" /> into a <see cref="Comment" />
        /// </summary>
        /// <param name="comment">The <see cref="Comment" /></param>
        /// <param name="reply">The <see cref="Reply" /></param>
        /// <returns>A <see cref="Task" /></returns>
        private async Task CreateReply(Comment comment, Reply reply)
        {
            var replyCreation = await this.replyService.CreateReply(this.ProjectId, comment.Id, reply);

            if (replyCreation.IsRequestSuccessful)
            {
                var newReply = replyCreation.Entity;
                comment.Replies.Add(newReply);
                this.IsOnReplyCreationMode = false;
            }
            else
            {
                this.ErrorMessageViewModel.HandleErrors(replyCreation.Errors);
            }
        }

        /// <summary>
        ///     Opens the creation popup for a <see cref="Reply" />
        /// </summary>
        /// <param name="comment">The <see cref="Comment"/></param>
        private void OpenReplyCreationPopup(Comment comment)
        {
            this.selectedComment = comment;
            this.ReplyCreationViewModel.Reply = new Reply();
            this.ErrorMessageViewModel.Errors.Clear();
            this.IsOnReplyCreationMode = true;
        }

        /// <summary>
        ///     Updates the status of the <see cref="Annotation" />
        /// </summary>
        /// <param name="annotation">The <see cref="Annotation" /></param>
        /// <returns>A <see cref="Task" /></returns>
        private async Task UpdateCommentStatus(Annotation annotation)
        {
            if (this.SelectedItem is IHaveThingRowViewModel thingRowViewModel)
            {
                await this.UpdateComment(thingRowViewModel, annotation);
            }
        }

        /// <summary>
        ///     Add a <see cref="Comment" /> on the selected thing
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private async Task AddOrUpdateComment()
        {
            if (this.SelectedItem is IHaveThingRowViewModel thingRowViewModel)
            {
                if (thingRowViewModel.ReviewItem == null)
                {
                    var createdReviewItem = await this.reviewItemService.GetReviewItemForThing(this.ProjectId, this.ReviewId, thingRowViewModel.ThingId);

                    if (createdReviewItem != null)
                    {
                        thingRowViewModel.UpdateReviewItem(createdReviewItem);
                    }
                    else
                    {
                        var reviewItemResponse = await this.reviewItemService.CreateReviewItem(this.ProjectId, this.ReviewId, thingRowViewModel.ThingId);

                        if (reviewItemResponse.IsRequestSuccessful)
                        {
                            thingRowViewModel.UpdateReviewItem(reviewItemResponse.Entity);
                        }
                        else
                        {
                            this.ErrorMessageViewModel.HandleErrors(reviewItemResponse.Errors);
                            return;
                        }
                    }
                }

                if (this.IsOnCommentCreationMode)
                {
                    await this.CreateComment(thingRowViewModel, this.CommentCreationViewModel.Comment);
                }

                if (this.IsOnCommentUpdateMode)
                {
                    await this.UpdateComment(thingRowViewModel, this.CommentCreationViewModel.Comment);
                }
            }
        }

        /// <summary>
        ///     Updates a <see cref="Annotation" />
        /// </summary>
        /// <param name="thingRowViewModel">The associated <see cref="IHaveThingRowViewModel" /></param>
        /// <param name="annotation">The <see cref="Annotation" /> to update</param>
        /// <returns>A <see cref="Task" /></returns>
        private async Task UpdateComment(IHaveThingRowViewModel thingRowViewModel, Annotation annotation)
        {
            var commentUpdate = await this.annotationService.UpdateAnnotation(this.ProjectId, annotation);

            if (commentUpdate.IsRequestSuccessful)
            {
                var updatedComment = commentUpdate.Entity as Comment;
                this.Comments.Replace(this.Comments.Items.FirstOrDefault(x => x.Id == updatedComment!.Id), updatedComment);
                thingRowViewModel.ReviewItem.Annotations.Replace(thingRowViewModel.ReviewItem.Annotations.FirstOrDefault(x => x.Id == updatedComment!.Id), updatedComment);
                this.IsOnCommentUpdateMode = false;
            }
            else
            {
                this.ErrorMessageViewModel.HandleErrors(commentUpdate.Errors);
            }
        }

        /// <summary>
        ///     Creates a new <see cref="Comment" />
        /// </summary>
        /// <param name="thingRowViewModel">The associated <see cref="IHaveThingRowViewModel" /></param>
        /// <param name="comment">The <see cref="Comment" /> to create</param>
        /// <returns>A <see cref="Task" /></returns>
        private async Task CreateComment(IHaveThingRowViewModel thingRowViewModel, Comment comment)
        {
            comment.View = this.CurrentView;
            comment.AnnotatableItems.Add(thingRowViewModel.ReviewItem);
            comment.CreatedInside = this.reviewTask;

            var commentCreation = await this.annotationService.CreateAnnotation(this.ProjectId, comment);

            if (commentCreation.IsRequestSuccessful)
            {
                var newComment = commentCreation.Entity;
                thingRowViewModel.ReviewItem?.Annotations.Add(newComment);
                this.Comments.Add(newComment as Comment);
                this.IsOnCommentCreationMode = false;
            }
            else
            {
                this.ErrorMessageViewModel.HandleErrors(commentCreation.Errors);
            }
        }

        /// <summary>
        ///     Deletes the current selected comment
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private async Task DeleteComment()
        {
            if (this.selectedComment != null)
            {
                await this.annotationService.DeleteAnnotation(this.ProjectId, this.selectedComment);
                this.Comments.Remove(this.Comments.Items.FirstOrDefault(x => x.Id == this.selectedComment.Id));
                this.RemoveCommentFromSelectedItem();
                this.selectedComment = null;
                this.CommentConfirmCancelPopupViewModel.IsVisible = false;
            }
        }

        /// <summary>
        ///     Removes the <see cref="Comment" /> from the <see cref="SelectedItem" />
        /// </summary>
        private void RemoveCommentFromSelectedItem()
        {
            if (this.selectedComment == null)
            {
                return;
            }

            foreach (var row in this.AvailableRows.OfType<IHaveThingRowViewModel>()
                         .Where(x => x.ReviewItem != null && x.ReviewItem.Annotations.Any(annotation => annotation.Id == this.selectedComment.Id)))
            {
                row.ReviewItem.Annotations.RemoveAll(x => x.Id == this.selectedComment.Id);
            }
        }

        /// <summary>
        ///     Asks the user to confirm the deletion of the <see cref="Comment" />
        /// </summary>
        /// <param name="comment">The <see cref="Comment" /> to delete</param>
        private void AskToDeleteComment(Comment comment)
        {
            this.selectedComment = comment;
            this.CommentConfirmCancelPopupViewModel.IsVisible = true;
        }

        /// <summary>
        ///     Open the update pop-up
        /// </summary>
        /// <param name="comment">The <see cref="Comment" /> to update</param>
        private void OpenUpdateComment(Comment comment)
        {
            this.CommentCreationViewModel.Comment = comment;
            this.ErrorMessageViewModel.Errors.Clear();
            this.IsOnCommentUpdateMode = true;
        }

        /// <summary>
        ///     Updates this viewmodel properties
        /// </summary>
        private async Task UpdateProperties()
        {
            this.Comments.Clear();

            if (this.SelectedItem is IHaveThingRowViewModel { ReviewItem: null } row)
            {
                var reviewItem = await this.reviewItemService.GetReviewItemForThing(this.ProjectId, this.ReviewId, row.ThingId);

                if (reviewItem != null)
                {
                    row.UpdateReviewItem(reviewItem);
                }
                else
                {
                    return;
                }
            }

            if (this.SelectedItem is IHaveAnnotatableItemRowViewModel { AnnotatableItemId: { } } annotatableItemRowViewModel)
            {
                var annotations = await this.annotationService.GetAnnotationsOfAnnotatableItem(this.ProjectId, 
                    annotatableItemRowViewModel.AnnotatableItemId.Value);

                this.Comments.AddRange(annotations.OfType<Comment>());
            }
        }
    }
}
