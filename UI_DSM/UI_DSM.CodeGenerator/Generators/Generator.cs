// --------------------------------------------------------------------------------------------------------
// <copyright file="Generator.cs" company="RHEA System S.A.">
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

namespace UI_DSM.CodeGenerator.Generators
{
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Base class for any code or data generator
    /// </summary>
    public abstract class Generator
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
        public abstract Task Generate(DirectoryInfo outputDirectory);

        /// <summary>
        ///     Writes the generated code to disk
        /// </summary>
        /// <param name="generatedCode">
        ///     The generated code that needs to be written to disk
        /// </param>
        /// <param name="outputDirectory">
        ///     The target <see cref="DirectoryInfo" />
        /// </param>
        /// <param name="fileName">
        ///     The name of the file
        /// </param>
        /// <returns>
        ///     an awaitable <see cref="Task" />
        /// </returns>
        protected static async Task Write(string generatedCode, DirectoryInfo outputDirectory, string fileName)
        {
            var filePath = Path.Combine(outputDirectory.FullName, fileName);
            await File.WriteAllTextAsync(filePath, generatedCode);
        }
    }
}
