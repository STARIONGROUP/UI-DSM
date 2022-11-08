// --------------------------------------------------------------------------------------------------------
// <copyright file="ReplyCreation.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.App.ReplyCreation
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.ViewModels.App.ReplyCreation;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Component that allow a user to create or edit a <see cref="Reply" />
    /// </summary>
    public partial class ReplyCreation
    {
        /// <summary>
        ///     The <see cref="IReplyCreationViewModel" />
        /// </summary>
        [Parameter]
        public IReplyCreationViewModel ViewModel { get; set; }

        /// <summary>
        ///     Handle the update of the current content of the <see cref="Comment" />
        /// </summary>
        /// <param name="content">The new content</param>
        /// <returns>A <see cref="Task" /></returns>
        private async Task OnValidSubmit(string content)
        {
            this.ViewModel.Reply.Content = content;
            await this.InvokeAsync(this.ViewModel.OnValidSubmit.InvokeAsync);
        }
    }
}
