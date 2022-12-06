// --------------------------------------------------------------------------------------------------------
// <copyright file="DtoGenerator.cs" company="RHEA System S.A.">
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

namespace UI_DSM.CodeGenerator.Generators
{
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using UI_DSM.CodeGenerator.Helpers;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     A Handlebars based for generating DTO
    /// </summary>
    public class DtoGenerator : HandleBarsGenerator
    {
        /// <summary>
        ///     Generates code specific to the concrete implementation
        /// </summary>
        /// <param name="outputDirectory">
        ///     The target <see cref="DirectoryInfo" />
        /// </param>
        /// <returns>
        ///     An awaitable <see cref="Task" />
        /// </returns>
        public override async Task Generate(DirectoryInfo outputDirectory)
        {
            var template = this.Templates["dto-class-template"];
            var assembly = Assembly.GetAssembly(typeof(Entity));
            var entityTypes = assembly!.GetTypes().Where(x => typeof(Entity) != x && typeof(Entity).IsAssignableFrom(x)).ToList();

            foreach (var entityType in entityTypes)
            {
                var generatedInterface = template(entityType);

                generatedInterface = this.CodeCleanup(generatedInterface);
                var fileName = $"{entityType.Name}Dto.cs";

                await Write(generatedInterface, outputDirectory, fileName);
            }
        }

        /// <summary>
        ///     Register the code templates
        /// </summary>
        protected override void RegisterTemplates()
        {
            this.RegisterTemplate("dto-class-template");
        }

        /// <summary>
        ///     Register the custom helpers
        /// </summary>
        protected override void RegisterHelpers()
        {
            this.Handlebars.RegisterPropertyReflectionHelper();
            this.Handlebars.RegisteredDocumentationHelper();
        }
    }
}
