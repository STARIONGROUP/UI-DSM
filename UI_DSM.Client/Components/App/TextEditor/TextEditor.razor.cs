// --------------------------------------------------------------------------------------------------------
// <copyright file="TextEditor.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.App.TextEditor
{
    using Microsoft.AspNetCore.Components;

    /// <summary>
    ///     Component that allows to edit rich text
    /// </summary>
    public partial class TextEditor
    {
        /// <summary>
        ///     References to the current content of the text
        /// </summary>
        [Parameter]
        public string Content { get; set; }

        /// <summary>
        ///     The <see cref="EventCallback" /> for submit
        /// </summary>
        [Parameter]
        public EventCallback<string> OnValidSubmit { get; set; }

        /// <summary>
        ///     Reference to the <see cref="ErrorMessage" />
        /// </summary>
        public ErrorMessage ErrorMessage { get; set; }

        /// <summary>
        ///     The text that has to be displayed into the submit button
        /// </summary>
        [Parameter]
        public string SubmitText { get; set; }

        /// <summary>
        ///     Handle the update of the current content
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private async Task OnSubmit()
        {
            if (this.Content == "<p><br></p>" || string.IsNullOrEmpty(this.Content))
            {
                this.ErrorMessage.ViewModel.HandleErrors(new List<string> { "The content cannot be empty" });
            }
            else
            {
                await this.InvokeAsync(() => this.OnValidSubmit.InvokeAsync(this.Content));
            }
        }
    }
}
