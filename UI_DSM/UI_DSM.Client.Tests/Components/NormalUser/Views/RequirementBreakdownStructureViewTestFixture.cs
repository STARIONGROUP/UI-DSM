// --------------------------------------------------------------------------------------------------------
// <copyright file="RequirementBreakdownStructureViewTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Components.NormalUser.Views
{
    using Bunit;

    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;

    using Microsoft.Extensions.DependencyInjection;

    using NUnit.Framework;

    using UI_DSM.Client.Components.NormalUser.Views;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class RequirementBreakdownStructureViewTestFixture
    {
        private TestContext context;
        private IRequirementBreakdownStructureViewViewModel viewModel;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.viewModel = new RequirementBreakdownStructureViewViewModel();
            this.context.ConfigureDevExpressBlazor();
            this.context.Services.AddSingleton(this.viewModel);
        }

        [TearDown]
        public void Teardown()
        {
            this.context.CleanContext();
        }

        [Test]
        public async Task VerifyComponent()
        {
            var renderer = this.context.RenderComponent<RequirementBreakdownStructureView>();

            var things = new List<Thing>();
            var requirementsSpecificiation = new RequirementsSpecification();
            var group = new RequirementsGroup();
            requirementsSpecificiation.Group.Add(group);
            
            requirementsSpecificiation.Requirement.Add(new Requirement()
            {
                Group = group,
                Definition = 
                {
                    new Definition()
                    {
                        Content = "A definition"
                    }
                }
            });

            requirementsSpecificiation.Requirement.Add(new Requirement());

            things.Add(requirementsSpecificiation);

            await renderer.InvokeAsync(() => renderer.Instance.InitializeViewModel(things));

            Assert.Multiple(() =>
            {
                Assert.That(this.viewModel.Things, Is.Not.Empty);
                Assert.That(this.viewModel.SelectedElement, Is.Null);
            });

            this.viewModel.SelectedElement = group;

            Assert.Multiple(() =>
            {
                Assert.That(this.viewModel.SelectedElement, Is.Not.Null);
                Assert.That(renderer.Instance.SelectedItemObservable, Is.Not.Null);
            });
        }
    }
}
