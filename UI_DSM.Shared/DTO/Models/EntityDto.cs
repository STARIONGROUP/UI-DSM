// --------------------------------------------------------------------------------------------------------
// <copyright file="EntityDto.cs" company="RHEA System S.A.">
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
    ///     The Data Transfer Object representing the abstract <see cref="Entity" /> class.
    /// </summary>
    public abstract class EntityDto
    {
        /// <summary>
        ///     Initiazes a new <see cref="EntityDto" />
        /// </summary>
        protected EntityDto()
        {
        }

        /// <summary>
        ///     Initiazes a new <see cref="EntityDto" />
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the represented <see cref="Entity" /></param>
        protected EntityDto(Guid id)
        {
            this.Id = id;
        }

        /// <summary>
        ///     Gets or sets the Universally Unique Identifier (UUID) that uniquely identifies an instance of the represented
        ///     <see cref="Entity" />
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     Instantiate a <see cref="Entity" /> from a <see cref="EntityDto" />
        /// </summary>
        /// <returns>A new <see cref="Entity" /></returns>
        public abstract Entity InstantiatePoco();
    }
}
