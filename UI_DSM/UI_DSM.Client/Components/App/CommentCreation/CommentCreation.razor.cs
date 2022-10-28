// --------------------------------------------------------------------------------------------------------
// <copyright file="CommentCreation.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.App.CommentCreation
{
    using System.Xml.Linq;

    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.ViewModels.App.CommentCreation;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Component to allow a user to create a <see cref="Comment" />
    /// </summary>
    public partial class CommentCreation
    {
        /// <summary>
        ///     The <see cref="ICommentCreationViewModel" />
        /// </summary>
        [Parameter]
        public ICommentCreationViewModel ViewModel { get; set; }

        /// <summary>
        ///     Reference to the <see cref="ErrorMessage" />
        /// </summary>
        public ErrorMessage ErrorMessage { get; set; }

        /// <summary>
        ///     Handle the update of the current content of the <see cref="Comment" />
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private async Task OnSubmit()
        {
            if (this.ViewModel.Comment.Content == "<p><br></p>" ||
                string.IsNullOrEmpty(this.ViewModel.Comment.Content) ||
                string.IsNullOrWhiteSpace(XElement.Parse(this.ViewModel.Comment.Content.Replace("&nbsp;", "")).Value))
            {
                this.ErrorMessage.ViewModel.HandleErrors(new List<string> { "The comment cannot have an empty content" });
            }
            else
            {
                await this.InvokeAsync(this.ViewModel.OnValidSubmit.InvokeAsync);
            }
        }
    }
}
