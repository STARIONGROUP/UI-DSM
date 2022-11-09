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
        DocumentBased = 1,

        /// <summary>
        ///     View associated to represent a Requirement Breakdown Structure
        /// </summary>
        RequirementBreakdownStructureView = 2,

        /// <summary>
        ///     View associated to represent a Product Breakdown Structure
        /// </summary>
        ProductBreakdownStructureView = 3,

        /// <summary>
        ///     View associated to represent a Functional Breakdown Structure
        /// </summary>
        FunctionalBreakdownStructureView = 4,

        /// <summary>
        ///     View associated to represent a Requirement Traceability to Requirement
        /// </summary>
        RequirementTraceabilityToRequirementView = 5,

        /// <summary>
        ///     View associated to represent a Requirement Traceability to Product
        /// </summary>
        RequirementTraceabilityToProductView = 6,

        /// <summary>
        ///     View associated to represent a Requirement Traceability to Function
        /// </summary>
        RequirementTraceabilityToFunctionView = 7,

        /// <summary>
        ///     View associated to represent a Function Traceability to Product
        /// </summary>
        FunctionalTraceabilityToProductView = 8,

        /// <summary>
        ///     View associated to represent a Requirement Verification Control
        /// </summary>
        RequirementVerificationControlView = 9,

        /// <summary>
        ///     View associated to represent a Budget
        /// </summary>
        BudgetView = 10,

        /// <summary>
        ///     View associated to represent an Interface
        /// </summary>
        InterfaceView = 11,

        /// <summary>
        ///     View associated to represent a Requirement Traceability to Requirement
        /// </summary>
        PhysicalFlowView = 12,

        /// <summary>
        ///     View associated to represent a TRL
        /// </summary>
        TrlView = 13
    }
}
