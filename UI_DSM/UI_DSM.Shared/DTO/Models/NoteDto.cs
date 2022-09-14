// --------------------------------------------------------------------------------------------------------
// <copyright file="NoteDto.cs" company="RHEA System S.A.">
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
    ///     The Data Transfer Object representing the <see cref="Note" /> class.
    /// </summary>
    public class NoteDto : AnnotationDto
    {
        /// <summary>
        ///     Initiazes a new <see cref="NoteDto" />
        /// </summary>
        public NoteDto()
        {
        }

        /// <summary>
        ///     Initiazes a new <see cref="NoteDto" />
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the represented <see cref="NoteDto" /></param>
        public NoteDto(Guid id) : base(id)
        {
        }

        /// <summary>
        ///     Instantiate a <see cref="Entity" /> from a <see cref="EntityDto" />
        /// </summary>
        /// <returns>A new <see cref="Entity" /></returns>
        public override Entity InstantiatePoco()
        {
            return new Note(this.Id);
        }
    }
}
