// --------------------------------------------------------------------------------------------------------
// <copyright file="AppKeyValue.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.App.AppKeyValue
{
    using Microsoft.AspNetCore.Components;

    /// <summary>
    ///     Component to display a pair of Key-Value
    /// </summary>
    public partial class AppKeyValue
    {
        /// <summary>
        ///     Different variants on the button, for styling purpose
        /// </summary>
        public enum VariantValue
        {
            Horizontal,
            Vertical
        }

        /// <summary>
        ///     Key label name
        /// </summary>
        [Parameter]
        public string Key { get; set; }

        /// <summary>
        ///     Value string
        /// </summary>
        [Parameter]
        public string Value { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="VariantValue" />
        /// </summary>
        [Parameter]
        public VariantValue Variant { get; set; } = VariantValue.Horizontal;

        /// <summary>
        ///     String value conversion based on <see cref="VariantValue" />
        /// </summary>
        private string componentVariant { get; set; }

        /// <summary>
        ///     The <see cref="EventCallback{TValue}" /> to handle the double click on a property for navigation
        /// </summary>
        [Parameter]
        public EventCallback<string> OnItemDoubleClick { get; set; }

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        /// </summary>
        protected override void OnInitialized()
        {
            this.componentVariant = this.Variant switch
            {
                VariantValue.Horizontal => "app-key-value--horizontal",
                VariantValue.Vertical => "app-key-value--vertical",
                _ => ""
            };
        }

        /// <summary>
        ///     Fire the <see cref="OnItemDoubleClick" /> callback
        /// </summary>
        /// <param name="propertyName">The name of the double-clicked property</param>
        /// <returns>A <see cref="Task" /></returns>
        private Task OnDoubleClick(string propertyName)
        {
            return this.OnItemDoubleClick.HasDelegate ? this.InvokeAsync(() => this.OnItemDoubleClick.InvokeAsync(propertyName)) : Task.CompletedTask;
        }
    }
}
