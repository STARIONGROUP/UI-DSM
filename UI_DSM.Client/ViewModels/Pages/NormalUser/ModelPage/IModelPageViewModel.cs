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

    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.Components.App.AnnotationLinker;
    using UI_DSM.Client.Components.NormalUser.Views;
    using UI_DSM.Client.ViewModels.App.AnnotationLinker;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface definition for <see cref="ModelPageViewModel" />
    /// </summary>
    public interface IModelPageViewModel
    {
        /// <summary>
        ///     Value indicating if the <see cref="AnnotationLinker" /> is visible
        /// </summary>
        bool IsLinkerVisible { get; set; }

        /// <summary>
        ///     The <see cref="IAnnotationLinkerViewModel" />
        /// </summary>
        IAnnotationLinkerViewModel AnnotationLinkerViewModel { get; }

        /// <summary>
        ///     The current <see cref="Participant" />
        /// </summary>
        Participant Participant { get; set; }

        /// <summary>
        ///     The <see cref="EventCallback{TValue}" /> for linking a <see cref="Comment" /> on other element
        /// </summary>
        EventCallback<Comment> OnLinkCallback { get; }

        /// <summary>
        ///     The current <see cref="Type" /> for the <see cref="GenericBaseView{TViewModel}" />
        /// </summary>
        Type CurrentBaseView { get; }

        /// <summary>
        ///     Value indicating if the baseView needs to be initialized
        /// </summary>
        bool ShouldInitializeBaseView { get; set; }

        /// <summary>
        ///     The current instance of <see cref="BaseView" />
        /// </summary>
        BaseView CurrentBaseViewInstance { get; set; }

        /// <summary>
        ///     A collection of <see cref="Thing" />
        /// </summary>
        IEnumerable<Thing> Things { get; }

        /// <summary>
        ///     The current <see cref="View" />
        /// </summary>
        View CurrentView { get; }

        /// <summary>
        ///     Initializes this view model properties
        /// </summary>
        /// <param name="projectGuid"></param>
        /// <param name="reviewGuid"></param>
        /// <param name="modelGuid"></param>
        /// <param name="selectedView">The selected view</param>
        /// <returns>A <see cref="Task" /></returns>
        Task InitializeProperties(Guid projectGuid, Guid reviewGuid, Guid modelGuid, View selectedView);

        /// <summary>
        ///     Get all accessible views
        /// </summary>
        /// <returns>A collection of <see cref="View" /></returns>
        List<View> GetAccessibleViews();
    }
}
