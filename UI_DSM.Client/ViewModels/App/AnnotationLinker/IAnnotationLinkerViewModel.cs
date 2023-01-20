// --------------------------------------------------------------------------------------------------------
// <copyright file="IAnnotationLinkerViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.App.AnnotationLinker
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.Enumerator;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface definition for <see cref="AnnotationLinkerViewModel" />
    /// </summary>
    public interface IAnnotationLinkerViewModel
    {
        /// <summary>
        ///     A collection of <see cref="IHaveAnnotatableItemRowViewModel" />
        /// </summary>
        IEnumerable<IHaveAnnotatableItemRowViewModel> AvailableItems { get; }

        /// <summary>
        ///     A collection of selected <see cref="IHaveAnnotatableItemRowViewModel" />
        /// </summary>
        IEnumerable<IHaveAnnotatableItemRowViewModel> SelectedItems { get; set; }

        /// <summary>
        ///     The current <see cref="UI_DSM.Shared.Models.Annotation" />
        /// </summary>
        Annotation CurrentAnnotation { get; }

        /// <summary>
        ///     <see cref="EventCallback" /> for submitting the link
        /// </summary>
        EventCallback OnSubmit { get; set; }

        /// <summary>
        ///     The <see cref="CreationStatus" />
        /// </summary>
        CreationStatus CreationStatus { get; set; }

        /// <summary>
        ///     Initializes this view model properties
        /// </summary>
        /// <param name="rows">All <see cref="IHaveAnnotatableItemRowViewModel" /> rows</param>
        /// <param name="annotation">The <see cref="Annotation" /></param>
        void InitializesViewModel(List<IHaveAnnotatableItemRowViewModel> rows, Annotation annotation);
    }
}
