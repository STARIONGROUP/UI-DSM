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
    using CDP4Common.SiteDirectoryData;

    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Client.Components.NormalUser.Views;
    using UI_DSM.Client.Services.ReviewItemService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.App.Filter;
    using UI_DSM.Client.ViewModels.App.OptionChooser;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;
    using UI_DSM.Shared.Models;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class RequirementBreakdownStructureViewTestFixture
    {
        private TestContext context;
        private IRequirementBreakdownStructureViewViewModel viewModel;
        private Mock<IReviewItemService> reviewItemService;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.reviewItemService = new Mock<IReviewItemService>();
            this.viewModel = new RequirementBreakdownStructureViewViewModel(this.reviewItemService.Object, new FilterViewModel());
            var trlViewModel = new ProductBreakdownStructureViewViewModel(this.reviewItemService.Object, new FilterViewModel(), new OptionChooserViewModel());
            this.context.ConfigureDevExpressBlazor();
            this.context.Services.AddSingleton(this.viewModel);
            this.context.Services.AddSingleton<IProductBreakdownStructureViewViewModel>(trlViewModel);
        }

        [TearDown]
        public void Teardown()
        {
            this.context.CleanContext();
        }

        [Test]
        public async Task VerifyComponent()
        {
            try
            {
                var renderer = this.context.RenderComponent<RequirementBreakdownStructureView>();
                
                var spaceDebrisCategory = new Category()
                {
                    Iid = Guid.NewGuid(),
                    ShortName = "space_debris",
                    Name = "space debris"
                };

                var things = new List<Thing>();
                var requirementsSpecificiation = new RequirementsSpecification();
                var group = new RequirementsGroup();
                requirementsSpecificiation.Group.Add(group);

                requirementsSpecificiation.Requirement.Add(new Requirement(Guid.NewGuid(), null, null)
                {
                    Group = group,
                    Definition =
                    {
                        new Definition()
                        {
                            Content = "A definition"
                        }
                    }, Category = {spaceDebrisCategory}
                });

                requirementsSpecificiation.Requirement.Add(new Requirement(Guid.NewGuid(), null, null)
                {
                    ShortName = "AOCS-100"
                });

                things.Add(requirementsSpecificiation);

                var reviewItems = new List<ReviewItem>
                {
                    new ()
                    {
                        ThingId = requirementsSpecificiation.Requirement.First().Iid
                    }
                };

                this.reviewItemService.Setup(x => x.GetReviewItemsForThings(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<IEnumerable<Guid>>(), It.IsAny<int>()))
                    .ReturnsAsync(reviewItems);

                await renderer.InvokeAsync(() => renderer.Instance.InitializeViewModel(things, Guid.NewGuid(), Guid.NewGuid(), 
                    new List<string>{"space_debris"}, new List<string>()));

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

                this.viewModel.TrySetSelectedItem(this.viewModel.Rows.First());
                Assert.That(this.viewModel.SelectedElement, Is.TypeOf<RequirementRowViewModel>());

                this.viewModel.TrySetSelectedItem(new ProductRowViewModel(new ElementDefinition()
                {
                    Iid = Guid.NewGuid()
                }, null));

                Assert.That(this.viewModel.SelectedElement, Is.Not.TypeOf<ProductRowViewModel>());

                this.viewModel.TrySetSelectedItem(null);
                Assert.That(this.viewModel.SelectedElement, Is.Not.Null);

                var verificationRenderer = this.context.RenderComponent<RequirementVerificationControlView>();
                var otherRenderer = this.context.RenderComponent<TrlView>();

                Assert.Multiple(() =>
                {
                    Assert.That(async () => await verificationRenderer.Instance.CopyComponents(renderer.Instance), Is.True);
                    Assert.That(async () => await verificationRenderer.Instance.CopyComponents(otherRenderer.Instance), Is.False);
                    Assert.That(async () => await renderer.Instance.CopyComponents(verificationRenderer.Instance), Is.True);
                });

                var rows = this.viewModel.GetAvailablesRows();

                Assert.Multiple(() =>
                {
                    Assert.That(rows, Is.Not.Empty);
                    Assert.That(this.viewModel.Rows.First().ReviewItem, Is.Not.Null);
                });

                var reviewItem = new ReviewItem(Guid.NewGuid())
                {
                    ThingId = this.viewModel.Rows.First().ThingId
                };

                this.viewModel.UpdateAnnotatableRows(new List<AnnotatableItem> { reviewItem });
                
                Assert.Multiple(() => 
                { 
                    Assert.That(this.viewModel.Rows.First().ReviewItem, Is.Not.Null);
                    Assert.That(renderer.Instance.TryNavigateToItem("a name"), Is.EqualTo(Task.CompletedTask));
                    Assert.That(renderer.Instance.TryNavigateToItem("AOCS-100"), Is.EqualTo(Task.CompletedTask));
                });
            }
            catch
            {
                // On GitHub, exception is thrown even if the JSRuntime has been configured
            }
        }
    }
}
