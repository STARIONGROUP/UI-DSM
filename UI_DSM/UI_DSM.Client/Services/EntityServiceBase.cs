// --------------------------------------------------------------------------------------------------------
// <copyright file="EntityServiceBase.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Client.Services
{
    using System.Text;

    using Microsoft.AspNetCore.WebUtilities;

    using Newtonsoft.Json;

    using UI_DSM.Shared.Assembler;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.Types;

    /// <summary>
    ///     <see cref="ServiceBase" /> that are used to manage <see cref="TEntity" /> and <see cref="TEntityDto" /> objects
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TEntityDto"></typeparam>
    public abstract class EntityServiceBase<TEntity, TEntityDto> : ServiceBase where TEntity : Entity where TEntityDto : EntityDto
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EntityServiceBase{TEntity, TEntityDto}" /> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="ServiceBase.HttpClient" /></param>
        protected EntityServiceBase(HttpClient httpClient) : base(httpClient)
        {
        }

        /// <summary>
        ///     Tries to get an <see cref="TEntityDto" /> based on its <see cref="Guid" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="EntityDto" /></param>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> with the collection of <see cref="EntityDto" /> if found</returns>
        protected async Task<IEnumerable<EntityDto>> GetEntityDto(Guid entityId, int deepLevel)
        {
            var getResponse = await this.HttpClient.GetAsync(this.CreateUri(Path.Combine(this.MainRoute, entityId.ToString()), deepLevel));
            var getContent = await getResponse.Content.ReadAsStringAsync();

            return !getResponse.IsSuccessStatusCode
                ? default
                : JsonConvert.DeserializeObject<IEnumerable<EntityDto>>(getContent, this.JsonSerializerOptions);
        }

        /// <summary>
        ///     Tries to get an <see cref="TEntity" /> based on its <see cref="Guid" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="TEntity" /></param>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> with the <see cref="TEntity" /> if found</returns>
        protected async Task<TEntity> GetEntity(Guid entityId, int deepLevel)
        {
            var dtos = await this.GetEntityDto(entityId, deepLevel);

            if (dtos == null)
            {
                return default;
            }

            var entities = Assembler.CreateEntities<TEntity>(dtos);

            return entities.FirstOrDefault();
        }

        /// <summary>
        ///     Gets a collection of all <see cref="EntityDto" />
        /// </summary>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="EntityDto" /></returns>
        protected async Task<List<EntityDto>> GetEntitiesDto(int deepLevel)
        {
            var response = await this.HttpClient.GetAsync(this.CreateUri(this.MainRoute, deepLevel));
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(content);
            }

            return JsonConvert.DeserializeObject<IEnumerable<EntityDto>>(content, this.JsonSerializerOptions).ToList();
        }

        /// <summary>
        ///     Gets a collection of all <see cref="TEntity" />
        /// </summary>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="TEntity" /></returns>
        protected async Task<List<TEntity>> GetEntities(int deepLevel)
        {
            var dtos = await this.GetEntitiesDto(deepLevel);

            return Assembler.CreateEntities<TEntity>(dtos).ToList();
        }

        /// <summary>
        ///     Creates a <see cref="TEntity" /> and gets the <see cref="EntityRequestResponseDto" /> response
        /// </summary>
        /// <param name="entity">The <see cref="TEntity" /> to create</param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityRequestResponseDto" /></returns>
        protected async Task<EntityRequestResponseDto> CreateEntityAndGetResponseDto(TEntity entity, int deepLevel)
        {
            var content = JsonConvert.SerializeObject((TEntityDto)entity.ToDto(), this.JsonSerializerOptions);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var response = await this.HttpClient.PostAsync(this.CreateUri(Path.Combine(this.MainRoute, "Create"), deepLevel), bodyContent);
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<EntityRequestResponseDto>(responseContent, this.JsonSerializerOptions);
        }

        /// <summary>
        ///     Creates a <see cref="TEntity" /> and gets the <see cref="EntityRequestResponse{TEntity}" /> response
        /// </summary>
        /// <param name="entity">The <see cref="TEntity" /> to create</param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityRequestResponse{TEntity}" /></returns>
        protected async Task<EntityRequestResponse<TEntity>> CreateEntity(TEntity entity, int deepLevel)
        {
            var entityRequest = await this.CreateEntityAndGetResponseDto(entity, deepLevel);

            if (!entityRequest!.IsRequestSuccessful)
            {
                return EntityRequestResponse<TEntity>.Fail(entityRequest.Errors);
            }

            var poco = Assembler.CreateEntities<TEntity>(entityRequest.Entities).FirstOrDefault();

            return poco == null
                ? EntityRequestResponse<TEntity>.Fail(new List<string> { "Error during the creation of the entity" })
                : EntityRequestResponse<TEntity>.Success(poco);
        }

        /// <summary>
        ///     Updates a <see cref="TEntity" /> and gets the <see cref="EntityRequestResponseDto" /> response
        /// </summary>
        /// <param name="entity">The <see cref="TEntity" /> to update</param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityRequestResponseDto" /></returns>
        protected async Task<EntityRequestResponseDto> UpdateEntityAndGetResponseDto(TEntity entity, int deepLevel)
        {
            var content = JsonConvert.SerializeObject((TEntityDto)entity.ToDto(), this.JsonSerializerOptions);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

            var response = await this.HttpClient.PutAsync(this.CreateUri(Path.Combine(this.MainRoute, entity.Id.ToString()), deepLevel), bodyContent);
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<EntityRequestResponseDto>(responseContent, this.JsonSerializerOptions);
        }

        /// <summary>
        ///     Updates a <see cref="TEntity" /> and gets the <see cref="EntityRequestResponse{TEntity}" /> response
        /// </summary>
        /// <param name="entity">The <see cref="TEntity" /> to update</param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityRequestResponse{TEntity}" /></returns>
        protected async Task<EntityRequestResponse<TEntity>> UpdateEntity(TEntity entity, int deepLevel)
        {
            var entityRequest = await this.UpdateEntityAndGetResponseDto(entity, deepLevel);

            if (!entityRequest!.IsRequestSuccessful)
            {
                return EntityRequestResponse<TEntity>.Fail(entityRequest.Errors);
            }

            var poco = Assembler.CreateEntities<TEntity>(entityRequest.Entities).FirstOrDefault();

            return poco == null
                ? EntityRequestResponse<TEntity>.Fail(new List<string> { "Error during the creation of the entity" })
                : EntityRequestResponse<TEntity>.Success(poco);
        }

        /// <summary>
        ///     Deletes an <see cref="TEntity" />
        /// </summary>
        /// <param name="entityToDelete">The <see cref="TEntity" /> to delete</param>
        /// <returns>A <see cref="Task" /> with the <see cref="RequestResponseDto" /></returns>
        protected async Task<RequestResponseDto> DeleteEntity(TEntity entityToDelete)
        {
            var url = Path.Combine(this.MainRoute, entityToDelete.Id.ToString());
            var deleteResponse = await this.HttpClient.DeleteAsync(url);
            var deleteContent = await deleteResponse.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<RequestResponseDto>(deleteContent, this.JsonSerializerOptions);

            if (response == null)
            {
                throw new HttpRequestException("Error during communication with the server");
            }

            return response;
        }

        /// <summary>
        ///     Verify that the <see cref="Entity" /> has a <see cref="Entity.EntityContainer" /> with a not empty
        ///     <see cref="Guid" />
        /// </summary>
        /// <typeparam name="TEntityContainer">A <see cref="Entity" /> for the type of the container</typeparam>
        /// <param name="entity">The <see cref="Entity" /> to verify</param>
        protected void VerifyEntityAndContainer<TEntityContainer>(Entity entity) where TEntityContainer : Entity
        {
            if (entity.EntityContainer is not TEntityContainer container || container.Id == Guid.Empty)
            {
                throw new ArgumentException($"Invalid container for the entity with id {entity.Id}");
            }
        }

        /// <summary>
        ///     Computes the uri with the <see cref="deepLevel" />
        /// </summary>
        /// <param name="baseUri">The base uri</param>
        /// <param name="deepLevel">The deep level</param>
        /// <returns>The computed uri</returns>
        private string CreateUri(string baseUri, int deepLevel)
        {
            return deepLevel > 0 ? QueryHelpers.AddQueryString(baseUri, "deepLevel", deepLevel.ToString()) : baseUri;
        }
    }
}
