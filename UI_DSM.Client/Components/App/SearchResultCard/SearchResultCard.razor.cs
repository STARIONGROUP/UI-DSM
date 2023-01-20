// --------------------------------------------------------------------------------------------------------
// <copyright file="SearchResultCard.razor.cs" company="RHEA System S.A.">
//  Copyright (c) 2023 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw, Nabil Abbar
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Client.Components.App.SearchResultCard
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Shared.DTO.Common;

    /// <summary>
    ///     Component to display <see cref="SearchResultDto" />
    /// </summary>
    public partial class SearchResultCard
    {
        /// <summary>
        ///     The <see cref="SearchResultDto" /> to display
        /// </summary>
        [Parameter]
        public SearchResultDto SearchResult { get; set; }

        /// <summary>
        ///     <see cref="EventCallback{TValue}" /> to call on click
        /// </summary>
        [Parameter]
        public EventCallback<SearchResultDto> OnClick { get; set; }

        /// <summary>
        ///     Handles the onclick event
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private async Task HandleOnClick()
        {
            if (this.OnClick.HasDelegate)
            {
                await this.InvokeAsync(() => this.OnClick.InvokeAsync(this.SearchResult));
            }
        }
    }
}
