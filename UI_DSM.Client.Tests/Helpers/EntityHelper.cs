// --------------------------------------------------------------------------------------------------------
// <copyright file="EntityHelper.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw, Nabil Abbar
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Client.Tests.Helpers
{
    using System.Reflection;

    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Helper class to allow registration of <see cref="Entity" /> for test
    /// </summary>
    public static class EntityHelper
    {
        /// <summary>
        ///     Register all <see cref="Entity" /> class for deep level properties computation
        /// </summary>
        public static void RegisterEntities()
        {
            var entityType = typeof(Entity);
            var entityAssembly = Assembly.GetAssembly(entityType)!;

            var entities = entityAssembly.GetExportedTypes()
                .Where(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(entityType)).ToList();

            foreach (var entity in entities)
            {
                Entity.RegisterEntityProperties(entity);
            }

            Entity.RegisterAbstractEntity(entityAssembly.GetExportedTypes()
                .Where(x => x.IsClass && x.IsSubclassOf(entityType)).ToList());
        }
    }
}
