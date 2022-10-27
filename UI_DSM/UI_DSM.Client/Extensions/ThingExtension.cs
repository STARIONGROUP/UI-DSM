// --------------------------------------------------------------------------------------------------------
// <copyright file="SimpleParameterizableThingExtension.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Extensions
{
    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    /// <summary>
    ///     Extension class for <see cref="CDP4Common.CommonData.Thing" />
    /// </summary>
    public static class ThingExtension
    {
        /// <summary>
        ///     Gets the value of a certain parameter
        /// </summary>
        /// <param name="thing">The <see cref="SimpleParameterizableThing" /></param>
        /// <param name="parameterName">The name of the parameter</param>
        /// <returns>The value if found</returns>
        public static string GetSimpleParameterValue(this SimpleParameterizableThing thing, string parameterName)
        {
            return thing.ParameterValue.FirstOrDefault(x => string.Equals(x.ParameterType.Name, parameterName, StringComparison.InvariantCultureIgnoreCase))
                ?.Value?.FirstOrDefault();
        }

        /// <summary>
        ///     Gets all name of <see cref="Category" />
        /// </summary>
        /// <param name="thing">The <see cref="ICategorizableThing" /></param>
        /// <param name="includeDeprecated">If deprecated <see cref="Category" /> should be included</param>
        /// <returns>A collection of name </returns>
        public static IEnumerable<string> GetCategories(this ICategorizableThing thing, bool includeDeprecated = false)
        {
            var categories = includeDeprecated ? thing.Category : thing.Category.Where(x => !x.IsDeprecated);
            return categories.Select(x => x.Name);
        }

        /// <summary>
        ///     Gets all <see cref="ParametricConstraint" /> contained into a <see cref="Requirement" />
        /// </summary>
        /// <param name="thing">The <see cref="Requirement" /></param>
        /// <returns>A collection of expressions</returns>
        public static IEnumerable<string> GetParametricConstraints(this Requirement thing)
        {
            return thing.ParametricConstraint.SelectMany(x => x.Expression.Select(expression => expression.StringValue)).ToList();
        }

        /// <summary>
        ///     Gets the shortname of the <see cref="IOwnedThing.Owner" />
        /// </summary>
        /// <param name="thing">The <see cref="IOwnedThing" /></param>
        /// <returns>The shortname</returns>
        public static string GetOwnerShortName(this IOwnedThing thing)
        {
            return thing.Owner.ShortName;
        }

        /// <summary>
        ///     Gets the first <see cref="Definition" /> content coded by the provided <see cref="languageCode" />, or the first
        ///     <see cref="Definition" /> if not present
        /// </summary>
        /// <param name="thing">The <see cref="DefinedThing" /></param>
        /// <param name="languageCode">The language code</param>
        /// <returns>The <see cref="Definition" /> content</returns>
        public static string GetFirstDefinition(this DefinedThing thing, string languageCode = "en")
        {
            var definition = thing.Definition.FirstOrDefault(x => x.LanguageCode == languageCode);
            return definition != null ? definition.Content : thing.Definition.FirstOrDefault()?.Content;
        }

        /// <summary>
        ///     Gets all <see cref="Thing" />s where the current <see cref="Thing" /> has a <see cref="BinaryRelationship" />
        ///     categorized by the given <see cref="categoryName" />
        /// </summary>
        /// <param name="thing">The <see cref="Thing" /></param>
        /// <param name="categoryName">The name of the <see cref="Category" /></param>
        /// <param name="classKind">The <see cref="ClassKind" /> of the related <see cref="Thing" /></param>
        /// <param name="isSource">If the <see cref="Thing" /> should be the source</param>
        /// <param name="getShortname">Value indicating if the retrieved value should be the name or the shortname of the <see cref="Thing"/></param>
        /// <returns>A collection of name of related <see cref="Thing" />s</returns>
        public static IEnumerable<string> GetRelatedThingsName(this Thing thing, string categoryName, ClassKind classKind, bool isSource = true, bool getShortname = true)
        {
            Func<BinaryRelationship, bool> filter = isSource
                ? x => x.Source.Iid == thing.Iid && x.Target.ClassKind == classKind
                : x => x.Target.Iid == thing.Iid && x.Source.ClassKind == classKind;

            var binaryRelationship = thing.QueryRelationships
                .OfType<BinaryRelationship>()
                .Where(x => x.Category.Any(cat => !cat.IsDeprecated
                                                  && string.Equals(cat.Name, categoryName, StringComparison.InvariantCultureIgnoreCase)))
                .Where(filter);

            return RelatedThingsName(binaryRelationship, !isSource, getShortname);
        }

        /// <summary>
        ///     Gets all <see cref="Thing" />s where the current <see cref="Thing" /> has a <see cref="BinaryRelationship" />
        ///     categorized by the given <see cref="categoryName" />
        /// </summary>
        /// <param name="thing">The <see cref="Thing" /></param>
        /// <param name="categoryName">The name of the <see cref="Category" /></param>
        /// <param name="classKind">The <see cref="ClassKind" /> of the related <see cref="Thing" /></param>
        /// <param name="filterCategory">The name of the <see cref="Category" /> that the related <see cref="Thing" /> should belongs</param>
        /// <param name="isSource">If the <see cref="Thing" /> should be the source</param>
        /// <param name="getShortname">Value indicating if the retrieved value should be the name or the shortname of the <see cref="Thing"/></param>
        /// <returns>A collection of name of related <see cref="Thing" />s</returns>
        public static IEnumerable<string> GetRelatedThingsName(this Thing thing, string categoryName, ClassKind classKind, string filterCategory, bool isSource = true, bool getShortname = true)
        {
            Func<BinaryRelationship, bool> filter = isSource
                ? x => x.Source.Iid == thing.Iid
                       && x.Target.ClassKind == classKind
                       && x.Target is ICategorizableThing categorizable
                       && categorizable.GetAllCategories().Any(cat => !cat.IsDeprecated && string.Equals(cat.Name, filterCategory, StringComparison.InvariantCultureIgnoreCase))
                : x => x.Target.Iid == thing.Iid
                       && x.Source.ClassKind == classKind
                       && x.Source is ICategorizableThing categorizable
                       && categorizable.GetAllCategories().Any(cat => !cat.IsDeprecated && string.Equals(cat.Name, filterCategory, StringComparison.InvariantCultureIgnoreCase));

            var binaryRelationship = thing.QueryRelationships
                .OfType<BinaryRelationship>()
                .Where(x => x.Category.Any(cat => !cat.IsDeprecated
                                                  && string.Equals(cat.Name, categoryName, StringComparison.InvariantCultureIgnoreCase))
                ).Where(filter);

            return RelatedThingsName(binaryRelationship, !isSource, getShortname);
        }

        /// <summary>
        ///     Retrieves names of <see cref="Thing" />s that defines the <see cref="BinaryRelationship" />
        /// </summary>
        /// <param name="relationships">The <see cref="BinaryRelationship" /></param>
        /// <param name="sourceThing">
        ///     If the <see cref="Thing" /> to retrieve is the source of the target of the
        ///     <see cref="BinaryRelationship" />
        /// </param>
        /// <param name="getShortname">Value indicating if the retrieved value should be the name or the shortname of the <see cref="Thing"/></param>
        /// <returns>The name of the <see cref="Thing" /></returns>
        private static IEnumerable<string> RelatedThingsName(IEnumerable<BinaryRelationship> relationships, bool sourceThing, bool getShortname)
        {
            return relationships.Select(binaryRelationship => getShortname
                ? RelatedThingShortName(binaryRelationship, sourceThing) 
                : RelatedThingName(binaryRelationship, sourceThing)).ToList();
        }

        /// <summary>
        ///     Retrieves the name of one of the <see cref="Thing" /> that defines the <see cref="BinaryRelationship" />
        /// </summary>
        /// <param name="binaryRelationship">The <see cref="BinaryRelationship" /></param>
        /// <param name="sourceThing">
        ///     If the <see cref="Thing" /> to retrieve is the source of the target of the
        ///     <see cref="BinaryRelationship" />
        /// </param>
        /// <returns>The name of the <see cref="Thing" /></returns>
        private static string RelatedThingName(BinaryRelationship binaryRelationship, bool sourceThing)
        {
            if (binaryRelationship == null)
            {
                return string.Empty;
            }

            var relatedThing = sourceThing ? binaryRelationship.Source : binaryRelationship.Target;

            if (relatedThing is INamedThing namedThing)
            {
                return namedThing.Name;
            }

            return relatedThing.UserFriendlyName;
        }

        /// <summary>
        ///     Retrieves the shortname of one of the <see cref="Thing" /> that defines the <see cref="BinaryRelationship" />
        /// </summary>
        /// <param name="binaryRelationship">The <see cref="BinaryRelationship" /></param>
        /// <param name="sourceThing">
        ///     If the <see cref="Thing" /> to retrieve is the source of the target of the
        ///     <see cref="BinaryRelationship" />
        /// </param>
        /// <returns>The shortname of the <see cref="Thing" /></returns>
        private static string RelatedThingShortName(BinaryRelationship binaryRelationship, bool sourceThing)
        {
            if (binaryRelationship == null)
            {
                return string.Empty;
            }

            var relatedThing = sourceThing ? binaryRelationship.Source : binaryRelationship.Target;

            if (relatedThing is IShortNamedThing shortNamedThing)
            {
                return shortNamedThing.ShortName;
            }

            return relatedThing.UserFriendlyShortName;
        }
    }
}
