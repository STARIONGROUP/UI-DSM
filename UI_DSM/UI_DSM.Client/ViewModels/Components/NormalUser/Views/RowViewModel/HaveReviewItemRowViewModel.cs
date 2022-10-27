// --------------------------------------------------------------------------------------------------------
// <copyright file="HaveThingRowViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel
{
    using CDP4Common.CommonData;

    using ReactiveUI;

    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface that defines that a row view model is linked to a <see cref="TThing" />
    /// </summary>
    /// <typeparam name="TThing">A <see cref="Thing" /></typeparam>
    public class HaveThingRowViewModel<TThing> : ReactiveObject, IHaveThingRowViewModel where TThing : Thing
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="HaveThingRowViewModel{TThing}" /> class.
        /// </summary>
        public HaveThingRowViewModel(TThing thing)
        {
            this.Thing = thing;
        }

        /// <summary>
        ///     The associated <see cref="TThing" />
        /// </summary>
        public TThing Thing { get; private set; }

        /// <summary>
        ///     The <see cref="Shared.Models.ReviewItem" /> Id
        /// </summary>
        public ReviewItem ReviewItem { get; private set; }

        /// <summary>
        ///     Gets the <see cref="CDP4Common.CommonData.Thing" /> Id
        /// </summary>
        /// <returns>The <see cref="Guid" /></returns>
        public Guid ThingId => this.Thing?.Iid ?? Guid.Empty;

        /// <summary>
        ///     Updates the current <see cref="IHaveThingRowViewModel.ReviewItem" />
        /// </summary>
        /// <param name="item"></param>
        public void UpdateReviewItem(ReviewItem item)
        {
            if (item?.ThingId == this.Thing.Iid)
            {
                this.ReviewItem = item;
            }
        }
    }
}
