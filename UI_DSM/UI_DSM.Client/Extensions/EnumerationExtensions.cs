// --------------------------------------------------------------------------------------------------------
// <copyright file="EnumerationExtensions.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw, Nabil Abbar, Jaime Bernar
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Client.Extensions
{
    using UI_DSM.Client.Enumerator;

    /// <summary>
    /// Static extension methods for <see cref="Enum"/>
    /// </summary>
    public static class EnumerationExtensions
    {
        /// <summary>
        /// Convert the <see cref="InterfaceCategory"/> to a string that represents an HTML color
        /// </summary>
        /// <param name="category">the <see cref="InterfaceCategory"/> to convert</param>
        /// <returns>the string color</returns>
        /// <exception cref="NotImplementedException">If the conversion is not implemented</exception>
        public static string ToColorString(this InterfaceCategory category)
        {
            switch (category)
            {
                case InterfaceCategory.Other: return "gray";
                case InterfaceCategory.Power_Interfaces: return "red";
                case InterfaceCategory.Signal_Interfaces: return "#00B0F0";
                case InterfaceCategory.TM_TC_Interfaces: return "#C85D7E";
                case InterfaceCategory.DataBus_Interfaces: return "yellow";
                case InterfaceCategory.Str_Interfaces: return "black";
                case InterfaceCategory.TC_Interfaces: return "#843C0C";
                case InterfaceCategory.Mechanisms_Interfaces: return "#7030A0";
                case InterfaceCategory.Prop_Interfaces: return "#FFC000";
                case InterfaceCategory.Comms_Interfaces: return "#99FFCC";
                default: throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Convert a <see cref="InterfaceCategory"/> to the full name
        /// </summary>
        /// <param name="category">the <see cref="InterfaceCategory"/> to convert</param>
        /// <returns>the name of the <see cref="InterfaceCategory"/></returns>
        /// <exception cref="NotImplementedException">If the conversion is not implemented</exception>
        public static string ToName(this InterfaceCategory category)
        {
            switch (category)
            {
                case InterfaceCategory.Other: return "Other";
                case InterfaceCategory.Power_Interfaces: return "Power";
                case InterfaceCategory.Signal_Interfaces: return "Signal";
                case InterfaceCategory.TM_TC_Interfaces: return "Tele-metry/command";
                case InterfaceCategory.DataBus_Interfaces: return "Data Bus";
                case InterfaceCategory.Str_Interfaces: return "Structural";
                case InterfaceCategory.TC_Interfaces: return "Thermal Control";
                case InterfaceCategory.Mechanisms_Interfaces: return "Mechanisms";
                case InterfaceCategory.Prop_Interfaces: return "Propulsion";
                case InterfaceCategory.Comms_Interfaces: return "Communications";
                default: throw new NotImplementedException();
            }
        }
    }
}
