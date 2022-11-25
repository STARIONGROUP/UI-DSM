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

        Power_Interfaces = 0,
        Signal_Interfaces = 1,
        TM_TC_Interfaces = 2,
        DataBus_Interfaces = 3,
        Str_Interfaces = 4,
        TC_Interfaces = 5,
        Mechanisms_Interfaces = 6,
        Prop_Interfaces = 7,
        Comms_Interfaces = 8,
        Other = 9,
    }
}
