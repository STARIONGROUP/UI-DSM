// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewService.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Client.Services.ReviewService
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.Types;

    /// <summary>
    ///     The <see cref="ReviewService" /> provide capability to manage <see cref="Review" />s inside a
    ///     <see cref="Project" />
    /// </summary>
    [Route("Project/{0}/Review")]
    public class ReviewService : EntityServiceBase<Review, ReviewDto>, IReviewService
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ReviewService" /> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="ServiceBase.HttpClient" /></param>
        /// <param name="jsonService">The <see cref="IJsonService" /></param>
        public ReviewService(HttpClient httpClient, IJsonService jsonService) : base(httpClient, jsonService)
        {
        }

        /// <summary>
        ///     Gets all <see cref="Review" />s contained inside a <see cref="Project" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> with the collection of <see cref="Review" /></returns>
        public async Task<List<Review>> GetReviewsOfProject(Guid projectId, int deepLevel = 0)
        {
            try
            {
                this.ComputeMainRoute(projectId);
                return await this.GetEntities(deepLevel);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Gets a <see cref="Review" /> contained inside a <see cref="Project" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="reviewId">The <see cref="Guid" /> of the <see cref="Review" /></param>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> with the collection of <see cref="Review" /></returns>
        public async Task<Review> GetReviewOfProject(Guid projectId, Guid reviewId, int deepLevel = 0)
        {
            try
            {
                this.ComputeMainRoute(projectId);
                return await this.GetEntity(reviewId, deepLevel);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Creates a new <see cref="Review" /> inside a <see cref="Project" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="review">The <see cref="Review" />to create</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityRequestResponse{Review}" /></returns>
        public async Task<EntityRequestResponse<Review>> CreateReview(Guid projectId, Review review)
        {
            try
            {
                this.ComputeMainRoute(projectId);
                return await this.CreateEntity(review, 0);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Updates a <see cref="Review" />
        /// </summary>
        /// <param name="review">The <see cref="Review" />to update</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityRequestResponse{Review}" /></returns>
        public async Task<EntityRequestResponse<Review>> UpdateReview(Review review)
        {
            this.VerifyEntityAndContainer<Project>(review);

            try
            {
                this.ComputeMainRoute(review.EntityContainer.Id);
                return await this.UpdateEntity(review, 0);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Deletes a <see cref="Review" />
        /// </summary>
        /// <param name="review">The <see cref="Review" /> to delete</param>
        /// <returns>A <see cref="Task" /> with the <see cref="RequestResponseDto" /></returns>
        public async Task<RequestResponseDto> DeleteReview(Review review)
        {
            this.VerifyEntityAndContainer<Project>(review);

            try
            {
                this.ComputeMainRoute(review.EntityContainer.Id);
                return await this.DeleteEntity(review);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Gets, for all <see cref="Review" />, the number of open <see cref="ReviewTask" /> and <see cref="Comment" />
        ///     related to the <see cref="Review" />
        /// </summary>
        /// <returns>A <see cref="Task" /> with a <see cref="Dictionary{Guid, ComputedProjectProperties}" /></returns>
        public async Task<Dictionary<Guid, ComputedProjectProperties>> GetOpenTasksAndComments(Guid projectId)
        {
            this.ComputeMainRoute(projectId);
            var response = await this.HttpClient.GetAsync($"{this.MainRoute}/OpenTasksAndComments");

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(response.ReasonPhrase);
            }
            var content = await response.Content.ReadAsStringAsync();
            return this.jsonService.Deserialize<Dictionary<Guid, ComputedProjectProperties>>(await response.Content.ReadAsStreamAsync());
        }
    }
}
