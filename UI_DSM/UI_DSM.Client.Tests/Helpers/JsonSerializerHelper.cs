// --------------------------------------------------------------------------------------------------------
// <copyright file="JsonSerializerHelper.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Helpers
{
    using Newtonsoft.Json;

    /// <summary>
    ///     Helper class for all call to Json Serialization inside tests
    /// </summary>
    public static class JsonSerializerHelper
    {
        /// <summary>
        ///     The <see cref="JsonSerializerSettings" />
        /// </summary>
        private static readonly JsonSerializerSettings settings = new()
        {
            TypeNameHandling = TypeNameHandling.All
        };

        /// <summary>
        ///     Serialize the <see cref="object" /> to a Json <see cref="string" /> with the correct settings applied
        /// </summary>
        /// <param name="instance">The <see cref="object" /> to serialize</param>
        /// <returns>The Json output</returns>
        public static string SerializeObject(object instance)
        {
            return JsonConvert.SerializeObject(instance, Formatting.Indented, settings);
        }
    }
}
