// --------------------------------------------------------------------------------------------------------
// <copyright file="IBelongsToInterfaceView.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel
{
    using UI_DSM.Client.Components.NormalUser.Views;

    /// <summary>
    ///     Interface that defines the fact that a row view model is part of the <see cref="InterfaceView" />
    /// </summary>
    public interface IBelongsToInterfaceView: IHaveThingRowViewModel
    {
        /// <summary>
        ///     The name of the source
        /// </summary>
        string SourceName { get; }

        /// <summary>
        ///     The name of the target
        /// </summary>
        string TargetName { get; }

        /// <summary>
        ///     The owner of the interface
        /// </summary>
        string Owner { get; }

        /// <summary>
        ///     The owner of the source
        /// </summary>
        string SourceOwner { get; }

        /// <summary>
        ///     The owner of the target
        /// </summary>
        string TargetOwner { get; }

        /// <summary>
        ///     The interface end of the Target
        /// </summary>
        string TargetEnd { get; }

        /// <summary>
        ///     The interface end of the Source
        /// </summary>
        string SourceEnd { get; }

        /// <summary>
        ///     A interface nature
        /// </summary>
        string Nature { get; }
    }
}
