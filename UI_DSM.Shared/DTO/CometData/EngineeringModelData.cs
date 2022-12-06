// --------------------------------------------------------------------------------------------------------
// <copyright file="EngineeringModelData.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Shared.DTO.CometData
{
    /// <summary>
    ///     Represents data relative to an EngineeringModel
    /// </summary>
    public class EngineeringModelData
    {
        /// <summary>
        ///     The <see cref="Guid" /> of the EngineeringModel
        /// </summary>
        public Guid EngineeringId { get; set; }

        /// <summary>
        ///     The EngineeringModel name
        /// </summary>
        public string EngineeringModelName { get; set; }

        /// <summary>
        ///     A collection of <see cref="IterationData" />
        /// </summary>
        public List<IterationData> Iterations { get; set; }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        ///     <see langword="true" /> if the specified object  is equal to the current object; otherwise,
        ///     <see langword="false" />.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is EngineeringModelData other)
            {
                return this.EngineeringId == other.EngineeringId && this.EngineeringModelName == other.EngineeringModelName;
            }

            return false;
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return this.EngineeringId.GetHashCode() + this.EngineeringId.GetHashCode() + this.Iterations.GetHashCode();
        }
    }
}
