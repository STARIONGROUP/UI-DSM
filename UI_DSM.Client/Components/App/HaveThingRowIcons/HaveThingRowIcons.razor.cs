// --------------------------------------------------------------------------------------------------------
// <copyright file="HaveThingRowIcons.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.App.HaveThingRowIcons
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;

    /// <summary>
    ///     Component to display icons based on a <see cref="IHaveThingRowViewModel" />
    /// </summary>
    public partial class HaveThingRowIcons
    {
        /// <summary>
        ///     The <see cref="IHaveThingRowViewModel" />
        /// </summary>
        [Parameter]
        public IHaveThingRowViewModel Row { get; set; }

        /// <summary>
        ///     Gets the css class for the <see cref="HaveThingRowIcons" />
        /// </summary>
        /// <returns>The css class</returns>
        private string GetCssClass()
        {
            return this.Row.HasComment() && this.Row.ReviewItem.IsReviewed ? "icons" : string.Empty;
        }
    }
}
