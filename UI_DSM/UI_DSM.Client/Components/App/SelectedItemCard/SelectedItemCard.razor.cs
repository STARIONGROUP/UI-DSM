// --------------------------------------------------------------------------------------------------------
// <copyright file="SelectedItemCard.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.App.SelectedItemCard
{
    using Microsoft.AspNetCore.Components;

    using ReactiveUI;

    using UI_DSM.Client.ViewModels.App;
    using UI_DSM.Client.ViewModels.App.SelectedItemCard;

    /// <summary>
    ///     Component that will provided related information for a selected item
    /// </summary>
    public partial class SelectedItemCard : IDisposable
    {
        /// <summary>
        ///     A collection of <see cref="IDisposable" />
        /// </summary>
        private List<IDisposable> disposables = new();

        /// <summary>
        ///     A <see cref="Dictionary{TKey,TValue}" /> for the <see cref="DynamicComponent.Parameters" />
        /// </summary>
        private Dictionary<string, object> parameters = new();

        /// <summary>
        ///     The selected item to observe
        /// </summary>
        [Inject]
        public ISelectedItemCardViewModel ViewModel { get; set; }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
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
            this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.SelectedItem)
                .Subscribe(async _ => await this.UpdateProperties()));
        }

        /// <summary>
        ///     Update this component property
        /// </summary>
        private async Task UpdateProperties()
        {
            this.parameters.Clear();
            this.parameters[nameof(this.ViewModel.SelectedItem)] = this.ViewModel.SelectedItem;
            await this.InvokeAsync(this.StateHasChanged);
        }
    }
}
