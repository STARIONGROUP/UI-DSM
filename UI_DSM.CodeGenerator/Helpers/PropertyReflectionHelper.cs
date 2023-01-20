// --------------------------------------------------------------------------------------------------------
// <copyright file="PropertyReflectionHelper.cs" company="RHEA System S.A.">
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

namespace UI_DSM.CodeGenerator.Helpers
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Reflection;

    using HandlebarsDotNet;

    using UI_DSM.CodeGenerator.Extensions;
    using UI_DSM.Shared.Annotations;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     The <see cref="HandlebarsBlockHelper" /> for Properties
    /// </summary>
    public static class PropertyReflectionHelper
    {
        /// <summary>
        ///     Registers the <see cref="PropertyReflectionHelper" />
        /// </summary>
        /// <param name="handlebars">
        ///     The <see cref="IHandlebars" /> context with which the helper needs to be registered
        /// </param>
        public static void RegisterPropertyReflectionHelper(this IHandlebars handlebars)
        {
            handlebars.RegisterHelper("PropertyReflection.GetProperties", (context, _) => GetPropertiesFromContext(context));

            handlebars.RegisterHelper("PropertyReflection.GetPropertyType", (output, _, arguments) =>
            {
                var propertyInfo = VerifyArgumentsCondition(arguments, "GetPropertyType");

                if (propertyInfo.PropertyType.GetInterface(nameof(IEnumerable)) != null && propertyInfo.PropertyType.GenericTypeArguments.Any())
                {
                    var genericType = propertyInfo.PropertyType.GenericTypeArguments[0];
                    output.WriteSafeString($"List<{genericType.TypeConversion()}>");
                }
                else
                {
                    output.WriteSafeString(propertyInfo.PropertyType.TypeConversion());
                }
            });

            handlebars.RegisterHelper("PropertyReflection.HasEnumeratorProperties", (context, _) =>
            {
                return GetPropertiesFromContext(context).Select(x => x.PropertyType)
                    .Any(x => x.IsEnum || (x.GetInterface(nameof(IEnumerable)) != null && x.GenericTypeArguments.Any(genereric => genereric.IsEnum)));
            });

            handlebars.RegisterHelper("PropertyReflection.GetAllProperties", (context, _) =>
            {
                if (context.Value is not Type type)
                {
                    throw new HandlebarsException("The context should be a Type");
                }

                var properties = type.GetProperties().Where(x => x.CanWrite);
                return properties;
            });

            handlebars.RegisterHelper("PropertyReflection.IsEnumerable", (_, arguments) =>
            {
                var propertyInfo = VerifyArgumentsCondition(arguments, "IsEnumerable");

                return propertyInfo.PropertyType.IsEnumerable();
            });

            handlebars.RegisterHelper("PropertyReflection.IsNumeric", (_, arguments) =>
            {
                var propertyInfo = VerifyArgumentsCondition(arguments, "IsNumeric");

                var type = propertyInfo.PropertyType.GetCorrectTypeToCheck();
                return type == typeof(int) || type == typeof(double);
            });

            handlebars.RegisterHelper("PropertyReflection.IsBool", (_, arguments) =>
            {
                var propertyInfo = VerifyArgumentsCondition(arguments, "IsBool");

                return propertyInfo.PropertyType.GetCorrectTypeToCheck() == typeof(bool);
            });

            handlebars.RegisterHelper("PropertyReflection.IsEnum", (_, arguments) =>
            {
                var propertyInfo = VerifyArgumentsCondition(arguments, "IsEnum");

                return propertyInfo.PropertyType.GetCorrectTypeToCheck().IsEnum;
            });

            handlebars.RegisterHelper("PropertyReflection.IsSerialized", (_, arguments) =>
            {
                var propertyInfo = VerifyArgumentsCondition(arguments, "IsSerialized");

                if (propertyInfo.PropertyType == typeof(User))
                {
                    return false;
                }

                if (propertyInfo.DeclaringType == typeof(UserEntity))
                {
                    return propertyInfo.Name != nameof(UserEntity.UserId);
                }

                if (propertyInfo.PropertyType.GetCorrectTypeToCheck().IsAssignableTo(typeof(Entity)))
                {
                    return propertyInfo.GetCustomAttributes<DeepLevelAttribute>().Any();
                }

                return true;
            });

            handlebars.RegisterHelper("PropertyReflection.IsReference", (_, arguments) =>
            {
                var propertyInfo = VerifyArgumentsCondition(arguments, "IsReference");

                return propertyInfo.PropertyType.GetCorrectTypeToCheck() == typeof(Guid);
            });

            handlebars.RegisterHelper("PropertyReflection.IsInteger", (_, arguments) =>
            {
                var propertyInfo = VerifyArgumentsCondition(arguments, "IsInteger");

                return propertyInfo.PropertyType.GetCorrectTypeToCheck() == typeof(int);
            });

            handlebars.RegisterHelper("PropertyReflection.IsDouble", (_, arguments) =>
            {
                var propertyInfo = VerifyArgumentsCondition(arguments, "IsDouble");

                return propertyInfo.PropertyType.GetCorrectTypeToCheck() == typeof(double);
            });

            handlebars.RegisterHelper("PropertyReflection.IsDateTime", (_, arguments) =>
            {
                var propertyInfo = VerifyArgumentsCondition(arguments, "IsDateTime");

                return propertyInfo.PropertyType.GetCorrectTypeToCheck() == typeof(DateTime);
            });

            handlebars.RegisterHelper("PropertyReflection.GetEnumValues", (context, _) =>
            {
                if (context.Value is not Type contextType)
                {
                    throw new HandlebarsException("{{PropertyReflection.GetEnumValues}} context has to be a Type");
                }

                if (!contextType.IsEnum)
                {
                    throw new HandlebarsException("{{PropertyReflection.GetEnumValues}} context has to be an Enum type");
                }

                return Enum.GetNames(contextType).ToList();
            });

            handlebars.RegisterHelper("PropertyReflection.EnumToUpperCase", (_, arguments) =>
            {
                if (arguments.Length != 1)
                {
                    throw new HandlebarsException("{{#PropertyReflection.EnumToUpperCase}} helper must have exactly one argument");
                }

                return arguments[0].ToString()?.ToUpper();
            });

            handlebars.RegisterHelper("PropertyReflection.GetTypeName", (writer, _ , arguments) =>
            {
                var propertyInfo = VerifyArgumentsCondition(arguments, "GetTypeName");

                writer.WriteSafeString(propertyInfo.PropertyType.GetCorrectTypeToCheck().Name);
            });

            handlebars.RegisterHelper("PropertyReflection.GetAllPropertiesExceptId", (context, _) =>
            {
                if (context.Value is not Type type)
                {
                    throw new HandlebarsException("The context should be a Type");
                }

                var properties = type.GetProperties().Where(x => x.Name != "Id" && x.CanWrite).ToList();
                return properties;
            }); 
        }

        /// <summary>
        ///     Verify if the <see cref="Arguments" /> has only one argument of type <see cref="PropertyInfo" />
        /// </summary>
        /// <param name="arguments">The <see cref="Arguments" /></param>
        /// <param name="helperName">The name of the helper</param>
        /// <returns>The retrieved <see cref="PropertyInfo" /></returns>
        private static PropertyInfo VerifyArgumentsCondition(Arguments arguments, string helperName)
        {
            if (arguments.Length != 1)
            {
                throw new HandlebarsException($"{{#PropertyReflection.{helperName}}} helper must have exactly one argument");
            }

            if (arguments.Single() is not PropertyInfo propertyInfo)
            {
                throw new HandlebarsException($"{{#PropertyReflection.{helperName}}} argument should be a PropertyInfo");
            }

            return propertyInfo;
        }

        /// <summary>
        ///     Retrieve all <see cref="PropertyInfo" /> from the context
        /// </summary>
        /// <param name="context">The <see cref="Context" /></param>
        /// <returns>An array of <see cref="PropertyInfo" /></returns>
        private static PropertyInfo[] GetPropertiesFromContext(Context context)
        {
            if (context.Value is not Type type)
            {
                throw new HandlebarsException("The context should be a Type");
            }

            var properties = type.GetProperties(BindingFlags.DeclaredOnly
                                                | BindingFlags.Public
                                                | BindingFlags.Instance);

            return properties;
        }
    }
}
