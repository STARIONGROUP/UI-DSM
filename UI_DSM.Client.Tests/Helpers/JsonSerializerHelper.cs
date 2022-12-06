// --------------------------------------------------------------------------------------------------------
// <copyright file="JsonSerializerHelper.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Helpers
{
    using CDP4Common.MetaInfo;

    using CDP4JsonSerializer;

    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Serializer.Json;

    /// <summary>
    ///     Helper class to get the <see cref="IJsonService" />
    /// </summary>
    public class JsonSerializerHelper
    {
        /// <summary>
        ///     Initialize a new <see cref="IJsonService" />
        /// </summary>
        /// <returns>The <see cref="IJsonService" /></returns>
        public static IJsonService CreateService()
        {
            return new JsonService(new JsonDeserializer(), new JsonSerializer(), new Cdp4JsonSerializer(new MetaDataProvider(), new Version("2.4.1")));
        }
    }
}
