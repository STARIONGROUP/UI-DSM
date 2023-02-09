// --------------------------------------------------------------------------------------------------------
// <copyright file="Model.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Shared.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    using UI_DSM.Shared.DTO.Models;

    /// <summary>
    ///     Represents a E-TM-10-25 model uploaded and linked to the project.
    /// </summary>
    [Table(nameof(Model))]
    public class Model : Artifact
    {
        /// <summary>
        ///     Initializes a new <see cref="Entity" />
        /// </summary>
        public Model()
        {
        }

        /// <summary>
        ///     Inilializes a new <see cref="Entity" />
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the <see cref="Entity" /></param>
        public Model(Guid id) : base(id)
        {
        }

        /// <summary>
        ///     The name of the <see cref="Model" />
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ModelName { get; set; }

        /// <summary>
        ///     The <see cref="Guid" /> of the <see cref="CDP4Common.EngineeringModelData.Iteration" />
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid IterationId { get; set; }

        /// <summary>
        ///     Resolve the properties of the current <see cref="Entity" /> from its <see cref="EntityDto" /> counter-part
        /// </summary>
        /// <param name="entityDto">The source <see cref="EntityDto" /></param>
        /// <param name="resolvedEntity">A <see cref="Dictionary{TKey,TValue}" /> of all others <see cref="Entity" /></param>
        public override void ResolveProperties(EntityDto entityDto, Dictionary<Guid, Entity> resolvedEntity)
        {
            base.ResolveProperties(entityDto, resolvedEntity);

            if (entityDto is not ModelDto modelDto)
            {
                throw new InvalidOperationException($"The DTO {entityDto.GetType()} does not match with the current Model POCO");
            }

            this.ModelName = modelDto.ModelName;
            this.IterationId = modelDto.IterationId;
        }

        /// <summary>
        ///     Instantiate a <see cref="EntityDto" /> from a <see cref="Entity" />
        /// </summary>
        /// <returns>A new <see cref="EntityDto" /></returns>
        public override EntityDto ToDto()
        {
            var dto = new ModelDto(this.Id)
            {
                ModelName = this.ModelName,
                IterationId = this.IterationId
            };

            dto.IncludeCommonProperties(this);
            return dto;
        }

        /// <summary>
        ///     Gets the iteration number of this <see cref="Model" />
        /// </summary>
        /// <returns>The iteration number</returns>
        public int GetIterationNumber()
        {
            var dashSplittedName = this.ModelName.Split(" - ");
            var iterationField = dashSplittedName[^2];
            var spaceSplitted = iterationField.Split(' ');
            return int.Parse(spaceSplitted[^1]);
        }
    }
}
