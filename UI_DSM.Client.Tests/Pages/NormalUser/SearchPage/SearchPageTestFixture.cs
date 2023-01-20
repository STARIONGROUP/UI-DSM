// --------------------------------------------------------------------------------------------------------
// <copyright file="SearchPageTestFixture.cs" company="RHEA System S.A.">
//  Copyright (c) 2023 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw, Nabil Abbar
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Client.Tests.Pages.NormalUser.SearchPage
{
    using Bunit;

    using Microsoft.AspNetCore.Components;
    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Client.Components.App.SearchResultCard;
    using UI_DSM.Client.Pages.NormalUser.SearchPage;
    using UI_DSM.Client.Services.SearchService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.Pages.NormalUser.SearchPage;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.Enumerator;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class SearchPageTestFixture
    {
        private TestContext context;
        private ISearchPageViewModel viewModel;
        private Mock<ISearchService> searchService;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.searchService = new Mock<ISearchService>();
            this.viewModel = new SearchPageViewModel(this.searchService.Object, null);
            this.context.Services.AddSingleton(this.viewModel);
            this.context.ConfigureDevExpressBlazor();
            this.viewModel.NavigationManager = this.context.Services.GetService<NavigationManager>();
        }

        [TearDown]
        public void Teardown()
        {
            this.context.CleanContext();
        }

        [Test]
        public async Task VerifyPage()
        {
            var searchDtos = new List<SearchResultDto>
            {
                new ()
                {
                    BaseUrl = "http://localhost/Project/1",
                    DisplayText = "Project 1",
                    ObjectKind = "Project"
                },
                new ()
                {
                    BaseUrl = "http://localhost/Project/1/Model/1",
                    DisplayText = "Product 1",
                    ObjectKind = "ElementDefinition",
                    Location = "Project 1 > Model 1",
                    SpecificCategory = "products",
                    AvailableViews = new List<View>
                    {
                        View.FunctionalTraceabilityToProductView,
                        View.ProductBreakdownStructureView
                    }
                }
            };

            this.searchService.Setup(x => x.SearchAfter("pro")).ReturnsAsync(searchDtos);

            var renderer = this.context.RenderComponent<SearchPage>(parameters =>
            {
                parameters.Add(p => p.Keyword, "pro");
            });

            var cards = renderer.FindComponents<SearchResultCard>();
            Assert.That(cards, Has.Count.EqualTo(2));
            await renderer.InvokeAsync(() => cards[0].Instance.OnClick.InvokeAsync(cards[0].Instance.SearchResult));
            Assert.That(this.viewModel.NavigationManager.Uri, Is.EqualTo("http://localhost/Project/1"));

            await renderer.InvokeAsync(() => cards[1].Instance.OnClick.InvokeAsync(cards[1].Instance.SearchResult));
            Assert.That(this.viewModel.IsOnViewSelectionMode, Is.True);
            var button = renderer.Find(".view__button");
            await renderer.InvokeAsync(() => button.Click());
            Assert.That(this.viewModel.NavigationManager.Uri, Is.EqualTo($"http://localhost/Project/1/Model/1?view=FunctionalTraceabilityToProductView&id={Guid.Empty}"));
        }
    }
}
