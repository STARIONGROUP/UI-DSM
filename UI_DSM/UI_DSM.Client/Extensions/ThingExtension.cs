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
    ///     Extension class for <see cref="Thing" />
    /// </summary>
    public static class ThingExtension
    {
        /// <summary>
        ///     Collection of <see cref="Category" /> name for <see cref="ElementDefinition" /> that should have a technology parameter
        /// </summary>
        private static readonly List<string> MandatoryCategoriesForTechnology = new()
        {
            "consumables",
            "equipment",
            "instruments",
            "subequipment",
            "thermal equipment"
        };

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
        /// <returns>A collection of name of related <see cref="Thing" />s</returns>
        public static IEnumerable<string> GetRelatedThings(this Thing thing, string categoryName, ClassKind classKind)
        {
            var binaryRelationship = thing.QueryRelationships(categoryName).Where(x => (x.Source.Iid == thing.Iid && x.Target.ClassKind == classKind)
                                                                                       || (x.Target.Iid == thing.Iid && x.Source.ClassKind == classKind));

            return binaryRelationship.Select(x => RelatedThingShortName(x, x.Source.Iid == thing.Iid));
        }

        /// <summary>
        ///     Gets all <see cref="Thing" />s where the current <see cref="Thing" /> has a <see cref="BinaryRelationship" />
        ///     categorized by the given <see cref="categoryName" />
        /// </summary>
        /// <param name="thing">The <see cref="Thing" /></param>
        /// <param name="categoryName">The name of the <see cref="Category" /></param>
        /// <param name="classKind">The <see cref="ClassKind" /> of the related <see cref="Thing" /></param>
        /// <param name="isSource">If the <see cref="Thing" /> should be the source</param>
        /// <param name="getShortname">
        ///     Value indicating if the retrieved value should be the name or the shortname of the
        ///     <see cref="Thing" />
        /// </param>
        /// <returns>A collection of name of related <see cref="Thing" />s</returns>
        public static IEnumerable<string> GetRelatedThingsName(this Thing thing, string categoryName, ClassKind classKind, bool isSource = true, bool getShortname = true)
        {
            Func<BinaryRelationship, bool> filter = isSource
                ? x => x.Source.Iid == thing.Iid && x.Target.ClassKind == classKind
                : x => x.Target.Iid == thing.Iid && x.Source.ClassKind == classKind;

            var binaryRelationship = thing.QueryRelationships(categoryName).Where(filter);

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
        /// <param name="getShortname">
        ///     Value indicating if the retrieved value should be the name or the shortname of the
        ///     <see cref="Thing" />
        /// </param>
        /// <returns>A collection of name of related <see cref="Thing" />s</returns>
        public static IEnumerable<string> GetRelatedThingsName(this Thing thing, string categoryName, ClassKind classKind, string filterCategory, bool isSource = true, bool getShortname = true)
        {
            Func<BinaryRelationship, bool> filter = isSource
                ? x => x.Source.Iid == thing.Iid
                       && x.Target.ClassKind == classKind
                       && x.Target is ICategorizableThing categorizable
                       && categorizable.IsCategorizedBy(filterCategory)
                : x => x.Target.Iid == thing.Iid
                       && x.Source.ClassKind == classKind
                       && x.Source is ICategorizableThing categorizable
                       && categorizable.IsCategorizedBy(filterCategory);

            var binaryRelationship = thing.QueryRelationships(categoryName).Where(filter);

            return RelatedThingsName(binaryRelationship, !isSource, getShortname);
        }

        /// <summary>
        ///     Verifies if a <see cref="Thing" /> has a <see cref="BinaryRelationship" /> with another <see cref="Thing" />
        ///     defined by the
        ///     <paramref name="otherThingId" />
        /// </summary>
        /// <param name="thing">The <see cref="Thing" /></param>
        /// <param name="otherThingId">The <see cref="Guid" /> of the other <see cref="Thing" /></param>
        /// <param name="relationshipCategory">
        ///     The name of the <see cref="Category" /> of the <see cref="BinaryRelationship" />
        /// </param>
        /// <returns>The value asserting is the <see cref="Thing" /> is linked to the other <see cref="Thing" /></returns>
        public static bool IsLinkedTo(this Thing thing, Guid otherThingId, string relationshipCategory)
        {
            if (thing.Iid == otherThingId)
            {
                return false;
            }

            var binaryRelationships = thing.QueryRelationships(relationshipCategory);
            return binaryRelationships.Any(x => x.Source.Iid == thing.Iid && x.Target.Iid == otherThingId);
        }

        /// <summary>
        ///     Gets the name of the <see cref="RequirementsSpecification" /> of a <see cref="Requirement" />
        /// </summary>
        /// <param name="requirement">The <see cref="Requirement" /></param>
        /// <returns>The name of the <see cref="RequirementsSpecification" /></returns>
        public static string GetSpecificationName(this Requirement requirement)
        {
            var specification = requirement.Container as RequirementsSpecification;
            return specification?.ShortName;
        }

        /// <summary>
        ///     Verifies that a <see cref="ElementDefinition" /> is a product
        /// </summary>
        /// <param name="elementDefinition">The <see cref="ElementDefinition" /></param>
        /// <returns>The asserts</returns>
        public static bool IsProduct(this ElementDefinition elementDefinition)
        {
            return elementDefinition.IsCategorizedBy("products");
        }

        /// <summary>
        ///     Verifies that a <see cref="ElementDefinition" /> is a function
        /// </summary>
        /// <param name="elementDefinition">The <see cref="ElementDefinition" /></param>
        /// <returns>The asserts</returns>
        public static bool IsFunction(this ElementDefinition elementDefinition)
        {
            return elementDefinition.IsCategorizedBy("functions");
        }

        /// <summary>
        ///     Asserts that a Product has to have a Technology parameter
        /// </summary>
        /// <param name="elementDefinition">The <see cref="ElementDefinition" /></param>
        /// <returns>The assert</returns>
        public static bool ShouldHaveTechnologyParameter(this ElementDefinition elementDefinition)
        {
            return elementDefinition.IsCategorizedBy(MandatoryCategoriesForTechnology);
        }

        /// <summary>
        ///     Tries to get the value of a defined parameter
        /// </summary>
        /// <param name="elementDefinition">The <see cref="ElementDefinition" /></param>
        /// <param name="parameterName">The name of the <see cref="ParameterType" /></param>
        /// <param name="option">The <see cref="Option" /></param>
        /// <param name="state">The <see cref="ActualFiniteState" /></param>
        /// <param name="value">The value of the parameter</param>
        /// <returns>True if has a the parameter</returns>
        public static bool TryGetParameterValue(this ElementDefinition elementDefinition, string parameterName, Option option, ActualFiniteState state, out string value)
        {
            value = string.Empty;

            var parameter = elementDefinition.Parameter
                .Where(x => string.Equals(x.ParameterType.Name, parameterName, StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            if (!parameter.Any())
            {
                return false;
            }

            value = parameter.Single().QueryParameterBaseValueSet(option, state).ActualValue.First();
            return true;
        }

        /// <summary>
        ///     Verifies that a <see cref="ICategorizableThing" /> is categorized by a <see cref="Category" /> where the name is
        ///     <param name="categoryName"></param>
        /// </summary>
        /// <param name="thing">The <see cref="ICategorizableThing" /></param>
        /// <param name="categoryName">The name of the <see cref="Category" /></param>
        /// <returns>The asserts</returns>
        public static bool IsCategorizedBy(this ICategorizableThing thing, string categoryName)
        {
            return thing.GetAllCategories().Any(x => !x.IsDeprecated && string.Equals(x.Name, categoryName, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        ///     Verifies that a <see cref="ICategorizableThing" /> is categorized by a one of the <see cref="Category" /> where the name is
        ///     <param name="categories"></param>
        /// </summary>
        /// <param name="thing">The <see cref="ICategorizableThing" /></param>
        /// <param name="categories">The collection of name of the <see cref="Category" /></param>
        /// <returns>The asserts</returns>
        private static bool IsCategorizedBy(this ICategorizableThing thing, IReadOnlyCollection<string> categories)
        {
            return thing.GetAllCategories().Any(x => !x.IsDeprecated &&
                                                     categories.Any(cat => string.Equals(x.Name, cat, StringComparison.InvariantCultureIgnoreCase)));
        }

        /// <summary>
        ///     Query all <see cref="BinaryRelationship" /> that has a <see cref="Category" /> of name
        ///     <paramref name="categoryName" />
        /// </summary>
        /// <param name="thing">The <see cref="Thing" /></param>
        /// <param name="categoryName">The name of the <see cref="Category" /></param>
        /// <returns>A collection of <see cref="BinaryRelationship" /></returns>
        private static IEnumerable<BinaryRelationship> QueryRelationships(this Thing thing, string categoryName)
        {
            return thing.QueryRelationships.OfType<BinaryRelationship>().Where(x => x.Category.Any(cat => !cat.IsDeprecated
                                                                                                          && string.Equals(cat.Name, categoryName, StringComparison.InvariantCultureIgnoreCase)));
        }

        /// <summary>
        ///     Retrieves names of <see cref="Thing" />s that defines the <see cref="BinaryRelationship" />
        /// </summary>
        /// <param name="relationships">The <see cref="BinaryRelationship" /></param>
        /// <param name="sourceThing">
        ///     If the <see cref="Thing" /> to retrieve is the source of the target of the
        ///     <see cref="BinaryRelationship" />
        /// </param>
        /// <param name="getShortname">
        ///     Value indicating if the retrieved value should be the name or the shortname of the
        ///     <see cref="Thing" />
        /// </param>
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
