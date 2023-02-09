// --------------------------------------------------------------------------------------------------------
// <copyright file="ProductRowViewModel.cs" company="RHEA System S.A.">
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

    using UI_DSM.Client.Extensions;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Row view model to display content for a <see cref="ElementBase" /> that are Product
    /// </summary>
    public class ProductRowViewModel : ElementBaseRowViewModel, IBelongsToInterfaceView
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ProductRowViewModel" /> class.
        /// </summary>
        /// <param name="thing">The <see cref="ElementBase" /></param>
        /// <param name="reviewItem">The associated <see cref="HaveThingRowViewModel{TThing}.ReviewItem" /></param>
        public ProductRowViewModel(ElementBase thing, ReviewItem reviewItem) : base(thing, reviewItem)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProductRowViewModel" /> class.
        /// </summary>
        public ProductRowViewModel()
        {
        }

        /// <summary>
        ///     The computed Id
        /// </summary>
        public string ComputedId { get; private set; }

        /// <summary>
        ///     The value of the technology parameter
        /// </summary>
        public string TechnologyValue { get; private set; } = string.Empty;

        /// <summary>
        ///     If the <see cref="ProductRowViewModel" /> has a valid TRL parameter
        /// </summary>
        public bool HasValidTrl { get; private set; }

        /// <summary>
        ///     If the <see cref="ProductRowViewModel" /> has a valid technology parameter
        /// </summary>
        public bool HasValidTechnology { get; private set; }

        /// <summary>
        ///     The cost value
        /// </summary>
        public string CostValue { get; private set; } = string.Empty;

        /// <summary>
        ///     The collection of <see cref="Requirement" /> that are satisfied by the product
        /// </summary>
        public IEnumerable<Requirement> SatisfiedRequirements => this.Thing.GetRelatedThings<Requirement>(ThingExtension.SatisfyCategoryName, ClassKind.Requirement);

        /// <summary>
        ///     The colection of functions that are implemented by the product
        /// </summary>
        public IEnumerable<ElementUsage> ImplementedFunctions => this.Thing.GetRelatedThings<ElementUsage>(ThingExtension.ImplementCategoryName, ClassKind.ElementUsage,
            ThingExtension.FunctionCategoryName);

        /// <summary>
        ///     Gets the Id of the current <see cref="IHaveThingRowViewModel" />
        /// </summary>
        public override string Id => this.ComputedId;

        /// <summary>
        ///     The name of the source
        /// </summary>
        public string SourceName => null;

        /// <summary>
        ///     The name of the target
        /// </summary>
        public string TargetName => null;

        /// <summary>
        ///     The owner of the source
        /// </summary>
        public string SourceOwner => null;

        /// <summary>
        ///     The owner of the target
        /// </summary>
        public string TargetOwner => null;

        /// <summary>
        ///     The interface end of the Target
        /// </summary>
        public string TargetEnd => null;

        /// <summary>
        ///     The interface end of the Source
        /// </summary>
        public string SourceEnd => null;

        /// <summary>
        ///     A interface nature
        /// </summary>
        public string Nature => null;

        /// <summary>
        ///     Updates the <see cref="ElementBaseRowViewModel.CurrentOption" /> property
        /// </summary>
        /// <param name="selectedOption">The new <see cref="Option" /></param>
        public override void UpdateOption(Option selectedOption)
        {
            base.UpdateOption(selectedOption);
            this.CostValue = this.Thing.TryGetParameterValue("cost", this.CurrentOption, null, out var retrievedValue) ? retrievedValue : "-";
        }

        /// <summary>
        ///     Compute the <see cref="ComputedId" />
        /// </summary>
        /// <param name="shouldShowTechnology">If should include the technology</param>
        public void ComputeId(bool shouldShowTechnology)
        {
            if (shouldShowTechnology && this.Thing.ShouldHaveTechnologyParameter())
            {
                this.ComputedId = $"{this.Thing.Name} ({this.TechnologyValue})";
            }
            else
            {
                this.ComputedId = this.Thing.Name;
            }
        }

        /// <summary>
        ///     Initializes this row view model properties
        /// </summary>
        protected override void InitializesProperties()
        {
            base.InitializesProperties();

            if (!this.Thing.ShouldHaveTechnologyParameter())
            {
                this.HasValidTechnology = true;
            }
            else
            {
                this.HasValidTechnology = this.Thing.TryGetParameterValue("technology", null, null, out var retrievedValue);
                this.TechnologyValue = retrievedValue;
                this.HasValidTechnology &= this.TechnologyValue != "-";
            }

            if (!this.Thing.ShouldHaveTrlParameter())
            {
                this.HasValidTrl = true;
            }
            else
            {
                this.HasValidTrl = this.Thing.TryGetParameterValue("trl", null, null, out var retrievedValue);

                if (this.HasValidTrl && int.TryParse(retrievedValue, out var trl))
                {
                    this.TrlValue = trl;
                }
                else
                {
                    this.TrlValue = null;
                    this.HasValidTrl = false;
                }
            }

            this.ComputeId(false);
        }
    }
}
