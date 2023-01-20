// --------------------------------------------------------------------------------------------------------
// <copyright file="InterfaceCategory.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Jaime Bernar
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------
namespace UI_DSM.Client.Enumerator
{
    /// <summary>
    /// Enumeration that defines different categories for the interfaces
    /// </summary>
    public enum InterfaceCategory
    {
        /// <summary>
        /// Power interfaces Category
        /// </summary>
        Power_Interfaces = 0,

        /// <summary>
        /// Signal interfaces Category
        /// </summary>
        Signal_Interfaces = 1,

        /// <summary>
        /// Telemetry-Telecommand Category
        /// </summary>
        TM_TC_Interfaces = 2,

        /// <summary>
        /// Data Bus Category
        /// </summary>
        DataBus_Interfaces = 3,

        /// <summary>
        /// Structural Category
        /// </summary>
        Str_Interfaces = 4,

        /// <summary>
        /// Thermal Control Category
        /// </summary>
        TC_Interfaces = 5,

        /// <summary>
        /// Mechanisms Category
        /// </summary>
        Mechanisms_Interfaces = 6,

        /// <summary>
        /// Propulsion Category
        /// </summary>
        Prop_Interfaces = 7,

        /// <summary>
        /// Communications Category
        /// </summary>
        Comms_Interfaces = 8,

        /// <summary>
        /// Other Category
        /// </summary>
        Other = 9,
    }
}
