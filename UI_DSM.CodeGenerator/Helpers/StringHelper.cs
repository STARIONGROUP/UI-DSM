// --------------------------------------------------------------------------------------------------------
// <copyright file="StringHelper.cs" company="RHEA System S.A.">
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
    using HandlebarsDotNet;
    
    using UI_DSM.CodeGenerator.Extensions;

    /// <summary>
    ///     A block helper that helps with all kind of <see cref="string" /> manipulation
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        ///     Registers the <see cref="StringHelper" />
        /// </summary>
        /// <param name="handlebars">The <see cref="IHandlebars" /> context with which the helper needs to be registered</param>
        public static void RegisterStringHelper(this IHandlebars handlebars)
        {
            handlebars.RegisterHelper("String.CapitalizeFirstLetter", (writer, _, parameters) =>
            {
                if (parameters.Length != 1)
                {
                    throw new HandlebarsException("{{#String.CapitalizeFirstLetter}} helper must have exactly one argument");
                }

                var value = parameters[0] as string;

                writer.WriteSafeString(value.CapitalizeFirstLetter());
            });

            handlebars.RegisterHelper("String.LowerCaseFirstLetter", (writer, _, parameters) =>
            {
                if (parameters.Length != 1)
                {
                    throw new HandlebarsException("{{#String.LowerCaseFirstLetter}} helper must have exactly one argument");
                }

                var value = parameters[0] as string;

                writer.WriteSafeString(value.LowerCaseFirstLetter());
            });
        }
    }
}
