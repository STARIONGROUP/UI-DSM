// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewObjectiveKind.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Shared.Enumerator
{
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Enumeration that define kinds of <see cref="ReviewObjective" />
    /// </summary>
    public enum ReviewObjectiveKind
    {
        /// <summary>
        ///     For PRELIMINARY REQUIREMENTS REVIEW
        /// </summary>
        Prr = 0,

        /// <summary>
        ///     For SYSTEM REQUIREMENTS REVIEW
        /// </summary>
        Srr = 1
    }
}
