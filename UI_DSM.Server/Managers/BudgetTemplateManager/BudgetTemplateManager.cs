// --------------------------------------------------------------------------------------------------------
// <copyright file="BudgetTemplateManager.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Managers.BudgetTemplateManager
{
    using Microsoft.EntityFrameworkCore;

    using UI_DSM.Server.Context;
    using UI_DSM.Server.Types;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This manager handles operation to the Database for <see cref="BudgetTemplate" />s
    /// </summary>
    public class BudgetTemplateManager: ContainedEntityManager<BudgetTemplate, Project>, IBudgetTemplateManager
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ContainedEntityManager{TEntity,TEntityContainer}" /> class.
        /// </summary>
        /// <param name="context">The <see cref="DatabaseContext" /></param>
        public BudgetTemplateManager(DatabaseContext context) : base(context)
        {
        }

        /// <summary>
        ///     Resolve all properties for the <see cref="BudgetTemplate" />
        /// </summary>
        /// <param name="entity">The <see cref="BudgetTemplate" /></param>
        /// <param name="dto">The <see cref="EntityDto" /></param>
        /// <returns>A <see cref="Task" /></returns>
        public override Task ResolveProperties(BudgetTemplate entity, EntityDto dto)
        {
            if (dto is not BudgetTemplateDto budgetTemplateDto)
            {
                return Task.CompletedTask;
            }

            entity.ResolveProperties(budgetTemplateDto, new Dictionary<Guid, Entity>());
            return Task.CompletedTask;
        }

        /// <summary>
        ///     Creates a new <see cref="BudgetTemplate" /> and store it into the <see cref="DatabaseContext" />
        /// </summary>
        /// <param name="entity">The <see cref="BudgetTemplate" /> to create</param>
        /// <returns>A <see cref="Task" /> with the result of the creation</returns>
        public override async Task<EntityOperationResult<BudgetTemplate>> CreateEntity(BudgetTemplate entity)
        {
            if (!this.ValidateCurrentEntity(entity, out var entityOperationResult))
            {
                return entityOperationResult;
            }

            var project = entity.EntityContainer as Project;

            var existingBudget = project!.Artifacts.OfType<BudgetTemplate>()
                .FirstOrDefault(x => string.Equals(x.BudgetName, entity.BudgetName,StringComparison.InvariantCultureIgnoreCase)
                && x.Id != entity.Id);

            if (existingBudget != null)
            {
                return EntityOperationResult<BudgetTemplate>.Failed("A budget template with the same name already exist");
            }

            return await this.AddEntityToContext(entity);
        }

        /// <summary>
        ///     Deletes a <see cref="BudgetTemplate" />
        /// </summary>
        /// <param name="entity">The <see cref="BudgetTemplate" /> to delete</param>
        /// <returns>A <see cref="Task" /> with the result of the deletion</returns>
        public override Task<EntityOperationResult<BudgetTemplate>> DeleteEntity(BudgetTemplate entity)
        {
            return HandleNotSupportedOperation();
        }

        /// <summary>
        ///     Updates a <see cref="BudgetTemplate" />
        /// </summary>
        /// <param name="entity">The <see cref="BudgetTemplate" /> to update</param>
        /// <returns>A <see cref="Task" /> with the result of the update</returns>
        public override Task<EntityOperationResult<BudgetTemplate>> UpdateEntity(BudgetTemplate entity)
        {
            return HandleNotSupportedOperation();
        }

        /// <summary>
        ///     Gets the URL to access the <see cref="BudgetTemplate" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="BudgetTemplate" /></param>
        /// <returns>A URL</returns>
        public override async Task<SearchResultDto> GetSearchResult(Guid entityId)
        {
            var budgetTemplate = await this.EntityDbSet.Where(x => x.Id == entityId)
                .Include(x => x.EntityContainer).FirstOrDefaultAsync();

            if (budgetTemplate == null)
            {
                return null;
            }

            var route = $"Project/{budgetTemplate.EntityContainer.Id}/Budget/{budgetTemplate.Id}";

            return new SearchResultDto()
            {
                BaseUrl = route,
                ObjectKind = nameof(Model),
                DisplayText = budgetTemplate.BudgetName,
                Location = ((Project)budgetTemplate.EntityContainer).ProjectName
            };
        }

        /// <summary>
        ///     Gets all <see cref="Entity" /> that needs to be unindexed when the current <see cref="Entity" /> is delete
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the entity</param>
        /// <returns>A collection of <see cref="Entity" /></returns>
        public override async Task<IEnumerable<Entity>> GetExtraEntitiesToUnindex(Guid entityId)
        {
            await Task.CompletedTask;
            return Enumerable.Empty<Entity>();
        }
    }
}
