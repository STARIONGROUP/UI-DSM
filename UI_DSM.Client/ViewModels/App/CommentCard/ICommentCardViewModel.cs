// --------------------------------------------------------------------------------------------------------
// <copyright file="ICommentCardViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.App.CommentCard
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.Components.App.ReplyCard;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface definition for <see cref="CommentCardViewModel" />
    /// </summary>
    public interface ICommentCardViewModel
    {
        /// <summary>
        ///     The <see cref="Comment" />
        /// </summary>
        Comment Comment { get; set; }

        /// <summary>
        ///     The <see cref="EventCallback{TValue}" /> when the user wants to edit the <see cref="Comment" />
        /// </summary>
        EventCallback<Comment> OnContentEditCallback { get; set; }

        /// <summary>
        ///     The <see cref="EventCallback{TValue}" /> when the user wants to navigate to the <see cref="Comment" />
        /// </summary>
        EventCallback<Comment> OnNavigateCallback { get; set; }

        /// <summary>
        ///     The <see cref="EventCallback{TValue}" /> when the user wants to delete the <see cref="Comment" />
        /// </summary>
        EventCallback<Comment> OnDeleteCallback { get; set; }

        /// <summary>
        ///     Value asserting if the current <see cref="Participant" /> is allow to edit the <see cref="Comment" />
        /// </summary>
        bool IsAllowedToEdit { get; }

        /// <summary>
        ///     The <see cref="EventCallback" /> when the user wants to reply to the <see cref="Comment" />
        /// </summary>
        EventCallback<Comment> OnReplyCallback { get; set; }

        /// <summary>
        ///     The current selected <see cref="ReplyCard" />
        /// </summary>
        ReplyCard SelectedReplyCard { get; set; }

        /// <summary>
        ///     The currently logged <see cref="Participant" />
        /// </summary>
        Participant CurrentParticipant { get; }

        /// <summary>
        ///     <see cref="EventCallback{TValue}" /> when we want to edit a <see cref="Reply" />
        /// </summary>
        EventCallback<Reply> OnContentEditReplyCallback { get; set; }

        /// <summary>
        ///     <see cref="EventCallback{TValue}" /> when we want to delete a <see cref="Reply" />
        /// </summary>
        EventCallback<Reply> OnDeleteReplyCallback { get; set; }

        /// <summary>
        ///     The <see cref="EventCallback" /> when the user wants to link other element to the <see cref="Comment" />
        /// </summary>
        EventCallback<Comment> OnLinkCallback { get; set; }

        /// <summary>
        ///     A collection of all <see cref="LinkedRows" />
        /// </summary>
        List<IHaveAnnotatableItemRowViewModel> LinkedRows { get; }

        /// <summary>
        ///     Update the status of the <see cref="Comment" />
        /// </summary>
        /// <param name="status">The new status</param>
        /// <returns>The <see cref="Task" /></returns>
        Task UpdateStatus(StatusKind status);
    }
}
