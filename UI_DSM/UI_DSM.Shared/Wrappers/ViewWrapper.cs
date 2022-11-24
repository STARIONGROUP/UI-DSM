// --------------------------------------------------------------------------------------------------------
// <copyright file="ViewWrapper.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Shared.Wrappers
{
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Extensions;

    /// <summary>
    ///     Wrapper class for the <see cref="Enumerator.View" /> enumerator to be used inside DevExpress component
    /// </summary>
    public class ViewWrapper
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ViewWrapper" /> class.
        /// </summary>
        /// <param name="view">The <see cref="View" /></param>
        public ViewWrapper(View view)
        {
            this.View = view;
            this.ViewName = view.GetEnumDisplayName();
        }

        /// <summary>
        ///     The <see cref="View" />
        /// </summary>
        public View View { get; }

        /// <summary>
        ///     The associated name
        /// </summary>
        public string ViewName { get; }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        ///     <see langword="true" /> if the specified object  is equal to the current object; otherwise,
        ///     <see langword="false" />.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is ViewWrapper viewWrapper)
            {
                return viewWrapper.View == this.View;
            }

            return false;
        }

        /// <summary>
        ///     Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(this.View);
        }
    }
}
