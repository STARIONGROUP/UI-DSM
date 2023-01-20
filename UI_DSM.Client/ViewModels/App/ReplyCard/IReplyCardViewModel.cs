// --------------------------------------------------------------------------------------------------------
// <copyright file="IReplyCardViewModel.cs" company="RHEA System S.A.">
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

    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface definition for <see cref="ReplyCardViewModel" />
    /// </summary>
    public interface IReplyCardViewModel
    {
        /// <summary>
        ///     The <see cref="Reply" />
        /// </summary>
        Reply Reply { get; set; }

        /// <summary>
        ///     The <see cref="EventCallback{TValue}" /> when the user wants to edit the <see cref="Reply" />
        /// </summary>
        EventCallback<Reply> OnContentEditCallback { get; set; }

        /// <summary>
        ///     The <see cref="EventCallback{TValue}" /> when the user wants to delete the <see cref="Reply" />
        /// </summary>
        EventCallback<Reply> OnDeleteCallback { get; set; }

        /// <summary>
        ///     Value asserting if the current <see cref="Participant" /> is allow to edit the <see cref="Comment" />
        /// </summary>
        bool IsAllowedToEdit { get; }
    }
}
