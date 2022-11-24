// --------------------------------------------------------------------------------------------------------
// <copyright file="PhysicalFlowView.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.NormalUser.Views
{
    using Blazor.Diagrams.Components;
    using Blazor.Diagrams.Core;
    using Blazor.Diagrams.Core.Models.Base;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Web;
    using System.Threading.Tasks;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views;
    using UI_DSM.Shared.Enumerator;

    /// <summary>
    /// Component for the <see cref="View.PhysicalFlowView"/>
    /// </summary>
    public partial class PhysicalFlowView : GenericBaseView<IInterfaceViewViewModel>, IReusableView
    {
        /// <summary>
        /// Gets or sets the diagram component.
        /// </summary>
        public Diagram Diagram { get; set; }

        /// <summary>
        /// Method invoked when the component is ready to start, having received its
        /// initial parameters from its parent in the render tree.
        /// </summary>
        protected async override void OnInitialized()
        {
            base.OnInitialized();

            this.Diagram = new Diagram();
            this.Diagram.MouseUp += Diagram_MouseUp;
        }

        /// <summary>
        /// MouseUp event for the diagram component.
        /// </summary>
        /// <param name="model">the model clicked (NodeModel,PortModel or LinkModel)</param>
        /// <param name="args">the args of the event</param>
        private void Diagram_MouseUp(Model model, MouseEventArgs args)
        {
            //Right button
            if(args.Button == 0)
            {
                this.ViewModel.SetSelectedModel(model);
            }
        }

        /// <summary>
        ///     Tries to copy components from another <see cref="BaseView" />
        /// </summary>
        /// <param name="otherView">The other <see cref="BaseView" /></param>
        /// <returns>Value indicating if it could copy components</returns>
        public async Task<bool> CopyComponents(BaseView otherView)
        {
            this.ViewModel.OnCentralNodeChanged -= this.OnCentralNodeChanged;
            if (otherView is not GenericBaseView<IInterfaceViewViewModel> interfaceView)
            {
                return false;
            }

            this.ViewModel = interfaceView.ViewModel;
            this.OnCentralNodeChanged();
            await this.HasChanged();

            this.ViewModel.OnCentralNodeChanged += this.OnCentralNodeChanged;

            return true;
        }

        /// <summary>
        /// Method that handles the on central node changed event.
        /// </summary>
        public void OnCentralNodeChanged()
        {
            this.Diagram.Nodes.Clear();
            this.ViewModel.ProductNodes.ForEach(node => this.Diagram.Nodes.Add(node));
            this.StateHasChanged();
        }
    }
}
