// --------------------------------------------------------------------------------------------------------
// <copyright file="IBudgetViewViewModel.cs" company="RHEA System S.A.">
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
    /// <summary>
    ///     Interface definition for <see cref="BudgetViewViewModel" />
    /// </summary>
    public interface IBudgetViewViewModel : IBaseViewViewModel
    {
        /// <summary>
        /// Gets or sets the Report Names
        /// </summary>
        IEnumerable<string> ReportNames { get; set; }

        /// <summary>
        /// Gets or sets the Report ID
        /// </summary>
        string ReportDtoAsString { get; set; }

        /// <summary>
        /// Gets or sets the Report Name
        /// </summary>
        string ReportName { get; set; }
    }
}
