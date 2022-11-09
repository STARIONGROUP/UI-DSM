// --------------------------------------------------------------------------------------------------------
// <copyright file="RequirementTraceabilityToRequirementViewViewModel.cs" company="RHEA System S.A.">
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

    using UI_DSM.Client.Model;
    using UI_DSM.Client.Services.ReviewItemService;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     ViewModel for the <see cref="Client.Components.NormalUser.Views.RequirementTraceabilityToRequirementView" />
    ///     component
    /// </summary>
    public class RequirementTraceabilityToRequirementViewViewModel : HaveTraceabilityTableViewModel, IRequirementTraceabilityToRequirementViewViewModel
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RequirementTraceabilityToRequirementViewViewModel" /> class.
        /// </summary>
        /// <param name="reviewItemService">The <see cref="IReviewItemService" /></param>
        public RequirementTraceabilityToRequirementViewViewModel(IReviewItemService reviewItemService) : base(reviewItemService)
        {
        }

        /// <summary>
        ///     The header name of the column
        /// </summary>
        protected override string HeaderName => "req/req";

        /// <summary>
        ///     The name of the category
        /// </summary>
        protected override string TraceCategoryName => "trace";

        /// <summary>
        ///     Apply a filtering on rows
        /// </summary>
        /// <param name="selectedFilters">The selected filters</param>
        public override void FilterRows(Dictionary<ClassKind, List<FilterRow>> selectedFilters)
        {
            FilterRequirements(selectedFilters, this.TraceabilityTableViewModel.Rows);
        }

        /// <summary>
        ///     Apply a filtering on columns
        /// </summary>
        /// <param name="selectedFilters">The selected filters</param>
        public override void FilterColumns(Dictionary<ClassKind, List<FilterRow>> selectedFilters)
        {
            FilterRequirements(selectedFilters, this.TraceabilityTableViewModel.Columns);
        }

        /// <summary>
        ///     Initialize this view model properties
        /// </summary>
        /// <param name="things">A collection of <see cref="Thing" /></param>
        /// <param name="projectId">The <see cref="Project" /> id</param>
        /// <param name="reviewId">The <see cref="Review" /> id</param>
        /// <returns>A <see cref="Task" /></returns>
        public override async Task InitializeProperties(IEnumerable<Thing> things, Guid projectId, Guid reviewId)
        {
            await base.InitializeProperties(things, projectId, reviewId);

            var filteredThings = this.Things.OfType<RequirementsSpecification>()
                .SelectMany(x => x.Requirement)
                .OrderBy(x => x.ShortName)
                .ToList();

            var reviewItems = await this.ReviewItemService.GetReviewItemsForThings(this.ProjectId, this.ReviewId,
                filteredThings.Select(x => x.Iid));

            var rows = new List<IHaveThingRowViewModel>(filteredThings
                .Select(x => new RequirementRowViewModel(x, reviewItems.FirstOrDefault(ri => ri.ThingId == x.Iid))));

            var columns = new List<IHaveThingRowViewModel>(filteredThings
                .Select(x => new RequirementRowViewModel(x, reviewItems.FirstOrDefault(ri => ri.ThingId == x.Iid))));

            this.InitializeFilters(rows);
            this.TraceabilityTableViewModel.InitializeProperties(rows, columns);
        }

        /// <summary>
        ///     Verifies that a <see cref="IHaveThingRowViewModel" /> is valid
        /// </summary>
        /// <param name="row">A <see cref="IHaveThingRowViewModel" /></param>
        /// <returns>The result of the verification</returns>
        protected override bool IsValidRow(IHaveThingRowViewModel row)
        {
            return true;
        }

        /// <summary>
        ///     Filters a collection of <see cref="IHaveThingRowViewModel" /> for <see cref="Requirement" />
        /// </summary>
        /// <param name="selectedFilters">The selected filters</param>
        /// <param name="collectionToFilter">The collection</param>
        private static void FilterRequirements(IReadOnlyDictionary<ClassKind, List<FilterRow>> selectedFilters, IEnumerable<IHaveThingRowViewModel> collectionToFilter)
        {
            var selectedOwners = selectedFilters[ClassKind.DomainOfExpertise];
            var selectedSpecification = selectedFilters[ClassKind.RequirementsSpecification];

            foreach (var requirementRowViewModel in collectionToFilter.OfType<RequirementRowViewModel>())
            {
                requirementRowViewModel.IsVisible = selectedOwners.Any(x => x.DefinedThing.Iid == requirementRowViewModel.Thing.Owner.Iid)
                                                    && selectedSpecification.Any(x => x.DefinedThing.Iid == requirementRowViewModel.Thing.Container?.Iid);
            }
        }

        /// <summary>
        ///     Initializes the <see cref="FilterModel" />
        /// </summary>
        /// <param name="rows">A collection of <see cref="IHaveThingRowViewModel" /></param>
        private void InitializeFilters(List<IHaveThingRowViewModel> rows)
        {
            this.AvailableRowFilters.Clear();
            this.AvailableColumnFilters.Clear();

            var availableOwners = new List<DefinedThing>(rows.OfType<RequirementRowViewModel>()
                .Select(x => x.Thing.Owner).DistinctBy(x => x.Iid));

            this.AvailableRowFilters.Add(new FilterModel
            {
                ClassKind = ClassKind.DomainOfExpertise,
                DisplayName = "Owner",
                Values = availableOwners
            });

            this.AvailableColumnFilters.Add(new FilterModel
            {
                ClassKind = ClassKind.DomainOfExpertise,
                DisplayName = "Owner",
                Values = availableOwners
            });

            var availableRequirementsSpecification = new List<DefinedThing>(rows.OfType<RequirementRowViewModel>()
                .Select(x => x.Thing.Container as RequirementsSpecification)
                .Where(x => x != null)
                .DistinctBy(x => x.Iid));

            this.AvailableRowFilters.Add(new FilterModel
            {
                ClassKind = ClassKind.RequirementsSpecification,
                DisplayName = "Specification",
                Values = availableRequirementsSpecification
            });

            this.AvailableColumnFilters.Add(new FilterModel
            {
                ClassKind = ClassKind.RequirementsSpecification,
                DisplayName = "Specification",
                Values = availableRequirementsSpecification
            });
        }
    }
}
