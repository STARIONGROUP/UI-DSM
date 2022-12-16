// --------------------------------------------------------------------------------------------------------
// <copyright file="AppKeyValues.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.App.AppKeyValues
{
    using Microsoft.AspNetCore.Components;

    /// <summary>
    ///     Component to display a pair of Key-Value where value is a collection of <see cref="string" />
    /// </summary>
    public partial class AppKeyValues
    {
        /// <summary>
        ///     Key label name
        /// </summary>
        [Parameter]
        public string Key { get; set; }

        /// <summary>
        ///     Values list
        /// </summary>
        [Parameter]
        public IEnumerable<string> Items { get; set; }

        /// <summary>
        ///     The <see cref="EventCallback{TValue}" /> to handle the double click on a property for navigation
        /// </summary>
        [Parameter]
        public EventCallback<string> OnItemDoubleClick { get; set; }

        /// <summary>
        ///     Value indicating if the panel is open
        /// </summary>
        public bool IsPanelOpen { get; set; }

        /// <summary>
        ///     Fire the <see cref="OnItemDoubleClick" /> callback
        /// </summary>
        /// <param name="propertyName">The name of the double-clicked property</param>
        /// <returns>A <see cref="Task" /></returns>
        private async Task OnDoubleClick(string propertyName)
        {
            if (this.OnItemDoubleClick.HasDelegate)
            {
                await this.InvokeAsync(() => this.OnItemDoubleClick.InvokeAsync(propertyName));
            }
        }
    }
}
