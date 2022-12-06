// --------------------------------------------------------------------------------------------------------
// <copyright file="EnumeratorExtension.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Shared.Extensions
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    ///     Extension class for Enumerator
    /// </summary>
    public static class EnumeratorExtension
    {
        /// <summary>
        ///     A <see cref="Dictionary{TKey,TValue}" /> to cache the name correspondance
        /// </summary>
        private static readonly Dictionary<Enum, string> displayName = new();

        /// <summary>
        ///     Retrieve the <see cref="DisplayAttribute" /> of an <see cref="Enum" /> value
        /// </summary>
        /// <param name="value">The <see cref="Enum" /> value</param>
        /// <returns>The <see cref="DisplayAttribute" /> value if found</returns>
        public static string GetEnumDisplayName(this Enum value)
        {
            if (displayName.ContainsKey(value))
            {
                return displayName[value];
            }

            var fi = value.GetType().GetField(value.ToString());

            if (fi == null)
            {
                return value.ToString();
            }

            var attributes = (DisplayAttribute[])fi.GetCustomAttributes(typeof(DisplayAttribute), false);
            var enumDisplayName = attributes is { Length: > 0 } ? attributes[0].Name : value.ToString();
            displayName[value] = enumDisplayName;
            return enumDisplayName;
        }
    }
}
