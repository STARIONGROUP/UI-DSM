// --------------------------------------------------------------------------------------------------------
// <copyright file="SelectableComponent.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.App.SelectableComponent
{
    /// <summary>
    ///     Defines that a Component can be selected
    /// </summary>
    public abstract partial class SelectableComponent
    {
        /// <summary>
        ///     Value indicating if the component is selected or not
        /// </summary>
        public bool IsSelected { get; private set; }

        /// <summary>
        ///     Selects the current component
        /// </summary>
        public virtual void Select()
        {
            this.IsSelected = true;
        }

        /// <summary>
        ///     Deselects the current component
        /// </summary>
        public virtual void Deselect()
        {
            this.IsSelected = false;
        }
    }
}
