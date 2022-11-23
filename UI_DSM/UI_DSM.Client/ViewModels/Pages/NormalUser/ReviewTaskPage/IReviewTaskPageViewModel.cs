// --------------------------------------------------------------------------------------------------------
// <copyright file="IReviewTaskPageViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Pages.NormalUser.ReviewTaskPage
{
    using CDP4Common.CommonData;

    using UI_DSM.Client.Components.NormalUser.Views;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.Wrappers;

    /// <summary>
    ///     Interface definition for <see cref="ReviewTaskPageViewModel" />
    /// </summary>
    public interface IReviewTaskPageViewModel
    {
        /// <summary>
        ///     The current <see cref="ReviewObjective" />
        /// </summary>
        ReviewObjective ReviewObjective { get; set; }

        /// <summary>
        ///     A collection of <see cref="Thing" /> that are suitable for the current <see cref="ReviewTask" />
        /// </summary>
        IEnumerable<Thing> Things { get; set; }

        /// <summary>
        ///     The current <see cref="Type" /> for the <see cref="GenericBaseView{TViewModel}" />
        /// </summary>
        Type CurrentBaseView { get; set; }

        /// <summary>
        ///     The <see cref="IReviewTaskPageViewModel.ReviewTask" />
        /// </summary>
        ReviewTask ReviewTask { get; set; }

        /// <summary>
        ///     The current <see cref="View" />
        /// </summary>
        View CurrentView { get; }

        /// <summary>
        ///     A collection of all available <see cref="ViewWrapper" />
        /// </summary>
        List<ViewWrapper> AvailableViews { get; }

        /// <summary>
        ///     Value indicating if the view selector is visible or not
        /// </summary>
        bool ViewSelectorVisible { get; set; }

        /// <summary>
        ///     The currently selected <see cref="ViewWrapper" />
        /// </summary>
        ViewWrapper SelectedView { get; set; }

        /// <summary>
        ///     Value indicating if the baseView needs to be initialized
        /// </summary>
        bool ShouldInitializeBaseView { get; set; }

        /// <summary>
        ///     The current instance of <see cref="BaseView" />
        /// </summary>
        BaseView CurrentBaseViewInstance { get; set; }

        /// <summary>
        ///     The current <see cref="Participant" />
        /// </summary>
        Participant Participant { get; set; }

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        ///     Override this method if you will perform an asynchronous operation and
        ///     want the component to refresh when that operation is completed.
        /// </summary>
        /// <param name="projectId">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <param name="reviewId">The <see cref="Guid" /> of the <see cref="Review" /></param>
        /// <param name="reviewObjectiveId">
        ///     The <see cref="Guid" /> of the <see cref="ReviewTaskPageViewModel.ReviewObjective" />
        /// </param>
        /// <param name="reviewTaskId">The <see cref="Guid" /> of the <see cref="IReviewTaskPageViewModel.ReviewTask" /></param>
        /// <returns>A <see cref="Task" /></returns>
        Task OnInitializedAsync(Guid projectId, Guid reviewId, Guid reviewObjectiveId, Guid reviewTaskId);

        /// <summary>
        ///     Updates the current view
        /// </summary>
        /// <param name="newView">The new <see cref="View" /></param>
        /// <param name="shouldUpdateAvailableViews">Indicate if the <see cref="AvailableViews" /> should also be updated</param>
        void UpdateView(View newView, bool shouldUpdateAvailableViews = false);

        /// <summary>
        ///     Gets the main related <see cref="View" />
        /// </summary>
        /// <returns>The main related <see cref="View" /></returns>
        View GetMainRelatedView();

        /// <summary>
        ///     Gets a collection of related <see cref="View" />s
        /// </summary>
        /// <returns>The collection of related <see cref="View" />s</returns>
        List<View> GetOtherRelatedViews();

        /// <summary>
        ///     Reset this view model
        /// </summary>
        void Reset();
    }
}
