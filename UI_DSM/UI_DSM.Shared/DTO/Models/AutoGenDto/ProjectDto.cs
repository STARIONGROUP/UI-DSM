// --------------------------------------------------------------------------------------------------------
// <copyright file="Project.cs" company="RHEA System S.A.">
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
    ///    The Data Transfer Object representing the <see cref="Project" /> class.
    /// </summary>
    public partial class ProjectDto : EntityDto
    {
        /// <summary>
        ///    Initializes a new <see cref="ProjectDto" /> class.
        /// </summary>
        public ProjectDto()
        {
            this.Participants = new List<Guid>();
            this.Reviews = new List<Guid>();
            this.Annotations = new List<Guid>();
        }

        /// <summary>
        ///    Initializes a new <see cref="ProjectDto" /> class.
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the represented <see cref="Entity" /></param>
        public ProjectDto(Guid id) : base(id)
        {
            this.Participants = new List<Guid>();
            this.Reviews = new List<Guid>();
            this.Annotations = new List<Guid>();
        }

        /// <summary>
        ///    Gets or sets the ProjectName of the Project
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        ///    Gets or sets the Participants of the Project
        /// </summary>
        public List<Guid> Participants { get; set; }

        /// <summary>
        ///    Gets or sets the Reviews of the Project
        /// </summary>
        public List<Guid> Reviews { get; set; }

        /// <summary>
        ///    Gets or sets the Annotations of the Project
        /// </summary>
        public List<Guid> Annotations { get; set; }

        /// <summary>
        ///    Instantiate a <see cref="Entity" /> from a <see cref="EntityDto" />
        /// </summary>
        public override Entity InstantiatePoco()
        {
            return new Project(this.Id);
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------