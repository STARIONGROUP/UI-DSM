// --------------------------------------------------------------------------------------------------------
// <copyright file="ProjectCreationTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Components.Administration.ProjectManagement
{
    using Bunit;

    using DevExpress.Blazor;

    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Forms;

    using NUnit.Framework;

    using UI_DSM.Client.Components.Administration.ProjectManagement;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.Components;
    using UI_DSM.Client.ViewModels.Components.Administration.ProjectManagement;
    using UI_DSM.Shared.Models;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class ProjectCreationTestFixture
    {
        private TestContext context;
        private IProjectCreationViewModel viewModel;
        private IErrorMessageViewModel errorMessage;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.errorMessage = new ErrorMessageViewModel();
            this.context.ConfigureDevExpressBlazor();

            this.viewModel = new ProjectCreationViewModel()
            {
                OnValidSubmit = new EventCallbackFactory().Create(this, () => this.viewModel.Project = new Project()),
                Project = new Project()
            };
        }

        [TearDown]
        public void TearDown()
        {
            this.context.CleanContext();
        }

        [Test]
        public void VerifyComponent()
        {
            var renderer = this.context.RenderComponent<ProjectCreation>(parameters =>
            {
                parameters.Add(p => p.ViewModel, this.viewModel);
                parameters.AddCascadingValue(this.errorMessage);
            });

            var inputs = renderer.FindComponents<DxTextBox>();
            Assert.That(inputs.Count(), Is.EqualTo(1));

            this.viewModel.Project.ProjectName = "Project";

            renderer.Render();
            Assert.That(inputs[0].Instance.Text, Is.EqualTo(this.viewModel.Project.ProjectName));

            var formSubmit = renderer.FindComponent<EditForm>();
            formSubmit.Instance.OnValidSubmit.InvokeAsync();
            Assert.That(this.viewModel.Project.ProjectName, Is.Null);
        }
    }
}
