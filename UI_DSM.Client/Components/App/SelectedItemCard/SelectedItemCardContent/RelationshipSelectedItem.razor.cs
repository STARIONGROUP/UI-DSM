// --------------------------------------------------------------------------------------------------------
// <copyright file="RelationshipSelectedItem.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.App.SelectedItemCard.SelectedItemCardContent
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.ViewModels.App.SelectedItemCard;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;

    /// <summary>
    ///     Component to visualize information related when a <see cref="RelationshipRowViewModel" /> is selected
    /// </summary>
    public partial class RelationshipSelectedItem
    {
        /// <summary>
        ///     A <see cref="Dictionary{TKey,TValue}" /> for the <see cref="DynamicComponent.Parameters" /> for source
        /// </summary>
        private readonly Dictionary<string, object> sourceParameters = new();

        /// <summary>
        ///     A <see cref="Dictionary{TKey,TValue}" /> for the <see cref="DynamicComponent.Parameters" /> for target
        /// </summary>
        private readonly Dictionary<string, object> targetParameters = new();

        /// <summary>
        ///     The correspondance for the source component
        /// </summary>
        private Tuple<Type, string> sourceCorrespondance;

        /// <summary>
        ///     The correspondance for the target component
        /// </summary>
        private Tuple<Type, string> targetCorrespondance;

        /// <summary>
        ///     Method invoked when the component has received parameters from its parent in
        ///     the render tree, and the incoming values have been assigned to properties.
        /// </summary>
        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            this.sourceParameters[nameof(this.SelectedItem)] = this.RowViewModel.SourceRow;
            this.targetParameters[nameof(this.SelectedItem)] = this.RowViewModel.TargetRow;
            this.sourceCorrespondance = SelectedItemCardViewModel.GetCorrespondances(this.RowViewModel.SourceRow);
            this.targetCorrespondance = SelectedItemCardViewModel.GetCorrespondances(this.RowViewModel.TargetRow);
        }
    }
}
