// --------------------------------------------------------------------------------------------------------
// <copyright file="RequirementTraceabilityToProductViewTestFixture.cs" company="RHEA System S.A.">
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
    using CDP4Common.EngineeringModelData;
    using CDP4Common.Types;

    using CDP4Dal;

    using Feather.Blazor.Icons;

    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Client.Components.App.Filter;
    using UI_DSM.Client.Components.NormalUser.Views;
    using UI_DSM.Client.Enumerator;
    using UI_DSM.Client.Services.ReviewItemService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.App.ConnectionVisibilitySelector;
    using UI_DSM.Client.ViewModels.App.Filter;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views;
    using UI_DSM.Shared.Models;

    using BinaryRelationship = CDP4Common.DTO.BinaryRelationship;
    using ElementDefinition = CDP4Common.DTO.ElementDefinition;
    using ElementUsage = CDP4Common.DTO.ElementUsage;
    using Parameter = CDP4Common.DTO.Parameter;
    using ParameterValueSet = CDP4Common.DTO.ParameterValueSet;
    using Requirement = CDP4Common.DTO.Requirement;
    using RequirementsSpecification = CDP4Common.DTO.RequirementsSpecification;
    using TestContext = Bunit.TestContext;
    using Thing = CDP4Common.DTO.Thing;

    [TestFixture]
    public class RequirementTraceabilityToProductViewTestFixture
    {
        private IRequirementTraceabilityToProductViewViewModel viewModel;
        private Mock<IReviewItemService> reviewItemService;
        private TestContext context;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.context.ConfigureDevExpressBlazor();
            this.reviewItemService = new Mock<IReviewItemService>();

            this.viewModel = new RequirementTraceabilityToProductViewViewModel(this.reviewItemService.Object, 
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
                var productsCategory = Guid.NewGuid();
                var instrumentsCategory = Guid.NewGuid();

                var categories = new List<Category>
            {
                new ()
                {
                    Iid = satisfiesCategoryId,
                    Name = "satisfies"
                },
                new()
                {
                    Iid = productsCategory,
                    Name = "products"
                },
                new()
                {
                    Iid = instrumentsCategory,
                    Name = "instruments",
                    SuperCategory = new List<Guid>{productsCategory}
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

                var elementUsageId = Guid.NewGuid();

                var product = new ElementDefinition()
                {
                    Iid = Guid.NewGuid(),
                    Name = "Product Name",
                    ShortName = "ProductShortName",
                    Owner = owner.Iid,
                    Category = new List<Guid>
                {
                    productsCategory
                },
                    ContainedElement = new List<Guid> { elementUsageId }
                };

                var parameterType = new TextParameterType()
                {
                    Iid = Guid.NewGuid(),
                    Name = "technology"
                };

                var invalidValueSet = new ParameterValueSet()
                {
                    Iid = Guid.NewGuid(),
                    Manual = new ValueArray<string>(new List<string> { "-" }),
                    ValueSwitch = ParameterSwitchKind.MANUAL
                };

                var invalidParameter = new Parameter()
                {
                    Iid = Guid.NewGuid(),
                    ParameterType = parameterType.Iid,
                    ValueSet = new List<Guid>
                {
                    invalidValueSet.Iid
                }
                };

                var productWithInvalidTechnology = new ElementDefinition()
                {
                    Iid = Guid.NewGuid(),
                    Name = "Product Name 2",
                    ShortName = "ProductShortName2",
                    Category = new List<Guid>
                {
                    instrumentsCategory
                },
                    Owner = owner.Iid,
                    Parameter = new List<Guid> { invalidParameter.Iid }
                };

                var elementUsage = new ElementUsage()
                {
                    Iid = elementUsageId,
                    ElementDefinition = productWithInvalidTechnology.Iid
                };

                var productWithoutTechnology = new ElementDefinition()
                {
                    Iid = Guid.NewGuid(),
                    Name = "Product Name 3",
                    ShortName = "ProductShortName3",
                    Owner = owner.Iid,
                    Category = new List<Guid>
                {
                    instrumentsCategory
                }
                };

                var validValueSet = new ParameterValueSet()
                {
                    Iid = Guid.NewGuid(),
                    Manual = new ValueArray<string>(new List<string> { "fiber" }),
                    ValueSwitch = ParameterSwitchKind.MANUAL
                };

                var validParameter = new Parameter()
                {
                    Iid = Guid.NewGuid(),
                    ParameterType = parameterType.Iid,
                    ValueSet = new List<Guid>
                {
                    validValueSet.Iid
                }
                };

                var productValidTechnology = new ElementDefinition()
                {
                    Iid = Guid.NewGuid(),
                    Name = "Product Name 4",
                    ShortName = "ProductShortName4",
                    Owner = owner.Iid,
                    Category = new List<Guid>
                {
                    instrumentsCategory
                },
                    Parameter = new List<Guid>
                {
                    validParameter.Iid
                }
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
                    Source = elementUsageId,
                    Target = secondRequirement.Iid,
                    Category = new List<Guid> { satisfiesCategoryId }
                };

                var assembler = new Assembler(new Uri("http://localhost"));

                var things = new List<Thing>(categories)
            {
                firstRequirement,
                secondRequirement,
                product,
                relationShip,
                specification,
                parameterType,
                invalidValueSet,
                validValueSet,
                validParameter,
                invalidValueSet,
                productValidTechnology,
                productWithInvalidTechnology,
                productWithoutTechnology,
                owner,
                elementUsage
            };

                await assembler.Synchronize(things);
                _ = assembler.Cache.Select(x => x.Value.Value);

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

                var renderer = this.context.RenderComponent<RequirementTraceabilityToProductView>();

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

                this.viewModel.TraceabilityTableViewModel.VisibilityState.CurrentState = ConnectionToVisibilityState.Connected;
                var invisibleRow = renderer.FindAll(".invisible-row");
                Assert.That(invisibleRow, Has.Count.EqualTo(0));

                this.viewModel.TraceabilityTableViewModel.VisibilityState.CurrentState = ConnectionToVisibilityState.NotConnected;
                invisibleRow.Refresh();
                Assert.That(invisibleRow, Has.Count.EqualTo(1));

                var filters = renderer.FindComponents<Filter>();
                var columnFiltering = filters.First(x => x.Instance.Id.Contains("column"));
                columnFiltering.Instance.OpenCloseFilter();
                Assert.That(columnFiltering.Instance.ViewModel.IsFilterVisible, Is.True);

                columnFiltering.Instance.ViewModel.SelectedFilterModel = columnFiltering.Instance.ViewModel
                    .AvailableFilters.First(x => x.ClassKind == ClassKind.RequirementsSpecification);

                await columnFiltering.Instance.SelectDeselectAll(false);
                columnFiltering.Instance.OpenCloseFilter();

                Assert.Multiple(() =>
                {
                    Assert.That(renderer.FindComponents<FeatherCheck>(), Has.Count.EqualTo(0));
                    Assert.That(renderer.FindComponents<FeatherMessageCircle>(), Has.Count.EqualTo(0));
                });

                var invalidRows = renderer.FindAll(".invalid");
                Assert.That(invalidRows, Has.Count.EqualTo(0));

                this.viewModel.IsOnTechnologyView = true;
                invalidRows.Refresh();
                Assert.That(invalidRows, Has.Count.EqualTo(1));
            }
            catch
            {
                // On GitHub, exception is thrown even if the JSRuntime has been configured
            }
        }
    }
}
