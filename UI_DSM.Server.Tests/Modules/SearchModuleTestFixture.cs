// --------------------------------------------------------------------------------------------------------
// <copyright file="SearchModuleTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Tests.Modules
{
    using System.Security.Claims;

    using CDP4JsonSerializer;
    
    using Microsoft.AspNetCore.Http;
    
    using Moq;
    
    using NUnit.Framework;

    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Serializer.Json;
    using UI_DSM.Server.Managers.RoleManager;
    using UI_DSM.Server.Managers.UserManager;
    using UI_DSM.Server.Modules;
    using UI_DSM.Server.Services.ResolverService;
    using UI_DSM.Server.Services.SearchService;
    using UI_DSM.Server.Tests.Helpers;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class SearchModuleTestFixture
    {
        private SearchModule module;
        private Mock<HttpContext> context;
        private IJsonService jsonService;
        private Mock<ISearchService> searchService;
        private Mock<IResolverService> resolverService;
        private Mock<IRoleManager> roleManager;
        private Mock<IUserManager> userManager;

        [SetUp]
        public void Setup()
        {
            this.module = new SearchModule();
            this.jsonService = new JsonService(new JsonDeserializer(), new JsonSerializer(), new Cdp4JsonSerializer());
            this.searchService = new Mock<ISearchService>();
            this.resolverService = new Mock<IResolverService>();
            this.roleManager = new Mock<IRoleManager>();
            this.userManager = new Mock<IUserManager>();

            ModuleTestHelper.Setup<SearchModule, EntityDto>(null, out this.context,
                out _, out _, out _);

            this.context.Setup(x => x.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()
            {
                new (ClaimTypes.Name, "user")
            })));
        }

        [Test]
        public async Task VerifySearch()
        {
            const string keyword = "pro";
            var stream = new StringContent(string.Empty);
            this.searchService.Setup(x => x.SearchAfter(keyword)).ReturnsAsync(await stream.ReadAsStreamAsync());

            this.userManager.Setup(x => x.FindEntity(It.IsAny<Guid>())).ReturnsAsync(new UserEntity());

            this.roleManager.Setup(x => x.FindEntities(It.IsAny<IEnumerable<Guid>>()))
                .ReturnsAsync(new List<Role>
                {
                    new ()
                });

            await this.module.Search(this.context.Object, keyword, this.searchService.Object, this.jsonService, this.resolverService.Object,
                this.roleManager.Object, this.userManager.Object);

            Assert.Multiple(() =>
            {
                this.searchService.Verify(x => x.IndexData(It.IsAny<UserEntityDto>()), Times.Once);
                this.searchService.Verify(x => x.IndexData(It.IsAny<IEnumerable<RoleDto>>()), Times.Once);
                
                this.resolverService.Verify(x => x.ResolveSearchResult(It.IsAny<List<CommonBaseSearchDto>>(), 
                    It.IsAny<string>()), Times.Never);
            });

            stream = new StringContent("[]");
            this.searchService.Setup(x => x.SearchAfter(keyword)).ReturnsAsync(await stream.ReadAsStreamAsync());

            await this.module.Search(this.context.Object, keyword, this.searchService.Object, this.jsonService, this.resolverService.Object,
                this.roleManager.Object, this.userManager.Object);
            
            Assert.Multiple(() =>
            {
                this.searchService.Verify(x => x.IndexData(It.IsAny<UserEntityDto>()), Times.Exactly(2));
                this.searchService.Verify(x => x.IndexData(It.IsAny<IEnumerable<RoleDto>>()), Times.Exactly(2));

                this.resolverService.Verify(x => x.ResolveSearchResult(It.IsAny<List<CommonBaseSearchDto>>(),
                    It.IsAny<string>()), Times.Once);
            });
        }
    }
}
