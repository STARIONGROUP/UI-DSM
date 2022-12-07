// --------------------------------------------------------------------------------------------------------
// <copyright file="ElementBreakdownStructureViewViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Components.NormalUser.Views
{
    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;

    using UI_DSM.Client.Extensions;
    using UI_DSM.Client.Model;
    using UI_DSM.Client.Services.ReviewItemService;
    using UI_DSM.Client.ViewModels.App.Filter;
    using UI_DSM.Client.ViewModels.App.OptionChooser;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Base view model for all breakdown Structure view for <see cref="ElementBase" />
    /// </summary>
    public abstract class ElementBreakdownStructureViewViewModel : BaseViewViewModel, IElementBreakdownStructureViewViewModel
    {
        /// <summary>
        ///     A collection of all available <see cref="ElementBaseRowViewModel" />
        /// </summary>
        protected readonly List<ElementBaseRowViewModel> AllRows = new();

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseViewViewModel" /> class.
        /// </summary>
        /// <param name="reviewItemService">The <see cref="IReviewItemService" /></param>
        /// <param name="filterViewModel">The <see cref="IFilterViewModel" /></param>
        /// <param name="optionChooserViewModel">The <see cref="IOptionChooserViewModel" /></param>
        protected ElementBreakdownStructureViewViewModel(IReviewItemService reviewItemService,
            IFilterViewModel filterViewModel, IOptionChooserViewModel optionChooserViewModel) : base(reviewItemService)
        {
            this.FilterViewModel = filterViewModel;
            this.OptionChooserViewModel = optionChooserViewModel;
        }

        /// <summary>
        ///     The <see cref="FilterViewModel" />
        /// </summary>
        public IFilterViewModel FilterViewModel { get; private set; }

        /// <summary>
        ///     The <see cref="IOptionChooserViewModel" />
        /// </summary>
        public IOptionChooserViewModel OptionChooserViewModel { get; private set; }

        /// <summary>
        ///     A collection of <see cref="ElementBaseRowViewModel" /> for the top element
        /// </summary>
        public List<ElementBaseRowViewModel> TopElement { get; private set; } = new();

        /// <summary>
        ///     Loads all children <see cref="ElementBaseRowViewModel" /> of the <see cref="ElementBaseRowViewModel" /> parent
        /// </summary>
        /// <param name="parent">The <see cref="ElementBaseRowViewModel" />parent</param>
        /// <returns>A collection of <see cref="ElementBaseRowViewModel" /> children</returns>
        public IEnumerable<ElementBaseRowViewModel> LoadChildren(ElementBaseRowViewModel parent)
        {
            return this.AllRows.Where(x => x.ContainerId == parent.ElementDefinitionId)
                .OrderBy(x => x.Name).ToList();
        }

        /// <summary>
        ///     Verifies if a <see cref="ElementBaseRowViewModel" /> has children
        /// </summary>
        /// <param name="rowData">The <see cref="ElementBaseRowViewModel" /></param>
        /// <returns>The assert</returns>
        public bool HasChildren(ElementBaseRowViewModel rowData)
        {
            return this.AllRows.Any(x => x.ContainerId == rowData.ElementDefinitionId && x.IsVisible);
        }

        /// <summary>
        ///     Verifies that a <see cref="ElementBaseRowViewModel" /> is visible
        /// </summary>
        /// <param name="rowData">The <see cref="ElementBaseRowViewModel" /></param>
        /// <returns>The assert</returns>
        public bool IsVisible(ElementBaseRowViewModel rowData)
        {
            if (this.TopElement.Any(x => x.ThingId == rowData.ThingId))
            {
                return true;
            }

            var row = this.AllRows.FirstOrDefault(x => x.ThingId == rowData.ThingId);
            return row is { IsVisible: true };
        }

        /// <summary>
        ///     Initialize this view model properties
        /// </summary>
        /// <param name="things">A collection of <see cref="Thing" /></param>
        /// <param name="projectId">The <see cref="Project" /> id</param>
        /// <param name="reviewId">The <see cref="Review" /> id</param>
        /// <param name="prefilters">A collection of prefilters</param>
        /// <param name="additionnalColumnsVisibleAtStart">A collection of columns name that can be visible by default at start</param>
        /// <returns>A <see cref="Task" /></returns>
        public override async Task InitializeProperties(IEnumerable<Thing> things, Guid projectId, Guid reviewId, List<string> prefilters, List<string> additionnalColumnsVisibleAtStart)
        {
            await base.InitializeProperties(things, projectId, reviewId, prefilters, additionnalColumnsVisibleAtStart);

            this.OptionChooserViewModel.InitializesViewModel(this.Things.OfType<Option>().OrderBy(x => x.RevisionNumber).ToList());
        }

        /// <summary>
        ///     Filters current rows
        /// </summary>
        /// <param name="selectedFilters">The selected filters</param>
        public abstract void FilterRows(IReadOnlyDictionary<ClassKind, List<FilterRow>> selectedFilters);

        /// <summary>
        ///     Tries to set the <see cref="IBaseViewViewModel.SelectedElement" /> to the previous selected item
        /// </summary>
        /// <param name="selectedItem">The previously selectedItem</param>
        public override void TrySetSelectedItem(object selectedItem)
        {
            if (selectedItem is ElementBaseRowViewModel elementBaseRow)
            {
                var topElement = this.TopElement.FirstOrDefault(x => x.ThingId == elementBaseRow.ThingId);
                this.SelectedElement = topElement ?? this.AllRows.FirstOrDefault(x => x.ThingId == elementBaseRow.ThingId);
            }
        }

        /// <summary>
        ///     Initializes the filters criteria for rows
        /// </summary>
        protected void InitializesFilter<T>(string categoryNameFiltering) where T : ElementBaseRowViewModel
        {
            var availableRowFilters = new List<FilterModel>();

            var owners = new List<DefinedThing>(this.AllRows.OfType<T>()
                    .Select(x => x.Thing.Owner)
                    .DistinctBy(x => x.Iid))
                .OrderBy(x => x.Name)
                .ToList();

            availableRowFilters.Add(new FilterModel
            {
                ClassKind = ClassKind.DomainOfExpertise,
                UseShortName = true,
                DisplayName = "Owner",
                Values = owners
            });

            var categories = new List<DefinedThing>(this.AllRows.OfType<T>()
                    .SelectMany(x => x.Thing.GetAppliedCategories())
                    .Where(x => string.Equals(categoryNameFiltering, x.Name, StringComparison.InvariantCultureIgnoreCase)
                                || x.AllSuperCategories()
                                    .Any(cat => !cat.IsDeprecated && string.Equals(cat.Name, categoryNameFiltering, StringComparison.InvariantCultureIgnoreCase)))
                    .DistinctBy(x => x.Iid))
                .OrderBy(x => x.Name)
                .ToList();

            availableRowFilters.Add(new FilterModel
            {
                ClassKind = ClassKind.Category,
                DisplayName = "Category",
                Values = categories
            });

            this.FilterViewModel.InitializeProperties(availableRowFilters);
        }
    }
}
