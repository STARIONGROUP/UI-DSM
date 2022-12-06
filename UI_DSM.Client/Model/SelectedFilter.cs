// --------------------------------------------------------------------------------------------------------
// <copyright file="SelectedFilter.cs" company="RHEA System S.A.">
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
    ///     Represents a Value of a <see cref="FilterModel" /> with the selected state
    /// </summary>
    public class FilterRow
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FilterRow" /> class.
        /// </summary>
        /// <param name="definedThing">The <see cref="DefinedThing" /></param>
        public FilterRow(DefinedThing definedThing)
        {
            this.DefinedThing = definedThing;
            this.IsSelected = true;
        }

        /// <summary>
        ///     The <see cref="DefinedThing" />
        /// </summary>
        public DefinedThing DefinedThing { get; private set; }

        /// <summary>
        ///     Asserts if the <see cref="DefinedThing" /> is selected or not
        /// </summary>
        public bool IsSelected { get; set; }
    }
}
