// --------------------------------------------------------------------------------------------------------
// <copyright file="RoleDtoDeserializer.cs" company="RHEA System S.A.">
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

// --------------------------------------------------------------------------------------------------------
// ------------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!------------
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Serializer.Json
{
    using System.Text.Json;

    using UI_DSM.Shared.DTO.Models;

    /// <summary>
    ///     The purpose of the <see cref="RoleDtoDeserializer" /> is to provide deserialization capabilities
    /// </summary>
    internal static class RoleDtoDeserializer
    {
        /// <summary>
        ///     Deserializes an instance of <see cref="RoleDto" /> using an <see cref="JsonElement"/>
        /// </summary>
        /// <param name="jsonElement">The <see cref="JsonElement"/> that contains the <see cref="RoleDto"/> json object</param>
        /// <returns>An instance of <see cref="RoleDto"/></returns>
        internal static RoleDto Deserialize(JsonElement jsonElement)
        {
            if (!jsonElement.TryGetProperty("@type", out var type))
            {
                throw new InvalidOperationException("The @type property is not available, the RoleDtoDeSerializer cannot be used to deserialize this JsonElement");
            }

            if (type.GetString() != "RoleDto")
            {
                throw new InvalidOperationException($"The RoleDtoDeserializer can only be used to deserialize objects of type RoleDto, a  {type.GetString()} was provided");
            }

            var dto = new RoleDto();

            if (jsonElement.TryGetProperty("id", out var idProperty))
            {
                var propertyValue = idProperty.GetString();

                if (propertyValue == null)
                {
                    throw new JsonException("The id property is not present, the RoleDto cannot be deserialized");
                }

                dto.Id = Guid.Parse(propertyValue);
            }

            if (jsonElement.TryGetProperty("roleName", out var roleNameProperty))
            {
                dto.RoleName = roleNameProperty.GetString();
            }

            if (jsonElement.TryGetProperty("accessRights", out var accessRightsProperty))
            {
                foreach (var item in accessRightsProperty.EnumerateArray())
                {
                    var propertyValue = item.GetString();

                    if (propertyValue != null)
                    {
                        dto.AccessRights.Add(AccessRightDeserializer.Deserialize(propertyValue));
                    }
                }
            }

            return dto;
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------