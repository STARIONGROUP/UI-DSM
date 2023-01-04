// --------------------------------------------------------------------------------------------------------
// <copyright file="ModelPageViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Pages.NormalUser.ModelPage
{
    using CDP4Common.CommonData;

    using UI_DSM.Client.Components.NormalUser.Views;
    using UI_DSM.Client.Services.ArtifactService;
    using UI_DSM.Client.Services.ThingService;
    using UI_DSM.Client.Services.ViewProviderService;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     View model for the <see cref="Client.Pages.NormalUser.ModelPage.ModelPage" /> page
    /// </summary>
    public class ModelPageViewModel : IModelPageViewModel
    {
        /// <summary>
        ///     The <see cref="IArtifactService" />
        /// </summary>
        private readonly IArtifactService artifactService;

        /// <summary>
        ///     The <see cref="IThingService" />
        /// </summary>
        private readonly IThingService thingService;

        /// <summary>
        ///     The <see cref="IViewProviderService" />
        /// </summary>
        private readonly IViewProviderService viewProviderService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ModelPageViewModel" /> class.
        /// </summary>
        /// <param name="thingService">The <see cref="IThingService" /></param>
        /// <param name="viewProviderService">The <see cref="IViewProviderService" /></param>
        /// <param name="artifactService">The <see cref="IArtifactService" /></param>
        public ModelPageViewModel(IThingService thingService, IViewProviderService viewProviderService, IArtifactService artifactService)
        {
            this.thingService = thingService;
            this.viewProviderService = viewProviderService;
            this.artifactService = artifactService;
        }

        /// <summary>
        ///     The current <see cref="Type" /> for the <see cref="GenericBaseView{TViewModel}" />
        /// </summary>
        public Type CurrentBaseView { get; private set; }

        /// <summary>
        ///     Value indicating if the baseView needs to be initialized
        /// </summary>
        public bool ShouldInitializeBaseView { get; set; }

        /// <summary>
        ///     The current instance of <see cref="BaseView" />
        /// </summary>
        public BaseView CurrentBaseViewInstance { get; set; }

        /// <summary>
        ///     A collection of <see cref="Thing" />
        /// </summary>
        public IEnumerable<Thing> Things { get; private set; }

        /// <summary>
        ///     The current <see cref="View" />
        /// </summary>
        public View CurrentView { get; private set; }

        /// <summary>
        ///     Initializes this view model properties
        /// </summary>
        /// <param name="projectId">The <see cref="Project" /> id</param>
        /// <param name="modelId">The <see cref="Model" /> id</param>
        /// <param name="selectedView">The selected view</param>
        /// <returns>A <see cref="Task" /></returns>
        public async Task InitializeProperties(Guid projectId, Guid modelId, View selectedView)
        {
            var model = (await this.artifactService.GetArtifactsOfProject(projectId)).OfType<Model>().FirstOrDefault(x => x.Id == modelId);
            this.CurrentBaseViewInstance = null;

            if (model != null)
            {
                this.CurrentView = selectedView;
                this.Things = await this.thingService.GetThings(projectId, model);
                this.CurrentBaseView = this.viewProviderService.GetViewType(selectedView);
                this.ShouldInitializeBaseView = true;
            }
        }

        /// <summary>
        ///     Get all accessible views
        /// </summary>
        /// <returns>A collection of <see cref="View" /></returns>
        public List<View> GetAccessibleViews()
        {
            var allViews = new List<View>
            {
                View.RequirementBreakdownStructureView,
                View.RequirementVerificationControlView,
                View.ProductBreakdownStructureView,
                View.TrlView,
                View.FunctionalBreakdownStructureView,
                View.RequirementTraceabilityToRequirementView,
                View.RequirementTraceabilityToProductView,
                View.RequirementTraceabilityToFunctionView,
                View.FunctionalTraceabilityToProductView,
                View.InterfaceView,
                View.DocumentBased
            };

            allViews.Remove(this.CurrentView);
            return allViews;
        }
    }
}
