// --------------------------------------------------------------------------------------------------------
// <copyright file="StatusKind.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Shared.Enumerator
{
    /// <summary>
    ///     Enumeration representing a status of a review, task, annotations
    /// </summary>
    public enum StatusKind
    {
        /// <summary>
        ///     When the thing is open
        /// </summary>
        Open = 0,

        /// <summary>
        ///     When the thing is done but needs to be verified
        /// </summary>
        Done = 1,

        /// <summary>
        ///     When the thing is done and verified
        /// </summary>
        Closed = 2,

        /// <summary>
        ///     When the thing has been approved
        /// </summary>
        Approved = 3,

        /// <summary>
        ///     When the thing has been rejected
        /// </summary>
        Rejected = 4
    }
}
