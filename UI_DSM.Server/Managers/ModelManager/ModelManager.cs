// --------------------------------------------------------------------------------------------------------
// <copyright file="ModelManager.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Managers.ModelManager
{
    using UI_DSM.Server.Context;
    using UI_DSM.Server.Types;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This manager handles operation to the Database for <see cref="Model" />s
    /// </summary>
    public class ModelManager : ContainedEntityManager<Model, Project>, IModelManager
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ModelManager" /> class.
        /// </summary>
        /// <param name="context">The <see cref="DatabaseContext" /></param>
        public ModelManager(DatabaseContext context) : base(context)
        {
        }

        /// <summary>
        ///     Updates a <see cref="Model" />
        /// </summary>
        /// <param name="entity">The <see cref="Model" /> to update</param>
        /// <returns>A <see cref="Task" /> with the result of the update</returns>
        public override Task<EntityOperationResult<Model>> UpdateEntity(Model entity)
        {
            return HandleNotSupportedOperation();
        }

        /// <summary>
        ///     Deletes a <see cref="Model" />
        /// </summary>
        /// <param name="entity">The <see cref="Model" /> to delete</param>
        /// <returns>A <see cref="Task" /> with the result of the deletion</returns>
        public override Task<EntityOperationResult<Model>> DeleteEntity(Model entity)
        {
            return HandleNotSupportedOperation();
        }

        /// <summary>
        ///     Resolve all properties for the <see cref="Model" />
        /// </summary>
        /// <param name="entity">The <see cref="Model" /></param>
        /// <param name="dto">The <see cref="EntityDto" /></param>
        /// <returns>A <see cref="Task" /></returns>
        public override Task ResolveProperties(Model entity, EntityDto dto)
        {
            if (dto is not ModelDto modelDto)
            {
                return Task.CompletedTask;
            }

            entity.ResolveProperties(modelDto, new Dictionary<Guid, Entity>());
            return Task.CompletedTask;
        }
    }
}
