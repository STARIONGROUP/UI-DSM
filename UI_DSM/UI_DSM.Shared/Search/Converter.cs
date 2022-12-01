// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Converter.cs" company="RHEA System S.A.">
// Copyright (c) 2022 RHEA System S.A.
//
// Author: RHEA System S.A.
//
// The current software is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 3 of the License, or (at your option) any later version.
//
// The current software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program; if not, write to the Free Software Foundation,
// Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------

namespace UI_DSM.Shared.Search
{
    using System.Collections;
    using System.Reflection;
    
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using UI_DSM.Shared.Search.Extensions;
    using UI_DSM.Shared.Search.Attributes;

    using GP.SearchService.SDK.Definitions;

    /// <summary>
    /// The <see cref="Converter" /> class provides conversion from Poco type to searchDto
    /// </summary >
    public class Converter
    {
        /// <summary>
        /// The available search dto <see cref="Type" />
        ///</summary>
        private readonly Dictionary<string, Type> availableSearchDtoTypes = new();

        /// <summary>
        /// Initialize a new <see cref="Converter" />
        ///</summary>
        private Converter()
        {
            foreach (var type in Assembly.GetAssembly(typeof(Converter)).GetTypes())
            {
                if (typeof(ISearchDto).IsAssignableFrom(type))
                {
                    this.availableSearchDtoTypes.Add(type.Name, type);
                }
            }
        }

