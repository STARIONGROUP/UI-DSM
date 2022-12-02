// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewTask.cs" company="RHEA System S.A.">
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
    ///    The Data Transfer Object representing the <see cref="ReviewTask" /> class.
    /// </summary>
    public partial class ReviewTaskDto : EntityDto
    {
        /// <summary>
        ///    Initializes a new <see cref="ReviewTaskDto" /> class.
        /// </summary>
        public ReviewTaskDto()
        {
            this.IsAssignedTo = new List<Guid>();
            this.Prefilters = new List<string>();
        }

        /// <summary>
        ///    Initializes a new <see cref="ReviewTaskDto" /> class.
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the represented <see cref="Entity" /></param>
        public ReviewTaskDto(Guid id) : base(id)
        {
            this.IsAssignedTo = new List<Guid>();
            this.Prefilters = new List<string>();
        }

        /// <summary>
        ///    Gets or sets the Title of the ReviewTask
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///    Gets or sets the Description of the ReviewTask
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///    Gets or sets the TaskNumber of the ReviewTask
        /// </summary>
        public int TaskNumber { get; set; }

        /// <summary>
        ///    Gets or sets the MainView of the ReviewTask
        /// </summary>
        public View MainView { get; set; }

        /// <summary>
        ///    Gets or sets the OptionalView of the ReviewTask
        /// </summary>
        public View OptionalView { get; set; }

        /// <summary>
        ///    Gets or sets the AdditionalView of the ReviewTask
        /// </summary>
        public View AdditionalView { get; set; }

        /// <summary>
        ///    Gets or sets the HasPrimaryView of the ReviewTask
        /// </summary>
        public bool HasPrimaryView { get; set; }

        /// <summary>
        ///    Gets or sets the Status of the ReviewTask
        /// </summary>
        public StatusKind Status { get; set; }

        /// <summary>
        ///    Gets or sets the CreatedOn of the ReviewTask
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        ///    Gets or sets the IsAssignedTo of the ReviewTask
        /// </summary>
        public List<Guid> IsAssignedTo { get; set; }

        /// <summary>
        ///    Gets or sets the Prefilters of the ReviewTask
        /// </summary>
        public List<string> Prefilters { get; set; }

        /// <summary>
        ///    Instantiate a <see cref="Entity" /> from a <see cref="EntityDto" />
        /// </summary>
        public override Entity InstantiatePoco()
        {
            return new ReviewTask(this.Id);
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------