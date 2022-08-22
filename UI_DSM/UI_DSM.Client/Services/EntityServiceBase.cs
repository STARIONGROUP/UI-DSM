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
    using System.Text.Json;

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
        /// <returns>A <see cref="Task" /> with the <see cref="TEntityDto" /> if found</returns>
        protected async Task<TEntityDto> GetEntityDto(Guid entityId)
        {
            var getResponse = await this.HttpClient.GetAsync(Path.Combine(this.MainRoute, entityId.ToString()));
            var getContent = await getResponse.Content.ReadAsStringAsync();

            return !getResponse.IsSuccessStatusCode
                ? default
                : JsonSerializer.Deserialize<TEntityDto>(getContent, this.JsonSerializerOptions);
        }

        /// <summary>
        ///     Tries to get an <see cref="TEntity" /> based on its <see cref="Guid" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="TEntity" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="TEntity" /> if found</returns>
        protected async Task<TEntity> GetEntity(Guid entityId)
        {
            var dto = await this.GetEntityDto(entityId);

            if (dto == null)
            {
                return default;
            }

            var poco = (TEntity)dto.InstantiatePoco();
            poco.ResolveProperties(dto);
            return poco;
        }

        /// <summary>
        ///     Gets a collection of all <see cref="TEntityDto" />
        /// </summary>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="TEntityDto" /></returns>
        protected async Task<List<TEntityDto>> GetEntitiesDto()
        {
            var response = await this.HttpClient.GetAsync(this.MainRoute);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(content);
            }

            return JsonSerializer.Deserialize<List<TEntityDto>>(content, this.JsonSerializerOptions);
        }

        /// <summary>
        ///     Gets a collection of all <see cref="TEntity" />
        /// </summary>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="TEntity" /></returns>
        protected async Task<List<TEntity>> GetEntities()
        {
            var dtos = await this.GetEntitiesDto();

            return dtos.Select(x =>
            {
                var poco = (TEntity)x.InstantiatePoco();
                poco.ResolveProperties(x);
                return poco;
            }).ToList();
        }

        /// <summary>
        ///     Creates a <see cref="TEntity" /> and gets the <see cref="EntityRequestResponseDto{TEntityDto}" /> response
        /// </summary>
        /// <param name="entity">The <see cref="TEntity" /> to create</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityRequestResponseDto{TEntityDto}" /></returns>
        protected async Task<EntityRequestResponseDto<TEntityDto>> CreateEntityAndGetResponseDto(TEntity entity)
        {
            var content = JsonSerializer.Serialize((TEntityDto)entity.ToDto());
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

            var response = await this.HttpClient.PostAsync(Path.Combine(this.MainRoute, "Create"), bodyContent);
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<EntityRequestResponseDto<TEntityDto>>(responseContent, this.JsonSerializerOptions);
        }

        /// <summary>
        ///     Creates a <see cref="TEntity" /> and gets the <see cref="EntityRequestResponse{TEntity}" /> response
        /// </summary>
        /// <param name="entity">The <see cref="TEntity" /> to create</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityRequestResponse{TEntity}" /></returns>
        protected async Task<EntityRequestResponse<TEntity>> CreateEntity(TEntity entity)
        {
            var entityRequest = await this.CreateEntityAndGetResponseDto(entity);

            if (!entityRequest!.IsRequestSuccessful)
            {
                return EntityRequestResponse<TEntity>.Fail(entityRequest.Errors);
            }

            var poco = (TEntity)entityRequest.Entity.InstantiatePoco();
            poco.ResolveProperties(entityRequest.Entity);

            return EntityRequestResponse<TEntity>.Success(poco);
        }

        /// <summary>
        ///     Updates a <see cref="TEntity" /> and gets the <see cref="EntityRequestResponseDto{TEntityDto}" /> response
        /// </summary>
        /// <param name="entity">The <see cref="TEntity" /> to update</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityRequestResponseDto{TEntityDto}" /></returns>
        protected async Task<EntityRequestResponseDto<TEntityDto>> UpdateEntityAndGetResponseDto(TEntity entity)
        {
            var url = Path.Combine(this.MainRoute, entity.Id.ToString());
            var content = JsonSerializer.Serialize((TEntityDto)entity.ToDto());
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

            var response = await this.HttpClient.PutAsync(url, bodyContent);
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<EntityRequestResponseDto<TEntityDto>>(responseContent, this.JsonSerializerOptions);
        }

        /// <summary>
        ///     Updates a <see cref="TEntity" /> and gets the <see cref="EntityRequestResponse{TEntity}" /> response
        /// </summary>
        /// <param name="entity">The <see cref="TEntity" /> to update</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityRequestResponse{TEntity}" /></returns>
        protected async Task<EntityRequestResponse<TEntity>> UpdateEntity(TEntity entity)
        {
            var entityRequest = await this.UpdateEntityAndGetResponseDto(entity);

            if (!entityRequest!.IsRequestSuccessful)
            {
                return EntityRequestResponse<TEntity>.Fail(entityRequest.Errors);
            }

            var poco = (TEntity)entityRequest.Entity.InstantiatePoco();
            poco.ResolveProperties(entityRequest.Entity);

            return EntityRequestResponse<TEntity>.Success(poco);
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

            return JsonSerializer.Deserialize<RequestResponseDto>(deleteContent, this.JsonSerializerOptions);
        }
    }
}
