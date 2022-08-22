// --------------------------------------------------------------------------------------------------------
// <copyright file="AccessRightHelper.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Shared.Helpers
{
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Wrappers;

    /// <summary>
    ///     Helper class for the <see cref="AccessRight" /> enumerator
    /// </summary>
    public static class AccessRightHelper
    {
        /// <summary>
        ///     A collection of <see cref="AccessRightWrapper" /> to cache data
        /// </summary>
        private static IEnumerable<AccessRightWrapper> accessRightWrappers;

        /// <summary>
        ///     Gets a collection of all <see cref="AccessRightWrapper" />
        /// </summary>
        /// <returns>A collection of all <see cref="AccessRightWrapper" /></returns>
        public static IEnumerable<AccessRightWrapper> GetAccessRights()
        {
            return accessRightWrappers ??= Enum.GetValues<AccessRight>().Select(x => new AccessRightWrapper(x));
        }
    }
}
