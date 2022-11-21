// --------------------------------------------------------------------------------------------------------
// <copyright file="RequirementBreakdownStructureViewViewModel.cs" company="RHEA System S.A.">
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
    using CDP4Common.SiteDirectoryData;

    using UI_DSM.Client.Components.NormalUser.Views;
    using UI_DSM.Client.Model;
    using UI_DSM.Client.Services.ReviewItemService;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     View model for the <see cref="RequirementBreakdownStructureView" /> component
    /// </summary>
    public class RequirementBreakdownStructureViewViewModel : BaseViewViewModel, IRequirementBreakdownStructureViewViewModel
    {
        /// <summary>
        ///     The collection of all <see cref="RequirementRowViewModel" />
        /// </summary>
        private List<RequirementRowViewModel> allRows;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RequirementBreakdownStructureViewViewModel" /> class.
        /// </summary>
        /// <param name="reviewItemService">The <see cref="IReviewItemService" /></param>
        public RequirementBreakdownStructureViewViewModel(IReviewItemService reviewItemService) : base(reviewItemService)
        {
            this.Rows = new List<RequirementRowViewModel>();
        }

        /// <summary>
        ///     A collection of available <see cref="FilterModel" /> for rows
        /// </summary>
        public List<FilterModel> AvailableRowFilters { get; } = new();

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

            var filteredThings = this.Things.OfType<RequirementsSpecification>()
                .SelectMany(x => x.Requirement)
                .OrderBy(x => x.ShortName)
                .ToList();

            var reviewItems = await this.ReviewItemService.GetReviewItemsForThings(this.ProjectId, this.ReviewId,
                filteredThings.Select(x => x.Iid));

            this.allRows = new List<RequirementRowViewModel>(filteredThings
                .Select(x => new RequirementRowViewModel(x, reviewItems.FirstOrDefault(ri => ri.ThingId == x.Iid))));

            this.Rows = new List<RequirementRowViewModel>(this.allRows);
            this.InitializesFilter();
        }

        /// <summary>
        ///     Initializes the <see cref="AvailableRowFilters"/> for <see cref="RequirementRowViewModel" /> rows
        /// </summary>
        private void InitializesFilter()
        {
            this.AvailableRowFilters.Clear();

            var availableRequirementsSpecification = new List<DefinedThing>(this.allRows
                .Select(x => x.Thing.Container as RequirementsSpecification)
                .Where(x => x != null)
                .DistinctBy(x => x.Iid));

            this.AvailableRowFilters.Add(new FilterModel
            {
                ClassKind = ClassKind.RequirementsSpecification,
                DisplayName = "Specification",
                Values = availableRequirementsSpecification
            });

            var requirementTypes = this.allRows.Select(x => x.RequirementType)
                .Distinct();

            var availableRequirementTypes = new List<DefinedThing>(requirementTypes.Select(x => new TextParameterType()
            {
                Name = x
            }));

            this.AvailableRowFilters.Add(new FilterModel()
            {
                ClassKind = ClassKind.TextParameterType,
                DisplayName = "Requirement Type",
                Values = availableRequirementTypes
            });
        }

        /// <summary>
        ///     Filters current rows
        /// </summary>
        /// <param name="selectedFilters">The selected filters</param>
        public void FilterRequirementRows(IReadOnlyDictionary<ClassKind, List<FilterRow>> selectedFilters)
        {
            var selectedSpecification = selectedFilters[ClassKind.RequirementsSpecification];
            var selectedRequirementType = selectedFilters[ClassKind.TextParameterType];

            this.Rows = this.allRows.Where(x => selectedSpecification.Any(rs => rs.DefinedThing.Iid == x.Thing.Container?.Iid)
                                                && selectedRequirementType.Any(rt => rt.DefinedThing.Name == x.RequirementType));
        }

        /// <summary>
        ///     A collection of <see cref="Requirement" /> that has been filtered
        /// </summary>
        public IEnumerable<RequirementRowViewModel> Rows { get; private set; }
    }
}
