// --------------------------------------------------------------------------------------------------------
// <copyright file="RoslynCodeCompiler.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Reporting.CodeCompiler
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Loader;

    using CDP4Reporting.ReportScript;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    /// <summary>
    /// Handles code compilation using Roslyn compiler
    /// </summary>
    public class RoslynCodeCompiler : CodeCompilerBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="RoslynCodeCompiler"/> class
        /// </summary>
        /// <param name="onOutput">An <see cref="Action{T}"/> of type <see cref="string"/> that is invoked when user output is needed during compilation or data retrieval</param>
        public RoslynCodeCompiler(Action<string> onOutput) : base(onOutput)
        {
        }

        /// <summary>
        /// Compiles source code and returns the <see cref="CompilerResults"/>
        /// </summary>
        /// <param name="source">The source code</param>
        /// <param name="assemblies"><see cref="IEnumerable{T}"/> of type <see cref="string"/> that holds locations of the referenced assemblies</param>
        /// <returns>The <see cref="CompilerResults"/></returns>
        public override CompilerResults Compile(string source, IEnumerable<string> assemblies)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(source);

            var assemblyName = Path.GetRandomFileName();

            var executingAssemblyLocation = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
            var dllReferences = Directory.GetFiles(executingAssemblyLocation, "*.dll");

            assemblies = assemblies.Union(dllReferences).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

            var references = assemblies.Select(r => MetadataReference.CreateFromFile(r)).ToArray();

            var compilation = CSharpCompilation.Create(
                assemblyName,
                new[] { syntaxTree },
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);

                if (!result.Success)
                {
                    var failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error).ToList();

                    foreach (var diagnostic in failures)
                    {
                        this.onOutput?.Invoke($"{diagnostic.Id}: {diagnostic.GetMessage()}");
                    }

                    return new CompilerResults(null, failures.Select(x => x.GetMessage()).ToList());
                }

                ms.Seek(0, SeekOrigin.Begin);

                return new CompilerResults(AssemblyLoadContext.Default.LoadFromStream(ms), null);
            }
        }
    }
}
