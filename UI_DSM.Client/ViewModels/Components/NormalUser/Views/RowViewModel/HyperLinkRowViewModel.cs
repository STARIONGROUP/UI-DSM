// --------------------------------------------------------------------------------------------------------
// <copyright file="HyperLinkRowViewModel.cs" company="RHEA System S.A.">
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

    using UI_DSM.Shared.Models;

    /// <summary>
    ///     <see cref="HaveThingRowViewModel{TThing}" /> for <see cref="HyperLink" />
    /// </summary>
    public class HyperLinkRowViewModel : HaveThingRowViewModel<HyperLink>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="HyperLinkRowViewModel" /> class.
        /// </summary>
        /// <param name="thing">The <see cref="HyperLink" /></param>
        /// <param name="reviewItem">The associated <see cref="HaveThingRowViewModel{TThing}.ReviewItem" /></param>
        public HyperLinkRowViewModel(HyperLink thing, ReviewItem reviewItem) : base(thing, reviewItem)
        {
        }

        /// <summary>
        ///     Gets the Id of the current <see cref="IHaveThingRowViewModel" />
        /// </summary>
        public override string Id => this.Thing.Content;

        /// <summary>
        ///     The url of the <see cref="HyperLink" />
        /// </summary>
        public string Url => this.Thing.Uri;
    }
}
