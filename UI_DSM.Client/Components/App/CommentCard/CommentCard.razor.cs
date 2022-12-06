﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="CommentCard.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.App.CommentCard
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.ViewModels.App.CommentCard;
    using UI_DSM.Client.ViewModels.App.ReplyCard;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     UI Card for a <see cref="Shared.Models.Comment" />
    /// </summary>
    public partial class CommentCard
    {
        /// <summary>
        ///     All available <see cref="StatusKind" />
        /// </summary>
        private static readonly List<StatusKind> AvailableStatus = new()
        {
            StatusKind.Open,
            StatusKind.Approved,
            StatusKind.Rejected,
            StatusKind.Closed
        };

        /// <summary>
        ///     The <see cref="ICommentCardViewModel" />
        /// </summary>
        [Parameter]
        public ICommentCardViewModel ViewModel { get; set; }

        /// <summary>
        ///     Value indicating if the component is on update mode
        /// </summary>
        public bool IsOnStatusUpdateMode { get; set; }

        /// <summary>
        ///     Handle the click event of the content edit button
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private Task OnContentEditClick()
        {
            return this.ViewModel.OnContentEditCallback.InvokeAsync(this.ViewModel.Comment);
        }

        /// <summary>
        ///     Handle the click event of the content delete button
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private Task OnDeleteClick()
        {
            return this.ViewModel.OnDeleteCallback.InvokeAsync(this.ViewModel.Comment);
        }

        /// <summary>
        ///     Creates a new <see cref="IReplyCardViewModel" /> based on a <see cref="Reply" />
        /// </summary>
        /// <param name="reply">The <see cref="Reply" /></param>
        /// <returns>The initialized <see cref="IReplyCardViewModel" /></returns>
        private IReplyCardViewModel CreateReplyCardViewModel(Reply reply)
        {
            return new ReplyCardViewModel(reply, this.ViewModel.OnContentEditReplyCallback, this.ViewModel.OnDeleteReplyCallback, this.ViewModel.CurrentParticipant);
        }

         /// <summary>
        ///     The position target
        /// </summary>
        public string PositionTarget => $"#{this.CardUniqueId}";

       /// <summary>
        ///     The unique html id
        /// </summary>
        public string CardUniqueId => $"comment_{this.ViewModel.Comment.Id}";

        /// <summary>
        ///     Handle the click event of the content reply button
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private Task OnReplyCallback()
        {
            return this.ViewModel.OnReplyCallback.InvokeAsync(this.ViewModel.Comment);
        }
    }
}