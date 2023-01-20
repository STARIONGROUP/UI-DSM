// --------------------------------------------------------------------------------------------------------
// <copyright file="AnnotationDto.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Shared.DTO.Models
{
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     The Data Transfer Object representing the <see cref="Annotation" /> class.
    /// </summary>
    public partial class AnnotationDto
    {
        /// <summary>
        ///     Includes common properties for <see cref="AnnotationDto" /> from an <see cref="Annotation" />
        /// </summary>
        /// <param name="annotation">The <see cref="Annotation" /></param>
        public void IncludeCommonProperties(Annotation annotation)
        {
            this.Author = annotation.Author?.Id ?? Guid.Empty;
            this.CreatedOn = annotation.CreatedOn;
            this.AnnotatableItems = annotation.AnnotatableItems.Select(x => x.Id).ToList();
            this.Content = annotation.Content;
        }
    }
}
