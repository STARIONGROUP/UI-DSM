// --------------------------------------------------------------------------------------------------------
// <copyright file="ArtifactManager.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Managers.ArtifactManager
{
    using UI_DSM.Server.Context;
    using UI_DSM.Server.Managers.ModelManager;
    using UI_DSM.Server.Types;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This manager handles operation to the Database for <see cref="Artifact" />s
    /// </summary>
    public class ArtifactManager : IArtifactManager
    {
        /// <summary>
        ///     The <see cref="IModelManager" />
        /// </summary>
        private readonly IModelManager modelManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ArtifactManager" /> class.
        /// </summary>
        /// <param name="modelManager">The <see cref="IModelManager" /></param>
        public ArtifactManager(IModelManager modelManager)
        {
            this.modelManager = modelManager;
        }

        /// <summary>
        ///     Gets a collection of all <see cref="Entity" />s and associated <see cref="Entity" />
        /// </summary>
        /// <param name="deepLevel">The level of deepnest that we need to retrieve associated <see cref="Entity" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /> as result</returns>
        public async Task<IEnumerable<Entity>> GetEntities(int deepLevel = 0)
        {
            var annotatableItems = new List<Entity>();
            annotatableItems.AddRange(await this.modelManager.GetEntities(deepLevel));
            return annotatableItems.DistinctBy(x => x.Id);
        }

        /// <summary>
        ///     Tries to get a <see cref="Entity" /> based on its <see cref="Guid" /> and its associated <see cref="Entity" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <param name="deepLevel">The level of deepnest that we need to retrieve associated <see cref="Entity" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /> if found</returns>
        public async Task<IEnumerable<Entity>> GetEntity(Guid entityId, int deepLevel = 0)
        {
            var artifact = await this.modelManager.GetEntity(entityId, deepLevel);
            return artifact;
        }

        /// <summary>
        ///     Tries to get a <see cref="Artifact" /> based on its <see cref="Guid" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="Artifact" /> if found</returns>
        public async Task<Artifact> FindEntity(Guid entityId)
        {
            return await this.modelManager.FindEntity(entityId);
        }

        /// <summary>
        ///     Tries to get all <see cref="Artifact" /> based on their <see cref="Guid" />
        /// </summary>
        /// <param name="entitiesId">A collection of <see cref="Guid" /></param>
        /// <returns>A collection of <see cref="Artifact" /></returns>
        public async Task<IEnumerable<Artifact>> FindEntities(IEnumerable<Guid> entitiesId)
        {
            var entities = new List<Artifact>();

            foreach (var id in entitiesId)
            {
                var entity = await this.FindEntity(id);

                if (entity != null)
                {
                    entities.Add(entity);
                }
            }

            return entities;
        }

        /// <summary>
        ///     Creates a new <see cref="Artifact" /> and store it into the <see cref="DatabaseContext" />
        /// </summary>
        /// <param name="entity">The <see cref="Artifact" /> to create</param>
        /// <returns>A <see cref="Task" /> with the result of the creation</returns>
        public async Task<EntityOperationResult<Artifact>> CreateEntity(Artifact entity)
        {
            switch (entity)
            {
                case Model model:
                    var modelResult = await this.modelManager.CreateEntity(model);

                    return modelResult.Succeeded
                        ? EntityOperationResult<Artifact>.Success(modelResult.Entity)
                        : EntityOperationResult<Artifact>.Failed(modelResult.Errors.ToArray());
            }

            return EntityOperationResult<Artifact>.Failed("Unknowned Annotation subclass");
        }

        /// <summary>
        ///     Updates a <see cref="Artifact" />
        /// </summary>
        /// <param name="entity">The <see cref="Artifact" /> to update</param>
        /// <returns>A <see cref="Task" /> with the result of the update</returns>
        public async Task<EntityOperationResult<Artifact>> UpdateEntity(Artifact entity)
        {
            switch (entity)
            {
                case Model model:
                    var modelResult = await this.modelManager.UpdateEntity(model);

                    return modelResult.Succeeded
                        ? EntityOperationResult<Artifact>.Success(modelResult.Entity)
                        : EntityOperationResult<Artifact>.Failed(modelResult.Errors.ToArray());
            }

            return EntityOperationResult<Artifact>.Failed("Unknowned Annotation subclass");
        }

        /// <summary>
        ///     Deletes a <see cref="Artifact" />
        /// </summary>
        /// <param name="entity">The <see cref="Artifact" /> to delete</param>
        /// <returns>A <see cref="Task" /> with the result of the deletion</returns>
        public async Task<EntityOperationResult<Artifact>> DeleteEntity(Artifact entity)
        {
            switch (entity)
            {
                case Model model:
                    var modelResult = await this.modelManager.DeleteEntity(model);

                    return modelResult.Succeeded
                        ? EntityOperationResult<Artifact>.Success(modelResult.Entity)
                        : EntityOperationResult<Artifact>.Failed(modelResult.Errors.ToArray());
            }

            return EntityOperationResult<Artifact>.Failed("Unknowned Annotation subclass");
        }

        /// <summary>
        ///     Resolve all properties for the <see cref="Artifact" />
        /// </summary>
        /// <param name="entity">The <see cref="Artifact" /></param>
        /// <param name="dto">The <see cref="EntityDto" /></param>
        /// <returns>A <see cref="Task" /></returns>
        public async Task ResolveProperties(Artifact entity, EntityDto dto)
        {
            switch (entity)
            {
                case Model model:
                    await this.modelManager.ResolveProperties(model, dto);
                    break;
            }
        }

        /// <summary>
        ///     Gets the <see cref="SearchResultDto"/> based on a <see cref="Guid"/>
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="Artifact" /></param>
        /// <returns>A URL</returns>
        public async Task<SearchResultDto> GetSearchResult(Guid entityId)
        {
            await Task.CompletedTask;
            return null;
        }

        /// <summary>
        ///     Gets all <see cref="Entity" /> that needs to be unindexed when the current <see cref="Entity" /> is delete
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the entity</param>
        /// <returns>A collection of <see cref="Entity" /></returns>
        public Task<IEnumerable<Entity>> GetExtraEntitiesToUnindex(Guid entityId)
        {
            return this.modelManager.GetExtraEntitiesToUnindex(entityId);
        }

        /// <summary>
        ///     Finds an <see cref="Artifact" /> and includes his <see cref="Entity" /> container
        /// </summary>
        /// <param name="entityId">The <see cref="Entity" /> id</param>
        /// <returns>A <see cref="Task" /> with the <see cref="Artifact" /></returns>
        public async Task<Artifact> FindEntityWithContainer(Guid entityId)
        {
            return await this.modelManager.FindEntityWithContainer(entityId);
        }

        /// <summary>
        ///     Gets all <see cref="Artifact" /> that are contained by the <see cref="Entity" /> with the Id
        ///     <see cref="containerId" /> and associated <see cref="Entity" />
        /// </summary>
        /// <param name="containerId">The <see cref="Guid" /> of the container</param>
        /// <param name="deepLevel">The deep level</param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /></returns>
        public async Task<IEnumerable<Entity>> GetContainedEntities(Guid containerId, int deepLevel = 0)
        {
            var artifacts = new List<Entity>();
            artifacts.AddRange(await this.modelManager.GetContainedEntities(containerId, deepLevel));
            return artifacts.DistinctBy(x => x.Id);
        }

        /// <summary>
        ///     Verifies if the container of the <see cref="Entity" /> has the given <see cref="Guid" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="Entity" /></param>
        /// <param name="containerId">The <see cref="Guid" /> of the container</param>
        /// <returns>A <see cref="Task" /> with the value of the check</returns>
        public async Task<bool> EntityIsContainedBy(Guid entityId, Guid containerId)
        {
            var artifact = await this.FindEntityWithContainer(entityId);
            return artifact != null && artifact.EntityContainer.Id == containerId;
        }
    }
}
