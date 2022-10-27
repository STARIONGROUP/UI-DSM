// --------------------------------------------------------------------------------------------------------
// <copyright file="ErrorMessageViewModel.cs" company="RHEA System S.A.">
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
    using DynamicData;

    using UI_DSM.Client.Components;

    /// <summary>
    ///     View model for <see cref="ErrorMessage" /> component
    /// </summary>
    public class ErrorMessageViewModel : IErrorMessageViewModel
    {
        /// <summary>
        ///     A collection of error message to display
        /// </summary>
        public SourceList<string> Errors { get; private set; } = new();

        /// <summary>
        ///     Display the new collection of errors
        /// </summary>
        /// <param name="errors">The new collection of errors</param>
        public void HandleErrors(IEnumerable<string> errors)
        {
            this.Errors.Clear();
            this.Errors.AddRange(errors);
        }
    }
}
