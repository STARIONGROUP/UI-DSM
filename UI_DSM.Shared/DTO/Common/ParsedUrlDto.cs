// --------------------------------------------------------------------------------------------------------
// <copyright file="ParsedUrlDto.cs" company="RHEA System S.A.">
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
    ///     Data transfer object that will contains computed data after parser a URL, for breadcrumb use
    /// </summary>
    public class ParsedUrlDto
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ParsedUrlDto" /> class.
        /// </summary>
        /// <param name="displayName">The display name</param>
        /// <param name="url">The url</param>
        public ParsedUrlDto(string displayName, string url)
        {
            this.DisplayName = displayName;
            this.Url = url;
        }

        /// <summary>
        ///     The name to display
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        ///     The linked url
        /// </summary>
        public string Url { get; set; }
    }
}
