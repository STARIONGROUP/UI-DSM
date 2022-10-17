// --------------------------------------------------------------------------------------------------------
// <copyright file="ViewDeserializer.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
//
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft
//
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
//
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

// --------------------------------------------------------------------------------------------------------
// ------------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!------------
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Serializer.Json
{
    using UI_DSM.Shared.Enumerator;

    /// <summary>
    ///     The purpose of the <see cref="ViewDeserializer" /> is to provide deserialization capabilities for enum
    /// </summary>
    internal static class ViewDeserializer
    {
        /// <summary>
        ///     Deserializes a string value to a <see cref="View"/>
        /// </summary>
        /// <param name="value">The string representation of the <see cref="View"/></param>
        /// <returns>The value of the <see cref="View"/></returns>
        internal static View Deserialize(string value)
        {
            return value switch
            {
                "NONE" => View.None,
                "DOCUMENTBASED" => View.DocumentBased,
                "REQUIREMENTBREAKDOWNSTRUCTUREVIEW" => View.RequirementBreakdownStructureView,
                "PRODUCTBREAKDOWNSTRUCTUREVIEW" => View.ProductBreakdownStructureView,
                "FUNCTIONALBREAKDOWNSTRUCTUREVIEW" => View.FunctionalBreakdownStructureView,
                "REQUIREMENTTRACEABILITYTOREQUIREMENTVIEW" => View.RequirementTraceabilityToRequirementView,
                "REQUIREMENTTRACEABILITYTOPRODUCTVIEW" => View.RequirementTraceabilityToProductView,
                "REQUIREMENTTRACEABILITYTOFUNCTIONVIEW" => View.RequirementTraceabilityToFunctionView,
                "FUNCTIONALTRACEABILITYTOPRODUCTVIEW" => View.FunctionalTraceabilityToProductView,
                "REQUIREMENTVERIFICATIONCONTROLVIEW" => View.RequirementVerificationControlView,
                "BUDGETVIEW" => View.BudgetView,
                "INTERFACEVIEW" => View.InterfaceView,
                "PHYSICALFLOWVIEW" => View.PhysicalFlowView,
                "TRLVIEW" => View.TrlView,
                "FUNCTIONALLOCATIONTOTECHNOLOGYVIEW" => View.FunctionAllocationToTechnologyView,
                _ => throw new ArgumentException($"{value} is not a valid View", nameof(value))
            };
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------