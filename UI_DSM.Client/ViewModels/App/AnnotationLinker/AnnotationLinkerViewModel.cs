// --------------------------------------------------------------------------------------------------------
// <copyright file="AnnotationLinkerViewModel.cs" company="RHEA System S.A.">
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
    ///     View Model for the <see cref="Client.Components.App.AnnotationLinker.AnnotationLinker" />
    /// </summary>
    public class AnnotationLinkerViewModel : IAnnotationLinkerViewModel
    {
        /// <summary>
        ///     A collection of <see cref="IHaveThingRowViewModel" />
        /// </summary>
        public IEnumerable<IHaveAnnotatableItemRowViewModel> AvailableItems { get; private set; } = new List<IHaveAnnotatableItemRowViewModel>();

        /// <summary>
        ///     A collection of selected <see cref="IHaveThingRowViewModel" />
        /// </summary>
        public IEnumerable<IHaveAnnotatableItemRowViewModel> SelectedItems { get; set; }

        /// <summary>
        ///     The current <see cref="Annotation" />
        /// </summary>
        public Annotation CurrentAnnotation { get; private set; }

        /// <summary>
        ///     <see cref="EventCallback" /> for submitting the link
        /// </summary>
        public EventCallback OnSubmit { get; set; }

        /// <summary>
        ///     The <see cref="CreationStatus" />
        /// </summary>
        public CreationStatus CreationStatus { get; set; }

        /// <summary>
        ///     Initializes this view model properties
        /// </summary>
        /// <param name="rows">All <see cref="IHaveAnnotatableItemRowViewModel" /> rows</param>
        /// <param name="annotation">The <see cref="Annotation" /></param>
        public void InitializesViewModel(List<IHaveAnnotatableItemRowViewModel> rows, Annotation annotation)
        {
            this.CreationStatus = CreationStatus.None;
            this.SelectedItems = new List<IHaveAnnotatableItemRowViewModel>();

            var availablesItems = new List<IHaveAnnotatableItemRowViewModel>();

            availablesItems.AddRange(rows.OfType<IHaveThingRowViewModel>().Where(x => x.ReviewItem == null || x.ReviewItem.Annotations.All(a => a.Id != annotation.Id))
                .DistinctBy(x => x.ThingId)
                .OrderBy(x => x.Id)
            );

            this.AvailableItems = availablesItems;

            this.CurrentAnnotation = annotation;
        }
    }
}
