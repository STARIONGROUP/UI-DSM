// --------------------------------------------------------------------------------------------------------
// <copyright file="PortDirection.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Jaime Bernar
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Client.Enumerator
{
    /// <summary>
    /// Enumeration that defines the different port directions
    /// </summary>
    public enum PortDirection
    {
        /// <summary>
        /// Port with InOut direction
        /// </summary>
        InOut = 0,

        /// <summary>
        /// Port with Out direction
        /// </summary>
        Out = 1,

        /// <summary>
        /// Port with In direction
        /// </summary>
        In = 2,
    }
}
