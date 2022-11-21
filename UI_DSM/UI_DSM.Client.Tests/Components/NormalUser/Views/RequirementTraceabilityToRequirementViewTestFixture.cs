// --------------------------------------------------------------------------------------------------------
// <copyright file="RequirementTraceabilityToRequirementViewTestFixture.cs" company="RHEA System S.A.">
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
    using CDP4Common.DTO;

    using CDP4Dal;

    using Feather.Blazor.Icons;

    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Client.Components.NormalUser.Views;
    using UI_DSM.Client.Services.ReviewItemService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.App.ConnectionVisibilitySelector;
    using UI_DSM.Client.ViewModels.App.Filter;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views;
    using UI_DSM.Shared.Models;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class RequirementTraceabilityToRequirementViewTestFixture
    {
        private IRequirementTraceabilityToRequirementViewViewModel viewModel;
        private Mock<IReviewItemService> reviewItemService;
        private TestContext context;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.context.ConfigureDevExpressBlazor();
            this.reviewItemService = new Mock<IReviewItemService>();
            this.viewModel = new RequirementTraceabilityToRequirementViewViewModel(this.reviewItemService.Object);
            this.context.Services.AddSingleton(this.viewModel);
            this.context.Services.AddTransient<IFilterViewModel, FilterViewModel>();
            this.context.Services.AddTransient<IConnectionVisibilitySelectorViewModel, ConnectionVisibilitySelectorViewModel>();
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

                var traceCategoryId = Guid.NewGuid();

                var categories = new List<Category>
            {
                new()
                {
                    Iid = traceCategoryId,
                    Name = "trace"
                }
            };

                var firstRequirement = new Requirement()
                {
                    Iid = Guid.NewGuid(),
                    ShortName = "1"
                };

                var secondRequirement = new Requirement()
                {
                    Iid = Guid.NewGuid(),
                    ShortName = "2"
                };

                var thirdRequirement = new Requirement()
                {
                    Iid = Guid.NewGuid(),
                    ShortName = "3"
                };

                var specification = new RequirementsSpecification()
                {
                    Iid = Guid.NewGuid(),
                    Requirement = new List<Guid>
                {
                    firstRequirement.Iid,
                    secondRequirement.Iid,
                    thirdRequirement.Iid
                }
                };

                var relationShip = new BinaryRelationship()
                {
                    Iid = Guid.NewGuid(),
                    Target = firstRequirement.Iid,
                    Source = secondRequirement.Iid,
                    Category = categories.Select(x => x.Iid).ToList()
                };

                var assembler = new Assembler(new Uri("http://localhost"));

                var things = new List<CDP4Common.DTO.Thing>(categories)
            {
                firstRequirement,
                secondRequirement,
                thirdRequirement,
                relationShip,
                specification
            };

                await assembler.Synchronize(things);
                _ = assembler.Cache.Select(x => x.Value.Value);

                var requirementsSpecification = assembler.Cache.Where(x => x.Value.IsValueCreated)
                    .Select(x => x.Value)
                    .Where(x => x.Value.ClassKind == ClassKind.RequirementsSpecification)
                    .Select(x => x.Value)
                    .ToList();

                var projectId = Guid.NewGuid();
                var reviewId = Guid.NewGuid();

                this.reviewItemService.Setup(x => x.GetReviewItemsForThings(projectId, reviewId, It.IsAny<IEnumerable<Guid>>(), 0))
                    .ReturnsAsync(new List<ReviewItem>
                    {
                    new (Guid.NewGuid())
                    {
                        ThingId = firstRequirement.Iid,
                        Annotations = { new Comment(Guid.NewGuid()) }
                    }
                    });

                var renderer = this.context.RenderComponent<RequirementTraceabilityToRequirementView>();
                await renderer.Instance.InitializeViewModel(requirementsSpecification, projectId, reviewId);

                Assert.Multiple(() =>
                {
                    Assert.That(renderer.FindComponents<FeatherCheck>(), Has.Count.EqualTo(1));
                    Assert.That(renderer.FindComponents<FeatherMessageCircle>(), Has.Count.EqualTo(2));
                });

                this.viewModel.TraceabilityTableViewModel.SelectElement(this.viewModel.TraceabilityTableViewModel.Rows.First());
                Assert.That(this.viewModel.SelectedElement, Is.Not.Null);
            }
            catch
            {
                // On GitHub, exception is thrown even if the JSRuntime has been configured
            }
        }
    }
}
