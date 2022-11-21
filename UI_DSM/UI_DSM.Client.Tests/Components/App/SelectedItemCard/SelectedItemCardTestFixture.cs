// --------------------------------------------------------------------------------------------------------
// <copyright file="SelectedItemCardTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Components.App.SelectedItemCard
{
    using Bunit;

    using CDP4Common.EngineeringModelData;

    using Microsoft.AspNetCore.Components;
    using Microsoft.Extensions.DependencyInjection;

    using NUnit.Framework;

    using UI_DSM.Client.Components.App.SelectedItemCard;
    using UI_DSM.Client.Components.App.SelectedItemCard.SelectedItemCardContent;
    using UI_DSM.Client.ViewModels.App.SelectedItemCard;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;
    using UI_DSM.Shared.Models;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class SelectedItemCardTestFixture
    {
        private ISelectedItemCardViewModel viewModel;
        private TestContext context;

        [SetUp]
        public void Setup()
        {
            this.viewModel = new SelectedItemCardViewModel();
            this.context = new TestContext();
            this.context.Services.AddSingleton(this.viewModel);
        }

        [TearDown]
        public void Teardown()
        {
            this.context.Dispose();
        }

        [Test]
        public void VerifySetSelectedItem()
        {
            var renderer = this.context.RenderComponent<SelectedItemCard>();
            Assert.That(renderer.FindComponents<DynamicComponent>(), Is.Empty);

            this.viewModel.SelectedItem = new ReviewTask()
            {
                Description = "A Description"
            };

            Assert.That(renderer.FindComponent<ReviewTaskSelectedItem>(), Is.Not.Null);

            this.viewModel.SelectedItem = new RequirementRowViewModel(new Requirement(), null);
            Assert.That(renderer.FindComponent<RequirementSelectedItem>(), Is.Not.Null);
        }
    }
}
