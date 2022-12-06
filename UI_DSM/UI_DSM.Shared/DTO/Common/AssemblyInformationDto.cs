// --------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyInformationDto.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Shared.DTO.Common
{
    /// <summary>
    ///     Data Transfer Object to transfer information related to an Assembly
    /// </summary>
    public class AssemblyInformationDto
    {
        /// <summary>
        ///     The name of the Assembly
        /// </summary>
        public string AssemblyName { get; set; }

        /// <summary>
        ///     The Version of the Assembly
        /// </summary>
        public Version AssemblyVersion { get; set; }
    }
}
