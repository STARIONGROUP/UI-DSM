// --------------------------------------------------------------------------------------------------------
// <copyright file="OptionChooser.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.App.OptionChooser
{
    using CDP4Common.EngineeringModelData;

    using Microsoft.AspNetCore.Components;

    using ReactiveUI;

    using UI_DSM.Client.ViewModels.App.OptionChooser;

    /// <summary>
    ///     Component to let the user choose the <see cref="Option" /> that he wants to see for building a tree
    /// </summary>
    public partial class OptionChooser : IDisposable
    {
        /// <summary>
        ///     A collection of <see cref="IDisposable" />
        /// </summary>
        private readonly List<IDisposable> disposables = new();

        /// <summary>
        ///     The <see cref="IOptionChooserViewModel" />
        /// </summary>
        [Parameter]
        public IOptionChooserViewModel ViewModel { get; set; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.disposables.ForEach(x => x.Dispose());
        }

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        /// </summary>
        protected override void OnInitialized()
        {
            this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.IsVisible)
                .Subscribe(_ => this.InvokeAsync(this.StateHasChanged)));

            this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.SelectedOption)
                .Subscribe(_ => this.OnSelectedOptionChange()));

            base.OnInitialized();
        }

        /// <summary>
        ///     Opens the OptionChooser
        /// </summary>
        private void OpenOptionChooser()
        {
            this.ViewModel.IsVisible = !this.ViewModel.IsVisible;
        }

        /// <summary>
        ///     Handle the change of the <see cref="IOptionChooserViewModel.SelectedOption" />
        /// </summary>
        private void OnSelectedOptionChange()
        {
            this.ViewModel.IsVisible = false;
        }
    }
}
