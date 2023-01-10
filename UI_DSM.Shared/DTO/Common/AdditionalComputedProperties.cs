// --------------------------------------------------------------------------------------------------------
// <copyright file="AdditionalComputedProperties.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Shared.DTO.Common
{
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Represents custom properties that needs to be computed based on certain filters
    /// </summary>
    public class AdditionalComputedProperties
    {
        /// <summary>
        ///     The number of contained <see cref="ReviewTask" />
        /// </summary>
        public int TaskCount { get; set; }

        /// <summary>
        ///     The number of contained <see cref="Comment" /> with an <see cref="StatusKind.Open" /> status
        /// </summary>
        public int OpenCommentCount { get; set; }

        /// <summary>
        ///     The number of contained <see cref="Comment" />
        /// </summary>
        public int TotalCommentCount { get; set; }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        ///     <see langword="true" /> if the specified object  is equal to the current object; otherwise,
        ///     <see langword="false" />.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is not AdditionalComputedProperties other)
            {
                return false;
            }

            return this.OpenCommentCount == other.OpenCommentCount && this.TaskCount == other.TaskCount
                                                                   && this.TotalCommentCount == other.TotalCommentCount;
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(this.TaskCount, this.OpenCommentCount);
        }
    }
}
