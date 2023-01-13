// --------------------------------------------------------------------------------------------------------
// <copyright file="ThingExtension.cs" company="RHEA System S.A.">
//  Copyright (c) 2023 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw, Nabil Abbar
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Server.Extensions
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
        ///     Gets all <see cref="Thing" /> that is referenced and contained into a <see cref="Thing" />
        /// </summary>
        /// <param name="thing">A <see cref="Thing" /></param>
        /// <returns>A collection of <see cref="Thing" /></returns>
        public static IEnumerable<Thing> GetContainedAndReferencedThings(this Thing thing)
        {
            var things = new List<Thing> { thing };

            things.AddRange(thing.QueryContainedThingsDeep());

            foreach (var deprecatedThing in things.Where(x => x is IDeprecatableThing { IsDeprecated: true }).ToList())
            {
                things.Remove(deprecatedThing);
            }

            foreach (var containedThing in things.ToList())
            {
                things.AddRange(containedThing.QueryRelationships);
            }

            foreach (var containedThings in things.ToList())
            {
                things.AddRange(containedThings.QueryReferencedThingsDeep());
            }

            foreach (var components in things.OfType<CompoundParameterType>()
                         .ToList().Select(compoundParameterType => compoundParameterType.Component))
            {
                things.AddRange(components);

                foreach (ParameterTypeComponent parameterTypeComponent in components)
                {
                    things.AddRange(parameterTypeComponent.QueryReferencedThingsDeep());
                    things.AddRange(parameterTypeComponent.QueryContainedThingsDeep());
                }
            }

            return things.DistinctById();
        }

        /// <summary>
        ///     Gets all <see cref="Thing" /> that is referenced and contained into all <see cref="Thing" /> from the
        ///     <see cref="collectionThings" />
        /// </summary>
        /// <param name="collectionThings">A collection of <see cref="Thing" /></param>
        /// <returns>A collection of <see cref="Thing" /></returns>
        public static IEnumerable<Thing> GetContainedAndReferencedThings(this IEnumerable<Thing> collectionThings)
        {
            var things = new List<Thing>();

            foreach (var thing in collectionThings)
            {
                things.AddRange(thing.GetContainedAndReferencedThings());
            }

            return things.DistinctById();
        }

        /// <summary>
        ///     Tries to get a <see cref="IEnumerable{T}" /> of <see cref="Thing" /> based on a <see cref="ClassKind" /> contained into an
        ///     <see cref="Iteration" />
        /// </summary>
        /// <param name="iteration">The <see cref="Iteration" /></param>
        /// <param name="classKind">The <see cref="ClassKind" /></param>
        /// <param name="things">The collection of <see cref="Thing" /></param>
        /// <returns>True if the <see cref="ClassKind" /> is available</returns>
        public static bool TryGetCollectionOfType(this Iteration iteration, ClassKind classKind, out IEnumerable<Thing> things)
        {
            things = default;

            Func<Iteration, IEnumerable<Thing>> collectionSelect = classKind switch
            {
                ClassKind.RequirementsSpecification => x => x.RequirementsSpecification.Where(req => !req.IsDeprecated),
                ClassKind.ElementDefinition => x => x.Element,
                _ => null
            };

            if (collectionSelect is null)
            {
                return false;
            }

            things = collectionSelect(iteration);
            return true;
        }

        /// <summary>
        ///     Distincts a collection of <see cref="Thing" /> by its <see cref="Thing.Iid" />
        /// </summary>
        /// <param name="things">The collection of <see cref="Thing" /></param>
        /// <returns>The distincted collection</returns>
        public static List<Thing> DistinctById(this IEnumerable<Thing> things)
        {
            return things.DistinctBy(x => x.Iid).ToList();
        }
    }
}
