// --------------------------------------------------------------------------------------------------------
// <copyright file="TypeEntension.cs" company="RHEA System S.A.">
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

namespace UI_DSM.CodeGenerator.Extensions
{
    using System;
    using System.Collections;
    using System.Linq;

    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Extension class for <see cref="Type" />
    /// </summary>
    public static class TypeExtension
    {
        /// <summary>
        ///     Converts a <see cref="Type" /> to a string based on the name to generate code
        /// </summary>
        /// <param name="type">The <see cref="Type" /></param>
        /// <returns>A string</returns>
        public static string TypeConversion(this Type type)
        {
            if (typeof(Entity).IsAssignableFrom(type))
            {
                return nameof(Guid);
            }

            if (type == typeof(int))
            {
                return "int";
            }

            if (type == typeof(string))
            {
                return "string";
            }

            if (type == typeof(bool))
            {
                return "bool";
            }

            return type?.Name;
        }

        /// <summary>
        ///     Verify if the current <see cref="Type" /> is an <see cref="Enumerable" />
        /// </summary>
        /// <param name="type">The <see cref="Type" /> to check</param>
        /// <returns>The result</returns>
        public static bool IsEnumerable(this Type type)
        {
            return type.GetInterface(nameof(IEnumerable)) != null && type.GenericTypeArguments.Any();
        }

        /// <summary>
        ///     Gets the <see cref="Type" /> to check. If the <see cref="Type" /> is a <see cref="Enumerable" />, gets the generic type argument
        /// </summary>
        /// <param name="type">The <see cref="Type" /></param>
        /// <returns>The correct <see cref="Type" /></returns>
        public static Type GetCorrectTypeToCheck(this Type type)
        {
            return type.IsEnumerable() ? type.GenericTypeArguments.First() : type;
        }
    }
}
