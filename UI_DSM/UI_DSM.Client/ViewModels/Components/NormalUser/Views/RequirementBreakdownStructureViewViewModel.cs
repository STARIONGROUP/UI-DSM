// --------------------------------------------------------------------------------------------------------
// <copyright file="RequirementBreakdownStructureViewViewModel.cs" company="RHEA System S.A.">
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
    using CDP4Common.EngineeringModelData;

    using UI_DSM.Client.Components.NormalUser.Views;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;

    /// <summary>
    ///     View model for the <see cref="RequirementBreakdownStructureView" /> component
    /// </summary>
    public class RequirementBreakdownStructureViewViewModel : BaseViewViewModel, IRequirementBreakdownStructureViewViewModel
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RequirementBreakdownStructureViewViewModel" /> class.
        /// </summary>
        public RequirementBreakdownStructureViewViewModel()
        {
            this.Rows = new List<RequirementBreakdownStructureViewRowViewModel>();
        }

        /// <summary>
        ///     Initialize this view model properties
        /// </summary>
        /// <param name="things">A collection of <see cref="BaseViewViewModel.Things" /></param>
        public override void InitializeProperties(IEnumerable<Thing> things)
        {
            base.InitializeProperties(things);

            this.Rows = new List<RequirementBreakdownStructureViewRowViewModel>(this.Things.OfType<RequirementsSpecification>().SelectMany(x => x.Requirement)
                .Select(x => new RequirementBreakdownStructureViewRowViewModel(x)));
        }

        /// <summary>
        ///     A collection of <see cref="Requirement" /> that has been filtered
        /// </summary>
        public IEnumerable<RequirementBreakdownStructureViewRowViewModel> Rows { get; private set; }
    }
}
