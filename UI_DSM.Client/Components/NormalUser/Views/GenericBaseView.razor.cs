// --------------------------------------------------------------------------------------------------------
// <copyright file="GenericBaseView.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.NormalUser.Views
{
    using CDP4Common.CommonData;

    using Microsoft.AspNetCore.Components;

    using ReactiveUI;

    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Base component to display a <see cref="View" /> to view <see cref="Thing" /> for reviewing the model
    /// </summary>
    public abstract partial class GenericBaseView<TViewModel>
    {
        /// <summary>
        ///     Gets a collection of all availables <see cref="IHaveAnnotatableItemRowViewModel" />
        /// </summary>
        /// <returns>The collection of <see cref="IHaveThingRowViewModel" /></returns>
        public override List<IHaveAnnotatableItemRowViewModel> GetAvailablesRows()
        {
            return this.ViewModel.GetAvailablesRows();
        }

        /// <summary>
        ///     Updates all <see cref="IHaveAnnotatableItemRowViewModel" /> rows
        /// </summary>
        /// <param name="annotatableItems">A collection of <see cref="AnnotatableItem"/></param>
        /// <returns>A <see cref="Task"/></returns>
        public override Task UpdateAnnotatableRows(List<AnnotatableItem> annotatableItems)
        {
            this.ViewModel.UpdateAnnotatableRows(annotatableItems);
            return this.HasChanged();
        }

        /// <summary>
        ///     The <see cref="TViewModel" />
        /// </summary>
        [Inject]
        public TViewModel ViewModel { get; set; }

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        /// </summary>
        protected override void OnInitialized()
        {
            this.SelectedItemObservable = this.WhenAnyValue(x => x.ViewModel.SelectedElement);
            base.OnInitialized();
        }

        /// <summary>
        ///     Initialize the correspondant ViewModel for this component
        /// </summary>
        /// <param name="things">The collection of <see cref="Thing" /></param>
        /// <param name="projectId">The <see cref="Project" /> id</param>
        /// <param name="reviewId">The <see cref="Review" /> id</param>
        /// <param name="reviewTaskId">The <see cref="ReviewTask" /> id</param>
        /// <param name="prefilters">A collection of prefilters</param>
        /// <param name="additionnalColumnsVisibleAtStart">A collection of columns name that can be visible by default at start</param>
        /// <param name="participant">The current <see cref="Participant"/></param>
        /// /// <returns>A <see cref="Task" /></returns>
        public override async Task InitializeViewModel(IEnumerable<Thing> things, Guid projectId, Guid reviewId, Guid reviewTaskId, List<string> prefilters, List<string> additionnalColumnsVisibleAtStart, Participant participant)
        {
            await this.ViewModel.InitializeProperties(things, projectId, reviewId, reviewTaskId, prefilters, additionnalColumnsVisibleAtStart, participant);
            await this.HasChanged();
        }

        /// <summary>
        ///     Tries to set the selected item to the ViewModel
        /// </summary>
        /// <param name="selectedItem">The previous selected <see cref="object" /></param>
        public override void TrySetSelectedItem(object selectedItem)
        {
	        if (selectedItem == null)
	        {
		        this.ViewModel.SelectedElement = null;
	        }
	        else
			{
				this.ViewModel.TrySetSelectedItem(selectedItem);
			}
		}
    }
}
