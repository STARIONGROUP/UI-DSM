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

namespace UI_DSM.Client.Services.JsonService
{
    using System.Text.Json;

    using CDP4Common.CommonData;
    using CDP4Common.MetaInfo;

    using CDP4JsonSerializer;

    using UI_DSM.Serializer.Json;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;

    using JsonSerializer = System.Text.Json.JsonSerializer;

    /// <summary>
    ///     This service provides capabilities to detects the correct JSON deserializer depending on the type of the object
    /// </summary>
    public class JsonService : IJsonService
    {
        /// <summary>
        ///     The <see cref="ICdp4JsonSerializer" />
        /// </summary>
        private readonly ICdp4JsonSerializer cdp4JsonSerializer;

        /// <summary>
        ///     The <see cref="IJsonDeserializer" /> to deserialize <see cref="EntityDto" />
        /// </summary>
        private readonly IJsonDeserializer deserializer;

        /// <summary>
        ///     The <see cref="JsonSerializerOptions" />
        /// </summary>
        private readonly JsonSerializerOptions options;

        /// <summary>
        ///     The <see cref="IJsonSerializer" />
        /// </summary>
        private readonly IJsonSerializer serializer;

        /// <summary>
        ///     The <see cref="JsonWriterOptions" />
        /// </summary>
        private readonly JsonWriterOptions writerOptions;

        /// <summary>
        ///     Initializes a new instance of the <see cref="JsonService" /> class.
        /// </summary>
        /// <param name="deserializer">The <see cref="IJsonDeserializer" /></param>
        /// <param name="serializer">The <see cref="IJsonSerializer" /></param>
        /// <param name="cdp4JsonSerializer">The <see cref="ICdp4JsonSerializer" /></param>
        public JsonService(IJsonDeserializer deserializer, IJsonSerializer serializer, ICdp4JsonSerializer cdp4JsonSerializer)
        {
            this.deserializer = deserializer;
            this.serializer = serializer;
            this.cdp4JsonSerializer = cdp4JsonSerializer;
            this.cdp4JsonSerializer.Initialize(new MetaDataProvider(), new Version("2.4.1"));

            this.options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            this.writerOptions = new JsonWriterOptions
            {
                Indented = true
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

            if (typeof(T).IsAssignableTo(typeof(IEnumerable<CDP4Common.DTO.Thing>)))
            {
                return (T)this.cdp4JsonSerializer.Deserialize(stream);
            }

            return JsonSerializer.Deserialize<T>(stream, this.options);
        }

        /// <summary>
        ///     Serialize a <see cref="T" /> into a <see cref="string" />
        /// </summary>
        /// <typeparam name="T">An object</typeparam>
        /// <param name="value">The object to serialize</param>
        /// <returns>The serialized value</returns>
        public string Serialize<T>(T value) where T : class
        {
            using var stream = new MemoryStream();

            switch (value)
            {
                case EntityDto dto:
                    this.serializer.Serialize(dto, stream, this.writerOptions);
                    break;
                case IEnumerable<EntityDto> dtos:
                    this.serializer.Serialize(dtos, stream, this.writerOptions);
                    break;
                case EntityRequestResponseDto requestDto:
                    this.serializer.SerializeEntityRequestDto(requestDto, stream, this.writerOptions);
                    break;
                case Thing thing:
                    this.cdp4JsonSerializer.SerializeToStream(thing, stream, false);
                    break;
                case IEnumerable<Thing> things:
                    var thingsDto = things.Select(x => x.ToDto());
                    this.cdp4JsonSerializer.SerializeToStream(thingsDto, stream);
                    break;
                default:
                    JsonSerializer.Serialize(stream, value);
                    break;
            }

            stream.Position = 0;
            using var streamReader = new StreamReader(stream);
            return streamReader.ReadToEnd();
        }
    }
}
