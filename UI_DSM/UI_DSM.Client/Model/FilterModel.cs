// --------------------------------------------------------------------------------------------------------
// <copyright file="FilterModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Model
{
    using CDP4Common.CommonData;

    /// <summary>
    ///     Represents model to apply filter on a dataset of <see cref="Thing" />s
    /// </summary>
    public class FilterModel
    {
        /// <summary>
        ///     The <see cref="ClassKind" /> for values that are available for filtering
        /// </summary>
        public ClassKind ClassKind { get; set; }

        /// <summary>
        ///     The display name of the filter
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        ///     A collection of <see cref="DefinedThing" /> for filtering
        /// </summary>
        public List<DefinedThing> Values { get; set; }

        /// <summary>
        ///     Value indicating if values has to be displayed by name or shortname
        /// </summary>
        public bool UseShortName { get; set; }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        ///     <see langword="true" /> if the specified object  is equal to the current object; otherwise,
        ///     <see langword="false" />.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is FilterModel model)
            {
                return this.ClassKind == model.ClassKind;
            }

            return false;
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return this.ClassKind.GetHashCode();
        }
    }
}