        /// <summary>
        /// Converts the provided <see cref="object" />
        /// </summary>
        /// <typeparam name="TSearchDto">The type of <see cref="ISearchDto" /></typeparam>
        /// <param name="thing">The <see cref="object" /></param>
        /// <returns>A <typeparamref name="TSearchDto" /></returns>
        public static TSearchDto Convert<TSearchDto>(object thing) where TSearchDto : ISearchDto
        {
            try
            {
                if (new Converter().TryConvert(thing, out var searchDto))
                {
                    return (TSearchDto)searchDto;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            return default;
        }

        /// <summary>
        /// Converts the provided <see cref="object" />
        /// </summary>
        /// <param name="things">The collection of <see cref="object" /></param>
        /// <returns>A collection <see cref="ISearchDto" /></returns>
        public static IEnumerable<ISearchDto>Convert(IEnumerable<object> things)
        {
            var converted = new List<ISearchDto>();

            try
            {
                var converter = new Converter();

                foreach (var thing in things)
                {
                    if (converter.TryConvert(thing, out var searchDto))
                    {
                        converted.Add(searchDto);
                    }
                }

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            return converted;
        }

        /// <summary>
        /// Attempt to convert the provided <see cref="object" /> into the out <param name="searchDto"></param>
        /// </summary>
        /// <param name="thing">The <see cref="object" /> to convert</param>
        /// <param name="searchDto">The out <see cref="ISearchDto" /></param>
        /// <param name="thingParent">The possible <paramref name="thing" /> parent</param>
        /// <returns>An assert</returns>
        private bool TryConvert(object thing, out ISearchDto searchDto, object thingParent = null)
        {
            searchDto = default;

            var thingProperties = thing.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            if (!TryGetThingSearchDtoType(thing, out var searchDtoType))
            {
                return false;
            }

            var searchDtoProperties = searchDtoType.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            searchDto = Activator.CreateInstance(searchDtoType) as ISearchDto;

            if (searchDto is null)
            {
                return false;
            }

            this.ProcessProperties(thing, searchDto, thingParent, searchDtoProperties, thingProperties);

            return true;
        }

        /// <summary>
        /// Process the provided <see cref="ISearchDto" /> properties
        /// </summary>
        /// <param name="thing">The <see cref="object" /> POCO thing</param>
        /// <param name="searchDto">The target <see cref="ISearchDto" /></param>
        /// <param name="thingParent">The possible <paramref name="thing" /> parent</param>
        /// <param name="searchDtoProperties">The collection of <see cref="PropertyInfo" /> from the <paramref name="searchDto" /> type</param>
        /// <param name="thingProperties">The collection of <see cref="PropertyInfo" /> from the <paramref name="thing" /> type</param>
        private void ProcessProperties(object thing, ISearchDto searchDto, object thingParent, 
                                       IEnumerable<PropertyInfo> searchDtoProperties, PropertyInfo[] thingProperties)
        {
            foreach (var property in searchDtoProperties)
            {
                if (thingProperties.FirstOrDefault(x => x.Name == property.Name
                || x.Name == this.OriginalPropertyName(property)) is not { } thingProperty)
                {
                    continue;
                }

                this.ProcessBasicPropertyType(thing, searchDto, property, thingProperty);

                this.ProcessReferenceType(thing, searchDto, thingParent, property, thingProperty);

                this.ProcessCollectionType(thing, searchDto, property, thingProperty);
            }
        }

        /// <summary>
        /// Process the provided property in case it is of type collection unless it is of type string
        /// </summary>
        /// <param name="thing">The <see cref="object" />POCO thing</param>
        /// <param name="searchDto">The target <see cref="ISearchDto" /></param>
        /// <param name="property">The <see cref="PropertyInfo" /> of the <paramref name="searchDto" /></param>
        /// <param name="thingProperty">The <see cref="PropertyInfo" /> of the <paramref name="thing" /></param>
        private void ProcessCollectionType(object thing, ISearchDto searchDto, PropertyInfo property, PropertyInfo thingProperty)
        {
            if (property.PropertyType != typeof(string) && property.IsIEnumerable() && thingProperty.IsIEnumerable())
            {
                if (Activator.CreateInstance(property.PropertyType) is not IList propertyCollectionValue)
                {
                    return;
                }

                foreach (var element in this.GetValue<IEnumerable>(thing, thingProperty) ?? Enumerable.Empty<object>())
                {
                    var elementType = element.GetType();

                    if ((elementType.IsPrimitive || elementType == typeof(string)) && property.PropertyType.GetGenericArguments().FirstOrDefault()?.Name == elementType.Name)
                    {
                        propertyCollectionValue.Add(element);
                    }
                    else if (this.TryConvert(element, out var convertedElement, thing))
                    {
                        propertyCollectionValue.Add(convertedElement);
                    }
                }

                property.SetValue(searchDto, propertyCollectionValue);
            }
        }

        /// <summary>
        /// Process the provided property in case it is a reference type
        /// </summary>
        /// <param name="thing">The <see cref="object" />POCO thing</param>
        /// <param name="searchDto">The target <see cref="ISearchDto" /></param>
        /// <param name="property">The <see cref="PropertyInfo" /> of the <paramref name="searchDto" /></param>
        /// <param name="thingProperty">The <see cref="PropertyInfo" /> of the <paramref name="thing" /></param>
        private void ProcessReferenceType(object thing, ISearchDto searchDto, object thingParent, PropertyInfo property, PropertyInfo thingProperty)
        {
            if (thingProperty.IsReferenceType() && this.GetValue(thing, thingProperty) is { } value)
            {
                if (value != thingParent && thingProperty.PropertyType.Name == property.PropertyType.Name && this.TryConvert(value, out var propertyValue, thing))
                {
                    property.SetValue(searchDto, propertyValue);
                }
                else if (property.PropertyType == typeof(Guid)
                && this.TryGetThingPropertyValueId(this.GetValue(thing, thingProperty), out var thingId))
                {
                    property.SetValue(searchDto, thingId);
                }
            }
        }

        /// <summary>
        /// Process the provided property in case it is a primitive | string | enum | struct
        /// </summary>
        /// <param name="thing">The <see cref="object" />POCO thing</param>
        /// <param name="searchDto">The target <see cref="ISearchDto" /></param>
        /// <param name="property">The <see cref="PropertyInfo" /> of the <paramref name="searchDto" /></param>
        /// <param name="thingProperty">The <see cref="PropertyInfo" /> of the <paramref name="thing" /></param>
        private void ProcessBasicPropertyType(object thing, ISearchDto searchDto, PropertyInfo property, PropertyInfo thingProperty)
        {
            if (thingProperty.PropertyType.IsEnum && property.PropertyType == typeof(string))
            {
                property.SetValue(searchDto, this.GetValue(thing, thingProperty).ToString());
            }

            if (thingProperty.PropertyType.IsPrimitive || thingProperty.PropertyType == typeof(string))
            {
                if (thingProperty.PropertyType == property.PropertyType)
                {
                    property.SetValue(searchDto, this.GetValue(thing, thingProperty));
                }
            }

            if (thingProperty.IsStruct())
            {
                if (thingProperty.IsDateTime(thing) && (property.PropertyType == typeof(long) 
                    || Nullable.GetUnderlyingType(property.PropertyType) == typeof(long)))
                {
                    property.SetValue(searchDto, ((DateTime)thingProperty.GetValue(thing)).ToDateTimeOffset().ToUnixTimeSeconds());
                }
                else
                {
                    property.SetValue(searchDto, thingProperty.GetValue(thing));
                }
            }
        }

        /// <summary>
        /// Gets the value as <see cref="object" /> from the specified <paramref name="thingProperty" /> from the specified <paramref name="thing" />
        /// </summary>
        /// <param name="thing">The <see cref="object" /></param>
        /// <param name="thingProperty">The <see cref="PropertyInfo" /></param>
        /// <returns>An <see cref="object" /></returns>
        private object GetValue(object thing, PropertyInfo thingProperty)
        {
            return this.GetValue<object>(thing, thingProperty);
        }

        /// <summary>
        /// Gets the value as <typeparamref name="TObject" /> from the specified <paramref name="thingProperty" /> from the specified <paramref name="thing" />
        ///</summary>
        /// <typeparam name="TObject">The type of object to retrive and return</typeparam>
        /// <param name="thing">The <typeparamref name="TObject" /></param>
        /// <param name="thingProperty">The <see cref="PropertyInfo" /></param>
        /// <returns>An <typeparamref name="TObject" /></returns>
        private TObject GetValue<TObject>(object thing, PropertyInfo thingProperty)
        {
            try
            {
                return (TObject)thingProperty.GetValue(thing);
            }
            catch (Exception exception)
            {
                if (exception.InnerException is not NotSupportedException)
                {
                    Console.WriteLine($"Getting value from [{thing}.{thingProperty.Name}] failed throwing {exception}");
                }

                return default(TObject);
            }
        }

        /// <summary>
        /// Attempts to get the Id of the provided <paramref name="thingPropertyValue" />
        /// </summary>
        /// <param name="thingPropertyValue">The property value</param>
        /// <param name="thingId">The Guid Id</param>
        /// <returns>An assert</returns>
        private bool TryGetThingPropertyValueId(object thingPropertyValue, out Guid? thingId)
        {
            thingId = thingPropertyValue.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(x =>
            {
                var idPropertyIndex = x.Name.IndexOf("id", StringComparison.OrdinalIgnoreCase);
                return x.PropertyType == typeof(Guid) && idPropertyIndex == 0 || idPropertyIndex == 1;
            })
            .Select(x => this.GetValue<Guid?>
                (thingPropertyValue, x))
                .FirstOrDefault();

            return thingId != null;
        }

        /// <summary>
        /// Attempts to get the search dto that match the type of the provided object
        /// </summary>
        /// <param name="thing">The object</param>
        /// <param name="searchDtoType">The matching search dto <see cref="Type" /></param>
        /// <returns>An assert</returns>
        private bool TryGetThingSearchDtoType(object thing, out Type searchDtoType)
        {
            if (!this.availableSearchDtoTypes.TryGetValue(thing.GetType().Name, out searchDtoType))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Reads the <see cref="OriginalPropertyName" /> value attribute from the provided <see cref="PropertyInfo" />
        /// </summary>
        /// <param name="property">The <see cref="PropertyInfo" /></param>
        /// <returns>A string</returns>
        private string OriginalPropertyName(PropertyInfo property) => (Attribute.GetCustomAttribute(property, typeof(OriginalPropertyNameAttribute)) as OriginalPropertyNameAttribute)?.Name;
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
