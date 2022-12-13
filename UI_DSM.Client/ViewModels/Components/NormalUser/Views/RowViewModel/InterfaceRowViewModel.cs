// --------------------------------------------------------------------------------------------------------
// <copyright file="InterfaceRowViewModel.cs" company="RHEA System S.A.">
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
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using UI_DSM.Client.Extensions;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Row view model to display content for a <see cref="BinaryRelationship" /> that are interface
    /// </summary>
    public class InterfaceRowViewModel : RelationshipRowViewModel, IBelongsToInterfaceView
    {
        /// <summary>
        ///     The source of the <see cref="BinaryRelationship" />
        /// </summary>
        private ElementUsage source;

        /// <summary>
        ///     The target of the <see cref="BinaryRelationship" />
        /// </summary>
        private ElementUsage target;

        /// <summary>
        /// The container of the source
        /// </summary>
        private ElementDefinition sourceContainer;

        /// <summary>
        /// The container of the target
        /// </summary>
        private ElementDefinition targetContainer;

        /// <summary>
        ///     Initializes a new instance of the <see cref="InterfaceRowViewModel" /> class.
        /// </summary>
        /// <param name="thing">The <see cref="BinaryRelationship" /></param>
        /// <param name="reviewItem">The associated <see cref="HaveThingRowViewModel{TThing}.ReviewItem" /></param>
        /// <param name="sourceRow">The <see cref="IHaveThingRowViewModel" /> for the source</param>
        /// <param name="targetRow">The <see cref="IHaveThingRowViewModel" /> for the target</param>
        public InterfaceRowViewModel(BinaryRelationship thing, ReviewItem reviewItem, 
            IHaveThingRowViewModel sourceRow, IHaveThingRowViewModel targetRow) : base(thing, reviewItem, sourceRow, targetRow)
        {
            this.InitializesProperties();
        }

        /// <summary>
        ///     The computed name of the thing
        /// </summary>
        public string ComputedName { get; private set; }

        /// <summary>
        ///     A interface nature
        /// </summary>
        public Category NatureCategory => this.Thing.GetAppliedCategories().FirstOrDefault();

        /// <summary>
        ///     The owner of the <see cref="BinaryRelationship" />
        /// </summary>
        public DomainOfExpertise InterfaceOwner => this.Thing.Owner;

        /// <summary>
        ///     Gets the Id of the current <see cref="IHaveThingRowViewModel" />
        /// </summary>
        public override string Id => this.ComputedName;

        /// <summary>
        ///     The owner of the interface
        /// </summary>
        public string Owner => this.Thing.GetOwnerShortName();

        /// <summary>
        ///     The interface end of the Target
        /// </summary>
        public string TargetEnd => $"{this.targetContainer.Name}.{this.target?.Name} ({this.target?.InterfaceEnd})";

        /// <summary>
        ///     The interface end of the Source
        /// </summary>
        public string SourceEnd => $"{this.sourceContainer.Name}.{this.source?.Name} ({this.source?.InterfaceEnd})";

        /// <summary>
        ///     A interface nature
        /// </summary>
        public string Nature => this.NatureCategory?.Name;

        /// <summary>
        ///     Initiliazes this row properties
        /// </summary>
        private void InitializesProperties()
        {
            this.source = this.Thing.Source as ElementUsage;
            this.target = this.Thing.Target as ElementUsage;
            this.sourceContainer = this.source?.Container as ElementDefinition;
            this.targetContainer = this.target?.Container as ElementDefinition;
            this.ComputedName = !string.IsNullOrEmpty(this.Thing.Name) ? this.Thing.Name : $"{this.SourceEnd} → {this.TargetEnd}";
        }
    }
}
