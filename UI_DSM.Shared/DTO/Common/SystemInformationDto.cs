// --------------------------------------------------------------------------------------------------------
// <copyright file="SystemInformationDto.cs" company="RHEA System S.A.">
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
    ///     Data Transfer Object to transfer information related to the current system (Uptime, DLL version,...)
    /// </summary>
    public class SystemInformationDto
    {
        /// <summary>
        ///     Verify the the server is alive
        /// </summary>
        public bool IsAlive { get; set; }

        /// <summary>
        ///     The start time of the server
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        ///     A collection of <see cref="AssemblyInformationDto" />
        /// </summary>
        public List<AssemblyInformationDto> AssembliesInformation { get; set; }

        /// <summary>
        ///     The <see cref="DatabaseInformationDto" />
        /// </summary>
        public DatabaseInformationDto DataBaseInformation { get; set; }
    }
}
