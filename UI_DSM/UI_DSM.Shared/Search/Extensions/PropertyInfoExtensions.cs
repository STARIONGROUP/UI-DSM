// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyInfoExtensions.cs" company="RHEA System S.A.">
// Copyright (c) 2022 RHEA System S.A.
//
// Author: RHEA System S.A.
//
// The current software is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 3 of the License, or (at your option) any later version.
//
// The current software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program; if not, write to the Free Software Foundation,
// Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------

namespace UI_DSM.Shared.Search.Extensions
{
    using System;
	using System.Collections;
    using System.Reflection;

    /// <summary>
    /// The <see cref="PropertyInfoExtensions"/> provides extensions on <see cref="PropertyInfo"/> for the <see cref="Converter"/>
    /// </summary>
    internal static class PropertyInfoExtensions
    {
        /// <summary>
        /// Asserts that the provided <see cref="PropertyInfo.PropertyType"/>  is of type struct
        /// </summary>
        /// <param name="property">This <see cref="PropertyInfo"/></param>
        /// <returns>A <see cref="bool"/></returns>
        internal static bool IsStruct(this PropertyInfo property) => property.PropertyType.IsValueType && !property.PropertyType.IsEnum;

        /// <summary>
        /// Asserts that the provided <see cref="PropertyInfo.PropertyType"/>  is of type <see cref="IEnumerable"/>
        /// </summary>
        /// <param name="property">This <see cref="PropertyInfo"/></param>
        /// <returns>A <see cref="bool"/></returns>
        internal static bool IsIEnumerable(this PropertyInfo property) => typeof(IEnumerable).IsAssignableFrom(property.PropertyType);

        /// <summary>
        /// Asserts that the provided <see cref="PropertyInfo.PropertyType"/>  is a reference type
        /// </summary>
        /// <param name="property">This <see cref="PropertyInfo"/></param>
        /// <returns>A <see cref="bool"/></returns>
        internal static bool IsReferenceType(this PropertyInfo property) => !property.IsIEnumerable() && property.PropertyType.IsClass;

        /// <summary>
        /// Asserts that the provided <see cref="PropertyInfo.PropertyType"/> is of type <see cref="DateTime"/>
        /// </summary>
        /// <param name="property">This <see cref="PropertyInfo"/></param>
        /// <param name="instance">The objec instance</param>
        /// <returns>A <see cref="bool"/></returns>
        internal static bool IsDateTime(this PropertyInfo property, object instance) => property.GetValue(instance) is DateTime;
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
