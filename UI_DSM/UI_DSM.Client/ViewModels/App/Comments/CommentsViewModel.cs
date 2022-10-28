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

    using UI_DSM.Client.Services.Administration.ParticipantService;
    using UI_DSM.Client.Services.AnnotationService;
    using UI_DSM.Client.Services.ReviewItemService;
    using UI_DSM.Client.ViewModels.App.CommentCreation;
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
        ///     The <see cref="IParticipantService" />
        /// </summary>
        private readonly IParticipantService participantService;

        /// <summary>
        ///     The <see cref="IReviewItemService" />
        /// </summary>
        private readonly IReviewItemService reviewItemService;

        /// <summary>
        ///     Backing field for <see cref="IsOnCreationMode" />
        /// </summary>
        private bool isOnCreationMode;

        /// <summary>
        ///     Backing field for <see cref="IsOnUpdateMode" />
        /// </summary>
        private bool isOnUpdateMode;

        /// <summary>
        ///     The currently selected <see cref="Comment" /> to update/delete
        /// </summary>
        private Comment selectedComment;

        /// <summary>
        ///     Backing field for <see cref="SelectedItem" />
        /// </summary>
        private object selectedItem;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CommentsViewModel" /> class.
        /// </summary>
        /// <param name="reviewItemService">The <see cref="IReviewItemService" /></param>
        /// <param name="annotationService">The <see cref="IAnnotationService" /></param>
        /// <param name="participantService">The <see cref="IParticipantService" /></param>
        public CommentsViewModel(IReviewItemService reviewItemService, IAnnotationService annotationService, IParticipantService participantService)
        {
            this.reviewItemService = reviewItemService;
            this.annotationService = annotationService;
            this.participantService = participantService;

            this.CommentCreationViewModel = new CommentCreationViewModel
            {
                OnValidSubmit = new EventCallbackFactory().Create(this, this.AddOrUpdateComment)
            };

            this.disposables.Add(this.WhenAnyValue(x => x.SelectedItem)
                .Subscribe(async _ => await this.UpdateProperties()));

            this.OnCommentEditCallback = new EventCallbackFactory().Create(this, (Comment comment) => this.OpenUpdateComment(comment));
            this.OnDeleteCallback = new EventCallbackFactory().Create(this, (Comment comment) => this.AskToDeleteComment(comment));
            this.OnUpdateStatusCallback = new EventCallbackFactory().Create(this, (Comment comment) => this.UpdateCommentStatus(comment));

            this.ConfirmCancelPopupViewModel = new ConfirmCancelPopupViewModel
            {
                OnConfirm = new EventCallbackFactory().Create(this, this.DeleteComment),
                HeaderText = "Remove Comment",
                ContentText = "Are you sure to want to remove this comment ?",
                OnCancel = new EventCallbackFactory().Create(this, () => this.ConfirmCancelPopupViewModel.IsVisible = false)
            };
        }

        /// <summary>
        ///     The current <see cref="View" />
        /// </summary>
        public View CurrentView { get; set; }

        /// <summary>
        ///     Value indicating if the current view is on update mode
        /// </summary>
        public bool IsOnUpdateMode
        {
            get => this.isOnUpdateMode;
            set => this.RaiseAndSetIfChanged(ref this.isOnUpdateMode, value);
        }

        /// <summary>
        ///     Event callback when a user wants to update the stauts of a <see cref="Comment" />
        /// </summary>
        public EventCallback<Comment> OnUpdateStatusCallback { get; set; }

        /// <summary>
        ///     Event callback when a user wants to delete a <see cref="Comment" />
        /// </summary>
        public EventCallback<Comment> OnDeleteCallback { get; set; }

        /// <summary>
        ///     Event callback when a user wants to update a <see cref="Comment" />
        /// </summary>
        public EventCallback<Comment> OnCommentEditCallback { get; private set; }

        /// <summary>
        ///     The <see cref="IConfirmCancelPopupViewModel" />
        /// </summary>
        public IConfirmCancelPopupViewModel ConfirmCancelPopupViewModel { get; set; }

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
        ///     Value indicating if the current view is on creation mode
        /// </summary>
        public bool IsOnCreationMode
        {
            get => this.isOnCreationMode;
            set => this.RaiseAndSetIfChanged(ref this.isOnCreationMode, value);
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
        /// <returns>A <see cref="Task" /></returns>
        public async Task InitializesProperties(Guid projectId, Guid reviewId, View currentView)
        {
            this.ProjectId = projectId;
            this.ReviewId = reviewId;
            this.CurrentView = currentView;
            this.Participant = await this.participantService.GetCurrentParticipant(this.ProjectId);
        }

        /// <summary>
        ///     Opens the creation popup
        /// </summary>
        public void OpenCreationPopup()
        {
            this.CommentCreationViewModel.Comment = new Comment();
            this.ErrorMessageViewModel.Errors.Clear();
            this.IsOnCreationMode = true;
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

                if (this.IsOnCreationMode)
                {
                    await this.CreateComment(thingRowViewModel, this.CommentCreationViewModel.Comment);
                }

                if (this.IsOnUpdateMode)
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
                this.IsOnUpdateMode = false;
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

            var commentCreation = await this.annotationService.CreateAnnotation(this.ProjectId, comment);

            if (commentCreation.IsRequestSuccessful)
            {
                var newComment = commentCreation.Entity;
                thingRowViewModel.ReviewItem?.Annotations.Add(newComment);
                this.Comments.Add(newComment as Comment);
                this.IsOnCreationMode = false;
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
                this.ConfirmCancelPopupViewModel.IsVisible = false;
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

            if (this.SelectedItem is IHaveThingRowViewModel thingRowViewModel)
            {
                thingRowViewModel.ReviewItem.Annotations.RemoveAll(x => x.Id == this.selectedComment.Id);
            }
        }

        /// <summary>
        ///     Asks the user to confirm the deletion of the <see cref="Comment" />
        /// </summary>
        /// <param name="comment">The <see cref="Comment" /> to delete</param>
        private void AskToDeleteComment(Comment comment)
        {
            this.selectedComment = comment;
            this.ConfirmCancelPopupViewModel.IsVisible = true;
        }

        /// <summary>
        ///     Open the update pop-up
        /// </summary>
        /// <param name="comment">The <see cref="Comment" /> to update</param>
        private void OpenUpdateComment(Comment comment)
        {
            this.CommentCreationViewModel.Comment = comment;
            this.ErrorMessageViewModel.Errors.Clear();
            this.IsOnUpdateMode = true;
        }

        /// <summary>
        ///     Updates this viewmodel properties
        /// </summary>
        private async Task UpdateProperties()
        {
            this.Comments.Clear();

            if (this.SelectedItem is IHaveThingRowViewModel { ReviewItem: { } } thingRowViewModel)
            {
                var reviewItem = await this.reviewItemService.GetReviewItemOfReview(this.ProjectId, this.ReviewId, thingRowViewModel.ReviewItem.Id, 2);
                this.Comments.AddRange(reviewItem.Annotations.OfType<Comment>());
            }
        }
    }
}
