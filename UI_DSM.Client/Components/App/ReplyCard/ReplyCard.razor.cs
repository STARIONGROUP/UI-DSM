// --------------------------------------------------------------------------------------------------------
// <copyright file="ReplyCard.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.App.ReplyCard
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.ViewModels.App.ReplyCard;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Component to display and manage <see cref="Reply" />
    /// </summary>
    public partial class ReplyCard
    {
        /// <summary>
        ///     The <see cref="IReplyCardViewModel" />
        /// </summary>
        [Parameter]
        public IReplyCardViewModel ViewModel { get; set; }

        /// <summary>
        ///     Handle the click event of the content edit button
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private Task OnContentEditClick()
        {
            return this.ViewModel.OnContentEditCallback.InvokeAsync(this.ViewModel.Reply);
        }

        /// <summary>
        ///     Handle the click event of the content delete button
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private Task OnDeleteClick()
        {
            return this.ViewModel.OnDeleteCallback.InvokeAsync(this.ViewModel.Reply);
        }
    }
}
