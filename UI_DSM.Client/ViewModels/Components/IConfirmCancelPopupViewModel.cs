// --------------------------------------------------------------------------------------------------------
// <copyright file="IConfirmCancelPopupViewModel.cs" company="RHEA System S.A.">
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

    /// <summary>
    ///     Interface definition for <see cref="ConfirmCancelPopupViewModel" />
    /// </summary>
    public interface IConfirmCancelPopupViewModel
    {
        /// <summary>
        ///     Value indicating is this popup is visible or not
        /// </summary>
        bool IsVisible { get; set; }

        /// <summary>
        ///     The <see cref="EventCallback" /> to call when the user cancels the action
        /// </summary>
        EventCallback OnCancel { get; set; }

        /// <summary>
        ///     The <see cref="EventCallback" /> to call when the user confirms the action
        /// </summary>
        EventCallback OnConfirm { get; set; }

        /// <summary>
        ///     The content of the header of the popup
        /// </summary>
        string HeaderText { get; set; }

        /// <summary>
        ///     The content of the body of the popup
        /// </summary>
        string ContentText { get; set; }

        /// <summary>
        ///     The <see cref="ButtonRenderStyle" /> to apply for the Cancel button
        /// </summary>
        ButtonRenderStyle CancelRenderStyle { get; set; }

        /// <summary>
        ///     The <see cref="ButtonRenderStyle" /> to apply for the Confirm button
        /// </summary>
        ButtonRenderStyle ConfirmRenderStyle { get; set; }
    }
}
