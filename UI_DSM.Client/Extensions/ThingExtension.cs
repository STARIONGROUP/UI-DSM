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
    using CDP4Common.Types;

    using UI_DSM.Client.Enumerator;
    using UI_DSM.Shared.Enumerator;

    /// <summary>
    ///     Extension class for <see cref="Thing" />
    /// </summary>
    public static class ThingExtension
    {
        /// <summary>
        ///     The name of the satisfy <see cref="Category" />
        /// </summary>
        public const string SatisfyCategoryName = "satisfies";

        /// <summary>
        ///     The name of the implement <see cref="Category" />
        /// </summary>
        public const string ImplementCategoryName = "implements";

        /// <summary>
        ///     The name of the derive <see cref="Category" />
        /// </summary>
        public const string DeriveCategoryName = "derives";

        /// <summary>
        ///     The name of the function <see cref="Category" />
        /// </summary>
        public const string FunctionCategoryName = "functions";

        /// <summary>
        ///     The name of the product <see cref="Category" />
        /// </summary>
        public const string ProductCategoryName = "products";

        /// <summary>
        ///     The name of the port <see cref="Category" />
        /// </summary>
        public const string PortCategoryName = "ports";

        /// <summary>
        ///     The name of the interface <see cref="Category" />
        /// </summary>
        public const string InterfaceCategoryName = "interfaces";

        /// <summary>
        ///     The name of the trace <see cref="Category" />
        /// </summary>
        public const string TraceCategoryName = "trace";

        /// <summary>
        ///     The name of the review external content <see cref="Parameter" />
        /// </summary>
        public const string ReviewExternalContentName = "review external content";

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
        ///     Collection of <see cref="Category" /> name for <see cref="ElementDefinition" /> that should have a TRL parameter
        /// </summary>
        private static readonly List<string> MandatoryCategoriesForTrl = new()
        {
            "equipment"
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
            return thing.GetAppliedCategories(includeDeprecated).Select(x => x.Name);
        }

        /// <summary>
        ///     Gets all applied <see cref="Category" />
        /// </summary>
        /// <param name="thing">The <see cref="ICategorizableThing" /></param>
        /// <param name="includeDeprecated">If deprecated <see cref="Category" /> should be included</param>
        /// <returns>A collection of <see cref="Category" /> </returns>
        public static IEnumerable<Category> GetAppliedCategories(this ICategorizableThing thing, bool includeDeprecated = false)
        {
            var categories = (includeDeprecated ? thing.Category : thing.Category.Where(x => !x.IsDeprecated)).ToList();

            if (thing is ElementUsage usage)
            {
                categories.AddRange(includeDeprecated ? usage.ElementDefinition.Category : usage.ElementDefinition.Category.Where(x => !x.IsDeprecated));
            }

            return categories.DistinctBy(x => x.Iid);
        }

        /// <summary>
        ///     Verifies that an <see cref="ICategorizableThing" /> has valid Categories based on the prefilters
        /// </summary>
        /// <param name="categorizable">The <see cref="ICategorizableThing" /></param>
        /// <param name="prefilters">The collection of prefilters</param>
        /// <returns>True if is valid</returns>
        public static bool IsValidForPrefilter(this ICategorizableThing categorizable, List<string> prefilters)
        {
            return !prefilters.Any() || categorizable.IsCategorizedBy(prefilters);
        }

        /// <summary>
        ///     Verifies that an <see cref="ElementDefinition" /> has valid Review External Content parameter value based on the prefilters
        /// </summary>
        /// <param name="elementDefinition">The <see cref="ElementDefinition" /></param>
        /// <param name="prefilters">The collection of prefilters</param>
        /// <returns>True if is valid</returns>
        public static bool HasValidReviewExternalContent(this ElementDefinition elementDefinition, List<string> prefilters)
        {
            var parameter = elementDefinition.Parameter
                .FirstOrDefault(x => string.Equals(x.ParameterType.Name, ReviewExternalContentName, StringComparison.InvariantCultureIgnoreCase));

            if (parameter == null)
            {
                return false;
            }

            if (!prefilters.Any())
            {
                return true;
            }

            var values = parameter.QueryParameterBaseValueSet(null, null);
            return values.ActualValue.Any(x => prefilters.Any(prefilter => prefilter == x));
        }

        /// <summary>
        ///     Verifies that an <see cref="Requirement" /> has valid Review External Content parameter value based on the prefilters
        /// </summary>
        /// <param name="requirement">The <see cref="Requirement" /></param>
        /// <param name="prefilters">The collection of prefilters</param>
        /// <returns>True if is valid</returns>
        public static bool HasValidReviewExternalContent(this Requirement requirement, List<string> prefilters)
        {
            var parameter = requirement.ParameterValue
                .FirstOrDefault(x => string.Equals(x.ParameterType.Name, ReviewExternalContentName, StringComparison.InvariantCultureIgnoreCase));

            if (parameter == null)
            {
                return false;
            }

            if (!prefilters.Any())
            {
                return true;
            }

            var values = parameter.Value;
            return values.Any(x => prefilters.Any(prefilter => prefilter == x));
        }

        /// <summary>
        ///     Asserts that a <see cref="ElementDefinition" /> has a parameter value Review External Content
        /// </summary>
        /// <param name="elementDefinition">The <see cref="ElementDefinition" /></param>
        /// <returns>True if contains the parameter value</returns>
        public static bool HasReviewExternalContent(this ElementDefinition elementDefinition)
        {
            return elementDefinition.Parameter
                .Any(x => string.Equals(x.ParameterType.Name, ReviewExternalContentName, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        ///     Asserts that a <see cref="Requirement" /> has a parameter value Review External Content
        /// </summary>
        /// <param name="requirement">The <see cref="Requirement" /></param>
        /// <returns>True if contains the parameter value</returns>
        public static bool HasReviewExternalContent(this Requirement requirement)
        {
            return requirement.ParameterValue
                .Any(x => string.Equals(x.ParameterType.Name, ReviewExternalContentName, StringComparison.InvariantCultureIgnoreCase));
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
        ///     Gets the shortname of the Owner of a <see cref="Thing" />
        /// </summary>
        /// <param name="thing">The <see cref="Thing" /></param>
        /// <returns>The shortname</returns>
        public static string GetOwnerShortName(this Thing thing)
        {
            return thing is IOwnedThing ownedThing ? ownedThing.GetOwnerShortName() : string.Empty;
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
        /// <typeparam name="TThing">A <see cref="Thing"/></typeparam>
        /// <returns>A collection of <see cref="TThing" />s</returns>
        public static IEnumerable<TThing> GetRelatedThings<TThing>(this Thing thing, string categoryName, ClassKind classKind, bool isSource = true) where TThing : Thing
        {
            Func<BinaryRelationship, bool> filter = isSource
                ? x => x.Source.Iid == thing.Iid && x.Target.ClassKind == classKind
                : x => x.Target.Iid == thing.Iid && x.Source.ClassKind == classKind;

            var binaryRelationships = thing.QueryRelationships(categoryName).Where(filter);

            return binaryRelationships.Select(x => GetRelatedThing<TThing>(x, !isSource))
                .Where(x => x != null);
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
        /// <typeparam name="TThing">A <see cref="Thing"/></typeparam>
        /// <returns>A collection of <see cref="TThing" />s</returns>
        public static IEnumerable<TThing> GetRelatedThings<TThing>(this Thing thing, string categoryName, ClassKind classKind, string filterCategory,
            bool isSource = true) where TThing : Thing
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

            var binaryRelationships = thing.QueryRelationships(categoryName).Where(filter);

            return binaryRelationships.Select(x => GetRelatedThing<TThing>(x, !isSource))
                .Where(x => x != null);
        }

        /// <summary>
        ///     Gets the <see cref="BinaryRelationship" /> that linked two <see cref="Thing" />
        /// </summary>
        /// <param name="thing">The <see cref="Thing" /></param>
        /// <param name="otherThingId">The <see cref="Guid" /> of the other <see cref="Thing" /></param>
        /// <param name="relationshipCategory">
        ///     The name of the <see cref="Category" /> of the <see cref="BinaryRelationship" />
        /// </param>
        /// <returns>The <see cref="BinaryRelationship" /> that link the two <see cref="Thing" /></returns>
        public static BinaryRelationship GetLinkTo(this Thing thing, Guid otherThingId, string relationshipCategory)
        {
            if (thing.Iid == otherThingId)
            {
                return null;
            }

            var binaryRelationships = thing.QueryRelationships(relationshipCategory);
            return binaryRelationships.FirstOrDefault(x => x.Source.Iid == thing.Iid && x.Target.Iid == otherThingId);
        }

        /// <summary>
        ///     Gets the name of the <see cref="RequirementsSpecification" /> of a <see cref="Requirement" />
        /// </summary>
        /// <param name="requirement">The <see cref="Requirement" /></param>
        /// <returns>The name of the <see cref="RequirementsSpecification" /></returns>
        public static string GetSpecificationName(this Requirement requirement)
        {
            var specification = requirement.Container as RequirementsSpecification;
            return specification?.Name;
        }

        /// <summary>
        ///     Verifies that a <see cref="ElementBase" /> is a product
        /// </summary>
        /// <param name="elementBase">The <see cref="ElementBase" /></param>
        /// <returns>The asserts</returns>
        public static bool IsProduct(this ElementBase elementBase)
        {
            return elementBase.IsCategorizedBy(ProductCategoryName);
        }

        /// <summary>
        ///     Verifies that a <see cref="ElementBase" /> is a port
        /// </summary>
        /// <param name="elementBase">The <see cref="ElementBase" /></param>
        /// <returns>The asserts</returns>
        public static bool IsPort(this ElementBase elementBase)
        {
            return elementBase.IsCategorizedBy(PortCategoryName);
        }

        /// <summary>
        ///     Verifies that a <see cref="ElementBase" /> is a function
        /// </summary>
        /// <param name="elementDefinition">The <see cref="ElementBase" /></param>
        /// <returns>The asserts</returns>
        public static bool IsFunction(this ElementBase elementDefinition)
        {
            return elementDefinition.IsCategorizedBy(FunctionCategoryName);
        }

        /// <summary>
        ///     Asserts that a Product has to have a Technology parameter
        /// </summary>
        /// <param name="elementBase">The <see cref="ElementBase" /></param>
        /// <returns>The assert</returns>
        public static bool ShouldHaveTechnologyParameter(this ElementBase elementBase)
        {
            return elementBase.IsCategorizedBy(MandatoryCategoriesForTechnology);
        }

        /// <summary>
        ///     Asserts that a Product has to have a TRL parameter
        /// </summary>
        /// <param name="elementBase">The <see cref="ElementBase" /></param>
        /// <returns>The assert</returns>
        public static bool ShouldHaveTrlParameter(this ElementBase elementBase)
        {
            return elementBase.IsCategorizedBy(MandatoryCategoriesForTrl);
        }

        /// <summary>
        ///     Gets the collection of <see cref="View" /> where the current <see cref="Thing" /> can be visible
        /// </summary>
        /// <param name="thing">The <see cref="Thing" /></param>
        /// <returns>A collection of <see cref="View" /></returns>
        public static IEnumerable<View> GetAvailableViews(this Thing thing)
        {
            return thing switch
            {
                Requirement => new List<View>
                {
                    View.RequirementBreakdownStructureView,
                    View.RequirementVerificationControlView,
                    View.RequirementTraceabilityToRequirementView,
                    View.RequirementTraceabilityToFunctionView,
                    View.RequirementTraceabilityToProductView
                },
                ElementBase elementBase when elementBase.IsProduct() => new List<View>
                {
                    View.ProductBreakdownStructureView,
                    View.TrlView,
                    View.RequirementTraceabilityToProductView,
                    View.FunctionalTraceabilityToProductView,
                    View.InterfaceView
                },
                ElementBase elementBase when elementBase.IsFunction() => new List<View>
                {
                    View.FunctionalBreakdownStructureView,
                    View.RequirementTraceabilityToFunctionView,
                    View.FunctionalTraceabilityToProductView
                },
                ElementBase elementBase when elementBase.IsPort() => new List<View>
                {
                    View.InterfaceView
                },
                ElementBase elementBase when !elementBase.IsPort() && !elementBase.IsFunction() && !elementBase.IsProduct() => new List<View>
                {
                    View.FunctionalBreakdownStructureView,
                    View.ProductBreakdownStructureView
                },
                BinaryRelationship binaryRelationship when binaryRelationship.IsInterface() => new List<View>
                {
                    View.InterfaceView
                },
                BinaryRelationship binaryRelationship when binaryRelationship.IsCategorizedBy(DeriveCategoryName) => new List<View>
                {
                    View.RequirementBreakdownStructureView,
                    View.RequirementVerificationControlView
                },
                BinaryRelationship binaryRelationship when binaryRelationship.IsCategorizedBy(TraceCategoryName) => new List<View>
                {
                    View.RequirementTraceabilityToRequirementView
                },
                BinaryRelationship binaryRelationship when binaryRelationship.IsCategorizedBy(ImplementCategoryName) => new List<View>
                {
                    View.FunctionalTraceabilityToProductView
                },
                BinaryRelationship binaryRelationship when binaryRelationship.IsCategorizedBy(SatisfyCategoryName)
                                                           && binaryRelationship.Source is ElementBase elementBase
                                                           && elementBase.IsProduct() => new List<View>
                {
                    View.RequirementTraceabilityToProductView
                },
                BinaryRelationship binaryRelationship when binaryRelationship.IsCategorizedBy(SatisfyCategoryName)
                                                           && binaryRelationship.Source is ElementBase elementBase
                                                           && elementBase.IsFunction() => new List<View>
                {
                    View.RequirementTraceabilityToFunctionView
                },
                HyperLink => new List<View> { View.DocumentBased },
                _ => Enumerable.Empty<View>()
            };
        }

        /// <summary>
        ///     Gets the specific object kind name base on a <see cref="Thing" />
        /// </summary>
        /// <param name="thing">The <see cref="Thing" /></param>
        /// <returns>The name of the category if any</returns>
        public static string GetSpecificObjectKindForThing(this Thing thing)
        {
            return thing switch
            {
                ElementBase elementBase when elementBase.IsProduct() => "Product",
                ElementBase elementBase when elementBase.IsFunction() => "Function",
                ElementBase elementBase when elementBase.IsPort() => "Port",
                BinaryRelationship binaryRelationship when binaryRelationship.IsInterface() => "Interface",
                BinaryRelationship binaryRelationship when binaryRelationship.IsCategorizedBy(DeriveCategoryName) => "Requirement flow-down Traceability",
                BinaryRelationship binaryRelationship when binaryRelationship.IsCategorizedBy(TraceCategoryName) => "Requirement Traces",
                BinaryRelationship binaryRelationship when binaryRelationship.IsCategorizedBy(ImplementCategoryName) => "Function Implementation Traceability",
                BinaryRelationship binaryRelationship when binaryRelationship.IsCategorizedBy(SatisfyCategoryName) => $"Requirement Implementation Traceability",
                _ => thing.GetType().Name
            };
        }

        /// <summary>
        ///     Gets the specific category name base on a <see cref="Thing" />
        /// </summary>
        /// <param name="thing">The <see cref="Thing" /></param>
        /// <returns>The name of the category if any</returns>
        public static string GetSpecificCategoryForThing(this Thing thing)
        {
            return thing switch
            {
                ElementBase elementBase when elementBase.IsProduct() => elementBase.GetCategories().First(),
                ElementBase elementBase when elementBase.IsFunction() => elementBase.GetCategories().First(),
                BinaryRelationship binaryRelationship when binaryRelationship.IsInterface() => binaryRelationship.GetCategories().First(),
                _ => string.Empty
            };
        }

        /// <summary>
        ///     Gets the specific name of a <see cref="Thing" />
        /// </summary>
        /// <param name="thing">The <see cref="Thing" /></param>
        /// <returns>The name of the <see cref="Thing" /></returns>
        public static string GetSpecificNameForThing(this Thing thing)
        {
            return thing switch
            {
                ElementDefinition elementDefinition => elementDefinition.Name,
                ElementUsage elementUsage => $"{((ElementDefinition)elementUsage.Container).Name}.{elementUsage.Name}",
                BinaryRelationship binaryRelationship => $"{binaryRelationship.Source.GetSpecificNameForThing()} -> {binaryRelationship.Target.GetSpecificNameForThing()}",
                Requirement requirement => requirement.ShortName,
                HyperLink hyperLink => hyperLink.Content,
                _ => thing.GetName()
            };
        }

        /// <summary>
        ///     Gets the <see cref="Thing.Container" /> if the current <see cref="ElementBase" /> is an
        ///     <see cref="ElementUsage" />
        /// </summary>
        /// <param name="element">The <see cref="ElementBase" /></param>
        /// <returns>A <see cref="ElementDefinition"/> container</returns>
        public static ElementDefinition GetElementContainer(this ElementBase element)
        {
            return element.GetContainerOfType<ElementDefinition>();
        }

        /// <summary>
        ///     Get all <see cref="ParameterOrOverrideBase" /> contained into an <see cref="ElementBase" />
        /// </summary>
        /// <param name="element">The <see cref="ElementBase" /></param>
        /// <returns>A collection of <see cref="ParameterOrOverrideBase" /></returns>
        public static List<ParameterOrOverrideBase> GetAllParameters(this ElementBase element)
        {
            switch (element)
            {
                case ElementDefinition elementDefinition:
                    return elementDefinition.Parameter.ToList<ParameterOrOverrideBase>();
                case ElementUsage elementUsage:
                {
                    var parameters = elementUsage.ElementDefinition.GetAllParameters();
                    var parametersOverride = elementUsage.ParameterOverride;
                    parameters.RemoveAll(p => parametersOverride.Any(x => x.Parameter.Iid == p.Iid));
                    parameters.AddRange(parametersOverride);
                    return parameters;
                }
                default:
                    return new List<ParameterOrOverrideBase>();
            }
        }

        /// <summary>
        ///     Tries to get the value of a defined parameter
        /// </summary>
        /// <param name="elementBase">The <see cref="ElementBase" /></param>
        /// <param name="parameterName">The name of the <see cref="ParameterType" /></param>
        /// <param name="option">The <see cref="Option" /></param>
        /// <param name="state">The <see cref="ActualFiniteState" /></param>
        /// <param name="value">The value of the parameter</param>
        /// <returns>True if has a the parameter</returns>
        public static bool TryGetParameterValue(this ElementBase elementBase, string parameterName, Option option, ActualFiniteState state, out string value)
        {
            value = string.Empty;

            return elementBase switch
            {
                ElementDefinition elementDefinition => elementDefinition.TryGetParameterValue(parameterName, option, state, out value),
                ElementUsage usage => usage.TryGetParameterValue(parameterName, option, state, out value),
                _ => false
            };
        }

        /// <summary>
        ///     Verifies that a <see cref="BinaryRelationship" /> is an Interface
        /// </summary>
        /// <param name="relationShip">The <see cref="BinaryRelationship" /></param>
        /// <returns>True if the <see cref="BinaryRelationship" /> is an Interface</returns>
        public static bool IsInterface(this BinaryRelationship relationShip)
        {
            return relationShip.IsCategorizedBy(InterfaceCategoryName)
                   && relationShip.Target is ElementUsage targetUsage
                   && targetUsage.IsPort()
                   && relationShip.Source is ElementUsage sourceUsage
                   && sourceUsage.IsPort();
        }

        /// <summary>
        ///     Gets the value of a <see cref="ParameterOrOverrideBase" />
        /// </summary>
        /// <param name="parameter">The <see cref="ParameterOrOverrideBase" /></param>
        /// <param name="option">The <see cref="Option" /></param>
        /// <param name="state">The <see cref="ActualFiniteState" /></param>
        /// <param name="index">The index inside the <see cref="ValueArray{T}" /></param>
        /// <returns>The parameter value</returns>
        public static string GetParameterValue(this ParameterOrOverrideBase parameter, Option option, ActualFiniteState state, int index = 0)
        {
            return parameter.GetParameterValueArray(option, state)[index];
        }

        /// <summary>
        ///     Gets the <see cref="ValueArray{T}" /> of a <see cref="ParameterOrOverrideBase" />
        /// </summary>
        /// <param name="parameter">The <see cref="ParameterOrOverrideBase" /></param>
        /// <param name="option">The <see cref="Option" /></param>
        /// <param name="state">The <see cref="ActualFiniteState" /></param>
        /// <returns>The <see cref="ValueArray{T}" /> value</returns>
        public static ValueArray<string> GetParameterValueArray(this ParameterOrOverrideBase parameter, Option option, ActualFiniteState state)
        {
            var optionToQuery = parameter.IsOptionDependent ? option : null;

            if (optionToQuery == null && parameter.IsOptionDependent)
            {
                optionToQuery = parameter.ValueSets.First(x => x.ActualOption != null).ActualOption;
            }

            ActualFiniteState stateToQuery = null;

            if (parameter.StateDependence != null)
            {
                stateToQuery = parameter.StateDependence.ActualState.Any(x => x.Iid == state?.Iid)
                    ? state
                    : parameter.StateDependence.ActualState.FirstOrDefault();
            }

            return parameter.QueryParameterBaseValueSet(optionToQuery, stateToQuery).ActualValue;
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
        ///     Gets the proper name of a <see cref="Thing" />
        /// </summary>
        /// <param name="thing">The <see cref="Thing" /></param>
        /// <returns>The name</returns>
        public static string GetName(this Thing thing)
        {
            return thing is INamedThing namedThing ? namedThing.Name : thing.UserFriendlyName;
        }

        /// <summary>
        ///     Gets a collection of <see cref="ElementBase" /> that are product linked to the current <see cref="ElementBase" />
        /// </summary>
        /// <param name="function">The <see cref="ElementBase" /></param>
        /// <returns>A collection of products</returns>
        public static List<ElementBase> GetLinkedProducts(this ElementBase function)
        {
            if (!function.IsFunction())
            {
                return new List<ElementBase>();
            }

            var relationShips = function.QueryRelationships(ImplementCategoryName);

            return relationShips.Where(x => x.Source is ElementBase elementBase && elementBase.IsProduct())
                .Select(x => x.Source as ElementBase).ToList();
        }

        /// <summary>
        ///     Gets all interface <see cref="BinaryRelationship" /> linked to the current port
        /// </summary>
        /// <param name="port">The port</param>
        /// <returns>A collection of <see cref="BinaryRelationship" /></returns>
        public static List<BinaryRelationship> GetInterfacesOfPort(this ElementUsage port)
        {
            return port.QueryRelationships.OfType<BinaryRelationship>().Where(x => x.IsInterface())
                .ToList();
        }

        /// <summary>
        ///     Convert the <see cref="Category"/> to an <see cref="InterfaceCategory"/>
        /// </summary>
        /// <param name="category">the <see cref="Category"/> to convert</param>
        /// <returns>The <see cref="InterfaceCategory"/></returns>
        public static InterfaceCategory ToInterfaceCategory(this Category category)
        {
            if (!Enum.TryParse(typeof(InterfaceCategory), category.ShortName, true, out var value))
            {
                return InterfaceCategory.Other;
            }

            return (InterfaceCategory)value!;
        }

        /// <summary>
        ///     Gets the shortname of the <see cref="IOwnedThing.Owner" />
        /// </summary>
        /// <param name="thing">The <see cref="IOwnedThing" /></param>
        /// <returns>The shortname</returns>
        private static string GetOwnerShortName(this IOwnedThing thing)
        {
            return thing.Owner.ShortName;
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
        private static bool TryGetParameterValue(this ElementDefinition elementDefinition, string parameterName, Option option, ActualFiniteState state, out string value)
        {
            value = string.Empty;

            var parameter = elementDefinition.Parameter
                .FirstOrDefault(x => string.Equals(x.ParameterType.Name, parameterName, StringComparison.InvariantCultureIgnoreCase));

            if (parameter == null)
            {
                return false;
            }

            value = parameter.GetParameterValue(option, state);
            return true;
        }

        /// <summary>
        ///     Tries to get the value of a defined parameter
        /// </summary>
        /// <param name="elementUsage">The <see cref="ElementUsage" /></param>
        /// <param name="parameterName">The name of the <see cref="ParameterType" /></param>
        /// <param name="option">The <see cref="Option" /></param>
        /// <param name="state">The <see cref="ActualFiniteState" /></param>
        /// <param name="value">The value of the parameter</param>
        /// <returns>True if has a the parameter</returns>
        private static bool TryGetParameterValue(this ElementUsage elementUsage, string parameterName, Option option, ActualFiniteState state, out string value)
        {
            value = string.Empty;

            var parameter = elementUsage.ParameterOverride
                .FirstOrDefault(x => string.Equals(x.ParameterType.Name, parameterName, StringComparison.InvariantCultureIgnoreCase));

            if (parameter == null)
            {
                return elementUsage.ElementDefinition.TryGetParameterValue(parameterName, option, state, out value);
            }

            value = parameter.GetParameterValue(option, state);
            return true;
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
                                                     categories.Any(cat => string.Equals(x.Name, cat, StringComparison.InvariantCultureIgnoreCase)
                                                                           || string.Equals(x.ShortName, cat, StringComparison.InvariantCultureIgnoreCase)));
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
        ///     Gets the a <see cref="Thing" /> that define a <see cref="BinaryRelationship" />
        /// </summary>
        /// <param name="binaryRelationship">The <see cref="BinaryRelationship" /></param>
        /// <param name="sourceThing">
        ///     If the <see cref="Thing" /> to retrieve is the source of the target of the
        ///     <see cref="BinaryRelationship" />
        /// </param>
        /// <returns>The <see cref="Thing" /></returns>
        private static TThing GetRelatedThing<TThing>(BinaryRelationship binaryRelationship, bool sourceThing) where TThing: Thing
        {
            if (binaryRelationship == null)
            {
                return null;
            }

            var relatedThing= sourceThing ? binaryRelationship.Source : binaryRelationship.Target;
            return relatedThing as TThing;
        }
    }
}
