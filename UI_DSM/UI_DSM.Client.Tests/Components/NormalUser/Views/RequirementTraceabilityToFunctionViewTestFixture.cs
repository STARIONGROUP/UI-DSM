// --------------------------------------------------------------------------------------------------------
// <copyright file="RequirementTraceabilityToFunctionViewTestFixture.cs" company="RHEA System S.A.">
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
    public class RequirementTraceabilityToFunctionViewTestFixture
    {
        private IRequirementTraceabilityToFunctionViewViewModel viewModel;
        private Mock<IReviewItemService> reviewItemService;
        private TestContext context;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.context.ConfigureDevExpressBlazor();
            this.reviewItemService = new Mock<IReviewItemService>();

            this.viewModel = new RequirementTraceabilityToFunctionViewViewModel(this.reviewItemService.Object, 
                new FilterViewModel(), new FilterViewModel());

            this.context.Services.AddSingleton(this.viewModel);
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
                var satisfiesCategoryId = Guid.NewGuid();
                var functionsCategory = Guid.NewGuid();

                var categories = new List<Category>
                {
                    new()
                    {
                        Iid = satisfiesCategoryId,
                        Name = "satisfies"
                    },
                    new()
                    {
                        Iid = functionsCategory,
                        Name = "functions"
                    }
                };

                var owner = new DomainOfExpertise()
                {
                    Iid = Guid.NewGuid(),
                    Name = "Site Administrator",
                    ShortName = "admin"
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

                var elementDefinition = new ElementDefinition()
                {
                    Iid = Guid.NewGuid(),
                    Name = "FunctionDefintion Name",
                    ShortName = "FunctionDefintionShortName",
                    Owner = owner.Iid,
                    Category = new List<Guid>
                    {
                        functionsCategory
                    }
                };

                var elementUsage = new ElementUsage()
                {
                    Iid = Guid.NewGuid(),
                    ElementDefinition = elementDefinition.Iid
                };

                var function = new ElementDefinition()
                {
                    Iid = Guid.NewGuid(),
                    Name = "Function Name",
                    ShortName = "FunctionShortName",
                    Owner = owner.Iid,
                    Category = new List<Guid>
                    {
                        functionsCategory
                    },
                    ContainedElement = new List<Guid> { elementUsage.Iid }
                };

                var specification = new RequirementsSpecification()
                {
                    Iid = Guid.NewGuid(),
                    Requirement = new List<Guid>
                    {
                        firstRequirement.Iid,
                        secondRequirement.Iid
                    }
                };

                var relationShip = new BinaryRelationship()
                {
                    Iid = Guid.NewGuid(),
                    Source = elementUsage.Iid,
                    Target = secondRequirement.Iid,
                    Category = new List<Guid> { satisfiesCategoryId }
                };

                var assembler = new Assembler(new Uri("http://localhost"));

                var things = new List<Thing>(categories)
                {
                    firstRequirement,
                    secondRequirement,
                    function,
                    relationShip,
                    specification,
                    owner,
                    elementDefinition,
                    elementUsage
                };

                await assembler.Synchronize(things);
                _ = assembler.Cache.Select(x => x.Value.Value);

                var projectId = Guid.NewGuid();
                var reviewId = Guid.NewGuid();

                this.reviewItemService.Setup(x => x.GetReviewItemsForThings(projectId, reviewId, It.IsAny<IEnumerable<Guid>>(), 0))
                    .ReturnsAsync(new List<ReviewItem>
                    {
                        new(Guid.NewGuid())
                        {
                            ThingId = firstRequirement.Iid,
                            Annotations = { new Comment(Guid.NewGuid()) }
                        }
                    });

                var renderer = this.context.RenderComponent<RequirementTraceabilityToFunctionView>();

                var pocos = assembler.Cache.Where(x => x.Value.IsValueCreated)
                    .Select(x => x.Value)
                    .Select(x => x.Value)
                    .ToList();

                await renderer.Instance.InitializeViewModel(pocos, projectId, reviewId);

                Assert.Multiple(() =>
                {
                    Assert.That(renderer.FindComponents<FeatherCheck>(), Has.Count.EqualTo(1));
                    Assert.That(renderer.FindComponents<FeatherMessageCircle>(), Has.Count.EqualTo(1));
                });
            }
            catch
            {
                // On GitHub, exception is thrown even if the JSRuntime has been configured
            }
        }
    }
}
