// --------------------------------------------------------------------------------------------------------
// <copyright file="ProjectDetailsTestFixture.cs" company="RHEA System S.A.">
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

    using Microsoft.AspNetCore.Components.Forms;
    using Microsoft.Extensions.DependencyInjection;

    using NUnit.Framework;

    using UI_DSM.Client.Components.Administration.ProjectManagement;
    using UI_DSM.Client.ViewModels.Components.Administration.ProjectManagement;
    using UI_DSM.Shared.Models;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class ProjectDetailsTestFixture
    {
        private TestContext context;
        private IProjectDetailsViewModel viewModel;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.context.Services.AddDevExpressBlazor();

            this.viewModel = new ProjectDetailsViewModel()
            {
                Project = new Project(Guid.NewGuid())
                {
                    ProjectName = "Project"
                }
            };
        }

        [TearDown]
        public void Teardown()
        {
            this.context.Dispose();
        }

        [Test]
        public void VerifyComponent()
        {
            var renderer = this.context.RenderComponent<ProjectDetails>(parameters =>
            {
                parameters.Add(p => p.ViewModel, this.viewModel);
            });

            var inputText = renderer.FindComponent<InputText>();
            Assert.That(inputText.Instance.Value, Is.EqualTo(this.viewModel.Project.ProjectName));
        }
    }
}
