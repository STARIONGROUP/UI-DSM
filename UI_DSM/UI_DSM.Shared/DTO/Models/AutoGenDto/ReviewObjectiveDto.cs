// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewObjective.cs" company="RHEA System S.A.">
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
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///    The Data Transfer Object representing the <see cref="ReviewObjective" /> class.
    /// </summary>
    public partial class ReviewObjectiveDto : AnnotatableItemDto
    {
        /// <summary>
        ///    Initializes a new <see cref="ReviewObjectiveDto" /> class.
        /// </summary>
        public ReviewObjectiveDto()
        {
            this.ReviewTasks = new List<Guid>();
            this.RelatedViews = new List<View>();
        }

        /// <summary>
        ///    Initializes a new <see cref="ReviewObjectiveDto" /> class.
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the represented <see cref="Entity" /></param>
        public ReviewObjectiveDto(Guid id) : base(id)
        {
            this.ReviewTasks = new List<Guid>();
            this.RelatedViews = new List<View>();
        }

        /// <summary>
        ///    Gets or sets the Title of the ReviewObjective
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///    Gets or sets the Description of the ReviewObjective
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///    Gets or sets the ReviewObjectiveKind of the ReviewObjective
        /// </summary>
        public ReviewObjectiveKind ReviewObjectiveKind { get; set; }

        /// <summary>
        ///    Gets or sets the ReviewObjectiveKindNumber of the ReviewObjective
        /// </summary>
        public int ReviewObjectiveKindNumber { get; set; }

        /// <summary>
        ///    Gets or sets the ReviewObjectiveNumber of the ReviewObjective
        /// </summary>
        public int ReviewObjectiveNumber { get; set; }

        /// <summary>
        ///    Gets or sets the Status of the ReviewObjective
        /// </summary>
        public StatusKind Status { get; set; }

        /// <summary>
        ///    Gets or sets the ReviewTasks of the ReviewObjective
        /// </summary>
        public List<Guid> ReviewTasks { get; set; }

        /// <summary>
        ///    Gets or sets the RelatedViews of the ReviewObjective
        /// </summary>
        public List<View> RelatedViews { get; set; }

        /// <summary>
        ///    Instantiate a <see cref="Entity" /> from a <see cref="EntityDto" />
        /// </summary>
        public override Entity InstantiatePoco()
        {
            return new ReviewObjective(this.Id);
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------