// --------------------------------------------------------------------------------------------------------
// <copyright file="BudgetTemplate.cs" company="RHEA System S.A.">
//  Copyright (c) 2023 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw, Nabil Abbar
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
    ///     Represents a budget template uploaded and linked to the project.
    /// </summary>
    [Table(nameof(BudgetTemplate))]
    public class BudgetTemplate: Artifact
    {
        /// <summary>
        ///     Initializes a new <see cref="BudgetTemplate" />
        /// </summary>
        public BudgetTemplate()
        {
        }

        /// <summary>
        ///     Inilializes a new <see cref="BudgetTemplate" />
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the <see cref="Entity" /></param>
        public BudgetTemplate(Guid id) : base(id)
        {
        }

        /// <summary>
        ///     The name of the <see cref="BudgetTemplate" />
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string BudgetName { get; set; }

        /// <summary>
        ///     Resolve the properties of the current <see cref="Entity" /> from its <see cref="EntityDto" /> counter-part
        /// </summary>
        /// <param name="entityDto">The source <see cref="EntityDto" /></param>
        /// <param name="resolvedEntity">A <see cref="Dictionary{TKey,TValue}" /> of all others <see cref="Entity" /></param>
        public override void ResolveProperties(EntityDto entityDto, Dictionary<Guid, Entity> resolvedEntity)
        {
            base.ResolveProperties(entityDto, resolvedEntity);

            if (entityDto is not BudgetTemplateDto budgetTemplateDto)
            {
                throw new InvalidOperationException($"The DTO {entityDto.GetType()} does not match with the current BudgetTemplate POCO");
            }

            this.BudgetName = budgetTemplateDto.BudgetName;
        }

        /// <summary>
        ///     Instantiate a <see cref="EntityDto" /> from a <see cref="Entity" />
        /// </summary>
        /// <returns>A new <see cref="EntityDto" /></returns>
        public override EntityDto ToDto()
        {
            var dto = new BudgetTemplateDto(this.Id)
            {
                BudgetName = this.BudgetName
            };

            dto.IncludeCommonProperties(this);
            return dto;
        }
    }
}
