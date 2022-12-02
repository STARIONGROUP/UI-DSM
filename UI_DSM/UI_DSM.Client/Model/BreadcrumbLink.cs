// --------------------------------------------------------------------------------------------------------
// <copyright file="BreadcrumbLink.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Model
{
    /// <summary>
    /// Class to keep track of the links in the <see cref="AppComponents.AppBreadcrumb"/>
    /// </summary>
    public class BreadcrumbLink
    {
        /// <summary>
        /// Gets or sets the title of the link
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the uri of the link
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// Creates a new instance of the type <see cref="BreadcrumbLinks"/>
        /// </summary>
        /// <param name="title">title of the link</param>
        /// <param name="uri">the absolute uri to navigate to</param>
        public BreadcrumbLink(string title, string uri)
        {
            this.Title = title;
            this.Uri = uri;
        }
    }
}
