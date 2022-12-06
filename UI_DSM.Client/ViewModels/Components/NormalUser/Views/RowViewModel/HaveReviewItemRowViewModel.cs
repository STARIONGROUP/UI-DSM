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
    public abstract class HaveThingRowViewModel<TThing> : ReactiveObject, IHaveThingRowViewModel where TThing : Thing
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="HaveThingRowViewModel{TThing}" /> class.
        /// </summary>
        /// <param name="thing">The <see cref="TThing" /></param>
        /// <param name="reviewItem">The associated <see cref="ReviewItem" /></param>
        protected HaveThingRowViewModel(TThing thing, ReviewItem reviewItem)
        {
            this.Thing = thing;
            this.UpdateReviewItem(reviewItem);
            this.IsVisible = true;
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

        /// <summary>
        ///     Gets the Id of the current <see cref="IHaveThingRowViewModel" />
        /// </summary>
        public abstract string Id { get; }

        /// <summary>
        ///     A value indicating if the row is visible or not
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        ///     <see langword="true" /> if the specified object  is equal to the current object; otherwise,
        ///     <see langword="false" />.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is HaveThingRowViewModel<TThing> other)
            {
                return other.ThingId == this.ThingId;
            }

            return false;
        }

        /// <summary>
        ///     Indicates if the current associated <see cref="ReviewItem" /> has some <see cref="Comment" />
        /// </summary>
        /// <returns>The assert</returns>
        public bool HasComment()
        {
            return this.ReviewItem != null && this.ReviewItem.Annotations.OfType<Comment>().Any();
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return this.ThingId.GetHashCode();
        }
    }
}
