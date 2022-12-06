// --------------------------------------------------------------------------------------------------------
// <copyright file="HandleBarsGenerator.cs" company="RHEA System S.A.">
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
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    using HandlebarsDotNet;
    using HandlebarsDotNet.Helpers;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Formatting;

    /// <summary>
    ///     Base class for all generators that will use HandleBars
    /// </summary>
    public abstract class HandleBarsGenerator : Generator
    {
        /// <summary>
        ///     The <see cref="IHandlebars" /> instance used to generate code with
        /// </summary>
        protected IHandlebars Handlebars;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        protected HandleBarsGenerator()
        {
            var assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            this.TemplateFolderPath = Path.Combine(assemblyFolder!, "Templates");
            this.Templates = new Dictionary<string, HandlebarsTemplate<object, object>>();

            this.Handlebars = HandlebarsDotNet.Handlebars.CreateSharedEnvironment();
            HandlebarsHelpers.Register(this.Handlebars);

            this.Register();
        }

        /// <summary>
        ///     Gets the registered <see cref="HandlebarsTemplate{TContext,TData}" />
        /// </summary>
        public Dictionary<string, HandlebarsTemplate<object, object>> Templates { get; private set; }

        /// <summary>
        ///     Gets the path where the template are stored
        /// </summary>
        public string TemplateFolderPath { get; private set; }

        /// <summary>
        ///     Register the code templates
        /// </summary>
        protected abstract void RegisterTemplates();

        /// <summary>
        ///     Register the custom helpers
        /// </summary>
        protected abstract void RegisterHelpers();

        /// <summary>
        ///     Register handle-bars templates based on the template (file) name (without extension)
        /// </summary>
        /// <param name="name">
        ///     (file) name (without extension)
        /// </param>
        protected void RegisterTemplate(string name)
        {
            var templatePath = Path.Combine(this.TemplateFolderPath, $"{name}.hbs");

            var template = File.ReadAllText(templatePath);

            var compiledTemplate = this.Handlebars.Compile(template);

            this.Templates.Add(name, compiledTemplate);
        }

        /// <summary>
        ///     Perform code cleanup
        /// </summary>
        /// <param name="generatedCode">
        ///     The generated code that needs to be cleaned
        /// </param>
        /// <returns>
        ///     Cleaned up code
        /// </returns>
        protected virtual string CodeCleanup(string generatedCode)
        {
            generatedCode = generatedCode.Replace("&nbsp;", " ");
            var workspace = new AdhocWorkspace();
            var syntaxTree = CSharpSyntaxTree.ParseText(generatedCode);
            var root = syntaxTree.GetRoot();
            var formattedSyntaxNode = Formatter.Format(root, workspace);
            generatedCode = formattedSyntaxNode.SyntaxTree.GetText().ToString();

            return generatedCode;
        }

        /// <summary>
        ///     Register all helpers and templates
        /// </summary>
        private void Register()
        {
            this.RegisterTemplates();
            this.RegisterHelpers();
        }
    }
}
