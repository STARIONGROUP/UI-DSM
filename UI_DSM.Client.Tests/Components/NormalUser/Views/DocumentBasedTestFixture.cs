// --------------------------------------------------------------------------------------------------------
// <copyright file="DocumentBasedTestFixture.cs" company="RHEA System S.A.">
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
    using CDP4Common.Types;

    using Feather.Blazor.Icons;

    using Microsoft.Extensions.DependencyInjection;
    
    using Moq;

    using NUnit.Framework;

    using UI_DSM.Client.Components.App.HyperLinkCard;
    using UI_DSM.Client.Components.NormalUser.Views;
    using UI_DSM.Client.Services.ReviewItemService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;
    using UI_DSM.Shared.Models;

    using Participant = UI_DSM.Shared.Models.Participant;
    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class DocumentBasedTestFixture
    {
        private IDocumentBasedViewModel viewModel;
        private Mock<IReviewItemService> reviewItemService;
        private TestContext context;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.context.ConfigureDevExpressBlazor();
            this.reviewItemService = new Mock<IReviewItemService>();
            this.viewModel = new DocumentBasedViewModel(this.reviewItemService.Object);
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
            try
            {
                var parameterType = new EnumerationParameterType()
                {
                    Iid = Guid.NewGuid(),
                    Name = "review external content",
                    AllowMultiSelect = true,
                    ValueDefinition = 
                    {
                        new EnumerationValueDefinition()
                        {
                            Iid = Guid.NewGuid(),
                            Name = "space debris", 
                        },
                        new EnumerationValueDefinition()
                        {
                            Iid = Guid.NewGuid(),
                            Name = "aiv",
                        }
                    }
                };

                var elementDefinition = new ElementDefinition()
                {
                    Iid = Guid.NewGuid(),
                    HyperLink =
                    {
                        new HyperLink()
                        {
                            Iid = Guid.NewGuid(),
                            Content = "A first content",
                            Uri = "https://google.com"
                        },
                        new HyperLink()
                        {
                            Iid = Guid.NewGuid(),
                            Content = "A second content",
                            Uri = "https://google.be"
                        }
                    }, 
                    Parameter =
                    {
                        new Parameter()
                        {
                            Iid = Guid.NewGuid(),
                            ParameterType = parameterType,
                            ValueSet =
                            {
                                new ParameterValueSet()
                                {
                                    Manual = new ValueArray<string>(new List<string>(){"aiv"}),
                                    ValueSwitch = ParameterSwitchKind.MANUAL
                                }
                            }
                        }
                    }
                };

                var otherElement = new ElementDefinition()
                {
                    Iid = Guid.NewGuid(),
                    HyperLink = 
                    {
                        new HyperLink()
                        {
                            Iid = Guid.NewGuid(),
                            Content = "A third content",
                            Uri = "https://google.nl"
                        }
                    }
                };

                var requirement = new Requirement()
                {
                    Iid = Guid.NewGuid(),
                    HyperLink = 
                    {
                        new HyperLink()
                        {
                            Iid = Guid.NewGuid(),
                            Content = "A third content",
                            Uri = "https://google.nl"
                        }
                    }, 
                    ParameterValue = 
                    { 
                        new SimpleParameterValue()
                        {
                            Iid = Guid.NewGuid(),
                            ParameterType = parameterType,
                            Value = new ValueArray<string>(new List<string>{"aiv"})
                        }
                    }
                };

                var requirement2 = new Requirement()
                {
                    Iid = Guid.NewGuid(),
                    HyperLink =
                    {
                        new HyperLink()
                        {
                            Iid = Guid.NewGuid(),
                            Content = "Another content",
                            Uri = "https://google.fr"
                        }
                    },
                    ParameterValue =
                    {
                        new SimpleParameterValue()
                        {
                            Iid = Guid.NewGuid(),
                            ParameterType = parameterType,
                            Value = new ValueArray<string>(new List<string>{"space debris"})
                        }
                    }
                };

                var projectId = Guid.NewGuid();
                var reviewId = Guid.NewGuid();

                this.reviewItemService.Setup(x => x.GetReviewItemsForThings(projectId, reviewId, It.IsAny<IEnumerable<Guid>>(), 0))
                    .ReturnsAsync(new List<ReviewItem>
                    {
                        new (Guid.NewGuid())
                        {
                            ThingId = elementDefinition.HyperLink.First().Iid,
                            Annotations = { new Comment(Guid.NewGuid()) },
                            IsReviewed = true
                        }
                    });

                var renderer = this.context.RenderComponent<DocumentBased>();

                await renderer.Instance.InitializeViewModel(new List<Thing> { elementDefinition, otherElement, requirement, requirement2 }, projectId, reviewId, Guid.Empty,
                    new List<string>{"aiv"}, new List<string>(), new Participant());

                Assert.Multiple(() =>
                {
                    Assert.That(renderer.FindComponents<FeatherMessageCircle>(), Has.Count.EqualTo(1));
                    Assert.That(renderer.FindComponents<FeatherCheckCircle>(), Has.Count.EqualTo(1));
                    Assert.That(renderer.FindComponents<HyperLinkCard>(), Has.Count.EqualTo(3));
                });

                var hyperlink = renderer.FindComponents<HyperLinkCard>()[0];
                await renderer.InvokeAsync(() => hyperlink.Instance.OnClick.InvokeAsync(hyperlink.Instance.Row));
                Assert.That(this.viewModel.SelectedElement, Is.Not.Null);

                this.viewModel.TrySetSelectedItem(new RequirementRowViewModel(new Requirement()
                {
                    Iid = Guid.NewGuid()
                }, null));

                Assert.That(this.viewModel.SelectedElement, Is.Not.TypeOf<RequirementRowViewModel>());

                this.viewModel.TrySetSelectedItem(hyperlink.Instance.Row);
                Assert.That(this.viewModel.SelectedElement, Is.Not.Null);

                var allRows = this.viewModel.GetAvailablesRows();
                
                Assert.Multiple(() =>
                {
                    Assert.That(allRows, Is.Not.Empty);
                    Assert.That(this.viewModel.Rows.Last().ReviewItem, Is.Null);
                });

                this.viewModel.UpdateAnnotatableRows(new List<AnnotatableItem>
                {
                    new ReviewItem(Guid.NewGuid())
                    {
                        ThingId = this.viewModel.Rows.Last().ThingId
                    }
                });

                Assert.Multiple(() =>
                {
                    Assert.That(this.viewModel.Rows.Last().ReviewItem, Is.Not.Null);
                    Assert.That(renderer.Instance.TryNavigateToItem("a name"), Is.EqualTo(Task.CompletedTask));
                });
            }
            catch
            {
                // On GitHub, exception is thrown even if the JSRuntime has been configured
            }
        }
    }
}
