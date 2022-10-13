// --------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableOfGuidRouteConstraint.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Extensions
{
    using System.Globalization;

    using UI_DSM.Client.Extensions;

    /// <summary>
    ///     The purpose of the <see cref="EnumerableOfGuidRouteConstraint" /> is to enable url routing
    ///     for GUID
    /// </summary>
    public class EnumerableOfGuidRouteConstraint : IRouteConstraint
    {
        /// <summary>
        ///     Determines whether the URL parameter contains a valid value for this constraint.
        /// </summary>
        /// <param name="httpContext">An object that encapsulates information about the HTTP request.</param>
        /// <param name="route">The router that this constraint belongs to.</param>
        /// <param name="routeKey">The name of the parameter that is being checked.</param>
        /// <param name="values">A dictionary that contains the parameters for the URL.</param>
        /// <param name="routeDirection">
        ///     An object that indicates whether the constraint check is being performed
        ///     when an incoming request is being handled or when a URL is being generated.
        /// </param>
        /// <returns><c>true</c> if the URL parameter contains a valid value; otherwise, <c>false</c>.</returns>
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (routeKey == null)
            {
                throw new ArgumentNullException(nameof(routeKey));
            }

            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (values.TryGetValue(routeKey, out var value) && value != null)
            {
                var valueString = Convert.ToString(value, CultureInfo.InvariantCulture)!;

                if (!valueString.StartsWith("["))
                {
                    return false;
                }

                if (!valueString.EndsWith("]"))
                {
                    return false;
                }

                try
                {
                    _ = valueString.FromGuidArray().ToList();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return false;
        }
    }
}
