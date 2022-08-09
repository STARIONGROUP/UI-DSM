// --------------------------------------------------------------------------------------------------------
// <copyright file="RequestResponseDto.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Shared.DTO.Common
{
    /// <summary>
    /// Common DTO used to respond to all basic operation (creation/update/delete) request
    /// </summary>
    public class RequestResponseDto
    {
        /// <summary>
        /// Value indicating if the request has proceed successfuly
        /// </summary>
        public bool IsRequestSuccessful { get; set; }

        /// <summary>
        /// A collection of errors
        /// </summary>
        public IEnumerable<string> Errors { get; set; } 
    }
}
