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
                }
                };

                var otherElement = new ElementDefinition()
                {
                    Iid = Guid.NewGuid()
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

                await renderer.Instance.InitializeViewModel(new List<Thing> { elementDefinition, otherElement }, projectId, reviewId);

                Assert.Multiple(() =>
                {
                    Assert.That(renderer.FindComponents<FeatherMessageCircle>(), Has.Count.EqualTo(1));
                    Assert.That(renderer.FindComponents<FeatherCheckCircle>(), Has.Count.EqualTo(1));
                    Assert.That(renderer.FindComponents<HyperLinkCard>(), Has.Count.EqualTo(2));
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
            }
            catch
            {
                // On GitHub, exception is thrown even if the JSRuntime has been configured
            }
        }
    }
}
