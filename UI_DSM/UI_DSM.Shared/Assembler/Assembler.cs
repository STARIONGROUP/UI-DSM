// --------------------------------------------------------------------------------------------------------
// <copyright file="Assembler.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Shared.Assembler
{
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     The <see cref="Assembler" /> provide utilities to convert <see cref="EntityDto" /> to <see cref="Entity" />
    /// </summary>
    public static class Assembler
    {
        /// <summary>
        ///     Creates a collection of <see cref="TEntity" /> based on the collection of <see cref="EntityDto" />
        /// </summary>
        /// <typeparam name="TEntity">A <see cref="Entity" /></typeparam>
        /// <param name="entities">A collection of <see cref="EntityDto" /></param>
        /// <returns>A collection of <see cref="TEntity" /></returns>
        public static IEnumerable<TEntity> CreateEntities<TEntity>(IEnumerable<EntityDto> entities) where TEntity : Entity
        {
            var entitiesDictionary = new Dictionary<Guid, Entity>();
            var entitiesDtoDictionary = new Dictionary<Guid, EntityDto>();

            foreach (var entityDto in entities)
            {
                entitiesDictionary[entityDto.Id] = entityDto.InstantiatePoco();
                entitiesDtoDictionary[entityDto.Id] = entityDto;
            }

            foreach (var id in entitiesDictionary.Keys)
            {
                entitiesDictionary[id].ResolveProperties(entitiesDtoDictionary[id], entitiesDictionary);
            }

            return entitiesDictionary.Values.Where(x => x.GetType() == typeof(TEntity)).Cast<TEntity>().ToList();
        }
    }
}
