// --------------------------------------------------------------------------------------------------------
// <copyright file="RelatedViews.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.App.RelatedViews
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Components that display all <see cref="View" />s that could be related to the current one
    /// </summary>
    public partial class RelatedViews
    {
        /// <summary>
        ///     A related view that is the main <see cref="View" /> or an optional <see cref="View" /> of the
        ///     <see cref="ReviewTask" />
        /// </summary>
        [Parameter]
        public View MainRelatedView { get; set; } = View.None;

        /// <summary>
        ///     A collection of other related <see cref="View" />s
        /// </summary>
        [Parameter]
        public List<View> OtherRelatedViews { get; set; }

        /// <summary>
        ///     The <see cref="EventCallback{TValue}" /> when a <see cref="View" /> is selected
        /// </summary>
        [Parameter]
        public EventCallback<View> OnViewSelect { get; set; }
        
        /// <summary>
        ///     Value indicating if the panel is open
        /// </summary>
        public bool IsPanelOpen { get; set; } = true;

        /// <summary>
        /// Handle the click on a <see cref="View"/>
        /// </summary>
        /// <param name="view">The <see cref="View"/></param>
        /// <returns>A <see cref="Task"/></returns>
        private Task OnClick(View view)
        {
            return this.OnViewSelect.InvokeAsync(view);
        }
    }
}
