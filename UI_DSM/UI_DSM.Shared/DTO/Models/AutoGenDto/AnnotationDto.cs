// --------------------------------------------------------------------------------------------------------
// <copyright file="Annotation.cs" company="RHEA System S.A.">
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

// --------------------------------------------------------------------------------------------------------
// ------------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!------------
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Shared.DTO.Models
{
    using UI_DSM.Shared.Models;

    /// <summary>
    ///    The Data Transfer Object representing the <see cref="Annotation" /> class.
    /// </summary>
    public abstract partial class AnnotationDto : EntityDto
    {
        /// <summary>
        ///    Initializes a new <see cref="AnnotationDto" /> class.
        /// </summary>
        protected AnnotationDto()
        {
            this.AnnotatableItems = new List<Guid>();
        }

        /// <summary>
        ///    Initializes a new <see cref="AnnotationDto" /> class.
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the represented <see cref="Entity" /></param>
        protected AnnotationDto(Guid id) : base(id)
        {
            this.AnnotatableItems = new List<Guid>();
        }

        /// <summary>
        ///    Gets or sets the Author of the Annotation
        /// </summary>
        public Guid Author { get; set; }

        /// <summary>
        ///    Gets or sets the CreatedOn of the Annotation
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        ///    Gets or sets the AnnotatableItems of the Annotation
        /// </summary>
        public List<Guid> AnnotatableItems { get; set; }

        /// <summary>
        ///    Gets or sets the Content of the Annotation
        /// </summary>
        public string Content { get; set; }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------