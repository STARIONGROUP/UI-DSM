// --------------------------------------------------------------------------------------------------------
// <copyright file="JsonDeserializer.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Serializer.Json
{
    using System.Runtime.Serialization;
    using System.Text.Json;

    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;

    /// <summary>
    ///     The purpose of the <see cref="JsonDeserializer" /> is to deserialize a JSON <see cref="Stream" /> to
    ///     an <see cref="EntityDto" /> and <see cref="IEnumerable{EntityDto}" />
    /// </summary>
    public class JsonDeserializer : IJsonDeserializer
    {
        /// <summary>
        ///     Deserializes the JSON stream to an <see cref="IEnumerable{EntityDto}" />
        /// </summary>
        /// <returns>A collection of <see cref="EntityDto" /></returns>
        public IEnumerable<EntityDto> Deserialize(Stream stream)
        {
            var result = new List<EntityDto>();

            using var document = JsonDocument.Parse(stream);
            var documentRoot = document.RootElement;

            switch (documentRoot.ValueKind)
            {
                case JsonValueKind.Object:
                    result.Add(this.DeserializeObject(documentRoot));
                    break;
                case JsonValueKind.Array:
                    result.AddRange(this.DeserializeArray(documentRoot));
                    break;
                default:
                    throw new SerializationException($"{documentRoot.ValueKind} not supported");
            }

            return result;
        }

        /// <summary>
        ///     Deserialize a <see cref="Stream" /> into a <see cref="EntityRequestResponseDto" />
        /// </summary>
        /// <param name="stream">The <see cref="Stream" /></param>
        /// <returns>The <see cref="EntityRequestResponseDto" /></returns>
        public EntityRequestResponseDto DeserializeEntityRequestResponseDto(Stream stream)
        {
            using var document = JsonDocument.Parse(stream);
            var documentRoot = document.RootElement;

            if (documentRoot.ValueKind != JsonValueKind.Object)
            {
                throw new InvalidOperationException($"The {nameof(documentRoot)} must be of type JsonValueKind.Object");
            }

            return EntityRequestResponseDtoDeserializer.Deserialize(documentRoot);
        }

        /// <summary>
        ///     Deserializes an <see cref="JsonElement" /> of type <see cref="JsonValueKind.Array" /> to an
        ///     <see cref="EntityDto" /> object
        /// </summary>
        /// <param name="jsonArray">The subject <see cref="JsonElement" /> </param>
        /// <returns>An instance of <see cref="EntityDto" /></returns>
        private IEnumerable<EntityDto> DeserializeArray(JsonElement jsonArray)
        {
            if (jsonArray.ValueKind != JsonValueKind.Array)
            {
                throw new ArgumentException($"The {nameof(jsonArray)} must be of type JsonValueKind.Object", nameof(jsonArray));
            }

            return jsonArray.EnumerateArray().Select(this.DeserializeObject).ToList();
        }

        /// <summary>
        ///     Deserializes an <see cref="JsonElement" /> of type <see cref="JsonValueKind.Object" /> to an
        ///     <see cref="EntityDto" /> object
        /// </summary>
        /// <param name="jsonObject">The subject <see cref="JsonElement" /> </param>
        /// <returns>An instance of <see cref="EntityDto" /></returns>
        private EntityDto DeserializeObject(JsonElement jsonObject)
        {
            if (jsonObject.ValueKind != JsonValueKind.Object)
            {
                throw new ArgumentException($"The {nameof(jsonObject)} must be of type JsonValueKind.Object", nameof(jsonObject));
            }

            if (jsonObject.TryGetProperty("@type", out var typeElement))
            {
                var typeName = typeElement.GetString();
                var func = DeserializationProvider.Provide(typeName);
                return func(jsonObject);
            }

            throw new SerializationException("The @type Json property is not available, the Deserializer cannot be used to deserialize this JsonElement");
        }
    }
}
