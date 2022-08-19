// --------------------------------------------------------------------------------------------------------
// <copyright file="Entity.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Shared.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using UI_DSM.Shared.DTO.Models;

    /// <summary>
    ///     Top level abstract superclass from which all domain concept classes in the model inherit
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        ///     Initializes a new <see cref="Entity" />
        /// </summary>
        protected Entity()
        {
        }

        /// <summary>
        ///     Inilializes a new <see cref="Entity" />
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the <see cref="Entity" /></param>
        protected Entity(Guid id)
        {
            this.Id = id;
        }

        /// <summary>
        ///     Gets or sets the Universally Unique Identifier (UUID) that uniquely identifies an instance of <see cref="Entity" />
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="Entity" /> container of the current <see cref="Entity" />
        /// </summary>
        public Entity EntityContainer { get; set; }

        /// <summary>
        ///     Instantiate a <see cref="EntityDto" /> from a <see cref="Entity" />
        /// </summary>
        /// <returns>A new <see cref="EntityDto" /></returns>
        public abstract EntityDto ToDto();

        /// <summary>
        ///     Resolve the properties of the current <see cref="Entity" /> from its <see cref="EntityDto" /> counter-part
        /// </summary>
        /// <param name="entityDto">The source <see cref="EntityDto" /></param>
        public abstract void ResolveProperties(EntityDto entityDto);
    }
}
