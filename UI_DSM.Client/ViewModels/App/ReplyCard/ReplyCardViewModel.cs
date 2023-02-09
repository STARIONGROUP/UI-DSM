// --------------------------------------------------------------------------------------------------------
// <copyright file="ReplyCardViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.App.ReplyCard
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     View model for the <see cref="Client.Components.App.ReplyCard.ReplyCard" /> component
    /// </summary>
    public class ReplyCardViewModel : IReplyCardViewModel
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ReplyCardViewModel" /> class.
        /// </summary>
        /// <param name="reply">The <see cref="Reply" /></param>
        /// <param name="currentParticipant">The logged <see cref="Participant" /></param>
        /// <param name="onContentEditCallback">The <see cref="EventCallback{TValue}" /> for content edition</param>
        /// <param name="onDeleteCallback">The <see cref="EventCallback{TValue}" /> for delete</param>
        public ReplyCardViewModel(Reply reply, EventCallback<Reply> onContentEditCallback, EventCallback<Reply> onDeleteCallback, Participant currentParticipant)
        {
            this.Reply = reply;
            this.OnContentEditCallback = onContentEditCallback;
            this.OnDeleteCallback = onDeleteCallback;
            this.CurrentParticipant = currentParticipant;
        }

        /// <summary>
        ///     The <see cref="Reply" />
        /// </summary>
        public Reply Reply { get; set; }

        /// <summary>
        ///     The <see cref="EventCallback{TValue}" /> when the user wants to edit the <see cref="Reply" />
        /// </summary>
        public EventCallback<Reply> OnContentEditCallback { get; set; }

        /// <summary>
        ///     The <see cref="EventCallback{TValue}" /> when the user wants to delete the <see cref="Reply" />
        /// </summary>
        public EventCallback<Reply> OnDeleteCallback { get; set; }

        /// <summary>
        ///     Value asserting if the current <see cref="Participant" /> is allow to edit the <see cref="Comment" />
        /// </summary>
        public bool IsAllowedToEdit => this.CurrentParticipant != null &&
                                       this.CurrentParticipant.IsAllowedTo(AccessRight.ReviewTask) &&  this.Reply.Author.Id == this.CurrentParticipant.Id;

        /// <summary>
        ///     The currently logged <see cref="Participant" />
        /// </summary>
        private Participant CurrentParticipant { get; set; }
    }
}
