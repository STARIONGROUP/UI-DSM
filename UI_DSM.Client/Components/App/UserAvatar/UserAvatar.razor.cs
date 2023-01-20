// --------------------------------------------------------------------------------------------------------
// <copyright file="UserAvatar.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.App.UserAvatar
{
    using Microsoft.AspNetCore.Components;

    /// <summary>
    ///     Component to display the avatar or initials of an user
    /// </summary>
    public partial class UserAvatar
    {
        /// <summary>
        ///     The initials of the user
        /// </summary>
        [Parameter]
        public string UserInitials { get; set; }

        /// <summary>
        ///     The path to the avatar, if any
        /// </summary>
        [Parameter]
        public string UserImage { get; set; }

        /// <summary>
        ///     The full name of the user
        /// </summary>
        [Parameter]
        public string UserName { get; set; }

        /// <summary>
        ///     Assert that the avatar is representing the current user
        /// </summary>
        [Parameter]
        public bool IsCurrentUser { get; set; }

        /// <summary>
        ///     Asserting that an avatar has been defined
        /// </summary>
        private bool HasImage => !string.IsNullOrEmpty(this.UserImage);
    }
}
