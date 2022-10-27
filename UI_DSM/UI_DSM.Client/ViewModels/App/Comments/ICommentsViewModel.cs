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
    using UI_DSM.Client.ViewModels.Components;
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
        ///     Value indicating if the current view is on creation
        /// </summary>
        bool IsOnCreationMode { get; set; }

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
        IConfirmCancelPopupViewModel ConfirmCancelPopupViewModel { get; set; }

        /// <summary>
        ///     Value indicating if the current view is on update mode
        /// </summary>
        bool IsOnUpdateMode { get; set; }

        /// <summary>
        ///     Event callback when a user wants to update the stauts of a <see cref="Comment" />
        /// </summary>
        EventCallback<Comment> OnUpdateStatusCallback { get; set; }

        /// <summary>
        ///     Initializes this viewModel properties
        /// </summary>
        /// <param name="projectId">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <param name="reviewId">The <see cref="Guid" /> of the <see cref="Review" /></param>
        /// <param name="currentView">The current <see cref="View" /></param>
        /// <returns>A <see cref="Task" /></returns>
        Task InitializesProperties(Guid projectId, Guid reviewId, View currentView);

        /// <summary>
        ///     Opens the creation popup
        /// </summary>
        void OpenCreationPopup();
    }
}
