// --------------------------------------------------------------------------------------------------------
// <copyright file="ProjectDto.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Shared.DTO.Models
{
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     The Data Transfer Object representing the <see cref="Project" /> class.
    /// </summary>
    public class ProjectDto : EntityDto
    {
        /// <summary>
        ///     Initiazes a new <see cref="ProjectDto" />
        /// </summary>
        public ProjectDto()
        {
            this.InitializeCollections();
        }

        /// <summary>
        ///     Initiazes a new <see cref="ProjectDto" />
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the represented <see cref="Project" /></param>
        public ProjectDto(Guid id) : base(id)
        {
            this.InitializeCollections();
        }

        /// <summary>
        ///     Gets or sets the name of the <see cref="Project" />
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        ///     Gets or sets the collection of <see cref="Guid" /> that represents <see cref="Participant" />s
        /// </summary>
        public List<Guid> Participants { get; set; }

        /// <summary>
        ///     Gets or sets the collection of <see cref="Guid" /> that represents <see cref="Review" />s
        /// </summary>
        public List<Guid> Reviews { get; set; }

        /// <summary>
        ///     Instantiate a <see cref="Project" /> from a <see cref="ProjectDto" />
        /// </summary>
        /// <returns>A new <see cref="Project" /></returns>
        public override Entity InstantiatePoco()
        {
            return new Project(this.Id);
        }

        /// <summary>
        ///     Initializes all collections for this <see cref="EntityDto" />
        /// </summary>
        private void InitializeCollections()
        {
            this.Participants = new List<Guid>();
            this.Reviews = new List<Guid>();
        }
    }
}
