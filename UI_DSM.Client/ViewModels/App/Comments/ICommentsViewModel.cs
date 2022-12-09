// --------------------------------------------------------------------------------------------------------
// <copyright file="ICommentsViewModel.cs" company="RHEA System S.A.">
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

    using UI_DSM.Client.ViewModels.App.CommentCreation;
    using UI_DSM.Client.ViewModels.App.ReplyCreation;
    using UI_DSM.Client.ViewModels.Components;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface definition for <see cref="CommentsViewModel" />
    /// </summary>
    public interface ICommentsViewModel : IDisposable
    {
        /// <summary>
        ///     The currently selected item in a View
        /// </summary>
        object SelectedItem { get; set; }

        /// <summary>
        ///     The <see cref="SourceList{T}" /> for <see cref="Comment" />
        /// </summary>
        SourceList<Comment> Comments { get; }

        /// <summary>
        ///     The <see cref="Guid" /> of the <see cref="Project" />
        /// </summary>
        Guid ProjectId { get; }

        /// <summary>
        ///     The <see cref="Guid" /> of the <see cref="Review" />
        /// </summary>
        Guid ReviewId { get; }

        /// <summary>
        ///     Value indicating if the current view is on creation mode for <see cref="Comment" />
        /// </summary>
        bool IsOnCommentCreationMode { get; set; }

        /// <summary>
        ///     The <see cref="ICommentCreationViewModel" />
        /// </summary>
        ICommentCreationViewModel CommentCreationViewModel { get; set; }

        /// <summary>
        ///     The <see cref="IErrorMessageViewModel" />
        /// </summary>
        IErrorMessageViewModel ErrorMessageViewModel { get; set; }

        /// <summary>
        ///     The current <see cref="Participant" />
        /// </summary>
        Participant Participant { get; }

        /// <summary>
        ///     Event callback when a user wants to delete a <see cref="Comment" />
        /// </summary>
        EventCallback<Comment> OnDeleteCallback { get; set; }

        /// <summary>
        ///     Event callback when a user wants to update a <see cref="Comment" />
        /// </summary>
        EventCallback<Comment> OnCommentEditCallback { get; }

        /// <summary>
        ///     The <see cref="IConfirmCancelPopupViewModel" />
        /// </summary>
        IConfirmCancelPopupViewModel CommentConfirmCancelPopupViewModel { get; set; }

        /// <summary>
        ///     Value indicating if the current view is on update mode
        /// </summary>
        bool IsOnCommentUpdateMode { get; set; }

        /// <summary>
        ///     Event callback when a user wants to update the statis of a <see cref="Comment" />
        /// </summary>
        EventCallback<Comment> OnUpdateStatusCallback { get; set; }

        /// <summary>
        ///     Event callback when a user wants to reply to a <see cref="Comment" />
        /// </summary>
        EventCallback<Comment> OnReplyCallback { get; set; }

        /// <summary>
        ///     Event callback when a user wants to edit the content of a <see cref="Reply" />
        /// </summary>
        EventCallback<Reply> OnReplyEditContentCallback { get; set; }

        /// <summary>
        ///     Event callback when a user wants to delete a <see cref="Reply" />
        /// </summary>
        EventCallback<Reply> OnDeleteReplyCallback { get; set; }

        /// <summary>
        ///     Value indicating if the current view is on creation mode for <see cref="Reply" />
        /// </summary>
        bool IsOnReplyCreationMode { get; set; }

        /// <summary>
        ///     Value indicating if the current view is on update mode for <see cref="Reply" />
        /// </summary>
        bool IsOnReplyUpdateMode { get; set; }

        /// <summary>
        ///     The <see cref="IReplyCreationViewModel" />
        /// </summary>
        IReplyCreationViewModel ReplyCreationViewModel { get; set; }

        /// <summary>
        ///     The <see cref="IConfirmCancelPopupViewModel" /> for <see cref="Reply" />
        /// </summary>
        IConfirmCancelPopupViewModel ReplyConfirmCancelPopupViewModel { get; set; }

        /// <summary>
        ///     Event callback when a user wants to link a <see cref="Comment" /> to another element
        /// </summary>
        EventCallback<Comment> OnLinkCallback { get; }

        /// <summary>
        ///     A collection of <see cref="AvailableRows" />
        /// </summary>
        List<IHaveAnnotatableItemRowViewModel> AvailableRows { get; set; }

        /// <summary>
        ///     Initializes this viewModel properties
        /// </summary>
        /// <param name="projectId">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <param name="reviewId">The <see cref="Guid" /> of the <see cref="Review" /></param>
        /// <param name="currentView">The current <see cref="View" /></param>
        /// <param name="currentParticipant">The current <see cref="Participant" /></param>
        /// <param name="onLinkCallback">The <see cref="EventCallback{TValue}" /> for linking a <see cref="Comment" /> on other element</param>
        void InitializesProperties(Guid projectId, Guid reviewId, View currentView, Participant currentParticipant, EventCallback<Comment> onLinkCallback);

        /// <summary>
        ///     Opens the creation popup
        /// </summary>
        void OpenCommentCreationPopup();

        /// <summary>
        ///     Sets the current selected <see cref="Comment" />
        /// </summary>
        /// <param name="comment">The <see cref="Comment" /></param>
        void SetCurrentComment(Comment comment);
    }
}
