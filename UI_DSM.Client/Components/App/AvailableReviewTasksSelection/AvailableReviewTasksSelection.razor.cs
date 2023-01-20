// --------------------------------------------------------------------------------------------------------
// <copyright file="AvailableReviewTasksSelection.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.App.AvailableReviewTasksSelection
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.ViewModels.App.AvailableReviewTasksSelection;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Component that allow the selection of a <see cref="ReviewTask" />
    /// </summary>
    public partial class AvailableReviewTasksSelection
    {
        /// <summary>
        ///     A <see cref="Dictionary{TKey,TValue}" /> for the availables <see cref="ReviewTask" /> grouped by
        ///     <see cref="ReviewObjective" />
        /// </summary>
        private Dictionary<ReviewObjective, IEnumerable<ReviewTask>> availablesReviewTasks = new();

        /// <summary>
        ///     A <see cref="Dictionary{TKey,TValue}" /> to track the opened panels
        /// </summary>
        private Dictionary<Guid, bool> openedPanels = new();

        /// <summary>
        ///     The <see cref="IAvailableReviewTasksSelectionViewModel" />
        /// </summary>
        [Parameter]
        public IAvailableReviewTasksSelectionViewModel ViewModel { get; set; }

        /// <summary>
        ///     Method invoked when the component has received parameters from its parent in
        ///     the render tree, and the incoming values have been assigned to properties.
        /// </summary>
        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            foreach (var reviewObjective in this.ViewModel.ReviewTasks.Select(x => x.EntityContainer)
                         .OfType<ReviewObjective>().DistinctBy(x => x.Id))
            {
                this.availablesReviewTasks[reviewObjective] = this.ViewModel.ReviewTasks.Where(x => x.EntityContainer.Id == reviewObjective.Id);
                this.openedPanels[reviewObjective.Id] = false;
            }
        }
    }
}
