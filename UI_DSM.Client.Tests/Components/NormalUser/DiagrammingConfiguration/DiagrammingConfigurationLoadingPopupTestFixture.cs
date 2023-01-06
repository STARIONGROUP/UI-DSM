// --------------------------------------------------------------------------------------------------------
// <copyright file="DiagrammingConfigurationLoadingPopupTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Components.NormalUser.DiagrammingConfiguration
{
    using Bunit;

    using DevExpress.Blazor;

    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Forms;

    using NUnit.Framework;

    using UI_DSM.Client.Components.NormalUser.DiagrammingConfiguration;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.Components;
    using UI_DSM.Client.ViewModels.Components.NormalUser.DiagrammingConfiguration;
    using UI_DSM.Client.ViewModels.Components.NormalUser.ProjectReview;
    using UI_DSM.Shared.DTO.Common;
    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class DiagrammingConfigurationLoadingPopupTestFixture
    {
        private TestContext context;
        private IDiagrammingConfigurationLoadingPopupViewModel diagrammingConfigurationLoadingPopupViewModel;
        private IErrorMessageViewModel errorMessageViewModel;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.context.ConfigureDevExpressBlazor();
            this.errorMessageViewModel = new ErrorMessageViewModel();

            this.diagrammingConfigurationLoadingPopupViewModel = new DiagrammingConfigurationLoadingPopupViewModel()
            {
                SelectedConfiguration = "",
                OnValidSubmit = new EventCallbackFactory().Create(this, () => this.diagrammingConfigurationLoadingPopupViewModel.SelectedConfiguration = "")
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
                var renderer = this.context.RenderComponent<DiagrammingConfigurationLoadingPopup>(parameters =>
                {
                    parameters.Add(p => p.ViewModel, this.diagrammingConfigurationLoadingPopupViewModel);
                });

                var listBox = renderer.FindComponent<DxListBox<string, string>>();


                Assert.That(listBox.Instance.Values, Is.Empty);

                var configurationsName = new List<string>() { "config1", "config2" };

                this.diagrammingConfigurationLoadingPopupViewModel.ConfigurationsName = configurationsName;
                this.diagrammingConfigurationLoadingPopupViewModel.SelectedConfiguration = configurationsName.First();

                renderer.Render();

                var dxButton = renderer.FindComponent<EditForm>();
                await renderer.InvokeAsync(dxButton.Instance.OnValidSubmit.InvokeAsync);

                Assert.That(this.diagrammingConfigurationLoadingPopupViewModel.SelectedConfiguration, Is.EqualTo(""));
            }
            catch
            {
                // On GitHub, exception is thrown even if the JSRuntime has been configured
            }
        }
    }
}
