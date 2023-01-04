// --------------------------------------------------------------------------------------------------------
// <copyright file="IModelPageViewModel.cs" company="RHEA System S.A.">
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
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface definition for <see cref="ModelPageViewModel" />
    /// </summary>
    public interface IModelPageViewModel
    {
        /// <summary>
        ///     The current instance of <see cref="BaseView" />
        /// </summary>
        BaseView CurrentBaseViewInstance { get; set; }

        /// <summary>
        ///     A collection of <see cref="Thing" />
        /// </summary>
        IEnumerable<Thing> Things { get; }

        /// <summary>
        ///     The current <see cref="Type" /> for the <see cref="GenericBaseView{TViewModel}" />
        /// </summary>
        Type CurrentBaseView { get; }

        /// <summary>
        ///     Value indicating if the baseView needs to be initialized
        /// </summary>
        bool ShouldInitializeBaseView { get; set; }

        /// <summary>
        ///     The current <see cref="View" />
        /// </summary>
        View CurrentView { get; }

        /// <summary>
        ///     Initializes this view model properties
        /// </summary>
        /// <param name="projectId">The <see cref="Project" /> id</param>
        /// <param name="modelId">The <see cref="Model" /> id</param>
        /// <param name="selectedView">The selected view</param>
        /// <returns>A <see cref="Task" /></returns>
        Task InitializeProperties(Guid projectId, Guid modelId, View selectedView);

        /// <summary>
        ///     Get all accessible views
        /// </summary>
        /// <returns>A collection of <see cref="View" /></returns>
        List<View> GetAccessibleViews();
    }
}
