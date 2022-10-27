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

    using UI_DSM.Shared.Enumerator;

    /// <summary>
    ///     Base component to display a <see cref="View" /> to view <see cref="Thing" /> for reviewing the model
    /// </summary>
    public abstract partial class GenericBaseView<TViewModel>
    {
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
    }
}
