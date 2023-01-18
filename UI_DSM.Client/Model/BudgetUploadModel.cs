// --------------------------------------------------------------------------------------------------------
// <copyright file="BudgetUploadModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Model
{
    using Microsoft.AspNetCore.Components.Forms;
    using System.ComponentModel.DataAnnotations;
   
    using UI_DSM.Client.Components.Administration.BudgetManagement;

    /// <summary>
    /// Model for Edit From for the <see cref="BudgetUpload"/> component
    /// </summary>
    public class BudgetUploadModel
    {
        /// <summary>
        ///     Gets or sets the name of the Budget
        /// </summary>
        [Required]
        public string BudgetName { get; set; }

        /// <summary>
        ///     Gets the <see cref="IBrowserFile" /> to upload
        /// </summary>
        [Required(ErrorMessage = "Please select a file")]
        public IBrowserFile BrowserFile { get; set; }
    }
}
