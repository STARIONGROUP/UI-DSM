// --------------------------------------------------------------------------------------------------------
// <copyright file="BaseViewViewModel.cs" company="RHEA System S.A.">
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

    using ReactiveUI;

    using UI_DSM.Client.Components.NormalUser.Views;
    using UI_DSM.Client.Services.ReviewItemService;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     View model for the <see cref="GenericBaseView{TViewModel}" /> component
    /// </summary>
    public abstract class BaseViewViewModel : ReactiveObject, IBaseViewViewModel
    {
        /// <summary>
        ///     Backing field for <see cref="SelectedElement" />
        /// </summary>
        private object selectedElement;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseViewViewModel" /> class.
        /// </summary>
        /// <param name="reviewItemService">The <see cref="IReviewItemService" /></param>
        protected BaseViewViewModel(IReviewItemService reviewItemService)
        {
            this.ReviewItemService = reviewItemService;
            this.Things = new List<Thing>();
        }

        /// <summary>
        ///     The <see cref="IReviewItemService" />
        /// </summary>
        protected IReviewItemService ReviewItemService { get; }

        /// <summary>
        ///     The <see cref="Project" /> id
        /// </summary>
        protected Guid ProjectId { get; private set; }

        /// <summary>
        ///     The <see cref="Review" /> id
        /// </summary>
        protected Guid ReviewId { get; private set; }

        /// <summary>
        ///     A collection of columns name that can be visible by default at start
        /// </summary>
        public List<string> AdditionnalColumnsVisibleAtStart { get; protected set; }

        /// <summary>
        ///     A collection of prefilters
        /// </summary>
        protected List<string> Prefilters { get; set; }

        /// <summary>
        ///     A collection of <see cref="Thing" />
        /// </summary>
        public IEnumerable<Thing> Things { get; private set; }

        /// <summary>
        ///     The currently selected element
        /// </summary>
        public object SelectedElement
        {
            get => this.selectedElement;
            set => this.RaiseAndSetIfChanged(ref this.selectedElement, value);
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
        public virtual Task InitializeProperties(IEnumerable<Thing> things, Guid projectId, Guid reviewId, List<string> prefilters, List<string> additionnalColumnsVisibleAtStart)
        {
            this.Things = things;
            this.ProjectId = projectId;
            this.ReviewId = reviewId;
            this.Prefilters = prefilters;
            this.AdditionnalColumnsVisibleAtStart = additionnalColumnsVisibleAtStart;
            return Task.CompletedTask;
        }

        /// <summary>
        ///     Tries to set the <see cref="IBaseViewViewModel.SelectedElement" /> to the previous selected item
        /// </summary>
        /// <param name="selectedItem">The previously selectedItem</param>
        public abstract void TrySetSelectedItem(object selectedItem);
    }
}
