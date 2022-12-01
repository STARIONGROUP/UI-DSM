// --------------------------------------------------------------------------------------------------------
// <copyright file="BaseView.cs" company="RHEA System S.A.">
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

    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     The top component for any Component that will render a specific <see cref="View" />
    /// </summary>
    public abstract partial class BaseView
    {
        /// <summary>
        /// Backing field for the <see cref="IsLoading"/> property
        /// </summary>
        private bool isLoading;

        /// <summary>
        /// Gets or sets if the view is loading
        /// </summary>
        public bool IsLoading
        {
            get => this.isLoading;
            set
            {
                this.isLoading = value;
                this.InvokeAsync(this.HasChanged);
            }
        }

        /// <summary>
        ///     An <see cref="IObservable{T}" /> for the current Selected Item
        /// </summary>
        public IObservable<object> SelectedItemObservable { get; protected set; }

        /// <summary>
        ///     Initialize the correspondant ViewModel for this component
        /// </summary>
        /// <param name="things">The collection of <see cref="Thing" /></param>
        /// <param name="projectId">The <see cref="Project" /> id</param>
        /// <param name="reviewId">The <see cref="Review" /> id</param>
        /// <returns>A <see cref="Task" /></returns>
        public abstract Task InitializeViewModel(IEnumerable<Thing> things, Guid projectId, Guid reviewId);

        /// <summary>
        ///     Handle the fact that something has changed and needs to update the view
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        public virtual async Task HasChanged()
        {
            await this.InvokeAsync(this.StateHasChanged);
        }

        /// <summary>
        ///     Tries to set the selected item to the ViewModel
        /// </summary>
        /// <param name="selectedItem">The previous selected <see cref="object" /></param>
        public abstract void TrySetSelectedItem(object selectedItem);
    }
}
