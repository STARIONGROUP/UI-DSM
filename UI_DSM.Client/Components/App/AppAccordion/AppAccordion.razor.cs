// --------------------------------------------------------------------------------------------------------
// <copyright file="AppAccordion.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.App.AppAccordion
{
    using Microsoft.AspNetCore.Components;

    /// <summary>
    ///     Component that allow collapsing and expanding data
    /// </summary>
    public partial class AppAccordion
    {
        /// <summary>
        ///     Different variants on the button, for styling purpose
        /// </summary>
        public enum VariantValue
        {
            Default,
            Small,
            Medium,
            Tiny,
            Reply
        }

        /// <summary>
        ///     The css class to apply to panel
        /// </summary>
        private string PanelClass => this.PanelOpen ? "app-accordion__panel--is-open" : null;

        /// <summary>
        ///     The css class to apply to button
        /// </summary>
        private string ButtonClass => this.PanelOpen ? "app-accordion__button--is-active" : null;

        /// <summary>
        ///     Gets or sets the <see cref="VariantValue" />
        /// </summary>
        [Parameter]
        public VariantValue Variant { get; set; } = VariantValue.Default;

        /// <summary>
        ///     String value conversion based on <see cref="VariantValue" />
        /// </summary>
        private string accordionVariant { get; set; }

        /// <summary>
        ///     Button label used as aria label and innerhtml
        /// </summary>
        [Parameter]
        public string Label { get; set; }

        /// <summary>
        ///     HTML ID for the app accordion
        /// </summary>
        [Parameter]
        public string Id { get; set; }

        /// <summary>
        ///     HTML child content for the panel
        /// </summary>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        ///     Bool to keep track of the open state of the panel
        /// </summary>
        [Parameter]
        public bool PanelOpen { get; set; } = true;

        /// <summary>
        ///     <see cref="EventCallback{TValue}" /> when the <see cref="PanelOpen" /> value changed
        /// </summary>

        [Parameter]
        public EventCallback<bool> PanelOpenChanged { get; set; }

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        /// </summary>
        protected override void OnInitialized()
        {
            this.accordionVariant = this.Variant switch
            {
                VariantValue.Default => "app-accordion__button--default",
                VariantValue.Small => "app-accordion__button--small",
                VariantValue.Medium => "app-accordion__button--medium",
                VariantValue.Tiny => "app-accordion__button--tiny",
                VariantValue.Reply => "app-accordion__button--reply",
                _ => ""
            };
        }

        /// <summary>
        ///     Method to toggle panel state
        /// </summary>
        private async Task TogglePanel()
        {
            this.PanelOpen = !this.PanelOpen;

            if (this.PanelOpenChanged.HasDelegate)
            {
                await this.PanelOpenChanged.InvokeAsync(this.PanelOpen);
            }
        }
    }
}
