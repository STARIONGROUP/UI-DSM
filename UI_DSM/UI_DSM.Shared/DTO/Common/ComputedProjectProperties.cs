// --------------------------------------------------------------------------------------------------------
// <copyright file="TaskCommentCount.cs" company="RHEA System S.A.">
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
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Represents custom properties for <see cref="Project" /> that needs to be computed based on certain filters
    /// </summary>
    public class ComputedProjectProperties
    {
        /// <summary>
        ///     The number of <see cref="ReviewTask" /> contained into the <see cref="Project" />
        /// </summary>
        public int TaskCount { get; set; }

        /// <summary>
        ///     The number of <see cref="Comment" /> contained into the <see cref="Project" />
        /// </summary>
        public int CommentCount { get; set; }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        ///     <see langword="true" /> if the specified object  is equal to the current object; otherwise,
        ///     <see langword="false" />.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is not ComputedProjectProperties other)
            {
                return false;
            }

            return this.CommentCount == other.CommentCount && this.TaskCount == other.TaskCount;
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(this.TaskCount, this.CommentCount);
        }
    }
}
