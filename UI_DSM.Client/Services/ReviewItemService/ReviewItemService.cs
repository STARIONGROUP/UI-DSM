// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewItemService.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Services.ReviewItemService
{
    using System.Text;

    using CDP4Common.CommonData;

    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Shared.Assembler;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.Types;

    /// <summary>
    ///     The <see cref="ReviewItemService" /> provide capability to manage <see cref="ReviewItem" />
    /// </summary>
    [Route("Project/{0}/Review/{1}/ReviewItem")]
    public class ReviewItemService : EntityServiceBase<ReviewItem, ReviewItemDto>, IReviewItemService
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ReviewItemService" /> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="ServiceBase.HttpClient" /></param>
        /// <param name="jsonService">The <see cref="IJsonService" /></param>
        public ReviewItemService(HttpClient httpClient, IJsonService jsonService) : base(httpClient, jsonService)
        {
        }

        /// <summary>
        ///     Gets all <see cref="ReviewItem" />s contained inside a <see cref="Review" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /> of the <see cref="Review" /></param>
        /// <param name="reviewId">The <see cref="Entity.Id" /> of the <see cref="Review" /></param>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> with the collection of <see cref="ReviewItem" /></returns>
        public async Task<List<ReviewItem>> GetReviewItemsOfReview(Guid projectId, Guid reviewId, int deepLevel = 0)
        {
            try
            {
                this.ComputeMainRoute(projectId, reviewId);
                return await this.GetEntities(deepLevel);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Gets a <see cref="ReviewItem" /> contained inside a <see cref="Review" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="reviewId">The <see cref="Guid" /> of the <see cref="Review" /></param>
        /// <param name="reviewItemId">The <see cref="Guid" /> of the <see cref="ReviewItem" /></param>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> with the collection of <see cref="ReviewItem" /></returns>
        public async Task<ReviewItem> GetReviewItemOfReview(Guid projectId, Guid reviewId, Guid reviewItemId, int deepLevel = 0)
        {
            try
            {
                this.ComputeMainRoute(projectId, reviewId);
                return await this.GetEntity(reviewItemId, deepLevel);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Creates a new <see cref="ReviewItem" /> inside a <see cref="Review" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="reviewId">The <see cref="Guid" /> of the <see cref="Review" /></param>
        /// <param name="thingId">The <see cref="Guid" /> of the linked <see cref="Thing" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityRequestResponse{ReviewItem}" /></returns>
        public async Task<EntityRequestResponse<ReviewItem>> CreateReviewItem(Guid projectId, Guid reviewId, Guid thingId)
        {
            try
            {
                var reviewItem = new ReviewItem
                {
                    ThingId = thingId
                };

                this.ComputeMainRoute(projectId, reviewId);
                return await this.CreateEntity(reviewItem, 0);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Gets a <see cref="ReviewItem" /> that is linked to a <see cref="Thing" />
        /// </summary>
        /// <param name="projectId">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <param name="reviewId">The <see cref="Guid" /> of the <see cref="Review" /></param>
        /// <param name="thingId">The <see cref="Guid" /> of the <see cref="Thing" /></param>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> with the retrieved <see cref="ReviewItem" /></returns>
        public async Task<ReviewItem> GetReviewItemForThing(Guid projectId, Guid reviewId, Guid thingId, int deepLevel = 0)
        {
            try
            {
                this.ComputeMainRoute(projectId, reviewId);
                var getResponse = await this.HttpClient.GetAsync(this.CreateUri(Path.Combine(this.MainRoute, "ForThing", thingId.ToString()), deepLevel));

                var entities = await this.GetEntitiesFromRequest(getResponse);
                return entities.FirstOrDefault();
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Gets a collection of <see cref="ReviewItem" />s that are linked to a <see cref="Thing" />
        /// </summary>
        /// <param name="projectId">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <param name="reviewId">The <see cref="Guid" /> of the <see cref="Review" /></param>
        /// <param name="thingIds">A collection of <see cref="Guid" />s of the <see cref="Thing" /></param>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> with the retrieved collection of <see cref="ReviewItem" /></returns>
        public async Task<List<ReviewItem>> GetReviewItemsForThings(Guid projectId, Guid reviewId, IEnumerable<Guid> thingIds, int deepLevel = 0)
        {
            try
            {
                this.ComputeMainRoute(projectId, reviewId);
                var stringContent = new StringContent(this.jsonService.Serialize(thingIds), Encoding.UTF8, "application/json");

                var response = await this.HttpClient.PostAsync(this.CreateUri($"{this.MainRoute}/ForThings", deepLevel), stringContent);
                return (await this.GetEntitiesFromRequest(response)).ToList();
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Updates a <see cref="ReviewItem" />
        /// </summary>
        /// <param name="projectId">The <see cref="Project" /> id</param>
        /// <param name="reviewId">The <see cref="Review" /> id</param>
        /// <param name="reviewItem">The <see cref="ReviewItem" /> to update</param>
        /// <returns>A <see cref="Task" /> with the updated <see cref="ReviewItem" /></returns>
        public Task<EntityRequestResponse<ReviewItem>> UpdateReviewItem(Guid projectId, Guid reviewId, ReviewItem reviewItem)
        {
            try
            {
                this.ComputeMainRoute(projectId, reviewId);
                return this.UpdateEntity(reviewItem, 0);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Gets a collection of <see cref="ReviewItem" /> that is contained into a <see cref="HttpRequestMessage" />
        /// </summary>
        /// <param name="response">The <see cref="HttpResponseMessage" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="ReviewItem" /></returns>
        private async Task<IEnumerable<ReviewItem>> GetEntitiesFromRequest(HttpResponseMessage response)
        {
            var dtos = !response.IsSuccessStatusCode
                ? default
                : this.jsonService.Deserialize<IEnumerable<EntityDto>>(await response.Content.ReadAsStreamAsync());

            return dtos == null ? Enumerable.Empty<ReviewItem>() : Assembler.CreateEntities<ReviewItem>(dtos);
        }
    }
}
