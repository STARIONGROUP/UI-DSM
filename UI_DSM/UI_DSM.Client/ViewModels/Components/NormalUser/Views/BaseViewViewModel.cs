// --------------------------------------------------------------------------------------------------------
// <copyright file="BaseViewViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Components.NormalUser.Views
{
    using CDP4Common.CommonData;

    using ReactiveUI;

    using UI_DSM.Client.Components.NormalUser.Views;

    /// <summary>
    ///     View model for the <see cref="GenericBaseView{TViewModel}" /> component
    /// </summary>
    public abstract class BaseViewViewModel : ReactiveObject, IBaseViewViewModel
    {
        /// <summary>
        ///     Backing field for <see cref="SelectedElement" />
        /// </summary>
        private object selectedElement;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseViewViewModel" /> class.
        /// </summary>
        protected BaseViewViewModel()
        {
            this.Things = new List<Thing>();
        }

        /// <summary>
        ///     A collection of <see cref="Thing" />
        /// </summary>
        public IEnumerable<Thing> Things { get; private set; }

        /// <summary>
        ///     The currently selected element
        /// </summary>
        public object SelectedElement
        {
            get => this.selectedElement;
            set => this.RaiseAndSetIfChanged(ref this.selectedElement, value);
        }

        /// <summary>
        ///     Initialize this view model properties
        /// </summary>
        /// <param name="things">A collection of <see cref="Things" /></param>
        public virtual void InitializeProperties(IEnumerable<Thing> things)
        {
            this.Things = things;
        }
    }
}
