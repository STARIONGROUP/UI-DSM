// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewCategoryModuleTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Tests.Modules
{
    using Microsoft.AspNetCore.Http;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Server.Managers;
    using UI_DSM.Server.Modules;
    using UI_DSM.Server.Services.SearchService;
    using UI_DSM.Server.Tests.Helpers;
    using UI_DSM.Server.Types;
    using UI_DSM.Server.Validator;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class ReviewCategoryModuleTestFixture
    {
        private ReviewCategoryModule module;
        private Mock<IEntityManager<ReviewCategory>> reviewCategoryManager;
        private List<ReviewCategory> reviewCategories;
        private Mock<HttpContext> httpContext;
        private Mock<HttpResponse> httpResponse;
        private Mock<ISearchService> searchService;

        [SetUp]
        public void Setup()
        {
            this.reviewCategories = new List<ReviewCategory>
            {
                new(Guid.NewGuid())
                {
                    ReviewCategoryName = "Category 1",
                    Description = "Description for category1",
                    TagColor = "#FFC300"
                }
            };

            this.reviewCategoryManager = new Mock<IEntityManager<ReviewCategory>>();
            this.reviewCategoryManager.Setup(x => x.GetEntities(0)).ReturnsAsync(this.reviewCategories);
            this.searchService = new Mock<ISearchService>();

            ModuleTestHelper.Setup<ReviewCategoryModule, ReviewCategoryDto>(new ReviewCategoryDtoValidator(), out this.httpContext, out this.httpResponse, out _, out _);
            this.module = new ReviewCategoryModule();
            Program.RegisterEntities();
        }

        [Test]
        public async Task VerifyGetReviewCategories()
        {
            await this.module.GetEntities(this.reviewCategoryManager.Object, this.httpContext.Object);
            this.reviewCategoryManager.Verify(x => x.GetEntities(0), Times.Once);

            this.reviewCategoryManager.Setup(x => x.GetEntity(this.reviewCategories.First().Id, 0)).ReturnsAsync(this.reviewCategories);

            var invalidGuid = Guid.NewGuid();
            this.reviewCategoryManager.Setup(x => x.GetEntity(invalidGuid, 0)).ReturnsAsync(Enumerable.Empty<ReviewCategory>());
            await this.module.GetEntity(this.reviewCategoryManager.Object, invalidGuid, this.httpContext.Object);
            this.httpResponse.VerifySet(x => x.StatusCode = 404, Times.Once);

            await this.module.GetEntity(this.reviewCategoryManager.Object, this.reviewCategories.First().Id, this.httpContext.Object);
            this.httpResponse.VerifySet(x => x.StatusCode = 404, Times.Once);
        }

        [Test]
        public async Task VerifyCreateReviewCategory()
        {
            var reviewCategoryDto = new ReviewCategoryDto();

            await this.module.CreateEntity(this.reviewCategoryManager.Object, reviewCategoryDto, this.searchService.Object, this.httpContext.Object);
            this.httpResponse.VerifySet(x => x.StatusCode = 422, Times.Once);
            reviewCategoryDto.ReviewCategoryName = "Category";
            reviewCategoryDto.Description = "Description";
            reviewCategoryDto.TagColor = "#72D33F";
            reviewCategoryDto.Id = Guid.NewGuid();

            await this.module.CreateEntity(this.reviewCategoryManager.Object, reviewCategoryDto, this.searchService.Object, this.httpContext.Object);
            this.httpResponse.VerifySet(x => x.StatusCode = 422, Times.Exactly(2));

            reviewCategoryDto.Id = Guid.Empty;

            this.reviewCategoryManager.Setup(x => x.CreateEntity(It.IsAny<ReviewCategory>()))
                .ReturnsAsync(EntityOperationResult<ReviewCategory>.Failed());

            await this.module.CreateEntity(this.reviewCategoryManager.Object, reviewCategoryDto, this.searchService.Object, this.httpContext.Object);
            this.httpResponse.VerifySet(x => x.StatusCode = 500, Times.Once);

            this.reviewCategoryManager.Setup(x => x.CreateEntity(It.IsAny<ReviewCategory>()))
                .ReturnsAsync(EntityOperationResult<ReviewCategory>.Success(new ReviewCategory(Guid.NewGuid())
                {
                    ReviewCategoryName = reviewCategoryDto.ReviewCategoryName
                }));

            await this.module.CreateEntity(this.reviewCategoryManager.Object, reviewCategoryDto, this.searchService.Object, this.httpContext.Object);
            this.httpResponse.VerifySet(x => x.StatusCode = 201, Times.Once);
        }

        [Test]
        public async Task VerifyDeleteReviewCategory()
        {
            var guid = Guid.NewGuid();
            this.reviewCategoryManager.Setup(x => x.FindEntity(guid)).ReturnsAsync((ReviewCategory)null);

            var response = await this.module.DeleteEntity(this.reviewCategoryManager.Object, guid, this.searchService.Object, this.httpContext.Object);
            Assert.That(response.IsRequestSuccessful, Is.False);
            this.httpResponse.VerifySet(x => x.StatusCode = 404, Times.Once);

            var reviewCategory = new ReviewCategory(guid)
            {
                ReviewCategoryName = "Category"
            };

            this.reviewCategoryManager.Setup(x => x.FindEntity(guid)).ReturnsAsync(reviewCategory);
            this.reviewCategoryManager.Setup(x => x.DeleteEntity(reviewCategory)).ReturnsAsync(EntityOperationResult<ReviewCategory>.Failed());
            response = await this.module.DeleteEntity(this.reviewCategoryManager.Object, guid, this.searchService.Object, this.httpContext.Object);
            Assert.That(response.IsRequestSuccessful, Is.False);
            this.httpResponse.VerifySet(x => x.StatusCode = 500, Times.Once);

            this.reviewCategoryManager.Setup(x => x.DeleteEntity(reviewCategory)).ReturnsAsync(EntityOperationResult<ReviewCategory>.Success(reviewCategory));
            response = await this.module.DeleteEntity(this.reviewCategoryManager.Object, guid, this.searchService.Object, this.httpContext.Object);
            Assert.That(response.IsRequestSuccessful, Is.True);
        }

        [Test]
        public async Task VerifyUpdateReviewCategory()
        {
            var reviewCategory = new ReviewCategory(Guid.NewGuid());
            this.reviewCategoryManager.Setup(x => x.FindEntity(reviewCategory.Id)).ReturnsAsync((ReviewCategory)null);
            await this.module.UpdateEntity(this.reviewCategoryManager.Object, reviewCategory.Id, (ReviewCategoryDto)reviewCategory.ToDto(), this.searchService.Object, this.httpContext.Object);
            this.httpResponse.VerifySet(x => x.StatusCode = 404, Times.Once);

            this.reviewCategoryManager.Setup(x => x.FindEntity(reviewCategory.Id)).ReturnsAsync(reviewCategory);
            await this.module.UpdateEntity(this.reviewCategoryManager.Object, reviewCategory.Id, (ReviewCategoryDto)reviewCategory.ToDto(), this.searchService.Object, this.httpContext.Object);
            this.httpResponse.VerifySet(x => x.StatusCode = 422, Times.Once);

            reviewCategory.ReviewCategoryName = "Category";
            reviewCategory.Description = "Description1";
            reviewCategory.TagColor = "#3F55D3";
            this.reviewCategoryManager.Setup(x => x.UpdateEntity(It.IsAny<ReviewCategory>())).ReturnsAsync(EntityOperationResult<ReviewCategory>.Failed());

            await this.module.UpdateEntity(this.reviewCategoryManager.Object, reviewCategory.Id, (ReviewCategoryDto)reviewCategory.ToDto(), this.searchService.Object, this.httpContext.Object);
            this.httpResponse.VerifySet(x => x.StatusCode = 500, Times.Once);

            this.reviewCategoryManager.Setup(x => x.UpdateEntity(It.IsAny<ReviewCategory>())).ReturnsAsync(EntityOperationResult<ReviewCategory>.Success(reviewCategory));
            await this.module.UpdateEntity(this.reviewCategoryManager.Object, reviewCategory.Id, (ReviewCategoryDto)reviewCategory.ToDto(), this.searchService.Object, this.httpContext.Object);
            this.httpResponse.VerifySet(x => x.StatusCode = 200, Times.Once);
        }
    }
}
