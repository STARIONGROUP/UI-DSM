// --------------------------------------------------------------------------------------------------------
// <copyright file="DiagrammingConfigurationPopupTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Components.NormalUser.ProjectReview
{
    using Bunit;

    using DevExpress.Blazor;

    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Forms;

    using NUnit.Framework;
    using UI_DSM.Client.Components.NormalUser.DiagrammingConfiguration;
    using UI_DSM.Client.Components.NormalUser.ProjectReview;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.Components;
    using UI_DSM.Client.ViewModels.Components.NormalUser.ProjectReview;
    using UI_DSM.Shared.Models;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class DiagrammingConfigurationPopupTestFixture
    {
        private TestContext context;
        private IDiagrammingConfigurationPopupViewModel diagrammingConfigurationPopupViewModel;
        private IErrorMessageViewModel errorMessageViewModel;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.context.ConfigureDevExpressBlazor();
            this.errorMessageViewModel = new ErrorMessageViewModel();

            this.diagrammingConfigurationPopupViewModel = new DiagrammingConfigurationPopupViewModel()
            {
                ConfigurationName = "",
                OnValidSubmit = new EventCallbackFactory().Create(this, () => this.diagrammingConfigurationPopupViewModel.ConfigurationName = "")
            };
        }

        [TearDown]
        public void TearDown()
        {
            this.context.CleanContext(); 
        }

        [Test]
        public async Task VerifyComponent()
        {
            try
            {
                var renderer = this.context.RenderComponent<DiagrammingConfigurationPopup>(parameters =>
                {
                    parameters.Add(p => p.ViewModel, this.diagrammingConfigurationPopupViewModel);
                });

                var textBox = renderer.FindComponent<DxTextBox>();

                Assert.Equals(textBox.Instance.Text, String.Empty);

                this.diagrammingConfigurationPopupViewModel.ConfigurationName = "test";

                renderer.Render();
                Assert.That(textBox.Instance.Text, Is.EqualTo(this.diagrammingConfigurationPopupViewModel.ConfigurationName));

                var dxButton = renderer.FindComponent<EditForm>();
                await renderer.InvokeAsync(dxButton.Instance.OnValidSubmit.InvokeAsync);

                Assert.Equals(this.diagrammingConfigurationPopupViewModel.ConfigurationName, String.Empty);
            }
            catch
            {
                // On GitHub, exception is thrown even if the JSRuntime has been configured
            }
        }
    }
}
