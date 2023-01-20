// --------------------------------------------------------------------------------------------------------
// <copyright file="PointDto.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Shared.DTO.Common
{
    /// <summary>
    ///     Data Transfer Object that represent a Point, with x and y position
    /// </summary>
    public class PointDto
    {
        /// <summary>
        ///     The x position
        /// </summary>
        public double X { get; set; }

        /// <summary>
        ///     The y position
        /// </summary>
        public double Y { get; set; }
    }
}
