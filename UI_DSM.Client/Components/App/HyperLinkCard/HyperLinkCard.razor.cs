// --------------------------------------------------------------------------------------------------------
// <copyright file="HyperLinkCard.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.App.HyperLinkCard
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;

    /// <summary>
    ///     Component to display <see cref="HyperLinkRowViewModel" />
    /// </summary>
    public partial class HyperLinkCard
    {
        /// <summary>
        ///     The <see cref="HyperLinkRowViewModel" />
        /// </summary>
        [Parameter]
        public HyperLinkRowViewModel Row { get; set; }

        /// <summary>
        ///     The <see cref="EventCallback{TValue}" /> on click
        /// </summary>
        [Parameter]
        public EventCallback<HyperLinkRowViewModel> OnClick { get; set; }
    }
}
