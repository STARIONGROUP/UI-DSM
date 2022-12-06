// --------------------------------------------------------------------------------------------------------
// <copyright file="PortRowViewModel.cs" company="RHEA System S.A.">
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
    using CDP4Common.EngineeringModelData;

    using UI_DSM.Client.Extensions;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Row view model to display content for a <see cref="ElementUsage" /> that are ports
    /// </summary>
    public class PortRowViewModel : ElementBaseRowViewModel, IBelongsToInterfaceView
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PortRowViewModel" /> class.
        /// </summary>
        /// <param name="thing">The <see cref="ElementBase" /></param>
        /// <param name="reviewItem">The associated <see cref="HaveThingRowViewModel{TThing}.ReviewItem" /></param>
        public PortRowViewModel(ElementUsage thing, ReviewItem reviewItem) : base(thing, reviewItem)
        {
            this.InitializesProperties();
        }

        /// <summary>
        ///     Value indicating if the port is connected
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        ///     The interface end of the port
        /// </summary>
        public string InterfaceEnd { get; private set; }

        /// <summary>
        ///     The <see cref="ElementDefinition" /> container
        /// </summary>
        public ElementDefinition ElementContainer { get; private set; }

        /// <summary>
        ///     Gets the Id of the current <see cref="PortRowViewModel" />
        /// </summary>
        public override string Id => this.Thing.Name;

        /// <summary>
        ///     The name of the source
        /// </summary>
        public string SourceName => null;

        /// <summary>
        ///     The name of the target
        /// </summary>
        public string TargetName => null;

        /// <summary>
        ///     The owner of the source
        /// </summary>
        public string SourceOwner => null;

        /// <summary>
        ///     The owner of the target
        /// </summary>
        public string TargetOwner => null;

        /// <summary>
        ///     The interface end of the Target
        /// </summary>
        public string TargetEnd => null;

        /// <summary>
        ///     The interface end of the Source
        /// </summary>
        public string SourceEnd => null;

        /// <summary>
        ///     A interface nature
        /// </summary>
        public string Nature => null;

        /// <summary>
        ///     Initiliazes this row properties
        /// </summary>
        private void InitializesProperties()
        {
            var elementUsage = this.Thing as ElementUsage;
            this.IsConnected = elementUsage.GetInterfacesOfPort().Any();
            this.ElementContainer = this.Thing.Container as ElementDefinition;
            this.InterfaceEnd = elementUsage.InterfaceEnd.ToString();
        }
    }
}
