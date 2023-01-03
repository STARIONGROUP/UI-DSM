// --------------------------------------------------------------------------------------------------------
// <copyright file="RouteParserService.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Services.RouteParserService
{
    using System.Text;

    using UI_DSM.Server.Managers.ModelManager;
    using UI_DSM.Server.Managers.ProjectManager;
    using UI_DSM.Server.Managers.ReviewManager;
    using UI_DSM.Server.Managers.ReviewObjectiveManager;
    using UI_DSM.Server.Managers.ReviewTaskManager;
    using UI_DSM.Server.Managers.RoleManager;
    using UI_DSM.Server.Managers.UserManager;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This service provides capability to parse a url for breadcrumb use
    /// </summary>
    public class RouteParserService : IRouteParserService
    {
        /// <summary>
        ///     The prefix for any administration page
        /// </summary>
        private const string AdministrationPrefix = "/Administration/";

        /// <summary>
        ///     The Postfix for an entry page for administration
        /// </summary>
        private const string AdministrationPostfix = "Management";

        /// <summary>
        ///     The prefix for any normal page
        /// </summary>
        private const string PagePrefix = "/Project/";

        /// <summary>
        ///     The <see cref="ParsedUrlDto" /> for the index page
        /// </summary>
        private readonly ParsedUrlDto index = new("Home", "/");

        /// <summary>
        ///     The <see cref="IModelManager" />
        /// </summary>
        private readonly IModelManager modelManager;

        /// <summary>
        ///     The <see cref="IProjectManager" />
        /// </summary>
        private readonly IProjectManager projectManager;

        /// <summary>
        ///     The <see cref="IReviewManager" />
        /// </summary>
        private readonly IReviewManager reviewManager;

        /// <summary>
        ///     The <see cref="IReviewObjectiveManager" />
        /// </summary>
        private readonly IReviewObjectiveManager reviewObjectiveManager;

        /// <summary>
        ///     The <see cref="IReviewTaskManager" />
        /// </summary>
        private readonly IReviewTaskManager reviewTaskManager;

        /// <summary>
        ///     The <see cref="IRoleManager" />
        /// </summary>
        private readonly IRoleManager roleManager;

        /// <summary>
        ///     The <see cref="IUserManager" />
        /// </summary>
        private readonly IUserManager userManager;

        /// <summary>
        ///     Initializes a new instance of <see cref="RouteParserService" />
        /// </summary>
        /// <param name="projectManager">The <see cref="IProjectManager" /></param>
        /// <param name="reviewManager">The <see cref="IReviewManager" /></param>
        /// <param name="reviewObjectiveManager">The <see cref="IReviewObjectiveManager" /></param>
        /// <param name="reviewTaskManager">The <see cref="IReviewTaskManager" /></param>
        /// <param name="userManager">The <see cref="IUserManager" /></param>
        /// <param name="roleManager">The <see cref="IRoleManager" /></param>
        /// <param name="modelManager">The <see cref="IModelManager" /></param>
        public RouteParserService(IProjectManager projectManager, IReviewManager reviewManager, IReviewObjectiveManager reviewObjectiveManager,
            IReviewTaskManager reviewTaskManager, IUserManager userManager, IRoleManager roleManager, IModelManager modelManager)
        {
            this.projectManager = projectManager;
            this.reviewManager = reviewManager;
            this.reviewObjectiveManager = reviewObjectiveManager;
            this.reviewTaskManager = reviewTaskManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.modelManager = modelManager;
        }

        /// <summary>
        ///     Parses the url to generate a collection of <see cref="ParsedUrlDto" />
        /// </summary>
        /// <param name="url">The <see cref="url" /> to parse</param>
        /// <returns>A <see cref="Task" /> with the collection of <see cref="ParsedUrlDto" /></returns>
        public async Task<List<ParsedUrlDto>> ParseUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return new List<ParsedUrlDto> { this.index };
            }

            var splittedUrl = url.Split('?')[0].Split('/').Where(x => !string.IsNullOrEmpty(x)).ToList();

            if (url.StartsWith(AdministrationPrefix, StringComparison.InvariantCultureIgnoreCase))
            {
                return await this.ParseAdministrationUrl(splittedUrl);
            }

            if (url.StartsWith(PagePrefix, StringComparison.InvariantCultureIgnoreCase))
            {
                return await this.ParsePageUrl(splittedUrl);
            }

            return new List<ParsedUrlDto> { this.index };
        }

        /// <summary>
        ///     Parse an url for a page
        /// </summary>
        /// <param name="splittedUrl">A <see cref="IReadOnlyList{T}" /> with all part of the url</param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="ParsedUrlDto" /></returns>
        private async Task<List<ParsedUrlDto>> ParsePageUrl(IReadOnlyList<string> splittedUrl)
        {
            if (splittedUrl.Count % 2 != 0 || splittedUrl.Count > 8)
            {
                return new List<ParsedUrlDto> { this.index };
            }

            var parsedUrlDtos = new List<ParsedUrlDto>
            {
                this.index
            };

            var currentUrl = new StringBuilder();

            for (var urlCount = 0; urlCount < splittedUrl.Count; urlCount += 2)
            {
                var entityTypeName = splittedUrl[urlCount];
                var entityId = splittedUrl[urlCount + 1];
                var displayName = await this.TryGetDisplayName(entityTypeName, entityId);

                if (!displayName.Success)
                {
                    return new List<ParsedUrlDto> { this.index };
                }

                currentUrl.Append($"/{entityTypeName}/{entityId}");
                parsedUrlDtos.Add(new ParsedUrlDto(displayName.Result, currentUrl.ToString()));
            }

            return parsedUrlDtos;
        }

        /// <summary>
        ///     Parse an url for an administration page
        /// </summary>
        /// <param name="splittedUrl">A <see cref="IReadOnlyList{T}" /> with all part of the url</param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="ParsedUrlDto" /></returns>
        private async Task<List<ParsedUrlDto>> ParseAdministrationUrl(IReadOnlyList<string> splittedUrl)
        {
            var parsedUrlDtos = new List<ParsedUrlDto> { this.index };
            var managementIndex = splittedUrl[1].IndexOf(AdministrationPostfix, StringComparison.InvariantCultureIgnoreCase);
            var pageName = managementIndex == -1 ? splittedUrl[1] : splittedUrl[1].Remove(managementIndex, splittedUrl[1].Length - managementIndex);
            parsedUrlDtos.Add(new ParsedUrlDto($"{pageName} Management", $"{AdministrationPrefix}{pageName}{AdministrationPostfix}"));

            if (!string.Equals(pageName, nameof(Project), StringComparison.InvariantCultureIgnoreCase) &&
                !string.Equals(pageName, nameof(User), StringComparison.InvariantCultureIgnoreCase) &&
                !string.Equals(pageName, nameof(Role), StringComparison.InvariantCultureIgnoreCase))
            {
                return new List<ParsedUrlDto> { this.index };
            }

            if (splittedUrl.Count != 3)
            {
                return parsedUrlDtos;
            }

            var displayName = await this.TryGetDisplayName(pageName, splittedUrl[2]);

            if (displayName.Success)
            {
                parsedUrlDtos.Add(new ParsedUrlDto(displayName.Result, $"{AdministrationPrefix}{pageName}/{splittedUrl[2]}"));
            }
            else
            {
                return new List<ParsedUrlDto> { this.index };
            }

            return parsedUrlDtos;
        }

        /// <summary>
        ///     Tries to get the display name of an <see cref="Entity" /> based on a typeName and an id
        /// </summary>
        /// <param name="typeName">The type name</param>
        /// <param name="id">The id</param>
        /// <returns>A <see cref="Task" /> with the success status and the result</returns>
        private async Task<(bool Success, string Result)> TryGetDisplayName(string typeName, string id)
        {
            if (!Guid.TryParse(id, out var guid))
            {
                return (false, null);
            }

            var displayName = await this.GetEntityName(typeName, guid);
            return string.IsNullOrEmpty(displayName) ? (false, null) : (true, displayName);
        }

        /// <summary>
        ///     Gets the name of an <see cref="Entity" /> based on its type and its <see cref="Guid" />
        /// </summary>
        /// <param name="typeName">The name of the type</param>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="Entity" /></param>
        /// <returns>A <see cref="Task" /> with the name of the <see cref="Entity" /> if found</returns>
        private async Task<string> GetEntityName(string typeName, Guid entityId)
        {
            if (string.Equals(typeName, nameof(Project), StringComparison.InvariantCultureIgnoreCase))
            {
                var project = await this.projectManager.FindEntity(entityId);
                return project?.ProjectName;
            }

            if (string.Equals(typeName, nameof(User), StringComparison.InvariantCultureIgnoreCase))
            {
                var user = await this.userManager.FindEntity(entityId);
                return user?.UserName;
            }

            if (string.Equals(typeName, nameof(Role), StringComparison.InvariantCultureIgnoreCase))
            {
                var role = await this.roleManager.FindEntity(entityId);
                return role?.RoleName;
            }

            if (string.Equals(typeName, nameof(Review), StringComparison.InvariantCultureIgnoreCase))
            {
                var review = await this.reviewManager.FindEntity(entityId);
                return review?.Title;
            }

            if (string.Equals(typeName, nameof(ReviewObjective), StringComparison.InvariantCultureIgnoreCase))
            {
                var reviewObjective = await this.reviewObjectiveManager.FindEntity(entityId);
                return reviewObjective?.Title;
            }

            if (string.Equals(typeName, nameof(ReviewTask), StringComparison.InvariantCultureIgnoreCase))
            {
                var reviewTask = await this.reviewTaskManager.FindEntity(entityId);
                return reviewTask?.Description;
            }

            if (string.Equals(typeName, nameof(Model), StringComparison.InvariantCultureIgnoreCase))
            {
                var model = await this.modelManager.FindEntity(entityId);
                return model?.ModelName;
            }

            return string.Empty;
        }
    }
}
