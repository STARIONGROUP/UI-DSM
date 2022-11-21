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
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;

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
        ///     A collection of available <see cref="FilterModel" /> for rows
        /// </summary>
        public List<FilterModel> AvailableRowFilters { get; } = new();

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseViewViewModel" /> class.
        /// </summary>
        /// <param name="reviewItemService">The <see cref="IReviewItemService" /></param>
        protected ElementBreakdownStructureViewViewModel(IReviewItemService reviewItemService) : base(reviewItemService)
        {
        }

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
        ///     Initializes the <see cref="AvailableRowFilters"/> for <see cref="RequirementRowViewModel" /> rows
        /// </summary>
        protected void InitializesFilter<T>(string categoryNameFiltering) where T: ElementBaseRowViewModel
        {
            this.AvailableRowFilters.Clear();

            var owners = new List<DefinedThing>(this.AllRows.OfType<T>()
                .Select(x => x.Thing.Owner)
                .DistinctBy(x => x.Iid));

            this.AvailableRowFilters.Add(new FilterModel()
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

            this.AvailableRowFilters.Add(new FilterModel()
            {
                ClassKind = ClassKind.Category,
                DisplayName = "Category",
                Values = categories
            });
        }

        /// <summary>
        ///     Filters current rows
        /// </summary>
        /// <param name="selectedFilters">The selected filters</param>
        public abstract void FilterRows(IReadOnlyDictionary<ClassKind, List<FilterRow>> selectedFilters);
    }
}
