// --------------------------------------------------------------------------------------------------------
// <copyright file="RequirementTraceabilityToProductViewViewModel.cs" company="RHEA System S.A.">
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

    using ReactiveUI;

    using UI_DSM.Client.Extensions;
    using UI_DSM.Client.Model;
    using UI_DSM.Client.Services.ReviewItemService;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     ViewModel for the <see cref="Client.Components.NormalUser.Views.RequirementTraceabilityToProductView" />
    ///     component
    /// </summary>
    public class RequirementTraceabilityToProductViewViewModel : HaveTraceabilityTableViewModel, IRequirementTraceabilityToProductViewViewModel
    {
        /// <summary>
        ///     Backing field for <see cref="IsOnTechnologyView" />
        /// </summary>
        private bool isOnTechnologyView;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RequirementTraceabilityToProductViewViewModel" /> class.
        /// </summary>
        /// <param name="reviewItemService">The <see cref="IReviewItemService" /></param>
        public RequirementTraceabilityToProductViewViewModel(IReviewItemService reviewItemService) : base(reviewItemService)
        {
        }

        /// <summary>
        ///     The header name of the column
        /// </summary>
        protected override string HeaderName => "prod/req";

        /// <summary>
        ///     The name of the category
        /// </summary>
        protected override string TraceCategoryName => "satisfies";

        /// <summary>
        ///     Apply a filtering on rows
        /// </summary>
        /// <param name="selectedFilters">The selected filters</param>
        public override void FilterRows(Dictionary<ClassKind, List<FilterRow>> selectedFilters)
        {
            var selectedOwners = selectedFilters[ClassKind.DomainOfExpertise];

            foreach (var productRowViewModel in this.TraceabilityTableViewModel.Rows.OfType<ProductRowViewModel>())
            {
                productRowViewModel.IsVisible = selectedOwners.Any(x => x.DefinedThing.Iid == productRowViewModel.Thing.Owner.Iid);
            }
        }

        /// <summary>
        ///     Apply a filtering on columns
        /// </summary>
        /// <param name="selectedFilters">The selected filters</param>
        public override void FilterColumns(Dictionary<ClassKind, List<FilterRow>> selectedFilters)
        {
            var selectedOwners = selectedFilters[ClassKind.DomainOfExpertise];
            var selectedSpecification = selectedFilters[ClassKind.RequirementsSpecification];

            foreach (var requirementRowViewModel in this.TraceabilityTableViewModel.Columns.OfType<RequirementRowViewModel>())
            {
                requirementRowViewModel.IsVisible = selectedOwners.Any(x => x.DefinedThing.Iid == requirementRowViewModel.Thing.Owner.Iid)
                                                    && selectedSpecification.Any(x => x.DefinedThing.Iid == requirementRowViewModel.Thing.Container?.Iid);
            }
        }

        /// <summary>
        ///     Initialize this view model properties
        /// </summary>
        /// <param name="things">A collection of <see cref="BaseViewViewModel.Things" /></param>
        /// <param name="projectId">The <see cref="Project" /> id</param>
        /// <param name="reviewId">The <see cref="Review" /> id</param>
        /// <returns>A <see cref="Task" /></returns>
        public override async Task InitializeProperties(IEnumerable<Thing> things, Guid projectId, Guid reviewId)
        {
            await base.InitializeProperties(things, projectId, reviewId);

            var requirements = this.Things.OfType<RequirementsSpecification>()
                .SelectMany(x => x.Requirement)
                .OrderBy(x => x.ShortName)
                .ToList();

            var products = this.Things.OfType<ElementDefinition>()
                .Where(x => x.IsProduct())
                .OrderBy(x => x.Name)
                .ToList();

            var filteredThings = new List<Thing>(requirements);
            filteredThings.AddRange(products);

            var reviewItems = await this.ReviewItemService.GetReviewItemsForThings(this.ProjectId, this.ReviewId,
                filteredThings.Select(x => x.Iid));

            var rows = new List<IHaveThingRowViewModel>(products
                .Select(x => new ProductRowViewModel(x, reviewItems.FirstOrDefault(ri => ri.ThingId == x.Iid))));

            var columns = new List<IHaveThingRowViewModel>(requirements
                .Select(x => new RequirementRowViewModel(x, reviewItems.FirstOrDefault(ri => ri.ThingId == x.Iid))));

            this.InitializeRowFilters(rows);
            this.InitializeColumnFilters(columns);
            this.TraceabilityTableViewModel.InitializeProperties(rows, columns);
        }

        /// <summary>
        ///     Value indicating that the current view should display Technology
        /// </summary>
        public bool IsOnTechnologyView
        {
            get => this.isOnTechnologyView;
            set => this.RaiseAndSetIfChanged(ref this.isOnTechnologyView, value);
        }

        /// <summary>
        ///     Perfoms the switch between view
        /// </summary>
        public void OnTechnologyViewChange()
        {
            foreach (var row in this.TraceabilityTableViewModel.Rows.OfType<ProductRowViewModel>())
            {
                row.ComputeId(this.IsOnTechnologyView);
            }
        }

        /// <summary>
        ///     Verifies that a <see cref="IHaveThingRowViewModel" /> is valid
        /// </summary>
        /// <param name="row">A <see cref="IHaveThingRowViewModel" /></param>
        /// <returns>The result of the verification</returns>
        protected override bool IsValidRow(IHaveThingRowViewModel row)
        {
            return !this.IsOnTechnologyView || ((ProductRowViewModel)row).HasValidTechnology;
        }

        /// <summary>
        ///     Initializes the <see cref="FilterModel" />s for rows
        /// </summary>
        /// <param name="rows">A collection of <see cref="IHaveThingRowViewModel" /></param>
        private void InitializeRowFilters(IEnumerable<IHaveThingRowViewModel> rows)
        {
            this.AvailableRowFilters.Clear();

            var availableOwners = new List<DefinedThing>(rows.OfType<ProductRowViewModel>()
                .Select(x => x.Thing.Owner).DistinctBy(x => x.Iid));

            this.AvailableRowFilters.Add(new FilterModel
            {
                ClassKind = ClassKind.DomainOfExpertise,
                DisplayName = "Owner",
                Values = availableOwners
            });
        }

        /// <summary>
        ///     Initializes the <see cref="FilterModel" />s for columns
        /// </summary>
        /// <param name="columns">A collection of <see cref="IHaveThingRowViewModel" /></param>
        private void InitializeColumnFilters(List<IHaveThingRowViewModel> columns)
        {
            this.AvailableColumnFilters.Clear();

            var availableOwners = new List<DefinedThing>(columns.OfType<RequirementRowViewModel>()
                .Select(x => x.Thing.Owner).DistinctBy(x => x.Iid));

            this.AvailableColumnFilters.Add(new FilterModel
            {
                ClassKind = ClassKind.DomainOfExpertise,
                DisplayName = "Owner",
                Values = availableOwners
            });

            var availableRequirementsSpecification = new List<DefinedThing>(columns.OfType<RequirementRowViewModel>()
                .Select(x => x.Thing.Container as RequirementsSpecification)
                .Where(x => x != null)
                .DistinctBy(x => x.Iid));

            this.AvailableColumnFilters.Add(new FilterModel
            {
                ClassKind = ClassKind.RequirementsSpecification,
                DisplayName = "Specification",
                Values = availableRequirementsSpecification
            });
        }
    }
}
