// --------------------------------------------------------------------------------------------------------
// <copyright file="JsonDeserializerProvider.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Client.Services.JsonDeserializerProvider
{
    using System.Text.Json;

    using UI_DSM.Serializer.Json;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;

    using JsonSerializer = System.Text.Json.JsonSerializer;

    /// <summary>
    ///     This service provides capabilities to detects the correct JSON deserializer depending on the type of the object
    /// </summary>
    public class JsonDeserializerService : IJsonDeserializerService
    {
        /// <summary>
        ///     The <see cref="IJsonDeserializer" /> to deserialize <see cref="EntityDto" />
        /// </summary>
        private readonly IJsonDeserializer deserializer;

        /// <summary>
        ///     The <see cref="JsonSerializerOptions" />
        /// </summary>
        private readonly JsonSerializerOptions options;

        /// <summary>
        ///     Initializes a new instance of the <see cref="JsonDeserializerService" /> class.
        /// </summary>
        /// <param name="deserializer">The <see cref="IJsonDeserializer" /></param>
        public JsonDeserializerService(IJsonDeserializer deserializer)
        {
            this.deserializer = deserializer;

            this.options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        /// <summary>
        ///     Deserialize a <see cref="Stream" /> into a <see cref="T" /> object
        /// </summary>
        /// <typeparam name="T">An object</typeparam>
        /// <param name="stream">The <see cref="Stream" /> to deserialize</param>
        /// <returns>The <see cref="T" /> object</returns>
        public T Deserialize<T>(Stream stream) where T : class
        {
            if (typeof(T).IsAssignableTo(typeof(IEnumerable<EntityDto>)))
            {
                return (T)this.deserializer.Deserialize(stream);
            }

            if (typeof(T) == typeof(EntityRequestResponseDto))
            {
                return this.deserializer.DeserializeEntityRequestResponseDto(stream) as T;
            }

            return JsonSerializer.Deserialize<T>(stream, this.options);
        }
    }
}
