// --------------------------------------------------------------------------------------------------------
// <copyright file="BudgetViewViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Components.NormalUser.Views
{
    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;

    using DevExpress.Blazor.Internal;

    using DynamicData.Binding;

    using ReactiveUI;

    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Client.Services.ReportService;
    using UI_DSM.Client.Services.ReviewItemService;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     View model for the <see cref="Client.Components.NormalUser.Views.BudgetView" /> component
    /// </summary>
    public class BudgetViewViewModel : BaseViewViewModel, IBudgetViewViewModel
    {
        /// <summary>
        /// The INJECTED <see cref="IJsonService"/>
        /// </summary>
        private readonly IJsonService ijsonService;

        /// <summary>
        /// The INJECTED <see cref="IReportService"/>
        /// </summary>
        private readonly IReportService reportService;

        /// <summary>
        /// Backing field for <see cref="Iteration"/>
        /// </summary>
        private Iteration iteration;

        /// <summary>
        /// Backing field for <see cref="ReportName"/>
        /// </summary>
        private string reportName;

        /// <summary>
        /// Gets or sets the available Report Names
        /// </summary>
        public IEnumerable<string> ReportNames { get; set; }

        /// <summary>
        /// Gets or sets the Report ID
        /// </summary>
        public string ReportDtoAsString { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseViewViewModel" /> class.
        /// </summary>
        /// <param name="reviewItemService">The <see cref="IReviewItemService" /></param>
        /// <param name="IjsonService">The <see cref="IJsonService" /></param>
        /// <param name="reportService">The <see cref="IReportService" /></param>
        public BudgetViewViewModel(IReviewItemService reviewItemService, IJsonService IjsonService, IReportService reportService) : base(reviewItemService)
        {
            this.ijsonService = IjsonService;
            this.reportService = reportService;

            this.WhenAnyPropertyChanged(nameof(this.Iteration), nameof(this.ReportName))
                .Subscribe(x => this.SetReportDtoAsString());
        }

        /// <summary>
        /// Gets or sets the <see cref="Iteration"/>
        /// </summary>
        public Iteration Iteration 
        {             
            get => this.iteration;
            set => this.RaiseAndSetIfChanged(ref this.iteration, value);
        }

        /// <summary>
        /// Gets or sets the report name
        /// </summary>
        public string ReportName 
        {             
            get => this.reportName;
            set => this.RaiseAndSetIfChanged(ref this.reportName, value);
        }

        /// <summary>
        ///     A collection of <see cref="HyperLinkRowViewModel" />
        /// </summary>
        public IEnumerable<HyperLinkRowViewModel> Rows { get; private set; } = new List<HyperLinkRowViewModel>();
        
        /// <summary>
        ///     Initialize this view model properties
        /// </summary>
        /// <param name="things">A collection of <see cref="Thing" /></param>
        /// <param name="projectId">The <see cref="Project" /> id</param>
        /// <param name="reviewId">The <see cref="Review" /> id</param>
        /// <param name="reviewTaskId">The <see cref="ReviewTask" /> id</param>
        /// <param name="prefilters">A collection of prefilters</param>
        /// <param name="additionnalColumnsVisibleAtStart">A collection of columns name that can be visible by default at start</param>
        /// <param name="participant"></param>
        /// <returns>A <see cref="Task" /></returns>
        public override async Task InitializeProperties(IEnumerable<Thing> things, Guid projectId, Guid reviewId, Guid reviewTaskId, List<string> prefilters, List<string> additionnalColumnsVisibleAtStart, Participant participant)
        {
            await base.InitializeProperties(things, projectId, reviewId, reviewTaskId, prefilters, additionnalColumnsVisibleAtStart, participant);

            await this.InitializeAvailableReports();

            this.Iteration = this.Things.OfType<Iteration>().FirstOrDefault();
        }

        /// <summary>
        /// Initializes the avaliable Report Names
        /// </summary>
        private async Task InitializeAvailableReports()
        {
            var reportNames = await this.reportService.GetAvailableReports(this.ProjectId);
            this.ReportNames = reportNames;

            if (!this.ReportNames.Contains(this.ReportName))
            {
                this.ReportName = "";
            }
        }
        
        /// <summary>
        /// Creates a <see cref="ReportDto"/> and writes its string representation to the ReportDtoAsString property.
        /// The UI Document Viewer will react to this by loading the corresponding report 
        /// </summary>
        private void SetReportDtoAsString()
        {
            var reportDto = new ReportDto();
            reportDto.Name = this.ReportName;
            reportDto.IterationId = this.Iteration?.Iid ?? Guid.Empty;
            reportDto.ProjectId = this.ProjectId;

            this.ReportDtoAsString = this.ijsonService.Serialize(reportDto);
        }

        /// <summary>
        ///     Tries to set the <see cref="IBaseViewViewModel.SelectedElement" /> to the previous selected item
        /// </summary>
        /// <param name="selectedItem">The previously selectedItem</param>
        public override void TrySetSelectedItem(object selectedItem)
        {
            if (selectedItem is string selectedItemString && Guid.TryParse(selectedItemString, out var selectedItemGuid))
            {
                if (this.Things.FirstOrDefault(x => x.Iid == selectedItemGuid) is ElementBase thing)
                {
                    var rowViewModel = new ElementBaseRowViewModel(thing, null);
                    this.SelectedElement = rowViewModel;
                }
            }
        }

        /// <summary>
        ///     Gets a collection of all availables <see cref="IHaveAnnotatableItemRowViewModel" />
        /// </summary>
        /// <returns>The collection of <see cref="IHaveAnnotatableItemRowViewModel" /></returns>
        public override List<IHaveAnnotatableItemRowViewModel> GetAvailablesRows()
        {
            return new List<IHaveAnnotatableItemRowViewModel>(this.Rows);
        }

        /// <summary>
        ///     Updates all <see cref="IHaveAnnotatableItemRowViewModel" />
        /// </summary>
        /// <param name="annotatableItems">A collection of <see cref="AnnotatableItem" /></param>
        public override void UpdateAnnotatableRows(List<AnnotatableItem> annotatableItems)
        {
            var reviewItems = annotatableItems.OfType<ReviewItem>();
            this.Rows.ForEach(x => x.UpdateReviewItem(reviewItems.FirstOrDefault(ri => ri.ThingId == x.ThingId)));
        }
    }
}
