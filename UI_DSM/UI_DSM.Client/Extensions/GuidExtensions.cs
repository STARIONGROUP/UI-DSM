// --------------------------------------------------------------------------------------------------------
// <copyright file="GuidExtensions.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Extensions
{
    /// <summary>
    ///     static extension methods for <see cref="Guid" />
    /// </summary>
    public static class GuidExtensions
    {
        /// <summary>
        ///     Creates a <see cref="IEnumerable{Guid}" /> based the Guid Array representation ->
        /// </summary>
        /// <param name="guids">
        ///     an <see cref="IEnumerable{String}" /> guid
        /// </param>
        /// <returns>
        ///     an <see cref="IEnumerable{Guid}" />
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     Thrown when the <paramref name="guids" /> does not start with '[' or ends with ']'
        /// </exception>
        public static IEnumerable<Guid> FromGuidArray(this string guids)
        {
            if (!guids.StartsWith("["))
            {
                InvalidGuidArray("must start with [", guids);
            }

            if (!guids.EndsWith("]"))
            {
                InvalidGuidArray("must end with ]", guids);
            }

            var listOfGuids = guids.TrimStart('[').TrimEnd(']').Split(';');

            foreach (var guid in listOfGuids)
            {
                if (Guid.TryParse(guid, out var parsedGuid))
                {
                    yield return parsedGuid;
                }
                else
                {
                    InvalidGuidArray($"unparsable Guid found: {guid}", guids);
                }
            }
        }

        /// <summary>
        ///     Converts a <see cref="IEnumerable{Guid}" /> to an array of <see cref="Guid" />, as a string
        /// </summary>
        /// <param name="guids">The <see cref="IEnumerable{Guid}" /></param>
        /// <returns>The converted string</returns>
        public static string ToGuidArray(this IEnumerable<Guid> guids)
        {
            return "[" + string.Join(";", guids.Select(x => x.ToString())) + "]";
        }

        /// <summary>
        ///     Throws an <see cref="Exception" /> because the given <see cref="guids" /> is invalid
        /// </summary>
        /// <param name="reason">The reason</param>
        /// <param name="guids">The array of guid, as string</param>
        private static void InvalidGuidArray(string reason, string guids)
        {
            throw new ArgumentException($"{guids} : Invalid Guid Array, {reason}", nameof(guids));
        }
    }
}
