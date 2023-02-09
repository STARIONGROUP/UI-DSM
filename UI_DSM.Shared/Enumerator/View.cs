// --------------------------------------------------------------------------------------------------------
// <copyright file="View.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Shared.Enumerator
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    ///     Enumeration that represents different kind of View that a user that have during his review journey
    /// </summary>
    public enum View
    {
        /// <summary>
        ///     When there is no view
        /// </summary>
        None = 0,

        /// <summary>
        ///     No specific view associated, based on external document
        /// </summary>
        [Display(Name = "Document Based")]
        DocumentBased = 1,

        /// <summary>
        ///     View associated to represent a Requirement Breakdown Structure
        /// </summary>
        [Display(Name = "Requirement Breakdown")]
        RequirementBreakdownStructureView = 2,

        /// <summary>
        ///     View associated to represent a Product Breakdown Structure
        /// </summary>
        [Display(Name = "Product Breakdown")]
        ProductBreakdownStructureView = 3,

        /// <summary>
        ///     View associated to represent a Functional Breakdown Structure
        /// </summary>
        [Display(Name = "Functional Breakdown")]
        FunctionalBreakdownStructureView = 4,

        /// <summary>
        ///     View associated to represent a Requirement Traceability to Requirement
        /// </summary>
        [Display(Name = "Requirement Traceability To Requirement")]
        RequirementTraceabilityToRequirementView = 5,

        /// <summary>
        ///     View associated to represent a Requirement Traceability to Product
        /// </summary>
        [Display(Name = "Requirement Traceability To Product")]
        RequirementTraceabilityToProductView = 6,

        /// <summary>
        ///     View associated to represent a Requirement Traceability to Function
        /// </summary>
        [Display(Name = "Requirement Traceability To Function")]
        RequirementTraceabilityToFunctionView = 7,

        /// <summary>
        ///     View associated to represent a Function Traceability to Product
        /// </summary>
        [Display(Name = "Function Traceability To Product")]
        FunctionalTraceabilityToProductView = 8,

        /// <summary>
        ///     View associated to represent a Requirement Verification Control
        /// </summary>
        [Display(Name = "Requirement Verification Control")]
        RequirementVerificationControlView = 9,

        /// <summary>
        ///     View associated to represent a Budget
        /// </summary>
        [Display(Name = "Budget")]
        BudgetView = 10,

        /// <summary>
        ///     View associated to represent an Interface
        /// </summary>
        [Display(Name = "Interface")]
        InterfaceView = 11,

        /// <summary>
        ///     View associated to represent the InterfaceView through Diagram
        /// </summary>
        [Display(Name = "Physical Architecture Diagram")]
        PhysicalFlowView = 12,

        /// <summary>
        ///     View associated to represent a TRL
        /// </summary>
        [Display(Name = "TRL")]
        TrlView = 13
    }
}
