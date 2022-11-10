// --------------------------------------------------------------------------------------------------------
// <copyright file="RelationshipRowViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel
{
    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;

    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Row view model to display content for a <see cref="ElementDefinition" />
    /// </summary>
    public class RelationshipRowViewModel : HaveThingRowViewModel<BinaryRelationship>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RelationshipRowViewModel" /> class.
        /// </summary>
        /// <param name="thing">The <see cref="BinaryRelationship" /></param>
        /// <param name="reviewItem">The associated <see cref="HaveThingRowViewModel{TThing}.ReviewItem" /></param>
        public RelationshipRowViewModel(BinaryRelationship thing, ReviewItem reviewItem) : base(thing, reviewItem)
        {
            this.InitializesProperties();
        }

        /// <summary>
        ///     Gets the Id of the current <see cref="RelationshipRowViewModel" />
        /// </summary>
        public override string Id => this.ComputedName;

        /// <summary>
        ///     The computed name of the <see cref="RelationshipRowViewModel" />
        /// </summary>
        public string ComputedName { get; set; }

        /// <summary>
        ///     Initiliazes this row properties
        /// </summary>
        private void InitializesProperties()
        {
            var source = FormatName(this.Thing.Source);
            var target = FormatName(this.Thing.Target);
            this.ComputedName = $"{source} → {target}";
        }

        /// <summary>
        ///     Format the name string for display which will include the shortname if available.
        /// </summary>
        /// <param name="thing">The thing.</param>
        /// <returns>The <see cref="string" /></returns>
        private static string FormatName(Thing thing)
        {
            var thingName = GetThingName(thing);

            var thingShortName = thing is IShortNamedThing shortNamedThing
                ? $" ({shortNamedThing.ShortName})"
                : string.Empty;

            return $"{thingName}{thingShortName}";
        }

        /// <summary>
        ///     Get the visual Name of the <see cref="Thing" />
        /// </summary>
        /// <param name="thing">The <see cref="Thing" /></param>
        /// <returns>Name of the <see cref="Thing" /></returns>
        private static string GetThingName(Thing thing)
        {
            return thing is INamedThing namedThing
                ? namedThing.Name
                : thing.UserFriendlyShortName;
        }
    }
}
