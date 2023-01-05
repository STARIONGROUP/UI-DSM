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

    using CDP4Common.DTO;

    using CDP4Dal;

    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Web;
    using Microsoft.Extensions.DependencyInjection;

    using NUnit.Framework;

    using UI_DSM.Client.Components.App.AppAccordion;
    using UI_DSM.Client.Components.App.AppKeyValue;
    using UI_DSM.Client.Components.App.AppKeyValues;
    using UI_DSM.Client.Components.App.HaveThingRowLinked;
    using UI_DSM.Client.Components.App.SelectedItemCard;
    using UI_DSM.Client.Components.App.SelectedItemCard.SelectedItemCardContent;
    using UI_DSM.Client.Extensions;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.App.SelectedItemCard;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;
    using UI_DSM.Shared.Models;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class SelectedItemCardTestFixture
    {
        private ISelectedItemCardViewModel viewModel;
        private TestContext context;
        private string callbackCalled;

        [SetUp]
        public void Setup()
        {
            this.viewModel = new SelectedItemCardViewModel();
            this.context = new TestContext();
            this.context.ConfigureDevExpressBlazor();
            this.context.Services.AddSingleton(this.viewModel);
        }

        [TearDown]
        public void Teardown()
        {
            this.context.CleanContext();
        }

        [Test]
        public async Task VerifySetSelectedItem()
        {
            var renderer = this.context.RenderComponent<SelectedItemCard>(paramaters =>
            {
                paramaters.Add(p => p.OnItemDoubleClick, new EventCallbackFactory().Create<string>(this, (x) => this.callbackCalled = x));
            });

            Assert.That(renderer.FindComponents<DynamicComponent>(), Is.Empty);

            this.viewModel.SelectedItem = new ReviewTask()
            {
                Description = "A Description"
            };

            Assert.That(renderer.FindComponent<ReviewTaskSelectedItem>(), Is.Not.Null);

            var owner = new DomainOfExpertise()
            {
                Iid = Guid.NewGuid(),
                ShortName = "SYS"
            };

            var requirementDto = new Requirement()
            {
                Iid = Guid.NewGuid(),
                Owner = owner.Iid,
                ShortName = "AOCS-010"
            };

            var tracedRequirement = new Requirement()
            {
                Iid = Guid.NewGuid(),
                Owner = owner.Iid, 
                ShortName = "MIS-010"
            };

            var productCategory = new Category()
            {
                Iid = Guid.NewGuid(),
                Name = ThingExtension.ProductCategoryName
            };

            var traceCategory = new Category()
            {
                Iid = Guid.NewGuid(),
                Name = ThingExtension.TraceCategoryName
            };

            var functionCategory = new Category()
            {
                Iid = Guid.NewGuid(),
                Name = ThingExtension.FunctionCategoryName
            };

            var satisfyCategory = new Category()
            {
                Iid = Guid.NewGuid(),
                Name = ThingExtension.SatisfyCategoryName
            };

            var elementDefinition = new ElementDefinition()
            {
                Iid = Guid.NewGuid(),
                Name = "Element"
            };

            var function = new ElementUsage()
            {
                Iid = Guid.NewGuid(),
                Category = new List<Guid>
                {
                    functionCategory.Iid
                },
                Name = "Function"
            };

            var product = new ElementUsage()
            {
                Iid = Guid.NewGuid(),
                Category = new List<Guid>
                {
                    productCategory.Iid
                },
                Name = "Product"
            };

            var traceRelationship = new BinaryRelationship()
            {
                Iid = Guid.NewGuid(),
                Category = new List<Guid> { traceCategory.Iid },
                Source = requirementDto.Iid,
                Target = tracedRequirement.Iid
            };

            var satisfiedByFunction = new BinaryRelationship()
            {
                Iid = Guid.NewGuid(),
                Category = new List<Guid> { satisfyCategory.Iid },
                Source = function.Iid,
                Target = requirementDto.Iid
            };

            var satisfiedByProduct = new BinaryRelationship()
            {
                Iid = Guid.NewGuid(),
                Category = new List<Guid> { satisfyCategory.Iid },
                Source = product.Iid,
                Target = requirementDto.Iid
            };

            var things = new List<Thing>
            {
                owner, requirementDto, tracedRequirement, productCategory, traceCategory, functionCategory, satisfyCategory, elementDefinition, 
                function, product, traceRelationship, satisfiedByFunction, satisfiedByProduct
            };

            var assembler = new Assembler(new Uri("http://localhost"));
            await assembler.Synchronize(things);
            _ = assembler.Cache.Select(x => x.Value.Value);

            this.viewModel.SelectedItem = new RequirementRowViewModel(assembler.Cache.First(x => x.Value.Value.Iid == requirementDto.Iid)
                .Value.Value as CDP4Common.EngineeringModelData.Requirement, null);

            Assert.That(renderer.FindComponent<RequirementSelectedItem>(), Is.Not.Null);

            var haveThingsRows = renderer.FindComponents<HaveThingRowLinkedBase>();
            var buttons = haveThingsRows[4].FindAll("button");

            await renderer.InvokeAsync(() => buttons[2].ClickAsync(new MouseEventArgs()));

            var appKeyValue = renderer.FindComponent<AppKeyValue>();
            var collectionAppAccordion = renderer.FindComponents<AppAccordion>();
            var collectionAppKeyValues = renderer.FindComponents<AppKeyValues>();
            var appKeyValues = collectionAppKeyValues[0];
            var appAccordion = collectionAppAccordion[0];

            Assert.Multiple(() =>
            {
                Assert.That(appKeyValue.Instance.Key, Is.EqualTo("ID"));
                Assert.That(appKeyValue.Instance.Value, Is.EqualTo(((IHaveThingRowViewModel)this.viewModel.SelectedItem).Id));
                Assert.That(appAccordion.Instance.PanelOpen, Is.True);
                Assert.That(() => appAccordion.Instance.PanelOpen = false, Throws.Nothing);
                Assert.That(appAccordion.Instance.PanelOpen, Is.False);
                Assert.That(appKeyValues.Instance.Items, Is.Empty);
            });

            var button = renderer.Find(".app-accordion__button");
            await renderer.InvokeAsync(() => button.Click());
            Assert.That(appAccordion.Instance.PanelOpen, Is.True);

            var isOpen = true;

            appAccordion.Instance.PanelOpenChanged = new EventCallbackFactory().Create<bool>(this, (x) => isOpen = x);
            await renderer.InvokeAsync(() => button.Click());

            Assert.That(isOpen, Is.False);
        }
    }
}
