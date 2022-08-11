// --------------------------------------------------------------------------------------------------------
// <copyright file="ConfirmCancelPopupViewModel.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Client.ViewModels.Components
{
    using DevExpress.Blazor;

    using Microsoft.AspNetCore.Components;

    using ReactiveUI;

    /// <summary>
    ///     Viewmodel for the <see cref="ConfirmCancelPopupViewModel" /> component
    /// </summary>
    public class ConfirmCancelPopupViewModel : ReactiveObject, IConfirmCancelPopupViewModel
    {
        /// <summary>
        ///     Backing field for <see cref="IsVisible" />
        /// </summary>
        private bool isVisible;

        /// <summary>
        ///     Value indicating is this popup is visible or not
        /// </summary>
        public bool IsVisible
        {
            get => this.isVisible;
            set => this.RaiseAndSetIfChanged(ref this.isVisible, value);
        }

        /// <summary>
        ///     The <see cref="EventCallback" /> to call when the user cancels the action
        /// </summary>
        public EventCallback OnCancel { get; set; }

        /// <summary>
        ///     The <see cref="EventCallback" /> to call when the user confirms the action
        /// </summary>
        public EventCallback OnConfirm { get; set; }

        /// <summary>
        ///     The <see cref="ButtonRenderStyle" /> to apply for the Cancel button
        /// </summary>
        public ButtonRenderStyle CancelRenderStyle { get; set; } = ButtonRenderStyle.Secondary;

        /// <summary>
        ///     The <see cref="ButtonRenderStyle" /> to apply for the Confirm button
        /// </summary>
        public ButtonRenderStyle ConfirmRenderStyle { get; set; } = ButtonRenderStyle.Primary;

        /// <summary>
        ///     The content of the header of the popup
        /// </summary>
        public string HeaderText { get; set; } = "Please confirm";

        /// <summary>
        ///     The content of the body of the popup
        /// </summary>
        public string ContentText { get; set; }
    }
}
