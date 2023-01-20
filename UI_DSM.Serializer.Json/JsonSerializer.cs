// --------------------------------------------------------------------------------------------------------
// <copyright file="JsonSerializer.cs" company="RHEA System S.A.">
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
    using System.Text.Json;

    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;

    /// <summary>
    ///     The purpose of the <see cref="Serializer" /> is to write an <see cref="EntityDto" /> and
    ///     <see cref="IEnumerable{EntityDto}" />
    ///     as JSON to a <see cref="Stream" />
    /// </summary>
    public class JsonSerializer : IJsonSerializer
    {
        /// <summary>
        ///     Serialize an <see cref="IEnumerable{EntityDto}" /> as JSON to a target <see cref="Stream" />
        /// </summary>
        /// <param name="dtos">The <see cref="IEnumerable{EntityDto}" /> that shall be serialized</param>
        /// <param name="stream">The target <see cref="Stream" /></param>
        /// <param name="jsonWriterOptions">The <see cref="JsonWriterOptions" /> to use</param>
        public void Serialize(IEnumerable<EntityDto> dtos, Stream stream, JsonWriterOptions jsonWriterOptions)
        {
            using var writer = new Utf8JsonWriter(stream, jsonWriterOptions);

            writer.WriteStartArray();

            foreach (var entityDto in dtos)
            {
                var serializationAction = SerializationProvider.Provide(entityDto.GetType());
                serializationAction(entityDto, writer);
                writer.Flush();
            }

            writer.WriteEndArray();
            writer.Flush();
        }

        /// <summary>
        ///     Serialize an <see cref="EntityDto" /> as JSON to a target <see cref="Stream" />
        /// </summary>
        /// <param name="dto">The <see cref="EntityDto" /> that shall be serialized</param>
        /// <param name="stream">The target <see cref="Stream" /></param>
        /// <param name="jsonWriterOptions">The <see cref="JsonWriterOptions" /> to use</param>
        public void Serialize(EntityDto dto, Stream stream, JsonWriterOptions jsonWriterOptions)
        {
            using var writer = new Utf8JsonWriter(stream, jsonWriterOptions);

            var serializationAction = SerializationProvider.Provide(dto.GetType());
            serializationAction(dto, writer);
            writer.Flush();
        }

        /// <summary>
        ///     Serialize an <see cref="EntityDto" /> as JSON to a target <see cref="Stream" />
        /// </summary>
        /// <param name="dto">The <see cref="EntityDto" /> that shall be serialized</param>
        /// <param name="stream">The target <see cref="Stream" /></param>
        /// <param name="jsonWriterOptions">The <see cref="JsonWriterOptions" /> to use</param>
        /// <returns>A <see cref="Task" /></returns>
        public async Task SerializeAsync(EntityDto dto, Stream stream, JsonWriterOptions jsonWriterOptions)
        {
            await using var writer = new Utf8JsonWriter(stream, jsonWriterOptions);

            var serializationAction = SerializationProvider.Provide(dto.GetType());
            serializationAction(dto, writer);
            await writer.FlushAsync();
        }

        /// <summary>
        ///     Serialize an <see cref="IEnumerable{EntityDto}" /> as JSON to a target <see cref="Stream" />
        /// </summary>
        /// <param name="dtos">The <see cref="IEnumerable{EntityDto}" /> that shall be serialized</param>
        /// <param name="stream">The target <see cref="Stream" /></param>
        /// <param name="jsonWriterOptions">The <see cref="JsonWriterOptions" /> to use</param>
        /// <returns>A <see cref="Task" /></returns>
        public async Task SerializeAsync(IEnumerable<EntityDto> dtos, Stream stream, JsonWriterOptions jsonWriterOptions)
        {
            await using var writer = new Utf8JsonWriter(stream, jsonWriterOptions);

            writer.WriteStartArray();

            foreach (var entityDto in dtos)
            {
                var serializationAction = SerializationProvider.Provide(entityDto.GetType());
                serializationAction(entityDto, writer);
                await writer.FlushAsync();
            }

            writer.WriteEndArray();
            await writer.FlushAsync();
        }

        /// <summary>
        ///     Serialize a <see cref="EntityRequestResponseDto" />
        /// </summary>
        /// <param name="requestResponse">The <see cref="EntityRequestResponseDto" /></param>
        /// <param name="stream">The <see cref="Stream" /></param>
        /// <param name="jsonWriterOptions">The <see cref="JsonWriterOptions" /></param>
        public void SerializeEntityRequestDto(EntityRequestResponseDto requestResponse, Stream stream, JsonWriterOptions jsonWriterOptions)
        {
            using var writer = new Utf8JsonWriter(stream, jsonWriterOptions);
            EntityRequestResponseDtoSerializer.Serialize(requestResponse, writer);
            writer.Flush();
        }

        /// <summary>
        ///     Serialize a <see cref="EntityRequestResponseDto" />
        /// </summary>
        /// <param name="requestResponse">The <see cref="EntityRequestResponseDto" /></param>
        /// <param name="stream">The <see cref="Stream" /></param>
        /// <param name="jsonWriterOptions">The <see cref="JsonWriterOptions" /></param>
        /// <returns>A <see cref="Task" /></returns>
        public async Task SerializeEntityRequestDtoAsync(EntityRequestResponseDto requestResponse, Stream stream, JsonWriterOptions jsonWriterOptions)
        {
            await using var writer = new Utf8JsonWriter(stream, jsonWriterOptions);
            EntityRequestResponseDtoSerializer.Serialize(requestResponse, writer);
            await writer.FlushAsync();
        }
    }
}
