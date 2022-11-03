// --------------------------------------------------------------------------------------------------------
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

    using UI_DSM.Client.Components.App.ReplyCard;
    using UI_DSM.Client.Components.App.SelectableComponent;
    using UI_DSM.Client.ViewModels.App.CommentCard;
    using UI_DSM.Client.ViewModels.App.ReplyCard;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     UI Card for a <see cref="Shared.Models.Comment" />
    /// </summary>
    public partial class CommentCard : SelectableComponent
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
        ///     The currently selected <see cref="ReplyCard" />
        /// </summary>
        private ReplyCard selectedReplyCard;

        /// <summary>
        ///     The <see cref="ICommentCardViewModel" />
        /// </summary>
        [Parameter]
        public ICommentCardViewModel ViewModel { get; set; }

        /// <summary>
        ///     <see cref="EventCallback{CommentCard}" /> when a click is performed
        /// </summary>
        [Parameter]
        public EventCallback<CommentCard> OnClick { get; set; }

        /// <summary>
        ///     Value indicating if the component is on update mode
        /// </summary>
        public bool IsOnStatusUpdateMode { get; set; }

        /// <summary>
        ///     Selects the current component
        /// </summary>
        public override void Select()
        {
            this.DeselectReplyCard();
            base.Select();
        }

        /// <summary>
        ///     Deselects the current component
        /// </summary>
        public override void Deselect()
        {
            this.DeselectReplyCard();
            base.Deselect();
        }

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
        ///     Handle the onClick event
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private Task OnClickEvent()
        {
            return this.OnClick.InvokeAsync(this);
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
        ///     Handle the selection of a <see cref="ReplyCard" />
        /// </summary>
        /// <param name="replyCard">The new selected <see cref="ReplyCard" /></param>
        private void OnReplyCardClick(ReplyCard replyCard)
        {
            this.selectedReplyCard?.Deselect();
            this.selectedReplyCard = replyCard;
            this.selectedReplyCard.Select();
        }

        /// <summary>
        ///     Force to deselect and reset the <see cref="selectedReplyCard" />
        /// </summary>
        private void DeselectReplyCard()
        {
            this.selectedReplyCard?.Deselect();
            this.selectedReplyCard = null;
        }
    }
}
