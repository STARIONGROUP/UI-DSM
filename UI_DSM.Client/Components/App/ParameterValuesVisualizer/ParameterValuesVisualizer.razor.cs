// --------------------------------------------------------------------------------------------------------
// <copyright file="ParameterValuesVisualizer.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.App.ParameterValuesVisualizer
{
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using DevExpress.Blazor;

    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.Extensions;

    /// <summary>
    ///     Component that enables to see all values of a <see cref="ParameterOrOverrideBase" />
    /// </summary>
    public partial class ParameterValuesVisualizer
    {
        /// <summary>
        ///     The value to display
        /// </summary>
        private string displayValue;

        /// <summary>
        ///     Value asserting that the <see cref="Parameter" /> has more than one value
        /// </summary>
        private bool hasMultipleValues;

        /// <summary>
        ///     The <see cref="ParameterOrOverrideBase" />
        /// </summary>
        [Parameter]
        public ParameterOrOverrideBase Parameter { get; set; }

        /// <summary>
        ///     The <see cref="Option" />
        /// </summary>
        [Parameter]
        public Option Option { get; set; }

        /// <summary>
        ///     Reference to the <see cref="DxPopup" />
        /// </summary>
        public DxPopup Popup { get; set; }

        /// <summary>
        ///     Method invoked when the component has received parameters from its parent in
        ///     the render tree, and the incoming values have been assigned to properties.
        /// </summary>
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            this.displayValue = this.ComputeDisplayValue();
            this.hasMultipleValues = this.ComputeHasMultipleValues();
        }

        /// <summary>
        ///     Verifies that the current <see cref="Parameter" /> has more than one value
        /// </summary>
        /// <returns>True if the <see cref="Parameter" /> has multiple values</returns>
        private bool ComputeHasMultipleValues()
        {
            if (this.Parameter.StateDependence != null)
            {
                return true;
            }

            return this.Parameter.ParameterType switch
            {
                CompoundParameterType => true,
                _ => false
            };
        }

        /// <summary>
        ///     Computes the value to display
        /// </summary>
        /// <returns>The value</returns>
        private string ComputeDisplayValue()
        {
            var actualFiniteState = this.Parameter.StateDependence?.ActualState[0];

            return this.Parameter.ParameterType switch
            {
                CompoundParameterType compoundParameterType => GetCompoundParameterTypeHeader(compoundParameterType),
                QuantityKind => actualFiniteState != null
                    ? $"{actualFiniteState.Name} : {this.Parameter.GetParameterValue(this.Option, actualFiniteState)} ({this.Parameter.Scale.ShortName})"
                    : $"{this.Parameter.GetParameterValue(this.Option, null)} ({this.Parameter.Scale.ShortName})",
                ScalarParameterType => actualFiniteState != null
                    ? $"{actualFiniteState.Name} : {this.Parameter.GetParameterValue(this.Option, actualFiniteState)}"
                    : $"{this.Parameter.GetParameterValue(this.Option, null)}",
                _ => "-"
            };
        }

        /// <summary>
        ///     Gets the header of a <see cref="CompoundParameterType" />
        /// </summary>
        /// <param name="compoundParameterType">The <see cref="CompoundParameterType" /></param>
        /// <returns>The header</returns>
        private static string GetCompoundParameterTypeHeader(CompoundParameterType compoundParameterType)
        {
            return $"[{string.Join(", ", compoundParameterType.Component.Select(x => x.ShortName))}]";
        }

        /// <summary>
        ///     Opens the <see cref="DxPopup" />
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private Task OpenPopup()
        {
            return this.Popup.ShowAsync();
        }
    }
}
