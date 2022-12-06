// --------------------------------------------------------------------------------------------------------
// <copyright file="DocumentationHelper.cs" company="RHEA System S.A.">
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

namespace UI_DSM.CodeGenerator.Helpers
{
    using System;
    using System.Reflection;

    using HandlebarsDotNet;

    /// <summary>
    ///     The <see cref="HandlebarsBlockHelper" /> for Documentation
    /// </summary>
    public static class DocumentationHelper
    {
        /// <summary>
        ///     Registers the <see cref="DocumentationHelper" />
        /// </summary>
        /// <param name="handlebars">
        ///     The <see cref="IHandlebars" /> context with which the helper needs to be registered
        /// </param>
        public static void RegisteredDocumentationHelper(this IHandlebars handlebars)
        {
            handlebars.RegisterHelper("Documentation", (output, context, arguments) =>
            {
                var type = (Type)context.Value;

                if (arguments.Length == 1)
                {
                    output.WriteSafeString($"/// <summary>{Environment.NewLine}");
                    output.WriteSafeString($"///    The Data Transfer Object representing the <see cref=\"{type.Name}\" /> class.{Environment.NewLine}");
                    output.WriteSafeString($"/// </summary>{Environment.NewLine}");
                }
                else
                {
                    var query = arguments[1] as string;

                    if (query == "constructor")
                    {
                        output.WriteSafeString($"/// <summary>{Environment.NewLine}");
                        output.WriteSafeString($"///    Initializes a new <see cref=\"{type.Name}Dto\" /> class.{Environment.NewLine}");
                        output.WriteSafeString($"/// </summary>{Environment.NewLine}");

                        if (arguments.Length > 2)
                        {
                            output.WriteSafeString($"/// <param name=\"id\">The <see cref=\"Guid\" /> of the represented <see cref=\"Entity\" /></param>{Environment.NewLine}");
                        }
                    }

                    if (query == "instantiatePoco")
                    {
                        output.WriteSafeString($"{Environment.NewLine}/// <summary>{Environment.NewLine}");
                        output.WriteSafeString($"///    Instantiate a <see cref=\"Entity\" /> from a <see cref=\"EntityDto\" />{Environment.NewLine}");
                        output.WriteSafeString($"/// </summary>{Environment.NewLine}");
                    }
                }
            });

            handlebars.RegisterHelper("Documentation.Property", (output, context, _) =>
            {
                var property = (PropertyInfo)context.Value;
                output.WriteSafeString($"{Environment.NewLine}/// <summary>{Environment.NewLine}");
                output.WriteSafeString($"///    Gets or sets the {property.Name} of the {property.DeclaringType!.Name}{Environment.NewLine}");
                output.WriteSafeString($"/// </summary>{Environment.NewLine}");
            });
        }
    }
}
