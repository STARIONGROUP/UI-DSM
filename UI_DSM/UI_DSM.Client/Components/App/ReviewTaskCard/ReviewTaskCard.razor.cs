// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewTaskCard.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.App.ReviewTaskCard
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.Pages.NormalUser.ReviewObjectivePage;
    using UI_DSM.Client.ViewModels.Pages.NormalUser.ReviewObjectivePage;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Component to display <see cref="ReviewTask" /> into the <see cref="ReviewObjectivePage" />
    /// </summary>
    public partial class ReviewTaskCard
    {
        /// <summary>
        ///     The <see cref="NavigationManager" />
        /// </summary>
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        /// <summary>
        ///     The <see cref="ReviewTask" /> to display information
        /// </summary>
        [Parameter]
        public ReviewTask ReviewTask { get; set; }

        /// <summary>
        ///     The id of the current <see cref="Project" />
        /// </summary>
        [Parameter]
        public string ProjectId { get; set; }

        /// <summary>
        ///     The id of the current <see cref="Review" />
        /// </summary>
        [Parameter]
        public string ReviewId { get; set; }

        /// <summary>
        ///     The id of the current <see cref="ReviewObjective" />
        /// </summary>
        [Parameter]
        public string ReviewObjectiveId { get; set; }

        /// <summary>
        ///     Value indicating if the NavLink component should be present
        /// </summary>
        [Parameter]
        public bool UseNavLink { get; set; } = true;

        /// <summary>
        ///     The <see cref="IReviewObjectivePageViewModel" /> for the component
        /// </summary>
        [Parameter]
        public IReviewObjectivePageViewModel ViewModel { get; set; }

        /// <summary>
        ///     Navigate to the correct uri
        /// </summary>
        private void Navigate()
        {
            if (this.UseNavLink)
            {
                this.NavigationManager.NavigateTo($"Project/{this.ProjectId}/Review/{this.ReviewId}/ReviewObjective/{this.ReviewObjectiveId}" +
                                                  $"/ReviewTask/{this.ReviewTask.Id}");
            }
        }
        protected override void OnInitialized()
        {
            if (this.ViewModel != null)
            {
                this.ViewModel.ProjectId = new Guid(this.ProjectId);
                this.ViewModel.ReviewId = new Guid(this.ReviewId);
            } 
        }
    }
}
