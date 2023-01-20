// --------------------------------------------------------------------------------------------------------
// <copyright file="StringExtension.cs" company="RHEA System S.A.">
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
    using System.Linq;

    /// <summary>
    ///     Extension class for <see cref="string" />
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        ///     Capitalize the first letter of a string
        /// </summary>
        /// <param name="input">The subject input string </param>
        /// <returns>Returns a string</returns>
        public static string CapitalizeFirstLetter(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException("string can't be empty!");
            }

            return string.Concat(input.First().ToString().ToUpper(), input.AsSpan(1));
        }

        /// <summary>
        ///     Lower ccase the first letter of a string
        /// </summary>
        /// <param name="input">The subject input string</param>
        /// <returns>Returns a string</returns>
        public static string LowerCaseFirstLetter(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException("string can't be empty!");
            }

            return string.Concat(input.First().ToString().ToLower(), input.AsSpan(1));
        }
    }
}
