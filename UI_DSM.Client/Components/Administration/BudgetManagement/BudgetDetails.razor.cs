// --------------------------------------------------------------------------------------------------------
// <copyright file="BudgetDetails.razor.cs" company="RHEA System S.A.">
//  Copyright (c) 2023 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw, Nabil Abbar
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Client.Components.Administration.BudgetManagement
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.ViewModels.Components.Administration.ProjectManagement;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Component used to show details about <see cref="BudgetTemplate" /> inside a <see cref="Project" />
    /// </summary>
    public partial class BudgetDetails
    {
        /// <summary>
        ///     The <see cref="IProjectDetailsViewModel" /> for the component
        /// </summary>
        [Parameter]
        public IProjectDetailsViewModel ViewModel { get; set; }

        /// <summary>
        ///     Gets the list of <see cref="BudgetTemplate" />
        /// </summary>
        public List<BudgetTemplate> ProjectBudgets => this.GetProjectBudgets();

        /// <summary>
        ///     Gets the collection of <see cref="BudgetTemplate" />
        /// </summary>
        /// <returns>The collection of <see cref="BudgetTemplate" /></returns>
        private List<BudgetTemplate> GetProjectBudgets()
        {
            return this.ViewModel.Project.Artifacts.OfType<BudgetTemplate>().ToList();
        }
    }
}
