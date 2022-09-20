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
    using System.Text.Json;

    using UI_DSM.Serializer.Json;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;

    /// <summary>
    ///     Helper class for all call to Json Serialization inside tests
    /// </summary>
    public static class JsonSerializerHelper
    {
        /// <summary>
        ///     The <see cref="JsonWriterOptions" />
        /// </summary>
        private static readonly JsonWriterOptions Settings = new()
        {
            Indented = true
        };
            
        private static readonly IJsonSerializer Serializer = new Serializer.Json.JsonSerializer();

        /// <summary>
        ///     Serialize the <see cref="object" /> to a Json <see cref="string" /> with the correct settings applied
        /// </summary>
        /// <param name="model">The <see cref="object" /> to serialize</param>
        /// <returns>The Json output</returns>
        public static string SerializeObject(object model)
        {
            var stream = new MemoryStream();

            if (model is EntityDto dto)
            {
                Serializer.Serialize(dto, stream, Settings);
                return ReadStreamResult(stream);
            }

            if (model is IEnumerable<EntityDto> dtos)
            {
                 Serializer.Serialize(dtos, stream, Settings);
                return ReadStreamResult(stream);
            }

            if (model is EntityRequestResponseDto entityRequest)
            {
                 Serializer.SerializeEntityRequestDto(entityRequest, stream, Settings);
                return ReadStreamResult(stream);
            }

            return System.Text.Json.JsonSerializer.Serialize(model, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = false
            });
        }

        private static string ReadStreamResult(MemoryStream stream)
        {
            using var reader = new StreamReader(stream);
            stream.Seek(0, SeekOrigin.Begin);
            var serialized = reader.ReadToEnd();
            return serialized;
        }
    }
}
