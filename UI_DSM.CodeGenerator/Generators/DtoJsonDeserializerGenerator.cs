// --------------------------------------------------------------------------------------------------------
// <copyright file="DtoJsonDeserializerGenerator.cs" company="RHEA System S.A.">
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
    using UI_DSM.Shared.DTO.Models;

    /// <summary>
    ///     A Handlebars based for generating DTO JSON Deserializers
    /// </summary>
    public class DtoJsonDeserializerGenerator : HandleBarsGenerator
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
            var assembly = Assembly.GetAssembly(typeof(EntityDto))!;
            await this.GenerateEnumDeserializers(assembly, outputDirectory); 
            await this.GenerateDtoDeserializers(assembly, outputDirectory);
            await this.GenerateDeSerializationProvider(assembly, outputDirectory);
        }

        /// <summary>
        ///     Generates the Deserializer classes for each enumerator in the provided <see cref="Assembly" />
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly" /> that contains the enumerators that need to be generated</param>
        /// <param name="outputDirectory">The target <see cref="DirectoryInfo" /></param>
        /// <returns> An awaitable <see cref="Task" /> </returns>
        private async Task GenerateEnumDeserializers(Assembly assembly, DirectoryInfo outputDirectory)
        {
            var template = this.Templates["enum-deserializer-template"];

            foreach (var enumType in assembly.GetExportedTypes().Where(x => x.IsEnum))
            {
                var generatedDeserializer = template(enumType);

                generatedDeserializer = this.CodeCleanup(generatedDeserializer);

                var fileName = $"{enumType.Name}Deserializer.cs";

                await Write(generatedDeserializer, outputDirectory, fileName);
            }
        }

        /// <summary>
        ///     Generates the DeserializationProvider class
        /// </summary>
        /// <param name="assembly"> The <see cref="Assembly" /> that contains the classes that need to be generated</param>
        /// <param name="outputDirectory">The target <see cref="DirectoryInfo" /></param>
        /// <returns> An awaitable <see cref="Task" /> </returns>
        private async Task GenerateDeSerializationProvider(Assembly assembly, DirectoryInfo outputDirectory)
        {
            var template = this.Templates["dto-deserialization-provider-template"];

            var dtoTypes = assembly.GetExportedTypes().Where(x => !x.IsAbstract
                                                                  && typeof(EntityDto).IsAssignableFrom(x)).OrderBy(x => x.Name).ToList();

            var generatedSerializationProvider = template(dtoTypes);

            generatedSerializationProvider = this.CodeCleanup(generatedSerializationProvider);

            const string fileName = "DeserializationProvider.cs";

            await Write(generatedSerializationProvider, outputDirectory, fileName);
        }

        /// <summary>
        ///     Generates the Deserializer classes for each <see cref="EntityDto" /> in the provided <see cref="Assembly" />
        /// </summary>
        /// <param name="assembly"> The <see cref="Assembly" /> that contains the classes that need to be generated</param>
        /// <param name="outputDirectory">The target <see cref="DirectoryInfo" /></param>
        /// <returns> An awaitable <see cref="Task" /> </returns>
        private async Task GenerateDtoDeserializers(Assembly assembly, DirectoryInfo outputDirectory)
        {
            var template = this.Templates["dto-deserializer-template"];

            foreach (var dtoType in assembly.GetExportedTypes().Where(x => !x.IsAbstract
                                                                           && typeof(EntityDto).IsAssignableFrom(x)))
            {
                var generatedDeserializer = template(dtoType);

                generatedDeserializer = this.CodeCleanup(generatedDeserializer);

                var fileName = $"{dtoType.Name}Deserializer.cs";

                await Write(generatedDeserializer, outputDirectory, fileName);
            }
        }

        /// <summary>
        ///     Register the code templates
        /// </summary>
        protected override void RegisterTemplates()
        {
            this.RegisterTemplate("enum-deserializer-template");
            this.RegisterTemplate("dto-deserializer-template");
            this.RegisterTemplate("dto-deserialization-provider-template");
        }

        /// <summary>
        ///     Register the custom helpers
        /// </summary>
        protected override void RegisterHelpers()
        {
            this.Handlebars.RegisterPropertyReflectionHelper();
            this.Handlebars.RegisteredDocumentationHelper();
            this.Handlebars.RegisterStringHelper();
        }
    }
}
