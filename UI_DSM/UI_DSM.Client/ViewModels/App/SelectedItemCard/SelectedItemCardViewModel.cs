// --------------------------------------------------------------------------------------------------------
// <copyright file="SelectedItemCardViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.App.SelectedItemCard
{
    using ReactiveUI;

    using UI_DSM.Client.Components.App.SelectedItemCard;
    using UI_DSM.Client.Components.App.SelectedItemCard.SelectedItemCardContent;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     View model for the <see cref="SelectedItemCard" /> component
    /// </summary>
    public class SelectedItemCardViewModel : ReactiveObject, ISelectedItemCardViewModel
    {
        /// <summary>
        ///     A <see cref="Dictionary{TKey,TValue}" /> to get correspond selected item component and the label
        /// </summary>
        private static readonly Dictionary<Type, Tuple<Type, string>> Correspondances = new()
        {
            [typeof(RequirementRowViewModel)] = new Tuple<Type, string>(typeof(RequirementSelectedItem), "Requirement Description"),
            [typeof(ProductRowViewModel)] = new Tuple<Type, string>(typeof(ProductSelectedItem), "Product Description"),
            [typeof(FunctionRowViewModel)] = new Tuple<Type, string>(typeof(FunctionSelectedItem), "Function Description"),
            [typeof(HyperLinkRowViewModel)] = new Tuple<Type, string>(typeof(HyperLinkSelectedItem), "HyperLink Description"),
            [typeof(RelationshipRowViewModel)] = new Tuple<Type, string>(typeof(RelationshipSelectedItem), "Relationship Description"),
            [typeof(PortRowViewModel)] = new Tuple<Type, string>(typeof(PortSelectedItem), "Port Description"),
            [typeof(InterfaceRowViewModel)] = new Tuple<Type, string>(typeof(InterfaceSelectedItem), "Interface Description"),
            [typeof(ElementBaseRowViewModel)] = new Tuple<Type, string>(typeof(ElementBaseSelectedItem), "Element Description"),
            [typeof(ReviewTask)] = new Tuple<Type, string>(typeof(ReviewTaskSelectedItem), "Task Description"),
        };

        /// <summary>
        ///     Backing field for <see cref="SelectedItem" />
        /// </summary>
        private object selectedItem;

        /// <summary>
        ///     The currently selected item
        /// </summary>
        public object SelectedItem
        {
            get => this.selectedItem;
            set => this.RaiseAndSetIfChanged(ref this.selectedItem, value);
        }

        /// <summary>
        ///     The label to display
        /// </summary>
        public string Label => this.GetLabel();

        /// <summary>
        ///     The <see cref="Type" /> of the component
        /// </summary>
        public Type ComponentType => this.GetComponentType();

        /// <summary>
        ///     Gets the correct <see cref="Type" /> component
        /// </summary>
        /// <returns>The <see cref="Type" /></returns>
        private Type GetComponentType()
        {
            if (this.SelectedItem == null)
            {
                return null;
            }

            var selectedItemType = this.SelectedItem.GetType();
            return Correspondances.ContainsKey(selectedItemType) ? Correspondances[selectedItemType].Item1 : null;
        }

        /// <summary>
        ///     Gets the correct label
        /// </summary>
        /// <returns>The label</returns>
        private string GetLabel()
        {
            if (this.SelectedItem == null)
            {
                return string.Empty;
            }

            var selectedItemType = this.SelectedItem.GetType();
            return Correspondances.ContainsKey(selectedItemType) ? Correspondances[selectedItemType].Item2 : null;
        }
    }
}
