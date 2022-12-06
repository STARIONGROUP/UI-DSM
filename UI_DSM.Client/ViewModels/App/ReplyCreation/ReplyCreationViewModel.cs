// --------------------------------------------------------------------------------------------------------
// <copyright file="ReplyCreationViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.App.ReplyCreation
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Shared.Models;

    /// <summary>
    ///     View model for <see cref="Client.Components.App.ReplyCreation.ReplyCreation" />
    /// </summary>
    public class ReplyCreationViewModel : IReplyCreationViewModel
    {
        /// <summary>
        ///     The <see cref="Reply" />
        /// </summary>
        public Reply Reply { get; set; }

        /// <summary>
        ///     The <see cref="EventCallback" /> for submit
        /// </summary>
        public EventCallback OnValidSubmit { get; set; }
    }
}
